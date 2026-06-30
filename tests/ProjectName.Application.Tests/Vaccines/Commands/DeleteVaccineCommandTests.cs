using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Vaccines.Commands;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Vaccines.Commands;

public sealed class DeleteVaccineCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task DeleteVaccineCommand_WhenVaccineExists_ShouldDeleteVaccine()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Vaccine vaccine = await ApplicationTestDataBuilder.CreateVaccineAsync(dbContext);

        Result deleteResult = await mediator.Send(new DeleteVaccineCommand(vaccine.Id));

        Assert.True(deleteResult.IsSuccess);
        Assert.False(await dbContext.Vaccines.AnyAsync(item => item.Id == vaccine.Id));
    }

    [Fact]
    public async Task DeleteVaccineCommand_WhenVaccineDoesNotExist_ShouldFailWithNotFoundError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Result deleteResult = await mediator.Send(new DeleteVaccineCommand(Guid.NewGuid()));

        Assert.True(deleteResult.IsFailed);
        Assert.Contains(deleteResult.Errors, error => error is NotFoundError);
        Assert.False(await dbContext.Vaccines.AnyAsync());
    }
}

