using System;
using System.IO;
using Serilog;
using System.Diagnostics;
using ModShardPackerReference;

namespace ModShardLauncher
{
    public static class UtilsPacker
    {
        /// <summary>
        /// Pack a mod located in <paramref name="path"/> using the packing method from <see cref="ModShardPackerReference"/>.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void Pack(string path)
        {
            FilePacker.Pack(
                null, 
                path, 
                ModLoader.ModPath, 
                path, 
                Main.Instance.mslVersion, 
                new Type[2] {typeof(ModShardLauncher.Mods.Mod), typeof(UndertaleModLib.Models.UndertaleCode)}
            );
        }
    }
}
