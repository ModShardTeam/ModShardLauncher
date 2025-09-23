using System.IO;

namespace ModShardLauncher.Core.Models
{
    public class FileChunk
    {
        public string name = string.Empty;
        public int offset;
        public int length;
    }
}