using Microsoft.AnalysisServices.AdomdClient;
using NUnit.Framework;

namespace MdxSharp.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class QueryExecutionTests
    {
        private readonly string cnxString =
            "Initial Catalog=AdventureWorksDW2014Multidimensional-EE;Data Source=(local)";


        [Test]
        public void EmptyQuery()
        {
            // select {measures.defaultmember} ON COLUMNS FROM [Adventure Works]
            using (var cnx = new AdomdConnection(cnxString))
            {
                var res = cnx.Query<MyCubeDef>()
                             .Execute();
            }
        }
    }
}
