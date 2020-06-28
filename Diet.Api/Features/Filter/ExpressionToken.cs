namespace Diet.Api.Features.Filter
{
    public class ExpressionToken
    {
        public ExpressionTokenCategory Category { get; set; }
        public string Text { get; set; }
        public int Index { get; set; }
    }
}