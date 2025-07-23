using TesteVsoft.Application.Interfaces.CQRS;

namespace TesteVsoft.Infrastructure.Tests.Fakes;

public class FakeNotificationHandler : INotificationHandler<FakeNotification>
{
    public bool Handled { get; private set; } = false;

    public Task Handle(FakeNotification notification, CancellationToken cancellationToken)
    {
        Handled = true;
        return Task.CompletedTask;
    }
}

