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
                Log.Error($"Cannot find room {name}.");
                throw;
            }
        }
        public static UndertaleRoom AddRoom(string name)
        {
            try
            {
                UndertaleRoom room = new()
                {
                    Name = ModLoader.Data.Strings.MakeString(name),
                    Caption = ModLoader.Data.Strings.MakeString(""),
                    Flags = UndertaleRoom.RoomEntryFlags.IsGMS2 ^ UndertaleRoom.RoomEntryFlags.EnableViews,
                };
                ModLoader.Data.Rooms.Add(room);
                Log.Information($"Successfully created room {name}.");
                return room;
            }
            catch
            {
                Log.Error($"Cannot add Room {name} since it already exists.");
                throw;
            }
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

            Log.Information($"Successfully created layer {name} of type {type} in room {room.Name}.");
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
                Log.Information($"Found layer {name} of type {type} in room {room.Name}.");
                return layer;
            }
            catch
            {
                Log.Error($"Cannot find layer {name} of type {type} in room {room.Name}.");
                throw;
            }
        }
        public static UndertaleRoom.GameObject AddGameObject(this UndertaleRoom room, string layerName, string obName, UndertaleCode? creationCode = null, int x = 0, int y = 0)
        {
            try
            {
                UndertaleGameObject ob = GetObject(obName);
                UndertaleRoom.GameObject gameObject = new()
                {
                    InstanceID = ModLoader.Data.GeneralInfo.LastObj++,
                    ObjectDefinition = ob,
                    X = x,
                    Y = y
                };
                if (creationCode != null) gameObject.CreationCode = creationCode;

                room.GameObjects.Add(gameObject);
                room.GetLayer(UndertaleRoom.LayerType.Instances, layerName).InstancesData.Instances.Add(gameObject);

                Log.Information($"Successfully created gameobject {obName} in layer {layerName} in room {room.Name}");
                return gameObject;
            }
            catch
            {
                Log.Error($"Cannot add the gameobject {obName} in layer {layerName} in room {room.Name}.");
                throw;
            }
        }
        public static UndertaleRoom.GameObject AddGameObject(this UndertaleRoom room, UndertaleRoom.Layer layer, string obName, UndertaleCode? creationCode = null, int x = 0, int y = 0)
        {
            try
            {
                UndertaleGameObject ob = GetObject(obName);
                UndertaleRoom.GameObject gameObject = new()
                {
                    InstanceID = ModLoader.Data.GeneralInfo.LastObj++,
                    ObjectDefinition = ob,
                    X = x,
                    Y = y
                };
                if (creationCode != null) gameObject.CreationCode = creationCode;

                room.GameObjects.Add(gameObject);
                layer.InstancesData.Instances.Add(gameObject);

                Log.Information($"Successfully created gameobject {obName} in layer {layer.LayerName} in room {room.Name}");
                return gameObject;
            }
            catch
            {
                Log.Error($"Cannot add the gameobject {obName} in layer {layer.LayerName} in room {room.Name}.");
                throw;
            }
        }
        public static UndertaleRoom.GameObject AddGameObject(this UndertaleRoom room, string layerName, UndertaleGameObject ob, UndertaleCode? creationCode = null, int x = 0, int y = 0)
        {
            try
            {
                UndertaleRoom.GameObject gameObject = new()
                {
                    InstanceID = ModLoader.Data.GeneralInfo.LastObj++,
                    ObjectDefinition = ob,
                    X = x,
                    Y = y
                };
                if (creationCode != null) gameObject.CreationCode = creationCode;

                room.GameObjects.Add(gameObject);
                room.GetLayer(UndertaleRoom.LayerType.Instances, layerName).InstancesData.Instances.Add(gameObject);

                Log.Information($"Successfully created gameobject {ob.Name} in layer {layerName} in room {room.Name}");
                return gameObject;
            }
            catch
            {
                Log.Error($"Cannot add the gameobject {ob.Name} in layer {layerName} in room {room.Name}.");
                throw;
            }
        }
        public static UndertaleRoom.GameObject AddGameObject(this UndertaleRoom room, UndertaleRoom.Layer layer, UndertaleGameObject ob, UndertaleCode? creationCode = null, int x = 0, int y = 0)
        {
            try
            {
                UndertaleRoom.GameObject gameObject = new()
                {
                    InstanceID = ModLoader.Data.GeneralInfo.LastObj++,
                    ObjectDefinition = ob,
                    X = x,
                    Y = y
                };
                if (creationCode != null) gameObject.CreationCode = creationCode;

                room.GameObjects.Add(gameObject);
                layer.InstancesData.Instances.Add(gameObject);

                Log.Information($"Successfully created gameobject {ob.Name} in layer {layer.LayerName} in room {room.Name}");
                return gameObject;
            }
            catch
            {
                Log.Error($"Cannot add the gameobject {ob.Name} in layer {layer.LayerName} in room {room.Name}.");
                throw;
            }
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
                Log.Error($"Cannot find instance of gameobject {obName} in layer {layerName} in room {room.Name}.");
                throw;
            }
        }
        internal static UndertaleRoom AddDisclaimerRoom(string[] modName, string[] authorsName)
        {
            UndertaleGameObject o_msl_mod_disclaimer = AddObject(
                name: "o_msl_mod_disclaimer", 
                spriteName: "", 
                parentName: "", 
                isVisible: true, 
                isPersistent: false, 
                isAwake: true,
                collisionShapeFlags: CollisionShapeFlags.Circle
            );

            string intro = $"You are playing with a modded version.";
            if (modName.Length > 0)
            {
                string names = string.Join(", ", modName);
                intro = $"You are playing with {names} mods.";
            }

            string disclaimerText = $@"scr_draw_text_doublecolor((global.cameraWidth / 2), (global.cameraHeight / 2), ""{intro}\nPlease do not report bugs to main developers."", """", 16777215, make_color_rgb(155, 27, 49), 1, 1, 0.69999999999999996, global.f_digits, 0, 16777215, "" Only report bugs to modding team."")";
            int delta = 55;
            foreach(string author in authorsName)
            {
                disclaimerText += $@"
scr_draw_text_doublecolor((global.cameraWidth / 2), ((global.cameraHeight / 2) + {delta}), ""By: {author}"", """", 16777215, make_color_rgb(155, 27, 49), 1, 1, 0.69999999999999996, global.f_digits, 0, 16777215)";
                delta += 30;
            }
            
            AddNewEvent(o_msl_mod_disclaimer, disclaimerText, EventType.Draw, 0);
            
            UndertaleRoom room = AddRoom("r_msl_mod_disclaimer");

            //room.AddLayerBackground("NewBackgroundLayer");
            UndertaleRoom.Layer layerInstance = room.AddLayerInstance("NewInstancesLayer");
            
            
            UndertaleRoom.GameObject overlay = room.AddGameObject(layerInstance, "o_init_overlay");
            room.AddGameObject(layerInstance, $"o_msl_mod_disclaimer");

            ModLoader.AddDisclaimer("r_msl_mod_disclaimer", overlay);
            return room;
        }
        public static void AddCreditDisclaimerRoom(string modName, params string[] authorsName)
        {
            ModLoader.AddCredit(modName, authorsName);
        }
        public static void AddCustomDisclaimerRoom(string roomName, UndertaleRoom.GameObject overlay)
        {
            ModLoader.AddDisclaimer(roomName, overlay);
        }
        internal static void ChainDisclaimerRooms(List<(string, UndertaleRoom.GameObject)> disclaimers)
        {
            if (disclaimers.Count == 0) return;

            string disclaimerCreationCode;
            int index = 0;
            string roomName = disclaimers[index].Item1;
            LoadGML("gml_RoomCC_r_disclaimer2_0_Create")
                .MatchFrom("roomNext")
                .ReplaceBy($"roomNext = {roomName}")
                .Save();

            foreach((string, UndertaleRoom.GameObject) disclaimer in disclaimers)
            {
                if (index+1 < disclaimers.Count && index < 3)
                {
                    roomName = disclaimers[index+1].Item1;
                }
                else
                {
                    roomName = "global.mainMenuRoom";
                }
                disclaimerCreationCode = $@"skippable = 0
roomNext = {roomName}
animationSpeed = 0.015
koeficient = 5";
                disclaimer.Item2.CreationCode = AddCode(disclaimerCreationCode, $"disclaimer_creation_{index++}");
                if (index > 3)
                {
                    break;
                }
            }
        }
    }
}