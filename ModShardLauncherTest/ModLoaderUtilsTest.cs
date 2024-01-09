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
            yield return new object[] { 
@"/*

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
state = 25" };

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