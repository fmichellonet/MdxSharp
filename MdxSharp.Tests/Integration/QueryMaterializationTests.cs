using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MdxSharp.Tests.Model.AdventureWorks;
using Microsoft.AnalysisServices.AdomdClient;
using NUnit.Framework;

namespace MdxSharp.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class QueryMaterializationTests
    {
        private readonly string cnxString =
            "Initial Catalog=AdventureWorksDW2014Multidimensional-EE;Data Source=(local)";


        [Test]
        public void RowsAndColumns()
        {
            IEnumerable<MatPoco> res;
            // select { [Measures].[Internet Order Count] } ON COLUMNS, { [Customer].[State-Province].&[NY]&[US] } FROM [Adventure Works]
            using (var cnx = new AdomdConnection(cnxString))
            {
                res = cnx.Query<MyCubeDef>()
                    .OnColumns(x => new Set(x.Measures.InternetOrderCount))
                    .OnRows(x => new Set(x.Customer.StateProvince.NewYork))
                    .Execute<MatPoco>();
            }

            res.Should().NotBeNull();
            res.Should().HaveCount(1);
            res.First().OrderCount.Should().Be(3);
            res.First().Province.Should().Be("[Customer].[State-Province].&[NY]&[US]");
        }

        [Test]
        public void ColumnsOnly()
        {
            IEnumerable<MatPoco> res;
            // select { [measures].[Internet Order Count] } ON COLUMNS FROM [Adventure Works]
            using (var cnx = new AdomdConnection(cnxString))
            {
                res = cnx.Query<MyCubeDef>()
                         .OnColumns(x => new Set(x.Measures.InternetOrderCount))
                         .Execute<MatPoco>();
            }

            res.Should().NotBeNull();
            res.Should().HaveCount(1);
            res.First().OrderCount.Should().Be(27659);
        }
    }

    public class MatPoco
    {
        [UniqueName("[Measures].[Internet Order Count]")]
        public int OrderCount { get; set; }

        [LevelName("[Customer].[State-Province].[State-Province]")]
        public string Province { get; set; }
    }
}