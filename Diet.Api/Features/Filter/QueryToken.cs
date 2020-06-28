using System;

namespace Diet.Api.Features.Filter
{
    public abstract class QueryToken
    {
        public abstract QueryTokenCategory Category { get; }
        public Type TypeReference { get; set; }
    }
}