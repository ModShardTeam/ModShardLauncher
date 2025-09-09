using System.IO;

namespace ModShardLauncher.Core.Models
{
    public class ModSource
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool Existed => File.Exists(Path);
        public override string ToString()
        {
            return Name;
        }
    }
}