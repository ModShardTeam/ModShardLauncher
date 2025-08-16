using ModShardLauncher.Mods;
using Serilog;
using System.Reflection;

namespace ModShardLauncherTest;
public class LocalizationBookTest
{
    [Theory]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "name")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "name")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;", "name")]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "content")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "content")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;", "content")]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "mid_text")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "mid_text")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;", "mid_text")]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "desc")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "desc")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;", "desc")]
    [InlineData(LocalizationUtilsData.oneLanguageString, "testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "type")]
    [InlineData(LocalizationUtilsData.multipleLanguagesString, "testRu;testEn;testCh;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;testEn;", "type")]
    [InlineData(LocalizationUtilsData.allLanguagesString, "testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;", "type")]
    public void CreateLine(string input, string output, string selector)
    {
        // Arrange
        output = $"testItem;{output}";

        // Act
        string res = new LocalizationBook("testItem", input, input, input, input, input)
            .CreateLine(selector)
            .Collect();
        
        // Assert
        Assert.Equal(output, res);
    }
    [Theory]
    [InlineData("name")]
    [InlineData("content")]
    [InlineData("mid_text")]
    [InlineData("desc")]
    [InlineData("type")]
    public void CreateLineFromExistingData(string selector)
    {
        // Arrange
        string output = "book;Монастырская книга;Monastic Book;《修院纪实》;Buch der Abtei;Libro monacal;Livre monastique;Libro Monastico;Livro Monástico;Klasztorna księga;Manastır Kitabı;修道士の本;수도원 일지;";
        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Монастырская книга"}, 
            {ModLanguage.English, "Monastic Book"}, 
            {ModLanguage.Chinese, "《修院纪实》"}, 
            {ModLanguage.German, "Buch der Abtei"}, 
            {ModLanguage.Spanish, "Libro monacal"}, 
            {ModLanguage.French, "Livre monastique"}, 
            {ModLanguage.Italian, "Libro Monastico"}, 
            {ModLanguage.Portuguese, "Livro Monástico"}, 
            {ModLanguage.Polish, "Klasztorna księga"}, 
            {ModLanguage.Turkish, "Manastır Kitabı"}, 
            {ModLanguage.Japanese, "修道士の本"}, 
            {ModLanguage.Korean, "수도원 일지"}
        };

        // Act
        string res = new LocalizationBook("book", input, input, input, input, input)
            .CreateLine(selector)
            .Collect();

        // Assert
        Assert.Equal(output, res);
    }
    [Fact]
    public void CreateInjectionBooksLocalization()
    {
        // Arrange
        string inputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""book_type_end;""
conv.s.v
push.s ""book_desc_end;""
conv.s.v
push.s ""book_mid_text_end;""
conv.s.v
push.s ""book_content_end;""
conv.s.v
push.s ""book_name_end;""
conv.s.v", 5);

        string outputTable = string.Format(LocalizationUtilsData.tableString, @"push.s ""book_type_end;""
conv.s.v
push.s ""book;Монастырская книга;Monastic Book;《修院纪实》;Buch der Abtei;Libro monacal;Livre monastique;Libro Monastico;Livro Monástico;Klasztorna księga;Manastır Kitabı;修道士の本;수도원 일지;""
conv.s.v
push.s ""book_desc_end;""
conv.s.v
push.s ""book;Монастырская книга;Monastic Book;《修院纪实》;Buch der Abtei;Libro monacal;Livre monastique;Libro Monastico;Livro Monástico;Klasztorna księga;Manastır Kitabı;修道士の本;수도원 일지;""
conv.s.v
push.s ""book_mid_text_end;""
conv.s.v
push.s ""book;Монастырская книга;Monastic Book;《修院纪实》;Buch der Abtei;Libro monacal;Livre monastique;Libro Monastico;Livro Monástico;Klasztorna księga;Manastır Kitabı;修道士の本;수도원 일지;""
conv.s.v
push.s ""book_content_end;""
conv.s.v
push.s ""book;Монастырская книга;Monastic Book;《修院纪实》;Buch der Abtei;Libro monacal;Livre monastique;Libro Monastico;Livro Monástico;Klasztorna księga;Manastır Kitabı;修道士の本;수도원 일지;""
conv.s.v
push.s ""book_name_end;""
conv.s.v
push.s ""book;Монастырская книга;Monastic Book;《修院纪实》;Buch der Abtei;Libro monacal;Livre monastique;Libro Monastico;Livro Monástico;Klasztorna księga;Manastır Kitabı;修道士の本;수도원 일지;""
conv.s.v", 10);

        Dictionary<ModLanguage, string> input = new() 
        {
            {ModLanguage.Russian, "Монастырская книга"}, 
            {ModLanguage.English, "Monastic Book"}, 
            {ModLanguage.Chinese, "《修院纪实》"}, 
            {ModLanguage.German, "Buch der Abtei"}, 
            {ModLanguage.Spanish, "Libro monacal"}, 
            {ModLanguage.French, "Livre monastique"}, 
            {ModLanguage.Italian, "Libro Monastico"}, 
            {ModLanguage.Portuguese, "Livro Monástico"}, 
            {ModLanguage.Polish, "Klasztorna księga"}, 
            {ModLanguage.Turkish, "Manastır Kitabı"}, 
            {ModLanguage.Japanese, "修道士の本"}, 
            {ModLanguage.Korean, "수도원 일지"}
        };

        LocalizationBook[] Locs = new[] {new LocalizationBook("book", input, input, input, input, input)};

        // Act
        string res = Msl.CreateInjectionBooksLocalization(Locs)(inputTable.Split('\n')).Collect();
       
        // Assert
        Assert.Equal(outputTable.Replace("\r\n", "\n"), res.Replace("\r\n", "\n"));
    }
}