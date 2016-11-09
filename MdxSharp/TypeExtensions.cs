using System;
using System.Linq;
using System.Reflection;

namespace MdxSharp
{
    public static class TypeExtensions
    {
        public static string GetUniqueNameOrDefault(this MemberInfo property)
        {
            var attr = property.GetCustomAttribute<UniqueNameAttribute>();
            return attr == null ? property.Name : attr.Name;
        }

        public static string GetLevelNameOrDefault(this MemberInfo property)
        {
            var attr = property.GetCustomAttribute<LevelNameAttribute>();
            return attr == null ? property.Name : attr.Name;
        }

        public static PropertyInfo GetMatchingProperty(this Type type, Func<MemberInfo, string> selector, string descriptor)
        {
            var minfo = type.GetProperties()
                            .FirstOrDefault(x => selector(x) == descriptor);

            return minfo;
        }
    }
}