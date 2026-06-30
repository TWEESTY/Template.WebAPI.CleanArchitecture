using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using ProjectName.Application.Common.Errors;
using ProjectName.Application.Veterinarians.Common;
using ProjectName.Application.Veterinarians.Commands;
using ProjectName.Domain.Entities;
using ProjectName.Infrastructure.Persistence;

namespace ProjectName.Application.Tests.Veterinarians.Commands;

public sealed class CreateVeterinarianCommandTests(SqliteApplicationTestFixture fixture) : IClassFixture<SqliteApplicationTestFixture>
{
    [Fact]
    public async Task CreateVeterinarianCommand_WhenVeterinarianIsValid_ShouldCreateVeterinarian()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Result<GetVeterinarianResponse> createResult = await mediator.Send(new CreateVeterinarianCommand("Jane", "Doe", $"vet-{Guid.NewGuid():N}@example.test", $"LIC-{Guid.NewGuid():N}"));

        Assert.True(createResult.IsSuccess);
        Assert.Equal("Jane", createResult.Value.FirstName);
        Veterinarian storedVeterinarian = await dbContext.Veterinarians.SingleAsync(veterinarian => veterinarian.Id == createResult.Value.Id);
        Assert.Equal("Doe", storedVeterinarian.LastName);
    }

    [Fact]
    public async Task CreateVeterinarianCommand_WhenEmailIsEmpty_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        string invalidEmail = string.Empty;
        string licenseNumber = $"LIC-{Guid.NewGuid():N}";

        Result<GetVeterinarianResponse> createResult = await mediator.Send(new CreateVeterinarianCommand("Jane", "Doe", invalidEmail, licenseNumber));

        Assert.True(createResult.IsFailed);
        Assert.Contains(createResult.Errors, error => error is ValidationError);
        Assert.False(await dbContext.Veterinarians.AnyAsync(veterinarian => veterinarian.Email == invalidEmail));
    }

    [Fact]
    public async Task CreateVeterinarianCommand_WhenLicenseNumberIsEmpty_ShouldFailWithValidationError()
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        string email = $"vet-{Guid.NewGuid():N}@example.test";

        Result<GetVeterinarianResponse> createResult = await mediator.Send(new CreateVeterinarianCommand("Jane", "Doe", email, string.Empty));

        Assert.True(createResult.IsFailed);
        Assert.Contains(createResult.Errors, error => error is ValidationError validationError && validationError.Identifier == nameof(CreateVeterinarianCommand.LicenseNumber));
        Assert.False(await dbContext.Veterinarians.AnyAsync(veterinarian => veterinarian.Email == email));
    }
}
