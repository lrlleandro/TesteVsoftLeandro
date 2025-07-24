using TesteVsoft.Application.Common.ValueObjects;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Common.Models;

public record Select<T, TId> where T : BaseEntity<TId>
{
    public Property<T, TId> PropertyName { get; set; }
}