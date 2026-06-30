using Mediator;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Common.Search;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Application.Veterinarians.Queries;

namespace ProjectName.Application.Tests.Veterinarians.Queries;

public sealed class GetVeterinariansQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetVeterinariansQuery_WhenVeterinariansExist_ShouldReturnCreatedVeterinarian()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);

        var getResult = await mediator.Send(new GetVeterinariansQuery(SearchParameters: null));

        Assert.True(getResult.IsSuccess);
        Assert.Contains(getResult.Value, v => v.Id == veterinarian.Id && v.Email == veterinarian.Email && v.LicenseNumber == veterinarian.LicenseNumber);
    }

    [Fact]
    public async Task GetVeterinariansQuery_WhenPageSizeIsOutOfRange_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        SearchParameters invalidSearchParameters = new(PageNumber: 1, PageSize: 101);

        Result<List<GetVeterinarianResponse>> getResult = await mediator.Send(new GetVeterinariansQuery(invalidSearchParameters));

        Assert.True(getResult.IsFailed);
        Assert.Contains(getResult.Errors, error => error is ValidationError validationError && validationError.Identifier == "SearchParameters.PageSize");
    }
}

