using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace ModShardLauncherTest
{
    public static class StringDataForTest
    {
        public const string oneLine = "var a = 0";
        public const string multipleLines = "var a = 0\nvar y = 1\ny = a + y\nreturn y";
        public const string randomBlock = @"/*

depth = 90

depth -= 1000


target_array = [257910, 257910, 257909, 257909]

event_inherited()
    with (oCamera)
depthshift = 0
    alarm[1] = 1


blend = 4279667175
event_inherited()
    }
}


isQuest = 1
event_inherited()
}
    var steal = ceil((0.1 * argument0))

    scr_setside(o_floor_target)
event_inherited()
if (skill == ""Magma_Rain"")
target_tag = ""S1""
eventsArrayLength = 0
can_broke = 1
    return _value;
{
{
event_perform_object(child_skill, ev_create, 0)
loot_script = gml_Script_scr_loot_leprosoriumBookcase
depth -= 2000
depth = -5
draw_sprite(s_settings_scrollbar_track, 0, ((x + rightContainerX) + rightContainerWidth), ((y + adaptiveOffsetY) - 1))
position_tag = ""r_Osbrookhouse02""
fillSurfaceMobs = 0
            {

HP = (10 + random(15))
            return argument0;
}

chance = 50

with (target)
    scr_audio_play_at(choose(208, 205, 1803, 202))
child_skill = o_skill_summon_blood_golem
event_inherited()

start_x = (x + 52)
myfloor_counter = ""H1""



event_inherited()
{
depth = (-y)


descID = ""elf""
}
event_inherited()
depth = (-y)
alphaPercent = 0.4
audio_group_set_gain(4, global.gain_sfx, 0)

{
                    if (!__is_undefined(rumor_id))

    else if (argument_count == 0)
loot_script = gml_Script_scr_loot_contFood

event_user(0)
snd = -4
instance_destroy()
    var array = scr_weapon_array_type()

else
stack = 1
}

    {

    var _percent_hp = scr_percent_hp(0.01)
startdepth = -180
        var _is_CD = ds_map_find_value(_map, ""CD"")
event_inherited()

loot_script = gml_Script_scr_loot_contGeneric

                var _yy = ds_map_find_value(_corpseMap, ""y"")
            speed = 0
event_inherited()
state = 25";
    }
    public class FlattenTest
    {
        [Theory]
        [InlineData(Match.Before)]
        [InlineData(Match.Matching)]
        [InlineData(Match.After)]
        public void Flatten_EmptyMatchString(Match m)
        {
            // Arrange
            List<(Match, string)> matchStringList = new()
            {
                (m, "")
            };

            // Act
            IEnumerable<string> matchStringList_flatten = matchStringList.Flatten();

            // Assert
            Assert.Equal("".Split('\n'), matchStringList_flatten);
        }
        [Theory]
        [InlineData(Match.Before, StringDataForTest.oneLine)]
        [InlineData(Match.Matching, StringDataForTest.oneLine)]
        [InlineData(Match.After, StringDataForTest.oneLine)]
        [InlineData(Match.Before, StringDataForTest.multipleLines)]
        [InlineData(Match.Matching, StringDataForTest.multipleLines)]
        [InlineData(Match.After, StringDataForTest.multipleLines)]
        [InlineData(Match.Before, StringDataForTest.randomBlock)]
        [InlineData(Match.Matching, StringDataForTest.randomBlock)]
        [InlineData(Match.After, StringDataForTest.randomBlock)]
        public void Flatten_NonEmptyMatchStrings(Match m, string input)
        {
            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach (string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<string> matchStringList_flatten = matchStringList.Flatten();

            // Assert
            Assert.Equal(input.Split('\n'), matchStringList_flatten);
        }
    }

    public class CollectTest
    {
        [Fact]
        public void Collect_EmptyString()
        {
            // Arrange
            IEnumerable<string> stringList = "".Split('\n');

            // Act
            string stringList_collect = stringList.Collect();

            // Assert
            Assert.Equal("", stringList_collect);
        }
        
        [Theory]
        [InlineData(Match.Before)]
        [InlineData(Match.Matching)]
        [InlineData(Match.After)]
        public void Collect_EmptyMatchString(Match m)
        {
            // Arrange
            List<(Match, string)> matchStringList = new()
            {
                (m, "")
            };

            // Act
            string matchStringList_collect = matchStringList.Collect();

            // Assert
            Assert.Equal("", matchStringList_collect);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void Collect_NonEmptyStrings(string input)
        {
            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            string stringList_collect = stringList.Collect();

            // Assert
            Assert.Equal(input, stringList_collect);
        }
        
        [Theory]
        [InlineData(Match.Before, StringDataForTest.oneLine)]
        [InlineData(Match.Matching, StringDataForTest.oneLine)]
        [InlineData(Match.After, StringDataForTest.oneLine)]
        [InlineData(Match.Before, StringDataForTest.multipleLines)]
        [InlineData(Match.Matching, StringDataForTest.multipleLines)]
        [InlineData(Match.After, StringDataForTest.multipleLines)]
        [InlineData(Match.Before, StringDataForTest.randomBlock)]
        [InlineData(Match.Matching, StringDataForTest.randomBlock)]
        [InlineData(Match.After, StringDataForTest.randomBlock)]
        public void Collect_NonEmptyMatchStrings(Match m, string input)
        {
            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach (string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            string matchStringList_collect = matchStringList.Collect();

            // Assert
            Assert.Equal(input, matchStringList_collect);
        }
    }

    public class MatchFromTest
    {
        [Fact]
        public void MatchFrom_EmptyString_WithEmptyString()
        {
            // Reference
            List<(Match, string)> matchStringListReference = new()
            {
                (Match.Matching, "")
            };

            // Arrange
            IEnumerable<string> stringList = "".Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFrom = stringList.MatchFrom("");

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFrom);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void MatchFrom_EmptyString_WithNonEmptyString(string input)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new()
            {
                (Match.Before, "")
            };

            // Arrange
            IEnumerable<string> stringList = "".Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFrom = stringList.MatchFrom(input);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFrom);
        }
        
        [Theory]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void MatchFrom_NonEmptyString_WithEmptyString(string input)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            Match m = Match.Matching;
            foreach (string s in input.Split('\n'))
            {
                matchStringListReference.Add((m, s));
                m = Match.After;
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFrom = stringList.MatchFrom("");

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFrom);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void MatchFrom_NonEmptyString_WithItself(string input)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach (string s in input.Split('\n'))
            {
                matchStringListReference.Add((Match.Matching, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFrom = stringList.MatchFrom(input);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFrom);
        }
        
        [Theory]
        [InlineData(StringDataForTest.oneLine, "return")]
        [InlineData(StringDataForTest.multipleLines, "b")]
        [InlineData(StringDataForTest.randomBlock, "aaaaaaaaaaaaaaaaaaaaaa")]
        public void MatchFrom_NonEmptyString_NoMatch(string input, string stringToMatch)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringListReference.Add((Match.Before, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFrom = stringList.MatchFrom(stringToMatch);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFrom);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, "var", 0)]
        [InlineData(StringDataForTest.multipleLines, "y =", 1)]
        [InlineData(StringDataForTest.randomBlock, "4279667175", 15)]
        public void MatchFrom_NonEmptyString_WithOneLine(string input, string stringToMatch, int ind)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i == ind)
                    matchStringListReference.Add((Match.Matching, s));
                else if (i < ind)
                    matchStringListReference.Add((Match.Before, s));
                else
                    matchStringListReference.Add((Match.After, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFrom = stringList.MatchFrom(stringToMatch);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFrom);
        }

        [Theory]
        [InlineData(StringDataForTest.multipleLines, "y =\na + y", 1, 2)] // this block is continuous from line 1 to line 2
        [InlineData(StringDataForTest.randomBlock, "257910, 257910\n\nevent_inher\nwit", 7, 10)] // this block is continuous from line 7 to line 10
        [InlineData(StringDataForTest.multipleLines, "y =\nurn y", 1, 1)] // this block is not continuous (includes line 1 and line 3)
        [InlineData(StringDataForTest.randomBlock, "257910, 257910\n\nwit\ndepthshift = 0", 7, 8)] // this block is not continuous (includes line 7, 8, 10 and 11)
        public void MatchFrom_NonEmptyString_WithBlocks(string input, string stringToMatch, int start, int end)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i >= start && i <= end)
                    matchStringListReference.Add((Match.Matching, s));
                else if (i < start)
                    matchStringListReference.Add((Match.Before, s));
                else
                    matchStringListReference.Add((Match.After, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFrom = stringList.MatchFrom(stringToMatch);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFrom);
        }
    }
    public class MatchBelowTest
    {
        [Fact]
        public void MatchBelow_EmptyString_WithEmptyString()
        {
            // Reference
            List<(Match, string)> matchStringListReference = new()
            {
                (Match.Before, "")
            };

            // Arrange
            IEnumerable<string> stringList = "".Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchBelow = stringList.MatchBelow("", 0);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchBelow);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void MatchBelow_EmptyString_WithNonEmptyString(string input)
        {
            // Reference
            
            List<(Match, string)> matchStringListReference = new()
            {
                (Match.Before, "")
            };

            // Arrange
            IEnumerable<string> stringList = "".Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchBelow = stringList.MatchBelow(input, 0);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchBelow);
        }
        
        [Theory]
        [InlineData(StringDataForTest.multipleLines, 0)]
        [InlineData(StringDataForTest.multipleLines, 1)]
        [InlineData(StringDataForTest.randomBlock, 0)]
        [InlineData(StringDataForTest.randomBlock, 20)]
        public void MatchBelow_NonEmptyString_WithEmptyString(string input, int len)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach ((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i == 0)
                    matchStringListReference.Add((Match.Before, s));
                else if (i > len)
                    matchStringListReference.Add((Match.After, s));
                else
                    matchStringListReference.Add((Match.Matching, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchBelow = stringList.MatchBelow("", len);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchBelow);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, 0)]
        [InlineData(StringDataForTest.multipleLines, 0)]
        [InlineData(StringDataForTest.randomBlock, 0)]   
        [InlineData(StringDataForTest.oneLine, 1)]
        [InlineData(StringDataForTest.multipleLines, 1)]
        [InlineData(StringDataForTest.randomBlock, 1)]
        [InlineData(StringDataForTest.oneLine, 5)]
        [InlineData(StringDataForTest.multipleLines, 5)]
        [InlineData(StringDataForTest.randomBlock, 5)]
        public void MatchBelow_NonEmptyString_WithItself(string input, int len)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach (string s in input.Split('\n'))
            {
                matchStringListReference.Add((Match.Before, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchBelow = stringList.MatchBelow(input, len);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchBelow);
        }
        
        [Theory]
        [InlineData(StringDataForTest.oneLine, "return", 0)]
        [InlineData(StringDataForTest.multipleLines, "b", 0)]
        [InlineData(StringDataForTest.randomBlock, "aaaaaaaaaaaaaaaaaaaaaa", 0)]
        [InlineData(StringDataForTest.oneLine, "return", 1)]
        [InlineData(StringDataForTest.multipleLines, "b", 1)]
        [InlineData(StringDataForTest.randomBlock, "aaaaaaaaaaaaaaaaaaaaaa", 1)]
        [InlineData(StringDataForTest.oneLine, "return", 5)]
        [InlineData(StringDataForTest.multipleLines, "b", 5)]
        [InlineData(StringDataForTest.randomBlock, "aaaaaaaaaaaaaaaaaaaaaa", 5)]
        public void MatchBelow_NonEmptyString_NoMatch(string input, string stringToMatch, int len)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringListReference.Add((Match.Before, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchBelow = stringList.MatchBelow(stringToMatch, len);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchBelow);
        }

        [Theory]
        [InlineData(StringDataForTest.multipleLines, "y =", 1, 1)]
        [InlineData(StringDataForTest.randomBlock, "4279667175", 15, 30)]
        public void MatchBelow_NonEmptyString_WithOneLine(string input, string stringToMatch, int ind, int len)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i > ind && i <= ind + len)
                    matchStringListReference.Add((Match.Matching, s));
                else if (i <= ind)
                    matchStringListReference.Add((Match.Before, s));
                else
                    matchStringListReference.Add((Match.After, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchBelow = stringList.MatchBelow(stringToMatch, len);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchBelow);
        }

        [Theory]
        [InlineData(StringDataForTest.multipleLines, "y =\na + y", 1, 2, 1)] // this block is continuous from line 1 to line 2
        [InlineData(StringDataForTest.randomBlock, "257910, 257910\n\nevent_inher\nwit", 7, 10, 2)] // this block is continuous from line 7 to line 10
        [InlineData(StringDataForTest.multipleLines, "y =\nurn y", 1, 1, 1)] // this block is not continuous (includes line 1 and line 3)
        [InlineData(StringDataForTest.randomBlock, "257910, 257910\n\nwit\ndepthshift = 0", 7, 8, 10)] // this block is not continuous (includes line 7, 8, 10 and 11)
        public void MatchBelow_NonEmptyString_WithBlocks(string input, string stringToMatch, int _, int end, int len)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i > end && i <= end + len)
                    matchStringListReference.Add((Match.Matching, s));
                else if (i <= end)
                    matchStringListReference.Add((Match.Before, s));
                else
                    matchStringListReference.Add((Match.After, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchBelow = stringList.MatchBelow(stringToMatch, len);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchBelow);
        }
    }
    public class MatchFromUntilTest
    {
        [Fact]
        public void MatchFromUntil_EmptyString_FromEmptyStringUntilEmptyString()
        {
            // Reference
            List<(Match, string)> matchStringListReference = new()
            {
                (Match.Matching, "")
            };

            // Arrange
            IEnumerable<string> stringList = "".Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFromUntil = stringList.MatchFromUntil("", "");

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFromUntil);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void MatchFromUntil_EmptyString_FromNonEmptyStringUntilEmptyString(string input)
        {
            // Reference
            
            List<(Match, string)> matchStringListReference = new()
            {
                (Match.Before, "")
            };

            // Arrange
            IEnumerable<string> stringList = "".Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFromUntil = stringList.MatchFromUntil(input, "");

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFromUntil);
        }
        
        [Theory]
        [InlineData(StringDataForTest.oneLine, "return")]
        [InlineData(StringDataForTest.multipleLines, "b")]
        [InlineData(StringDataForTest.randomBlock, "aaaaaaaaaaaaaaaaaaaaaa")]
        public void MatchFromUntil_FromNoMatch_UntilEmptyString(string input, string from)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach (string s in input.Split('\n'))
            {
                matchStringListReference.Add((Match.Before, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFromUntil = stringList.MatchFromUntil(from, "");

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFromUntil);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void MatchFromUntil_FromItself(string input)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach (string s in input.Split('\n'))
            {
                matchStringListReference.Add((Match.Matching, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFromUntil = stringList.MatchFromUntil(input, "");

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFromUntil);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, "var", 0)]
        [InlineData(StringDataForTest.multipleLines, "y =", 1)]
        [InlineData(StringDataForTest.randomBlock, "4279667175", 15)]
        public void MatchFromUntil_FromOneLineUntilEmpty(string input, string from, int ind)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i == ind || i == ind + 1)
                    matchStringListReference.Add((Match.Matching, s));
                else if (i < ind)
                    matchStringListReference.Add((Match.Before, s));
                else
                    matchStringListReference.Add((Match.After, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFromUntil = stringList.MatchFromUntil(from, "");

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFromUntil);
        }
        
        [Theory]
        [InlineData(StringDataForTest.multipleLines, "y =", "y = a +", 1, 2)]
        [InlineData(StringDataForTest.randomBlock, "4279667175", "isQuest", 15, 7)]
        [InlineData(StringDataForTest.multipleLines, "y =\na + y", "eturn y", 1, 3)] // this block is continuous from line 1 to line 2, until line 3
        [InlineData(StringDataForTest.randomBlock, "257910, 257910\n\nevent_inher\nwit", "alarm", 7, 6)] // this block is continuous from line 7 to line 10, until line 12
        [InlineData(StringDataForTest.multipleLines, "y =\nurn y", "eturn y", 1, 3)] // this block is not continuous (includes line 1 and line 3), until line 3
        [InlineData(StringDataForTest.randomBlock, "257910, 257910\n\nwit\ndepthshift = 0", "nd = 42796", 7, 9)] // this block is not continuous (includes line 7, 8, 10 and 11), until line 15
        public void MatchFromUntil(string input, string from, string until, int start, int len)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i >= start && i < start + len)
                    matchStringListReference.Add((Match.Matching, s));
                else if (i < start)
                    matchStringListReference.Add((Match.Before, s));
                else
                    matchStringListReference.Add((Match.After, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFromUntil = stringList.MatchFromUntil(from, until);

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFromUntil);
        }
    }

    public class MatchAllTest
    {
        [Theory]
        [InlineData("")]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void MatchAll(string input)
        {
            // Reference
            List<(Match, string)> matchStringListReference = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringListReference.Add((Match.Matching, s));
            }

            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<(Match, string)> stringList_matchFrom = stringList.MatchAll();

            // Assert
            Assert.Equal(matchStringListReference, stringList_matchFrom);
        }
    }
    public class PeekTest
    {
        [Theory]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void Peek_String(string input)
        {
            // Arrange
            IEnumerable<string> stringList = input.Split('\n');

            // Act
            IEnumerable<string> stringList_peek = stringList.Peek();

            // Assert
            Assert.Equal(input.Split('\n'), stringList_peek);
        }
        
        [Theory]
        [InlineData(StringDataForTest.oneLine, Match.Before)]
        [InlineData(StringDataForTest.multipleLines, Match.Before)]
        [InlineData(StringDataForTest.randomBlock, Match.Before)]
        [InlineData(StringDataForTest.oneLine, Match.Matching)]
        [InlineData(StringDataForTest.multipleLines, Match.Matching)]
        [InlineData(StringDataForTest.randomBlock, Match.Matching)]
        [InlineData(StringDataForTest.oneLine, Match.After)]
        [InlineData(StringDataForTest.multipleLines, Match.After)]
        [InlineData(StringDataForTest.randomBlock, Match.After)]
        public void Peek_MatchString(string input, Match m)
        {
            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<(Match, string)> matchStringList_peek = matchStringList.Peek();

            // Assert
            Assert.Equal(matchStringList, matchStringList_peek);
        }
    }
    public class RemoveTest
    {
        [Theory]
        [InlineData(StringDataForTest.oneLine, Match.Before)]
        [InlineData(StringDataForTest.multipleLines, Match.Before)]
        [InlineData(StringDataForTest.randomBlock, Match.Before)]
        [InlineData(StringDataForTest.oneLine, Match.After)]
        [InlineData(StringDataForTest.multipleLines, Match.After)]
        [InlineData(StringDataForTest.randomBlock, Match.After)]
        public void Remove_DoesNothing(string input, Match m)
        {
            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<string> matchStringList_remove = matchStringList.Remove();

            // Assert
            Assert.Equal(input.Split('\n'), matchStringList_remove);
        }
        
        [Theory]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void Remove_AllLines(string input)
        {
            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((Match.Matching, s));
            }

            // Act
            IEnumerable<string> matchStringList_remove = matchStringList.Remove();

            // Assert
            Assert.Equal(Enumerable.Empty<string>(), matchStringList_remove);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, 0, 1)]
        [InlineData(StringDataForTest.multipleLines, 0, 2)]
        [InlineData(StringDataForTest.randomBlock, 0, 10)]
        [InlineData(StringDataForTest.randomBlock, 7, 14)]
        public void Remove_SpecificLines(string input, int start, int len)
        {
            // Reference
            List<string> stringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i < start || i >= start + len)
                    stringListReference.Add(s);
            }

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i < start)
                    matchStringList.Add((Match.Before, s));
                else if (i >= start + len)
                    matchStringList.Add((Match.After, s));
                else
                    matchStringList.Add((Match.Matching, s));
            }

            // Act
            IEnumerable<string> matchStringList_remove = matchStringList.Remove();

            // Assert
            Assert.Equal(stringListReference, matchStringList_remove);
        }
    }
    public class KeepOnlyTest
    {
        [Theory]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void KeepOnly_AllLines(string input)
        {
            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((Match.Matching, s));
            }

            // Act
            IEnumerable<string> matchStringList_keepOnly = matchStringList.KeepOnly();

            // Assert
            Assert.Equal(input.Split('\n'), matchStringList_keepOnly);
        }
        [Theory]
        [InlineData(StringDataForTest.oneLine, Match.Before)]
        [InlineData(StringDataForTest.multipleLines, Match.Before)]
        [InlineData(StringDataForTest.randomBlock, Match.Before)]
        [InlineData(StringDataForTest.oneLine, Match.After)]
        [InlineData(StringDataForTest.multipleLines, Match.After)]
        [InlineData(StringDataForTest.randomBlock, Match.After)]
        public void KeepOnly_NoLinesKept(string input, Match m)
        {
            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<string> matchStringList_keepOnly = matchStringList.KeepOnly();

            // Assert
            Assert.Equal(Enumerable.Empty<string>(), matchStringList_keepOnly);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, 0, 1)]
        [InlineData(StringDataForTest.multipleLines, 0, 2)]
        [InlineData(StringDataForTest.randomBlock, 0, 10)]
        [InlineData(StringDataForTest.randomBlock, 7, 14)]
        public void KeepOnly_SpecificLines(string input, int start, int len)
        {
            // Reference
            List<string> stringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i >= start && i < start + len)
                    stringListReference.Add(s);
            }

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i < start)
                    matchStringList.Add((Match.Before, s));
                else if (i >= start + len)
                    matchStringList.Add((Match.After, s));
                else
                    matchStringList.Add((Match.Matching, s));
            }

            // Act
            IEnumerable<string> matchStringList_keepOnly = matchStringList.KeepOnly();

            // Assert
            Assert.Equal(stringListReference, matchStringList_keepOnly);
        }
    }

    public class FilterMatchTest
    {
        [Theory]
        [InlineData(StringDataForTest.oneLine, 0, 1)]
        [InlineData(StringDataForTest.multipleLines, 0, 2)]
        [InlineData(StringDataForTest.randomBlock, 0, 10)]
        [InlineData(StringDataForTest.randomBlock, 7, 14)]
        public void FilterMatch_EqualsBefore(string input, int start, int len)
        {
            // Reference
            List<string> stringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i < start)
                    stringListReference.Add(s);
            }

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i < start)
                    matchStringList.Add((Match.Before, s));
                else if (i >= start + len)
                    matchStringList.Add((Match.After, s));
                else
                    matchStringList.Add((Match.Matching, s));
            }

            // Act
            IEnumerable<string> stringList_filterMatch = matchStringList.FilterMatch(x => x == Match.Before);

            // Assert
            Assert.Equal(stringListReference, stringList_filterMatch);
        }
        [Theory]
        [InlineData(StringDataForTest.oneLine, 0, 1)]
        [InlineData(StringDataForTest.multipleLines, 0, 2)]
        [InlineData(StringDataForTest.randomBlock, 0, 10)]
        [InlineData(StringDataForTest.randomBlock, 7, 14)]
        public void FilterMatch_EqualsMatching(string input, int start, int len) // same as KeepOnly in theory
        {
            // Reference
            List<string> stringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i >= start && i < start + len)
                    stringListReference.Add(s);
            }

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i < start)
                    matchStringList.Add((Match.Before, s));
                else if (i >= start + len)
                    matchStringList.Add((Match.After, s));
                else
                    matchStringList.Add((Match.Matching, s));
            }

            // Act
            IEnumerable<string> stringList_filterMatch = matchStringList.FilterMatch(x => x == Match.Matching);

            // Assert
            Assert.Equal(stringListReference, stringList_filterMatch);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, 0, 1)]
        [InlineData(StringDataForTest.multipleLines, 0, 2)]
        [InlineData(StringDataForTest.randomBlock, 0, 10)]
        [InlineData(StringDataForTest.randomBlock, 7, 14)]
        public void FilterMatch_EqualsAfter(string input, int start, int len)
        {
            // Reference
            List<string> stringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i >= start + len)
                    stringListReference.Add(s);
            }

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i < start)
                    matchStringList.Add((Match.Before, s));
                else if (i >= start + len)
                    matchStringList.Add((Match.After, s));
                else
                    matchStringList.Add((Match.Matching, s));
            }

            // Act
            IEnumerable<string> stringList_filterMatch = matchStringList.FilterMatch(x => x == Match.After);

            // Assert
            Assert.Equal(stringListReference, stringList_filterMatch);
        }
        
        [Theory]
        [InlineData(StringDataForTest.oneLine, 0, 1)]
        [InlineData(StringDataForTest.multipleLines, 0, 2)]
        [InlineData(StringDataForTest.randomBlock, 0, 10)]
        [InlineData(StringDataForTest.randomBlock, 7, 14)]
        public void FilterMatch_NotEqualsBefore(string input, int start, int len)
        {
            // Reference
            List<string> stringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i >= start)
                    stringListReference.Add(s);
            }

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i < start)
                    matchStringList.Add((Match.Before, s));
                else if (i >= start + len)
                    matchStringList.Add((Match.After, s));
                else
                    matchStringList.Add((Match.Matching, s));
            }

            // Act
            IEnumerable<string> stringList_filterMatch = matchStringList.FilterMatch(x => x != Match.Before);

            // Assert
            Assert.Equal(stringListReference, stringList_filterMatch);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, 0, 1)]
        [InlineData(StringDataForTest.multipleLines, 0, 2)]
        [InlineData(StringDataForTest.randomBlock, 0, 10)]
        [InlineData(StringDataForTest.randomBlock, 7, 14)]
        public void FilterMatch_NotEqualsMatching(string input, int start, int len) // same as Remove in theory
        {
            // Reference
            List<string> stringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i < start || i >= start + len)
                    stringListReference.Add(s);
            }

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i < start)
                    matchStringList.Add((Match.Before, s));
                else if (i >= start + len)
                    matchStringList.Add((Match.After, s));
                else
                    matchStringList.Add((Match.Matching, s));
            }

            // Act
            IEnumerable<string> stringList_filterMatch = matchStringList.FilterMatch(x => x != Match.Matching);

            // Assert
            Assert.Equal(stringListReference, stringList_filterMatch);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, 0, 1)]
        [InlineData(StringDataForTest.multipleLines, 0, 2)]
        [InlineData(StringDataForTest.randomBlock, 0, 10)]
        [InlineData(StringDataForTest.randomBlock, 7, 14)]
        public void FilterMatch_NotEqualsAfter(string input, int start, int len)
        {
            // Reference
            List<string> stringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i < start + len)
                    stringListReference.Add(s);
            }

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i < start)
                    matchStringList.Add((Match.Before, s));
                else if (i >= start + len)
                    matchStringList.Add((Match.After, s));
                else
                    matchStringList.Add((Match.Matching, s));
            }

            // Act
            IEnumerable<string> stringList_filterMatch = matchStringList.FilterMatch(x => x != Match.After);

            // Assert
            Assert.Equal(stringListReference, stringList_filterMatch);
        }
    }

    public class InsertBelowTest
    {
        [Theory]
        [InlineData(StringDataForTest.oneLine, Match.Before)]
        [InlineData(StringDataForTest.multipleLines, Match.Before)]
        [InlineData(StringDataForTest.randomBlock, Match.Before)]
        public void InsertBelow_NothingHappensIfNoMatch(string input, Match m)
        {
            string toInsert = "Aaaaaa";

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<string> stringList_insertBelow = matchStringList.InsertBelow(toInsert);

            // Assert
            Assert.Equal(input.Split('\n'), stringList_insertBelow);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, Match.Matching)]
        [InlineData(StringDataForTest.multipleLines, Match.Matching)]
        [InlineData(StringDataForTest.randomBlock, Match.Matching)]
        public void InsertBelow_AllMatchingInsertOneline(string input, Match m)
        {
            string toInsert = "Aaaaaa";

            // Reference
            List<string> stringListReference = input.Split('\n').ToList();
            stringListReference.Add(toInsert);

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<string> stringList_insertBelow = matchStringList.InsertBelow(toInsert);

            // Assert
            Assert.Equal(stringListReference, stringList_insertBelow);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, Match.Matching)]
        [InlineData(StringDataForTest.multipleLines, Match.Matching)]
        [InlineData(StringDataForTest.randomBlock, Match.Matching)]
        public void InsertBelow_AllMatchingInsertSomeLines(string input, Match m)
        {
            string toInsert = StringDataForTest.multipleLines;

            // Reference
            List<string> stringListReference = input.Split('\n').ToList();
            stringListReference.AddRange(toInsert.Split('\n'));

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<string> stringList_insertBelow = matchStringList.InsertBelow(toInsert);

            // Assert
            Assert.Equal(stringListReference, stringList_insertBelow);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, 0)]
        [InlineData(StringDataForTest.multipleLines, 1)]
        [InlineData(StringDataForTest.randomBlock, 40)]
        public void InsertBelow_SpecificIndexSomeLines(string input, int index)
        {
            string toInsert = StringDataForTest.multipleLines;

            // Reference
            List<string> stringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                stringListReference.Add(s);
                if (i == index)
                    stringListReference.AddRange(toInsert.Split('\n'));
            }

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i == index)
                    matchStringList.Add((Match.Matching, s));
                else if (i < index)
                    matchStringList.Add((Match.Before, s));
                else
                    matchStringList.Add((Match.After, s));
            }

            // Act
            IEnumerable<string> stringList_insertBelow = matchStringList.InsertBelow(toInsert);

            // Assert
            Assert.Equal(stringListReference, stringList_insertBelow);
        }
    }

    public class InsertAboveTest
    {
        [Theory]
        [InlineData(StringDataForTest.oneLine, Match.Before)]
        [InlineData(StringDataForTest.multipleLines, Match.Before)]
        [InlineData(StringDataForTest.randomBlock, Match.Before)]
        public void InsertAbove_NothingHappensIfNoMatch(string input, Match m)
        {
            string toInsert = "Aaaaaa";

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<string> stringList_insertAbove = matchStringList.InsertAbove(toInsert);

            // Assert
            Assert.Equal(input.Split('\n'), stringList_insertAbove);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, Match.Matching)]
        [InlineData(StringDataForTest.multipleLines, Match.Matching)]
        [InlineData(StringDataForTest.randomBlock, Match.Matching)]
        public void InsertAbove_AllMatchingInsertOneline(string input, Match m)
        {
            string toInsert = "Aaaaaa";

            // Reference
            List<string> stringListReference = new() { toInsert };
            stringListReference.AddRange(input.Split('\n'));

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<string> stringList_insertAbove = matchStringList.InsertAbove(toInsert);

            // Assert
            Assert.Equal(stringListReference, stringList_insertAbove);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, Match.Matching)]
        [InlineData(StringDataForTest.multipleLines, Match.Matching)]
        [InlineData(StringDataForTest.randomBlock, Match.Matching)]
        public void InsertAbove_AllMatchingInsertSomeLines(string input, Match m)
        {
            string toInsert = StringDataForTest.multipleLines;

            // Reference
            List<string> stringListReference = toInsert.Split('\n').ToList();
            stringListReference.AddRange(input.Split('\n'));

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<string> stringList_insertAbove = matchStringList.InsertAbove(toInsert);

            // Assert
            Assert.Equal(stringListReference, stringList_insertAbove);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, 0)]
        [InlineData(StringDataForTest.multipleLines, 1)]
        [InlineData(StringDataForTest.randomBlock, 40)]
        public void InsertAbove_SpecificIndexSomeLines(string input, int index)
        {
            string toInsert = StringDataForTest.multipleLines;

            // Reference
            List<string> stringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i == index)
                    stringListReference.AddRange(toInsert.Split('\n'));
                stringListReference.Add(s);
            }

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i == index)
                    matchStringList.Add((Match.Matching, s));
                else if (i < index)
                    matchStringList.Add((Match.Before, s));
                else
                    matchStringList.Add((Match.After, s));
            }

            // Act
            IEnumerable<string> stringList_insertAbove = matchStringList.InsertAbove(toInsert);

            // Assert
            Assert.Equal(stringListReference, stringList_insertAbove);
        }
    }
    public class ReplaceByTest
    {
        [Theory]
        [InlineData(StringDataForTest.oneLine, Match.Before)]
        [InlineData(StringDataForTest.multipleLines, Match.Before)]
        [InlineData(StringDataForTest.randomBlock, Match.Before)]
        public void ReplaceBy_NothingHappensIfNoMatch(string input, Match m)
        {
            string toInsert = "Aaaaaa";

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<string> stringList_replaceBy = matchStringList.ReplaceBy(toInsert);

            // Assert
            Assert.Equal(input.Split('\n'), stringList_replaceBy);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, Match.Matching)]
        [InlineData(StringDataForTest.multipleLines, Match.Matching)]
        [InlineData(StringDataForTest.randomBlock, Match.Matching)]
        public void ReplaceBy_AllMatchingReplaceByOneLine(string input, Match m)
        {
            string toInsert = "Aaaaaa";

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<string> stringList_replaceBy = matchStringList.ReplaceBy(toInsert);

            // Assert
            Assert.Equal(toInsert.Split('\n'), stringList_replaceBy);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, Match.Matching)]
        [InlineData(StringDataForTest.multipleLines, Match.Matching)]
        [InlineData(StringDataForTest.randomBlock, Match.Matching)]
        public void ReplaceBy_AllMatchingReplaceBySomeLines(string input, Match m)
        {
            string toInsert = StringDataForTest.multipleLines;

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach(string s in input.Split('\n'))
            {
                matchStringList.Add((m, s));
            }

            // Act
            IEnumerable<string> stringList_replaceBy = matchStringList.ReplaceBy(toInsert);

            // Assert
            Assert.Equal(toInsert.Split('\n'), stringList_replaceBy);
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine, 0)]
        [InlineData(StringDataForTest.multipleLines, 1)]
        [InlineData(StringDataForTest.randomBlock, 40)]
        public void ReplaceBy_SpecificIndexSomeLines(string input, int index)
        {
            string toInsert = StringDataForTest.multipleLines;

            // Reference
            List<string> stringListReference = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i == index)
                    stringListReference.AddRange(toInsert.Split('\n'));
                else
                    stringListReference.Add(s);
            }

            // Arrange
            List<(Match, string)> matchStringList = new();
            foreach((int i, string s) in input.Split('\n').Enumerate())
            {
                if (i == index)
                    matchStringList.Add((Match.Matching, s));
                else if (i < index)
                    matchStringList.Add((Match.Before, s));
                else
                    matchStringList.Add((Match.After, s));
            }

            // Act
            IEnumerable<string> stringList_replaceBy = matchStringList.ReplaceBy(toInsert);

            // Assert
            Assert.Equal(stringListReference, stringList_replaceBy);
        }
    }

    public class ApplyTest
    {
        private IEnumerable<string> DoNothingIterator(IEnumerable<string> input)
        {
            foreach (string element in input)
            {
                yield return element;
            }
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void Apply_DoNothing(string input)
        {
            // Arrange
            List<string> listStringInput = input.Split('\n').ToList();

            // Act
            IEnumerable<string> res = listStringInput.Apply(DoNothingIterator);
            
            // Assert
            Assert.Equal(res, listStringInput);
        }
        
        private IEnumerable<string> PickFirstLineIterator(IEnumerable<string> input)
        {
            bool send = false;
            foreach (string element in input)
            {
                if (!send)
                {
                    yield return element;
                    send = true;
                }
            }
        }

        [Theory]
        [InlineData(StringDataForTest.oneLine)]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void Apply_PickFirstLine(string input)
        {
            // Arrange
            List<string> listStringInput = input.Split('\n').ToList();

            // Act
            IEnumerable<string> res = listStringInput.Apply(PickFirstLineIterator);
            
            // Assert
            Assert.Equal(res, listStringInput.GetRange(0, 1));
        }
        private IEnumerable<string> PickFirstTwoLinesIterator(IEnumerable<string> input)
        {
            bool send_first = false;
            bool send_second = false;
            foreach (string element in input)
            {
                if (!send_first)
                {
                    yield return element;
                    send_first = true;
                }
                else if (!send_second)
                {
                    yield return element;
                    send_second = true;
                }
            }
        }

        [Theory]
        [InlineData(StringDataForTest.multipleLines)]
        [InlineData(StringDataForTest.randomBlock)]
        public void Apply_PickFirstTwoLines(string input)
        {
            // Arrange
            List<string> listStringInput = input.Split('\n').ToList();

            // Act
            IEnumerable<string> res = listStringInput.Apply(PickFirstTwoLinesIterator);
            
            // Assert
            Assert.Equal(res, listStringInput.GetRange(0, 2));
        }
    }
}