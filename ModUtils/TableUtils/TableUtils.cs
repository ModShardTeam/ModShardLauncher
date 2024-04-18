using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace ModShardLauncher
{ 
    public partial class Msl
    {
        private static string? GetEnumMemberValue<T>(this T value)
            where T : Enum
        {
            return typeof(T)
                .GetTypeInfo()
                .DeclaredMembers
                .SingleOrDefault(x => x.Name == value.ToString())?
                .GetCustomAttribute<EnumMemberAttribute>(false)?
                .Value ?? value.ToString();
        }
        public static void InjectTableAnimalsAI()
        {
            string table = "gml_GlobalScript_table_animals_ai";
            throw new NotImplementedException();
        }
    }
}
