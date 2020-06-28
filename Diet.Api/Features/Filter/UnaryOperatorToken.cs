namespace Diet.Api.Features.Filter
{
    public class UnaryOperatorToken : QueryToken
    {
        public override QueryTokenCategory Category => QueryTokenCategory.UnaryOperator;

        public UnaryOperatorCategory OperatorCategory { get; }
        public QueryToken Operand { get; }

        public UnaryOperatorToken(UnaryOperatorCategory operatorCategory, QueryToken operand)
        {
            OperatorCategory = operatorCategory;
            Operand = operand;
        }
    }
}