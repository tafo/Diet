namespace Diet.Api.Features.Filter
{
    public enum ExpressionTokenCategory
    {
        None,
        Identifier,
        StringLiteral,
        IntegerLiteral,
        DecimalLiteral,
        Minus,
        OpenParenthesis,
        CloseParenthesis,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Equal,
        NotEqual,
        And,
        Or,
        Not,
        End
    }
}