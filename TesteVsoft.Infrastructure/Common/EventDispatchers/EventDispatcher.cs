using Microsoft.Extensions.DependencyInjection;
using TesteVsoft.Application.Interfaces.CQRS;

namespace TesteVsoft.Infrastructure.Common.EventDispatchers;

public sealed class EventDispatcher(IServiceProvider serviceProvider, IHandlerResolver handlerResolver) : IEventDispatcher
{
    public async Task NotifyAsync(INotification notification, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var handlerTypes = handlerResolver.GetHandlerTypesForNotificationType(notification.GetType());

        var tasks = new List<Task>();

        foreach (var handlerType in handlerTypes!)
        {
            var handler = serviceProvider.GetService(handlerType);

            if (handler is null)
            {
                continue;
            }

            var handlerMethod = handlerType.GetMethod("Handle");

            if (handlerMethod is not null)
            {
                var task = (Task)handlerMethod.Invoke(handler, [notification, cancellationToken])!;
                tasks.Add(task);
            }
        }

        await Task.WhenAll(tasks);
    }


    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();

        var handlerType = handlerResolver.GetHandlerTypesForType(command.GetType())
            ?? throw new InvalidCastException($"Nenhum handler encontrado para o request do tipo {command.GetType().Name}");

        var handler = scope.ServiceProvider.GetService(handlerType)
            ?? throw new InvalidCastException($"Nenhum handler encontrado para o request do tipo {command.GetType().Name}");

        var handlerMethod = handlerType.GetMethod("Handle");

        if (handlerMethod is not null)
        {
            await (Task)handlerMethod.Invoke(handler, [command, cancellationToken])!;
        }
    }

    public async Task<TResponse> SendAsync<TResponse>(ICommand command, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();

        var handlerType = handlerResolver.GetHandlerTypesForType(command.GetType())
            ?? throw new InvalidCastException($"Nenhum handler encontrado para o request do tipo {command.GetType().Name}");

        var handler = scope.ServiceProvider.GetService(handlerType)
            ?? throw new InvalidCastException($"Nenhum handler encontrado para o request do tipo {command.GetType().Name}");

        var handlerMethod = handlerType.GetMethod("Handle")
            ?? throw new InvalidOperationException($"O handler {handlerType.Name} não possui o método Handle");

        var response = await (Task<TResponse>)handlerMethod.Invoke(handler, [command, cancellationToken])!;
        return response;
    }

    public async Task<TResponse> QueryAsync<TResponse>(IQuery query, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();

        var handlerType = handlerResolver.GetHandlerTypesForType(query.GetType())
            ?? throw new InvalidCastException($"Nenhum handler encontrado para o request do tipo {query.GetType().Name}");

        var handler = scope.ServiceProvider.GetService(handlerType)
            ?? throw new InvalidCastException($"Nenhum handler encontrado para o request do tipo {query.GetType().Name}");

        var handlerMethod = handlerType.GetMethod("Handle")
            ?? throw new InvalidOperationException($"O handler {handlerType.Name} não possui o método Handle");

        var response = await (Task<TResponse>)handlerMethod.Invoke(handler, [query, cancellationToken])!;
        return response;
    }
}
