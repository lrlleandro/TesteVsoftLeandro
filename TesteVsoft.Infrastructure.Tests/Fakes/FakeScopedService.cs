using TesteVsoft.Infrastructure.Common.Attributes;

namespace TesteVsoft.Infrastructure.Tests.Fakes;

public interface IFakeScopedService
{
    string Ping();
}

[Scoped]
public class FakeScopedService : IFakeScopedService
{
    public string Ping()
    {
        return "pong";
    }
}