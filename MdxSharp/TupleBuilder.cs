using System;
using System.Linq;
using System.Linq.Expressions;

namespace MdxSharp
{
    internal class TupleBuilder
    {
        
        public static Tuple Build(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var name = Mapper.Reduce(expr as MemberExpression);
                    return new Tuple(new Member(name));

                case ExpressionType.New:
                    var ctor = (NewExpression)expr;
                    if (ctor.Type == typeof(Tuple))
                    {
                        switch (ctor.Arguments.First().NodeType)
                        {
                            // Build tuple from member[] initializer
                            case ExpressionType.NewArrayInit:
                                return BuildTupleFromArray(ctor);

                            // Build tuple with constructor taking 1 member
                            case ExpressionType.MemberAccess:
                                return BuildTupleWithOneMember(ctor);

                            default:
                                throw new NotImplementedException($"Cannot build a tuple with {expr}");
                        }
                    }
                    throw new NotImplementedException($"Cannot build a tuple from {expr.Type}");
                default:
                    throw new NotImplementedException($"{expr.NodeType} is not implemented ");
            }
        }

        private static Tuple BuildTupleFromArray(NewExpression expr)
        {
            Tuple t = Tuple.Empty();
            if (expr.Arguments.Any())
            {
                var arrayExpr = expr.Arguments.First() as NewArrayExpression;

                foreach (var itemExpr in arrayExpr.Expressions)
                {
                    t = t.Union(Build(itemExpr));
                }
            }
            else
            {
                throw new NotImplementedException($"Cannot build a tuple with {expr}");
            }
            return t;
        }

        private static Tuple BuildTupleWithOneMember(NewExpression ctor)
        {
            if (ctor.Arguments.Count != 1)
            {
                throw new NotImplementedException($"Cannot build a tuple with {ctor}");
            }
            else
            {
                return Build(ctor.Arguments.First());
            }
        }
    }
}