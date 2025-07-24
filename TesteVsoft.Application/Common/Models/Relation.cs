using System.Linq.Expressions;
using TesteVsoft.Application.Common.ValueObjects;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Common.Models;

public class Relation<T, TId> where T : BaseEntity<TId>
{
    public Property<T, TId> PropertyName { get; set; }

    public Expression<Func<T, object>> ToExpression()
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(parameter, PropertyName);

        Expression body = property.Type.IsValueType
            ? Expression.Convert(property, typeof(object))
            : property;

        return Expression.Lambda<Func<T, object>>(body, parameter);
    }
}