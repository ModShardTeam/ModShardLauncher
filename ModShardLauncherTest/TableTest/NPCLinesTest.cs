using ModShardLauncher.Mods;
using System.Reflection;

namespace ModShardLauncherTest;
public class LocalizationSentenceTest
{
    [Theory]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;")]
    public void CreateLine(string input, string output)
    {
        // Arrange
        output = $"testItem;any;any;any;any;any;{output}";

        // Act
        string res = new LocalizationSentence("testItem", input)
            .CreateLine(null)
            .Collect();
        
        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateLineFromExistingData()
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
            {ModLanguage.Polish, "Tak?.."}, 
            {ModLanguage.Turkish, "Yes?.."}, 
            {ModLanguage.Japanese, "何か…？"}, 
            {ModLanguage.Korean, "뭔가...?"}
        };

        // Act
        string res = new LocalizationSentence("greeting", input)
            .CreateLine(null)
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateInjectionSentenceLocalization()
    {
        // Arrange
        string inputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""NPC - GREETINGS;""
conv.s.v", 1);

        string outputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""NPC - GREETINGS;""
conv.s.v
push.s ""greeting;any;any;any;any;any;Да?..;Yes?..;什么事儿?;Ja ...?;Yes?..;Oui...?;Sì...?;Sim..?;Tak?..;Yes?..;何か…？;뭔가...?;""
conv.s.v", 2);

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
            {ModLanguage.Polish, "Tak?.."}, 
            {ModLanguage.Turkish, "Yes?.."}, 
            {ModLanguage.Japanese, "何か…？"}, 
            {ModLanguage.Korean, "뭔가...?"}
        };

        LocalizationSentence[] Locs = new[] {new LocalizationSentence("greeting", input)};

        // Act
        string res = Msl.CreateInjectionDialogLocalization(Locs)(inputTable.Split('\n')).Collect();
       
        // Assert
        Assert.Equal(outputTable.Replace("\r\n", "\n"), res.Replace("\r\n", "\n"));
    }
}