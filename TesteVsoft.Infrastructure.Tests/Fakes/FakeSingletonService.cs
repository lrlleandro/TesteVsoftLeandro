using TesteVsoft.Infrastructure.Common.Attributes;

namespace TesteVsoft.Infrastructure.Tests.Fakes;

public interface IFakeSingletonService
{
}

[Singleton]
public class FakeSingletonService : IFakeSingletonService
{
}