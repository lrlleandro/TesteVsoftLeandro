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
public class ListUsersQueryHandlerUnitTests
{
    private Mock<IUserRepository> _repositoryMock = null!;
    private ListUsersQueryHandler _handler = null!;
    private Faker _faker = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new ListUsersQueryHandler(_repositoryMock.Object);
        _faker = new Faker("pt_BR");
    }

    private User CreateUser(string? name = null, string? email = null)
    {
        return User.Create(string.IsNullOrEmpty(name) ? _faker.Name.FirstName() : name,
            _faker.Internet.UserName(),
            _faker.Internet.Password(),
            string.IsNullOrEmpty(email) ? _faker.Internet.Email() : email);
    }

    [Test]
    public async Task Handle_ShouldCallRepositoryWithCorrectFilter_AndReturnPaginatedList()
    {
        // Arrange
        var user1 = CreateUser();
        var user2 = CreateUser();

        var filters = new FilterBuilder<User, Guid>()
            .WithPage(1)
            .WithPageSize(2)
            .Build()
            .ToFilterDto();

        var query = new ListUsersQuery();

        filters.GetType().GetProperties().ToList().ForEach(p =>
        {
            p.SetValue(query, p.GetValue(filters));
        });

        List<User> expectedItems = [user1, user2];

        var expectedResult = new PaginatedList<User, Guid>(
            expectedItems,
            totalCount: 2,
            page: 1,
            pageSize: 10
        );

        _repositoryMock
            .Setup(r => r.GetPaginatedAsync(
                It.IsAny<Filter<User, Guid>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(query!, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Items.Should().ContainSingle(t => t.Name == user1.Name);

        _repositoryMock.Verify(r =>
            r.GetPaginatedAsync(It.IsAny<Filter<User, Guid>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
