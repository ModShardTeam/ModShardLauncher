using ModShardLauncher.Core.Models;
using ModShardLauncher.Mods;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ModShardLauncher
{
    public enum PatchStatus
    {
        None,
        Patching,
        Success,
    }
    public static class FileReader
    {
        private static List<FileChunk> ReadChunks(FileStream fs)
        {
            int count = BitConverter.ToInt32(ReadStream(fs, 4), 0);
            List<FileChunk> fileChunks = new();
            for (int i = 0; i < count; i++)
            {
                int len = BitConverter.ToInt32(ReadStream(fs, 4), 0);

                FileChunk chunk = new()
                {
                    name = Encoding.UTF8.GetString(ReadStream(fs, len)),
                    offset = BitConverter.ToInt32(ReadStream(fs, 4)),
                    length = BitConverter.ToInt32(ReadStream(fs, 4))
                };

                fileChunks.Add(chunk);
            }
            return fileChunks;
        }
        private static bool GetConcreteType(Type typeToGet, Type t)
        {
            // expect a non interface, non abstract typeToGet and with a constructor without any parameter
            return typeToGet.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null;
        }
        private static Mod? GetModInstance(Assembly assembly)
        {
            Type[] types;

            // load all types in the assembly
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                Log.Error("Failed to load types from assembly {0}: {1}", assembly.GetName().Name, ex.Message);
                foreach (Exception? loaderEx in ex.LoaderExceptions.Where(e => e != null))
                {
                    Log.Warning("Loader exception: {LoaderError}", loaderEx!.Message);
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error loading types from {0}: {1}", assembly.GetName().Name, ex.Message);
                return null;
            }

            // capture the Mod type if it exists
            Type? modType = Array.Find(types, t => GetConcreteType(typeof(Mod), t));

            // check if Mod was correctly found
            if (modType == null)
            {
                Log.Warning(
                    "No valid Mod class found in assembly {0}. Expected a non-abstract class inheriting from Mod with parameterless constructor.",
                    assembly.GetName().Name
                );
                return null;
            }

            try
            {
                if (Activator.CreateInstance(modType) is not Mod mod)
                {
                    Log.Error("Created instance of {0} is not assignable to Mod (this should not happen)", modType.Name);
                    return null;
                }
                return mod;
                
            }
            catch (Exception ex)
            {
                Log.Error("Failed to create instance of mod type {0}: {1}", modType.Name, ex.Message);
            }
            return null;
        }
        public static ModFile? Read(string path)
        {
            using FileStream fs = new(path, FileMode.Open);
            string nameMod = fs.Name.Split("\\")[^1].Replace(".sml", "");
            if (Encoding.UTF8.GetString(ReadStream(fs, 4)) != "MSLM")
            {
                fs.Close();
                return null;
            }

            Regex reg = new("0([0-9])");
            byte pointBytes = 0x2E;
            byte zeroBytes = 0x30;
            byte nineBytes = 0x39;
            // the version number should be at least 24 bytes
            byte[] readbytes = ReadStream(fs, 24);
            // resizing
            int size = 24;
            // i = 0 is a v
            for (int i = 1; i < 24; i++)
            {
                // either a point or in a range of [0-9]
                if (readbytes[i] != pointBytes && (readbytes[i] > nineBytes || readbytes[i] < zeroBytes))
                {
                    size = i;
                    break;
                }
            }

            if (size == 24)
            {
                Log.Warning("Version number seems ill formed");
            }

            // restarting the buffer
            fs.Seek(-24, SeekOrigin.Current);
            // reading the correct version
            byte[] versionbytes = ReadStream(fs, size);

            string version = reg.Replace(Encoding.UTF8.GetString(versionbytes), "$1");
            List<FileChunk> fileChunks = new();

            // read textures
            fileChunks.AddRange(ReadChunks(fs));
            // scripts
            fileChunks.AddRange(ReadChunks(fs));
            // codes
            fileChunks.AddRange(ReadChunks(fs));
            // assembly
            fileChunks.AddRange(ReadChunks(fs));

            int fileOffset = (int)fs.Position;

            int fileCount = fileChunks.Count;
            if (fileCount > 0)
            {
                FileChunk? f = fileChunks[fileCount - 1];
                ReadStream(fs, f.offset + f.length);
            }

            int count = BitConverter.ToInt32(ReadStream(fs, 4), 0);
            Assembly fileAssembly = Assembly.Load(ReadStream(fs, count));
            Mod? mod = GetModInstance(fileAssembly);
            if (mod == null) return null;

            ModFile modFile = new(nameMod, version, fileAssembly, path, fileChunks, fileOffset, mod);
            try
            {
                modFile.Icon = modFile.GetIcon(fs);
            }
            catch (Exception ex)
            {
                Log.Information("{0}", ex);
                Log.Information("Cannot find the icon.png associated to {0}", fs.Name.Split("\\")[^1]);
            }

            fs.Close();
            Log.Information("Reading {{{0}}} built with version {{{1}}}", nameMod, modFile.Version);
            return modFile;
        }
        public static byte[] ReadStream(FileStream fs, int length)
        {
            byte[] bytes = new byte[length];
            if(fs.Length - fs.Position < length)
            {
                fs.Close();
                throw new Exception($"In FileReader.Read cannot read {length} bytes in the mod {fs.Name.Split("\\")[^1]}");
            }
            fs.Read(bytes, 0, length);
            return bytes;
        }
    }
}
