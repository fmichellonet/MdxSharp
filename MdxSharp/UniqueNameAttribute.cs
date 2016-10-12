using System;

namespace MdxSharp
{
    public class UniqueNameAttribute : Attribute
    {
        public string Name { get; }

        public UniqueNameAttribute(string name)
        {
            Name = name;
        }
    }
}