namespace Diet.Api.Features.Filter
{
    public sealed class PropertyToken : QueryToken
    {
        public override QueryTokenCategory Category => QueryTokenCategory.Property;

        public QueryToken Instance { get; }
        public string PropertyName { get; }

        public PropertyToken(QueryToken instance, string propertyName)
        {
            Instance = instance;
            PropertyName = propertyName;
        }
    }
}