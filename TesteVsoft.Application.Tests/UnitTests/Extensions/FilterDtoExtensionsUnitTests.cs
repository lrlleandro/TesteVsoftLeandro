using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TesteVsoft.Application.Common.Enums;
using TesteVsoft.Application.Common.Extensions;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Tests.Fakes;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Tests.UnitTests.Extensions;

[TestFixture]
public class FilterDtoExtensionsUnitTests
{
    [Test]
    public void ToFilter_WithNullFilterDto_ShouldThrowArgumentNullException()
    {
        // Arrange
        FilterDto filterDto = null!;

        // Act
        Action act = () => filterDto.ToFilter<FakeEntity, Guid>();

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithMessage("Os filtros não podem ser nulos*");
    }

    [Test]
    public void ToFilter_WithEmptyFilterDto_ShouldReturnFilterWithDefaults()
    {
        // Arrange
        var dto = new FilterDto();

        // Act
        var result = dto.ToFilter<FakeEntity, Guid>();

        // Assert
        result.Should().NotBeNull();
        result.Page.Should().Be(0);
        result.PageSize.Should().Be(0);
        result.OrderBys.Should().BeEmpty();
        result.Selects.Should().BeEmpty();
        result.WithRelations.Should().BeEmpty();
        result.Wheres.Should().BeEmpty();
    }

    [Test]
    public void ToFilter_WithFullFilterDto_ShouldReturnConfiguredFilter()
    {
        // Arrange
        var dto = new FilterDto
        {
            Page = 2,
            PageSize = 20,
            OrderBys = new List<OrderByDto>
            {
                new OrderByDto 
                { 
                    PropertyName = "Name",
                    Direction = OrderDirectionTypes.Descending 
                }
            },
            Selects = new List<string> { "Name" },
            Relations = new List<string> { "Related" },
            Wheres = new List<WhereDto>
            {
                new WhereDto 
                { 
                    PropertyName = "Age", 
                    Operation = WhereOperationTypes.Equal, 
                    Value = "10"
                }
            }
        };

        // Act
        var result = dto.ToFilter<FakeEntity, Guid>();

        // Assert
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(20);
        result.OrderBys.Should().ContainSingle(o => o.PropertyName == "Name" && o.Direction == OrderDirectionTypes.Descending);
        result.Selects.Should().ContainSingle(s => s.PropertyName == "Name");
        result.WithRelations.Should().ContainSingle(r => r.PropertyName == "Related");
        result.Wheres.Should().ContainSingle(w => w.Property == "Age" && w.Operation == WhereOperationTypes.Equal && w.Value.Equals("10"));
    }
}
