using ModShardLauncher.Mods;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ModShardLauncher
{
    public class FileChunk 
    {
        public string name;
        public int offset;
        public int length;
    }
    public class ModSource
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool isExisted => File.Exists(Path);
        public override string ToString()
        {
            return Name;
        }
    }
    public enum PatchStatus
    {
        None,
        Patching,
        Success,
    }
    public class ModFile
    {
        public string Name;
        public string Version {  get; set; }
        public List<FileChunk> Files = new();
        public Assembly Assembly;
        public int FileOffset;
        public FileStream Stream;
        public string Path;
        public Mod instance { get; set; }
        public bool isEnabled { get; set; }
        public PatchStatus PatchStatus { get; set; } = PatchStatus.None;
        public bool isExisted => File.Exists(Path);
        public byte[] Icon { get; set; } = Array.Empty<byte>();
        public override string ToString()
        {
            return instance.ToString();
        }
        public byte[] GetFile(string fileName)
        {
            if(!isExisted)
            {
                MessageBox.Show(Application.Current.FindResource("ModLostWarning").ToString() + " : " + Name);
                ModLoader.LoadFiles();
                return Array.Empty<byte>();
            }

            // https://sonarsource.github.io/rspec/#/rspec/S6602/csharp
            // for list, Find should be used instead of FirstOrDefault
            FileChunk? file = Files.Find(t => System.IO.Path.GetFileName(t.name) == fileName);
            if (file != null)
            {
                if(!Stream.CanRead) Stream = new FileStream(Path, FileMode.Open);
                Stream.Position = FileOffset;
                FileReader.Read(Stream, file.offset);
                byte[] fileStream = FileReader.Read(Stream, file.length);
                Stream.Close();
                return fileStream;
            }
            throw new FileNotFoundException(string.Format("File {0} not found in the packed sml.", fileName));
        }
        public string GetCode(string fileName)
        {
            try
            {
                byte[] data = GetFile(fileName);

                // if a BOM is found aka: 0xEF 0xBB 0xBF at the beginning of the file, remove it since UTMT will not understand these characters.
                // BOM are produced if a script is made through Visual Studio
                if (data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF) data = data.Skip(3).ToArray();

                string text = Encoding.UTF8.GetString(data);
                if(text.Length == 0)
                {
                    MessageBox.Show(Application.Current.FindResource("ModLostWarning").ToString() + " : " + fileName);
                    throw new ArgumentException("String cannot be of length zero");
                }
                return text;
            }
            catch
            {
                throw;
            }
        }
        public bool FileExist(string fileName)
        {
            try
            {
                return GetFile(fileName).Length > 0;
            }
            catch
            {
                throw;
            }
        }
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
            Log.Information(string.Format("Reading {{{0}}} built with version {1}", nameMod, file.Version));

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
                Log.Information(string.Format("Cannot find the icon.png associated to {0}", fs.Name.Split("\\")[^1]));
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
                throw new Exception(string.Format("In FileReader.Read cannot read {0} bytes in the mod {1} ",  length, fs.Name.Split("\\")[^1]));
            }
            fs.Read(bytes, 0, length);
            return bytes;
        }
    }
}
