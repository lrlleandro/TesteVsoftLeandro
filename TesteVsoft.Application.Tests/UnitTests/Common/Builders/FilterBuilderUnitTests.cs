using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using TesteVsoft.Application.Common.Builders;
using TesteVsoft.Application.Common.Enums;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Tests.Fakes;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Tests.UnitTests.Common.Builders;

public class FilterBuilderUnitTests
{
    [Test]
    public void FilterBuilder_DefaultValues_ShouldSetPageAndPageSize()
    {
        // Arrange & Act
        var builder = new FilterBuilder<FakeEntity, Guid>();
        var filter = builder.Build();

        // Assert
        filter.Page.Should().Be(1);
        filter.PageSize.Should().Be(10);
    }

    [Test]
    public void WithPage_ShouldSetPage()
    {
        // Arrange
        var builder = new FilterBuilder<FakeEntity, Guid>();

        // Act
        var filter = builder.WithPage(5).Build();

        // Assert
        filter.Page.Should().Be(5);
    }

    [Test]
    public void WithPageSize_ShouldSetPageSize()
    {
        // Arrange
        var builder = new FilterBuilder<FakeEntity, Guid>();

        // Act
        var filter = builder.WithPageSize(25).Build();

        // Assert
        filter.PageSize.Should().Be(25);
    }

    [Test]
    public void AddSelect_ShouldAddToSelects()
    {
        // Arrange
        var builder = new FilterBuilder<FakeEntity, Guid>();
        var select = new Select<FakeEntity, Guid>();

        // Act
        var filter = builder.AddSelect(select).Build();

        // Assert
        filter.Selects.Should().ContainSingle().Which.Should().Be(select);
    }

    [Test]
    public void AddRelation_ShouldAddRelationWithCorrectPropertyName()
    {
        // Arrange
        var builder = new FilterBuilder<FakeEntity, Guid>();

        // Act
        var filter = builder.AddRelation(x => x.Name).Build();

        // Assert
        filter.WithRelations.Should().ContainSingle();
        filter.WithRelations[0].PropertyName.Name.Should().Be(nameof(FakeEntity.Name));
    }

    [Test]
    public void AddWhere_ShouldAddWhereWithCorrectValues()
    {
        // Arrange
        var builder = new FilterBuilder<FakeEntity, Guid>();

        // Act
        var filter = builder.AddWhere(x => x.Name, WhereOperationTypes.Contains, "test").Build();

        // Assert
        filter.Wheres.Should().ContainSingle();
        var where = filter.Wheres[0];
        where.Operation.Should().Be(WhereOperationTypes.Contains);
        where.Value.Should().Be("test");
        where.Property.Name.Should().Be(nameof(FakeEntity.Name));
    }

    [Test]
    public void AddOrderBy_ShouldAddCorrectOrder()
    {
        // Arrange
        var builder = new FilterBuilder<FakeEntity, Guid>();

        // Act
        var filter = builder.AddOrderBy(x => x.Age, OrderDirectionTypes.Descending).Build();
        var order = filter.OrderBys[0];

        // Assert
        filter.OrderBys.Should().ContainSingle();        
        order.PropertyName.Name.Should().Be(nameof(FakeEntity.Age));
        order.Direction.Should().Be(OrderDirectionTypes.Descending);
    }

    [Test]
    public void GetPropertyName_ShouldThrowOnInvalidExpression()
    {
        // Arrange
        var builder = new FilterBuilder<FakeEntity, Guid>();
        Expression<Func<FakeEntity, object>> expr = x => x.GetHashCode(); // método, não propriedade

        // Act
        Action act = () => builder.AddRelation(expr);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Invalid expression");
    }
}
