using Mediator;
using Microsoft.Extensions.DependencyInjection;
using FluentResults;
using ProjectName.Domain.Entities;
using ProjectName.Application.Appointments.Queries;
using ProjectName.Application.Appointments.Common;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Tests.Search;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Appointments.Queries;

public sealed class GetAppointmentsQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetAppointmentsQuery_WhenAppointmentsExist_ShouldReturnCreatedAppointment()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        Pet pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);
        Clinic clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);
        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);
        Appointment appointment = await ApplicationTestDataBuilder.CreateAppointmentAsync(dbContext, pet.Id, veterinarian.Id, clinic.Id);

        Result<List<GetAppointmentResponse>> getResult = await mediator.Send(new GetAppointmentsQuery(SearchParameters: null));

        Assert.True(getResult.IsSuccess);
        Assert.Contains(getResult.Value, a =>
            a.Id == appointment.Id &&
            a.Reason == appointment.Reason &&
            a.PetId == appointment.PetId &&
            a.VeterinarianId == appointment.VeterinarianId &&
            a.ClinicId == appointment.ClinicId);
    }

    [Fact]
    public async Task GetAppointmentsQuery_WhenSortedByStartAtUtcDescending_ShouldReturnAppointmentsInDescendingOrder()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Owner owner = await ApplicationTestDataBuilder.CreateOwnerAsync(dbContext);
        Pet pet = await ApplicationTestDataBuilder.CreatePetAsync(dbContext, owner.Id);
        Clinic clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);
        Veterinarian veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);

        Appointment earlierAppointment = new(
            pet.Id,
            veterinarian.Id,
            clinic.Id,
            new DateTimeOffset(2026, 1, 2, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 1, 2, 10, 30, 0, TimeSpan.Zero),
            "Earlier appointment");

        Appointment laterAppointment = new(
            pet.Id,
            veterinarian.Id,
            clinic.Id,
            new DateTimeOffset(2026, 1, 2, 12, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 1, 2, 12, 30, 0, TimeSpan.Zero),
            "Later appointment");

        _ = await dbContext.Appointments.AddAsync(earlierAppointment);
        _ = await dbContext.Appointments.AddAsync(laterAppointment);
        _ = await dbContext.SaveChangesAsync();

        SearchParameters searchParameters = new(
            SortCriteriaCollection:
            [
                new SearchParameters.SortCriteria("startAtUtc", OrderDirection.Descending)
            ]);

        Result<List<GetAppointmentResponse>> getResult = await mediator.Send(new GetAppointmentsQuery(searchParameters));

        Assert.True(getResult.IsSuccess);
        Assert.Equal(2, getResult.Value.Count);
        Assert.Equal(laterAppointment.Id, getResult.Value[0].Id);
        Assert.Equal(earlierAppointment.Id, getResult.Value[1].Id);
    }

    [Fact]
    public async Task GetAppointmentsQuery_WhenPageSizeIsOutOfRange_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await SearchParametersValidationTestHelper.AssertInvalidPageSizeAsync(mediator, searchParameters => new GetAppointmentsQuery(searchParameters));
    }
}

