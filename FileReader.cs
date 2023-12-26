using ModShardLauncher.Mods;
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
        public byte[] Icon { get; set; } = new byte[0];
        public override string ToString()
        {
            return instance.ToString();
        }
        public byte[] GetFile(string name)
        {
            if(!isExisted)
            {
                MessageBox.Show(Application.Current.FindResource("ModLostWarning").ToString() + " : " + Name);
                ModLoader.LoadFiles();
                return new byte[0];
            }
            var file = Files.FirstOrDefault(t => t.name == name);
            if(file == default) file = Files.FirstOrDefault(t => t.name.Split("\\").Last() == name);
            if (file != default)
            {
                if(!Stream.CanRead) Stream = new FileStream(Path, FileMode.Open);
                Stream.Position = FileOffset;
                FileReader.Read(Stream, file.offset);
                return FileReader.Read(Stream, file.length);
            }
            else return new byte[0];
        }
        public string GetCode(string name)
        {
            var data = GetFile(name);

            // if a BOM is found aka: 0xEF 0xBB 0xBF at the beginning of the file, remove it since UTMT will not understand these characters.
            // BOM are produced if a script is made through Visual Studio
            if (data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF) data = data.Skip(3).ToArray();
            
            var text = Encoding.UTF8.GetString(data);
            if(text.Length == 0)
            {
                MessageBox.Show(Application.Current.FindResource("ModLostWarning").ToString() + " : " + name);
                return "";
            }
            return text;
        }
        public bool FileExist(string name)
        {
            return GetFile(name).Length > 0;
        }
    }
    public class FileReader
    {
        public static ModFile Read(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            var file = new ModFile();
            file.Stream = fs;
            file.Name = fs.Name.Split("\\").Last().Replace(".sml", "");
            if (Encoding.UTF8.GetString(Read(fs, 4)) != "MSLM")
            {
                fs.Close();
                return null;
            }
            var reg = new Regex("0([0-9])");
            file.Version = reg.Replace(Encoding.UTF8.GetString(Read(fs, 12)), "$1");
            var count = BitConverter.ToInt32(Read(fs, 4), 0);
            for(int i = 0; i < count; i++)
            {
                var len = BitConverter.ToInt32(Read(fs, 4));
                var chunk = new FileChunk();
                chunk.name = Encoding.UTF8.GetString(Read(fs, len));
                chunk.offset = BitConverter.ToInt32(Read(fs, 4));
                chunk.length = BitConverter.ToInt32(Read(fs, 4));
                file.Files.Add(chunk);
            }
            count = BitConverter.ToInt32(Read(fs, 4), 0);
            for (int i = 0; i < count; i++)
            {
                var len = BitConverter.ToInt32(Read(fs, 4), 0);
                var chunk = new FileChunk();
                chunk.name = Encoding.UTF8.GetString(Read(fs, len));
                chunk.offset = BitConverter.ToInt32(Read(fs, 4));
                chunk.length = BitConverter.ToInt32(Read(fs, 4));
                file.Files.Add(chunk);
            }
            count = BitConverter.ToInt32(Read(fs, 4), 0);
            for (int i = 0; i < count; i++)
            {
                var len = BitConverter.ToInt32(Read(fs, 4), 0);
                var chunk = new FileChunk();
                chunk.name = Encoding.UTF8.GetString(Read(fs, len));
                chunk.offset = BitConverter.ToInt32(Read(fs, 4));
                chunk.length = BitConverter.ToInt32(Read(fs, 4));
                file.Files.Add(chunk);
            }
            file.FileOffset = (int)fs.Position;
            
            if(file.Files.Count > 0)
            {
                var f = file.Files.Last();
                Read(fs, f.offset + f.length);
            }
            count = BitConverter.ToInt32(Read(fs, 4), 0);
            file.Assembly = Assembly.Load(Read(fs, count));
            file.Path = path;
            if (file.FileExist(file.Name + "\\icon.png"))
                file.Icon = file.GetFile(file.Name + "\\icon.png");
            fs.Close();
            return file;
        }
        public static byte[] Read(FileStream fs, int length, int offset = 0)
        {
            byte[] bytes = new byte[length];
            if(fs.Length - fs.Position < length)
            {
                fs.Close();
                throw new Exception("Mod File Error: " + fs.Name.Split("\\").Last());
            }
            fs.Read(bytes, 0, length);
            return bytes;
        }
    }
}
