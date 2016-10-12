using System.Collections.Generic;
using System.Linq;

namespace MdxSharp
{
    public class Set : IMdxStatement
    {
        private readonly IEnumerable<Member> _members;

        public Set(Member member)
        {
            _members = new List<Member>(new[] {member});
        }

        public Set(IEnumerable<Member> members)
        {
            this._members = members;
        }

        public IEnumerable<Member> Members => _members;

        public static Set Empty()
        {
            return new Set(new Member[]{});
        }

        public Set Union(Set set)
        {
            return new Set(_members.Union(set.Members));
        }

        public string ToMdx()
        {
            return $"{{ {string.Join(", ", _members.Select(x => x.ToMdx()))} }}";
        }
    }

    public interface IMdxStatement
    {
        string ToMdx();
    }
}