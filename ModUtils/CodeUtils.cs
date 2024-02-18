using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using ModShardLauncher.Resources.Codes;
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
        private readonly Regex variableRegex = new (@"\bpop\.v\.\w\s(?<var>\w+)\.(?<name>\w+)");
        public readonly Header header;
        public readonly IEnumerable<T> ienumerable;
        public FileEnumerable(Header header, IEnumerable<T> ienumerable) 
        {
            this.header = header;
            this.ienumerable = ienumerable;
        }
        /// <summary>
        /// Check pop variables for intructions as string and create them if needed.
        /// </summary>
        /// <param name="instructions"></param>
        public void CheckInstructionsVariables(string instructions)
        {
            if (header.patchingWay != PatchingWay.AssemblyAsString) return;

            foreach (string instruction in instructions.Split('\n').Where(x => x.Contains("pop.v")))
            {
                System.Text.RegularExpressions.Match matches = variableRegex.Match(instruction);
                if (matches.Success) 
                {
                    string instanceValue = matches.Groups["var"].Value;
                    if(instanceValue == "self")
                    {
                        AssemblyWrapper.CheckRefVariableOrCreate(matches.Groups["name"].Value, UndertaleInstruction.InstanceType.Self);
                    }
                    else if(instanceValue == "global")
                    {
                        AssemblyWrapper.CheckRefVariableOrCreate(matches.Groups["name"].Value, UndertaleInstruction.InstanceType.Global);
                    }
                    else if(instanceValue == "local")
                    {
                        AssemblyWrapper.CheckRefVariableOrCreate(matches.Groups["name"].Value, UndertaleInstruction.InstanceType.Local);
                    }
                    else
                    {
                        Log.Warning("Cannot infer the instance type of {0}. There is a risk it will lead to an undefined variable.", instruction);
                    }
                }
            }
        }
    }
    /// <summary>
    /// A static class for notably IEnumerable Extensions to provide a functional-programing-like api while modding.
    /// </summary>
    public static partial class Msl
    {
        /// <summary>
        /// Return the UndertaleCode from <paramref name="fileName"/>.
        /// </summary>
        public static UndertaleCode GetUMTCodeFromFile(string fileName)
        {
            try 
            {
                UndertaleCode code = ModLoader.Data.Code.First(t => t.Name?.Content == fileName);
                Log.Information(string.Format("Found function: {0}", code.ToString()));

                return code;
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static UndertaleCode AddCode(string codeAsString, string name)
        {
            try
            {
                UndertaleCode code = new();
                UndertaleCodeLocals locals = new();
                code.Name = ModLoader.Data.Strings.MakeString(name);
                locals.Name = code.Name;
                UndertaleCodeLocals.LocalVar argsLocal = new()
                {
                    Name = ModLoader.Data.Strings.MakeString("arguments"),
                    Index = 0
                };
                locals.Locals.Add(argsLocal);
                code.LocalsCount = 1;
                ModLoader.Data.CodeLocals.Add(locals);
                code.ReplaceGML(codeAsString, ModLoader.Data);
                ModLoader.Data.Code.Add(code);
                return code;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get code from file in this tool.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string GetCodeRes(string name)
        {
            var data = CodeResources.ResourceManager.GetObject(name, CodeResources.Culture) as byte[];
            if (data == null)
            {
                Log.Information($"Code resource not found :{name}");
                return "";
            }
            return Encoding.UTF8.GetString(data);
        }
        /// <summary>
        /// Add a new code from the code in this tool.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static UndertaleCode AddInnerCode(string name) => AddCode(GetCodeRes(name), name);
        /// <summary>
        /// Add a new function named <paramref name="name"/>.
        /// </summary>
        /// <param name="codeAsString"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static UndertaleCode AddFunction(string codeAsString, string name)
        {
            try
            {
                Log.Information(string.Format("Trying to add the function : {0}", name.ToString()));

                UndertaleCode scriptCode = AddCode(codeAsString, name);
                ModLoader.Data.Code.Add(ModLoader.Data.Code[0]);
                ModLoader.Data.Code.RemoveAt(0);

                Log.Information(string.Format("Successfully added the function : {0}", name.ToString()));
                return scriptCode;
            }
            catch 
            {
                throw;
            }
        }
        /// <summary>
        /// Add a new function from the code in this tool.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static UndertaleCode AddInnerFunction(string name) => AddFunction(GetCodeRes(name), name);
        /// <summary>
        /// Return the UndertaleCode as string from <paramref name="fileName"/>.
        /// </summary>
        public static string GetStringGMLFromFile(string fileName)
        {
            try 
            {
                UndertaleCode code = GetUMTCodeFromFile(fileName);
                GlobalDecompileContext context = new(ModLoader.Data, false);

                return Decompiler.Decompile(code, context);
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        /// <summary>
        /// Set the UndertaleCode in <paramref name="fileName"/> as <paramref name="codeAsString"/>.
        /// </summary>
        public static void SetStringGMLInFile(string codeAsString, string fileName)
        {
            try 
            {
                UndertaleCode code = GetUMTCodeFromFile(fileName);
                code.ReplaceGML(codeAsString, ModLoader.Data);
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        /// <summary>
        /// Insert GML <paramref name="codeAsString"/> from a string in <paramref name="fileName"/> at a given <paramref name="position"/>.
        /// <para>
        /// <example>For example:
        /// <code>
        /// InsertGMLString("scr_atr("LVL") == global.max_level", "gml_Object_o_character_panel_mask_Draw_0", 3);
        /// </code>
        /// results in gml_Object_o_character_panel_mask_Draw_0 line 3 being <c>scr_atr("LVL") == global.max_level</c>.
        /// </example>
        /// </para>
        /// </summary>
        /// <param name="codeAsString">The code to insert.</param>
        /// <param name="fileName">The file to be patched.</param>
        /// <param name="position">The exact position to insert.</param>
        public static void InsertGMLString(string codeAsString, string fileName, int position)
        {
            try 
            {
                Log.Information(string.Format("Trying insert code in: {0}", fileName.ToString()));

                List<string>? originalCode = GetStringGMLFromFile(fileName).Split("\n").ToList();
                originalCode.Insert(position, codeAsString);
                SetStringGMLInFile(string.Join("\n", originalCode), fileName);

                Log.Information(string.Format("Patched function with InsertGMLString: {0}", fileName.ToString()));
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        /// <summary>
        /// Replace an existing GML code by another <paramref name="code"/> from a string in <paramref name="file"/> at a given <paramref name="position"/>.
        /// <para>
        /// <example>For example:
        /// <code>
        /// ReplaceGMLString("scr_atr("LVL") == global.max_level", "gml_Object_o_character_panel_mask_Draw_0", 3);
        /// </code>
        /// results in gml_Object_o_character_panel_mask_Draw_0 line 3 being replaced by <c>scr_atr("LVL") == global.max_level</c>.
        /// </example>
        /// </para>
        /// </summary>
        /// <param name="code">The code to insert.</param>
        /// <param name="file">The file to be patched.</param>
        /// <param name="position">The exact position to insert.</param>
        public static void ReplaceGMLString(string codeAsString, string fileName, int position)
        {
            try 
            {
                Log.Information(string.Format("Trying replace code in: {0}", fileName.ToString()));

                List<string>? originalCode = GetStringGMLFromFile(fileName).Split("\n").ToList();
                originalCode[position] = codeAsString;
                SetStringGMLInFile(string.Join("\n", originalCode), fileName);

                Log.Information(string.Format("Patched function with ReplaceGMLString: {0}", fileName.ToString()));
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        /// <summary>
        /// Replace an existing GML code by another <paramref name="code"/> from a string in <paramref name="file"/> at a given <paramref name="position"/>
        /// and remove the next len-1 lines.
        /// <para>
        /// <example>For example:
        /// <code>
        /// ReplaceGMLString("scr_atr("LVL") == global.max_level", "gml_Object_o_character_panel_mask_Draw_0", 3, 2);
        /// </code>
        /// results in gml_Object_o_character_panel_mask_Draw_0 line 3 being replaced by <c>scr_atr("LVL") == global.max_level</c>
        /// and line 4 being removed.
        /// </example>
        /// </para>
        /// </summary>
        /// <param name="code">The code to insert.</param>
        /// <param name="file">The file to be patched.</param>
        /// <param name="position">The exact position to insert.</param>
        public static void ReplaceGMLString(string codeAsString, string fileName, int start, int len)
        {
            try 
            {
                Log.Information(string.Format("Trying replace code in: {0}", fileName.ToString()));

                List<string>? originalCode = GetStringGMLFromFile(fileName).Split("\n").ToList();
                originalCode[start] = codeAsString;
                for (int i = 1; i < Math.Min(len, originalCode.Count - start); i++) {
                    originalCode[start + i] = "";
                }

                SetStringGMLInFile(string.Join("\n", originalCode), fileName);

                Log.Information(string.Format("Patched function with ReplaceGMLString: {0}", fileName.ToString()));
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
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
        /// A selector that tags the first block of continuous lines that matches a first list of string until it matches a second list of string.
        /// Does nothing on its own, only tagging, needs to be used with an Action to properly modify your input.
        /// <example>
        /// For example:
        /// <code>
        /// List&lt; string &gt; example = new() { "Hello", "World", "!" };
        /// var matched_example = example.MatchFrom(new List&lt; string &gt;() { "Hel" }, new List&lt; string &gt;() { "!" });
        /// </code>
        /// results in <c>matched_example</c> being new IEnumerable&lt;(Match, string)&gt;() { (Match.Matching, "Hello"), (Match.Matching, "World"), (Match.Matching, "!") };.
        /// </example>
        /// </summary>
        public static IEnumerable<(Match, string)> MatchFromUntil(this IEnumerable<string> ienumerable, IEnumerable<string> otherfrom, IEnumerable<string> otheruntil)
        {
            bool foundUntil = false;
            bool exitMatching = false;

            string? otherUntilString = null;
            IEnumerator<string> otherUntilEnumerator = otheruntil.GetEnumerator();
            if(otherUntilEnumerator.MoveNext())
                otherUntilString = otherUntilEnumerator.Current;

            foreach ((Match m, string element) in ienumerable.MatchFrom(otherfrom))
            {
                if (m == Match.Before || m == Match.Matching)
                {
                    // before and matching stays as before and matching
                    yield return (m, element);
                }
                else if (!exitMatching && otherUntilString != null && element.Contains(otherUntilString))
                {
                    // if we match with the until, stay as matching
                    foundUntil = true;
                    yield return (Match.Matching, element);
                    if(otherUntilEnumerator.MoveNext())
                        otherUntilString = otherUntilEnumerator.Current;
                    else 
                    {
                        exitMatching = true;
                    }
                }
                else if (!foundUntil)
                {
                    // until we never encounter the until string, stay as matching
                    yield return (Match.Matching, element);
                }
                else
                {
                    // we encountered the until part and leave
                    // we are now in after
                    exitMatching = true;
                    yield return (Match.After, element);
                }
            }
        }
        /// <summary>
        /// Same behaviour as <see cref="MatchFromUntil"/> but using <paramref name="otherfrom"/>.Split('\n') and <paramref name="otheruntil"/>.Split('\n') for the comparison. 
        /// </summary>
        public static IEnumerable<(Match, string)> MatchFromUntil(this IEnumerable<string> ienumerable, string otherfrom, string otheruntil)
        {
            return ienumerable.MatchFromUntil(otherfrom.Split("\n"), otheruntil.Split("\n"));
        }
        /// <summary>
        /// Wrapper of <see cref="MatchFromUntil"/> for the <see cref="FileEnumerable"/> class.
        /// </summary>
        public static FileEnumerable<(Match, string)> MatchFromUntil(this FileEnumerable<string> fe, string otherfrom, string otheruntil)
        {
            return new(fe.header, fe.ienumerable.MatchFromUntil(otherfrom.Split("\n"), otheruntil.Split("\n")));
        }
        /// <summary>
        /// Wrapper of <see cref="MatchFromUntil"/> for the <see cref="FileEnumerable"/> class using the content of <paramref name="filenameOther"/> and <paramref name="filenameUntil"/> for the comparison.
        /// </summary>
        public static FileEnumerable<(Match, string)> MatchFromUntil(this FileEnumerable<string> fe, ModFile modFile, string filenameOther, string filenameUntil)
        {
            return new(fe.header, fe.ienumerable.MatchFromUntil(modFile.GetCode(filenameOther).Split("\n"), modFile.GetCode(filenameUntil).Split("\n")));
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
            fe.CheckInstructionsVariables(inserting);
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
            fe.CheckInstructionsVariables(inserting);
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
        public static FileEnumerable<string> ReplaceBy(this FileEnumerable<(Match, string)> fe, string inserting)
        {
            fe.CheckInstructionsVariables(inserting);
            return new(fe.header, fe.ienumerable.ReplaceBy(inserting.Split("\n")));
        }
        /// <summary>
        /// Wrapper of <see cref="ReplaceBy"/> for the <see cref="FileEnumerable"/> class using the content of <paramref name="fileName"/> for the comparison.
        /// </summary>
        public static FileEnumerable<string> ReplaceBy(this FileEnumerable<(Match, string)> fe, ModFile modFile, string fileName)
        {
            return new(fe.header, fe.ienumerable.ReplaceBy(modFile.GetCode(fileName).Split("\n")));
        }
        /// <summary>
        /// Apply an <paramref name="iterator"/> to an <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="ienumerable"></param>
        /// <param name="iterator"></param>
        /// <returns></returns>
        public static IEnumerable<string> Apply(this IEnumerable<string> ienumerable, Func<IEnumerable<string>, IEnumerable<string>> iterator)
        {
            return iterator(ienumerable);
        }
        /// <summary>
        /// Wrapper of <see cref="Apply"/> for the <see cref="FileEnumerable"/>.
        /// </summary>
        public static FileEnumerable<string> Apply(this FileEnumerable<string> fe, Func<IEnumerable<string>, IEnumerable<string>> iterator)
        {
            return new(fe.header, fe.ienumerable.Apply(iterator));
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
            catch
            {
                throw;
            }
        }
    }
}