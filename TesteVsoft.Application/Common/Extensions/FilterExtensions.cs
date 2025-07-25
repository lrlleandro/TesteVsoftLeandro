using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Common.Extensions;

public static class FilterExtensions
{
    public static FilterDto ToFilterDto<T, TId>(this Filter<T, TId> filter) where T : BaseEntity<TId>
    {
        if (filter is null)
        {
            throw new ArgumentNullException(nameof(filter), "O filtro não pode ser nulo");
        }

        var dto = new FilterDto
        {
            Page = filter.Page,
            PageSize = filter.PageSize,
            OrderBys = filter.OrderBys.Select(order => new OrderByDto
            {
                PropertyName = order.PropertyName,
                Direction = order.Direction
            }).ToList(),

            Selects = filter.Selects.Select(s => s.PropertyName.Name).ToList(),

            Relations = filter.WithRelations.Select(r => r.PropertyName.Name).ToList(),

            Wheres = filter.Wheres.Select(w => new WhereDto
            {
                PropertyName = w.Property.Name,
                Operation = w.Operation,
                Value = w.Value.ToString() ?? string.Empty
            }).ToList()
        };

        return dto;
    }
}
