using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MdxSharp
{
    internal static class Mapper
    {
        internal static string Reduce(MemberExpression mExpr)
        {
            if (mExpr.Expression.NodeType == ExpressionType.MemberAccess)
            {
                var childXpr = (MemberExpression)mExpr.Expression;
                var name = Reduce(childXpr);
                return $"{name}.{GetUniqueName(mExpr.Member)}";
            }
            if (mExpr.Expression.NodeType == ExpressionType.Parameter)
            {
                return $"{GetUniqueName(mExpr.Member)}";
            }
            throw new InvalidOperationException($"Cannot reduce tuple for expression {mExpr}");
        }

        private static string GetUniqueName(MemberInfo t)
        {
            var attr = t.GetCustomAttribute<UniqueNameAttribute>();
            return attr == null ? t.Name : attr.Name;
        }
    }
}