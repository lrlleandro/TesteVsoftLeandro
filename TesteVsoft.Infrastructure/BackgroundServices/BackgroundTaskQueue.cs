using System.Threading.Channels;
using TesteVsoft.Application.Interfaces.BackgroundServices;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Infrastructure.Common.Attributes;

namespace TesteVsoft.Infrastructure.BackgroundServices;

[Singleton]
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<IUserRepository, CancellationToken, Task>> _queue;

    public BackgroundTaskQueue()
    {
        _queue = Channel.CreateUnbounded<Func<IUserRepository, CancellationToken, Task>>(
            new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            });
    }

    public ValueTask EnqueueAsync(Func<IUserRepository, CancellationToken, Task> workItem)
    {
        if (workItem is null)
            throw new ArgumentNullException(nameof(workItem));

        return _queue.Writer.WriteAsync(workItem);
    }

    public ValueTask<Func<IUserRepository, CancellationToken, Task>?> DequeueAsync(CancellationToken cancellationToken)
    {
        return _queue.Reader.ReadAsync(cancellationToken);
    }
}
