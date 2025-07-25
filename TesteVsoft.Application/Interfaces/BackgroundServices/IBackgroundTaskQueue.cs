using TesteVsoft.Application.Interfaces.Repositories;

namespace TesteVsoft.Application.Interfaces.BackgroundServices;

public interface IBackgroundTaskQueue
{
    ValueTask EnqueueAsync(Func<IUserRepository, CancellationToken, Task> workItem);
    ValueTask<Func<IUserRepository, CancellationToken, Task>?> DequeueAsync(CancellationToken cancellationToken);
}
