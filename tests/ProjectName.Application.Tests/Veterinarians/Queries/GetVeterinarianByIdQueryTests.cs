using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Queries;

namespace ProjectName.Application.Tests.Veterinarians.Queries;

public sealed class GetVeterinarianByIdQueryTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task GetVeterinarianByIdQuery_WhenVeterinarianExists_ShouldReturnVeterinarian()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var veterinarian = await ApplicationTestDataBuilder.CreateVeterinarianAsync(dbContext);

        var getResult = await mediator.Send(new GetVeterinarianByIdQuery(veterinarian.Id));

        Assert.True(getResult.IsSuccess);
        Assert.Equal(veterinarian.Id, getResult.Value.Id);
    }

    [Fact]
    public async Task GetVeterinarianByIdQuery_WhenVeterinarianDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectName.Infrastructure.Persistence.AppDbContext>();

        var getResult = await mediator.Send(new GetVeterinarianByIdQuery(Guid.NewGuid()));

        Assert.True(getResult.IsFailed);
        Assert.Contains(getResult.Errors, error => error is NotFoundError);
    }
}

