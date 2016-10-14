using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace MdxSharp.Tests
{
    [TestFixture]
    public class TupleTests
    {
        [Test]
        public void MemberIsUnique()
        {
            // Arrange 
            var t1 = new Tuple(new Member("[measures].[a]"));
            var t2 = new Tuple(new Member("[measures].[a]"));

            // Act 
            var t3 = t1.Union(t2);

            // Assert
            t3.Members.Should().HaveCount(1);
        }
    }
}
