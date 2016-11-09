using System.Collections.Generic;
using Microsoft.AnalysisServices.AdomdClient;

namespace MdxSharp
{
    public static class Materializer<T>
        where T : new()
    {
        private static readonly int columnIndex = 0;
        private static readonly int rowIndex = 1;

        public static IEnumerable<T> Bake(CellSet cset)
        {
            if (cset.Axes.Count > 1)
            {
                return BakeMultidim(cset);
            }
            return ColumnsOnly(cset);
        }

        private static IEnumerable<T> ColumnsOnly(CellSet cset)
        {
            List<T> res = new List<T>();

            // Number of dimensions on the column
            //var colDimCount = cset.Axes[columnIndex].Positions[0].Members.Count;

            // Number of columns
            var colCount = cset.Axes[columnIndex].Positions.Count;

            var item = new T();

            for (int colIdx = 0; colIdx < colCount; colIdx++)
            {
                var v = cset[colIdx];
                var m1 = cset.Axes[columnIndex].Positions[0].Members[colIdx];
                SetValue(item, v, m1);
            }
            res.Add(item);

            return res;
        }

        private static IEnumerable<T> BakeMultidim(CellSet cset)
        {
            List<T> res = new List<T>();

            //Number of dimensions on the column
            var colDimCount = cset.Axes[columnIndex].Positions[0].Members.Count;

            //Number of dimensions on the row
            int rowDimCount = 0;

            int rowCount = 0;

            if (cset.Axes[rowIndex].Positions[0].Members.Count > 0)
            {
                rowDimCount = cset.Axes[rowIndex].Positions[0].Members.Count;
            }

            //number of rows + rows for column headers
            rowCount = cset.Axes[rowIndex].Positions.Count + colDimCount;

            //number of columns + columns for row headers
            var colCount = cset.Axes[columnIndex].Positions.Count + rowDimCount;

            for (int rowIdx = colDimCount; rowIdx < rowCount; rowIdx++)
            {
                var item = new T();

                for (int colIdx = 0; colIdx < colCount; colIdx++)
                {
                    // row of header.
                    //if (rowIdx < colDimCount)
                    //{
                    //    // empty cell
                    //    if (colIdx < rowDimCount)
                    //        break;
                    //    else
                    //    {
                    //        // we're on a membercell
                    //        Microsoft.AnalysisServices.AdomdClient.Member m = cset.Axes[columnIndex].Positions[colIdx - rowDimCount].Members[rowIdx];
                    //    }
                    //}

                    // this a row with data.
                    if (colIdx < rowDimCount)
                    {
                        var m = cset.Axes[rowIndex].Positions[rowIdx - colDimCount].Members[colIdx];
                        SetMember(item, m);
                    }
                    else
                    {
                        var v = cset[colIdx - rowDimCount, rowIdx - colDimCount];
                        int ordinal = int.Parse(v.CellProperties["CellOrdinal"].Value.ToString());
                        var m1 = cset.Axes[columnIndex].Positions[rowIdx - colDimCount].Members[ordinal];
                        SetValue(item, v, m1);
                    }
                }

                res.Add(item);
            }
            return res;
        }

        private static void SetMember(T item, Microsoft.AnalysisServices.AdomdClient.Member member)
        {
            var mInfo = item.GetType().GetMatchingProperty(x => x.GetLevelNameOrDefault(), member.LevelName);
            if (mInfo != null)
            {
                mInfo.SetValue(item, member.Name);
            }
        }

        private static void SetValue(T item, Cell value, Microsoft.AnalysisServices.AdomdClient.Member relatedMember)
        {
            var mInfo = item.GetType().GetMatchingProperty(x => x.GetUniqueNameOrDefault(), relatedMember.UniqueName);
            if (mInfo != null)
            {
                mInfo.SetValue(item, value.Value);
            }
        }
    }
}