using TesteVsoft.Application.Common.Extensions;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Queries;

public class ListUserTasksQuery : FilterDto, IQuery
{
}

public class ListUserTasksQueryHandler(IUserTaskRepository repository) : IQueryHandler<ListUserTasksQuery, PaginatedList<UserTask, Guid>>
{
    public async Task<PaginatedList<UserTask, Guid>> Handle(ListUserTasksQuery query, CancellationToken cancellationToken)
    {
        return await repository.GetPaginatedAsync(query.ToFilter<UserTask, Guid>(), cancellationToken);
    }
}