﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace MdxSharp
{
    internal class SetBuilder
    {
        public static Set Build(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var name = Mapper.Reduce(expr as MemberExpression);
                    return new Set(new Member(name));

                case ExpressionType.New:
                    var ctor = (NewExpression) expr;
                    if (ctor.Type == typeof(Set))
                    {
                        switch (ctor.Arguments.First().NodeType)
                        {
                            // Build set from member[] initializer
                            case ExpressionType.NewArrayInit:
                                return BuildSetFromArray(ctor);

                            // Build set with constructor taking 1 member
                            case ExpressionType.MemberAccess:
                                return BuildSetWithOneMember(ctor);

                            default:
                                throw new NotImplementedException($"Cannot build a set with {expr}");
                        }
                    }
                    throw new NotImplementedException($"Cannot build a set from {expr.Type}");
                default:
                    throw new NotImplementedException($"{expr.NodeType} is not implemented ");
            }
        }

        private static Set BuildSetFromArray(NewExpression expr)
        {
            Set s = Set.Empty();
            if (expr.Arguments.Any())
            {
                var arrayExpr = expr.Arguments.First() as NewArrayExpression;

                foreach (var itemExpr in arrayExpr.Expressions)
                {
                    s = s.Union(Build(itemExpr));
                }
            }
            else
            {
                throw new NotImplementedException($"Cannot build a set with {expr}");
            }
            return s;
        }

        private static Set BuildSetWithOneMember(NewExpression ctor)
        {
            if (ctor.Arguments.Count != 1)
            {
                throw new NotImplementedException($"Cannot build a set with {ctor}");
            }
            else
            {
                return Build(ctor.Arguments.First());
            }
        }
    }
}
