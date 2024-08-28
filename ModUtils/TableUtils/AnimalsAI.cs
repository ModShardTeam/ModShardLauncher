using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;

namespace ModShardLauncher;

public static class DataAI
{
    public enum Behaviour
    {
        None,
        Attack,
        Territorial,
        Fleeing,
    }
    public static string StrBehaviour(Behaviour behaviour)
    {
        return behaviour switch
        {
            Behaviour.Attack => "1",
            Behaviour.Territorial => "2",
            Behaviour.Fleeing => "3",
            _ => "",
        };
    }
    public static Behaviour ParseBehaviour(string s)
    {
        return s switch
        {
            "1" => Behaviour.Attack,
            "2" => Behaviour.Territorial,
            "3" => Behaviour.Fleeing,
            _ => Behaviour.None,
        };
    }
    public static int ConvertSquaredCoordinates(int i, int j)
    {
        // i is row
        // j is column
        return i <= j ? j*j + i: i*i + 2*i - j;
    }
    public static readonly string TableName = "gml_GlobalScript_table_animals_ai";
    public static List<string> Factions = new();
    // Behaviours matrix is represented by layers instead of a class row, column representation.
    // see https://cs.stackexchange.com/questions/27627/algorithm-dimension-increase-in-1d-representation-of-square-matrix for more information.
    public static List<Behaviour> Behaviours = new();
    internal static void LoadAITable()
    {
        if (Factions.Any() || Behaviours.Any())
        {
            // already Imported
            return;
        }

        try
        {
            UndertaleCode code = Msl.GetUMTCodeFromFile(TableName); // can fail InvalidOperationException

            string table = code.Disassemble(ModLoader.Data.Variables, ModLoader.Data.CodeLocals.For(code));
            IEnumerable<System.Text.RegularExpressions.Match> matches = Regex.Matches(table, @"push.s ""(.+)""@\d+").Reverse();

            foreach((int i, System.Text.RegularExpressions.Match match) in matches.Enumerate())
            {
                string line = match.Groups[1].Value;
                if (line.Contains("1;- Combat")) break;
                if (line.Contains("x;")) continue;

                foreach((int j, string s) in line.Split(';').Enumerate())
                {
                    if (j == 0)
                    {
                        Factions.Add(s);
                        Log.Information("found faction {0}", s);
                        continue;
                    }

                    int index = ConvertSquaredCoordinates(i - 1, j - 1);
                    if (Behaviours.Count <= index) Behaviours.AddRange(Enumerable.Repeat(Behaviour.None, 2*Behaviours.Count + 1));
                    Behaviours[index] = ParseBehaviour(s);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }

        Log.Information("found {0} factions", Factions.Count);
        Log.Information("found {0} behaviours", Behaviours.Count);
    }
    internal static void PrintAITable()
    {
        int N = Factions.Count;

        for(int i = 0; i < N; i++)
        {
            string l = Factions[i];
             for(int j = 0; j < N; j++)
            {
                l += $"\t{Behaviours[ConvertSquaredCoordinates(i, j)]}";
            }
            Log.Information(l);
        }
    }
    internal static IEnumerable<string> CreateIATable()
    {
        yield return $"x;{string.Concat(Factions.Select(x => $"{x};"))}";

        int N = Factions.Count;
        for(int i = 0; i < N; i++)
        {
            string l = $"{Factions[i]};";
             for(int j = 0; j < N; j++)
            {
                l += $"{Behaviours[ConvertSquaredCoordinates(i, j)]};";
            }
            yield return l;
        }

        yield return $"1;- Combat-переход;{string.Concat(Enumerable.Repeat(';', N))}";
        yield return $"1;- Threat-переход;{string.Concat(Enumerable.Repeat(';', N))}";
        yield return $"1;- Flee-переход;{string.Concat(Enumerable.Repeat(';', N))}";
    }
}
public class StatAI
{
    private static void SetElements(string faction, Func<int, int, int> conv, params (string, DataAI.Behaviour)[] actions)
    {
        if (!DataAI.Factions.Contains(faction))
        {
            DataAI.Behaviours.AddRange(Enumerable.Repeat(DataAI.Behaviour.None, 2*DataAI.Factions.Count + 1));
            DataAI.Factions.Add(faction);
        }

        (int i, string _) = DataAI.Factions.Enumerate().First(x => x.Item2 == faction);

        foreach ((string f, DataAI.Behaviour b) in actions)
        {
            (int j, string? factionFound) = DataAI.Factions.Enumerate().FirstOrDefault(x => x.Item2 == f);
            if (factionFound != null)
            {
                int index = conv(i, j);
                DataAI.Behaviours[index] = b;
            }
        }
    }
    public static void SetAction(string faction, params (string, DataAI.Behaviour)[] responses)
    {
        // add or change a line
        SetElements(faction, DataAI.ConvertSquaredCoordinates, responses);
    }
    public static void SetResponse(string faction, params (string, DataAI.Behaviour)[] responses)
    {
        // add or change a column
        SetElements(faction, (i, j) => DataAI.ConvertSquaredCoordinates(j, i), responses);
    }
}
public partial class Msl
{
    public static void LoadAITable()
    {
        DataAI.LoadAITable();
        DataAI.PrintAITable();
    }
    public static void SaveAITable()
    {
        static IEnumerable<string> func(IEnumerable<string> input)
        {
            int sizeTable = 0;
            bool ignore_lines = false;
            foreach (string item in input)
            {
                if (item.Contains("setowner"))
                {
                    yield return item;
                    IEnumerable<string> table = DataAI.CreateIATable();
                    foreach(string line in table)
                    {
                        sizeTable++;
                        yield return $"push.s \"{line}\"";
                        yield return "conv.s.v";
                    }
                    ignore_lines = true;
                }
                else if (item.Contains("NewGMLArray"))
                {
                    yield return $"call.i @@NewGMLArray@@(argc={sizeTable})";
                    ignore_lines = false;
                    continue;
                }

                if (!ignore_lines)
                {
                    yield return item;
                }
                else
                {
                    yield return item;
                }
            }
        }

        Msl.LoadAssemblyAsString(DataAI.TableName)
            .Apply(func)
            .Save();
    }
    public static void InjectTableAction(string faction, params (string, DataAI.Behaviour)[] responses)
    {
        StatAI.SetAction(faction, responses);
    }
    public static void InjectTableResponse(string faction, params (string, DataAI.Behaviour)[] responses)
    {
        StatAI.SetResponse(faction, responses);
    }
}
        