using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModTool;
using System.Text;
using System.Text.Json;
using UndertaleModLib.Scripting;
using System.Xml.Linq;
using System;


namespace ModShardLauncher
{
    public static partial class Msl
    {
        public static IEnumerable<UndertaleRoom> GetRooms()
        {
            return ModLoader.Data.Rooms;
        }

        public static void AddRoomJson(string jsonString)
        {
            UndertaleRoom newRoom = new UndertaleRoom();

            Log.Information($"A room is being added or edited");
            byte[] jsonUtf8Bytes = Encoding.UTF8.GetBytes(jsonString);

            JsonReaderOptions options = new JsonReaderOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };

            Utf8JsonReader reader = new Utf8JsonReader(jsonUtf8Bytes, options);

            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartObject);

            ReadName(ref reader, ref newRoom);
            ReadMainValues(ref reader, ref newRoom);

            ClearRoomLists(ref newRoom);

            ReadBackgrounds(ref reader, ref newRoom);
            ReadViews(ref reader, ref newRoom);
            ReadGameObjects(ref reader, ref newRoom);
            ReadTiles(ref reader, ref newRoom);
            ReadLayers(ref reader, ref newRoom);

            if (ModLoader.Data.Rooms.ByName(newRoom.Name.Content) == null)
                ModLoader.Data.Rooms.Add(newRoom);

