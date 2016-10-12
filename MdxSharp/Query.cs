using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AnalysisServices.AdomdClient;

namespace MdxSharp
{
    public class Query<T>
    {
        private readonly AdomdConnection _cnx;

        private Set _rows;
        private Set _columns;

        internal Query(AdomdConnection cnx, Set rows, Set columns)
        {
            _cnx = cnx;
            _rows = rows;
            _columns = columns;
        }

        public CellSet Execute()
        {
            using (var cmd = new AdomdCommand(BuildQuery(), _cnx))
            {
                _cnx.Open();
                return cmd.ExecuteCellSet();
            }
        }

        public Query<T> OnColumns<TMeasure>(Expression<Func<T, TMeasure>> rowSelector)
        {
            Set columns;
            var expr = rowSelector.Body;
            //switch (expr.NodeType)
            //{
            //    case ExpressionType.MemberAccess:
            //        var mAccess = (MemberExpression)expr;
            //        var name = string.Empty;
            //        ReduceSet(mAccess, out name);
            //        columns = new Set(new Member(name));
            //        break;

            //    case ExpressionType.New:
            //        var ctor = (NewExpression)expr;
            //        if (expr.Type == typeof(Set))
            //        {
            //            var mem = ctor.Members;
            //            if (ctor.Arguments.Any())
            //            {
            //                var arrayExpr = ctor.Arguments.First() as NewArrayExpression;

            //            }
            //            columns = null;
            //        }
            //        else
            //        {
            //            throw new NotImplementedException($"Cannot build a set from {expr.Type}");
            //        }
            //        break;
            //    default:
            //        throw new NotImplementedException($"{expr.NodeType} is not implemented ");
            //}
            columns = SetBuilder.Build(expr);
            return new Query<T>(_cnx, _rows, columns);
        }



        private string BuildQuery()
        {
            var cubeName = MdxHelper.NormalizeCubeName(GetCubeName(typeof(T)));

            var columns = _columns?.ToMdx() ?? "{ [Measures].defaultmember }";
            var qry = $"SELECT {columns} ON COLUMNS FROM {cubeName}";

            return qry;
        }

        private string GetCubeName(Type t)
        {
            var attr = t.GetCustomAttribute<UniqueNameAttribute>();
            return attr == null ? t.Name : attr.Name;
        }

        

        public override string ToString()
        {
            return BuildQuery();
        }
    }

    public static class MdxHelper
    {
        public static string NormalizeCubeName(string name)
        {
            if (name.Trim().StartsWith("[") && name.Trim().StartsWith("]"))
            {
                return name.Trim();
            }
            else
            {
                return $"[{name.Trim()}]";
            }
        }
    }
}