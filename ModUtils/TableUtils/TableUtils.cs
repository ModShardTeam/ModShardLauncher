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
                .Value;
        }
        public static void InjectTableAnimalsAI()
        {
            string table = "gml_GlobalScript_table_animals_ai";
        }
        public static void InjectTableWeapons()
        {
            string table = "gml_GlobalScript_table_weapons";
        }
        public static void InjectTableEnemyBalance()
        {
            string table = "gml_GlobalScript_table_enemy_balance";
        }
        public static void InjectTableArmor()
        {
            string table = "gml_GlobalScript_table_armor";
        }
        public static void InjectTableCreditsBackers()
        {
            string table = "gml_GlobalScript_table_credits_backers";
        }
        public static void InjectTablePotion()
        {
            string table = "gml_GlobalScript_table_Potion";
        }
        public static void InjectTableContract()
        {
            string table = "gml_GlobalScript_table_Contract";
        }
    }
}