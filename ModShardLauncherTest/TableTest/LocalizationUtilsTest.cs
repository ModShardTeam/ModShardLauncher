using ModShardLauncher.Mods;
using System.Reflection;

namespace ModShardLauncherTest;
public static class LocalizationUtilsData
{
    public const string oneLanguageString = "testEn";
    public const string multipleLanguagesString = "testRu;testEn;testCh";
    public const string allLanguagesString = "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr";
    public const string tableString = @":[0]
b [3]

> gml_Script_table_example (locals=0, argc=0)
:[1]
push.i 1
setowner.e
{0}
call.i @@NewGMLArray@@(argc={1})
ret.v

:[2]
exit.i

:[3]
push.i gml_Script_table_example
conv.i.v
pushi.e -1
conv.i.v
call.i method(argc=2)
dup.v 0
pushi.e -1
pop.v.v [stacktop]self.table_example
popz.v

:[end]";
}
public class ToDictTest
{
    [Theory]
    [InlineData(LocalizationUtilsData.oneLanguageString, 0)] // First element aka English as default language
    [InlineData(LocalizationUtilsData.oneLanguageString, 1)] // Second element, but there is none then first element aka English as default language
    public void ToDict_OneElement(string str, int index)
    {
        // Arrange
        Dictionary<ModLanguage, string> expectedResult = new() 
        {
            {ModLanguage.Russian, "testEn"}, {ModLanguage.English, "testEn"}, {ModLanguage.Chinese, "testEn"}, {ModLanguage.German, "testEn"}, {ModLanguage.Spanish, "testEn"}, 
            {ModLanguage.French, "testEn"}, {ModLanguage.Italian, "testEn"}, {ModLanguage.Portuguese, "testEn"}, {ModLanguage.Polish, "testEn"}, {ModLanguage.Turkish, "testEn"}, 
            {ModLanguage.Japanese, "testEn"}, {ModLanguage.Korean, "testEn"}
        };

        // Act
        MethodInfo? methodInfo = typeof(ModShardLauncher.Localization).GetMethod("ToDict", BindingFlags.NonPublic | BindingFlags.Static);
        if (methodInfo == null)
        {
            Assert.Fail("Cannot find the tested method ToDict");
        }
        
        object? result = methodInfo.Invoke(null, new object[] { str, index });
        if (result == null)
        {
            Assert.Fail("Invalid result from ToDict");
        }

        Dictionary<ModLanguage, string> res = (Dictionary<ModLanguage, string>)result;

        // Assert
        foreach (ModLanguage modLanguage in Localization.LanguageList)
        {
            Assert.Equal(expectedResult[modLanguage], res[modLanguage]);
        }
    }
    [Theory]
    [InlineData(LocalizationUtilsData.multipleLanguagesString)]
    public void ToDict_MultipleElements(string str)
    {
        // Arrange
        Dictionary<ModLanguage, string> expectedResult = new() 
        {
            {ModLanguage.Russian, "testRu"}, {ModLanguage.English, "testEn"}, {ModLanguage.Chinese, "testCh"}, {ModLanguage.German, "testEn"}, {ModLanguage.Spanish, "testEn"}, 
            {ModLanguage.French, "testEn"}, {ModLanguage.Italian, "testEn"}, {ModLanguage.Portuguese, "testEn"}, {ModLanguage.Polish, "testEn"}, {ModLanguage.Turkish, "testEn"}, 
            {ModLanguage.Japanese, "testEn"}, {ModLanguage.Korean, "testEn"}
        };

        // Act
        MethodInfo? methodInfo = typeof(Localization).GetMethod("ToDict", BindingFlags.NonPublic | BindingFlags.Static);
        if (methodInfo == null)
        {
            Assert.Fail("Cannot find the tested method ToDict");
        }
        
        object? result = methodInfo.Invoke(null, new object[] { str, 1 });
        if (result == null)
        {
            Assert.Fail("Invalid result from ToDict");
        }

        Dictionary<ModLanguage, string> res = (Dictionary<ModLanguage, string>)result;

        // Assert
        foreach (ModLanguage modLanguage in Localization.LanguageList)
        {
            Assert.Equal(expectedResult[modLanguage], res[modLanguage]);
        }
    }
    [Theory]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, 0)] // First element aka Russian as default language
    [InlineData(LocalizationUtilsData.multipleLanguagesString, 100)] // 101st element, but there is none then first element aka Russian as default language
    public void ToDict_MultipleElementsDifferentDefault(string str, int index)
    {
        // Arrange
        Dictionary<ModLanguage, string> expectedResult = new() 
        {
            {ModLanguage.Russian, "testRu"}, {ModLanguage.English, "testEn"}, {ModLanguage.Chinese, "testCh"}, {ModLanguage.German, "testRu"}, {ModLanguage.Spanish, "testRu"}, 
            {ModLanguage.French, "testRu"}, {ModLanguage.Italian, "testRu"}, {ModLanguage.Portuguese, "testRu"}, {ModLanguage.Polish, "testRu"}, {ModLanguage.Turkish, "testRu"}, 
            {ModLanguage.Japanese, "testRu"}, {ModLanguage.Korean, "testRu"}
        };

        // Act
        MethodInfo? methodInfo = typeof(Localization).GetMethod("ToDict", BindingFlags.NonPublic | BindingFlags.Static);
        if (methodInfo == null)
        {
            Assert.Fail("Cannot find the tested method ToDict");
        }
        
        object? result = methodInfo.Invoke(null, new object[] { str, index });
        if (result == null)
        {
            Assert.Fail("Invalid result from ToDict");
        }

        Dictionary<ModLanguage, string> res = (Dictionary<ModLanguage, string>)result;

        // Assert
        foreach (ModLanguage modLanguage in Localization.LanguageList)
        {
            Assert.Equal(expectedResult[modLanguage], res[modLanguage]);
        }
    }
    [Theory]
    [InlineData(LocalizationUtilsData.allLanguagesString)]
    public void ToDict_AllElements(string str)
    {
        // Arrange
        Dictionary<ModLanguage, string> expectedResult = new() 
        {
            {ModLanguage.Russian, "testRu"}, {ModLanguage.English, "testEn"}, {ModLanguage.Chinese, "testCh"}, {ModLanguage.German, "testGe"}, {ModLanguage.Spanish, "testSp"}, 
            {ModLanguage.French, "testFr"}, {ModLanguage.Italian, "testIt"}, {ModLanguage.Portuguese, "testPr"}, {ModLanguage.Polish, "testPl"}, {ModLanguage.Turkish, "testTu"}, 
            {ModLanguage.Japanese, "testJp"}, {ModLanguage.Korean, "testKr"}
        };

        // Act
        MethodInfo? methodInfo = typeof(Localization).GetMethod("ToDict", BindingFlags.NonPublic | BindingFlags.Static);
        if (methodInfo == null)
        {
            Assert.Fail("Cannot find the tested method ToDict");
        }
        
        object? result = methodInfo.Invoke(null, new object[] { str, 1 });
        if (result == null)
        {
            Assert.Fail("Invalid result from ToDict");
        }

        Dictionary<ModLanguage, string> res = (Dictionary<ModLanguage, string>)result;

        // Assert
        foreach (ModLanguage modLanguage in Localization.LanguageList)
        {
            Assert.Equal(expectedResult[modLanguage], res[modLanguage]);
        }
    }
}
public class CreateLineLocalizationSentenceTest
{
    [Theory]
    [InlineData(LocalizationUtilsData.oneLanguageString, "id;any;any;any;any;any;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "id;any;any;any;any;any;testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "id;any;any;any;any;any;testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;")]
    public void CreateLine(string str, string expectedResult)
    {
        // Arrange
        LocalizationSentence sentence = new("id", str);

        // Act
        string res = sentence.CreateLine(null).First();

        // Assert
        Assert.Equal(expectedResult, res);
    }
}