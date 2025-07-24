using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using Testcontainers.PostgreSql;
using TesteVsoft.Application.Common.Builders;
using TesteVsoft.Application.Common.Enums;
using TesteVsoft.Infrastructure.Data;
using TesteVsoft.Infrastructure.Repositories;
using User = TesteVsoft.Domain.Entities.User;

namespace TesteVsoft.Infrastructure.Tests.IntegrationTests.Repositories;

[TestFixture]
public class UserRepositoryIntegrationTests
{
    private PostgreSqlContainer _container;
    private ApplicationDbContext _context;
    private UserRepository _repository;
    private Faker _faker;

    [SetUp]
    public async Task Setup()
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
        await _context.Database.EnsureCreatedAsync();

        _repository = new UserRepository(_context);
        _faker = new Faker("pt_BR");
    }

    [Test]
    public async Task Should_Add_And_Get_User()
    {
        // Arrange
        var user = User.Create(_faker.Person.FirstName, _faker.Internet.UserName(), _faker.Internet.Password(), _faker.Internet.Email());

        // Act
        await _repository.AddAsync(user, CancellationToken.None);
        var result = await _repository.GetOneByIdAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Name);
        result.UserName.Should().Be(user.UserName);
        result.Password.Should().Be(user.Password);
        result.Email.Should().Be(user.Email);
    }

    [Test]
    public async Task Should_AddRange_And_GetAll()
    {
        // Arrange
        var users = Enumerable.Range(0, 3)
            .Select(_ => User.Create(_faker.Person.FullName, _faker.Internet.UserName(), _faker.Internet.Password(), _faker.Internet.Email()))
            .ToList();

        // Act
        await _repository.AddRangeAsync(users, CancellationToken.None);

        // Assert
        foreach (var user in users)
        {
            var found = await _repository.GetOneByIdAsync(user.Id, CancellationToken.None);
            found.Should().NotBeNull();
        }
    }

    [Test]
    public async Task Should_GetOne_With_Filter()
    {
        // Arrange
        var user = User.Create("specific_user", _faker.Internet.UserName(), _faker.Internet.Password(), _faker.Internet.Email());
        await _repository.AddAsync(user, CancellationToken.None);

        // Act
        var filter = new FilterBuilder<User, Guid>()
            .AddWhere(u => u.Name, WhereOperationTypes.Equal, "specific_user")
            .Build();

        var found = await _repository.GetOneAsync(filter, CancellationToken.None);

        // Assert
        found.Should().NotBeNull();
        found!.Name.Should().Be("specific_user");
    }

    [Test]
    public async Task Should_GetPaginated_WithOrderByAscending()
    {
        // Arrange
        var users = Enumerable.Range(1, 10)
            .Select(_ => User.Create(_faker.Person.FullName, _faker.Internet.UserName(), _faker.Internet.Password(), _faker.Internet.Email()))
            .ToList();

        // Act
        await _repository.AddRangeAsync(users, CancellationToken.None);

        var filter = new FilterBuilder<User, Guid>()
            .WithPage(1)
            .WithPageSize(5)
            .AddOrderBy(u => u.UserName, OrderDirectionTypes.Ascending)
            .Build();

        var result = await _repository.GetPaginatedAsync(filter, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(5);
        result.Items.Select(u => u.UserName).Should().BeInAscendingOrder(StringComparer.InvariantCultureIgnoreCase);
        result.TotalCount.Should().Be(10);
    }

    [Test]
    public async Task Should_GetPaginated_WithOrderByDescending()
    {
        // Arrange
        var users = Enumerable.Range(1, 10)
            .Select(_ => User.Create(_faker.Person.FullName, _faker.Internet.UserName(), _faker.Internet.Password(), _faker.Internet.Email()))
            .ToList();

        // Act
        await _repository.AddRangeAsync(users, CancellationToken.None);

        var filter = new FilterBuilder<User, Guid>()
            .WithPage(1)
            .WithPageSize(5)
            .AddOrderBy(u => u.UserName, OrderDirectionTypes.Descending)
            .Build();

        var result = await _repository.GetPaginatedAsync(filter, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(5);
        result.Items.Select(u => u.UserName).Should().BeInDescendingOrder(StringComparer.InvariantCultureIgnoreCase);
        result.TotalCount.Should().Be(10);
    }

    [Test]
    public async Task Should_GetAll_With_Filter()
    {
        // Arrange
        var filter = new FilterBuilder<User, Guid>()
            .WithPage(1)
            .WithPageSize(100)
            .AddOrderBy(u => u.UserName, OrderDirectionTypes.Descending)
            .Build();

        // Act
        var allUsers = await _repository.GetAsync(filter, CancellationToken.None);

        // Assert
        allUsers.Should().NotBeNull();
        allUsers.Should().BeAssignableTo<IEnumerable<User>>();
    }

    [Test]
    public async Task Should_Update()
    {
        // Arrange
        var user = User.Create(_faker.Person.FirstName, _faker.Internet.UserName(), "123", _faker.Internet.Email());
                
        await _repository.AddAsync(user, CancellationToken.None);

        var newName = "Updated User";        
        var newUserName = "updated_user";
        var newpassword = "new_password";        
        var newEmail = "new_email@email.com";
        
        user.Update(newName, newUserName, "123", newpassword, newEmail);
        
        // Act
        await _repository.UpdateAsync(user, CancellationToken.None);

        var updated = await _repository.GetOneByIdAsync(user.Id, CancellationToken.None);
        
        // Assert
        updated!.Name.Should().Be(newName);
        updated!.UserName.Should().Be(newUserName);
        updated!.Email.Should().Be(new MailAddress(newEmail));
        BCrypt.Net.BCrypt.Verify(newpassword, updated!.Password).Should().BeTrue();
    }

    [Test]
    public async Task Should_UpdateRange()
    {
        // Arrange
        var user1 = User.Create(_faker.Person.FirstName, _faker.Internet.UserName(), "123", _faker.Internet.Email());
        var user2 = User.Create(_faker.Person.FirstName, _faker.Internet.UserName(), "456", _faker.Internet.Email());

        await _repository.AddRangeAsync([user1, user2], CancellationToken.None);

        var newName1 = "Updated User 1";
        var newName2 = "Updated User 2";
        var newUserName1 = "updated_user1";
        var newUserName2 = "updated_user2";
        var newpassword1 = "new_password1";
        var newpassword2 = "new_password2";
        var newEmail1 = "new_email1@email.com";
        var newEmail2 = "new_email2@email.com";

        user1.Update(newName1, newUserName1, "123", newpassword1, newEmail1);
        user2.Update(newName2, newUserName2, "456", newpassword2, newEmail2);

        // Act
        await _repository.UpdateRangeAsync([user1, user2], CancellationToken.None);

        var updated1 = await _repository.GetOneByIdAsync(user1.Id, CancellationToken.None);
        var updated2 = await _repository.GetOneByIdAsync(user2.Id, CancellationToken.None);

        // Assert
        updated1!.Name.Should().Be(newName1);
        updated2!.Name.Should().Be(newName2);
        updated1!.UserName.Should().Be(newUserName1);
        updated2!.UserName.Should().Be(newUserName2);
        updated1!.Email.Should().Be(new MailAddress(newEmail1));
        updated2!.Email.Should().Be(new MailAddress(newEmail2));
        BCrypt.Net.BCrypt.Verify(newpassword1, updated1!.Password).Should().BeTrue();
        BCrypt.Net.BCrypt.Verify(newpassword2, updated2!.Password).Should().BeTrue();
    }

    [Test]
    public async Task Should_Remove()
    {
        // Arrange
        var user = User.Create("Delete User", "delete_user1", "123", "delete_user1@email.com");
        
        await _repository.AddAsync(user, CancellationToken.None);

        // Act
        await _repository.RemoveAsync(user, CancellationToken.None);
        var result = await _repository.GetOneByIdAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task Should_RemoveRange()
    {
        // Arrange
        var user1 = User.Create("delete_user1", "delete_user1_name", "123", "delete_user1@email.com");
        var user2 = User.Create("delete_user2", "delete_user2_name", "123", "delete_user2@email.com");

        await _repository.AddRangeAsync([user1, user2], CancellationToken.None);

        // Act
        await _repository.RemoveRangeAsync([user1, user2], CancellationToken.None);
        var result1 = await _repository.GetOneByIdAsync(user1.Id, CancellationToken.None);
        var result2 = await _repository.GetOneByIdAsync(user2.Id, CancellationToken.None);

        // Assert
        result1.Should().BeNull();
        result2.Should().BeNull();
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
