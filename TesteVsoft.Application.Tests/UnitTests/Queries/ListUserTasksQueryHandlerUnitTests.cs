using Bogus;
using FluentAssertions;
using Moq;
using TesteVsoft.Application.Common.Builders;
using TesteVsoft.Application.Common.Extensions;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Application.Queries;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Tests.UnitTests.Queries;

[TestFixture]
public class ListUserTasksQueryHandlerUnitTests
{
    private Mock<IUserTaskRepository> _repositoryMock = null!;
    private ListUserTasksQueryHandler _handler = null!;
    private Faker _faker = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IUserTaskRepository>();
        _handler = new ListUserTasksQueryHandler(_repositoryMock.Object);
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
    public async Task Handle_ShouldCallRepositoryWithCorrectFilter_AndReturnPaginatedList()
    {
        // Arrange
        var userTask1 = CreateUserTask(CreateUser());
        var userTask2 = CreateUserTask(CreateUser());

        var filters = new FilterBuilder<User, Guid>()
            .WithPage(1)
            .WithPageSize(2)
            .Build()
            .ToFilterDto();

        var query = new ListUserTasksQuery();

        filters.GetType().GetProperties().ToList().ForEach(p =>
        {
            p.SetValue(query, p.GetValue(filters));
        });

        List<UserTask> expectedItems = [userTask1, userTask2];

        var expectedResult = new PaginatedList<UserTask, Guid>(
            expectedItems,
            totalCount: 2,
            page: 1,
            pageSize: 10
        );

        _repositoryMock
            .Setup(r => r.GetPaginatedAsync(
                It.IsAny<Filter<UserTask, Guid>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(query!, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Items.Should().ContainSingle(t => t.Title == userTask1.Title);

        _repositoryMock.Verify(r =>
            r.GetPaginatedAsync(It.IsAny<Filter<UserTask, Guid>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
