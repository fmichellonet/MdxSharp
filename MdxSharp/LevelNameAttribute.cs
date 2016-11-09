using System;

namespace MdxSharp
{
    public class LevelNameAttribute : Attribute
    {
        public string Name { get; }

        public LevelNameAttribute(string name)
        {
            Name = name;
        }
    }
}