using TesteVsoft.Application.Common.Models;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Interfaces.Repositories.Common;

public interface IRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
    Task<TEntity?> GetOneAsync(Filter<TEntity, TId> filter, CancellationToken cancellationToken);
    Task<TEntity?> GetOneByIdAsync(TId id, CancellationToken cancellationToken);
    Task<PaginatedList<TEntity, TId>> GetPaginatedAsync(Filter<TEntity, TId> filter, CancellationToken cancellationToken);
    Task<IEnumerable<TEntity>> GetAsync(Filter<TEntity, TId> filter, CancellationToken cancellationToken);
    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken);
    Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
}