using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Testcontainers.PostgreSql;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Infrastructure.Data;

namespace TesteVsoft.Infrastructure.Tests.IntegrationTests.Data;

[TestFixture]
public class ApplicationDbContextInitialiserIntegrationTests
{
    private PostgreSqlContainer _container;
    private ApplicationDbContext _context;
    private ApplicationDbContextInitialiser _initialiser;
    private Mock<ILogger<ApplicationDbContextInitialiser>> _loggerMock;
    
    [SetUp]
    public async Task OneTimeSetup()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithImage("postgres:16")
            .WithCleanUp(true)
            .Build();

        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        _context = new ApplicationDbContext(options);
    }

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<ApplicationDbContextInitialiser>>();
        
        _initialiser = new ApplicationDbContextInitialiser(
            _loggerMock.Object,
            _context);
    }

    [Test]
    public async Task ApplyMigrationsAsync_Should_Migrate_Database()
    {
        // Act
        await _initialiser.ApplyMigrationsAsync();

        // Assert
        var pending = await _context.Database.GetPendingMigrationsAsync();
        pending.Should().BeEmpty();
    }

    [Test]
    public async Task SeedAsync_Should_Insert_Admin_User()
    {
        // Arrange
        await _initialiser.ApplyMigrationsAsync();
        
        // Act
        await _initialiser.SeedAsync();

        // Assert
        var admin = await _context.Set<User>().FirstOrDefaultAsync(x => x.UserName == "admin");
        admin.Should().NotBeNull();
        BCrypt.Net.BCrypt.Verify("vsoft", admin.Password);
    }

    [TearDown]
    public async Task Cleanup()
    {
        if (_context is not null)
        {
            await _context.DisposeAsync();
        }

        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }
}
