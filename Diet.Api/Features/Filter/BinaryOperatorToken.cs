namespace Diet.Api.Features.Filter
{
    public class BinaryOperatorToken : QueryToken
    {
        public override QueryTokenCategory Category => QueryTokenCategory.BinaryOperator;

        public QueryToken LeftOperand { get; set; }
        public QueryToken RightOperand { get; set; }
        public BinaryOperatorCategory OperatorCategory { get; }

        public BinaryOperatorToken(BinaryOperatorCategory category, QueryToken left, QueryToken right)
        {
            OperatorCategory = category;
            LeftOperand = left;
            RightOperand = right;
        }
    }
}