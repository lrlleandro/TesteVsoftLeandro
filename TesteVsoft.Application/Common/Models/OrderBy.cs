using System.Linq.Expressions;
using TesteVsoft.Application.Common.Enums;
using TesteVsoft.Application.Common.ValueObjects;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Common.Models;

public class OrderBy<T, TId> where T : BaseEntity<TId>
{
    public Property<T, TId> PropertyName { get; set; }
    public OrderDirectionTypes Direction { get; set; }

    public Expression<Func<T, object>> ToExpression()
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, PropertyName.Name);
        var convert = Expression.Convert(property, typeof(object));
        return Expression.Lambda<Func<T, object>>(convert, parameter);
    }
}