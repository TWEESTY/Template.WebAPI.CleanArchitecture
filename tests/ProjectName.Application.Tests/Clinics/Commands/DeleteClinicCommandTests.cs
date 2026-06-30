using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Clinics.Commands;
using ProjectName.Application.Clinics.Common;
using ProjectName.Application.Clinics.Queries;
using ProjectName.Application.Common.Errors;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Clinics.Commands;

public sealed class DeleteClinicCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task DeleteClinicCommand_WhenClinicExists_ShouldDeleteClinic()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Clinic clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);

        Result deleteResult = await mediator.Send(new DeleteClinicCommand(clinic.Id));
        Result<GetClinicResponse> getResult = await mediator.Send(new GetClinicByIdQuery(clinic.Id));

        Assert.True(deleteResult.IsSuccess);
        Assert.True(getResult.IsFailed);
        Assert.Contains(getResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Clinics.AnyAsync(item => item.Id == clinic.Id));
    }

    [Fact]
    public async Task DeleteClinicCommand_WhenClinicDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Result deleteResult = await mediator.Send(new DeleteClinicCommand(Guid.NewGuid()));

        Assert.True(deleteResult.IsFailed);
        Assert.Contains(deleteResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Clinics.AnyAsync());
    }
}

