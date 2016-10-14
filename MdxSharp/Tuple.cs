using System.Collections.Generic;
using System.Linq;

namespace MdxSharp
{
    public class Tuple : IMdxStatement
    {
        private readonly HashSet<Member> _members;

        public Tuple(Member member)
        {
            _members = new HashSet<Member>(new[] {member}, new MemberComparer());
        }

        public Tuple(IEnumerable<Member> members)
        {
            _members = new HashSet<Member>(members, new MemberComparer());
        }

        public IEnumerable<Member> Members => _members;

        public static Tuple Empty()
        {
            return new Tuple(new Member[]{});
        }

        public Tuple Union(Tuple set)
        {
            return new Tuple(_members.Union(set.Members));
        }

        public string ToMdx()
        {
            return $"( {string.Join(", ", _members.Select(x => x.ToMdx()))} )";
        }

        private class MemberComparer : IEqualityComparer<Member> {
            public bool Equals(Member x, Member y)
            {
                return x?.Name?.Equals(y?.Name) ?? false;
            }

            public int GetHashCode(Member m)
            {
                return m.Name.GetHashCode();
            }
        }
    }
}