using ModShardLauncher.Core.Models;
using Serilog;
using System;
using System.IO;
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
        public static ModFile? Read(string path)
        {
            FileStream fs = new(path, FileMode.Open);
            string nameMod = fs.Name.Split("\\")[^1].Replace(".sml", "");

            ModFile file = new()
            {
                Stream = fs,
                Name = nameMod
            };

            if (Encoding.UTF8.GetString(Read(fs, 4)) != "MSLM")
            {
                fs.Close();
                return null;
            }

            Regex? reg = new("0([0-9])");
            byte pointBytes = 0x2E;
            byte zeroBytes = 0x30;
            byte nineBytes = 0x39;
            // the version number should be at least 24 bytes
            byte[] readbytes = Read(fs, 24);
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
            byte[] versionbytes = Read(fs, size);

            file.Version = reg.Replace(Encoding.UTF8.GetString(versionbytes), "$1");
            Log.Information("Reading {{{0}}} built with version {{{1}}}", nameMod, file.Version);

            // read textures
            int count = BitConverter.ToInt32(Read(fs, 4), 0);
            for (int i = 0; i < count; i++)
            {
                int len = BitConverter.ToInt32(Read(fs, 4));

                FileChunk chunk = new()
                {
                    name = Encoding.UTF8.GetString(Read(fs, len)),
                    offset = BitConverter.ToInt32(Read(fs, 4)),
                    length = BitConverter.ToInt32(Read(fs, 4))
                };

                file.Files.Add(chunk);
            }
            
            // scripts
            count = BitConverter.ToInt32(Read(fs, 4), 0);
            for (int i = 0; i < count; i++)
            {
                int len = BitConverter.ToInt32(Read(fs, 4), 0);

                FileChunk chunk = new()
                {
                    name = Encoding.UTF8.GetString(Read(fs, len)),
                    offset = BitConverter.ToInt32(Read(fs, 4)),
                    length = BitConverter.ToInt32(Read(fs, 4))
                };

                file.Files.Add(chunk);
            }

            // codes
            count = BitConverter.ToInt32(Read(fs, 4), 0);
            for (int i = 0; i < count; i++)
            {
                int len = BitConverter.ToInt32(Read(fs, 4), 0);

                FileChunk chunk = new()
                {
                    name = Encoding.UTF8.GetString(Read(fs, len)),
                    offset = BitConverter.ToInt32(Read(fs, 4)),
                    length = BitConverter.ToInt32(Read(fs, 4))
                };

                file.Files.Add(chunk);
            }
            
            // assembly
            count = BitConverter.ToInt32(Read(fs, 4), 0);
            for (int i = 0; i < count; i++)
            {
                int len = BitConverter.ToInt32(Read(fs, 4), 0);

                FileChunk chunk = new()
                {
                    name = Encoding.UTF8.GetString(Read(fs, len)),
                    offset = BitConverter.ToInt32(Read(fs, 4)),
                    length = BitConverter.ToInt32(Read(fs, 4))
                };

                file.Files.Add(chunk);
            }

            file.FileOffset = (int)fs.Position;
            
            int fileCount = file.Files.Count;
            if(fileCount > 0)
            {
                FileChunk? f = file.Files[fileCount - 1];
                Read(fs, f.offset + f.length);
            }

            count = BitConverter.ToInt32(Read(fs, 4), 0);
            file.Assembly = Assembly.Load(Read(fs, count));
            file.Path = path;

            try
            {
                file.Icon = file.GetFile(file.Name + "\\icon.png");
            }
            catch
            {
                Log.Information("Cannot find the icon.png associated to {0}", fs.Name.Split("\\")[^1]);
            }

            fs.Close();

            return file;
        }
        public static byte[] Read(FileStream fs, int length)
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
