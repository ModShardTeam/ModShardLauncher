using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
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