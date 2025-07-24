using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Tests.Fakes;

public class FakeEntity : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public OtherEntity Related { get; set; } = new();
    public List<OtherEntity> Children { get; set; } = [];
}

