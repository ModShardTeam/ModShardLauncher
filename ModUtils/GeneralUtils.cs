using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ModShardLauncher.Mods;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    /// <summary>
    /// Static utilities for ModLoader
    /// </summary>
    public static partial class Msl
    {
         public static readonly int ModLanguageSize = Enum.GetNames(typeof(ModLanguage)).Length;
        public static FileEnumerable<string> LoadGML(string fileName)
        {
            try 
            {
                UndertaleCode code = GetUMTCodeFromFile(fileName);
                GlobalDecompileContext context = new(ModLoader.Data, false);

                return new(
                    new(
                        fileName,
                        code,
                        PatchingWay.GML
                    ),
                    Decompiler.Decompile(code, context).Split("\n")
                );
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static FileEnumerable<string> LoadAssemblyAsString(string fileName)
        {
            try 
            {
                UndertaleCode code = GetUMTCodeFromFile(fileName);
                
                return new(
                    new(
                        fileName,
                        code,
                        PatchingWay.AssemblyAsString
                    ),
                    code.Disassemble(ModLoader.Data.Variables, ModLoader.Data.CodeLocals.For(code)).Split("\n")
                );
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
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
}