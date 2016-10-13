using FluentAssertions;
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
               .OnColumns(x => x.Measures.InternetOrderCount)
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
               .OnColumns(x => x.Measures.InternetOrderCount)
               .OnRows(x => x.Customer.StateProvince.NewYork)
               .ToString()
               .Should().ContainEquivalentOf(expected);
        }
    }

    [UniqueName("[Adventure Works]")]
    public class MyCubeDef
    {
        [UniqueName("[measures]")]
        public Measures Measures { get; }

        [UniqueName("[Customer]")]
        public Customer Customer { get; }
    }

    public class Measures : Dimension
    {
        [UniqueName("[Internet Order Count]")]
        public Member InternetOrderCount { get; }
    }

    public class Customer : Dimension
    {
        [UniqueName("[State-Province]")]
        public StateProvince StateProvince { get; }
    }

    public class StateProvince : Hierarchy
    {
        [UniqueName("&[NY]&[US]")]
        public Member NewYork { get; }
    }
}