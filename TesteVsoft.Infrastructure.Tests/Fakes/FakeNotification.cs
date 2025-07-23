using TesteVsoft.Application.Interfaces.CQRS;

namespace TesteVsoft.Infrastructure.Tests.Fakes;

public class FakeNotification : INotification
{
    public string? Value { get; set; }
}

