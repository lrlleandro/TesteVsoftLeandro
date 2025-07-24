using Bogus;
using FluentAssertions;
using Moq;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Common.Exceptions;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Tests.UnitTests.Commands;

[TestFixture]
public class UpdateUserCommandHandlerUnitTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private UpdateUserCommandHandler _handler = null!;
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _faker = new Faker("pt_BR");
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new UpdateUserCommandHandler(_userRepositoryMock.Object);
    }

    [Test]
    public void Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = _faker.Random.Guid(),
            UserName = _faker.Internet.UserName(),
            OldPassword = _faker.Internet.Password(),
            NewPassword = _faker.Internet.Password()
        };

        _userRepositoryMock
            .Setup(r => r.GetOneByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<NotFoundException>()
           .WithMessage($"Usuário {command.UserName} não encontrado.");
    }

    [Test]
    public async Task Handle_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var oldName = _faker.Person.FullName;
        var oldUserName = _faker.Internet.UserName();
        var oldPassword = _faker.Internet.Password();
        var oldEmail = _faker.Internet.Email();

        var user = User.Create(oldName, oldUserName, oldPassword, oldEmail);

        var command = new UpdateUserCommand
        {
            Id = user.Id,
            Name = _faker.Name.FullName(),
            UserName = _faker.Internet.UserName(),
            OldPassword = oldPassword,
            NewPassword = _faker.Internet.Password(),
            Email = _faker.Internet.Email()
        };

        _userRepositoryMock
            .Setup(r => r.GetOneByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.Name.Should().NotBe(oldName);
        user.UserName.Should().NotBe(oldUserName);
        user.Email.Should().NotBe(oldEmail);
        BCrypt.Net.BCrypt.Verify(oldPassword, user.Password).Should().BeFalse();
        BCrypt.Net.BCrypt.Verify(command.NewPassword, user.Password).Should().BeTrue();
        _userRepositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}