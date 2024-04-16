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
        public static void InjectTableArmor()
        {
            string table = "gml_GlobalScript_table_armor";
            throw new NotImplementedException();
        }
        public static void InjectTableCreditsBackers()
        {
            string table = "gml_GlobalScript_table_credits_backers";
            throw new NotImplementedException();
        }
        public static void InjectTablePotion()
        {
            string table = "gml_GlobalScript_table_Potion";
            throw new NotImplementedException();
        }
    }
}
