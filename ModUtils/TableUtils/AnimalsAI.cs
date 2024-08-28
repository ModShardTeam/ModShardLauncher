using System;
using System.Collections.Generic;
using System.Linq;

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
    public static int NFactions = 0;
    public static List<string> Factions = new();
    // Behaviours matrix is represented by layers instead of a class row, column representation.
    // see https://cs.stackexchange.com/questions/27627/algorithm-dimension-increase-in-1d-representation-of-square-matrix for more information.
    public static List<Behaviour> Behaviours = new();
}
public class StatAI
{
    string table = "gml_GlobalScript_table_animals_ai";
    private static int ConvertSquaredCoordinates(int i, int j)
    {
        // i is row
        // j is column
        return i <= j ? j*j + i: i*i + 2*i - j;
    }
    private static void SetElements(string faction, Func<int, int, int> conv, params (string, DataAI.Behaviour)[] actions)
    {
        // adds a new line
        // i is then fixed
        if (!DataAI.Factions.Contains(faction))
        {
            DataAI.Factions.Add(faction);
            DataAI.Behaviours.AddRange(Enumerable.Repeat(DataAI.Behaviour.None, 2*DataAI.NFactions + 1));
            DataAI.NFactions++;
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
        SetElements(faction, ConvertSquaredCoordinates, responses);
    }
    public static void SetResponse(string faction, params (string, DataAI.Behaviour)[] responses)
    {
        // add or change a column
        SetElements(faction, (i, j) => ConvertSquaredCoordinates(j, i), responses);
    }
}
public partial class Msl
{
    public static void InjectTableAction(string faction, params (string, DataAI.Behaviour)[] responses)
    {
        StatAI.SetAction(faction, responses);
    }
    public static void InjectTableResponse(string faction, params (string, DataAI.Behaviour)[] responses)
    {
        StatAI.SetResponse(faction, responses);
    }
}
        