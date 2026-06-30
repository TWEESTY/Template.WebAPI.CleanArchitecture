using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Vaccines.Queries;

namespace ProjectName.Application.Tests.Vaccines.Queries;

public sealed class GetVaccineByIdQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetVaccineByIdQuery_WhenVaccineExists_ShouldReturnVaccine()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var vaccine = await ApplicationTestDataBuilder.CreateVaccineAsync(dbContext);

        var getResult = await mediator.Send(new GetVaccineByIdQuery(vaccine.Id));

        Assert.True(getResult.IsSuccess);
        Assert.Equal(vaccine.Id, getResult.Value.Id);
    }

    [Fact]
    public async Task GetVaccineByIdQuery_WhenVaccineDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var getResult = await mediator.Send(new GetVaccineByIdQuery(Guid.NewGuid()));

        Assert.True(getResult.IsFailed);
        Assert.Contains(getResult.Errors, error => error is NotFoundError);
    }
}

