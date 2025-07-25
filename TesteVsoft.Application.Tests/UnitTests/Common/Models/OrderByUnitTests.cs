using FluentAssertions;
using TesteVsoft.Application.Common.Enums;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Common.ValueObjects;
using TesteVsoft.Application.Tests.Fakes;

namespace TesteVsoft.Application.Tests.UnitTests.Common.Models;

public class OrderByUnitTests
{
    [Test]
    public void Constructor_Should_Set_Properties()
    {
        // Arrange
        var property = new Property<FakeEntity, Guid>("Name");

        // Act
        var orderBy = new OrderBy<FakeEntity, Guid>
        {
            PropertyName = property,
            Direction = OrderDirectionTypes.Ascending
        };

        // Assert
        orderBy.PropertyName.Should().Be(property);
        orderBy.Direction.Should().Be(OrderDirectionTypes.Ascending);
    }

    [Test]
    public void ToExpression_Should_Return_Correct_Expression_For_Name()
    {
        // Arrange
        var entity = new FakeEntity { Name = "TestName" };
        var orderBy = new OrderBy<FakeEntity, Guid>
        {
            PropertyName = new Property<FakeEntity, Guid>("Name"),
            Direction = OrderDirectionTypes.Descending
        };

        // Act
        var expr = orderBy.ToExpression();
        var compiled = expr.Compile();
        var value = compiled(entity);

        // Assert
        expr.Should().NotBeNull();
        value.Should().Be("TestName");
    }

    [Test]
    public void ToExpression_Should_Return_Correct_Expression_For_Age()
    {
        // Arrange
        var entity = new FakeEntity { Age = 42 };
        var orderBy = new OrderBy<FakeEntity, Guid>
        {
            PropertyName = new Property<FakeEntity, Guid>("Age"),
            Direction = OrderDirectionTypes.Ascending
        };

        // Act
        var expr = orderBy.ToExpression();
        var compiled = expr.Compile();
        var result = compiled(entity);

        // Assert
        result.Should().Be(42);
    }
}