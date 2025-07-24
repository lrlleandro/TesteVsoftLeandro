using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Tests.UnitTests.Commands;

[TestFixture]
public class CreateRandomUsersCommandHandlerUnitTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private CreateRandomUsersCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new CreateRandomUsersCommandHandler(_userRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_ShouldCreateUsers_WhenValidCommand()
    {
        // Arrange
        var command = new CreateRandomUsersCommand
        {
            Amount = 10,
            UserNameMask = "test_{{random}}"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<User>>(list => list.Count() == 10), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public async Task Handle_ShouldThrowValidationException_WhenAmountIsInvalidAsync(int amount)
    {
        // Arrange
        var command = new CreateRandomUsersCommand
        {
            Amount = amount,
            UserNameMask = "mask_{{random}}"
        };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("A quantidade deve ser maior que zero.");
    }

    [TestCase("")]
    [TestCase(" ")]
    public async Task Handle_ShouldThrowValidationException_WhenUserNameMaskIsEmptyAsync(string mask)
    {
        // Arrange
        var command = new CreateRandomUsersCommand
        {
            Amount = 10,
            UserNameMask = mask
        };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("A máscara do nome de usuário é obrigatória.");
    }

    public async Task Handle_ShouldThrowValidationException_WhenUserNameMaskPatternNotFound(string mask)
    {
        // Arrange
        var command = new CreateRandomUsersCommand
        {
            Amount = 10,
            UserNameMask = "invalid_mask_{{not_found}}"
        };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("A máscara deve conter o marcador {{random}} para gerar valores aleatórios.");
    }
}
