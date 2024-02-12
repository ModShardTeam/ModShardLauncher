using System;
using System.Linq;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public static class GameObjectUtils
    {
        static public void ApplyEvent(this UndertaleGameObject gameObject, ModFile modFile, params MslEvent[] mslEvents)
        {
            foreach (MslEvent mslEvent in mslEvents)
            {
                mslEvent.Apply(gameObject, modFile);
            }
        }
    }
    public static partial class Msl
    {
        public static UndertaleGameObject AddObject(
            string name, 
            string spriteName = "",
            string parentName = "", 
            bool isVisible = false, 
            bool isPersistent = false, 
            bool isAwake = false, 
            CollisionShapeFlags collisionShapeFlags = CollisionShapeFlags.Circle)
        {
            try 
            {
                // check if the object exists already
                UndertaleGameObject? existingObj = ModLoader.Data.GameObjects.FirstOrDefault(t => t.Name.Content == name);
                if(existingObj != null)
                {
                    Log.Information(string.Format("Cannot create the GameObject since it already exists: {0}", name.ToString()));
                    return existingObj;
                }

                // retrieve possible parent and sprite
                UndertaleSprite sprite = new();
                if (spriteName != "") sprite = GetSprite(spriteName);
                UndertaleGameObject parent = new();
                if (parentName != "") parent = GetObject(parentName);

                // doesnt exist so it can be added
                UndertaleGameObject obj = new()
                {
                    Name = ModLoader.Data.Strings.MakeString(name),
                    Sprite = sprite,
                    ParentId = parent,
                    Visible = isVisible,
                    Persistent = isPersistent,
                    CollisionShape = collisionShapeFlags,
                    Awake = isAwake,
                };
                ModLoader.Data.GameObjects.Add(obj);
                Log.Information(string.Format("Successfully created gameObject: {0}", name.ToString()));
                return obj;
            }
            catch 
            {
                throw;
            }
        }
        public static UndertaleGameObject GetObject(string name)
        {
            try
            {
                UndertaleGameObject gameObject = ModLoader.Data.GameObjects.First(t => t.Name.Content == name);
                Log.Information(string.Format("Found gameObject: {0}", name.ToString()));
                return gameObject;
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static void SetObject(string name, UndertaleGameObject o)
        {
            try
            {
                (int indexObj, _) = ModLoader.Data.GameObjects.Enumerate().First(t => t.Item2.Name.Content == name);
                ModLoader.Data.GameObjects[indexObj] = o;
                Log.Information(string.Format("Successfully replaced gameObject: {0}", name.ToString()));
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
    }
}