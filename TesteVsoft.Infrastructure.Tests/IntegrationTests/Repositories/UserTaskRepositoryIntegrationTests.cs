using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using TesteVsoft.Application.Common.Builders;
using TesteVsoft.Application.Common.Enums;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Domain.Enums;
using TesteVsoft.Infrastructure.Data;
using TesteVsoft.Infrastructure.Repositories;

namespace TesteVsoft.Infrastructure.Tests.IntegrationTests.Repositories;

[TestFixture]
public class UserTaskRepositoryIntegrationTests
{
    private PostgreSqlContainer _container;
    private ApplicationDbContext _context;
    private UserTaskRepository _repository;
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

        _repository = new UserTaskRepository(_context);
        _faker = new Faker("pt_BR");
    }

    private User CreateUser(string? name = null, string? email = null)
    {
        return User.Create(string.IsNullOrEmpty(name) ? _faker.Name.FirstName() : name,
            _faker.Internet.UserName(),
            _faker.Internet.Password(),
            string.IsNullOrEmpty(email) ? _faker.Internet.Email() : email);
    }

    private UserTask CreateUserTask(User user)
    {
        return UserTask.Create(
            title: _faker.Lorem.Word(),
            description: _faker.Lorem.Paragraph(),
            dueDate: DateTime.UtcNow.AddDays(3),
            assignedUser: user
        );
    }

    [Test]
    public async Task Should_Add_And_Get_UserTask()
    {
        // Arrange
        var user = CreateUser();
        var userTask = CreateUserTask(user);

        await _context.Users.AddAsync(user);

        // Act
        await _repository.AddAsync(userTask, CancellationToken.None);
        var result = await _repository.GetOneByIdAsync(userTask.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userTask.Id);
        result.Title.Should().Be(userTask.Title);
        result.Description.Should().Be(userTask.Description);
        result.DueDate.Should().Be(userTask.DueDate);
        result.AssignedUser.Should().Be(userTask.AssignedUser);
        result.Status.Should().Be(UserTaskStatusTypes.Pending);
    }

    [Test]
    public async Task Should_AddRange_And_GetAll()
    {
        // Arrange
        var userTasks = Enumerable.Range(0, 3)
            .Select(_ => CreateUserTask(CreateUser()))
            .ToList();

        // Act
        await _repository.AddRangeAsync(userTasks, CancellationToken.None);

        // Assert
        foreach (var userTask in userTasks)
        {
            var found = await _repository.GetOneByIdAsync(userTask.Id, CancellationToken.None);
            found.Should().NotBeNull();
            found!.Id.Should().Be(userTask.Id);
            found.Title.Should().Be(userTask.Title);
            found.Description.Should().Be(userTask.Description);
            found.DueDate.Should().Be(userTask.DueDate);
            found.AssignedUser.Should().Be(userTask.AssignedUser);
            found.Status.Should().Be(UserTaskStatusTypes.Pending);
        }
    }

    [Test]
    public async Task Should_GetOne_With_Filter()
    {
        // Arrange
        var user = CreateUser();
        var userTask = CreateUserTask(user);
        await _repository.AddAsync(userTask, CancellationToken.None);

        // Act
        var filter = new FilterBuilder<UserTask, Guid>()
            .AddWhere(u => u.Title, WhereOperationTypes.Equal, userTask.Title)
            .Build();

        var found = await _repository.GetOneAsync(filter, CancellationToken.None);

        // Assert
        found.Should().NotBeNull();
        found!.Id.Should().Be(userTask.Id);
        found.Title.Should().Be(userTask.Title);
        found.Description.Should().Be(userTask.Description);
        found.DueDate.Should().Be(userTask.DueDate);
        found.AssignedUser.Should().Be(userTask.AssignedUser);
        found.Status.Should().Be(UserTaskStatusTypes.Pending);
    }

    [Test]
    public async Task Should_GetPaginated_WithOrderByAscending()
    {
        // Arrange
        var UserTasks = Enumerable.Range(1, 10)
            .Select(_ => CreateUserTask(CreateUser()))
            .ToList();

        // Act
        await _repository.AddRangeAsync(UserTasks, CancellationToken.None);

        var filter = new FilterBuilder<UserTask, Guid>()
            .WithPage(1)
            .WithPageSize(5)
            .AddOrderBy(u => u.Title, OrderDirectionTypes.Ascending)
            .Build();

        var result = await _repository.GetPaginatedAsync(filter, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(5);
        result.Items.Select(u => u.Title).Should().BeInAscendingOrder(StringComparer.InvariantCultureIgnoreCase);
        result.TotalCount.Should().Be(10);
    }

    [Test]
    public async Task Should_GetPaginated_WithOrderByDescending()
    {
        // Arrange
        var UserTasks = Enumerable.Range(1, 10)
            .Select(_ => CreateUserTask(CreateUser()))
            .ToList();

        // Act
        await _repository.AddRangeAsync(UserTasks, CancellationToken.None);

        var filter = new FilterBuilder<UserTask, Guid>()
            .WithPage(1)
            .WithPageSize(5)
            .AddOrderBy(u => u.Title, OrderDirectionTypes.Descending)
            .Build();

        var result = await _repository.GetPaginatedAsync(filter, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(5);
        result.Items.Select(u => u.Title).Should().BeInDescendingOrder(StringComparer.InvariantCultureIgnoreCase);
        result.TotalCount.Should().Be(10);
    }

    [Test]
    public async Task Should_GetAll_With_Filter()
    {
        // Arrange
        var filter = new FilterBuilder<UserTask, Guid>()
            .WithPage(1)
            .WithPageSize(100)
            .AddOrderBy(u => u.Title, OrderDirectionTypes.Descending)
            .Build();

        // Act
        var allUserTasks = await _repository.GetAsync(filter, CancellationToken.None);

        // Assert
        allUserTasks.Should().NotBeNull();
        allUserTasks.Should().BeAssignableTo<IEnumerable<UserTask>>();
    }

    [Test]
    public async Task Should_Update()
    {
        // Arrange
        var userTask = CreateUserTask(CreateUser());

        await _repository.AddAsync(userTask, CancellationToken.None);

        var newTitle = _faker.Lorem.Word();
        var newDescription = _faker.Lorem.Paragraph();
        var newAssignedUser = CreateUser();
        var newDueDate = _faker.Date.Future().ToUniversalTime();
        var newStatus = _faker.Random.Enum<UserTaskStatusTypes>();

        _context.Set<User>().Add(newAssignedUser);
        await _context.SaveChangesAsync();
        userTask.Update(newTitle, newDescription, newDueDate, newStatus, newAssignedUser);

        // Act
        await _repository.UpdateAsync(userTask, CancellationToken.None);

        var updated = await _repository.GetOneByIdAsync(userTask.Id, CancellationToken.None);

        // Assert
        updated!.Title.Should().Be(newTitle);
        updated!.Description.Should().Be(newDescription);
        updated!.AssignedUser.Should().Be(newAssignedUser);
        updated!.DueDate.Should().Be(newDueDate);
        updated!.Status.Should().Be(newStatus);
    }

    [Test]
    public async Task Should_UpdateRange()
    {
        // Arrange
        var UserTask1 = CreateUserTask(CreateUser());
        var UserTask2 = CreateUserTask(CreateUser());

        await _repository.AddRangeAsync([UserTask1, UserTask2], CancellationToken.None);

        var newTitle1 = _faker.Lorem.Word();
        var newTitle2 = _faker.Lorem.Word();
        var newDescription1 = _faker.Lorem.Paragraph();
        var newDescription2 = _faker.Lorem.Paragraph();
        var newAssignedUser1 = CreateUser();
        var newAssignedUser2 = CreateUser();
        var newDueDate1 = _faker.Date.Future().ToUniversalTime();
        var newDueDate2 = _faker.Date.Future().ToUniversalTime();
        var newStatus1 = _faker.Random.Enum<UserTaskStatusTypes>();
        var newStatus2 = _faker.Random.Enum<UserTaskStatusTypes>();

        _context.Set<User>().AddRange([newAssignedUser1, newAssignedUser2]);
        await _context.SaveChangesAsync();
        UserTask1.Update(newTitle1, newDescription1, newDueDate1, newStatus1, newAssignedUser1);
        UserTask2.Update(newTitle2, newDescription2, newDueDate2, newStatus2, newAssignedUser2);

        // Act
        await _repository.UpdateRangeAsync([UserTask1, UserTask2], CancellationToken.None);

        var updated1 = await _repository.GetOneByIdAsync(UserTask1.Id, CancellationToken.None);
        var updated2 = await _repository.GetOneByIdAsync(UserTask2.Id, CancellationToken.None);

        // Assert
        updated1!.Title.Should().Be(newTitle1);
        updated2!.Title.Should().Be(newTitle2);
        updated1!.Description.Should().Be(newDescription1);
        updated2!.Description.Should().Be(newDescription2);
        updated1!.AssignedUser.Should().Be(newAssignedUser1);
        updated2!.AssignedUser.Should().Be(newAssignedUser2);
        updated1!.DueDate.Should().Be(newDueDate1);
        updated2!.DueDate.Should().Be(newDueDate2);
        updated1!.Status.Should().Be(newStatus1);
        updated2!.Status.Should().Be(newStatus2);
    }

    [Test]
    public async Task Should_Remove()
    {
        // Arrange
        var userTask = CreateUserTask(CreateUser());

        await _repository.AddAsync(userTask, CancellationToken.None);

        // Act
        await _repository.RemoveAsync(userTask, CancellationToken.None);
        var result = await _repository.GetOneByIdAsync(userTask.Id, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task Should_RemoveRange()
    {
        // Arrange
        var user1 = CreateUser();
        var user2 = CreateUser();
        var UserTask1 = CreateUserTask(user1);
        var UserTask2 = CreateUserTask(user2);

        await _repository.AddRangeAsync([UserTask1, UserTask2], CancellationToken.None);

        // Act
        await _repository.RemoveRangeAsync([UserTask1, UserTask2], CancellationToken.None);
        var result1 = await _repository.GetOneByIdAsync(UserTask1.Id, CancellationToken.None);
        var result2 = await _repository.GetOneByIdAsync(UserTask2.Id, CancellationToken.None);

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
