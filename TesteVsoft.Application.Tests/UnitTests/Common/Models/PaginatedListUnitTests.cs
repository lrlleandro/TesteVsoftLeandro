using FluentAssertions;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Tests.Fakes;

namespace TesteVsoft.Application.Tests.UnitTests.Common.Models;

public class PaginatedListUnitTests
{
    [Test]
    public void Constructor_Should_Set_Properties_Correctly_When_PageSize_Is_Perfect_Division()
    {
        // Arrange
        var items = new List<FakeEntity>
        {
            new FakeEntity { Name = "Item1" },
            new FakeEntity { Name = "Item2" }
        };
        int totalCount = 4;
        int page = 1;
        int pageSize = 2;

        // Act
        var paginatedList = new PaginatedList<FakeEntity, Guid>(items, totalCount, page, pageSize);

        // Assert
        paginatedList.Page.Should().Be(page);
        paginatedList.TotalCount.Should().Be(totalCount);
        paginatedList.TotalPages.Should().Be(2);
        paginatedList.Items.Should().BeEquivalentTo(items);
    }

    [Test]
    public void Constructor_Should_Set_TotalPages_With_Ceiling_When_Not_Perfect_Division()
    {
        // Arrange
        var items = new List<FakeEntity>
        {
            new FakeEntity { Name = "Item1" },
            new FakeEntity { Name = "Item2" }
        };
        int totalCount = 5;
        int page = 1;
        int pageSize = 2;

        // Act
        var paginatedList = new PaginatedList<FakeEntity, Guid>(items, totalCount, page, pageSize);

        // Assert
        paginatedList.TotalPages.Should().Be(3);
    }

    [Test]
    public void Constructor_Should_Allow_Empty_Item_List()
    {
        // Arrange
        var items = new List<FakeEntity>();
        int totalCount = 0;
        int page = 1;
        int pageSize = 10;

        // Act
        var paginatedList = new PaginatedList<FakeEntity, Guid>(items, totalCount, page, pageSize);

        // Assert
        paginatedList.Page.Should().Be(page);
        paginatedList.TotalCount.Should().Be(0);
        paginatedList.TotalPages.Should().Be(0);
        paginatedList.Items.Should().BeEmpty();
    }

    [Test]
    public void Constructor_Should_Handle_PageSize_One()
    {
        // Arrange
        var items = new List<FakeEntity>
        {
            new FakeEntity { Name = "Single" }
        };
        int totalCount = 1;
        int page = 1;
        int pageSize = 1;

        // Act
        var paginatedList = new PaginatedList<FakeEntity, Guid>(items, totalCount, page, pageSize);

        // Assert
        paginatedList.TotalPages.Should().Be(1);
        paginatedList.Items.Should().ContainSingle()
            .Which.Name.Should().Be("Single");
    }

    [Test]
    public void Constructor_Should_Handle_Large_PageSize()
    {
        // Arrange
        var items = new List<FakeEntity>
        {
            new FakeEntity { Name = "OnlyOne" }
        };
        int totalCount = 1;
        int page = 1;
        int pageSize = 1000;

        // Act
        var paginatedList = new PaginatedList<FakeEntity, Guid>(items, totalCount, page, pageSize);

        // Assert
        paginatedList.TotalPages.Should().Be(1);
        paginatedList.Items.Should().HaveCount(1);
    }
}
