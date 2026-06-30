using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Owners.Common;
using ProjectName.Application.Owners.Commands;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Owners.Commands;

public sealed class CreateOwnerCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task CreateOwnerCommand_WhenOwnerIsValid_ShouldPersistAndBeQueryableById()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        string uniqueEmail = $"owner-{Guid.NewGuid():N}@example.test";

        Result<GetOwnerResponse> createResult = await mediator.Send(new CreateOwnerCommand("Alice", "Cooper", uniqueEmail, "+1-555-0100"));

        Assert.True(createResult.IsSuccess);
        Owner storedOwner = await dbContext.Owners.SingleAsync(owner => owner.Id == createResult.Value.Id);
        Assert.Equal(uniqueEmail, storedOwner.Email);
    }

    [Fact]
    public async Task CreateOwnerCommand_WhenFirstNameIsEmpty_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        string invalidEmail = $"owner-{Guid.NewGuid():N}@example.test";

        Result<GetOwnerResponse> createResult = await mediator.Send(new CreateOwnerCommand(string.Empty, "Cooper", invalidEmail, "+1-555-0100"));

        Assert.True(createResult.IsFailed);
        Assert.Contains(createResult.Errors, error => error is ValidationError);
        Assert.False(await dbContext.Owners.AnyAsync(owner => owner.Email == invalidEmail));
    }
}
