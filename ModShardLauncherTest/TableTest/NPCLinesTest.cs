using ModShardLauncher.Mods;
using System.Reflection;

namespace ModShardLauncherTest;
public class LocalizationItemTest
{
    [Theory]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;")]
    public void CreateLine(string input, string output)
    {
        // Arrange
        output = $"testItem;{input}//;";

        // Act
        string res = new LocalizationItem("testItem", input, input, input)
            .CreateLine("name")
            .Collect();
        
        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void TestName()
    {
        // Arrange
        string output = "greeting;any;any;any;any;any;Да?..;Yes?..;什么事儿?;Ja ...?;Yes?..;Oui...?;Sì...?;Sim..?;Tak?..;Yes?..;何か…？;뭔가...?;";
        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Да?.."}, 
            {ModLanguage.English, "Yes?.."}, 
            {ModLanguage.Chinese, "什么事儿?"}, 
            {ModLanguage.German, "Ja ...?"}, 
            {ModLanguage.Spanish, "Yes?.."}, 
            {ModLanguage.French, "Oui...?"}, 
            {ModLanguage.Italian, "Sì...?"}, 
            {ModLanguage.Portuguese, "Sim..?"}, 
            {ModLanguage.Polish, "testEn"}, 
            {ModLanguage.Turkish, "testEn"}, 
            {ModLanguage.Japanese, "testEn"}, 
            {ModLanguage.Korean, "testEn"}
        };

        // Act
        string res = new LocalizationItem("greeting", input, input, input)
            .CreateLine("name")
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
}