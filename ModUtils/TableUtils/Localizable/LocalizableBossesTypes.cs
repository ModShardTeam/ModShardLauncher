using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Serilog;

namespace ModShardLauncher;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public partial class Msl
{
    [Flags]
    public enum LocalizableBossesTypesTypes
    {
        all,
        noone,
        Melee,
        Ranged,
        Mage
    }

    public static void InjectTableLocalizableBossesTypes(string prefix, LocalizableBossesTypesTypes types, LocalizedStrings translations)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_bosses_types";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{prefix};{types};{translations.Russian};{translations.English};{translations.Chinese};{translations.German};{translations.SpanishLatam};{translations.French};{translations.Italian};{translations.Portuguese};{translations.Polish};{translations.Turkish};{translations.Japanese};{translations.Korean};";
        
        // Add line to table
        table.Add(newline);
        ModLoader.SetTable(table, tableName);
        Log.Information($"Injected Boss Type {prefix} into table {tableName}");
    }
}