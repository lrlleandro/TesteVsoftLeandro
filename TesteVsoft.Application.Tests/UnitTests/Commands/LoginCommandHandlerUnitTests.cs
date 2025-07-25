using Bogus;
using FluentAssertions;
using Moq;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Application.Interfaces.Security;
using TesteVsoft.Domain.Entities;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace TesteVsoft.Application.Tests.UnitTests.Commands;

public class LoginCommandHandlerUnitTests
{
    private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _faker = new Faker("pt_BR");
    }

    private User CreateUser(string? password = null)
    {
        return User.Create(_faker.Name.FirstName(),
            _faker.Internet.UserName(),
            string.IsNullOrEmpty(password) ? _faker.Internet.Password() : password,
            _faker.Internet.Email());
    }

    private LoginCommandHandler CreateHandler()
    {
        return new LoginCommandHandler(
            _jwtTokenGeneratorMock.Object,
            _userRepositoryMock.Object);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public async Task Handle_Should_Throw_When_Username_Is_Empty(string? userName)
    {
        // Arrange
        var command = new LoginCommand(userName!, "password");
        var handler = CreateHandler();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>().WithMessage("O nome de usuário é obrigatório.");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public async Task Handle_Should_Throw_When_Password_Is_Empty(string? password)
    {
        // Arrange
        var command = new LoginCommand("username", password!);
        var handler = CreateHandler();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>().WithMessage("A senha é obrigatória.");
    }

    [Test]
    public async Task Handle_Should_Throw_When_User_Not_Found()
    {
        // Arrange
        var command = new LoginCommand("user", "pass");

        _userRepositoryMock
            .Setup(x => x.GetOneAsync(It.IsAny<Filter<User, Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Nome de usuário ou senha incorretas");
    }

    [Test]
    public async Task Handle_Should_Throw_When_Password_Is_Invalid()
    {
        // Arrange
        var user = CreateUser();
        var command = new LoginCommand(user.UserName, "wrongpass");
        

        _userRepositoryMock
            .Setup(x => x.GetOneAsync(It.IsAny<Filter<User, Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Nome de usuário ou senha incorretas.");
    }

    [Test]
    public async Task Handle_Should_Return_Token_When_Valid()
    {
        // Arrange
        var user = CreateUser("validpass");
        var command = new LoginCommand(user.UserName, "validpass");
        var expectedToken = new JwtTokenDto("jwt-token", 3600, user.Id, user.UserName, user.Name);

        _userRepositoryMock
            .Setup(x => x.GetOneAsync(It.IsAny<Filter<User, Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateToken(user))
            .Returns(expectedToken);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedToken);
    }
}
