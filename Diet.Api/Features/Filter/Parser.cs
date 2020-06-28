using System;
using System.Net;
using Diet.Api.Infrastructure.ExceptionHandling;

namespace Diet.Api.Features.Filter
{
    // ToDo : Implement missing exception handling statements
    // Check => http://www.cs.man.ac.uk/~pjj/farrell/comp3.html
    // Check => https://sites.google.com/site/fredm/linguist-parsing-system
    public class Parser
    {
        private Lexer Lexer { get; set; }

        public QueryToken GetQueryToken(string filter)
        {
            Lexer = new Lexer(filter);
            var token = ParseExpression();
            Lexer.ValidateTokenCategory(ExpressionTokenCategory.End);
            return token;
        }

        private QueryToken ParseExpression()
        {
            return ParseLogicalOr();
        }

        private QueryToken ParseLogicalOr()
        {
            var left = ParseLogicalAnd();
            while (Lexer.Token.Category == ExpressionTokenCategory.Or)
            {
                Lexer.NextToken();
                var right = ParseLogicalAnd();
                left = new BinaryOperatorToken(BinaryOperatorCategory.Or, left, right);
            }
            return left;
        }

        private QueryToken ParseLogicalAnd()
        {
            var left = ParseComparison();
            while (Lexer.Token.Category == ExpressionTokenCategory.And)
            {
                Lexer.NextToken();
                var right = ParseComparison();
                left = new BinaryOperatorToken(BinaryOperatorCategory.And, left, right);
            }
            return left;
        }

        private QueryToken ParseComparison()
        {
            var left = ParseUnary();
            while (Lexer.Token.Category == ExpressionTokenCategory.Equal ||
                   Lexer.Token.Category == ExpressionTokenCategory.NotEqual ||
                   Lexer.Token.Category == ExpressionTokenCategory.GreaterThan ||
                   Lexer.Token.Category == ExpressionTokenCategory.GreaterThanOrEqual ||
                   Lexer.Token.Category == ExpressionTokenCategory.LessThan ||
                   Lexer.Token.Category == ExpressionTokenCategory.LessThanOrEqual)
            {

                var operatorCategory = Lexer.Token.Category;
                Lexer.NextToken();
                var right = ParseUnary();

                left = operatorCategory switch
                {
                    ExpressionTokenCategory.Equal
                        => new BinaryOperatorToken(BinaryOperatorCategory.Equal, left, right),
                    ExpressionTokenCategory.NotEqual
                        => new BinaryOperatorToken(BinaryOperatorCategory.NotEqual, left, right),
                    ExpressionTokenCategory.GreaterThan
                        => new BinaryOperatorToken(BinaryOperatorCategory.GreaterThan, left, right),
                    ExpressionTokenCategory.GreaterThanOrEqual
                        => new BinaryOperatorToken(BinaryOperatorCategory.GreaterThanOrEqual, left, right),
                    ExpressionTokenCategory.LessThan
                        => new BinaryOperatorToken(BinaryOperatorCategory.LessThan, left, right),
                    ExpressionTokenCategory.LessThanOrEqual
                        => new BinaryOperatorToken(BinaryOperatorCategory.LessThanOrEqual, left, right),
                    _ => left
                };
            }
            return left;
        }

        private QueryToken ParseUnary()
        {
            if (Lexer.Token.Category != ExpressionTokenCategory.Minus)
            {
                return ParseContent();
            }

            var operatorCategory = Lexer.Token.Category;
            var operatorIndex = Lexer.Token.Index;
            Lexer.NextToken();
            if (operatorCategory == ExpressionTokenCategory.Minus &&
                (Lexer.Token.Category == ExpressionTokenCategory.IntegerLiteral ||
                 Lexer.Token.Category == ExpressionTokenCategory.DecimalLiteral))
            {
                Lexer.Token.Text = "-" + Lexer.Token.Text;
                Lexer.Token.Index = operatorIndex;
                return ParseContent();
            }
            var expression = ParseUnary();
            expression = operatorCategory == ExpressionTokenCategory.Minus
                ? new UnaryOperatorToken(UnaryOperatorCategory.Negate, expression)
                : new UnaryOperatorToken(UnaryOperatorCategory.Not, expression);
            return expression;
        }

        private QueryToken ParseContent()
        {
            return Lexer.Token.Category switch
            {
                ExpressionTokenCategory.OpenParenthesis => ParseParenthesisExpression(),
                ExpressionTokenCategory.Identifier => ParseIdentifier(),
                ExpressionTokenCategory.StringLiteral => ParseStringLiteral(),
                ExpressionTokenCategory.IntegerLiteral => ParseIntegerLiteral(),
                ExpressionTokenCategory.DecimalLiteral => ParseDecimalLiteral(),
                _ => throw new RestException(HttpStatusCode.Conflict, "Title", $"at {Lexer.Token.Index}")
            };
        }

        private QueryToken ParseDecimalLiteral()
        {
            ConstantToken token = null;

            if (decimal.TryParse(Lexer.Token.Text, out var decimalValue))
            {
                token = new ConstantToken(decimalValue, typeof(decimal));
            }
            else
            {
                Error("Filter text has invalid decimal literal at {0}", Lexer.Token.Index);
            }

            Lexer.NextToken();

            return token;
        }

        private QueryToken ParseParenthesisExpression()
        {
            Lexer.NextToken();
            var token = ParseExpression();
            Lexer.NextToken();
            return token;
        }

        private QueryToken ParseIdentifier()
        {
            var identifier = Lexer.Token.Text;
            Lexer.NextToken();
            return new PropertyToken(null, identifier);
        }

        private QueryToken ParseStringLiteral()
        {
            // Remove quotes
            var value = Lexer.Token.Text.Substring(1, Lexer.Token.Text.Length - 2);

            // Remove escaped quotes
            value = value.Replace("''", "'");

            ConstantToken token;

            if (DateTime.TryParse(value, out var dateTimeValue))
            {
                token = new ConstantToken(dateTimeValue, typeof(DateTime));
            }
            else if (Guid.TryParse(value, out var guidValue))
            {
                token = new ConstantToken(guidValue, typeof(Guid));
            }
            else if (bool.TryParse(value, out var booleanValue))
            {
                token = new ConstantToken(booleanValue, typeof(bool));
            }
            else
            {
                token = new ConstantToken(value, typeof(string));
            }

            Lexer.NextToken();
            return token;
        }

        private QueryToken ParseIntegerLiteral()
        {
            var text = Lexer.Token.Text;

            if (!int.TryParse(text, out var integerValue))
            {
                Error("Invalid integer literal at {0}", Lexer.Token.Index);
            }
            Lexer.NextToken();
            return new ConstantToken(integerValue, typeof(int));
        }

        private void Error(string message, int errorIndex)
        {
            throw new RestException(HttpStatusCode.Conflict, message, string.Format(message, errorIndex));
        }
    }
}