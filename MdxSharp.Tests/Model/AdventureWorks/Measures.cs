namespace MdxSharp.Tests.Model.AdventureWorks
{
    public class Measures : Dimension
    {
        [UniqueName("[Internet Order Count]")]
        public Member InternetOrderCount { get; }
    }
}