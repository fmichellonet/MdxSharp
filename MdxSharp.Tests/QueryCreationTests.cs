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
        public void CanCreateOnColumnQuery()
        {
            string expected = "SELECT { [measures].[Internet Order Count] } ON COLUMNS FROM [Adventure Works]";

            var cnx = new AdomdConnection(cnxString);
            cnx.Query<MyCubeDef>()
               .OnColumns(x => x.Measures.InternetOrderCount)
               .ToString()
               .Should().ContainEquivalentOf(expected);
        }

        [Test]
        public void CanCreateOnColumnQueryWithMultipleMember()
        {
            string expected = "SELECT { [measures].[Internet Order Count], [measures].[Internet Order Count] } ON COLUMNS FROM [Adventure Works]";

            var cnx = new AdomdConnection(cnxString);
            cnx.Query<MyCubeDef>()
               .OnColumns(x => new Set(new[] {x.Measures.InternetOrderCount, x.Measures.InternetOrderCount}))
               .ToString()
               .Should().ContainEquivalentOf(expected);
        }

        //[Test]
        //public void CanCreateOnColumnQueryWithMultipleMemberFromIEnumerable()
        //{
        //    Assert.Fail();
        //}
    }

    [UniqueName("Adventure Works")]
    public class MyCubeDef
    {
        public Measures Measures { get; }
    }

    public class Measures : Dimension
    {
        [UniqueName("Internet Order Count")]
        public Member InternetOrderCount { get; }
    }
}
