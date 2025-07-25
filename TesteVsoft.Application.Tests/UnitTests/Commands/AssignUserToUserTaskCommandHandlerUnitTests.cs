using Bogus;
using FluentAssertions;
using MassTransit;
using Moq;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Common.Exceptions;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Tests.UnitTests.Commands;

[TestFixture]
public class AssignUserToUserTaskCommandHandlerUnitTests
{
    private Mock<IUserTaskRepository> _userTaskRepositoryMock = null!;
    private Mock<IPublishEndpoint> _publishEndpointMock = null!;
    private AssignUserToUserTaskCommandHandler _handler = null!;
    private Faker _faker = null!;

    [SetUp]
    public void Setup()
    {
        _userTaskRepositoryMock = new Mock<IUserTaskRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new AssignUserToUserTaskCommandHandler(
            _userTaskRepositoryMock.Object,
            _publishEndpointMock.Object);

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
    public async Task Handle_Should_AssignUser_When_TaskExists_And_PublishEvent_IfUserChanged()
    {
        // Arrange
        var oldUser = CreateUser();
        var newUser = CreateUser();

        var userTask = CreateUserTask(oldUser);
        var taskId = userTask.Id;

        _userTaskRepositoryMock.Setup(r => r.GetOneByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userTask);

        _userTaskRepositoryMock.Setup(r => r.UpdateAsync(userTask, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userTask);

        var command = new AssignUserToUserTaskCommand
        {
            Id = taskId,
            UserId = Guid.NewGuid(),
            User = newUser
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(userTask);
        userTask.AssignedUser.Should().Be(newUser);

        _publishEndpointMock.Verify(p => p.Publish(
            It.Is<UserTaskAssignedToUserDto>(dto => dto.UserTaskId == userTask.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_Should_NotPublishEvent_When_UserIsSame()
    {
        // Arrange
        var user = CreateUser();

        var userTask = CreateUserTask(user);

        _userTaskRepositoryMock.Setup(r => r.GetOneByIdAsync(userTask.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userTask);

        _userTaskRepositoryMock.Setup(r => r.UpdateAsync(userTask, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userTask);

        var command = new AssignUserToUserTaskCommand
        {
            Id = userTask.Id,
            UserId = Guid.NewGuid(),
            User = user
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(userTask);
        _publishEndpointMock.Verify(p => p.Publish(It.IsAny<UserTaskAssignedToUserDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_Should_ThrowNotFoundException_When_TaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new AssignUserToUserTaskCommand
        {
            Id = taskId,
            UserId = Guid.NewGuid(),
            User = CreateUser()
        };

        _userTaskRepositoryMock.Setup(r => r.GetOneByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserTask?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Tarefa não encontrada");
    }
}
