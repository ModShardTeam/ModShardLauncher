namespace ModShardLauncher.Core.Models
{
    public class Mod
    {
        public override string ToString()
        {
            return Name;
        }
        public virtual string Name { get => GetType().Name; }
        public virtual string Author { get => "未知"; }
        public virtual string Description { get => "未知"; }
        public virtual string ShortDesc { get => "未知"; }
        public virtual string Version { get => "v0.0.0.0"; }
        public virtual string TargetVersion { get => "v0.0.0.0"; }
        public ModFile ModFiles = new();
        public Mod() { }
        public virtual void PatchMod()
        {

        }
        public byte[] GetFile(string fileName)
        {
            return ModFiles.GetFile(fileName);
        }
        public string GetCode(string fileName)
        {
            return ModFiles.GetCode(fileName);
        }
        public bool FileExist(string fileName)
        {
            return GetFile(fileName).Length > 0;
        }
    }
}
