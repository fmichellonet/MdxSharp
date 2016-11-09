namespace MdxSharp.Tests.Model.AdventureWorks
{
    public class Customer : Dimension
    {
        [UniqueName("[State-Province]")]
        public StateProvince StateProvince { get; }
    }
}