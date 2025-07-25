using Bogus;
using FluentAssertions;
using MassTransit;
using Moq;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Tests.UnitTests.Commands;

public class CreateUserTaskCommandHandlerUnitTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IUserTaskRepository> _userTaskRepositoryMock;
    private Mock<IPublishEndpoint> _publishEndpointMock;
    private CreateUserTaskCommandHandler _handler;
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _faker = new Faker("pt_BR");
        _userRepositoryMock = new Mock<IUserRepository>();
        _userTaskRepositoryMock = new Mock<IUserTaskRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();

        _handler = new CreateUserTaskCommandHandler(
            _userRepositoryMock.Object,
            _userTaskRepositoryMock.Object,
            _publishEndpointMock.Object
        );
    }

    private User CreateUser(string? name = null, string? email = null)
    {
        return User.Create(
            string.IsNullOrEmpty(name) ? _faker.Name.FirstName() : name,
            _faker.Internet.UserName(),
            _faker.Internet.Password(),
            string.IsNullOrEmpty(email) ? _faker.Internet.Email() : email
        );
    }

    [Test]
    public async Task Handle_Should_Create_UserTask_When_User_Exists()
    {
        // Arrange
        var user = CreateUser();
        var command = new CreateUserTaskCommand
        {
            UserId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            DueDate = _faker.Date.Future()
        };

        var createdTask = UserTask.Create(command.Title, command.Description, command.DueDate, user);

        _userRepositoryMock
            .Setup(x => x.GetOneByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userTaskRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<UserTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Title.Should().Be(command.Title);
        result.Description.Should().Be(command.Description);
        result.DueDate.Should().Be(command.DueDate);
        result.AssignedUser.Should().Be(user);

        _userRepositoryMock.Verify(x => x.GetOneByIdAsync(command.UserId, It.IsAny<CancellationToken>()), Times.Once);
        _userTaskRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserTask>(), It.IsAny<CancellationToken>()), Times.Once);
        _publishEndpointMock.Verify(x => x.Publish(
            It.Is<UserTaskAssignedToUserDto>(dto => dto.UserTaskId == createdTask.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_Should_Throw_UnauthorizedAccessException_When_User_Not_Found()
    {
        // Arrange
        var command = new CreateUserTaskCommand
        {
            UserId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            DueDate = _faker.Date.Future()
        };

        _userRepositoryMock
            .Setup(x => x.GetOneByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();

        _userRepositoryMock.Verify(x => x.GetOneByIdAsync(command.UserId, It.IsAny<CancellationToken>()), Times.Once);
        _userTaskRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserTask>(), It.IsAny<CancellationToken>()), Times.Never);
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<UserTaskAssignedToUserDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}