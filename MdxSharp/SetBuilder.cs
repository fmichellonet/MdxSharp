using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MdxSharp
{
    internal class SetBuilder
    {

        public static Set Build(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var mAccess = (MemberExpression)expr;
                    var name = string.Empty;
                    ReduceSet(mAccess, out name);
                    return new Set(new Member(name));

                case ExpressionType.New:
                    var ctor = (NewExpression)expr;
                    if (expr.Type == typeof(Set))
                    {
                        var mem = ctor.Members;
                        Set s = Set.Empty();
                        if (ctor.Arguments.Any())
                        {
                            var arrayExpr = ctor.Arguments.First() as NewArrayExpression;
                            
                            foreach (var itemExpr in arrayExpr.Expressions)
                            {
                                s = s.Union(Build(itemExpr));
                            }
                        }
                        return s;
                    }
                    throw new NotImplementedException($"Cannot build a set from {expr.Type}");
                default:
                    throw new NotImplementedException($"{expr.NodeType} is not implemented ");
            }
            throw new NotImplementedException($"{expr.NodeType} is not implemented ");
        }

        private static void ReduceSet(MemberExpression mExpr, out string name)
        {
            name = string.Empty;
            if (mExpr.Expression.NodeType == ExpressionType.MemberAccess)
            {
                var childXpr = (MemberExpression)mExpr.Expression;
                ReduceSet(childXpr, out name);
                name += $".[{GetMemberName(mExpr.Member)}]";
            }
            if (mExpr.Expression.NodeType == ExpressionType.Parameter)
            {
                name = $"[{mExpr.Member.Name}]";
            }
        }

        private static string GetMemberName(MemberInfo t)
        {
            var attr = t.GetCustomAttribute<UniqueNameAttribute>();
            return attr == null ? t.Name : attr.Name;
        }
    }
}
