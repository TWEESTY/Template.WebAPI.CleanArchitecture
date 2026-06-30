using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Clinics.Commands;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Clinics.Commands;

public sealed class CreateClinicCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task CreateClinicCommand_WhenClinicIsValid_ShouldCreateClinic()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Result<GetClinicResponse> createResult = await mediator.Send(new CreateClinicCommand("North Clinic", "200 Main Street"));

        Assert.True(createResult.IsSuccess);
        Assert.Equal("North Clinic", createResult.Value.Name);
        Clinic storedClinic = await dbContext.Clinics.SingleAsync(clinic => clinic.Id == createResult.Value.Id);
        Assert.Equal("200 Main Street", storedClinic.Address);
    }

    [Fact]
    public async Task CreateClinicCommand_WhenNameIsEmpty_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        string uniqueAddress = $"200 Main Street {Guid.NewGuid():N}";

        Result<GetClinicResponse> createResult = await mediator.Send(new CreateClinicCommand("   ", uniqueAddress));

        Assert.True(createResult.IsFailed);
        Assert.Contains(createResult.Errors, error => error is ValidationError);
        Assert.False(await dbContext.Clinics.AnyAsync(clinic => clinic.Address == uniqueAddress));
    }
}
