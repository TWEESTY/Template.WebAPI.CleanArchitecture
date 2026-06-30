using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Clinics.Queries;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Application.Tests.Clinics.Queries;

public sealed class GetClinicByIdQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetClinicByIdQuery_WhenClinicExists_ShouldReturnClinic()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);

        var getResult = await mediator.Send(new GetClinicByIdQuery(clinic.Id));

        Assert.True(getResult.IsSuccess);
        Assert.Equal(clinic.Id, getResult.Value.Id);
    }

    [Fact]
    public async Task GetClinicByIdQuery_WhenClinicDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var getResult = await mediator.Send(new GetClinicByIdQuery(Guid.NewGuid()));

        Assert.True(getResult.IsFailed);
        Assert.Contains(getResult.Errors, error => error is NotFoundError);
    }
}

