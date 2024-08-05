using System;
using System.IO;
using Serilog;
using System.Diagnostics;
using ModShardPackerReference;

namespace ModShardLauncher
{
    public static class UtilsPacker
    {
        public static bool Pack(string path)
        {
            bool resultPacking = false;

            try
            {
                resultPacking = FilePacker.Pack(
                    null, 
                    path, 
                    ModLoader.ModPath, 
                    path, 
                    Main.Instance.mslVersion, 
                    new Type[2] {typeof(ModShardLauncher.Mods.Mod), typeof(UndertaleModLib.Models.UndertaleCode)}
                );
            }
            catch(Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentException || ex is IOException || ex is DirectoryNotFoundException)
                {
                    Log.Error(ex.ToString());
                }
                else
                {
                    Log.Error(ex, "Unexpected error");
                }
                Console.WriteLine(ex.Message);
                Log.Error(ex.ToString());
            }

            return resultPacking;
        }
    }
}
