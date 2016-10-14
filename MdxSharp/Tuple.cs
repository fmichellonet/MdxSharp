using System.Collections.Generic;
using System.Linq;

namespace MdxSharp
{
    public class Tuple : IMdxStatement
    {
        private readonly IEnumerable<Member> _members;

        public Tuple(Member member)
        {
            _members = new HashSet<Member>(new[] {member});
        }

        public Tuple(IEnumerable<Member> members)
        {
            _members = new HashSet<Member>(members);
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
    }
}