using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using ModShardLauncher.Mods;
using Serilog;

namespace ModShardLauncher.Core.Models
{
    public class ModFile
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public List<FileChunk> Files { get; set; }
        public Assembly Assembly { get; set; }
        public byte[] Icon { get; set; } = Array.Empty<byte>();
        public int FileOffset { get; set; }
        public string Path { get; set; }
        public bool Existed => File.Exists(Path);
        public Mod Instance { get; set; }
        public PatchStatus PatchStatus { get; set; } = PatchStatus.None;
        public bool Enabled { get; set; } = false;
        public ModFile() { }
        public ModFile(string name, string version, Assembly assembly, string path, List<FileChunk> files, int fileOffset, Mod instance)
        {
            Name = name;
            Version = version;
            Assembly = assembly;
            Path = path;
            Files = files;
            FileOffset = fileOffset;

            Instance = instance;
            instance.ModFiles = this;
        }
        public bool FileExist(string fileName)
        {
            return GetFile(fileName).Length > 0;
        }
        public override string ToString()
        {
            return Instance.ToString();
        }
        public byte[] GetFile(string fileName)
        {
            using FileStream stream = new(Path, FileMode.Open);

            byte[] read = GetFile(stream, fileName);
            stream.Close();
            
            return read;
        }
        public byte[] GetFile(FileStream fileStream, string fileName)
        {
            if (!Existed)
            {
                // TODO make a throw instead
                MessageBox.Show(Application.Current.FindResource("ModLostWarning").ToString() + " : " + Name);
                ModLoader.LoadFiles();
                return Array.Empty<byte>();
            }

            FileChunk? file = Files.Find(t => System.IO.Path.GetFileName(t.name) == System.IO.Path.GetFileName(fileName));
            if (file != null)
            {
                fileStream.Position = FileOffset;
                FileReader.ReadStream(fileStream, file.offset);

                byte[] read = FileReader.ReadStream(fileStream, file.length);
                return read;
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
        public byte[] GetIcon()
        {
            return GetFile(Name + "\\icon.png");
        }
        public byte[] GetIcon(FileStream fileStream)
        {
            return GetFile(fileStream, Name + "\\icon.png");
        }
    }
}