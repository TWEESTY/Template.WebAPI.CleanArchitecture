using FluentResults;
using Mediator;
using ProjectName.Application.Common.Persistence;

namespace ProjectName.Application.Common.PipelineBehaviors;

/// <summary>
/// Represents a behavior that manages the unit of work for a given request.
/// It ensures that a unit of work is started before the request is handled and completed after the request is handled.
/// </summary>
/// <typeparam name="TMessage">The type of the message being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
/// <param name="unitOfWorkManager">The unit of work manager used to manage the unit of work.</param>
internal sealed class UnitOfWorkBehavior<TMessage, TResponse>(IUnitOfWorkManager unitOfWorkManager) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : class, ICommand<TResponse>
    where TResponse : ResultBase, new()
{
    private static readonly bool _useUnitOfWork = typeof(TMessage)
        .GetCustomAttributes(typeof(UseUnitOfWorkAttribute), false)
        .Length > 0;

    private readonly IUnitOfWorkManager _unitOfWorkManager = unitOfWorkManager;

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_useUnitOfWork)
        {
            return await next(message, cancellationToken);
        }

        await using IUnitOfWork unitOfWork = await _unitOfWorkManager.StartOneUnitOfWorkAsync();

        try
        {
            TResponse response = await next(message, cancellationToken);
            await _unitOfWorkManager.EndUnitOfWorkAsync(unitOfWork, forceRollback: response.IsFailed);
            return response;
        }
        catch
        {
            await _unitOfWorkManager.EndUnitOfWorkAsync(unitOfWork, forceRollback: true);
            throw;
        }
    }
}

/// <summary>
/// Indicates that the request should be handled within a unit of work.
/// When this attribute is applied to a request type, the UnitOfWorkBehavior will ensure that a unit of work is started before the request is handled and completed after the request is handled.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class UseUnitOfWorkAttribute : Attribute
{
}