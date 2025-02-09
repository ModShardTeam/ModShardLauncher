using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public enum LocalizableNamesGender
    {
        Male,
        Female
    }
    public static void InjectTableLocalizableNames(
        string id, 
        LocalizedStrings? names = null, 
        LocalizableNamesGender? gender = null,
        LocalizedStrings? npcInfo = null,
        LocalizedStrings? constantName = null
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_names";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Required setup
        int ind = 0;
        string? foundLine = null;

        #region NAMES
        if (names != null)
        {
            // Ensuring gender arg is set
            if (gender == null)
            {
                Log.Error("The 'names' argument requires the 'gender' argument to be set.");
                throw new ArgumentException("The 'names' argument requires the 'gender' argument to be set.");
            }
            // Prepare line
            string namesStr = $";{names.Russian};{names.English};{names.Chinese};{names.German};{names.SpanishLatam};{names.French};{names.Italian};{names.Portuguese};{names.Polish};{names.Turkish};{names.Japanese};{names.Korean};";
    
            // Find hook in table
            switch (gender)
            {
                case LocalizableNamesGender.Male:
                    (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("male_human_end"));
                    break;
                case LocalizableNamesGender.Female:
                    (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("female_human_end"));
                    break;
            }
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind, namesStr);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Name {id} into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Name {id} into table {tableName}");
        }
        #endregion

        #region NPC_INFO
        if (npcInfo != null)
        {
            // Prepare line
            string npcInfoStr = $"{id};{npcInfo.Russian};{npcInfo.English};{npcInfo.Chinese};{npcInfo.German};{npcInfo.SpanishLatam};{npcInfo.French};{npcInfo.Italian};{npcInfo.Portuguese};{npcInfo.Polish};{npcInfo.Turkish};{npcInfo.Japanese};{npcInfo.Korean};";
        
            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("NPC_info_end"));
        
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind, npcInfoStr);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected NPC Info {id} into table {tableName}");
            }
            else
                Log.Error($"Failed to inject NPC Info {id} into table {tableName}");
        }
        #endregion

        #region CONSTANT_NAME
        if (constantName != null)
        {
            // Prepare line
            string constantNameStr = $"{id};{constantName.Russian};{constantName.English};{constantName.Chinese};{constantName.German};{constantName.SpanishLatam};{constantName.French};{constantName.Italian};{constantName.Portuguese};{constantName.Polish};{constantName.Turkish};{constantName.Japanese};{constantName.Korean};";
        
            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("Constant_Name_end"));
        
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind, constantNameStr);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Constant Name {id} into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Constant Name {id} into table {tableName}");
        }
        #endregion
    }
}