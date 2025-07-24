using TesteVsoft.Application.Common.Builders;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Common.ValueObjects;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Common.Extensions;

public static class FilterDtoExtensions
{
    public static Filter<T, TId> ToFilter<T, TId>(this FilterDto filterDto) where T : BaseEntity<TId>
    {
        if (filterDto is null)
        {
            throw new ArgumentNullException(nameof(filterDto), "Os filtros não podem ser nulos");
        }

        var filterBuilder = new FilterBuilder<T, TId>()
            .WithPage(filterDto.Page)
            .WithPageSize(filterDto.PageSize);

        foreach (var orderBy in filterDto?.OrderBys ?? [])
        {
            filterBuilder.AddOrderBy(new Property<T, TId>(orderBy.PropertyName).Value, orderBy.Direction);
        }

        foreach (var select in filterDto?.Selects ?? [])
        {
            filterBuilder.AddSelect(new Select<T, TId>
            {
                PropertyName = new Property<T, TId>(select).Value
            });
        }

        foreach (var relation in filterDto?.Relations ?? [])
        {
            filterBuilder.AddRelation(new Property<T, TId>(relation).Value);
        }

        foreach (var where in filterDto?.Wheres ?? [])
        {
            filterBuilder.AddWhere(new Property<T, TId>(where.PropertyName).Value, where.Operation, where.Value);
        }

        return filterBuilder.Build();
    }
}
