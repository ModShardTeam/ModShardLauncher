using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Serilog;

namespace ModShardLauncher.Core.Models
{
    public class FileChunk 
    {
        public string name = string.Empty;
        public int offset;
        public int length;
    }
    public class ModFile
    {
        public string Name = string.Empty;
        public string Version { get; set; } = string.Empty;
        public List<FileChunk> Files = new();
        public Assembly Assembly;
        public int FileOffset;
        public FileStream Stream;
        public string Path = string.Empty;
        public Mod Instance { get; set; }
        public bool Enabled { get; set; }
        public PatchStatus PatchStatus { get; set; } = PatchStatus.None;
        public bool Existed => File.Exists(Path);
        public byte[] Icon { get; set; } = Array.Empty<byte>();
        public override string ToString()
        {
            return Instance.ToString();
        }
        public byte[] GetFile(string fileName)
        {
            if (!Existed)
            {
                MessageBox.Show(Application.Current.FindResource("ModLostWarning").ToString() + " : " + Name);
                ModLoader.LoadFiles();
                return Array.Empty<byte>();
            }

            // https://sonarsource.github.io/rspec/#/rspec/S6602/csharp
            // for list, Find should be used instead of FirstOrDefault
            FileChunk? file = Files.Find(t => System.IO.Path.GetFileName(t.name) == System.IO.Path.GetFileName(fileName));
            if (file != null)
            {
                if (!Stream.CanRead) Stream = new FileStream(Path, FileMode.Open);
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
            byte[] data = GetFile(fileName);
            if (data.Length == 0)
            {
                Log.Warning($"{fileName} is empty.");
                return "";
            }
            // if a BOM is found aka: 0xEF 0xBB 0xBF at the beginning of the file, remove it since UTMT will not understand these characters.
            // BOM are produced if a script is made through Visual Studio
            if (data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF) data = data.Skip(3).ToArray();

            string text = Encoding.UTF8.GetString(data);
            if (text.Length == 0)
            {
                MessageBox.Show(Application.Current.FindResource("ModLostWarning").ToString() + " : " + fileName);
                throw new ArgumentException("String cannot be of length zero");
            }
            return text;
        }
        public bool FileExist(string fileName)
        {
            return GetFile(fileName).Length > 0;
        }
    }
}