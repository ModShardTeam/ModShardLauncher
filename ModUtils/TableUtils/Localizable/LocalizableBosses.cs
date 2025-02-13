using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public class LocalizableBossesBuilder
    {
        private Dictionary<string, LocalizedStrings> _localizedStrings = new();
        
        // Helper method to add a key-value pair to the dictionary
        private LocalizableBossesBuilder Add(string key, LocalizedStrings text)
        {
            _localizedStrings[key] = text;
            return this;
        }
        
        // Helper method to get the hook for a key
        internal static string GetHookForKey(string key)
        {
            return key switch
            {
                "possessed" => "Possessed_end",
                "powerful" => "Powerful_end",
                "vindictive" => "Vindictive_end",
                "persistent" => "Persistent_end",
                "trapmaker" => "Trapmaker_end",
                "resistant" => "Resistant_end",
                "summoner" => "Summoner_end",
                "fatebinded" => "Fatebinded_end",
                "berserking" => "Berserking_end",
                "immortal" => "Immortal_end",
                "illusionist" => "Illusionist_end",
                "overcharged" => "Overcharged_end",
                "leeching" => "Leeching_end",
                "auramancer" => "Auramancer_end",
                "unstable" => "Unstable_end",
                "raging" => "Raging_end",
                "nimble" => "Nimble_end",
                "exhausting" => "Exhausting_end",
                "masochistic" => "Masochistic_end",
                "restorating" => "Restorating_end",
                "fiery" => "Fiery_end",
                "toxic" => "Toxic_end",
                "acidic" => "Acidic_end",
                "watchful" => "Watchful_end",
                "bossNamePart1" => "boss_name_part_1_end",
                "bossNamePart2" => "boss_name_part_2_end",
                "bossFemale" => "boss_female_end",
                "bossNameBrigand" => "boss_name_brigand_end",
                _ => throw new KeyNotFoundException($"Key '{key}' not found.")
            };
        }
        
        // Helper method to format a line for a key
        internal static string FormatLineForKey(string key, LocalizedStrings text)
        {
            return key switch
            {
                "possessed" or 
                "powerful" or 
                "vindictive" or 
                "persistent" or 
                "trapmaker" or 
                "resistant" or 
                "summoner" or 
                "fatebinded" or 
                "berserking" or 
                "immortal" or 
                "illusionist" or 
                "overcharged" or 
                "leeching" or 
                "auramancer" or 
                "unstable" or 
                "raging" or 
                "nimble" or 
                "exhausting" or 
                "masochistic" or 
                "restorating" or 
                "fiery" or 
                "toxic" or 
                "acidic" or 
                "watchful" or 
                "bossNamePart1" or 
                "bossNamePart2" or 
                "bossFemale" or 
                "bossNameBrigand" => $";{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};",
                _ => throw new KeyNotFoundException($"Key {key} not found.")
            };
        }
        
        // API methods to set the localized strings
        public LocalizableBossesBuilder WithPossessed(LocalizedStrings text) => Add("possessed", text);
        public LocalizableBossesBuilder WithPowerful(LocalizedStrings text) => Add("powerful", text);
        public LocalizableBossesBuilder WithVindictive(LocalizedStrings text) => Add("vindictive", text);
        public LocalizableBossesBuilder WithPersistent(LocalizedStrings text) => Add("persistent", text);
        public LocalizableBossesBuilder WithTrapmaker(LocalizedStrings text) => Add("trapmaker", text);
        public LocalizableBossesBuilder WithResistant(LocalizedStrings text) => Add("resistant", text);
        public LocalizableBossesBuilder WithSummoner(LocalizedStrings text) => Add("summoner", text);
        public LocalizableBossesBuilder WithFatebinded(LocalizedStrings text) => Add("fatebinded", text);
        public LocalizableBossesBuilder WithBerserking(LocalizedStrings text) => Add("berserking", text);
        public LocalizableBossesBuilder WithImmortal(LocalizedStrings text) => Add("immortal", text);
        public LocalizableBossesBuilder WithIllusionist(LocalizedStrings text) => Add("illusionist", text);
        public LocalizableBossesBuilder WithOvercharged(LocalizedStrings text) => Add("overcharged", text);
        public LocalizableBossesBuilder WithLeeching(LocalizedStrings text) => Add("leeching", text);
        public LocalizableBossesBuilder WithAuramancer(LocalizedStrings text) => Add("auramancer", text);
        public LocalizableBossesBuilder WithUnstable(LocalizedStrings text) => Add("unstable", text);
        public LocalizableBossesBuilder WithRaging(LocalizedStrings text) => Add("raging", text);
        public LocalizableBossesBuilder WithNimble(LocalizedStrings text) => Add("nimble", text);
        public LocalizableBossesBuilder WithExhausting(LocalizedStrings text) => Add("exhausting", text);
        public LocalizableBossesBuilder WithMasochistic(LocalizedStrings text) => Add("masochistic", text);
        public LocalizableBossesBuilder WithRestorating(LocalizedStrings text) => Add("restorating", text);
        public LocalizableBossesBuilder WithFiery(LocalizedStrings text) => Add("fiery", text);
        public LocalizableBossesBuilder WithToxic(LocalizedStrings text) => Add("toxic", text);
        public LocalizableBossesBuilder WithAcidic(LocalizedStrings text) => Add("acidic", text);
        public LocalizableBossesBuilder WithWatchful(LocalizedStrings text) => Add("watchful", text);
        public LocalizableBossesBuilder WithBossNamePart1(LocalizedStrings text) => Add("bossNamePart1", text);
        public LocalizableBossesBuilder WithBossNamePart2(LocalizedStrings text) => Add("bossNamePart2", text);
        public LocalizableBossesBuilder WithBossFemale(LocalizedStrings text) => Add("bossFemale", text);
        public LocalizableBossesBuilder WithBossNameBrigand(LocalizedStrings text) => Add("bossNameBrigand", text);
        
        // API method called to finish the builder and call the injector
        public void Inject()
        {
            if (_localizedStrings.Count > 0)
                DoInjectTableLocalizableBosses(_localizedStrings);
            else
            {
                Log.Error("Failed to inject localizable effect: Nothing to inject.");
                throw new ArgumentException("Failed to inject localizable effect: Nothing to inject.");
            }
        }
    }
    
    // API method exposed to modders
    public static LocalizableBossesBuilder InjectTableLocalizableBosses() => new();
    
    // Method actually responsible for the injection
    private static void DoInjectTableLocalizableBosses(Dictionary<string, LocalizedStrings> localizedStrings)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_bosses";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));

        foreach ((string key, LocalizedStrings text) in localizedStrings)
        {
            // Get hook for the key
            string hook = LocalizableBossesBuilder.GetHookForKey(key);
            
            // Find hook in table
            (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hook));
            if (foundLine == null)
                Log.Error($"Failed to inject {key} into table {tableName}: Hook '{hook}' not found.");;
            
            // Prepare line
            string newline = LocalizableBossesBuilder.FormatLineForKey(key, text);
            
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