using ModShardLauncher.Mods;
using Serilog;
using System.Reflection;

namespace ModShardLauncherTest;
public class LocalizationSpeechTest
{
    [Theory]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;")]
    public void CreateLine(string input, string output)
    {
        // Arrange
        string start = "kill;kill;kill;kill;kill;kill;kill;kill;kill;kill;kill;kill;kill;";
        string end = "kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;";
        output = $"{start}\n;{output}\n;{output}\n{end}";

        // Act
        string res = new LocalizationSpeech("kill", input, input)
            .CreateLine(null)
            .Collect();
        
        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateLineFromExistingData()
    {
        // Arrange
        string start = "kill;kill;kill;kill;kill;kill;kill;kill;kill;kill;kill;kill;kill;";
        string end = "kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;";
        string txt = ";Ха!;Ha!;哈！;Ha!;¡Ja!;Ha !;Ha!;Rá!;Ha!;Hah!;はあっ！;흥!;";
        string output = $"{start}\n{txt}\n{txt}\n{end}";

        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Ха!"}, 
            {ModLanguage.English, "Ha!"}, 
            {ModLanguage.Chinese, "哈！"}, 
            {ModLanguage.German, "Ha!"}, 
            {ModLanguage.Spanish, "¡Ja!"}, 
            {ModLanguage.French, "Ha !"}, 
            {ModLanguage.Italian, "Ha!"}, 
            {ModLanguage.Portuguese, "Rá!"}, 
            {ModLanguage.Polish, "Ha!"}, 
            {ModLanguage.Turkish, "Hah!"}, 
            {ModLanguage.Japanese, "はあっ！"}, 
            {ModLanguage.Korean, "흥!"}
        };

        // Act
        string res = new LocalizationSpeech("kill", input, input)
            .CreateLine(null)
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateInjectionSkillsLocalization()
    {
        // Arrange
        string inputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""FORBIDDEN MAGIC;""
conv.s.v", 1);

        string outputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""FORBIDDEN MAGIC;""
conv.s.v
push.s ""kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;kill_end;""
conv.s.v
push.s "";Ха!;Ha!;哈！;Ha!;¡Ja!;Ha !;Ha!;Rá!;Ha!;Hah!;はあっ！;흥!;""
conv.s.v
push.s "";Ха!;Ha!;哈！;Ha!;¡Ja!;Ha !;Ha!;Rá!;Ha!;Hah!;はあっ！;흥!;""
conv.s.v
push.s ""kill;kill;kill;kill;kill;kill;kill;kill;kill;kill;kill;kill;kill;""
conv.s.v", 5);

        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Ха!"}, 
            {ModLanguage.English, "Ha!"}, 
            {ModLanguage.Chinese, "哈！"}, 
            {ModLanguage.German, "Ha!"}, 
            {ModLanguage.Spanish, "¡Ja!"}, 
            {ModLanguage.French, "Ha !"}, 
            {ModLanguage.Italian, "Ha!"}, 
            {ModLanguage.Portuguese, "Rá!"}, 
            {ModLanguage.Polish, "Ha!"}, 
            {ModLanguage.Turkish, "Hah!"}, 
            {ModLanguage.Japanese, "はあっ！"}, 
            {ModLanguage.Korean, "흥!"}
        };

        LocalizationSpeech[] Locs = new[] {new LocalizationSpeech("kill", input, input)};

        // Act
        string res = Msl.CreateInjectionSpeechesLocalization(Locs)(inputTable.Split('\n')).Collect();
       
        // Assert
        Assert.Equal(outputTable.Replace("\r\n", "\n"), res.Replace("\r\n", "\n"));
    }
}