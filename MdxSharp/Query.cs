using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AnalysisServices.AdomdClient;

namespace MdxSharp
{
    public class Query<T>
    {
        private readonly AdomdConnection _cnx;

        private readonly Set _rows;
        private readonly Set _columns;

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

        public Query<T> OnColumns<TSet>(Expression<Func<T, TSet>> columnSelector)
        {
            var expr = columnSelector.Body;
            var columns = SetBuilder.Build(expr);
            return new Query<T>(_cnx, _rows, columns);
        }

        public Query<T> OnRows<TSet>(Expression<Func<T, TSet>> rowSelector)
        {
            var expr = rowSelector.Body;
            var rows = SetBuilder.Build(expr);
            return new Query<T>(_cnx, rows, _columns);
        }

        private string BuildQuery()
        {
            var cubeName = MdxHelper.NormalizeCubeName(GetCubeName(typeof(T)));

            var columns = _columns?.ToMdx() ?? "{ [Measures].defaultmember }";
            var qry = $"SELECT {columns} ON COLUMNS";
            if (_rows != null)
            {
                qry += $", {_rows.ToMdx()} ON ROWS";
            }
            qry += $" FROM {cubeName}";
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
                return $"{name.Trim()}";
            }
        }
    }
}