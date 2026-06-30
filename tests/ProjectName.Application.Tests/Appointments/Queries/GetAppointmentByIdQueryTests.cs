using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Appointments.Queries;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Tests.Appointments.Queries;

public sealed class GetAppointmentByIdQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetAppointmentByIdQuery_WhenAppointmentExists_ShouldReturnAppointment()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        var pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);
        var clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);
        var veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);
        var appointment = await ApplicationTestDataBuilder.CreateAppointmentAsync(dbContext, pet.Id, veterinarian.Id, clinic.Id);

        var getResult = await mediator.Send(new GetAppointmentByIdQuery(appointment.Id));

        Assert.True(getResult.IsSuccess);
        Assert.Equal(appointment.Id, getResult.Value.Id);
    }

    [Fact]
    public async Task GetAppointmentByIdQuery_WhenAppointmentDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var getResult = await mediator.Send(new GetAppointmentByIdQuery(Guid.NewGuid()));

        Assert.True(getResult.IsFailed);
        Assert.Contains(getResult.Errors, error => error is NotFoundError);
    }
}

