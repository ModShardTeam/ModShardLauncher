using Xunit;
using System.Collections.Generic;
using ModShardLauncher.Mods;
using System.Reflection;
using ModShardLauncher.Extensions;

namespace ModShardLauncherTest
{
    public static class TableUtilsData
    {
        public const string oneLanguageString = "testEn";
        public const string multipleLanguagesString = "testRu;testEn;testCh";
        public const string allLanguagesString = "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr";
    }

    public class ToDictTest
    {
        [Theory]
        [InlineData(TableUtilsData.oneLanguageString, 0)]
        [InlineData(TableUtilsData.oneLanguageString, 1)]
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
        [InlineData(TableUtilsData.multipleLanguagesString)]
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
        [InlineData(TableUtilsData.multipleLanguagesString, 0)]
        [InlineData(TableUtilsData.multipleLanguagesString, 100)]
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
        [InlineData(TableUtilsData.allLanguagesString)]
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

    public class CreateLineLocalizationItemTest
    {
        [Fact]
        public void CreateLine()
        {
            // Arrange
            string expectedResult = "testItem;testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;//;";
            Dictionary<ModLanguage, string> input = new() 
            {
                {ModLanguage.Russian, "testRu"}, {ModLanguage.English, "testEn"}, {ModLanguage.Chinese, "testCh"}, {ModLanguage.German, "testGe"}, {ModLanguage.Spanish, "testSp"}, 
                {ModLanguage.French, "testFr"}, {ModLanguage.Italian, "testIt"}, {ModLanguage.Portuguese, "testPr"}, {ModLanguage.Polish, "testPl"}, {ModLanguage.Turkish, "testTu"}, 
                {ModLanguage.Japanese, "testJp"}, {ModLanguage.Korean, "testKr"}
            };

            // Act
            MethodInfo? methodInfo = typeof(LocalizationItem).GetMethod("CreateLine", BindingFlags.NonPublic | BindingFlags.Static);
            if (methodInfo == null)
            {
                Assert.Fail("Cannot find the tested method CreateLine");
            }
            
            object? result = methodInfo.Invoke(null, new object[] { "testItem", input });
            if (result == null)
            {
                Assert.Fail("Invalid result from CreateLine");
            }

            string res = (string)result;

            // Assert
            Assert.Equal(expectedResult, res);
            
        }
    }

    public class CreateLineLocalizationSentenceTest
    {
        [Theory]
        [InlineData(TableUtilsData.oneLanguageString, "id;any;any;any;any;any;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
        [InlineData(TableUtilsData.multipleLanguagesString, "id;any;any;any;any;any;testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;")]
        [InlineData(TableUtilsData.allLanguagesString, "id;any;any;any;any;any;testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;")]
        public void CreateLine(string str, string expectedResult)
        {
            // Arrange
            LocalizationSentence sentence = new("id", str);

            // Act
            string res = sentence.CreateLine();

            // Assert
            Assert.Equal(expectedResult, res);
        }
    }
}