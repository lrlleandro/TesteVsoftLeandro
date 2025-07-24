using FluentAssertions;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Common.ValueObjects;
using TesteVsoft.Application.Tests.Fakes;

namespace TesteVsoft.Application.Tests.UnitTests.Common.Models;

public class RelationUnitTests
{
    [Test]
    public void PropertyName_Should_Be_Assigned_And_Reflect_Simple_Property()
    {
        // Arrange
        var property = new Property<FakeEntity, Guid>("Name");

        // Act
        var relation = new Relation<FakeEntity, Guid> { PropertyName = property };

        // Assert
        relation.PropertyName.Should().NotBeNull();
        relation.PropertyName.Name.Should().Be("Name");
        relation.PropertyName.Type.Should().Be(typeof(string));
        relation.PropertyName.IsNavigation.Should().BeFalse();
    }

    [Test]
    public void PropertyName_Should_Be_Assigned_And_Reflect_Navigation_Property()
    {
        // Arrange
        var property = new Property<FakeEntity, Guid>("Related");

        // Act
        var relation = new Relation<FakeEntity, Guid> { PropertyName = property };

        // Assert
        relation.PropertyName.Should().NotBeNull();
        relation.PropertyName.Name.Should().Be("Related");
        relation.PropertyName.Type.Should().Be(typeof(OtherEntity));
        relation.PropertyName.IsNavigation.Should().BeTrue();
    }

    [Test]
    public void PropertyName_Should_Be_Assigned_And_Reflect_Collection_Navigation_Property()
    {
        // Arrange
        var property = new Property<FakeEntity, Guid>("Children");

        // Act
        var relation = new Relation<FakeEntity, Guid> { PropertyName = property };

        // Assert
        relation.PropertyName.Should().NotBeNull();
        relation.PropertyName.Name.Should().Be("Children");
        relation.PropertyName.IsNavigation.Should().BeTrue();
    }

    [Test]
    public void PropertyName_Can_Be_Implicitly_Assigned_From_String()
    {
        // Arrange & Act
        Relation<FakeEntity, Guid> relation = new Relation<FakeEntity, Guid>
        {
            PropertyName = "Name"
        };

        // Assert
        relation.PropertyName.Name.Should().Be("Name");
    }

    [Test]
    public void PropertyName_Can_Be_Implicitly_Converted_To_String()
    {
        // Arrange
        var relation = new Relation<FakeEntity, Guid>
        {
            PropertyName = "Name"
        };

        // Act
        string propName = relation.PropertyName;

        // Assert
        propName.Should().Be("Name");
    }
}
