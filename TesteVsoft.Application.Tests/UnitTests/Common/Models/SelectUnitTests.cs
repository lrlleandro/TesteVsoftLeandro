using FluentAssertions;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Common.ValueObjects;
using TesteVsoft.Application.Tests.Fakes;

namespace TesteVsoft.Application.Tests.UnitTests.Common.Models;

public class SelectUnitTests
{
    [Test]
    public void Constructor_Should_Set_PropertyName()
    {
        // Arrange
        var property = new Property<FakeEntity, Guid>("Name");

        // Act
        var select = new Select<FakeEntity, Guid> { PropertyName = property };

        // Assert
        select.PropertyName.Should().Be(property);
    }

    [Test]
    public void Records_With_Same_Values_Should_Be_Equal()
    {
        // Arrange
        var prop = new Property<FakeEntity, Guid>("Name");

        // Act
        var s1 = new Select<FakeEntity, Guid> { PropertyName = prop };
        var s2 = new Select<FakeEntity, Guid> { PropertyName = prop };

        // Assert
        s1.Should().Be(s2);
        (s1 == s2).Should().BeTrue();
        (s1 != s2).Should().BeFalse();
        s1.Equals(s2).Should().BeTrue();
        s1.GetHashCode().Should().Be(s2.GetHashCode());
    }

    [Test]
    public void Records_With_Different_Values_Should_Not_Be_Equal()
    {
        // Arrange & Act
        var s1 = new Select<FakeEntity, Guid> { PropertyName = new Property<FakeEntity, Guid>("Name") };
        var s2 = new Select<FakeEntity, Guid> { PropertyName = new Property<FakeEntity, Guid>("Age") };

        // Assert
        s1.Should().NotBe(s2);
        (s1 == s2).Should().BeFalse();
        (s1 != s2).Should().BeTrue();
        s1.Equals(s2).Should().BeFalse();
    }

    [Test]
    public void ToString_Should_Return_Correct_Format()
    {
        // Arrange
        var prop = new Property<FakeEntity, Guid>("Name");

        // Act
        var select = new Select<FakeEntity, Guid> { PropertyName = prop };

        // Assert
        select.ToString().Should().Contain("PropertyName = Name");
    }
}
