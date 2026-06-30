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

public sealed class CreateVaccineCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task CreateVaccineCommand_WhenVaccineIsValid_ShouldCreateVaccine()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Result<GetVaccineResponse> createResult = await mediator.Send(new CreateVaccineCommand("RAB", "Rabies"));

        Assert.True(createResult.IsSuccess);
        Assert.Equal("RAB", createResult.Value.Code);
        Vaccine storedVaccine = await dbContext.Vaccines.SingleAsync(vaccine => vaccine.Id == createResult.Value.Id);
        Assert.Equal("Rabies", storedVaccine.Name);
    }

    [Fact]
    public async Task CreateVaccineCommand_WhenCodeIsEmpty_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        string invalidCode = string.Empty;

        Result<GetVaccineResponse> createResult = await mediator.Send(new CreateVaccineCommand(invalidCode, "Rabies"));

        Assert.True(createResult.IsFailed);
        Assert.Contains(createResult.Errors, error => error is ValidationError);
        Assert.False(await dbContext.Vaccines.AnyAsync(vaccine => vaccine.Code == invalidCode));
    }

    [Fact]
    public async Task CreateVaccineCommand_WhenNameIsEmpty_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Result<GetVaccineResponse> createResult = await mediator.Send(new CreateVaccineCommand("RAB", string.Empty));

        Assert.True(createResult.IsFailed);
        Assert.Contains(createResult.Errors, error => error is ValidationError validationError && validationError.Identifier == nameof(CreateVaccineCommand.Name));
        Assert.False(await dbContext.Vaccines.AnyAsync(vaccine => vaccine.Code == "RAB" && vaccine.Name == string.Empty));
    }
}
