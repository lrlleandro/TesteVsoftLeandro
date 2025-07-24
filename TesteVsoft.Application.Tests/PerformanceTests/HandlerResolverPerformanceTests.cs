using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Common.Builders;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Infrastructure.Data;
using TesteVsoft.Infrastructure.Repositories;

namespace TesteVsoft.Application.Tests.PerformanceTests;

[TestFixture]
public class CreateRandomUsersCommandHandlerPerformanceTests
{
    private ServiceProvider _serviceProvider = null!;

    [SetUp]
    public async Task SetUp()
    {
        var services = new ServiceCollection();

        // In-memory database for test
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
        });

        services.AddScoped<IUserRepository, UserRepository>();

        _serviceProvider = services.BuildServiceProvider();

        var db = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider.Dispose();
    }

    [TestCase(100, 10)]
    [TestCase(1_000, 100)]
    [TestCase(10_000, 1000)]
    public async Task Handle_Should_Create_Users_Efficiently(int amount, int miliseconds)
    {
        // Arrange
        var repository = _serviceProvider.GetRequiredService<IUserRepository>();
        var handler = new CreateRandomUsersCommandHandler(repository);

        var command = new CreateRandomUsersCommand
        {
            Amount = amount,
            UserNameMask = "user_{{random}}"
        };

        var stopwatch = Stopwatch.StartNew();

        // Act
        await handler.Handle(command, CancellationToken.None);

        stopwatch.Stop();

        // Assert
        var filter = new FilterBuilder<User, Guid>()
            .WithPage(1)
            .WithPageSize(1)
            .Build();

        var result = await repository.GetPaginatedAsync(filter, CancellationToken.None);

        result.TotalCount.Should().BeGreaterThanOrEqualTo(amount);
        stopwatch.Elapsed.TotalSeconds.Should().BeLessThan(miliseconds);
    }
}
