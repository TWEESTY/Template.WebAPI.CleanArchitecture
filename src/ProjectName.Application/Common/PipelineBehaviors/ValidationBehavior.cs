using FluentResults;
using FluentValidation;
using Mediator;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Common.PipelineBehaviors;

/// <summary>
/// Validates incoming mediator messages using registered FluentValidation validators.
/// </summary>
/// <typeparam name="TMessage">The type of the message being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
internal sealed class ValidationBehavior<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : ResultBase, new()
{
    private readonly IEnumerable<IValidator<TMessage>> _validators = validators;

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(message, cancellationToken);
        }

        ValidationContext<TMessage> context = new(message);

        FluentValidation.Results.ValidationFailure[] failures = [
            .. (await Task.WhenAll(
                _validators.Select(x => x.ValidateAsync(context, cancellationToken))))
                .SelectMany(result => result.Errors)
                .Where(f => f is not null)
        ];

        if (failures.Length == 0)
        {
            return await next(message, cancellationToken);
        }

        TResponse response = new();

        foreach (FluentValidation.Results.ValidationFailure failure in failures)
        {
            response.Reasons.Add(new ValidationError(failure.PropertyName, failure.ErrorMessage));
        }

        return response;
    }
}
