using ModShardLauncher.Mods;
using System.Reflection;

namespace ModShardLauncherTest;
public class LocalizationNameTest
{
    [Theory]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;")]
    public void CreateLine(string input, string output)
    {
        // Arrange
        output = $";{output}";

        // Act
        string res = new LocalizationName(input)
            .CreateLine(null)
            .Collect();
        
        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateLineFromExistingData()
    {
        // Arrange
        string output = ";Адал;Adal;阿达尔;Adal;Adal;Adal;Adal;Adal;Adal;Adal;アダル;에이들;";
        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Адал"}, 
            {ModLanguage.English, "Adal"}, 
            {ModLanguage.Chinese, "阿达尔"}, 
            {ModLanguage.German, "Adal"}, 
            {ModLanguage.Spanish, "Adal"}, 
            {ModLanguage.French, "Adal"}, 
            {ModLanguage.Italian, "Adal"}, 
            {ModLanguage.Portuguese, "Adal"}, 
            {ModLanguage.Polish, "Adal"}, 
            {ModLanguage.Turkish, "Adal"}, 
            {ModLanguage.Japanese, "アダル"}, 
            {ModLanguage.Korean, "에이들"}
        };

        // Act
        string res = new LocalizationName(input)
            .CreateLine(null)
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateInjectionNamesLocalization()
    {
        // Arrange
        string inputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""Names_end;""
conv.s.v", 1);

        string outputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""Names_end;""
conv.s.v
push.s "";Адал;Adal;阿达尔;Adal;Adal;Adal;Adal;Adal;Adal;Adal;アダル;에이들;""
conv.s.v", 2);

        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Адал"}, 
            {ModLanguage.English, "Adal"}, 
            {ModLanguage.Chinese, "阿达尔"}, 
            {ModLanguage.German, "Adal"}, 
            {ModLanguage.Spanish, "Adal"}, 
            {ModLanguage.French, "Adal"}, 
            {ModLanguage.Italian, "Adal"}, 
            {ModLanguage.Portuguese, "Adal"}, 
            {ModLanguage.Polish, "Adal"}, 
            {ModLanguage.Turkish, "Adal"}, 
            {ModLanguage.Japanese, "アダル"}, 
            {ModLanguage.Korean, "에이들"}
        };

        LocalizationName[] Locs = new[] {new LocalizationName(input)};

        // Act
        string res = Msl.CreateInjectionNamesLocalization(Locs)(inputTable.Split('\n')).Collect();
       
        // Assert
        Assert.Equal(outputTable.Replace("\r\n", "\n"), res.Replace("\r\n", "\n"));
    }
}
public class LocalizationQuestNameTest
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
        string res = new LocalizationQuestName("testItem", input)
            .CreateLine(null)
            .Collect();
        
        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateLineFromExistingData()
    {
        // Arrange
        string output = "47;Арвон;Arvon;阿冯;Arvon;Arvon;Arvon;Arvon;Arvon;Arvon;Arvon;アルヴォン;아르본;";
        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Арвон"}, 
            {ModLanguage.English, "Arvon"}, 
            {ModLanguage.Chinese, "阿冯"}, 
            {ModLanguage.German, "Arvon"}, 
            {ModLanguage.Spanish, "Arvon"}, 
            {ModLanguage.French, "Arvon"}, 
            {ModLanguage.Italian, "Arvon"}, 
            {ModLanguage.Portuguese, "Arvon"}, 
            {ModLanguage.Polish, "Arvon"}, 
            {ModLanguage.Turkish, "Arvon"}, 
            {ModLanguage.Japanese, "アルヴォン"}, 
            {ModLanguage.Korean, "아르본"}
        };

        // Act
        string res = new LocalizationQuestName("47", input)
            .CreateLine(null)
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateInjectionNamesLocalization()
    {
        // Arrange
        string inputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""Constant_Name_end;""
conv.s.v", 1);

        string outputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""Constant_Name_end;""
conv.s.v
push.s ""47;Арвон;Arvon;阿冯;Arvon;Arvon;Arvon;Arvon;Arvon;Arvon;Arvon;アルヴォン;아르본;""
conv.s.v", 2);

        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Арвон"}, 
            {ModLanguage.English, "Arvon"}, 
            {ModLanguage.Chinese, "阿冯"}, 
            {ModLanguage.German, "Arvon"}, 
            {ModLanguage.Spanish, "Arvon"}, 
            {ModLanguage.French, "Arvon"}, 
            {ModLanguage.Italian, "Arvon"}, 
            {ModLanguage.Portuguese, "Arvon"}, 
            {ModLanguage.Polish, "Arvon"}, 
            {ModLanguage.Turkish, "Arvon"}, 
            {ModLanguage.Japanese, "アルヴォン"}, 
            {ModLanguage.Korean, "아르본"}
        };

        LocalizationQuestName[] Locs = new[] {new LocalizationQuestName("47", input)};

        // Act
        string res = Msl.CreateInjectionQuestNamesLocalization(Locs)(inputTable.Split('\n')).Collect();
       
        // Assert
        Assert.Equal(outputTable.Replace("\r\n", "\n"), res.Replace("\r\n", "\n"));
    }
}
public class LocalizationOccupationNameTest
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
        string res = new LocalizationOccupationName("testItem", input)
            .CreateLine(null)
            .Collect();
        
        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateLineFromExistingData()
    {
        // Arrange
        string output = "messenger;Гонец;Courier;急使;Kurier;;Messager;Corriere;Portador;Kurier;Courier;配達人;특사;";
        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Гонец"}, 
            {ModLanguage.English, "Courier"}, 
            {ModLanguage.Chinese, "急使"}, 
            {ModLanguage.German, "Kurier"}, 
            {ModLanguage.Spanish, ""}, 
            {ModLanguage.French, "Messager"}, 
            {ModLanguage.Italian, "Corriere"}, 
            {ModLanguage.Portuguese, "Portador"}, 
            {ModLanguage.Polish, "Kurier"}, 
            {ModLanguage.Turkish, "Courier"}, 
            {ModLanguage.Japanese, "配達人"}, 
            {ModLanguage.Korean, "특사"}
        };

        // Act
        string res = new LocalizationOccupationName("messenger", input)
            .CreateLine(null)
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateInjectionNamesLocalization()
    {
        // Arrange
        string inputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""NPC_info_end;""
conv.s.v", 1);

        string outputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""NPC_info_end;""
conv.s.v
push.s ""messenger;Гонец;Courier;急使;Kurier;;Messager;Corriere;Portador;Kurier;Courier;配達人;특사;""
conv.s.v", 2);

        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Гонец"}, 
            {ModLanguage.English, "Courier"}, 
            {ModLanguage.Chinese, "急使"}, 
            {ModLanguage.German, "Kurier"}, 
            {ModLanguage.Spanish, ""}, 
            {ModLanguage.French, "Messager"}, 
            {ModLanguage.Italian, "Corriere"}, 
            {ModLanguage.Portuguese, "Portador"}, 
            {ModLanguage.Polish, "Kurier"}, 
            {ModLanguage.Turkish, "Courier"}, 
            {ModLanguage.Japanese, "配達人"}, 
            {ModLanguage.Korean, "특사"}
        };

        LocalizationOccupationName[] Locs = new[] {new LocalizationOccupationName("messenger", input)};

        // Act
        string res = Msl.CreateInjectionOccupationNamesLocalization(Locs)(inputTable.Split('\n')).Collect();
       
        // Assert
        Assert.Equal(outputTable.Replace("\r\n", "\n"), res.Replace("\r\n", "\n"));
    }
}