using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModTool;

namespace ModShardLauncher
{
    public static partial class Msl
    {
        public static IEnumerable<UndertaleRoom> GetRooms()
        {
            return ModLoader.Data.Rooms;
        }
        public static UndertaleRoom GetRoom(string name)
        {
            try
            {
                UndertaleRoom room = ModLoader.Data.Rooms.First(t => t.Name.Content == name);
                Log.Information($"Found room {name}");
                return room;
            }
            catch
            {
                throw;
            }
        }
        public static UndertaleRoom AddRoom(string name)
        {
            UndertaleRoom room = new()
            {
                Name = ModLoader.Data.Strings.MakeString(name),
                Caption = ModLoader.Data.Strings.MakeString(""),
                Flags = UndertaleRoom.RoomEntryFlags.IsGMS2 ^ UndertaleRoom.RoomEntryFlags.EnableViews,
            };
            ModLoader.Data.Rooms.Add(room);
            Log.Information($"Successfully created room {name}");
            return room;
        }
        public static UndertaleRoom AddRoom(string name, uint width, uint height)
        {
            UndertaleRoom room = AddRoom(name);
            room.Width = width;
            room.Height = height; 
            return room;
        }
        private static UndertaleRoom.Layer CreateLayer<T>(UndertaleRoom room, UndertaleRoom.LayerType type, string name) where T : UndertaleRoom.Layer.LayerData, new()
        {
            uint largest_layerid = 0;
            foreach (UndertaleRoom Room in ModLoader.Data.Rooms)
            {
                foreach (uint layerId in Room.Layers.Select(Layer => Layer.LayerId))
                {
                    if (layerId > largest_layerid)
                        largest_layerid = layerId;
                }
            }

            long layerDepth = 0;
            if (room.Layers.Count > 0)
            {
                layerDepth = room.Layers.Select(l => l.LayerDepth).Max();
                if (layerDepth + 100 > int.MaxValue)
                {
                    if (layerDepth + 1 > int.MaxValue)
                    {
                        layerDepth -= 1;
                    }
                    else
                        layerDepth += 1;
                }
                else
                    layerDepth += 100;
            }

            string? baseName = null;
            int nameNum = 0;
            Regex trailingNumberRegex = new(@"\d+$", RegexOptions.Compiled);
            while (room.Layers.Any(l => l.LayerName.Content == name))
            {
                if (baseName is null)
                {
                    // Get the trailing number from the name ("name123" => "123")
                    System.Text.RegularExpressions.Match numMatch = trailingNumberRegex.Match(name);

                    // Name has a trailing number, so we parse the basename and number, increment the number and
                    // set the new name to the basename and incremented number ("name123" -> "name124")
                    if (numMatch.Success)
                    {
                        baseName = name[..^numMatch.Length];
                        nameNum = int.Parse(numMatch.Groups[0].Value) + 1;
                    }
                    // Name doesn't have a trailing number, so it's the first duplicate.
                    // Thus we set baseName and nameNum to produce "name1" on the next loop.
                    else
                    {
                        baseName = name;
                        nameNum = 1;
                    }
                }
                // If base name is already extracted, increment "nameNum" and append it to the base name
                else
                    nameNum++;
                // Update name using baseName and nameNum
                name = baseName + nameNum;
            }

            UndertaleRoom.Layer newLayer = new()
            {
                LayerName = ModLoader.Data.Strings.MakeString(name),
                LayerId = largest_layerid + 1,
                LayerType = type,
                LayerDepth = (int)layerDepth,
                Data = new T()
            };
            room.Layers.Add(newLayer);
            room.UpdateBGColorLayer();

            if (room.Layers.Count > 1)
            {
                LayerZIndexConverter.ProcessOnce = true;
                foreach (UndertaleRoom.Layer l in room.Layers)
                    l.UpdateZIndex();
            }
            newLayer.ParentRoom = room;

            Log.Information($"Successfully created layer {name} of type {type} in room {room.Name}");
            return newLayer;
        }
        public static UndertaleRoom.Layer AddLayer<T>(this UndertaleRoom room, UndertaleRoom.LayerType type, string name) where T : UndertaleRoom.Layer.LayerData, new()
        {
            return CreateLayer<T>(room, type, name);
        }
        public static UndertaleRoom.Layer AddLayerInstance(this UndertaleRoom room, string name)
        {
            return CreateLayer<UndertaleRoom.Layer.LayerInstancesData>(room, UndertaleRoom.LayerType.Instances, name);
        }
        public static UndertaleRoom.Layer AddLayerBackground(this UndertaleRoom room, string name)
        {
            return CreateLayer<UndertaleRoom.Layer.LayerBackgroundData>(room, UndertaleRoom.LayerType.Background, name);
        }
        public static UndertaleRoom.Layer GetLayer(this UndertaleRoom room, UndertaleRoom.LayerType type, string name)
        {
            try
            {
                UndertaleRoom.Layer layer = room.Layers.First(t => t.LayerName.Content == name && t.LayerType == type);
                Log.Information($"Found layer {name} of type {type}");
                return layer;
            }
            catch
            {
                throw;
            }
        }
        public static UndertaleRoom.GameObject AddGameObject(this UndertaleRoom room, string layerName, string obName)
        {
            try
            {
                UndertaleGameObject ob = GetObject(obName);
                UndertaleRoom.GameObject gameObject = new()
                {
                    InstanceID = ModLoader.Data.GeneralInfo.LastObj++,
                    ObjectDefinition = ob
                };
                room.GameObjects.Add(gameObject);
                room.GetLayer(UndertaleRoom.LayerType.Instances, layerName).InstancesData.Instances.Add(gameObject);

                Log.Information($"Successfully created gameobject {obName} in layer {layerName} in room {room.Name}");
                return gameObject;
            }
            catch
            {
                throw;
            }
        }
        public static UndertaleRoom.GameObject AddGameObject(this UndertaleRoom room, UndertaleRoom.Layer layer, string obName)
        {
            try
            {
                UndertaleGameObject ob = GetObject(obName);
                UndertaleRoom.GameObject gameObject = new()
                {
                    InstanceID = ModLoader.Data.GeneralInfo.LastObj++,
                    ObjectDefinition = ob
                };
                room.GameObjects.Add(gameObject);
                layer.InstancesData.Instances.Add(gameObject);

                Log.Information($"Successfully created gameobject {obName} in layer {layer.LayerName} in room {room.Name}");
                return gameObject;
            }
            catch
            {
                throw;
            }
        }
        public static UndertaleRoom.GameObject AddGameObject(this UndertaleRoom room, UndertaleRoom.Layer layer, string obName, UndertaleCode creationCode)
        {
            UndertaleRoom.GameObject gameObject = room.AddGameObject(layer, obName);
            gameObject.CreationCode = creationCode;
            return gameObject;
        }
        public static UndertaleRoom.GameObject AddGameObject(this UndertaleRoom room, UndertaleRoom.Layer layer, string obName, UndertaleCode creationCode, int x, int y)
        {
            UndertaleRoom.GameObject gameObject = room.AddGameObject(layer, obName);
            gameObject.CreationCode = creationCode;
            gameObject.X = x;
            gameObject.Y = y;
            return gameObject;
        }
        public static UndertaleRoom.GameObject GetGameObject(this UndertaleRoom room, string layerName, string obName)
        {
            try
            {
                UndertaleRoom.GameObject go = room.GetLayer(UndertaleRoom.LayerType.Instances, layerName).InstancesData.Instances.First(t => t.ObjectDefinition.Name.Content == obName);
                Log.Information($"Found GameObject {obName} in layer {layerName} in room {room.Name}");
                return go;
            }
            catch
            {
                throw;
            }
        }
        public static UndertaleRoom AddDisclaimerRoom(string modName, params string[] authorsName)
        {
            string modNameShort = Regex.Replace(modName, @"[\s\+-]+", "");
            UndertaleGameObject o_mod_disclaimer = AddObject(
                name: $"o_mod_disclaimer_{modNameShort}", 
                spriteName: "", 
                parentName: "", 
                isVisible: true, 
                isPersistent: false, 
                isAwake: true,
                collisionShapeFlags: CollisionShapeFlags.Circle
            );
            string disclaimerText = $@"scr_draw_text_doublecolor((global.cameraWidth / 2), (global.cameraHeight / 2), ""You are playing with {modName}. Please"", "" do not report bugs to main developers."", 16777215, make_color_rgb(155, 27, 49), 1, 1, 0.69999999999999996, global.f_digits, 0, 16777215, ""only report bugs to modding team."")";
            int delta = 55;
            foreach(string author in authorsName)
            {
                disclaimerText += $@"
scr_draw_text_doublecolor((global.cameraWidth / 2), ((global.cameraHeight / 2) + {delta}), ""By: {author}"", """", 16777215, make_color_rgb(155, 27, 49), 1, 1, 0.69999999999999996, global.f_digits, 0, 16777215)";
                delta += 30;
            }
            
            AddNewEvent(o_mod_disclaimer, disclaimerText, EventType.Draw, 0);
            
            UndertaleRoom room = AddRoom($"r_mod_disclaimer_{modNameShort}");

            UndertaleRoom.Layer layerInstance = room.AddLayerInstance("NewInstancesLayer");
            room.AddLayerBackground("NewBackgroundLayer");
            
            UndertaleRoom.GameObject overlay = room.AddGameObject(layerInstance, "o_init_overlay");
            room.AddGameObject(layerInstance, $"o_mod_disclaimer_{modNameShort}");

            ModLoader.AddDisclaimer(modNameShort, overlay);
            return room;
        }
        public static void AddCustomDisclaimerRoom(string modName, UndertaleRoom.GameObject overlay)
        {
            string modNameShort = Regex.Replace(modName, @"[\s\+-]+", "");
            ModLoader.AddDisclaimer(modNameShort, overlay);
        }
        internal static void ChainDisclaimerRooms(List<(string, UndertaleRoom.GameObject)> disclaimers)
        {
            if (disclaimers.Count == 0) return;

            string disclaimerCreationCode;
            int index = 0;
            string modNameShort = disclaimers[index].Item1;
            LoadGML("gml_RoomCC_r_disclaimer2_0_Create")
                .MatchFrom("roomNext")
                .ReplaceBy($"roomNext = r_mod_disclaimer_{modNameShort}")
                .Save();

            foreach((string, UndertaleRoom.GameObject) disclaimer in disclaimers)
            {
                if (index+1 < disclaimers.Count)
                {
                    modNameShort = disclaimers[index+1].Item1;
                    modNameShort = $"r_mod_disclaimer_{modNameShort}";
                }
                else
                {
                    modNameShort = "global.mainMenuRoom";
                }
                disclaimerCreationCode = $@"skippable = 0
roomNext = {modNameShort}
animationSpeed = 0.015
koeficient = 5";
                disclaimer.Item2.CreationCode = AddCode(disclaimerCreationCode, $"disclaimer_creation_{index++}");
            }
        }
    }
}