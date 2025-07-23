namespace TesteVsoft.Application.Interfaces.CQRS;

public interface IQueryHandler<TQuery, TResponse> where TQuery : IQuery
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}