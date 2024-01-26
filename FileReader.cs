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
    public class ModFile
    {
        public string Name;
        public string Version {  get; set; }
        public List<FileChunk> Files = new List<FileChunk>();
        public Assembly Assembly;
        public int FileOffset;
        public FileStream Stream;
        public string Path;
        public Mod instance { get; set; }
        public bool isEnabled { get; set; }
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
            FileChunk? file = Files.Find(t => t.name == fileName) ?? Files.Find(t => t.name.Split("\\")[^1] == fileName);
            if (file != null)
            {
                if(!Stream.CanRead) Stream = new FileStream(Path, FileMode.Open);
                Stream.Position = FileOffset;
                FileReader.Read(Stream, file.offset);
                return FileReader.Read(Stream, file.length);
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

            ModFile file = new()
            {
                Stream = fs,
                Name = fs.Name.Split("\\")[^1].Replace(".sml", "")
            };

            if (Encoding.UTF8.GetString(Read(fs, 4)) != "MSLM")
            {
                fs.Close();
                return null;
            }

            Regex? reg = new("0([0-9])");
            file.Version = reg.Replace(Encoding.UTF8.GetString(Read(fs, 12)), "$1");

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
