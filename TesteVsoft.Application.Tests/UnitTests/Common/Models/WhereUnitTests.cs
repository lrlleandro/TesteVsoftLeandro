using FluentAssertions;
using TesteVsoft.Application.Common.Enums;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Common.ValueObjects;
using TesteVsoft.Application.Tests.Fakes;

namespace TesteVsoft.Application.Tests.UnitTests.Common.Models;

public class WhereUnitTests
{
    private Where<FakeEntity, Guid> CreateWhere(string propName, WhereOperationTypes op, object value)
    {
        return new Where<FakeEntity, Guid>
        {
            Property = new Property<FakeEntity, Guid>(propName),
            Operation = op,
            Value = value
        };
    }

    [TestCase(WhereOperationTypes.Equal, 30, 30, true)]
    [TestCase(WhereOperationTypes.Equal, 25, 30, false)]
    [TestCase(WhereOperationTypes.NotEqual, 25, 30, true)]
    [TestCase(WhereOperationTypes.NotEqual, 30, 30, false)]
    [TestCase(WhereOperationTypes.GreaterThanOrEqual, 35, 30, true)]
    [TestCase(WhereOperationTypes.GreaterThanOrEqual, 30, 30, true)]
    [TestCase(WhereOperationTypes.GreaterThanOrEqual, 25, 30, false)]
    [TestCase(WhereOperationTypes.LessThanOrEqual, 25, 30, true)]
    [TestCase(WhereOperationTypes.LessThanOrEqual, 30, 30, true)]
    [TestCase(WhereOperationTypes.LessThanOrEqual, 35, 30, false)]
    [TestCase(WhereOperationTypes.LessThan, 25, 30, true)]
    [TestCase(WhereOperationTypes.LessThan, 30, 30, false)]
    [TestCase(WhereOperationTypes.LessThan, 35, 30, false)]
    public void ToExpression_Should_Work_For_Integer_Operations(WhereOperationTypes operation, int entityAge, int value, bool expected)
    {
        // Arrange
        var where = CreateWhere("Age", operation, value);
        var expr = where.ToExpression();
        var compiled = expr.Compile();
        var entity = new FakeEntity { Age = entityAge };

        // Act
        var result = compiled(entity);

        // Assert
        result.Should().Be(expected);
    }

    [TestCase(WhereOperationTypes.Contains, "Hello", "Hello World", true)]
    [TestCase(WhereOperationTypes.Contains, "World", "Hello World", true)]
    [TestCase(WhereOperationTypes.Contains, "Bye", "Hello World", false)]
    [TestCase(WhereOperationTypes.StartsWith, "Hello", "Hello World", true)]
    [TestCase(WhereOperationTypes.StartsWith, "World", "Hello World", false)]
    [TestCase(WhereOperationTypes.EndsWith, "World", "Hello World", true)]
    [TestCase(WhereOperationTypes.EndsWith, "Hello", "Hello World", false)]
    public void ToExpression_Should_Work_For_String_Operations(WhereOperationTypes operation, string value, string entityName, bool expected)
    {
        // Arrange
        var where = CreateWhere("Name", operation, value);
        var expr = where.ToExpression();
        var compiled = expr.Compile();
        var entity = new FakeEntity { Name = entityName };

        // Act
        var result = compiled(entity);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void ToExpression_Should_Throw_On_Invalid_Operation()
    {
        // Arrange
        var where = CreateWhere("Name", (WhereOperationTypes)999, "test");

        // Act
        Action act = () => where.ToExpression();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void ToExpression_Should_Return_Expression_With_Correct_Parameter_Name()
    {
        // Arrange
        var where = CreateWhere("Name", WhereOperationTypes.Equal, "test");

        // Act
        var expr = where.ToExpression();

        // Assert
        expr.Parameters[0].Name.Should().Be("x");
    }

    [Test]
    public void ToExpression_Should_Compare_Strings_Correctly()
    {
        // Arrange
        var where = CreateWhere("Name", WhereOperationTypes.Equal, "TestName");
        var expr = where.ToExpression();
        var compiled = expr.Compile();

        // Act
        var entityMatch = new FakeEntity { Name = "TestName" };
        var entityNoMatch = new FakeEntity { Name = "OtherName" };

        // Assert
        compiled(entityMatch).Should().BeTrue();
        compiled(entityNoMatch).Should().BeFalse();
    }
}
