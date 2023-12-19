using ModShardLauncher;
using ModShardLauncher.Mods;
using System.Collections.Generic;
using System.Linq;

namespace SpeedShard
{
    public class SpeedShard : Mod
    {
        public override string Author => "Zizani";
        public override string Name => "SpeedShard";
        public override string Description => "This mod is an attempt to reduce player time to reach late game content";
        public override string Version => "1.0.0.1";

        public List<string> WaterContainers = new List<string>();

        public override void PatchMod()
        {
            InsertLoadIni();
            LoadWaterContainers();
            SetWaterCharge();
            ModLoader.SetDecompiledCode(ModFiles.GetCode("4xSpeed.gml"), "gml_Object_o_player_KeyPress_115");
            ModLoader.SetDecompiledCode(ModFiles.GetCode("QuickSave.gml"), "gml_Object_o_player_KeyPress_116");
        }
        /// <summary>
        /// load ini file
        /// </summary>
        public void InsertLoadIni()
        {
            ModLoader.InsertDecompiledCode(ModFiles.GetCode("LoadIni.gml"), "gml_GlobalScript_scr_sessionDataInit", 2);
            ModLoader.InsertDecompiledCode("game_set_speed(global.gamespeed, gamespeed_fps)", "gml_Object_o_player_Create_0", 6);
        }
        /// <summary>
        /// load all water containers need to set
        /// </summary>
        public void LoadWaterContainers()
        {
            WaterContainers.Add("gml_Object_o_inv_wooden_bucket_water_Create_0");
            WaterContainers.Add("gml_Object_o_inv_hunting_horn_water_Create_0");
            WaterContainers.Add("gml_Object_o_inv_golden_cup_water_Create_0");
            WaterContainers.Add("gml_Object_o_inv_clay_pot_water_Create_0");
            WaterContainers.Add("gml_Object_o_inv_wineskin_Create_0");
        }
        /// <summary>
        /// add this code
        /// </summary>
        public void SetWaterCharge()
        {
            foreach(var item in WaterContainers)
                ModLoader.InsertDecompiledCode("charge *= global.water_container_modifier", item, 2);
        }
        /// <summary>
        /// change campBed code
        /// </summary>
        public void SetCampBed()
        {
            ModLoader.ReplaceDecompiledCode("if (_days >= global.campbed_despawn_days)", "gml_Object_c_bed_sleep_crafted_Alarm_0", 2);
            ModLoader.ReplaceDecompiledCode("   if (object_index == o_campbed_crafted && !global.campbed_not_destroyed)", "gml_Object_o_sleepController_Other_12", 38);
        }
    }
}