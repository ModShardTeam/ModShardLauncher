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
            // work around to find the FileVersion of ModShardLauncher.dll for single file publishing
            // see: https://github.com/dotnet/runtime/issues/13051
            string mslVersion;
            try
            {
                ProcessModule mainProcess = Msl.ThrowIfNull(Process.GetCurrentProcess().MainModule);
                string mainProcessName = Msl.ThrowIfNull(mainProcess.FileName);
                mslVersion = "v" + FileVersionInfo.GetVersionInfo(mainProcessName).FileVersion;
            }
            catch(FileNotFoundException ex)
            {
                Log.Error(ex, "Cannot find the dll of ModShardLauncher");
                return false;
            }

            bool resultPacking = false;

            try
            {
                resultPacking = FilePacker.Pack(
                    null, 
                    path, 
                    ModLoader.ModPath, 
                    path, 
                    mslVersion, 
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
