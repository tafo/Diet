namespace Diet.Api.Features.Filter
{
    public class FilteredRequest
    {
        public string Filter { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
    }
}