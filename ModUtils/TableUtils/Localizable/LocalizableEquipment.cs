using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public class LocalizableEquipmentBuilder
    {
        private string _id;
        private Dictionary<string, LocalizedStrings> _localizedStrings = new();
        
        // Helper method to add a key-value pair to the dictionary
        private LocalizableEquipmentBuilder Add(string key, LocalizedStrings text)
        {
            _localizedStrings[key] = text;
            return this;
        }
        
        // Helper method to get the hook for a key
        internal static string GetHookForKey(string key)
        {
            return key switch
            {
                "weaponName" => "weapon_name_end",
                "weaponDesc" => "weapon_desc_end",
                "armorName" => "armor_name_end",
                "armorDesc" => "armor_desc_end",
                "armorClass" => "armor_class_end",
                "itemMaterial" => "item_material_end",
                "itemTier" => "item_tier_end",
                "weaponType" => "weapon_type_end",
                "weaponPronoun" => "weapon_pronoun_end",
                "armorPronoun" => "armor_pronoun_end",
                "weaponTypeGender" => "weapon_type_gender_end",
                _ => throw new KeyNotFoundException($"Key '{key}' not found.")
            };
        }
        
        // Helper method to format a line for a key
        internal static string FormatLineForKey(string key, string id, LocalizedStrings text)
        {
            return key switch
            {
                "weaponName" or 
                "weaponDesc" or 
                "armorName" or 
                "armorDesc" or 
                "armorClass" or 
                "itemMaterial" or 
                "itemTier" or 
                "weaponType" or 
                "weaponPronoun" or 
                "armorPronoun" or 
                "weaponTypeGender" => $"{id};{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};",
                _ => throw new KeyNotFoundException($"Key {key} not found.")
            };
        }
        
        // API methods to set the localized strings
        public LocalizableEquipmentBuilder WithId(string id)
        {
            _id = id;
            return this;
        }
        public LocalizableEquipmentBuilder WithWeaponName(LocalizedStrings text) => Add("weaponName", text);
        public LocalizableEquipmentBuilder WithWeaponDesc(LocalizedStrings text) => Add("weaponDesc", text);
        public LocalizableEquipmentBuilder WithArmorName(LocalizedStrings text) => Add("armorName", text);
        public LocalizableEquipmentBuilder WithArmorDesc(LocalizedStrings text) => Add("armorDesc", text);
        public LocalizableEquipmentBuilder WithArmorClass(LocalizedStrings text) => Add("armorClass", text);
        public LocalizableEquipmentBuilder WithItemMaterial(LocalizedStrings text) => Add("itemMaterial", text);
        public LocalizableEquipmentBuilder WithItemTier(LocalizedStrings text) => Add("itemTier", text);
        public LocalizableEquipmentBuilder WithWeaponType(LocalizedStrings text) => Add("weaponType", text);
        public LocalizableEquipmentBuilder WithWeaponPronoun(LocalizedStrings text) => Add("weaponPronoun", text);
        public LocalizableEquipmentBuilder WithArmorPronoun(LocalizedStrings text) => Add("armorPronoun", text);
        public LocalizableEquipmentBuilder WithWeaponTypeGender(LocalizedStrings text) => Add("weaponTypeGender", text);
        
        // API method called to finish the builder and call the injector
        public void Inject()
        {
            if (_localizedStrings.Count > 0)
                DoInjectTableLocalizableEquipment(_id, _localizedStrings);
            else
            {
                Log.Error("Failed to inject localizable effect: Nothing to inject.");
                throw new ArgumentException("Failed to inject localizable effect: Nothing to inject.");
            }
        }
    }
    
    // API exposed to modders
    public static LocalizableEquipmentBuilder InjectTableLocalizableEquipment() => new();
    
    // Method actually responsible for the injection
    private static void DoInjectTableLocalizableEquipment(string id, Dictionary<string, LocalizedStrings> localizedStrings)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_equipment";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));

        foreach ((string key, LocalizedStrings text) in localizedStrings)
        {
            // Get the hook for the key
            string hook = LocalizableEquipmentBuilder.GetHookForKey(key);
            
            // Find hook in table
            (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hook));
            if (foundLine == null)
                Log.Error($"Failed to inject {key} into table {tableName}: Hook '{hook}' not found");
            
            // Prepare line
            string newline = LocalizableEquipmentBuilder.FormatLineForKey(key, id, text);
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind, newline);
                Log.Information($"Injected {key} into table '{tableName}' at hook '{hook}'.");
            }
        }
        ModLoader.SetTable(table, tableName);
    }
}