using TesteVsoft.Application.Interfaces.CQRS;

namespace TesteVsoft.Infrastructure.Tests.Fakes;

public class FakeCommandWithResponseHandler : ICommandHandler<FakeCommandWithResponse, bool>
{
    public bool Handled { get; private set; } = false;

    public Task<bool> Handle(FakeCommandWithResponse command, CancellationToken cancellationToken)
    {
        Handled = true;
        return Task.FromResult(Handled);
    }
}

