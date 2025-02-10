using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

[SuppressMessage("ReSharper", "CommentTypo")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public partial class Msl
{
    public class LocalizableTextBuilder
    {
        private string _id;
        private Dictionary<string, LocalizedStrings> _localizedStrings = new();
        
        
        // Helper method to add a key-value pair to the dictionary
        private LocalizableTextBuilder Add(string key, LocalizedStrings text)
        {
            _localizedStrings[key] = text;
            return this;
        }
        
        // Helper method to get the hook for a key
        internal static string GetHookForKey(string key)
        {
            return key switch
            {
                "charPanel" => "Char_panel_end",
                "invent" => "Invent_end",
                "craftingCategory" => "crafting_category_end",
                "skillMenu" => "Skill_menu_end",
                "journalMenu" => "Journal_menu_end",
                "tierName" => "Tier_name_end",
                "rarity" => "rarity_end",
                "prefix" => "prefix_end",
                "weapParam" => "weap_param_end",
                "otherHover" => "other_hover_end",
                "attitudeName" => "attitude_name_end",
                "attitudeDesc" => "attitude_desc_end",
                "charDesc" => "char_desc_end",
                "className" => "class_name_end",
                "charName" => "char_name_end",
                "countryDesc" => "country_desc_end",
                "raceName" => "race_name_end",
                "countryName" => "country_name_end",
                "sexDesc" => "sex_desc_end",
                "sexName" => "sex_name_end",
                "psychicName" => "psychic_name_end",
                "psychicMidText" => "psychic_mid_text_end",
                "psychicInfo" => "psychic_info_end",
                "psychicDesc" => "psychic_desc_end",
                "contextMenu" => "context_menu_end",
                "buttonHover" => "button_hover_end",
                "controlButtons" => "control_buttons_end",
                "tradeCategory" => "trade_category_end",
                "map" => "map_end",
                "townDesc" => "town_desc_end",
                "repSourcesPos" => "rep_sources_pos_end",
                "repSourcesNeg" => "rep_sources_neg_end",
                "repHistory" => "rep_history_end",
                "repPerks" => "rep_perks_end",
                "repPerksEffect" => "rep_perks_effect_end",
                "repPerksFlavor" => "rep_perks_flavor_end",
                "situationName" => "situation_name_end",
                "situationEffect" => "situation_effect_end",
                "situationDesc" => "situation_desc_end",
                "repOther" => "rep_other_end",
                "tradeTooltip" => "trade_tooltip_end",
                "skilltreeHover" => "skilltree_hover_end",
                "rumorName" => "rumor_name_end",
                "sight" => "sight_end",
                _ => throw new KeyNotFoundException($"Key '{key}' not found.")
            };
        }
        
        // Helper method to format a line for a key
        internal static string FormatLineForKey(string key, string id, LocalizedStrings text)
        {
            return key switch
            {
                "charPanel" 
                 or "invent"
                 or "craftingCategory"
                 or "skillMenu"
                 or "journalMenu"
                 or "tierName"
                 or "rarity"
                 or "prefix"
                 or "weapParam"
                 or "otherHover"
                 or "attitudeName"
                 or "attitudeDesc"
                 or "charDesc"
                 or "className"
                 or "charName"
                 or "countryDesc"
                 or "raceName"
                 or "countryName"
                 or "sexDesc"
                 or "sexName"
                 or "psychicName"
                 or "psychicMidText"
                 or "psychicInfo"
                 or "psychicDesc"
                 or "contextMenu"
                 or "buttonHover"
                 or "controlButtons"
                 or "tradeCategory"
                 or "map"
                 or "townDesc"
                 or "repSourcesPos"
                 or "repSourcesNeg"
                 or "repHistory"
                 or "repPerks"
                 or "repPerksEffect"
                 or "repPerksFlavor"
                 or "situationName"
                 or "situationEffect"
                 or "situationDesc"
                 or "repOther"
                 or "tradeTooltip"
                 or "skilltreeHover"
                 or "rumorName"
                 or "sight" 
                    => $"{id};{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};",
                _ => throw new KeyNotFoundException($"Key '{key}' not found.")
            };
        }
        
        // API methods to set the localized strings
        #region SETTERS
        public LocalizableTextBuilder WithId(string id)
        {
            _id = id;
            return this;
        }
        public LocalizableTextBuilder WithCharPanel(LocalizedStrings charPanel) => Add("charPanel", charPanel); 
        public LocalizableTextBuilder WithInvent(LocalizedStrings invent) => Add("invent", invent);
        public LocalizableTextBuilder WithCraftingCategory(LocalizedStrings craftingCategory) => Add("craftingCategory", craftingCategory);
        public LocalizableTextBuilder WithSkillMenu(LocalizedStrings skillMenu) => Add("skillMenu", skillMenu);
        public LocalizableTextBuilder WithJournalMenu(LocalizedStrings journalMenu) => Add("journalMenu", journalMenu);
        public LocalizableTextBuilder WithTierName(LocalizedStrings tierName) => Add("tierName", tierName);
        public LocalizableTextBuilder WithRarity(LocalizedStrings rarity) => Add("rarity", rarity);
        public LocalizableTextBuilder WithPrefix(LocalizedStrings prefix) => Add("prefix", prefix);
        public LocalizableTextBuilder WithWeapParam(LocalizedStrings weapParam) => Add("weapParam", weapParam);
        public LocalizableTextBuilder WithOtherHover(LocalizedStrings otherHover) => Add("otherHover", otherHover);
        public LocalizableTextBuilder WithAttitudeName(LocalizedStrings attitudeName) => Add("attitudeName", attitudeName);
        public LocalizableTextBuilder WithAttitudeDesc(LocalizedStrings attitudeDesc) => Add("attitudeDesc", attitudeDesc);
        public LocalizableTextBuilder WithCharDesc(LocalizedStrings charDesc) => Add("charDesc", charDesc);
        public LocalizableTextBuilder WithClassName(LocalizedStrings className) => Add("className", className);
        public LocalizableTextBuilder WithCharName(LocalizedStrings charName) => Add("charName", charName);
        public LocalizableTextBuilder WithCountryDesc(LocalizedStrings countryDesc) => Add("countryDesc", countryDesc);
        public LocalizableTextBuilder WithRaceName(LocalizedStrings raceName) => Add("raceName", raceName);
        public LocalizableTextBuilder WithCountryName(LocalizedStrings countryName) => Add("countryName", countryName);
        public LocalizableTextBuilder WithSexDesc(LocalizedStrings sexDesc) => Add("sexDesc", sexDesc);
        public LocalizableTextBuilder WithSexName(LocalizedStrings sexName) => Add("sexName", sexName);
        public LocalizableTextBuilder WithPsychicName(LocalizedStrings psychicName) => Add("psychicName", psychicName);
        public LocalizableTextBuilder WithPsychicMidText(LocalizedStrings psychicMidText) => Add("psychicMidText", psychicMidText);
        public LocalizableTextBuilder WithPsychicInfo(LocalizedStrings psychicInfo) => Add("psychicInfo", psychicInfo);
        public LocalizableTextBuilder WithPsychicDesc(LocalizedStrings psychicDesc) => Add("psychicDesc", psychicDesc);
        public LocalizableTextBuilder WithContextMenu(LocalizedStrings contextMenu) => Add("contextMenu", contextMenu);
        public LocalizableTextBuilder WithButtonHover(LocalizedStrings buttonHover) => Add("buttonHover", buttonHover);
        public LocalizableTextBuilder WithControlButtons(LocalizedStrings controlButtons) => Add("controlButtons", controlButtons);
        public LocalizableTextBuilder WithTradeCategory(LocalizedStrings tradeCategory) => Add("tradeCategory", tradeCategory);
        public LocalizableTextBuilder WithMap(LocalizedStrings map) => Add("map", map);
        public LocalizableTextBuilder WithTownDesc(LocalizedStrings townDesc) => Add("townDesc", townDesc);
        public LocalizableTextBuilder WithRepSourcesPos(LocalizedStrings repSourcesPos) => Add("repSourcesPos", repSourcesPos);
        public LocalizableTextBuilder WithRepSourcesNeg(LocalizedStrings repSourcesNeg) => Add("repSourcesNeg", repSourcesNeg);
        public LocalizableTextBuilder WithRepHistory(LocalizedStrings repHistory) => Add("repHistory", repHistory);
        public LocalizableTextBuilder WithRepPerks(LocalizedStrings repPerks) => Add("repPerks", repPerks);
        public LocalizableTextBuilder WithRepPerksEffect(LocalizedStrings repPerksEffect) => Add("repPerksEffect", repPerksEffect);
        public LocalizableTextBuilder WithRepPerksFlavor(LocalizedStrings repPerksFlavor) => Add("repPerksFlavor", repPerksFlavor);
        public LocalizableTextBuilder WithSituationName(LocalizedStrings situationName) => Add("situationName", situationName);
        public LocalizableTextBuilder WithSituationEffect(LocalizedStrings situationEffect) => Add("situationEffect", situationEffect);
        public LocalizableTextBuilder WithSituationDesc(LocalizedStrings situationDesc) => Add("situationDesc", situationDesc);
        public LocalizableTextBuilder WithRepOther(LocalizedStrings repOther) => Add("repOther", repOther);
        public LocalizableTextBuilder WithTradeTooltip(LocalizedStrings tradeTooltip) => Add("tradeTooltip", tradeTooltip);
        public LocalizableTextBuilder WithSkilltreeHover(LocalizedStrings skilltreeHover) => Add("skilltreeHover", skilltreeHover);
        public LocalizableTextBuilder WithRumorName(LocalizedStrings rumorName) => Add("rumorName", rumorName);
        public LocalizableTextBuilder WithSight(LocalizedStrings sight) => Add("sight", sight);
        #endregion

        // API method called to finish the builder and call the injector
        public void Inject()
        {
            if (_localizedStrings.Count > 0)
                DoInjectLocalizableTableText(_id, _localizedStrings);
            else
            {
                Log.Error("Failed to inject localizable text: Nothing to inject.");
                throw new ArgumentException("Failed to inject localizable text: Nothing to inject.");
            }
        }
    }

    // API method exposed to modders
    public static LocalizableTextBuilder InjectTableLocalizableText() => new();

    // Method actually responsible for the injection
    private static void DoInjectLocalizableTableText(string id, Dictionary<string, LocalizedStrings> localizedStrings)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_text";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));

        foreach ((string key, LocalizedStrings text) in localizedStrings)
        {
            // Get hook for the key
            string hook = LocalizableTextBuilder.GetHookForKey(key);
            
            // Find hook in table
            (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hook));
            if (foundLine == null)
                Log.Error($"Failed to inject {key} into table {tableName}: Hook '{hook}' not found");
            
            // Prepare line
            string newline = LocalizableTextBuilder.FormatLineForKey(key, id, text);
            
            // Add line to table
            table.Insert(ind, newline);
            Log.Information($"Injected {key} into table {tableName} at hook '{hook}'.");
        }
        ModLoader.SetTable(table, tableName);
    }
}