namespace MdxSharp.Tests.Model.AdventureWorks
{
    [UniqueName("[Adventure Works]")]
    public class MyCubeDef
    {
        [UniqueName("[measures]")]
        public Measures Measures { get; }

        [UniqueName("[Customer]")]
        public Customer Customer { get; }
    }
}