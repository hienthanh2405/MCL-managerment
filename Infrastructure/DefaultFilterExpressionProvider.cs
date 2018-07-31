using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace API.Infrastructure
{
    public class DefaultFilterExpressionProvider : IFilterExpressionProvider
    {
        protected const string EqualsOperator = "eq";
        protected const string LikeOperator = "like";

        public virtual IEnumerable<string> GetOperators()
        {
            yield return EqualsOperator;
        }


        public virtual Expression GetComparison(
            MemberExpression left,
            string op,
            ConstantExpression right)
        {
            MethodInfo contains = typeof(string).GetMethod("Contains");

            switch (op.ToLower())
            {
                case EqualsOperator: return Expression.Equal(left, right);
                case LikeOperator: return Expression.Call(left , contains, right);
                default: throw new ArgumentException($"Invalid operator '{op}'.");
            }
        }

        public virtual ConstantExpression GetValue(string input)
            => Expression.Constant(input);
    }
}
