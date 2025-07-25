using Bogus;
using FluentAssertions;
using MassTransit;
using Moq;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Common.Exceptions;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Domain.Enums;

namespace TesteVsoft.Tests.Unit.Application.Commands;

[TestFixture]
public class UpdateUserTaskCommandHandlerUnitTests
{
    private Mock<IUserTaskRepository> _userTaskRepository = null!;
    private Mock<IUserRepository> _userRepository = null!;
    private Mock<IPublishEndpoint> _publishEndpoint = null!;
    private UpdateUserTaskCommandHandler _handler = null!;
    private Faker _faker = null!;

    [SetUp]
    public void Setup()
    {
        _faker = new Faker("pt_BR");
        _userTaskRepository = new Mock<IUserTaskRepository>();
        _userRepository = new Mock<IUserRepository>();
        _publishEndpoint = new Mock<IPublishEndpoint>();
        _handler = new UpdateUserTaskCommandHandler(_userTaskRepository.Object, _userRepository.Object, _publishEndpoint.Object);
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

    private UpdateUserTaskCommand CreateCommand(Guid userTaskId, Guid assignedUserId)
    {
        return new UpdateUserTaskCommand
        {
            UserId = Guid.NewGuid(),
            Id = userTaskId,
            Title = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            DueDate = DateTime.UtcNow.AddDays(10),
            Status = UserTaskStatusTypes.InProgress,
            AssignedUserId = assignedUserId
        };
    }

    [Test]
    public async Task Handle_Should_Update_Task_And_Publish_Event_When_AssignedUser_Changes()
    {
        // Arrange
        var oldUser = CreateUser();
        var newUser = CreateUser();
        var userTask = CreateUserTask(oldUser);

        _userTaskRepository.Setup(x => x.GetOneByIdAsync(userTask.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userTask);

        _userRepository.Setup(x => x.GetOneByIdAsync(newUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newUser);

        _userRepository.Setup(x => x.GetOneByIdAsync(oldUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldUser);

        _userTaskRepository.Setup(x => x.UpdateAsync(It.IsAny<UserTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserTask t, CancellationToken _) => t);

        var command = CreateCommand(userTask.Id, newUser.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AssignedUserId.Should().Be(newUser.Id);

        _publishEndpoint.Verify(p => p.Publish(It.IsAny<UserTaskAssignedToUserDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_Should_Update_Task_Without_Publishing_When_AssignedUser_Same()
    {
        // Arrange
        var user = CreateUser();
        var userTask = CreateUserTask(user);

        _userTaskRepository.Setup(x => x.GetOneByIdAsync(userTask.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userTask);

        _userRepository.Setup(x => x.GetOneByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userTaskRepository.Setup(x => x.UpdateAsync(It.IsAny<UserTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserTask t, CancellationToken _) => t);

        var command = CreateCommand(userTask.Id, user.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AssignedUserId.Should().Be(user.Id);

        _publishEndpoint.Verify(p => p.Publish(It.IsAny<UserTaskAssignedToUserDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void Handle_Should_Throw_When_UserTask_Not_Found()
    {
        // Arrange
        var command = CreateCommand(Guid.NewGuid(), Guid.NewGuid());
        _userTaskRepository.Setup(x => x.GetOneByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserTask?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<NotFoundException>().WithMessage("Tarefa não encontrada");
    }

    [Test]
    public void Handle_Should_Throw_When_AssignedUser_Not_Found()
    {
        // Arrange
        var userTask = CreateUserTask(CreateUser());
        var command = CreateCommand(userTask.Id, Guid.NewGuid());

        _userTaskRepository.Setup(x => x.GetOneByIdAsync(userTask.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userTask);

        _userRepository.Setup(x => x.GetOneByIdAsync(command.AssignedUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<NotFoundException>().WithMessage("Usuário atribuído não encontrado");
    }

    [Test]
    public void Handle_Should_Throw_When_Title_Is_Empty()
    {
        // Arrange
        var userTask = CreateUserTask(CreateUser());
        var command = CreateCommand(userTask.Id, Guid.NewGuid());
        command.Title = "";

        _userTaskRepository.Setup(x => x.GetOneByIdAsync(userTask.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userTask);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<ValidationException>().WithMessage("O título da tarefa é obrigatório");
    }

    [Test]
    public void Handle_Should_Throw_When_Description_Is_Empty()
    {
        // Arrange
        var userTask = CreateUserTask(CreateUser());
        var command = CreateCommand(userTask.Id, Guid.NewGuid());
        command.Description = "";

        _userTaskRepository.Setup(x => x.GetOneByIdAsync(userTask.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userTask);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<ValidationException>().WithMessage("A descrição da tarefa é obrigatória");
    }
}
