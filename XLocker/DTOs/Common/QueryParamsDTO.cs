namespace XLocker.DTOs.Common
{
    public class QueryParamsDTO
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public string? Order { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public bool IsPaginated()
        {
            return Page > 0 && PageSize > 0;
        }

        public bool ShouldSearchByDate()
        {
            return FromDate != null && ToDate != null;
        }
    }
}
