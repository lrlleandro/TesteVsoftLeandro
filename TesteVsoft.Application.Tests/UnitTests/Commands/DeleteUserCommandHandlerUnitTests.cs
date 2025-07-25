using FluentAssertions;
using Moq;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Common.Exceptions;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Tests.UnitTests.Commands;

[TestFixture]
public class DeleteUserCommandHandlerUnitTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private DeleteUserCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new DeleteUserCommandHandler(_userRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var user = User.CreateRandom("user_{{random}}");
        var command = new DeleteUserCommand(user.Id);

        _userRepositoryMock
            .Setup(r => r.GetOneByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(r => r.RemoveAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _userRepositoryMock.Verify(r => r.RemoveAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);

        _userRepositoryMock
            .Setup(r => r.GetOneByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<NotFoundException>()
           .WithMessage($"Usuário não encontrado.");

        _userRepositoryMock.Verify(r => r.RemoveAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
