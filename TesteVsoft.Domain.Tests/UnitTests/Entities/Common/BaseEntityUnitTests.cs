using FluentAssertions;
using TesteVsoft.Domain.Tests.Fakes;

namespace TesteVsoft.Domain.Tests.UnitTests.Entities.Common;

public partial class BaseEntityUnitTests
{

    [Test]
    public void Constructor_Should_Generate_Guid_When_TId_Is_Guid_And_No_Id_Provided()
    {
        // Act
        var entity = new GuidEntity();

        // Assert
        entity.Id.Should().NotBeEmpty();
    }

    [Test]
    public void Constructor_Should_Assign_Guid_Id_When_Provided()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var entity = new GuidEntity(id);

        // Assert
        entity.Id.Should().Be(id);
    }

    [Test]
    public void Constructor_Should_Assign_Int_Id_When_Provided()
    {
        // Arrange
        var id = 123;

        // Act
        var entity = new IntEntity(id);

        // Assert
        entity.Id.Should().Be(id);
    }

    [Test]
    public void Constructor_Should_Not_Assign_Int_Id_When_Not_Provided()
    {
        // Act
        var entity = new IntEntity();

        // Assert
        entity.Id.Should().Be(0);
    }
}
