using System;
using System.Linq.Expressions;

namespace Diet.Api.Features.Filter
{
    public interface IExpressionProvider<T>
    {
        public Expression<Func<T, bool>> Filter(string filter);
    }

    public class ExpressionProvider<T> : IExpressionProvider<T>
    {
        private ParameterExpression _filterParameter;

        public Expression<Func<T, bool>> Filter(string filter)
        {
            var parser = new Parser();
            var token = parser.GetQueryToken(filter);
            return (Expression<Func<T, bool>>)GetLambdaExpression(token, typeof(T));
        }

        private LambdaExpression GetLambdaExpression(QueryToken expression, Type elementType)
        {
            _filterParameter = Expression.Parameter(elementType, "x");
            var body = GetExpressionBody(expression);
            return Expression.Lambda(body, _filterParameter);
        }

        private Expression GetExpressionBody(QueryToken token)
        {
            return token.Category switch
            {
                QueryTokenCategory.UnaryOperator => GetUnaryExpression(token as UnaryOperatorToken),
                QueryTokenCategory.Property => GetPropertyExpression(token as PropertyToken),
                QueryTokenCategory.BinaryOperator => GetBinaryExpression(token as BinaryOperatorToken),
                QueryTokenCategory.Constant => GetConstantExpression(token as ConstantToken),
                _ => throw new Exception()
            };
        }

        private Expression GetUnaryExpression(UnaryOperatorToken unaryOperatorToken)
        {
            var inner = GetExpressionBody(unaryOperatorToken.Operand);
            return unaryOperatorToken.OperatorCategory switch
            {
                UnaryOperatorCategory.Negate => Expression.Negate(inner),
                UnaryOperatorCategory.Not => Expression.Not(inner),
                _ => throw new Exception("Filter error")
            };
        }

        private Expression GetPropertyExpression(PropertyToken propertyToken)
        {
            var parameter = _filterParameter;
            return Expression.Property(parameter, propertyToken.PropertyName);
        }

        private Expression GetBinaryExpression(BinaryOperatorToken binaryOperatorToken)
        {
            var left = GetExpressionBody(binaryOperatorToken.LeftOperand);
            var right = GetExpressionBody(binaryOperatorToken.RightOperand);

            var innerLambda = binaryOperatorToken.OperatorCategory switch
            {
                BinaryOperatorCategory.Or => Expression.Or(left, right),
                BinaryOperatorCategory.And => Expression.And(left, right),
                BinaryOperatorCategory.Equal => Expression.Equal(left, right),
                BinaryOperatorCategory.NotEqual => Expression.NotEqual(left, right),
                BinaryOperatorCategory.GreaterThan => Expression.GreaterThan(left, right),
                BinaryOperatorCategory.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, right),
                BinaryOperatorCategory.LessThan => Expression.LessThan(left, right),
                BinaryOperatorCategory.LessThanOrEqual => Expression.LessThanOrEqual(left, right),
                _ => throw new Exception("Filter error")
            };

            return innerLambda;
        }

        private Expression GetConstantExpression(ConstantToken constantToken)
        {
            return Expression.Constant(constantToken.Value, constantToken.TypeReference);
        }
    }
}