using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Interop;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    /// <summary>
    /// Enum used in <see cref="MatchFrom"/>, <see cref="MatchBelow"/> and <see cref="MatchAll"/> to tag specific lines.
    /// </summary>
    public enum Match 
    {
        Before,
        Matching,
        After,
    }
    /// <summary>
    /// Enum to know how the code handled was extracted with UTMT. The most classic cases are code as string either decompiled from GML or disassemblied from Assembly-like GML.
    /// </summary>
    public enum PatchingWay 
    {
        GML,
        AssemblyAsInstructions,
        AssemblyAsString,
    }
    /// <summary>
    /// Informations around the code handled to notably recompiled it easily.
    /// </summary>
    public class Header
    {
        public readonly string fileName;
        public readonly UndertaleCode originalCode;
        public readonly PatchingWay patchingWay;

        public Header(string fileName, UndertaleCode originalCode, PatchingWay patchingWay) 
        {
            this.fileName = fileName;
            this.originalCode = originalCode;
            this.patchingWay = patchingWay;
        }
    }
    /// <summary>
    /// Result of the <see cref="Save"/> method. Contains information that can help debugging if something went wrong.
    /// </summary>
    public class ModSummary
    {
        public readonly string fileName;
        public readonly string newCode;
        public readonly PatchingWay patchingWay;

        public ModSummary(string fileName, string newCode, PatchingWay patchingWay) 
        {
            this.fileName = fileName;
            this.newCode = newCode;
            this.patchingWay = patchingWay;
        }

        public override string ToString()
        {
            return string.Format("{0} was patched by {1}:\n{2}", fileName, patchingWay, newCode);
        }
    }
    /// <summary>
    /// A class wrapper that encapsulate a <see cref="Header"/> and an IEnumerable. That's the main class used while modding under the hood.
    /// </summary>
    public class FileEnumerable<T>
    {
        public readonly Header header;
        public readonly IEnumerable<T> ienumerable;
        public FileEnumerable(Header header, IEnumerable<T> ienumerable) 
        {
            this.header = header;
            this.ienumerable = ienumerable;
        }
    }
    /// <summary>
    /// A static class for notably IEnumerable Extensions to provide a functional-programing-like api while modding.
    /// </summary>
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
        /// <summary>
        /// Convert a (Match, string) IEnumerable into a string IEnumerable by selecting for all elements only the string part.
        /// <example>
        /// For example:
        /// <code>
        /// List&lt;(Match, string)&gt; example = new() { (Match.Before, "Aaa") };
        /// var flatten_example = example.Flatten();
        /// </code>
        /// results in <c>flatten_example</c> being IEnumerable&lt; string &gt; { "Aaa" }.
        /// </example>
        /// </summary>
        public static IEnumerable<string> Flatten(this IEnumerable<(Match, string)> ienumerable)
        {
            foreach((Match _, string element) in ienumerable)
            {
                yield return element;
            }
        }
        /// <summary>
        /// Wrapper of <see cref="Flatten"/> for the <see cref="FileEnumerable"/>  class.
        /// </summary>
        public static FileEnumerable<string> Flatten(this FileEnumerable<(Match, string)> fe)
        {
            return new(fe.header, fe.ienumerable.Flatten());
        }
        /// <summary>
        /// A wrapper for string.Join("\n").
        /// <example>
        /// For example:
        /// <code>
        /// List&lt; string &gt; example = new() { "Hello", "World" };
        /// var collect_example = example.Collect();
        /// </code>
        /// results in <c>collect_example</c> being "HelloWorld".
        /// </example>
        /// </summary>
        public static string Collect(this IEnumerable<string> ienumerable)
        {
            return string.Join("\n", ienumerable);
        }
        /// <summary>
        /// A wrapper for string.Join("\n") that works for (Match, string) IEnumerable by dropping the Match part.
        /// <example>
        /// For example:
        /// <code>
        /// List&lt;(Match, string)&gt; example = new() { (Match.Before, "Hello"), (Match.Matching, "World") };
        /// var collect_example = example.Collect();
        /// </code>
        /// results in <c>collect_example</c> being "HelloWorld".
        /// </example>
        /// </summary>
        public static string Collect(this IEnumerable<(Match, string)> ienumerable)
        {
            return string.Join("\n", ienumerable.Flatten());
        }
        /// <summary>
        /// A wrapper for string.Join("\n") on the IEnumerable attribute of a string <see cref="FileEnumerable"/>. <see cref="Collect"/>
        /// </summary>
        public static string Collect(this FileEnumerable<string> fe)
        {
            return fe.ienumerable.Collect();
        }
        /// <summary>
        /// A wrapper for string.Join("\n") on the IEnumerable attribute of a (Match, string) <see cref="FileEnumerable"/>. <see cref="Collect"/>
        /// </summary>
        public static string Collect(this FileEnumerable<(Match, string)> fe)
        {
            return fe.ienumerable.Collect();
        }
        /// <summary>
        /// A selector that tags the first block of continuous lines that matches a given list of string.
        /// Does nothing on its own, only tagging, needs to be used with an Action to properly modify your input.
        /// <example>
        /// For example:
        /// <code>
        /// List&lt; string &gt; example = new() { "Hello", "World" };
        /// var matched_example = example.MatchFrom(new List&lt; string &gt;() { "Hel" });
        /// </code>
        /// results in <c>matched_example</c> being new IEnumerable&lt;(Match, string)&gt;() { (Match.Matching, "Hello"), (Match.After, "World") };.
        /// </example>
        /// </summary>
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
        /// <summary>
        /// Same behaviour as <see cref="MatchFrom"/> but using <paramref name="other"/>.Split('\n') for the comparison. 
        /// </summary>
        public static IEnumerable<(Match, string)> MatchFrom(this IEnumerable<string> ienumerable, string other) 
        {
            return ienumerable.MatchFrom(other.Split("\n"));
        }
        /// <summary>
        /// Wrapper of <see cref="MatchFrom"/> for the <see cref="FileEnumerable"/>  class.
        /// </summary>
        public static FileEnumerable<(Match, string)> MatchFrom(this FileEnumerable<string> fe, string other) 
        {
            return new(fe.header, fe.ienumerable.MatchFrom(other.Split("\n")));
        }
        /// <summary>
        /// Wrapper of <see cref="MatchFrom"/> for the <see cref="FileEnumerable"/> class using the content of <paramref name="fileName"/> for the comparison.
        /// </summary>
        public static FileEnumerable<(Match, string)> MatchFrom(this FileEnumerable<string> fe, ModFile modFile, string fileName) 
        {
            return new(fe.header, fe.ienumerable.MatchFrom(modFile.GetCode(fileName).Split("\n")));
        }
        /// <summary>
        /// A selector that tags the <paramref name="len"/>-th lines of code below a given list of string.
        /// Does nothing on its own, only tagging, needs to be used with an Action to properly modify your input.
        /// <example>
        /// For example:
        /// <code>
        /// List&lt; string &gt; example = new() { "Hello", "World" };
        /// var matched_example = example.MatchBelow(new List&lt; string &gt;() { "Hel" }, 1);
        /// </code>
        /// results in <c>matched_example</c> being new IEnumerable&lt;(Match, string)&gt;() { (Match.Before, "Hello"), (Match.Matching, "World") };.
        /// </example>
        /// </summary>
        public static IEnumerable<(Match, string)> MatchBelow(this IEnumerable<string> ienumerable, IEnumerable<string> other, int len)
        {
            int i = 0; // used for keeping tracked about how many lines after the block we pass by
            bool encounteredTheBlock = false; // bool to track if we already encountered the block, disabling the 2nd case of the if/else
            bool passedTheBlock = false; // bool to track if we already passed the block, disabling the 1st case of the if/else
            string? otherString = null;
            IEnumerator<string> otherEnumerator = other.GetEnumerator();
            if(otherEnumerator.MoveNext())
                otherString = otherEnumerator.Current;

            foreach (string element in ienumerable)
            {
                if (!passedTheBlock && otherString != null && element.Contains(otherString)) // can only test the other iter if in Before
                {
                    encounteredTheBlock = true;
                    yield return (Match.Before, element);
                    if(otherEnumerator.MoveNext())
                        otherString = otherEnumerator.Current;
                    else {
                        // consumed the iter, time go to in matching
                        passedTheBlock = true;
                    }
                }
                else if (!encounteredTheBlock) // here when you still havent encountered the other iter
                {
                    yield return (Match.Before, element);
                }
                else if (i < len) // here when either the iter was consumed, either it was not matching anymore
                {
                    passedTheBlock = true;
                    yield return (Match.Matching, element);
                    i++; // can stay only len in matching
                }
                else // here in after, nothing to do
                {
                    yield return (Match.After, element);
                }
            }
        }
        /// <summary>
        /// Same behaviour as <see cref="MatchBelow"/> but using <paramref name="other"/>.Split('\n') for the comparison. 
        /// </summary>
        public static IEnumerable<(Match, string)> MatchBelow(this IEnumerable<string> ienumerable, string other, int len) 
        {
            return ienumerable.MatchBelow(other.Split("\n"), len);
        }
        /// <summary>
        /// Wrapper of <see cref="MatchBelow"/> for the <see cref="FileEnumerable"/> class.
        /// </summary>
        public static FileEnumerable<(Match, string)> MatchBelow(this FileEnumerable<string> fe, string other, int len) 
        {
            return new(fe.header, fe.ienumerable.MatchBelow(other.Split("\n"), len));
        }
        /// <summary>
        /// Wrapper of <see cref="MatchBelow"/> for the <see cref="FileEnumerable"/> class using the content of <paramref name="fileName"/> for the comparison.
        /// </summary>
        public static FileEnumerable<(Match, string)> MatchBelow(this FileEnumerable<string> fe, ModFile modFile, string fileName, int len) 
        {
            return new(fe.header, fe.ienumerable.MatchBelow(modFile.GetCode(fileName).Split("\n"), len));
        }
        /// <summary>
        /// A selector that tags the all lines of the input.
        /// Does nothing on its own, only tagging, needs to be used with an Action to properly modify your input.
        /// <example>
        /// For example:
        /// <code>
        /// List&lt; string &gt; example = new() { "Hello", "World" };
        /// var matched_example = example.MatchAll();
        /// </code>
        /// results in <c>matched_example</c> being new IEnumerable&lt;(Match, string)&gt;() { (Match.Matching, "Hello"), (Match.Matching, "World") };.
        /// </example>
        /// </summary>
        public static IEnumerable<(Match, string)> MatchAll(this IEnumerable<string> ienumerable)
        {
            foreach (string element in ienumerable)
            {
                yield return (Match.Matching, element);
            }
        }
        /// <summary>
        /// Wrapper of <see cref="MatchAll"/> for the <see cref="FileEnumerable"/> class.
        /// </summary>
        public static FileEnumerable<(Match, string)> MatchAll(this FileEnumerable<string> fe) 
        {
            return new(fe.header, fe.ienumerable.MatchAll());
        }
        /// <summary>
        /// An action on an IEnumerable that prints each line on the log console but does not alter the data flow.
        /// <example>
        /// For example:
        /// <code>
        /// List&lt; string &gt; example = new() { "Hello", "World" };
        /// var peeked_example = example.Peek();
        /// </code>
        /// results in <c>peeked_example</c> being new IEnumerable&lt; string &gt;() { "Hello", "World" };.
        /// </example>
        /// </summary>
        public static IEnumerable<T> Peek<T>(this IEnumerable<T> ienumerable)
        {
            foreach(T element in ienumerable)
            {
                Log.Information(element?.ToString() ?? "<null>");
                yield return element;
            }
        }
        /// <summary>
        /// Wrapper of <see cref="Peek"/> for the <see cref="FileEnumerable"/> class.
        /// </summary>
        public static FileEnumerable<T> Peek<T>(this FileEnumerable<T> fe)
        {
            return new(fe.header, fe.ienumerable.Peek());
        }
        /// <summary>
        /// An action on an IEnumerable that removes all lines tagged as Matching. It's the complementary operation of <see cref="KeepOnly"/>.
        /// <example>
        /// For example:
        /// <code>
        /// List&lt;(Match, string)&gt; example = new() { (Match.Before, "Hello"), (Match.Matching, "World") };
        /// var remove_example = example.Remove();
        /// </code>
        /// results in <c>remove_example</c> being new IEnumerable&lt; string &gt;() { "Hello" };.
        /// </example>
        /// </summary>
        public static IEnumerable<string> Remove(this IEnumerable<(Match, string)> ienumerable)
        {
            foreach((Match matched, string element) in ienumerable)
            {
                if(matched != Match.Matching)
                    yield return element;
            }
        }
        /// <summary>
        /// Wrapper of <see cref="Remove"/> for the <see cref="FileEnumerable"/> class.
        /// </summary>
        public static  FileEnumerable<string> Remove(this FileEnumerable<(Match, string)> fe)
        {
            return new(fe.header, fe.ienumerable.Remove());
        }
        /// <summary>
        /// An action on an IEnumerable that removes all lines not tagged as Matching. It's the complementary operation of <see cref="Remove"/>.
        /// <example>
        /// For example:
        /// <code>
        /// List&lt;(Match, string)&gt; example = new() { (Match.Before, "Hello"), (Match.Matching, "World") };
        /// var keepOnly_example = example.KeepOnly();
        /// </code>
        /// results in <c>keepOnly_example</c> being new IEnumerable&lt; string &gt;() { "World" };.
        /// </example>
        /// </summary>
        public static IEnumerable<string> KeepOnly(this IEnumerable<(Match, string)> ienumerable)
        {
            foreach((Match matched, string element) in ienumerable)
            {
                if(matched == Match.Matching)
                    yield return element;
            }
        }
        /// <summary>
        /// Wrapper of <see cref="KeepOnly"/> for the <see cref="FileEnumerable"/> class.
        /// </summary>
        public static  FileEnumerable<string> KeepOnly(this FileEnumerable<(Match, string)> fe)
        {
            return new(fe.header, fe.ienumerable.KeepOnly());
        }
        /// <summary>
        /// An action on an IEnumerable that keeps all lines such that the <paramref name="predicate"/> is True. 
        /// It's a generalization of <see cref="Remove"/> and <see cref="KeepOnly"/>.
        /// <example>
        /// For example:
        /// <code>
        /// List&lt;(Match, string)&gt; example = new() { (Match.Before, "Hello"), (Match.Matching, "World"), (Match.After, "!") };
        /// var filterMatch_example = example.FilterMatch(x => x != Match.Matching);
        /// </code>
        /// results in <c>filterMatch_example</c> being new IEnumerable&lt; string &gt;() { "World", "!" };.
        /// </example>
        /// </summary>
        public static IEnumerable<string> FilterMatch(this IEnumerable<(Match, string)> ienumerable, Predicate<Match> predicate)
        {
            foreach((Match matched, string element) in ienumerable)
            {
                if(predicate(matched))
                    yield return element;
            }
        }
        /// <summary>
        /// Wrapper of <see cref="FilterMatch"/> for the <see cref="FileEnumerable"/> class.
        /// </summary>
        public static  FileEnumerable<string> FilterMatch(this FileEnumerable<(Match, string)> fe, Predicate<Match> predicate)
        {
            return new(fe.header, fe.ienumerable.FilterMatch(predicate));
        }
        /// <summary>
        /// An action on an IEnumerable that inserts lines of code below the Matching block. 
        /// <example>
        /// For example:
        /// <code>
        /// List&lt;(Match, string)&gt; example = new() { (Match.Before, "Hello"), (Match.Matching, "World"), (Match.After, "!") };
        /// var inserted_example = example.InsertBelow(new IEnumerable&lt; string &gt;() { " ", "I'm an example" });
        /// </code>
        /// results in <c>inserted_example</c> being new IEnumerable&lt; string &gt;() { "Hello", "World", " ", "I'm an example", "!" };.
        /// </example>
        /// </summary>
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
        /// <summary>
        /// Same behaviour as <see cref="InsertBelow"/> but using <paramref name="other"/>.Split('\n') for the comparison. 
        /// </summary>
        public static IEnumerable<string> InsertBelow(this IEnumerable<(Match, string)> ienumerable, string inserting)
        {
            return ienumerable.InsertBelow(inserting.Split("\n"));
        }
        /// <summary>
        /// Wrapper of <see cref="InsertBelow"/> for the <see cref="FileEnumerable"/> class.
        /// </summary>
        public static  FileEnumerable<string> InsertBelow(this FileEnumerable<(Match, string)> fe, string inserting)
        {
            return new(fe.header, fe.ienumerable.InsertBelow(inserting.Split("\n")));
        }
        /// <summary>
        /// Wrapper of <see cref="InsertBelow"/> for the <see cref="FileEnumerable"/> class using the content of <paramref name="fileName"/> for the comparison.
        /// </summary>
        public static  FileEnumerable<string> InsertBelow(this FileEnumerable<(Match, string)> fe, ModFile modFile, string fileName)
        {
            return new(fe.header, fe.ienumerable.InsertBelow(modFile.GetCode(fileName).Split("\n")));
        }
        /// <summary>
        /// An action on an IEnumerable that inserts lines of code above the Matching block. 
        /// <example>
        /// For example:
        /// <code>
        /// List&lt;(Match, string)&gt; example = new() { (Match.Before, "Hello"), (Match.Matching, "World"), (Match.After, "!") };
        /// var inserted_example = example.InsertAbove(new IEnumerable&lt; string &gt;() { " ", "I'm an example" });
        /// </code>
        /// results in <c>inserted_example</c> being new IEnumerable&lt; string &gt;() { "Hello", " ", "I'm an example", "World", "!" };.
        /// </example>
        /// </summary>
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
        /// <summary>
        /// Same behaviour as <see cref="InsertAbove"/> but using <paramref name="other"/>.Split('\n') for the comparison. 
        /// </summary>
        public static IEnumerable<string> InsertAbove(this IEnumerable<(Match, string)> ienumerable, string inserting)
        {
            return ienumerable.InsertAbove(inserting.Split("\n"));
        }
        /// <summary>
        /// Wrapper of <see cref="InsertAbove"/> for the <see cref="FileEnumerable"/> class.
        /// </summary>
        public static  FileEnumerable<string> InsertAbove(this FileEnumerable<(Match, string)> fe, string inserting)
        {
            return new(fe.header, fe.ienumerable.InsertAbove(inserting.Split("\n")));
        }
        /// <summary>
        /// Wrapper of <see cref="InsertAbove"/> for the <see cref="FileEnumerable"/> class using the content of <paramref name="fileName"/> for the comparison.
        /// </summary>
        public static  FileEnumerable<string> InsertAbove(this FileEnumerable<(Match, string)> fe, ModFile modFile, string fileName)
        {
            return new(fe.header, fe.ienumerable.InsertAbove(modFile.GetCode(fileName).Split("\n")));
        }
        /// <summary>
        /// An action on an IEnumerable that replace the Matching block with the lines given. 
        /// <example>
        /// For example:
        /// <code>
        /// List&lt;(Match, string)&gt; example = new() { (Match.Before, "Hello"), (Match.Matching, "World"), (Match.After, "!") };
        /// var replaced_example = example.ReplaceBy(new IEnumerable&lt; string &gt;() { " ", "I'm an example" });
        /// </code>
        /// results in <c>replaced_example</c> being new IEnumerable&lt; string &gt;() { "Hello", " ", "I'm an example", "!" };.
        /// </example>
        /// </summary>
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
        /// <summary>
        /// Same behaviour as <see cref="ReplaceBy"/> but using <paramref name="other"/>.Split('\n') for the comparison. 
        /// </summary>
        public static IEnumerable<string> ReplaceBy(this IEnumerable<(Match, string)> ienumerable, string replacing)
        {
            return ienumerable.ReplaceBy(replacing.Split("\n"));
        }
        /// <summary>
        /// Wrapper of <see cref="ReplaceBy"/> for the <see cref="FileEnumerable"/> class.
        /// </summary>
        public static  FileEnumerable<string> ReplaceBy(this FileEnumerable<(Match, string)> fe, string inserting)
        {
            return new(fe.header, fe.ienumerable.ReplaceBy(inserting.Split("\n")));
        }
        /// <summary>
        /// Wrapper of <see cref="ReplaceBy"/> for the <see cref="FileEnumerable"/> class using the content of <paramref name="fileName"/> for the comparison.
        /// </summary>
        public static  FileEnumerable<string> ReplaceBy(this FileEnumerable<(Match, string)> fe, ModFile modFile, string fileName)
        {
            return new(fe.header, fe.ienumerable.ReplaceBy(modFile.GetCode(fileName).Split("\n")));
        }
        /// <summary>
        /// Save the code handled during the chain of Matches and Actions.
        /// </summary>
        /// <returns>
        /// A <see cref="ModSummary"/> instance that can be used for debug purpose.
        /// </returns>
        public static ModSummary Save(this FileEnumerable<string> fe)
        {
            try {
                string newCode = string.Join("\n", fe.ienumerable);
                switch(fe.header.patchingWay) 
                {
                    case PatchingWay.GML:
                        fe.header.originalCode.ReplaceGML(newCode, ModLoader.Data);
                    break;

                    case PatchingWay.AssemblyAsString:
                        fe.header.originalCode.Replace(Assembler.Assemble(newCode, ModLoader.Data));
                    break;

                    default:
                    break;
                }
                Log.Information("Successfully patched function {{{0}}} with {{{1}}}", fe.header.fileName, fe.header.patchingWay.ToString());
                return new(
                    fe.header.fileName,
                    newCode,
                    fe.header.patchingWay
                );
            }
            catch(Exception ex) {
                Log.Error(ex, "Something went wrong");
                throw;
            }
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
        // https://stackoverflow.com/questions/70430422/how-to-implicitly-convert-nullable-type-to-non-nullable-type
        public static T ThrowIfNull<T>(
            this T? argument,
            string? message = default,
            [CallerArgumentExpression("argument")] string? paramName = default
        ) where T : notnull
        {
            if (argument is null)
            {
                Log.Error(string.Format("{0} is null.", argument));
                throw new ArgumentNullException(paramName, message);
            }
            else
            {
                return argument;
            }
        }

        public static T ThrowIfNull<T>(
            this T? argument,
            string? message = default,
            [CallerArgumentExpression("argument")] string? paramName = default
        ) where T : unmanaged
        {
            if (argument is null)
            {
                Log.Error(string.Format("{0} is null.", argument));
                throw new ArgumentNullException(paramName, message);
            }
            else
            {
                return (T)argument;
            }
        }
    }

    public static class RandomUtils
    {
        // Use to generate seed for UINT64 PRNG
        // SplitMix64, see https://prng.di.unimi.it/
        // this is an adaptation of https://prng.di.unimi.it/splitmix64.c
        private static ulong seed = 0;
        private static ulong[] s = { 0, 0, 0, 0};
        public static ulong NextSeed() {
            ulong z = seed += 0x9e3779b97f4a7c15;
            z = (z ^ (z >> 30)) * 0xbf58476d1ce4e5b9;
            z = (z ^ (z >> 27)) * 0x94d049bb133111eb;
            return z ^ (z >> 31);
        }
        private static void SetSeed(ulong newSeed) {
            seed = newSeed;
            s[0] = NextSeed();
            s[1] = NextSeed();
            s[2] = NextSeed();
            s[3] = NextSeed();
        }
        public static ulong Seed 
        { 
            get => seed; 
            set => SetSeed(value); 
        }
        // see https://prng.di.unimi.it/xoshiro256starstar.c
        private static ulong Rotl(ulong x, int k) 
        {
	        return (x << k) | (x >> (64 - k));
        }

        private static ulong NextUINT64() {
            ulong result = Rotl(s[1] * 5, 7) * 9;
            ulong t = s[1] << 17;

            s[2] ^= s[0];
            s[3] ^= s[1];
            s[1] ^= s[2];
            s[0] ^= s[3];

            s[2] ^= t;

            s[3] = Rotl(s[3], 45);

            return result;
        }

        // from Knuth Donald, The Art Of Computer Programming, Volume 2, Third Edition
        // 3.4.2 Random Sampling and Shuffling, p142
        // Algorithm S
        public static IEnumerable<T> SelectionSamplingTechnique<T>(this IList<T> list, int n)
        {
            // number of elements dealt with
            int tt = 0;
            // number of elements selected by the algorithm
            int m = 0;
            int N = list.Count;
            // firewall if we want more elements than the size of the list
            int nn = Math.Min(n, N);

            ulong x;
            double u;
            
            while(m < nn)
            {
                // they implement the xoshiro256** but only for non-negative int64
                // what some fucking donkeys
                // so I've written a proper xoshiro256** return a UINT64
                x = NextUINT64();
                // conversion to a [0, 1] uniform double
                // see https://prng.di.unimi.it/
                u = BitConverter.UInt64BitsToDouble(0x3FFL << 52 | x >> 12) - 1.0;

                if((N - tt)*u >= nn - m) 
                {
                    // element not selected
                    tt++;
                }
                else
                {
                    // element selected
                    yield return list[tt];
                    tt++;
                    m++;
                }
            }
        }
        public static IEnumerable<T> SelectionSamplingTechnique<T>(this IList<T> list, int n, ulong seed)
        {
            SetSeed(seed);
            return list.SelectionSamplingTechnique(n);
        }

        // from Knuth Donald, The Art Of Computer Programming, Volume 2, Third Edition
        // 3.4.2 Random Sampling and Shuffling, p145
        // Algorithm P
        // Known as the Fisher-Yates-Durstenfeld-Knuth algorithm
        public static void FydkShuffling<T>(this IList<T> list)
        {
            ulong tt = (ulong)list.Count;
            int j = (int)tt - 1;
            int k = 0;
            T temp;

            ulong x;
            // not an useless operation here since the division is an euclidian division
            // for instance 5 / 2 * 2 = 4 with int division
            ulong maxForMod = ulong.MaxValue / tt * tt;

            while(j > 0)
            {
                do
                {
                    x = NextUINT64();
                    // unbiased k in uniform [0, tt]
                    if (x < maxForMod)
                        k = (int)(x % tt);
                } while(x >= maxForMod);

                temp = list[k];
                list[k] = list[j];
                list[j] = temp;

                j--;
            }
        }
        public static void FydkShuffling<T>(this IList<T> list, ulong seed)
        {
            SetSeed(seed);
            list.FydkShuffling();
        }
    }

    public static class GenericUtils
    {
        public static void GenerateNRandomLinesFromCode(IList<UndertaleCode> code, GlobalDecompileContext context, int numberCode, int numberLinesByCode, ulong seed)
        {
            List<string> s = new();
            RandomUtils.Seed = seed;
            IEnumerable<UndertaleCode> selector = code.SelectionSamplingTechnique(numberCode);

            foreach(UndertaleCode uc in selector)
            {
                try
                {
                    s.AddRange(Decompiler.Decompile(uc, context).Split('\n').SelectionSamplingTechnique(numberLinesByCode));
                }
                catch(InvalidOperationException invalid)
                {
                    try
                    {
                        Log.Information(invalid.ToString());
                        // we encounter an error since we can't decompile a nested function
                        // the error message indicates where to look instead
                        // but you need to parse the message to retrieve the needed code
                        // "This code block represents a function nested inside " + code.ParentEntry.Name + " - decompile that instead"
                        string name = invalid.Message.Split('\"')[1];
                        Log.Information(string.Format("Looking for {{{0}}} instead", name));
                        s.AddRange(Decompiler.Decompile(code.First(x => x.Name.Content == name), context).Split('\n').SelectionSamplingTechnique(numberLinesByCode));
                    }
                    // not all code can be decompiled sadly
                    catch
                    {
                        string name = invalid.Message.Split('\"')[1];
                        Log.Information(string.Format("Cannot decompile {{{0}}}, skipping that file", name));
                        continue;
                    }
                    
                }
            }
            s.FydkShuffling();
            string joinedS = string.Join('\n', s);
            File.WriteAllText("_random_lines_for_test.txt", joinedS);
        }                 
    }
}