using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModShardLauncher.Mods
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
        public List<Weapon> ModWeapons = new();
        public ModFile ModFiles = new();
        public Mod() { }
        public virtual void LoadAssembly()
        {

        }
        public virtual void PatchMod()
        {

        }
    }
    public enum ModLanguage
    {
        Russian,
        English,
        Chinese,
        German,
        Spanish,
        French,
        Italian,
        Portuguese,
        Polish,
        Turkish,
        Japanese,
        Korean
    }
}
