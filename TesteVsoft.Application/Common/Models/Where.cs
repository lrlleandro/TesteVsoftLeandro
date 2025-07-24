using System.Linq.Expressions;
using TesteVsoft.Application.Common.Enums;
using TesteVsoft.Application.Common.ValueObjects;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Common.Models;

public class Where<T, TId> where T : BaseEntity<TId>
{
    public Property<T, TId> Property { get; set; }
    public WhereOperationTypes Operation { get; set; }
    public object Value { get; set; }

    public Expression<Func<T, bool>> ToExpression()
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, Property.Name);
        var constant = Expression.Constant(Value);

        Expression body;
        switch (Operation)
        {
            case WhereOperationTypes.Equal:
                body = Expression.Equal(property, constant);
                break;
            case WhereOperationTypes.NotEqual:
                body = Expression.NotEqual(property, constant);
                break;
            case WhereOperationTypes.GreaterThanOrEqual:
                body = Expression.GreaterThanOrEqual(property, constant);
                break;
            case WhereOperationTypes.LessThanOrEqual:
                body = Expression.LessThanOrEqual(property, constant);
                break;
            case WhereOperationTypes.LessThan:
                body = Expression.LessThan(property, constant);
                break;
            case WhereOperationTypes.Contains:
                body = Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant);
                break;
            case WhereOperationTypes.StartsWith:
                body = Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), constant);
                break;
            case WhereOperationTypes.EndsWith:
                body = Expression.Call(property, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), constant);
                break;
            default:
                throw new ArgumentOutOfRangeException("Argumento fora do intervalo");
        }

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}