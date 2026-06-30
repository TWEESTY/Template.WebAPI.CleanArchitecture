using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Common;
using ProjectName.Application.Pets.Commands;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Pets.Commands;

public sealed class TransferPetOwnershipCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task TransferPetOwnershipCommand_WhenPetExists_ShouldSucceed()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner originalOwner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        Owner newOwner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        Pet pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, originalOwner.Id);

        Result<GetPetResponse> transferResult = await mediator.Send(new TransferPetOwnershipCommand(pet.Id, newOwner.Id));

        Assert.True(transferResult.IsSuccess);
        Assert.Equal(pet.Id, transferResult.Value.Id);
        Pet storedPet = await dbContext.Pets.SingleAsync(item => item.Id == pet.Id);
        Assert.Equal(newOwner.Id, storedPet.OwnerId);
    }

    [Fact]
    public async Task TransferPetOwnershipCommand_WhenPetDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);

        Result<GetPetResponse> transferResult = await mediator.Send(new TransferPetOwnershipCommand(Guid.NewGuid(), owner.Id));

        Assert.True(transferResult.IsFailed);
        Assert.Contains(transferResult.Errors, error => error is NotFoundError);
        Assert.True(await dbContext.Owners.AnyAsync(item => item.Id == owner.Id));
    }
}

