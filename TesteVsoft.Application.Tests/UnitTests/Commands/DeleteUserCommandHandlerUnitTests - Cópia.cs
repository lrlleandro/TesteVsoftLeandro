using Bogus;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Common.Exceptions;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Tests.UnitTests.Commands;

[TestFixture]
public class DeleteUserTaskCommandHandlerUnitTests
{
    private Mock<IUserTaskRepository> _usertaskRepositoryMock = null!;
    private DeleteUserTaskCommandHandler _handler = null!;
    private Faker _faker = null!;

    [SetUp]
    public void SetUp()
    {
        _usertaskRepositoryMock = new Mock<IUserTaskRepository>();
        _handler = new DeleteUserTaskCommandHandler(_usertaskRepositoryMock.Object);
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
    public async Task Handle_ShouldDeleteUserTask_WhenUserTaskExists()
    {
        // Arrange
        var usertask = UserTask.Create(
            title: _faker.Lorem.Word(),
            description: _faker.Lorem.Paragraph(),
            dueDate: DateTime.UtcNow.AddDays(3),
            assignedUser: CreateUser());

        var command = new DeleteUserTaskCommand(usertask.Id);

        _usertaskRepositoryMock
            .Setup(r => r.GetOneByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usertask);

        _usertaskRepositoryMock
            .Setup(r => r.RemoveAsync(usertask, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _usertaskRepositoryMock.Verify(r => r.RemoveAsync(usertask, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void Handle_ShouldThrowNotFoundException_WhenUserTaskDoesNotExist()
    {
        // Arrange
        var usertaskId = Guid.NewGuid();
        var command = new DeleteUserTaskCommand(usertaskId);

        _usertaskRepositoryMock
            .Setup(r => r.GetOneByIdAsync(usertaskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserTask?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<NotFoundException>()
           .WithMessage($"Usuário não encontrado.");

        _usertaskRepositoryMock.Verify(r => r.RemoveAsync(It.IsAny<UserTask>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
