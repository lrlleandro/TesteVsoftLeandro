using System.ComponentModel.DataAnnotations;
using System.Reflection;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Common.ValueObjects;

public class Property<T, TId> where T : BaseEntity<TId>
{
    public Property(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ValidationException("O nome da propriedade é obrigatório");
        }

        var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

        if (propertyInfo is null)
        {
            propertyInfo = typeof(T).GetProperties()
                                    .FirstOrDefault(p => p.Name.ToLower() == propertyName.ToLower());

            if (propertyInfo is null)
            {
                throw new ValidationException($"A propriedade '{propertyName}' não existe no tipo '{typeof(T).Name}'");
            }
        }

        var name = propertyInfo.Name;
        var type = propertyInfo.PropertyType;
        var isNavigation = false;

        if (type.IsClass && type != typeof(string))
        {
            isNavigation = true;
        }
        else if (type.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
        {
            var genericArguments = type.GetGenericArguments();
            if (genericArguments.Length == 1 && genericArguments[0].IsClass && genericArguments[0] != typeof(string))
            {
                isNavigation = true;
            }
        }

        Value = propertyName;
        Name = name;
        Type = type;
        IsNavigation = isNavigation;
    }

    public string Value { get; private set; }
    public string Name { get; private set; }
    public Type Type { get; private set; }
    public bool IsNavigation { get; private set; }

    public static implicit operator string(Property<T, TId> propertyName) => propertyName.Value;
    public static implicit operator Property<T, TId>(string propertyName) => new Property<T, TId>(propertyName);

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (Property<T, TId>)obj;
        return Value.Equals(other.Value);
    }

    public override int GetHashCode() => Value.GetHashCode();
}