using TesteVsoft.Application.Interfaces.CQRS;

namespace TesteVsoft.Infrastructure.Tests.Fakes;

public class FakeQueryHandler : IQueryHandler<FakeQuery, bool>
{
    public bool Handled { get; private set; } = false;

    public Task<bool> Handle(FakeQuery query, CancellationToken cancellationToken)
    {
        Handled = true;
        return Task.FromResult(Handled);
    }
}
