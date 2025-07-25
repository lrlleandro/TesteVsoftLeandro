using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Domain.Tests.Fakes;

public class IntEntity : BaseEntity<int>
{
    public IntEntity() : base()
    {
    }

    public IntEntity(int id) : base(id)
    {
    }
}