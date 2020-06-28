using System;

namespace Diet.Api.Features.Filter
{
    public sealed class ConstantToken : QueryToken
    {
        public override QueryTokenCategory Category => QueryTokenCategory.Constant;

        public object Value { get; }

        public ConstantToken(object value, Type typeReference)
        {
            Value = value;
            TypeReference = typeReference;
        }
    }
}