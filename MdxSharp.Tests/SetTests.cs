using FluentAssertions;
using NUnit.Framework;

namespace MdxSharp.Tests
{
    public class SetTests
    {
        [Test]
        public void MemberIsNonUnique()
        {
            // Arrange 
            var s1 = new Set(new Member("[measures].[a]"));
            var s2 = new Set(new Member("[measures].[a]"));

            // Act 
            var s3 = s1.Union(s2);

            // Assert
            s3.Members.Should().HaveCount(2);
        }
    }
}