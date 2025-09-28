using System;

namespace Main.Modules.DisasterPredictionModule.Models.Responses;

public class PaginationResponse<T> where T : class
{
    public int TotalPage { get; set; }
    public int TotalRecord { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<T> Data { get; set; }
}
