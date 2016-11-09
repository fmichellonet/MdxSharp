using FluentAssertions;
using MdxSharp.Tests.Model.AdventureWorks;
using Microsoft.AnalysisServices.AdomdClient;
using NUnit.Framework;

namespace MdxSharp.Tests
{
    [TestFixture]
    public class QueryCreationTests
    {
        private readonly string cnxString =
            "Initial Catalog=AdventureWorksDW2014Multidimensional-EE;Data Source=(local)";

        [Test]
        public void CanCreateEmptyQuery()
        {
            string expected = "SELECT { [measures].defaultmember } ON COLUMNS FROM [Adventure Works]";

            var cnx = new AdomdConnection();
            cnx.Query<MyCubeDef>().ToString()
                                  .Should().ContainEquivalentOf(expected);
        }

        [Test]
        public void CanCreateOnColumnsQuery()
        {
            string expected = "SELECT { [measures].[Internet Order Count] } ON COLUMNS FROM [Adventure Works]";

            var cnx = new AdomdConnection(cnxString);
            cnx.Query<MyCubeDef>()
               .OnColumns(x => new Set(x.Measures.InternetOrderCount))
               .ToString()
               .Should().ContainEquivalentOf(expected);
        }

        [Test]
        public void CanCreateOnColumnsQueryWithMultipleMember()
        {
            string expected = "SELECT { [measures].[Internet Order Count], [measures].[Internet Order Count] } ON COLUMNS FROM [Adventure Works]";

            var cnx = new AdomdConnection(cnxString);
            cnx.Query<MyCubeDef>()
               .OnColumns(x => new Set(new[] {x.Measures.InternetOrderCount, x.Measures.InternetOrderCount}))
               .ToString()
               .Should().ContainEquivalentOf(expected);
        }

        [Test]
        public void CanCreateOnColumnsOnRowsQuery()
        {
            string expected = "SELECT { [measures].[Internet Order Count] } ON COLUMNS, { [Customer].[State-Province].&[NY]&[US] } ON ROWS FROM [Adventure Works]";

            var cnx = new AdomdConnection(cnxString);
            cnx.Query<MyCubeDef>()
               .OnColumns(x => new Set(x.Measures.InternetOrderCount))
               .OnRows(x => new Set(x.Customer.StateProvince.NewYork))
               .ToString()
               .Should().ContainEquivalentOf(expected);
        }

        [Test]
        public void CanCreateWhereQuery()
        {
            string expected = "SELECT { [measures].[Internet Order Count] } ON COLUMNS FROM [Adventure Works] WHERE ( [Customer].[State-Province].&[NY]&[US] )";

            var cnx = new AdomdConnection(cnxString);
            cnx.Query<MyCubeDef>()
               .OnColumns(x => new Set(x.Measures.InternetOrderCount))
               .Where(x => new Tuple(x.Customer.StateProvince.NewYork))
               .ToString()
               .Should().ContainEquivalentOf(expected);
        }
    }
}