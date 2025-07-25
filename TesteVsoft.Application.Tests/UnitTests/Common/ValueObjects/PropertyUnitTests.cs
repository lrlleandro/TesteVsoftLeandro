using FluentAssertions;
using NUnit.Framework.Internal;
using System.ComponentModel.DataAnnotations;
using TesteVsoft.Application.Common.ValueObjects;
using TesteVsoft.Application.Tests.Fakes;

namespace TesteVsoft.Application.Tests.UnitTests.Common.ValueObjects;

public partial class PropertyUnitTests
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void Constructor_Should_Throw_When_PropertyName_Is_Null_Or_Empty(string? propertyName)
    {
        // Arrande & Act
        var act = () => new Property<FakeEntity, Guid>(propertyName!);

        // Assert
        act.Should().Throw<ValidationException>().WithMessage("O nome da propriedade é obrigatório");
    }

    [Test]
    public void Constructor_Should_Throw_When_Property_Does_Not_Exist()
    {
        // Arrande & Act
        var act = () => new Property<FakeEntity, Guid>("NonExistent");

        // Assert
        act.Should().Throw<ValidationException>()
           .WithMessage($"A propriedade 'NonExistent' não existe no tipo '{typeof(FakeEntity).Name}'");
    }

    [Test]
    public void Constructor_Should_Set_Properties_Correctly_For_Scalar()
    {
        // Arrande & Act
        var prop = new Property<FakeEntity, Guid>("Age");

        // Assert
        prop.Name.Should().Be("Age");
        prop.Type.Should().Be(typeof(int));
        prop.Value.Should().Be("Age");
        prop.IsNavigation.Should().BeFalse();
    }

    [Test]
    public void Constructor_Should_Set_IsNavigation_True_For_Class_Property()
    {
        // Arrande & Act
        var prop = new Property<FakeEntity, Guid>("Related");

        // Assert
        prop.IsNavigation.Should().BeTrue();
    }

    [Test]
    public void Constructor_Should_Set_IsNavigation_True_For_Collection_Of_Class()
    {
        // Arrande & Act
        var prop = new Property<FakeEntity, Guid>("Children");

        // Assert
        prop.IsNavigation.Should().BeTrue();
    }

    [Test]
    public void Implicit_Conversion_To_String_Should_Return_Value()
    {
        // Arrange
        Property<FakeEntity, Guid> prop = new("Name");

        // Act
        string value = prop;

        // Assert
        value.Should().Be("Name");
    }

    [Test]
    public void Implicit_Conversion_From_String_Should_Create_Property()
    {
        // Arrande & Act
        Property<FakeEntity, Guid> prop = "Name";

        // Assert
        prop.Name.Should().Be("Name");
    }

    [Test]
    public void Equals_Should_Return_True_When_Values_Match()
    {
        // Arrande & Act
        Property<FakeEntity, Guid> prop1 = "Name";
        Property<FakeEntity, Guid> prop2 = "Name";

        // Assert
        prop1.Equals(prop2).Should().BeTrue();
    }

    [Test]
    public void Equals_Should_Return_False_When_Values_Differ()
    {
        // Arrande & Act
        Property<FakeEntity, Guid> prop1 = "Name";
        Property<FakeEntity, Guid> prop2 = "Age";

        // Assert
        prop1.Equals(prop2).Should().BeFalse();
    }

    [Test]
    public void GetHashCode_Should_Match_Value_HashCode()
    {
        // Arrande & Act
        Property<FakeEntity, Guid> prop = "Name";

        // Assert
        prop.GetHashCode().Should().Be("Name".GetHashCode());
    }

    [Test]
    public void ToString_Should_Return_Value()
    {
        // Arrande & Act
        var prop = new Property<FakeEntity, Guid>("Age");

        // Assert
        prop.ToString().Should().Be("Age");
    }
}
