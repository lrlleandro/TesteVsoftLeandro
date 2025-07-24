using FluentAssertions;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Tests.Fakes;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Tests.UnitTests.Common.Models;

public class FilterUnitTests
{
    [Test]
    public void Filter_DefaultConstructor_ShouldInitializeEmptyLists()
    {
        // Arrange & Act
        var filter = new Filter<FakeEntity, Guid>();

        // Assert
        filter.Selects.Should().NotBeNull();
        filter.Selects.Should().BeEmpty();

        filter.WithRelations.Should().NotBeNull();
        filter.WithRelations.Should().BeEmpty();

        filter.Wheres.Should().NotBeNull();
        filter.Wheres.Should().BeEmpty();

        filter.OrderBys.Should().NotBeNull();
        filter.OrderBys.Should().BeEmpty();
    }

    [Test]
    public void Filter_SetProperties_ShouldAssignCorrectly()
    {
        // Arrange
        var selects = new List<Select<FakeEntity, Guid>> { new() };
        var relations = new List<Relation<FakeEntity, Guid>> { new() };
        var wheres = new List<Where<FakeEntity, Guid>> { new() };
        var orderBys = new List<OrderBy<FakeEntity, Guid>> { new() };

        var page = 2;
        var pageSize = 50;

        // Act
        var filter = new Filter<FakeEntity, Guid>
        {
            Page = page,
            PageSize = pageSize,
            Selects = selects,
            WithRelations = relations,
            Wheres = wheres,
            OrderBys = orderBys
        };

        // Assert
        filter.Page.Should().Be(page);
        filter.PageSize.Should().Be(pageSize);
        filter.Selects.Should().BeEquivalentTo(selects);
        filter.WithRelations.Should().BeEquivalentTo(relations);
        filter.Wheres.Should().BeEquivalentTo(wheres);
        filter.OrderBys.Should().BeEquivalentTo(orderBys);
    }
}
