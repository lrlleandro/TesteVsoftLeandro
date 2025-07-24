using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Common.Models;

public class Filter<T, TId> where T : BaseEntity<TId>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<Select<T, TId>> Selects { get; set; } = [];
    public List<Relation<T, TId>> WithRelations { get; set; } = [];
    public List<Where<T, TId>> Wheres { get; set; } = [];
    public List<OrderBy<T, TId>> OrderBys { get; set; } = [];
}