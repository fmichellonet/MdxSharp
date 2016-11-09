namespace MdxSharp.Tests.Model.AdventureWorks
{
    public class StateProvince : Hierarchy
    {
        [UniqueName("&[NY]&[US]")]
        public Member NewYork { get; }
    }
}