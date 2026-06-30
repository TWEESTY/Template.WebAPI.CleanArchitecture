using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Clinics.Queries;
using ProjectName.Application.Tests.Search;

namespace ProjectName.Application.Tests.Clinics.Queries;

public sealed class GetClinicsQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetClinicsQuery_WhenClinicsExist_ShouldReturnCreatedClinic()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var clinic = await ApplicationTestDataBuilder.CreateClinicAsync(dbContext);

        var getResult = await mediator.Send(new GetClinicsQuery(SearchParameters: null));

        Assert.True(getResult.IsSuccess);
        Assert.Contains(getResult.Value, c => c.Id == clinic.Id && c.Name == clinic.Name && c.Address == clinic.Address);
    }

    [Fact]
    public async Task GetClinicsQuery_WhenPageNumberIsZero_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await SearchParametersValidationTestHelper.AssertInvalidPageNumberAsync(mediator, searchParameters => new GetClinicsQuery(searchParameters));
    }
}

