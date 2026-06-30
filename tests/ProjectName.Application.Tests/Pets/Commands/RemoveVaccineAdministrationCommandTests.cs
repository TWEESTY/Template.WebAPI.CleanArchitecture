using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Commands;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Pets.Commands;

public sealed class RemoveVaccineAdministrationCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task RemoveVaccineAdministrationCommand_WhenAdministrationDoesNotExist_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        Pet pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);
        Vaccine vaccine = await ApplicationTestDataBuilder.CreateVaccineAsync(dbContext);
        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);
        dbContext.ChangeTracker.Clear();

        Result removeResult = await mediator.Send(new RemoveVaccineAdministrationCommand(pet.Id, Guid.NewGuid()));

        Assert.True(removeResult.IsFailed);
        Assert.Contains(removeResult.Errors, error => error is ValidationError);
        Pet storedPet = await dbContext.Pets.Include(item => item.VaccineAdministrations).SingleAsync(item => item.Id == pet.Id);
        Assert.Empty(storedPet.VaccineAdministrations);
    }

    [Fact]
    public async Task RemoveVaccineAdministrationCommand_WhenPetDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Result removeResult = await mediator.Send(new RemoveVaccineAdministrationCommand(Guid.NewGuid(), Guid.NewGuid()));

        Assert.True(removeResult.IsFailed);
        Assert.Contains(removeResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Pets.AnyAsync());
    }
}

