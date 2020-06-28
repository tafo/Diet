using System.Collections.Generic;

namespace Diet.Api.Features.Filter
{
    public class FilteredResponse<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }

        public List<T> Items { get; set; }
    }
}