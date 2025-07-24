using FluentAssertions;
using TesteVsoft.Application.Common.Exceptions;

namespace TesteVsoft.Application.Tests.UnitTests.Common.Exceptions;

[TestFixture]
public class NotFoundExceptionUnitTests
{
    [Test]
    public void DefaultConstructor_ShouldCreateInstance()
    {
        // Act
        var exception = new NotFoundException();

        // Assert
        exception.Should().BeOfType<NotFoundException>();
        exception.Message.Should().NotBeNull();
    }

    [Test]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        var message = "Recurso não encontrado";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Test]
    public void Constructor_WithMessageAndInnerException_ShouldSetProperties()
    {
        // Arrange
        var message = "Erro com causa interna";
        var inner = new InvalidOperationException("Detalhe da exceção interna");

        // Act
        var exception = new NotFoundException(message, inner);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(inner);
    }
}
