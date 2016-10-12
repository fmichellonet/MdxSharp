namespace MdxSharp
{

    public class Member : IMdxStatement
    {
        public string Name { get; }

        public Member(string name)
        {
            Name = name;
        }

        public string ToMdx()
        {
            return $"{Name}";
        }
    } 

    //public class Member<T>
    //{
        
    //}
}