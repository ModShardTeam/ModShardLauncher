using ModShardLauncher.Mods;
using Serilog;
using System.Reflection;

namespace ModShardLauncherTest;
public class LocalizationSkillTest
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
        string res = new LocalizationSkill("testItem", input, input)
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
        string output = "Backwards_Dash;Отпрыгивание;Jump Away;后撤;Wegspringen;Salto;Bond en Arrière;Balzo;Pular Fora;Odskok;Uzaklaşma;飛びのき;뒤로 뛰기;";
        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Отпрыгивание"}, 
            {ModLanguage.English, "Jump Away"}, 
            {ModLanguage.Chinese, "后撤"}, 
            {ModLanguage.German, "Wegspringen"}, 
            {ModLanguage.Spanish, "Salto"}, 
            {ModLanguage.French, "Bond en Arrière"}, 
            {ModLanguage.Italian, "Balzo"}, 
            {ModLanguage.Portuguese, "Pular Fora"}, 
            {ModLanguage.Polish, "Odskok"}, 
            {ModLanguage.Turkish, "Uzaklaşma"}, 
            {ModLanguage.Japanese, "飛びのき"}, 
            {ModLanguage.Korean, "뒤로 뛰기"}
        };

        // Act
        string res = new LocalizationSkill("Backwards_Dash", input, input)
            .CreateLine(selector)
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateInjectionSkillsLocalization()
    {
        // Arrange
        string inputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""skill_desc;""
conv.s.v
push.s ""skill_name;""
conv.s.v", 2);

        string outputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""Backwards_Dash;Отпрыгивание;Jump Away;后撤;Wegspringen;Salto;Bond en Arrière;Balzo;Pular Fora;Odskok;Uzaklaşma;飛びのき;뒤로 뛰기;""
conv.s.v
push.s ""skill_desc;""
conv.s.v
push.s ""Backwards_Dash;Отпрыгивание;Jump Away;后撤;Wegspringen;Salto;Bond en Arrière;Balzo;Pular Fora;Odskok;Uzaklaşma;飛びのき;뒤로 뛰기;""
conv.s.v
push.s ""skill_name;""
conv.s.v", 4);

        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Отпрыгивание"}, 
            {ModLanguage.English, "Jump Away"}, 
            {ModLanguage.Chinese, "后撤"}, 
            {ModLanguage.German, "Wegspringen"}, 
            {ModLanguage.Spanish, "Salto"}, 
            {ModLanguage.French, "Bond en Arrière"}, 
            {ModLanguage.Italian, "Balzo"}, 
            {ModLanguage.Portuguese, "Pular Fora"}, 
            {ModLanguage.Polish, "Odskok"}, 
            {ModLanguage.Turkish, "Uzaklaşma"}, 
            {ModLanguage.Japanese, "飛びのき"}, 
            {ModLanguage.Korean, "뒤로 뛰기"}
        };

        LocalizationSkill[] Locs = new[] {new LocalizationSkill("Backwards_Dash", input, input)};

        // Act
        string res = Msl.CreateInjectionSkillsLocalization(Locs)(inputTable.Split('\n')).Collect();
       
        // Assert
        Assert.Equal(outputTable.Replace("\r\n", "\n"), res.Replace("\r\n", "\n"));
    }
}