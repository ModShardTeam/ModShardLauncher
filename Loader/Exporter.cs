using UndertaleModLib;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serilog;
using UndertaleModLib.Decompiler;

namespace ModShardLauncher.Loader
{
    public enum DataType
    {
        Codes,
        Items,
        DungeonPresets,
    }
    static public class ExporterExtensions
    {
        /// <summary>
        /// Export all code, variables and rooms name in json.
        /// </summary>
        private static void ExportCodes(UndertaleData data)
        {
            File.WriteAllText("json_dump_code.json", JsonConvert.SerializeObject(data.Code.Select(t => t.Name.Content)));
            File.WriteAllText("json_dump_variables.json", JsonConvert.SerializeObject(data.Variables.Select(t => t.Name.Content)));
            File.WriteAllText("json_dump_rooms.json", JsonConvert.SerializeObject(data.Rooms.Select(t => t.Name.Content)));
        }
        /// <summary>
        /// Export all items, weapons and armors in csv.
        /// </summary>
        private static void ExportItems(UndertaleData data)
        {
            DirectoryInfo dir = new("export");
            if (dir.Exists) dir.Delete(true);
            dir.Create();

            List<string>? weapons = ModLoader.GetTable("gml_GlobalScript_table_weapons");
            List<string>? armor = ModLoader.GetTable("gml_GlobalScript_table_armor");

            File.WriteAllLines(
                Path.Join(dir.FullName, Path.DirectorySeparatorChar.ToString(), "_all_items.csv"),
                data.GameObjects.Select(t => t.Name.Content).Where(x => x.Contains("o_inv_")).Select(x => x.Replace("o_inv_", ""))
            );

            if (weapons != null)
            {
                File.WriteAllLines(
                    Path.Join(dir.FullName, Path.DirectorySeparatorChar.ToString(), "_all_weapons.csv"),
                    weapons.Select(x => string.Join(';', x.Split(';').Take(4)))
                );
            }

            if (armor != null)
            {
                File.WriteAllLines(
                    Path.Join(dir.FullName, Path.DirectorySeparatorChar.ToString(), "_all_armors.csv"),
                    armor.Select(x => string.Join(';', x.Split(';').Take(6)))
                );
            }
        }
        /// <summary>
        /// Export all preset data for all dungeons in json.
        /// </summary>
        private static void ExportPresets(UndertaleData data)
        {
            GlobalDecompileContext context = new(ModLoader.Data, false);
            File.WriteAllText("json_preset_bastion.json", Decompiler.Decompile(data.Code.First(t => t.Name.Content.Contains("scr_preset_bastion_1")), context));
            File.WriteAllText("json_preset_catacombs.json", Decompiler.Decompile(data.Code.First(t => t.Name.Content.Contains("scr_preset_catacombs")), context));
            File.WriteAllText("json_preset_crypt.json", Decompiler.Decompile(data.Code.First(t => t.Name.Content.Contains("scr_preset_crypt_1")), context));
        }
        /// <summary>
        /// Export data for code exploration in json or csv.
        /// </summary>
        public static void Export(this UndertaleData data, DataType dataType)
        {
            try
            {
                switch (dataType)
                {
                    case DataType.Codes:
                        ExportCodes(data);
                        break;
                    case DataType.Items:
                        ExportItems(data);
                        break;
                    case DataType.DungeonPresets:
                        ExportPresets(data);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong while exporting {{{0}}}", dataType);
            }
        }
    }
}