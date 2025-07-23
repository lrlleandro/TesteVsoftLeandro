using TesteVsoft.Application.Interfaces.CQRS;

namespace TesteVsoft.Infrastructure.Tests.Fakes;

public class FakeCommandHandler : ICommandHandler<FakeCommand>
{
    public bool Handled { get; private set; } = false;

    public Task Handle(FakeCommand command, CancellationToken cancellationToken)
    {
        Handled = true;
        return Task.CompletedTask;
    }
}
