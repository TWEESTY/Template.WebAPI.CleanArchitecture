using Mediator;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Vaccines.Common;
using ProjectName.Application.Vaccines.Queries;

namespace ProjectName.Application.Tests.Vaccines.Queries;

public sealed class GetVaccinesQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetVaccinesQuery_WhenVaccinesExist_ShouldReturnCreatedVaccine()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var vaccine = await ApplicationTestDataBuilder.CreateVaccineAsync(dbContext);

        var getResult = await mediator.Send(new GetVaccinesQuery(SearchParameters: null));

        Assert.True(getResult.IsSuccess);
        Assert.Contains(getResult.Value, v => v.Id == vaccine.Id && v.Code == vaccine.Code && v.Name == vaccine.Name);
    }

    [Fact]
    public async Task GetVaccinesQuery_WhenPageNumberIsZero_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        SearchParameters invalidSearchParameters = new(PageNumber: 0, PageSize: 10);

        Result<List<GetVaccineResponse>> getResult = await mediator.Send(new GetVaccinesQuery(invalidSearchParameters));

        Assert.True(getResult.IsFailed);
        Assert.Contains(getResult.Errors, error => error is ValidationError validationError && validationError.Identifier == "SearchParameters.PageNumber");
    }
}

