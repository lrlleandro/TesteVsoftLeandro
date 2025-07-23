namespace TesteVsoft.Application.Interfaces.CQRS;

public interface IEventDispatcher
{
    Task NotifyAsync(INotification notification, CancellationToken cancellationToken);
    Task<TResponse> QueryAsync<TResponse>(IQuery query, CancellationToken cancellationToken);
    Task SendAsync(ICommand command, CancellationToken cancellationToken);
    Task<TResponse> SendAsync<TResponse>(ICommand command, CancellationToken cancellationToken = default);
}