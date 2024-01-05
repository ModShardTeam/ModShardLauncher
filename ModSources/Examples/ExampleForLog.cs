using ModShardLauncher;
using ModShardLauncher.Mods;
using UndertaleModLib;
using UndertaleModLib.Models;
using System.Collections.Generic;
using Serilog;

namespace ExampleForLog;
public class ExampleForLog : Mod
{
    public override string Author => "zizani";
    public override string Name => "ExampleForLog";
    public override string Description => "Some examples to play with the Log";
    public override string Version => "1.0.0.0";
    
    public override void PatchMod()
    {
        string input_code = "var a = 1\nvar y = a + 1\nreturn y";

        // case 1: a match on one line
        IEnumerable<(Match, string)> test1 = input_code.Split('\n')
            .MatchFrom("var y".Split('\n'));

        Log.Information("\ncase1");
        foreach(var t in test1) 
        {
            Log.Information(t.ToString());
        }

        // case 2: a match on multiple lines
        IEnumerable<(Match, string)> test2 = input_code.Split('\n')
            .MatchFrom("var y\nretur".Split('\n'));

        Log.Information("\ncase2");
        foreach(var t in test2) 
        {
            Log.Information(t.ToString());
        }
        
        // case 3: a match on multiple lines but cant find a consecutive block to match
        IEnumerable<(Match, string)> test3 = input_code.Split('\n')
            .MatchFrom("var\nretur".Split('\n'));

        Log.Information("\ncase3");
        foreach(var t in test3) 
        {
            Log.Information(t.ToString());
        }
    
        // case 4: same selection as case 1 but Remove the Match
        IEnumerable<string> test4 = input_code.Split('\n')
            .MatchFrom("var y".Split('\n'))
            .Remove();

        Log.Information("\ncase4");
        foreach(var t in test4) 
        {
            Log.Information(t.ToString());
        }
        
        // case 5: A more complex manipulation close to real life examples
        IEnumerable<string> test5 = input_code.Split('\n')
            .MatchFrom("var a".Split('\n'))
            .InsertBelow("var a = 2\nvar b = a * 2".Split('\n'))
            .MatchFrom("var a".Split('\n'))
            .Remove()
            .MatchFrom("return".Split('\n'))
            .InsertBelow("return b * y".Split('\n'))
            .MatchFrom("return".Split('\n'))
            .Remove();

        Log.Information("\ncase5");
        foreach(var t in test5) 
        {
            Log.Information(t.ToString());
        }
        
        // case 6: Same result as case 5 but using ReplaceBy instead of InsertBelow and Remove chained
        IEnumerable<string> test6 = "var a = 1\nvar y = a + 1\nreturn y".Split('\n')
            .MatchFrom("var a".Split('\n'))
            .ReplaceBy("var a = 2\nvar b = a * 2".Split('\n'))
            .MatchFrom("return".Split('\n'))
            .ReplaceBy("return b * y".Split('\n'));

        Log.Information("\ncase6");
        foreach(var t in test6) 
        {
            Log.Information(t.ToString());
        }
    }
}