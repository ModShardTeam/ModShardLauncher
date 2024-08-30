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
    public static List<string> ActingFactions = new(); // each row represent how a faction acts to the presence of another one
    public static List<string> RespondingFactions = new(); // each columns represents how others factions respond to the present of another one
    // Behaviours matrix is represented by layers instead of a class row, column representation.
    // see https://cs.stackexchange.com/questions/27627/algorithm-dimension-increase-in-1d-representation-of-square-matrix for more information.
    public static List<Behaviour> Behaviours = new();
    internal static void LoadAITable()
    {
        if (ActingFactions.Any() || RespondingFactions.Any() || Behaviours.Any())
        {
            // already Imported
            Log.Warning("The AITable was already loaded.");
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
                if (line.Contains("x;"))
                {
                    foreach((int j, string s) in line.Split(';').Enumerate())
                    {
                        if (j == 0) continue;
                        RespondingFactions.Add(s);
                        Log.Information("found responding faction {0}", s);
                    }
                    continue;
                }

                foreach((int j, string s) in line.Split(';').Enumerate())
                {
                    if (j == 0)
                    {
                        ActingFactions.Add(s);
                        Log.Information("found acting faction {0}", s);
                        continue;
                    }

                    int index = ConvertSquaredCoordinates(i - 1, j - 1);
                    if (Behaviours.Count <= index)
                    {
                        int old_size = Behaviours.Count;
                        int increasing_size = (int)(2 *Math.Sqrt(Behaviours.Count) + 1);
                        Log.Information("By adding {2}, need to increase behaviours size from {0} to {1}.", old_size, old_size + increasing_size, index);
                        Behaviours.AddRange(Enumerable.Repeat(Behaviour.None, increasing_size));
                    }
                    Behaviours[index] = ParseBehaviour(s);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }

        Log.Information("found {0} acting factions", ActingFactions.Count);
        Log.Information("found {0} responding factions", RespondingFactions.Count);
        Log.Information("found {0} behaviours", Behaviours.Count);
    }
    internal static void PrintAITable()
    {
        int N = ActingFactions.Count;
        int M = RespondingFactions.Count;

        string l = "x\t\t";
        for(int j = 0; j < M; j++)
        {
            l += $" {RespondingFactions[j]}";
        }

        for(int i = 0; i < N; i++)
        {
            l = $"{ActingFactions[i]}\t\t";
             for(int j = 0; j < M; j++)
            {
                // l += $"\t{Behaviours[ConvertSquaredCoordinates(i, j)]}";
                l += $" {StrBehaviour(Behaviours[ConvertSquaredCoordinates(i, j)])}";
            }
            Log.Information(l);
        }
    }
    internal static IEnumerable<string> CreateIATable()
    {
        yield return $"x;{string.Concat(RespondingFactions.Select(x => $"{x};"))}";

        int N = ActingFactions.Count;
        int M = RespondingFactions.Count;

        for(int i = 0; i < N; i++)
        {
            string l = $"{ActingFactions[i]};";
             for(int j = 0; j < M; j++)
            {
                l += $"{StrBehaviour(Behaviours[ConvertSquaredCoordinates(i, j)])};";
            }
            yield return l;
        }

        yield return $"1;- Combat-переход;{string.Concat(Enumerable.Repeat(';', M))}";
        yield return $"1;- Threat-переход;{string.Concat(Enumerable.Repeat(';', M))}";
        yield return $"1;- Flee-переход;{string.Concat(Enumerable.Repeat(';', M))}";
    }
}
public class StatAI
{
    private static void SetElements(string faction, List<string> factionsList, Func<int, int, int> conv, params (string, DataAI.Behaviour)[] actions)
    {
        if (!factionsList.Contains(faction))
        {
            DataAI.Behaviours.AddRange(Enumerable.Repeat(DataAI.Behaviour.None, (int)(2 *Math.Sqrt(DataAI.Behaviours.Count) + 1)));
            factionsList.Add(faction);
        }

        (int i, string _) = factionsList.Enumerate().First(x => x.Item2 == faction);

        foreach ((string f, DataAI.Behaviour b) in actions)
        {
            (int j, string? factionFound) = factionsList.Enumerate().FirstOrDefault(x => x.Item2 == f);
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
        SetElements(faction, DataAI.RespondingFactions, DataAI.ConvertSquaredCoordinates, responses);
    }
    public static void SetResponse(string faction, params (string, DataAI.Behaviour)[] responses)
    {
        // add or change a column
        SetElements(faction, DataAI.ActingFactions, (i, j) => DataAI.ConvertSquaredCoordinates(j, i), responses);
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
                    IEnumerable<string> table = DataAI.CreateIATable().Reverse();
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
        