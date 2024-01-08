using Xunit;
using System.Collections.Generic;

namespace ModShardLauncherTest
{
    public static class StringDataForTest
    {
        public static IEnumerable<object[]> StringData()
        {
            yield return new object[] { "var a = 0" };
            yield return new object[] { "var a = 0\nvar y = 1\ny = a + y\nreturn y" };
            yield return new object[] { "var a = 0\nvar y = 1\ny = a + y\nreturn y\naaa" };
        }

        public static IEnumerable<object[]> MatchData()
        {
            yield return new object[] { Match.Before };
            yield return new object[] { Match.Matching };
            yield return new object[] { Match.After };
        }

        public static IEnumerable<object[]> CrossData()
        {
           return StringData().SelectMany(x => MatchData().Select(y => new object[] { y[0], x[0] }));
        }
    }
    public class FlattenTest
    {
        [Theory]
        [InlineData(Match.Before)]
        [InlineData(Match.Matching)]
        [InlineData(Match.After)]
        public void Flatten_EmptyMatchString(Match m)
        {
            List<(Match, string)> ms = new()
            {
                (m, "")
            };
            Assert.Equal("".Split('\n'), ms.Flatten());
        }
        [Theory]
        [MemberData(nameof(StringDataForTest.CrossData), MemberType = typeof(StringDataForTest))]
        public void Flatten_NonEmptyMatchStrings(Match m, string input)
        {
            List<(Match, string)> ms = new();
            foreach (string s in input.Split('\n'))
            {
                ms.Add((m, s));
            }
            Assert.Equal(input.Split('\n'), ms.Flatten());
        }
    }

    public class CollectTest
    {
        [Fact]
        public void Collect_EmptyString()
        {
            Assert.Equal("", "".Split('\n').Collect());
        }
        
        [Theory]
        [InlineData(Match.Before)]
        [InlineData(Match.Matching)]
        [InlineData(Match.After)]
        public void Collect_EmptyMatchString(Match m)
        {
            List<(Match, string)> ms = new()
            {
                (m, "")
            };
            Assert.Equal("", ms.Collect());
        }

        [Theory]
        [MemberData(nameof(StringDataForTest.StringData), MemberType = typeof(StringDataForTest))]
        public void Collect_NonEmptyStrings(string input)
        {
            Assert.Equal(input, input.Split('\n').Collect());
        }
        
        [Theory]
        [MemberData(nameof(StringDataForTest.CrossData), MemberType = typeof(StringDataForTest))]
        public void Collect_NonEmptyMatchStrings(Match m, string input)
        {
            List<(Match, string)> ms = new();
            foreach (string s in input.Split('\n'))
            {
                ms.Add((m, s));
            }

            Assert.Equal(input, ms.Collect());
        }
    }

    public class MatchFromTest
    {

    }
}