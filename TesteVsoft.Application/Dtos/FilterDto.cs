using TesteVsoft.Application.Common.Enums;

namespace TesteVsoft.Application.Dtos;

public class FilterDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<string> Selects { get; set; } = [];
    public List<string> Relations { get; set; } = [];
    public List<WhereDto> Wheres { get; set; } = [];
    public List<OrderByDto> OrderBys { get; set; } = [];
}

public class OrderByDto
{
    public required string PropertyName { get; set; }
    public required OrderDirectionTypes Direction { get; set; }
}

public class WhereDto
{
    public required string PropertyName { get; set; }
    public required WhereOperationTypes Operation { get; set; }
    public required string Value { get; set; }
}