using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using Serilog;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Equivalent of enumerate found in Python but for C# IEnumerable.
        /// </summary>
        /// <returns>
        /// Return for each element a tuple of the current index and the element.
        /// </returns>
        public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> ienumerable) 
        {
            int ind = 0;
            foreach(T element in ienumerable) {
                yield return (ind++, element);
            }
        }
        public enum Match 
        {
            Before,
            Matching,
            After,
        }
        public static IEnumerable<(Match, string)> MatchFrom(this IEnumerable<string> ienumerable, IEnumerable<string> other) 
        {
            Match m = Match.Before;
            string? otherString = null;
            IEnumerator<string> otherEnumerator = other.GetEnumerator();
            if(otherEnumerator.MoveNext())
                otherString = otherEnumerator.Current;

            foreach (string element in ienumerable)
            {
                if (m != Match.After && otherString != null && element.Contains(otherString)) 
                {
                    m = Match.Matching;
                    yield return (m, element);
                    if(otherEnumerator.MoveNext())
                        otherString = otherEnumerator.Current;
                    else {
                        m = Match.After;
                    }
                }
                else {
                    if (m == Match.Matching)
                        m = Match.After;
                    yield return (m, element);
                }
            }
        }
        public static IEnumerable<(Match, string)> MatchFrom(this IEnumerable<string> ienumerable, string other) 
        {
            return ienumerable.MatchFrom(other.Split("\n"));
        }
        public static IEnumerable<T> Peek<T>(this IEnumerable<T> ienumerable)
        {
            foreach(T element in ienumerable)
            {
                Log.Information(element?.ToString() ?? "<null>");
                yield return element;
            }
        }
        public static IEnumerable<string> Remove(this IEnumerable<(Match, string)> ienumerable)
        {
            foreach((Match matched, string element) in ienumerable)
            {
                if(matched != Match.Matching)
                    yield return element;
            }
        }
        public static IEnumerable<string> KeepOnly(this IEnumerable<(Match, string)> ienumerable)
        {
            foreach((Match matched, string element) in ienumerable)
            {
                if(matched == Match.Matching)
                    yield return element;
            }
        }
        public static IEnumerable<string> FilterMatch(this IEnumerable<(Match, string)> ienumerable, Predicate<Match> predicate)
        {
            foreach((Match matched, string element) in ienumerable)
            {
                if(predicate(matched))
                    yield return element;
            }
        }
        public static IEnumerable<string> InsertBelow(this IEnumerable<(Match, string)> ienumerable, IEnumerable<string> inserting)
        {
            bool foundAfter = false;
            Match lastMatched = Match.Before;
            foreach((Match matched, string element) in ienumerable)
            {
                if(!foundAfter && matched == Match.After)
                {
                    foundAfter = true;
                    foreach(string elementInserted in inserting)
                    {
                        yield return elementInserted;
                    }
                } 
                yield return element;
                lastMatched = matched;
            }

            if (!foundAfter && lastMatched == Match.Matching)
            {
                foreach(string element in inserting)
                {
                    yield return element;
                }
            }
        }
        public static IEnumerable<string> InsertBelow(this IEnumerable<(Match, string)> ienumerable, string inserting)
        {
            return ienumerable.InsertBelow(inserting.Split("\n"));
        }
        public static IEnumerable<string> InsertAbove(this IEnumerable<(Match, string)> ienumerable, IEnumerable<string> inserting)
        {
            bool alreadyInserted = false;
            foreach((Match matched, string element) in ienumerable)
            {
                if(!alreadyInserted && matched == Match.Matching)
                {
                    foreach(string elementInserted in inserting)
                    {
                        yield return elementInserted;
                    }
                    alreadyInserted = true;
                } 
                yield return element;
            }
        }
        public static IEnumerable<string> InsertAbove(this IEnumerable<(Match, string)> ienumerable, string inserting)
        {
            return ienumerable.InsertAbove(inserting.Split("\n"));
        }
        public static IEnumerable<string> ReplaceBy(this IEnumerable<(Match, string)> ienumerable, IEnumerable<string> replacing)
        {
            bool alreadyReplaced = false;
            foreach((Match matched, string element) in ienumerable)
            {
                if(matched == Match.Matching)
                {
                    if (!alreadyReplaced) {
                        foreach(string elementInserted in replacing)
                        {
                            yield return elementInserted;
                        }
                        alreadyReplaced = true;
                    }
                } 
                else {
                    yield return element;
                }
            }
        }
        public static IEnumerable<string> ReplaceBy(this IEnumerable<(Match, string)> ienumerable, string replacing)
        {
            return ienumerable.ReplaceBy(replacing.Split("\n"));
        }
    }
    /// <summary>
    /// Static utilities for ModLoader
    /// </summary>
    public static class ModLoaderUtils 
    {
        /// <summary>
        /// Utility function that print the details around an <paramref name="instruction"/> in the log.
        /// <para>
        /// Not all these fields seem to be always relevant and depending on the <paramref name="instruction"/> one should pays attention at different details. 
        /// </para>
        /// <para>
        /// Following the instruction, I advice looking at:
        /// <list type="bullet">
        /// <item>
        /// <term>Branches</term>
        /// <description>Address and JumpOffset</description>
        /// </item>
        /// <item>
        /// <term>Comparison</term>
        /// <description>ComparisonKind</description>
        /// </item>
        /// <item>
        /// <term>Push</term>
        /// <description>Type1, Type2, TypeInst and Value</description>
        /// </item>
        /// <item>
        /// <term>Pop</term>
        /// <description>Type1, Type2, TypeInst and Destination</description>
        /// </item>
        /// <item>
        /// <term>Call</term>
        /// <description>ArgumentsCount and Function</description>
        /// </item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="instruction">An undertale instruction which is an abstraction made by the UMT team to approximate GML-assembly.</param>
        public static void LogInstruction(UndertaleInstruction instruction) 
        {
            try {
                Log.Information(string.Format(@"{{{0}}}:
                (   
                    .Address = {10}, 
                    .JumpOffset = {11}, 
                    .Kind = {1},
                    .ComparisonKind = {8},
                    .Type1 = {2},
                    .Type2 = {3},
                    .TypeInst = {4},
                    .Value = {6},
                    .Destination = {5},
                    .Function = {7},
                    .ArgumentsCount = {9},
                )
                ",
                instruction.ToString(),
                instruction.Kind.ToString(),
                instruction.Type1.ToString(),
                instruction.Type2.ToString(),
                instruction.TypeInst.ToString(),
                (instruction.Destination != null) ? 
                    string.Format(@"(
                            .Type = {0},
                            .Target = ( .Name = {1}, .InstanceType = {2}, ),
                        )", 
                        instruction.Destination?.Type.ToString(),
                        instruction.Destination?.Target.Name.ToString(),
                        instruction.Destination?.Target.InstanceType.ToString()) : 
                    "<null>",
                (instruction.Value is UndertaleInstruction.Reference<UndertaleVariable>) ? 
                    string.Format(@"(
                            .Type = {0},
                            .Target = ( .Name = {1}, .InstanceType = {2}, ),
                        )", 
                        (instruction.Value as UndertaleInstruction.Reference<UndertaleVariable>)?.Type.ToString(),
                        (instruction.Value as UndertaleInstruction.Reference<UndertaleVariable>)?.Target.Name.ToString(),
                        (instruction.Value as UndertaleInstruction.Reference<UndertaleVariable>)?.Target.InstanceType.ToString()) : 
                    instruction.Value?.ToString() ?? "<null>",

                (instruction.Function != null) ? 
                    string.Format(@"(
                            .Type = {0},
                            .Target = ( .Name = {1}, .Classification = {2}, ),
                        )", 
                        instruction.Function?.Type.ToString(),
                        instruction.Function?.Target.Name.ToString(),
                        instruction.Function?.Target.Classification.ToString()) : 
                    "<null>",
                instruction.ComparisonKind.ToString(),
                instruction.ArgumentsCount.ToString(),
                instruction.Address.ToString(),
                instruction.JumpOffset.ToString()
                ));
            }
            catch(Exception ex) {
                Log.Error(ex, string.Format("Cannot log {0}", instruction.ToString()));
                throw;
            }
            
        }
    }
}