            Log.Information($"Successfully created room {newRoom.Name}.");
        }

        #region AddRoomJson
        static void ReadMainValues(ref Utf8JsonReader reader,ref UndertaleRoom newRoom)
        {
            string caption = ReadString(ref reader);

            newRoom.Width = (uint)ReadNum(ref reader);
            newRoom.Height = (uint)ReadNum(ref reader);
            newRoom.Speed = (uint)ReadNum(ref reader);
            newRoom.Persistent = ReadBool(ref reader);
            newRoom.BackgroundColor = (uint)(0xFF000000 | ReadNum(ref reader)); // make alpha 255 (BG color doesn't have alpha)
            newRoom.DrawBackgroundColor = ReadBool(ref reader);

            string ccIdName = ReadString(ref reader);

            newRoom.Flags = (UndertaleRoom.RoomEntryFlags)ReadNum(ref reader);
            newRoom.World = ReadBool(ref reader);
            newRoom.Top = (uint)ReadNum(ref reader);
            newRoom.Left = (uint)ReadNum(ref reader);
            newRoom.Right = (uint)ReadNum(ref reader);
            newRoom.Bottom = (uint)ReadNum(ref reader);
            newRoom.GravityX = ReadFloat(ref reader);
            newRoom.GravityY = ReadFloat(ref reader);
            newRoom.MetersPerPixel = ReadFloat(ref reader);

            newRoom.Caption = (caption == null) ? null : new UndertaleString(caption);

            var captionToAdd = newRoom.Caption;

            if ((newRoom.Caption != null) && !ModLoader.Data.Strings.Any(s => s == captionToAdd))
                ModLoader.Data.Strings.Add(newRoom.Caption);

            newRoom.CreationCodeId = (ccIdName == null) ? null : ModLoader.Data.Code.ByName(ccIdName);
        }

        static void ReadName(ref Utf8JsonReader reader, ref UndertaleRoom newRoom)
        {
            string name = ReadString(ref reader);
            if (name == null)
                throw new ScriptException("ERROR: Object name was null - object name must be defined!");

            if (ModLoader.Data.Rooms.ByName(name) != null)
            {
                newRoom = ModLoader.Data.Rooms.ByName(name);
            }
            else
            {
                newRoom = new UndertaleRoom();
                newRoom.Name = new UndertaleString(name);
                ModLoader.Data.Strings.Add(newRoom.Name);
            }
        }

        static void ClearRoomLists(ref UndertaleRoom newRoom)
        {
            newRoom.Backgrounds.Clear();
            newRoom.Views.Clear();
            newRoom.GameObjects.Clear();
            newRoom.Tiles.Clear();
            newRoom.Layers.Clear();
        }

        static void ReadBackgrounds(ref Utf8JsonReader reader,ref UndertaleRoom newRoom)
        {
            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartArray);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    UndertaleRoom.Background newBg = new UndertaleRoom.Background();

                    newBg.ParentRoom = newRoom;

                    newBg.Enabled = ReadBool(ref reader);
                    newBg.Foreground = ReadBool(ref reader);
                    string bgDefName = ReadString(ref reader);
                    newBg.X = (int)ReadNum(ref reader);
                    newBg.Y = (int)ReadNum(ref reader);
                    newBg.TiledHorizontally = ReadBool(ref reader);
                    newBg.TiledVertically = ReadBool(ref reader);
                    newBg.SpeedX = (int)ReadNum(ref reader);
                    newBg.SpeedY = (int)ReadNum(ref reader);
                    newBg.Stretch = ReadBool(ref reader);

                    newBg.BackgroundDefinition = (bgDefName == null) ? null : ModLoader.Data.Backgrounds.ByName(bgDefName);

                    ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);

                    newRoom.Backgrounds.Add(newBg);
                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                throw new ScriptException($"ERROR: Unexpected token type. Expected Integer - found {reader.TokenType}");
            }
        }

        static void ReadViews(ref Utf8JsonReader reader,ref UndertaleRoom newRoom)
        {
            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartArray);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    UndertaleRoom.View newView = new UndertaleRoom.View();

                    newView.Enabled = ReadBool(ref reader);
                    newView.ViewX = (int)ReadNum(ref reader);
                    newView.ViewY = (int)ReadNum(ref reader);
                    newView.ViewWidth = (int)ReadNum(ref reader);
                    newView.ViewHeight = (int)ReadNum(ref reader);
                    newView.PortX = (int)ReadNum(ref reader);
                    newView.PortY = (int)ReadNum(ref reader);
                    newView.PortWidth = (int)ReadNum(ref reader);
                    newView.PortHeight = (int)ReadNum(ref reader);
                    newView.BorderX = (uint)ReadNum(ref reader);
                    newView.BorderY = (uint)ReadNum(ref reader);
                    newView.SpeedX = (int)ReadNum(ref reader);
                    newView.SpeedY = (int)ReadNum(ref reader);
                    string objIdName = ReadString(ref reader);

                    newView.ObjectId = (objIdName == null) ? null : ModLoader.Data.GameObjects.ByName(objIdName);

                    ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);

                    newRoom.Views.Add(newView);
                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                throw new ScriptException($"ERROR: Unexpected token type. Expected Integer - found {reader.TokenType}");
            }
        }

        static void ReadGameObjects(ref Utf8JsonReader reader,ref UndertaleRoom newRoom)
        {
            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartArray);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    UndertaleRoom.GameObject newObj = new UndertaleRoom.GameObject();

                    newObj.X = (int)ReadNum(ref reader);
                    newObj.Y = (int)ReadNum(ref reader);

                    string objDefName = ReadString(ref reader);

                    uint ids = (uint)ReadNum(ref reader);
                    newObj.InstanceID = ++ModLoader.Data.GeneralInfo.LastObj;

                    string ccIdName = ReadString(ref reader);

                    newObj.ScaleX = ReadFloat(ref reader);
                    newObj.ScaleY = ReadFloat(ref reader);
                    newObj.Color = (uint)ReadNum(ref reader);
                    newObj.Rotation = ReadFloat(ref reader);

                    string preCcIdName = ReadString(ref reader);

                    newObj.ImageSpeed = ReadFloat(ref reader);
                    newObj.ImageIndex = (int)ReadNum(ref reader);

                    newObj.ObjectDefinition = (objDefName == null) ? null : ModLoader.Data.GameObjects.ByName(objDefName);
                    newObj.CreationCode = (ccIdName == null) ? null : ModLoader.Data.Code.ByName(ccIdName);
                    newObj.PreCreateCode = (preCcIdName == null) ? null : ModLoader.Data.Code.ByName(preCcIdName);
                    ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);
                    //remove empty instances
                    if (!ModLoader.Data.IsGameMaker2() && newObj.ObjectDefinition != null && ModLoader.Data.GameObjects.ByName(objDefName) != null)
                        newRoom.GameObjects.Add(newObj);
                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                throw new ScriptException($"ERROR: Unexpected token type. Expected Integer - found {reader.TokenType}");
            }
        }

        static void ReadTiles(ref Utf8JsonReader reader,ref UndertaleRoom newRoom)
        {
            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartArray);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    UndertaleRoom.Tile newTile = new UndertaleRoom.Tile();

                    newTile.spriteMode = ReadBool(ref reader);
                    newTile.X = (int)ReadNum(ref reader);
                    newTile.Y = (int)ReadNum(ref reader);

                    string bgDefName = ReadString(ref reader);
                    string sprDefName = ReadString(ref reader);

                    newTile.SourceX = (uint)ReadNum(ref reader);
                    newTile.SourceY = (uint)ReadNum(ref reader);
                    newTile.Width = (uint)ReadNum(ref reader);
                    newTile.Height = (uint)ReadNum(ref reader);
                    newTile.TileDepth = (int)ReadNum(ref reader);
                    newTile.InstanceID = (uint)ReadNum(ref reader);
                    newTile.ScaleX = ReadFloat(ref reader);
                    newTile.ScaleY = ReadFloat(ref reader);
                    newTile.Color = (uint)ReadNum(ref reader);

                    newTile.BackgroundDefinition = (bgDefName == null) ? null : ModLoader.Data.Backgrounds.ByName(bgDefName);
                    newTile.SpriteDefinition = (sprDefName == null) ? null : ModLoader.Data.Sprites.ByName(sprDefName);

                    ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);

                    newRoom.Tiles.Add(newTile);
                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                throw new ScriptException($"ERROR: Unexpected token type. Expected Integer - found {reader.TokenType}");
            }
        }

        static void ReadLayers(ref Utf8JsonReader reader,ref UndertaleRoom newRoom)
        {
            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartArray);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    UndertaleRoom.Layer newLayer = new UndertaleRoom.Layer();

                    string layerName = ReadString(ref reader);

                    newLayer.LayerId = (uint)ReadNum(ref reader);
                    newLayer.LayerType = (UndertaleRoom.LayerType)ReadNum(ref reader);
                    newLayer.LayerDepth = (int)ReadNum(ref reader);
                    newLayer.XOffset = ReadFloat(ref reader);
                    newLayer.YOffset = ReadFloat(ref reader);
                    newLayer.HSpeed = ReadFloat(ref reader);
                    newLayer.VSpeed = ReadFloat(ref reader);
                    newLayer.IsVisible = ReadBool(ref reader);


                    newLayer.LayerName = (layerName == null) ? null : new UndertaleString(layerName);

                    if ((layerName != null) && !ModLoader.Data.Strings.Any(s => s == newLayer.LayerName))
                        ModLoader.Data.Strings.Add(newLayer.LayerName);

                    switch (newLayer.LayerType)
                    {
                        case UndertaleRoom.LayerType.Background:
                            ReadBackgroundLayer(ref reader, newLayer,ref newRoom);
                            break;
                        case UndertaleRoom.LayerType.Instances:
                            ReadInstancesLayer(ref reader, newLayer,ref newRoom);
                            break;
                        case UndertaleRoom.LayerType.Assets:
                            ReadAssetsLayer(ref reader, newLayer, ref newRoom);
                            break;
                        case UndertaleRoom.LayerType.Tiles:
                            ReadTilesLayer(ref reader, newLayer,ref newRoom);
                            break;
                        default:
                            throw new ScriptException("ERROR: Invalid value for layer data type.");
                    }

                    ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);

                    newRoom.Layers.Add(newLayer);
                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                throw new ScriptException($"ERROR: Unexpected token type. Expected Integer - found {reader.TokenType}");
            }
        }

        static void ReadBackgroundLayer(ref Utf8JsonReader reader, UndertaleRoom.Layer newLayer,ref UndertaleRoom newRoom)
        {
            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartObject);
            //Layer needs to have a Parent Room otherwise a sprite update will cause a null refrence.
            newLayer.ParentRoom = newRoom;
            UndertaleRoom.Layer.LayerBackgroundData newLayerData = new UndertaleRoom.Layer.LayerBackgroundData();
            newLayerData.Visible = ReadBool(ref reader);
            newLayerData.Foreground = ReadBool(ref reader);
            string sprite = ReadString(ref reader);
            newLayerData.TiledHorizontally = ReadBool(ref reader);
            newLayerData.TiledVertically = ReadBool(ref reader);
            newLayerData.Stretch = ReadBool(ref reader);
            newLayerData.Color = (uint)ReadNum(ref reader);
            newLayerData.FirstFrame = ReadFloat(ref reader);
            newLayerData.AnimationSpeed = ReadFloat(ref reader);
            newLayerData.AnimationSpeedType = (AnimationSpeedType)ReadNum(ref reader);
            newLayerData.Sprite = null;
            newLayerData.ParentLayer = newLayer;
            UndertaleSprite bgsprite = ModLoader.Data.Sprites.ByName(sprite);
            if (bgsprite is not null)
                newLayerData.Sprite = bgsprite;
            ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);
            newLayer.Data = newLayerData;

        }

        static void ReadInstancesLayer(ref Utf8JsonReader reader, UndertaleRoom.Layer newLayer,ref UndertaleRoom newRoom)
        {
            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartObject);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                    continue;

                if (reader.TokenType != JsonTokenType.StartArray)
                    throw new ScriptException("ERROR: Did not correctly stop reading instances layer");

                UndertaleRoom.Layer.LayerInstancesData newLayerData = new UndertaleRoom.Layer.LayerInstancesData();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.PropertyName)
                        continue;

                    if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        UndertaleRoom.GameObject newObj = new UndertaleRoom.GameObject();

                        newObj.X = (int)ReadNum(ref reader);
                        newObj.Y = (int)ReadNum(ref reader);

                        string objDefName = ReadString(ref reader);

                        uint ids = (uint)ReadNum(ref reader);
                        newObj.InstanceID = ++ModLoader.Data.GeneralInfo.LastObj;

                        string ccIdName = ReadString(ref reader);

                        newObj.ScaleX = ReadFloat(ref reader);
                        newObj.ScaleY = ReadFloat(ref reader);
                        newObj.Color = (uint)ReadNum(ref reader);
                        newObj.Rotation = ReadFloat(ref reader);

                        string preCcIdName = ReadString(ref reader);

                        newObj.ImageSpeed = ReadFloat(ref reader);
                        newObj.ImageIndex = (int)ReadNum(ref reader);

                        newObj.ObjectDefinition = (objDefName == null) ? null : ModLoader.Data.GameObjects.ByName(objDefName);

                        newObj.CreationCode = (ccIdName == null) ? null : ModLoader.Data.Code.ByName(ccIdName);

                        newObj.PreCreateCode = (preCcIdName == null) ? null : ModLoader.Data.Code.ByName(preCcIdName);

                        ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);
                        //get rid of those nasty empty instances
                        if (newObj.ObjectDefinition != null && ModLoader.Data.GameObjects.ByName(objDefName) != null)
                        {
                            newLayerData.Instances.Add(newObj);
                            newRoom.GameObjects.Add(newObj);
                        }
                        continue;
                    }

                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;

                    throw new ScriptException("ERROR: Did not correctly stop reading instances in instance layer");
                }

                ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);

                newLayer.Data = newLayerData;

                return;

            }
        }

        static void ReadAssetsLayer(ref Utf8JsonReader reader, UndertaleRoom.Layer newLayer,ref UndertaleRoom newRoom)
        {
            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartObject);
            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartArray);
            UndertaleRoom.Layer.LayerAssetsData newLayerData = new UndertaleRoom.Layer.LayerAssetsData();

            newLayerData.LegacyTiles = new UndertalePointerList<UndertaleRoom.Tile>();
            newLayerData.Sprites = new UndertalePointerList<UndertaleRoom.SpriteInstance>();
            newLayerData.Sequences = new UndertalePointerList<UndertaleRoom.SequenceInstance>();
            newLayerData.NineSlices = new UndertalePointerList<UndertaleRoom.SpriteInstance>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    UndertaleRoom.Tile newTile = new UndertaleRoom.Tile();

                    newTile.spriteMode = ReadBool(ref reader);
                    newTile.X = (int)ReadNum(ref reader);
                    newTile.Y = (int)ReadNum(ref reader);

                    string bgDefName = ReadString(ref reader);
                    string sprDefName = ReadString(ref reader);

                    newTile.SourceX = (uint)ReadNum(ref reader);
                    newTile.SourceY = (uint)ReadNum(ref reader);
                    newTile.Width = (uint)ReadNum(ref reader);
                    newTile.Height = (uint)ReadNum(ref reader);
                    newTile.TileDepth = (int)ReadNum(ref reader);
                    newTile.InstanceID = (uint)ReadNum(ref reader);
                    newTile.ScaleX = ReadFloat(ref reader);
                    newTile.ScaleY = ReadFloat(ref reader);
                    newTile.Color = (uint)ReadNum(ref reader);

                    newTile.BackgroundDefinition = (bgDefName == null) ? null : ModLoader.Data.Backgrounds.ByName(bgDefName);

                    newTile.SpriteDefinition = (sprDefName == null) ? null : ModLoader.Data.Sprites.ByName(sprDefName);

                    ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);

                    newLayerData.LegacyTiles.Add(newTile);
                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                throw new ScriptException($"ERROR: Unexpected token type. Expected Integer - found {reader.TokenType}");
            }

            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartArray);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                    continue;

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    UndertaleRoom.SpriteInstance newSpr = new UndertaleRoom.SpriteInstance();

                    string name = ReadString(ref reader);
                    string spriteName = ReadString(ref reader);

                    newSpr.X = (int)ReadNum(ref reader);
                    newSpr.Y = (int)ReadNum(ref reader);
                    newSpr.ScaleX = ReadFloat(ref reader);
                    newSpr.ScaleY = ReadFloat(ref reader);
                    newSpr.Color = (uint)ReadNum(ref reader);
                    newSpr.AnimationSpeed = ReadFloat(ref reader);
                    newSpr.AnimationSpeedType = (AnimationSpeedType)ReadNum(ref reader);
                    newSpr.FrameIndex = ReadFloat(ref reader);
                    newSpr.Rotation = ReadFloat(ref reader);

                    newSpr.Name = (name == null) ? null : new UndertaleString(name);

                    if ((name != null) && !ModLoader.Data.Strings.Any(s => s == newSpr.Name))
                        ModLoader.Data.Strings.Add(newSpr.Name);

                    newSpr.Sprite = (spriteName == null) ? null : ModLoader.Data.Sprites.ByName(spriteName);

                    ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);

                    newLayerData.Sprites.Add(newSpr);
                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                throw new ScriptException("ERROR: Did not correctly stop reading instances in instance layer");
            }

            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartArray);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                    continue;

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    UndertaleRoom.SequenceInstance newSeq = new UndertaleRoom.SequenceInstance();

                    string name = ReadString(ref reader);
                    string sequenceName = ReadString(ref reader);

                    newSeq.X = (int)ReadNum(ref reader);
                    newSeq.Y = (int)ReadNum(ref reader);
                    newSeq.ScaleX = ReadFloat(ref reader);
                    newSeq.ScaleY = ReadFloat(ref reader);
                    newSeq.Color = (uint)ReadNum(ref reader);
                    newSeq.AnimationSpeed = ReadFloat(ref reader);
                    newSeq.AnimationSpeedType = (AnimationSpeedType)ReadNum(ref reader);
                    newSeq.FrameIndex = ReadFloat(ref reader);
                    newSeq.Rotation = ReadFloat(ref reader);


                    newSeq.Name = (name == null) ? null : new UndertaleString(name);

                    if ((name != null) && !ModLoader.Data.Strings.Any(s => s == newSeq.Name))
                        ModLoader.Data.Strings.Add(newSeq.Name);

                    newSeq.Sequence = (sequenceName == null) ? null : ModLoader.Data.Sequences.ByName(sequenceName);

                    ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);

                    newLayerData.Sequences.Add(newSeq);
                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                throw new ScriptException("ERROR: Did not correctly stop reading instances in instance layer");
            }

            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartArray);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                    continue;

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    UndertaleRoom.SpriteInstance newSpr = new UndertaleRoom.SpriteInstance();

                    string name = ReadString(ref reader);
                    string spriteName = ReadString(ref reader);

                    newSpr.X = (int)ReadNum(ref reader);
                    newSpr.Y = (int)ReadNum(ref reader);
                    newSpr.ScaleX = ReadFloat(ref reader);
                    newSpr.ScaleY = ReadFloat(ref reader);
                    newSpr.Color = (uint)ReadNum(ref reader);
                    newSpr.AnimationSpeed = ReadFloat(ref reader);
                    newSpr.AnimationSpeedType = (AnimationSpeedType)ReadNum(ref reader);
                    newSpr.FrameIndex = ReadFloat(ref reader);
                    newSpr.Rotation = ReadFloat(ref reader);

                    newSpr.Name = (name == null) ? null : new UndertaleString(name);

                    if ((name != null) && !ModLoader.Data.Strings.Any(s => s == newSpr.Name))
                        ModLoader.Data.Strings.Add(newSpr.Name);

                    newSpr.Sprite = spriteName == null ? null : ModLoader.Data.Sprites.ByName(spriteName);

                    ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);

                    newLayerData.NineSlices.Add(newSpr);
                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                throw new ScriptException("ERROR: Did not correctly stop reading instances in instance layer");
            }

            newLayer.Data = newLayerData;
            ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);
        }

        static void ReadTilesLayer(ref Utf8JsonReader reader, UndertaleRoom.Layer newLayer,ref UndertaleRoom newRoom)
        {
            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartObject);
            UndertaleRoom.Layer.LayerTilesData newLayerData = new UndertaleRoom.Layer.LayerTilesData();

            string backgroundName = ReadString(ref reader);

            newLayerData.TilesX = (uint)ReadNum(ref reader);
            newLayerData.TilesY = (uint)ReadNum(ref reader);

            newLayerData.Background = (backgroundName == null) ? null : ModLoader.Data.Backgrounds.ByName(backgroundName);

            uint[][] tileIds = new uint[newLayerData.TilesY][];
            for (int i = 0; i < newLayerData.TilesY; i++)
            {
                tileIds[i] = new uint[newLayerData.TilesX];
            }

            ReadAnticipateJSONObject(ref reader, JsonTokenType.StartArray);
            for (int y = 0; y < newLayerData.TilesY; y++)
            {
                ReadAnticipateJSONObject(ref reader, JsonTokenType.StartArray);
                for (int x = 0; x < newLayerData.TilesX; x++)
                {
                    ReadAnticipateJSONObject(ref reader, JsonTokenType.StartObject);
                    (tileIds[y])[x] = (uint)ReadNum(ref reader);
                    ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);
                }

                ReadAnticipateJSONObject(ref reader, JsonTokenType.EndArray);
            }

            newLayerData.TileData = tileIds;
            ReadAnticipateJSONObject(ref reader, JsonTokenType.EndArray);
            ReadAnticipateJSONObject(ref reader, JsonTokenType.EndObject);

            newLayer.Data = newLayerData;
        }

        // Read tokens of specified type

        static bool ReadBool(ref Utf8JsonReader reader)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName: continue;
                    case JsonTokenType.True: return true;
                    case JsonTokenType.False: return false;
                    default: throw new ScriptException($"ERROR: Unexpected token type. Expected Boolean - found {reader.TokenType}");
                }
            }

            throw new ScriptException("ERROR: Did not find value of expected type. Expected Boolean.");
        }

        static long ReadNum(ref Utf8JsonReader reader)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName: continue;
                    case JsonTokenType.Number: return reader.GetInt64();
                    default: throw new ScriptException($"ERROR: Unexpected token type. Expected Integer - found {reader.TokenType}");
                }
            }

            throw new ScriptException("ERROR: Did not find value of expected type. Expected Integer.");
        }

       static float ReadFloat(ref Utf8JsonReader reader)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName: continue;
                    case JsonTokenType.Number: return reader.GetSingle();
                    default: throw new ScriptException($"ERROR: Unexpected token type. Expected Decimal - found {reader.TokenType}");
                }
            }

            throw new ScriptException("ERROR: Did not find value of expected type. Expected Decimal.");
        }

        static string ReadString(ref Utf8JsonReader reader)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName: continue;
                    case JsonTokenType.String: return reader.GetString();
                    case JsonTokenType.Null: return null;
                    default: throw new ScriptException($"ERROR: Unexpected token type. Expected String - found {reader.TokenType}");
                }
            }

            throw new ScriptException("ERROR: Did not find value of expected type. Expected String.");
        }

        static void ReadAnticipateJSONObject(ref Utf8JsonReader reader, JsonTokenType allowedTokenType)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                    continue;
                if (reader.TokenType == allowedTokenType)
                    return;
                throw new ScriptException($"ERROR: Unexpected token type. Expected {allowedTokenType} - found {reader.TokenType}");
            }

            throw new ScriptException("ERROR: Did not find value of expected type. Expected String.");
        }

        #endregion


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
            foreach (string author in authorsName)
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

            foreach ((string, UndertaleRoom.GameObject) disclaimer in disclaimers)
            {
                if (index + 1 < disclaimers.Count && index < 3)
                {
                    roomName = disclaimers[index + 1].Item1;
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