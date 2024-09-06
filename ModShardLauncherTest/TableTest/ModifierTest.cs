using ModShardLauncher.Mods;
using Serilog;
using System.Reflection;

namespace ModShardLauncherTest;
public class LocalizationModifierTest
{
    [Theory]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "name")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "name")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;", "name")]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "description")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "description")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;", "description")]
    public void CreateLine(string input, string output, string selector)
    {
        // Arrange
        output = $"testItem;{output}";

        // Act
        string res = new LocalizationModifier("testItem", input, input)
            .CreateLine(selector)
            .Collect();
        
        // Assert
        Assert.Equal(output, res);
    }
    [Theory]
    [InlineData("name")]
    [InlineData("description")]
    public void CreateLineFromExistingData(string selector)
    {
        // Arrange
        string output = "o_db_blind;Слепота;Blindness;盲目;Blindheit;Ceguera;Cécité;Cecità;Cegueira;Oślepienie;Körlük;盲目;맹목;";
        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Слепота"}, 
            {ModLanguage.English, "Blindness"}, 
            {ModLanguage.Chinese, "盲目"}, 
            {ModLanguage.German, "Blindheit"}, 
            {ModLanguage.Spanish, "Ceguera"}, 
            {ModLanguage.French, "Cécité"}, 
            {ModLanguage.Italian, "Cecità"}, 
            {ModLanguage.Portuguese, "Cegueira"}, 
            {ModLanguage.Polish, "Oślepienie"}, 
            {ModLanguage.Turkish, "Körlük"}, 
            {ModLanguage.Japanese, "盲目"}, 
            {ModLanguage.Korean, "맹목"}
        };

        // Act
        string res = new LocalizationModifier("o_db_blind", input, input)
            .CreateLine(selector)
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateInjectionModifiersLocalization()
    {
        // Arrange
        string inputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""buff_desc_end;""
conv.s.v
push.s ""buff_name_end;""
conv.s.v", 2);

        string outputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""buff_desc_end;""
conv.s.v
push.s ""o_db_blind;Слепота;Blindness;盲目;Blindheit;Ceguera;Cécité;Cecità;Cegueira;Oślepienie;Körlük;盲目;맹목;""
conv.s.v
push.s ""buff_name_end;""
conv.s.v
push.s ""o_db_blind;Слепота;Blindness;盲目;Blindheit;Ceguera;Cécité;Cecità;Cegueira;Oślepienie;Körlük;盲目;맹목;""
conv.s.v", 4);

        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Слепота"}, 
            {ModLanguage.English, "Blindness"}, 
            {ModLanguage.Chinese, "盲目"}, 
            {ModLanguage.German, "Blindheit"}, 
            {ModLanguage.Spanish, "Ceguera"}, 
            {ModLanguage.French, "Cécité"}, 
            {ModLanguage.Italian, "Cecità"}, 
            {ModLanguage.Portuguese, "Cegueira"}, 
            {ModLanguage.Polish, "Oślepienie"}, 
            {ModLanguage.Turkish, "Körlük"}, 
            {ModLanguage.Japanese, "盲目"}, 
            {ModLanguage.Korean, "맹목"}
        };

        LocalizationModifier[] Locs = new[] {new LocalizationModifier("o_db_blind", input, input)};

        // Act
        string res = Msl.CreateInjectionModifiersLocalization(Locs)(inputTable.Split('\n')).Collect();
       
        // Assert
        Assert.Equal(outputTable.Replace("\r\n", "\n"), res.Replace("\r\n", "\n"));
    }
}