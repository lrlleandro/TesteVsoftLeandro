using TesteVsoft.Infrastructure.Common.Attributes;

namespace TesteVsoft.Infrastructure.Tests.Fakes;

public interface IFakeTransientService
{
}

[Transient]
public class FakeTransientService : IFakeTransientService
{
}