using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TesteVsoft.Application.Interfaces.BackgroundServices;
using TesteVsoft.Application.Interfaces.Repositories;

namespace TesteVsoft.Infrastructure.Workers;

public class UserCreationWorker : BackgroundService
{
    private readonly IBackgroundTaskQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserCreationWorker> _logger;

    public UserCreationWorker(IBackgroundTaskQueue queue, IServiceScopeFactory scopeFactory, ILogger<UserCreationWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var task = await _queue.DequeueAsync(stoppingToken);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                await task(userRepository, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao executar criação de usuário em segundo plano {ex}", ex);
            }
        }
    }
}
