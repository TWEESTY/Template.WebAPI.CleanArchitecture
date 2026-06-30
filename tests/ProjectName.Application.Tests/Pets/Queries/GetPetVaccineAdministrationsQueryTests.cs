using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Pets.Commands;
using ProjectName.Application.Pets.Queries;

namespace ProjectName.Application.Tests.Pets.Queries;

public sealed class GetPetVaccineAdministrationsQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetPetVaccineAdministrationsQuery_WhenPetExists_ShouldReturnAdministrations()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        var pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);
        var vaccine = await ApplicationTestDataBuilder.CreateVaccineAsync(dbContext);
        var veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);
        FakeTimeProvider fakeTimeProvider = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        await mediator.Send(new AddVaccineAdministrationCommand(
            pet.Id,
            vaccine.Id,
            veterinarian.Id,
            DateOnly.FromDateTime(fakeTimeProvider.GetUtcNow().UtcDateTime.Date)));

        var getResult = await mediator.Send(new GetPetVaccineAdministrationsQuery(pet.Id));

        Assert.True(getResult.IsSuccess);
        Assert.Single(getResult.Value);
    }

    [Fact]
    public async Task GetPetVaccineAdministrationsQuery_WhenPetDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var getResult = await mediator.Send(new GetPetVaccineAdministrationsQuery(Guid.NewGuid()));

        Assert.True(getResult.IsFailed);
        Assert.Contains(getResult.Errors, error => error is NotFoundError);
    }
}

