using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
}