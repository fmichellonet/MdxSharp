using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AnalysisServices.AdomdClient;

namespace MdxSharp
{
    public class Query<T>
    {
        private readonly AdomdConnection _cnx;

        private readonly Set _rows;
        private readonly Set _columns;
        private readonly Tuple _where;

        internal Query(AdomdConnection cnx, Set rows, Set columns, Tuple where)
        {
            _cnx = cnx;
            _rows = rows;
            _columns = columns;
            _where = where;
        }

        public CellSet Execute()
        {
            using (var cmd = new AdomdCommand(BuildQuery(), _cnx))
            {
                _cnx.Open();
                return cmd.ExecuteCellSet();
            }
        }

        public IEnumerable<TResult> Execute<TResult>()
            where TResult : new()
        {
            var cset = Execute();
            return Materializer<TResult>.Bake(cset);
        }

        public Query<T> OnColumns<TSet>(Expression<Func<T, TSet>> columnSelector)
            where TSet : Set
        {
            var expr = columnSelector.Body;
            var columns = SetBuilder.Build(expr);
            return new Query<T>(_cnx, _rows, columns, _where);
        }

        public Query<T> OnRows<TSet>(Expression<Func<T, TSet>> rowSelector)
            where TSet : Set
        {
            var expr = rowSelector.Body;
            var rows = SetBuilder.Build(expr);
            return new Query<T>(_cnx, rows, _columns, _where);
        }

        public Query<T> Where<TTuple>(Expression<Func<T, TTuple>> whereSelector)
            where TTuple : Tuple
        {
            var expr = whereSelector.Body;
            var where = TupleBuilder.Build(expr);
            return new Query<T>(_cnx, _rows, _columns, where);
        }

        private string BuildQuery()
        {
            var cubeName = typeof(T).GetUniqueNameOrDefault();

            var columns = _columns?.ToMdx() ?? "{ [Measures].defaultmember }";
            var qry = $"SELECT {columns} ON COLUMNS";
            if (_rows != null)
            {
                qry += $", {_rows.ToMdx()} ON ROWS";
            }
            qry += $" FROM {cubeName}";
            if (_where != null)
            {
                qry += $" WHERE {_where.ToMdx()}";
            }
            return qry;
        }

        public override string ToString()
        {
            return BuildQuery();
        }
    }
}