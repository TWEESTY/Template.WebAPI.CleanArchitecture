using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Commands;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Pets.Commands;

public sealed class AddVaccineAdministrationCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task AddVaccineAdministrationCommand_WhenPetExists_ShouldPersistVaccineAdministrationAndReturnUnexpectedError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        Pet pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);
        Vaccine vaccine = await ApplicationTestDataBuilder.CreateVaccineAsync(dbContext);
        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);

        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        DateOnly administrationOn = DateOnly.FromDateTime(fakeTimeProvider.GetUtcNow().UtcDateTime.Date);

        Result addResult = await mediator.Send(new AddVaccineAdministrationCommand(pet.Id, vaccine.Id, veterinarian.Id, administrationOn));

        Assert.True(addResult.IsFailed);
        Assert.Contains(addResult.Errors, error => error is UnexpectedError);
        Pet storedPet = await dbContext.Pets.Include(item => item.VaccineAdministrations).SingleAsync(item => item.Id == pet.Id);
        Assert.Single(storedPet.VaccineAdministrations);
        VaccineAdministration administration = storedPet.VaccineAdministrations.Single();
        Assert.Equal(vaccine.Id, administration.VaccineId);
        Assert.Equal(veterinarian.Id, administration.VeterinarianId);
        Assert.Equal(administrationOn, administration.AdministrationOn);
    }

    [Fact]
    public async Task AddVaccineAdministrationCommand_WhenPetDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Vaccine vaccine = await ApplicationTestDataBuilder.CreateVaccineAsync(dbContext);
        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);
        Guid missingPetId = Guid.NewGuid();
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Result addResult = await mediator.Send(new AddVaccineAdministrationCommand(
            missingPetId,
            vaccine.Id,
            veterinarian.Id,
            DateOnly.FromDateTime(fakeTimeProvider.GetUtcNow().UtcDateTime.Date)));

        Assert.True(addResult.IsFailed);
        Assert.Contains(addResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Pets.AnyAsync(item => item.Id == missingPetId));
    }
}

