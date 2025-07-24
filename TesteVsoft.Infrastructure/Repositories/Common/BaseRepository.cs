using Microsoft.EntityFrameworkCore;
using TesteVsoft.Infrastructure.Data;
using TesteVsoft.Application.Common.Models;
using TesteVsoft.Application.Common.Enums;
using TesteVsoft.Application.Interfaces.Repositories.Common;
using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Infrastructure.Repositories.Common;

public abstract class BaseRepository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : BaseEntity<TId>
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected BaseRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<TEntity?> GetOneByIdAsync(TId id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }

    public async Task<TEntity?> GetOneAsync(Filter<TEntity, TId> filter, CancellationToken cancellationToken)
    {
        var query = _dbSet.AsQueryable();

        foreach (var whereClause in filter.Wheres)
        {
            query = query.Where(whereClause.ToExpression());
        }

        var item = await query.FirstOrDefaultAsync(cancellationToken);

        return item;
    }

    public async Task<PaginatedList<TEntity, TId>> GetPaginatedAsync(Filter<TEntity, TId> filter, CancellationToken cancellationToken)
{
    var query = _dbSet.AsQueryable();

    foreach (var whereClause in filter.Wheres)
    {
        query = query.Where(whereClause.ToExpression());
    }

    IOrderedQueryable<TEntity>? orderedQuery = null;

    foreach (var orderByClause in filter.OrderBys)
    {
        var expression = orderByClause.ToExpression();

        if (orderedQuery == null)
        {
            orderedQuery = orderByClause.Direction == OrderDirectionTypes.Ascending
                ? query.OrderBy(expression)
                : query.OrderByDescending(expression);
        }
        else
        {
            orderedQuery = orderByClause.Direction == OrderDirectionTypes.Ascending
                ? orderedQuery.ThenBy(expression)
                : orderedQuery.ThenByDescending(expression);
        }
    }

    if (orderedQuery != null)
        query = orderedQuery;
    else
        query = query.OrderBy(x => x.Id);

    foreach (var relation in filter.WithRelations)
    {
        query = query.Include(relation.ToExpression());
    }

    var totalCount = await query.CountAsync(cancellationToken);

    var items = await query
        .Skip((filter.Page - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToListAsync(cancellationToken);

    return new PaginatedList<TEntity, TId>(items, totalCount, filter.Page, filter.PageSize);
}

    public async Task<IEnumerable<TEntity>> GetAsync(Filter<TEntity, TId> filter, CancellationToken cancellationToken)
{
    var query = _dbSet.AsQueryable();

    foreach (var whereClause in filter.Wheres)
    {
        query = query.Where(whereClause.ToExpression());
    }

    IOrderedQueryable<TEntity>? orderedQuery = null;

    foreach (var orderByClause in filter.OrderBys)
    {
        var expression = orderByClause.ToExpression();

        if (orderedQuery == null)
        {
            orderedQuery = orderByClause.Direction == OrderDirectionTypes.Ascending
                ? query.OrderBy(expression)
                : query.OrderByDescending(expression);
        }
        else
        {
            orderedQuery = orderByClause.Direction == OrderDirectionTypes.Ascending
                ? orderedQuery.ThenBy(expression)
                : orderedQuery.ThenByDescending(expression);
        }
    }

    if (orderedQuery != null)
        query = orderedQuery;
    else
        query = query.OrderBy(x => x.Id);

    var items = await query
        .Skip((filter.Page - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToListAsync(cancellationToken);

    return items;
}

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        foreach (var entity in entities)
        {
            _dbSet.Update(entity);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        _dbSet.RemoveRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
    }
}