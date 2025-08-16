using ModShardLauncher.Mods;
using Serilog;
using System.Reflection;

namespace ModShardLauncherTest;
public class LocalizationWeaponTextTest
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
        string res = new LocalizationWeaponText("testItem", input, input)
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
        string output = "Wooden Sword;Деревянный меч;Wooden Sword;木剑;Holzschwert;Espada de madera;Épée en Bois;Spada di Legno;Espada de Madeira;Drewniany miecz;Tahta Kılıç;木製の剣;목검;";
        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Деревянный меч"}, 
            {ModLanguage.English, "Wooden Sword"}, 
            {ModLanguage.Chinese, "木剑"}, 
            {ModLanguage.German, "Holzschwert"}, 
            {ModLanguage.Spanish, "Espada de madera"}, 
            {ModLanguage.French, "Épée en Bois"}, 
            {ModLanguage.Italian, "Spada di Legno"}, 
            {ModLanguage.Portuguese, "Espada de Madeira"}, 
            {ModLanguage.Polish, "Drewniany miecz"}, 
            {ModLanguage.Turkish, "Tahta Kılıç"}, 
            {ModLanguage.Japanese, "木製の剣"}, 
            {ModLanguage.Korean, "목검"}
        };

        // Act
        string res = new LocalizationWeaponText("Wooden Sword", input, input)
            .CreateLine(selector)
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateInjectionWeaponTextsLocalization()
    {
        // Arrange
        string inputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""weapon_desc_end;""
conv.s.v
push.s ""weapon_name_end;""
conv.s.v", 2);

        string outputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""weapon_desc_end;""
conv.s.v
push.s ""Wooden Sword;Деревянный меч;Wooden Sword;木剑;Holzschwert;Espada de madera;Épée en Bois;Spada di Legno;Espada de Madeira;Drewniany miecz;Tahta Kılıç;木製の剣;목검;""
conv.s.v
push.s ""weapon_name_end;""
conv.s.v
push.s ""Wooden Sword;Деревянный меч;Wooden Sword;木剑;Holzschwert;Espada de madera;Épée en Bois;Spada di Legno;Espada de Madeira;Drewniany miecz;Tahta Kılıç;木製の剣;목검;""
conv.s.v", 4);

        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Деревянный меч"}, 
            {ModLanguage.English, "Wooden Sword"}, 
            {ModLanguage.Chinese, "木剑"}, 
            {ModLanguage.German, "Holzschwert"}, 
            {ModLanguage.Spanish, "Espada de madera"}, 
            {ModLanguage.French, "Épée en Bois"}, 
            {ModLanguage.Italian, "Spada di Legno"}, 
            {ModLanguage.Portuguese, "Espada de Madeira"}, 
            {ModLanguage.Polish, "Drewniany miecz"}, 
            {ModLanguage.Turkish, "Tahta Kılıç"}, 
            {ModLanguage.Japanese, "木製の剣"}, 
            {ModLanguage.Korean, "목검"}
        };

        LocalizationWeaponText[] Locs = new[] {new LocalizationWeaponText("Wooden Sword", input, input)};

        // Act
        string res = Msl.CreateInjectionWeaponTextsLocalization(Locs)(inputTable.Split('\n')).Collect();
       
        // Assert
        Assert.Equal(outputTable.Replace("\r\n", "\n"), res.Replace("\r\n", "\n"));
    }
}