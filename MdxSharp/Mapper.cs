using System;
using System.Linq.Expressions;

namespace MdxSharp
{
    internal static class Mapper
    {
        internal static string Reduce(MemberExpression mExpr)
        {
            if (mExpr.Expression.NodeType == ExpressionType.MemberAccess)
            {
                var childXpr = (MemberExpression) mExpr.Expression;
                var name = Reduce(childXpr);
                return $"{name}.{mExpr.Member.GetUniqueNameOrDefault()}";
            }
            if (mExpr.Expression.NodeType == ExpressionType.Parameter)
            {
                return $"{mExpr.Member.GetUniqueNameOrDefault()}";
            }
            throw new InvalidOperationException($"Cannot reduce tuple for expression {mExpr}");
        }
    }
}