using TesteVsoft.Application.Common.Extensions;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Dtos;
using TesteVsoft.Application.Interfaces.CQRS;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Queries;

public class ListUsersQuery : FilterDto, IQuery
{
}

public class ListUsersQueryHandler(IUserRepository repository) : IQueryHandler<ListUsersQuery, PaginatedList<User, Guid>>
{
    public async Task<PaginatedList<User, Guid>> Handle(ListUsersQuery query, CancellationToken cancellationToken)
    {
        return await repository.GetPaginatedAsync(query.ToFilter<User, Guid>(), cancellationToken);
    }
}