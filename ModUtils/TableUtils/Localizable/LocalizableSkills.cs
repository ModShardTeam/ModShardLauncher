using System.Collections.Generic;
using System.Linq;
using ModShardLauncher.Mods;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public static void InjectTableLocalizableSkills(
        string id, 
        LocalizedStrings? name = null,
        LocalizedStrings? desc = null,
        LocalizedStrings? infoText = null,
        LocalizedStrings? category = null,
        LocalizedStrings? type = null,
        LocalizedStrings? subtype = null,
        LocalizedStrings? midText = null
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_skills";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));


        // Required setup
        int ind = 0;
        string? foundLine;
        
        #region NAME
        if (name != null)
        {
            // Prepare line
            string nameStr = $"{id};{name.Russian};{name.English};{name.Chinese};{name.German};{name.SpanishLatam};{name.French};{name.Italian};{name.Portuguese};{name.Polish};{name.Turkish};{name.Japanese};{name.Korean};";
            
            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("skill_name"));
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind + 1, nameStr);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Skill Name {id} into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Skill Name {id} into table {tableName}");
        }
        #endregion
        
        #region DESC
        if (desc != null)
        {
            // Prepare line
            string descStr = $"{id};{desc.Russian};{desc.English};{desc.Chinese};{desc.German};{desc.SpanishLatam};{desc.French};{desc.Italian};{desc.Portuguese};{desc.Polish};{desc.Turkish};{desc.Japanese};{desc.Korean};";
    
            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("skill_desc"));
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind + 1, descStr);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Skill Desc {id} into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Skill Desc {id} into table {tableName}");
        }
        #endregion

        #region INFOTEXT
        if (infoText != null)
        {
            // Prepare line
            string infoTextStr = $"{id};{infoText.Russian};{infoText.English};{infoText.Chinese};{infoText.German};{infoText.SpanishLatam};{infoText.French};{infoText.Italian};{infoText.Portuguese};{infoText.Polish};{infoText.Turkish};{infoText.Japanese};{infoText.Korean};";

            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("skill_info_text"));
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind + 1, infoTextStr);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Skill InfoText {id} into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Skill InfoText {id} into table {tableName}");
        }
        #endregion

        #region CATEGORY
        // Prepare line
        if (category != null)
        {
            string categoryStr = $"{id};{category.Russian};{category.English};{category.Chinese};{category.German};{category.SpanishLatam};{category.French};{category.Italian};{category.Portuguese};{category.Polish};{category.Turkish};{category.Japanese};{category.Korean};";

            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("skill_category"));
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind + 1, categoryStr);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Skill Category {id} into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Skill Category {id} into table {tableName}");
        }
        #endregion

        #region TYPE
        // Prepare line
        if (type != null)
        {
            string typeStr = $"{id};{type.Russian};{type.English};{type.Chinese};{type.German};{type.SpanishLatam};{type.French};{type.Italian};{type.Portuguese};{type.Polish};{type.Turkish};{type.Japanese};{type.Korean};";

            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("skill_type"));
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind + 1, typeStr);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Skill Type {id} into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Skill Type {id} into table {tableName}");
        }
        #endregion

        #region SUBTYPE
        if (subtype != null)
        {
            // Prepare line
            string subtypeStr = $"{id};{subtype.Russian};{subtype.English};{subtype.Chinese};{subtype.German};{subtype.SpanishLatam};{subtype.French};{subtype.Italian};{subtype.Portuguese};{subtype.Polish};{subtype.Turkish};{subtype.Japanese};{subtype.Korean};";
            
            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("skill_subtype"));
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind + 1, subtypeStr);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Skill Subtype {id} into table {tableName}");
            }
            else 
                Log.Error($"Failed to inject Skill Subtype {id} into table {tableName}");
        }
        #endregion

        #region MIDTEXT
        if (midText != null)
        {
            // Prepare line
            string midTextStr = $"{id};{midText.Russian};{midText.English};{midText.Chinese};{midText.German};{midText.SpanishLatam};{midText.French};{midText.Italian};{midText.Portuguese};{midText.Polish};{midText.Turkish};{midText.Japanese};{midText.Korean};";
            
            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("skill_mid_text"));
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind +1, midTextStr);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Skill Mid Text {id} into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Skill Mid Text {id} into table {tableName}");
        }
        #endregion
    }
}