using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Commands;
using ProjectName.Application.Veterinarians.Queries;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Veterinarians.Commands;

public sealed class DeleteVeterinarianCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task DeleteVeterinarianCommand_WhenVeterinarianExists_ShouldDeleteVeterinarian()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);

        Result deleteResult = await mediator.Send(new DeleteVeterinarianCommand(veterinarian.Id));
        var getResult = await mediator.Send(new GetVeterinarianByIdQuery(veterinarian.Id));

        Assert.True(deleteResult.IsSuccess);
        Assert.True(getResult.IsFailed);
        Assert.Contains(getResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Veterinarians.AnyAsync(item => item.Id == veterinarian.Id));
    }

    [Fact]
    public async Task DeleteVeterinarianCommand_WhenVeterinarianDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Result deleteResult = await mediator.Send(new DeleteVeterinarianCommand(Guid.NewGuid()));

        Assert.True(deleteResult.IsFailed);
        Assert.Contains(deleteResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Veterinarians.AnyAsync());
    }
}

