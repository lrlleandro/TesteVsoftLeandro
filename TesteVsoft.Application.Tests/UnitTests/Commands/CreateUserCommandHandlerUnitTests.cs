using Bogus;
using FluentAssertions;
using Moq;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace TesteVsoft.Application.Tests.UnitTests.Commands;

public class CreateUserCommandHandlerUnitTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private CreateUserCommandHandler _handler;
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _faker = new Faker("pt_BR");
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new CreateUserCommandHandler(_userRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_Should_CreateUser_When_UsernameIsUnique()
    {
        // Arrange
        var command = new CreateUserCommand(_faker.Person.FirstName,
            _faker.Internet.UserName(),
            _faker.Internet.Password(),
            _faker.Internet.Email());

        var user = User.Create(command.Name, command.UserName, command.Password, command.Email);

        _userRepositoryMock.Setup(r => r.GetOneAsync(It.IsAny<Filter<User, Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(user));

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.Name.Should().Be(command.Name);
        result.UserName.Should().Be(command.UserName);
        result.Password.Should().Be(user.Password);
        result.Email.Should().Be(command.Email);

        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_Should_ReturnError_When_UsernameAlreadyExists()
    {
        // Arrange
        var command = new CreateUserCommand(_faker.Person.FirstName,
            _faker.Internet.UserName(),
            _faker.Internet.Password(),
            _faker.Internet.Email());

        var user = User.Create(command.Name, command.UserName, command.Password, command.Email);

        _userRepositoryMock.Setup(r => r.GetOneAsync(It.IsAny<Filter<User, Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var act = async () => await _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage($"O nome de usuário '{command.UserName}' já está em uso.");
    }
}