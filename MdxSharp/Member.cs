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

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Member))
                return false;
            Member other = (Member) obj;
            return this.Name.Equals(other.Name);
        }
    } 
}