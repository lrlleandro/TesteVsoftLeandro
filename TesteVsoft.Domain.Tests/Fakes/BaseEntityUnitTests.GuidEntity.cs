using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Domain.Tests.Fakes;

public class GuidEntity : BaseEntity<Guid>
{
    public GuidEntity() : base()
    {
    }
    public GuidEntity(Guid id) : base(id)
    {
    }
}
