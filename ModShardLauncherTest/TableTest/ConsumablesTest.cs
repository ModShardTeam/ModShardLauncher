using ModShardLauncher.Mods;
using Serilog;
using System.Reflection;

namespace ModShardLauncherTest;
public class LocalizationItemTest
{
    [Theory]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "name")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "name")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;", "name")]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "effect")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "effect")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;", "effect")]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "description")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "description")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;", "description")]
    public void CreateLine(string input, string output, string selector)
    {
        // Arrange
        output = $"testItem;{output}//;";

        // Act
        string res = new LocalizationItem("testItem", input, input, input)
            .CreateLine(selector)
            .Collect();
        
        // Assert
        Assert.Equal(output, res);
    }
    [Theory]
    [InlineData("name")]
    [InlineData("effect")]
    [InlineData("description")]
    public void CreateLineFromExistingData(string selector)
    {
        // Arrange
        string output = "bandage;Бинт;Bandage;绷带;Verband;Venda;Bandage;Benda;Atadura;Bandaż;Bandaj;包帯;붕대;//;";
        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Бинт"}, 
            {ModLanguage.English, "Bandage"}, 
            {ModLanguage.Chinese, "绷带"}, 
            {ModLanguage.German, "Verband"}, 
            {ModLanguage.Spanish, "Venda"}, 
            {ModLanguage.French, "Bandage"}, 
            {ModLanguage.Italian, "Benda"}, 
            {ModLanguage.Portuguese, "Atadura"}, 
            {ModLanguage.Polish, "Bandaż"}, 
            {ModLanguage.Turkish, "Bandaj"}, 
            {ModLanguage.Japanese, "包帯"}, 
            {ModLanguage.Korean, "붕대"}
        };

        // Act
        string res = new LocalizationItem("bandage", input, input, input)
            .CreateLine(selector)
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateInjectionItemsLocalization()
    {
        // Arrange
        string inputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""consum_desc_end;""
conv.s.v
push.s ""consum_mid_end;""
conv.s.v
push.s ""consum_name_end;""
conv.s.v", 3);

        string outputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""consum_desc_end;""
conv.s.v
push.s ""bandage;Бинт;Bandage;绷带;Verband;Venda;Bandage;Benda;Atadura;Bandaż;Bandaj;包帯;붕대;//;""
conv.s.v
push.s ""consum_mid_end;""
conv.s.v
push.s ""bandage;Бинт;Bandage;绷带;Verband;Venda;Bandage;Benda;Atadura;Bandaż;Bandaj;包帯;붕대;//;""
conv.s.v
push.s ""consum_name_end;""
conv.s.v
push.s ""bandage;Бинт;Bandage;绷带;Verband;Venda;Bandage;Benda;Atadura;Bandaż;Bandaj;包帯;붕대;//;""
conv.s.v", 6);

        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Бинт"},
            {ModLanguage.English, "Bandage"},
            {ModLanguage.Chinese, "绷带"},
            {ModLanguage.German, "Verband"},
            {ModLanguage.Spanish, "Venda"},
            {ModLanguage.French, "Bandage"},
            {ModLanguage.Italian, "Benda"},
            {ModLanguage.Portuguese, "Atadura"},
            {ModLanguage.Polish, "Bandaż"},
            {ModLanguage.Turkish, "Bandaj"},
            {ModLanguage.Japanese, "包帯"},
            {ModLanguage.Korean, "붕대"}
        };

        LocalizationItem[] Locs = new[] {new LocalizationItem("bandage", input, input, input)};

        // Act
        string res = Msl.CreateInjectionItemsLocalization(Locs)(inputTable.Split('\n')).Collect();
       
        // Assert
        Assert.Equal(outputTable.Replace("\r\n", "\n"), res.Replace("\r\n", "\n"));
    }
}