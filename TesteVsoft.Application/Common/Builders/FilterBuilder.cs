using System.Linq.Expressions;
using TesteVsoft.Application.Common.Enums;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Common.ValueObjects;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Common.Builders;

public class FilterBuilder<T, TId> where T : BaseEntity<TId>
{
    private readonly Filter<T, TId> _filter;

    public FilterBuilder()
    {
        _filter = new Filter<T, TId>
        {
            Page = 1,
            PageSize = 10
        };
    }

    public FilterBuilder<T, TId> WithPage(int page)
    {
        _filter.Page = page;
        return this;
    }

    public FilterBuilder<T, TId> WithPageSize(int pageSize)
    {
        _filter.PageSize = pageSize;
        return this;
    }

    public FilterBuilder<T, TId> AddSelect(Select<T, TId> select)
    {
        _filter.Selects.Add(select);
        return this;
    }

    public FilterBuilder<T, TId> AddRelation(Expression<Func<T, object>> expression)
    {
        _filter.WithRelations.Add(new Relation<T, TId>
        {
            PropertyName = new Property<T, TId>(GetPropertyName(expression)).Value
        });
        return this;
    }

    public FilterBuilder<T, TId> AddRelation(Property<T, TId> property)
    {
        _filter.WithRelations.Add(new Relation<T, TId>
        {
            PropertyName = property
        });
        return this;
    }

    public FilterBuilder<T, TId> AddWhere(Expression<Func<T, object>> property, WhereOperationTypes operation, object value)
    {
        _filter.Wheres.Add(new Where<T, TId>
        {
            Operation = operation,
            Value = value,
            Property = new Property<T, TId>(GetPropertyName(property)).Value
        });

        return this;
    }

    public FilterBuilder<T, TId> AddWhere(Property<T, TId> property, WhereOperationTypes operation, object value)
    {
        _filter.Wheres.Add(new Where<T, TId>
        {
            Operation = operation,
            Value = value,
            Property = property
        });

        return this;
    }

    private static string GetPropertyName(Expression<Func<T, object>> expression)
    {
        if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression member1)
            return member1.Member.Name;

        if (expression.Body is MemberExpression member2)
            return member2.Member.Name;

        throw new ArgumentException("Invalid expression");
    }

    public FilterBuilder<T, TId> AddOrderBy(Expression<Func<T, object>> expression, OrderDirectionTypes direction)
    {
        _filter.OrderBys.Add(new OrderBy<T, TId>
        {
            PropertyName = GetPropertyName(expression),
            Direction = direction
        });

        return this;
    }

    public FilterBuilder<T, TId> AddOrderBy(Property<T, TId> property, OrderDirectionTypes direction)
    {
        _filter.OrderBys.Add(new OrderBy<T, TId>
        {
            PropertyName = property,
            Direction = direction
        });

        return this;
    }

    public Filter<T, TId> Build()
    {
        return _filter;
    }
}
