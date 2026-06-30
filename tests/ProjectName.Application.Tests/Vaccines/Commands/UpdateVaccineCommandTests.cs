using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Application.Vaccines.Commands;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Vaccines.Commands;

public sealed class UpdateVaccineCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task UpdateVaccineCommand_WhenVaccineExists_ShouldUpdateVaccine()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Vaccine vaccine = await ApplicationTestDataBuilder.CreateVaccineAsync(dbContext);
        dbContext.ChangeTracker.Clear();

        Result<GetVaccineResponse> updateResult = await mediator.Send(new UpdateVaccineCommand(vaccine.Id, "RAB2", "Rabies Booster"));

        Assert.True(updateResult.IsSuccess);
        Assert.Equal("RAB2", updateResult.Value.Code);
        Vaccine storedVaccine = await dbContext.Vaccines.SingleAsync(item => item.Id == vaccine.Id);
        Assert.Equal("Rabies Booster", storedVaccine.Name);
    }

    [Fact]
    public async Task UpdateVaccineCommand_WhenVaccineDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Guid missingVaccineId = Guid.NewGuid();

        Result<GetVaccineResponse> updateResult = await mediator.Send(new UpdateVaccineCommand(missingVaccineId, "RAB2", "Rabies Booster"));

        Assert.True(updateResult.IsFailed);
        Assert.Contains(updateResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Vaccines.AnyAsync(item => item.Id == missingVaccineId));
    }

    [Fact]
    public async Task UpdateVaccineCommand_WhenIdIsEmpty_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Result<GetVaccineResponse> updateResult = await mediator.Send(new UpdateVaccineCommand(Guid.Empty, "RAB2", "Rabies Booster"));

        Assert.True(updateResult.IsFailed);
        Assert.Contains(updateResult.Errors, error => error is ValidationError validationError && validationError.Identifier == nameof(UpdateVaccineCommand.Id));
    }
}

