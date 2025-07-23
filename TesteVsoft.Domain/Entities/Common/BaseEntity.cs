namespace TesteVsoft.Domain.Entities.Common;

public abstract class BaseEntity<TId>
{
    public TId? Id { get; protected set; }

    protected BaseEntity()
    {
        if (typeof(TId) == typeof(Guid))
        {
            Id = (TId)(object)Guid.NewGuid();
        }
    }

    protected BaseEntity(TId id)
    {
        Id = id;
    }
}