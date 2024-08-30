using ModShardLauncher.Mods;
using Serilog;
using System.Reflection;

namespace ModShardLauncherTest;
public class LocalizationTextTreeTest
{
    [Theory]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "tier")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "tier")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;", "tier")]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "hover")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "hover")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;", "hover")]
    public void CreateLine(string input, string output, string selector)
    {
        // Arrange
        output = $"testItem;{output}";

        // Act
        string res = new LocalizationTextTree("testItem", input, input)
            .CreateLine(selector)
            .Collect();
        
        // Assert
        Assert.Equal(output, res);
    }
    [Theory]
    [InlineData("tier")]
    [InlineData("hover")]
    public void CreateLineFromExistingData(string selector)
    {
        // Arrange
        string output = "Swords;Мечи;Swords;单手刀剑;Schwerter;Espadas;Épées;Spade;Espadas;Miecze;Kılıçlar;剣;검;";
        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Мечи"}, 
            {ModLanguage.English, "Swords"}, 
            {ModLanguage.Chinese, "单手刀剑"}, 
            {ModLanguage.German, "Schwerter"}, 
            {ModLanguage.Spanish, "Espadas"}, 
            {ModLanguage.French, "Épées"}, 
            {ModLanguage.Italian, "Spade"}, 
            {ModLanguage.Portuguese, "Espadas"}, 
            {ModLanguage.Polish, "Miecze"}, 
            {ModLanguage.Turkish, "Kılıçlar"}, 
            {ModLanguage.Japanese, "剣"}, 
            {ModLanguage.Korean, "검"}
        };

        // Act
        string res = new LocalizationTextTree("Swords", input, input)
            .CreateLine(selector)
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateInjectionTextTreesLocalization()
    {
        // Arrange
        string inputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""skilltree_hover;""
conv.s.v
push.s ""Tier_name;""
conv.s.v", 2);

        string outputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""Swords;Мечи;Swords;单手刀剑;Schwerter;Espadas;Épées;Spade;Espadas;Miecze;Kılıçlar;剣;검;""
conv.s.v
push.s ""skilltree_hover;""
conv.s.v
push.s ""Swords;Мечи;Swords;单手刀剑;Schwerter;Espadas;Épées;Spade;Espadas;Miecze;Kılıçlar;剣;검;""
conv.s.v
push.s ""Tier_name;""
conv.s.v", 4);

        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Мечи"}, 
            {ModLanguage.English, "Swords"}, 
            {ModLanguage.Chinese, "单手刀剑"}, 
            {ModLanguage.German, "Schwerter"}, 
            {ModLanguage.Spanish, "Espadas"}, 
            {ModLanguage.French, "Épées"}, 
            {ModLanguage.Italian, "Spade"}, 
            {ModLanguage.Portuguese, "Espadas"}, 
            {ModLanguage.Polish, "Miecze"}, 
            {ModLanguage.Turkish, "Kılıçlar"}, 
            {ModLanguage.Japanese, "剣"}, 
            {ModLanguage.Korean, "검"}
        };

        LocalizationTextTree[] Locs = new[] {new LocalizationTextTree("Swords", input, input)};

        // Act
        string res = Msl.CreateInjectionTextTreesLocalization(Locs)(inputTable.Split('\n')).Collect();
       
        // Assert
        Assert.Equal(outputTable.Replace("\r\n", "\n"), res.Replace("\r\n", "\n"));
    }
}
public class LocalizationTextRarityTest
{
    [Theory]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;")]
    public void CreateLine(string input, string output)
    {
        // Arrange
        output = $"testItem;{output}";

        // Act
        string res = new LocalizationTextRarity("testItem", input)
            .CreateLine(null)
            .Collect();
        
        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateLineFromExistingData()
    {
        // Arrange
        string output = "1;обычный / обычная / обычное / обычные;common;普通;gewöhnlicher / gewöhnliche / gewöhnliches / gewöhnliche;común / común / comunes / comunes;commun / commune / communs / communes;oggetto comune - ;comum;Zwyczajny / Zwyczajna / Zwyczajne / Zwyczajne;sıradan;コモン;평범한;";
        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "обычный / обычная / обычное / обычные"}, 
            {ModLanguage.English, "common"}, 
            {ModLanguage.Chinese, "普通"}, 
            {ModLanguage.German, "gewöhnlicher / gewöhnliche / gewöhnliches / gewöhnliche"}, 
            {ModLanguage.Spanish, "común / común / comunes / comunes"}, 
            {ModLanguage.French, "commun / commune / communs / communes"}, 
            {ModLanguage.Italian, "oggetto comune - "}, 
            {ModLanguage.Portuguese, "comum"}, 
            {ModLanguage.Polish, "Zwyczajny / Zwyczajna / Zwyczajne / Zwyczajne"}, 
            {ModLanguage.Turkish, "sıradan"}, 
            {ModLanguage.Japanese, "コモン"}, 
            {ModLanguage.Korean, "평범한"}
        };

        // Act
        string res = new LocalizationTextRarity("1", input)
            .CreateLine(null)
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateInjectionTextRaritysLocalization()
    {
        // Arrange
        string inputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""rarity;""
conv.s.v", 1);

        string outputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""1;обычный / обычная / обычное / обычные;common;普通;gewöhnlicher / gewöhnliche / gewöhnliches / gewöhnliche;común / común / comunes / comunes;commun / commune / communs / communes;oggetto comune - ;comum;Zwyczajny / Zwyczajna / Zwyczajne / Zwyczajne;sıradan;コモン;평범한;""
conv.s.v
push.s ""rarity;""
conv.s.v", 2);

        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "обычный / обычная / обычное / обычные"}, 
            {ModLanguage.English, "common"}, 
            {ModLanguage.Chinese, "普通"}, 
            {ModLanguage.German, "gewöhnlicher / gewöhnliche / gewöhnliches / gewöhnliche"}, 
            {ModLanguage.Spanish, "común / común / comunes / comunes"}, 
            {ModLanguage.French, "commun / commune / communs / communes"}, 
            {ModLanguage.Italian, "oggetto comune - "}, 
            {ModLanguage.Portuguese, "comum"}, 
            {ModLanguage.Polish, "Zwyczajny / Zwyczajna / Zwyczajne / Zwyczajne"}, 
            {ModLanguage.Turkish, "sıradan"}, 
            {ModLanguage.Japanese, "コモン"}, 
            {ModLanguage.Korean, "평범한"}
        };

        LocalizationTextRarity[] Locs = new[] {new LocalizationTextRarity("1", input)};

        // Act
        string res = Msl.CreateInjectionTextRaritysLocalization(Locs)(inputTable.Split('\n')).Collect();
       
        // Assert
        Assert.Equal(outputTable.Replace("\r\n", "\n"), res.Replace("\r\n", "\n"));
    }
}