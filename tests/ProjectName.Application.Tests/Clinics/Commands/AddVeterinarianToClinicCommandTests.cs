using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Clinics.Commands;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Clinics.Commands;

public sealed class AddVeterinarianToClinicCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task AddVeterinarianToClinicCommand_WhenClinicExists_ShouldAddVeterinarian()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Clinic clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);
        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);
        dbContext.ChangeTracker.Clear();

        Result addResult = await mediator.Send(new AddVeterinarianToClinicCommand(clinic.Id, veterinarian.Id));

        Assert.True(addResult.IsSuccess);
        Assert.True(await dbContext.ClinicVeterinarians.AnyAsync(item => item.ClinicId == clinic.Id && item.VeterinarianId == veterinarian.Id));
    }

    [Fact]
    public async Task AddVeterinarianToClinicCommand_WhenClinicDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);

        Result addResult = await mediator.Send(new AddVeterinarianToClinicCommand(Guid.NewGuid(), veterinarian.Id));

        Assert.True(addResult.IsFailed);
        Assert.Contains(addResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.ClinicVeterinarians.AnyAsync(item => item.VeterinarianId == veterinarian.Id));
    }
}

