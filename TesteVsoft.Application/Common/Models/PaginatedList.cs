using TesteVsoft.Domain.Entities.Common;

namespace TesteVsoft.Application.Common.Models;

public class PaginatedList<T, TId> where T : BaseEntity<TId>
{
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public IEnumerable<T> Items { get; set; }

    public PaginatedList(IEnumerable<T> items, int totalCount, int page, int pageSize)
    {
        Page = page;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        Items = items;
    }
}