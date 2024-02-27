using System;
using System.Linq;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public static class GameObjectUtils
    {
        /// <summary>
        /// Extension method to apply several <see cref="MslEvent"/> to a <see cref="gameObject"/> simultaneously.
        /// <example>For example:
        /// <code>
        /// gameObject.ApplyEvent(ModFiles, 
        ///     new MslEvent(fileName0, EventType.Create, 0),
        ///     new MslEvent(fileName1, EventType.Other, 10),
        ///     new MslEvent(fileName2, EventType.Other, 11)
        /// );
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="modFile"></param>
        /// <param name="mslEvents"></param>
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
        public static UndertaleGameObject AddObject(string name)
        {
            return AddObject(
                name, 
                spriteName:"", 
                parentName: "", 
                isVisible: false, 
                isPersistent: false, 
                isAwake: false, 
                collisionShapeFlags: CollisionShapeFlags.Circle);
        }
        /// <summary>
        /// Add and return a new <see cref="UndertaleGameObject"/> named <paramref name="name"/> to the data.win if this name is not used already.
        /// Else return the existing <see cref="UndertaleGameObject"/>.
        /// A lot of parametrization is possible when creating this <see cref="UndertaleGameObject"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="spriteName"></param>
        /// <param name="parentName"></param>
        /// <param name="isVisible"></param>
        /// <param name="isPersistent"></param>
        /// <param name="isAwake"></param>
        /// <param name="collisionShapeFlags"></param>
        /// <returns>
        /// </returns>
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
                UndertaleSprite? sprite = null;
                if (spriteName != "") sprite = GetSprite(spriteName);
                UndertaleGameObject? parent = null;
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
        /// <summary>
        /// Return the <see cref="UndertaleGameObject"/> named <paramref name="name"/> if it exists. Else raise an exception.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static UndertaleGameObject GetObject(string name)
        {
            try
            {
                UndertaleGameObject gameObject = ModLoader.Data.GameObjects.First(t => t.Name.Content == name);
                Log.Information(string.Format("Found gameObject: {0}", name.ToString()));
                return gameObject;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Replace the <see cref="UndertaleGameObject"/> named <paramref name="name"/> by <paramref name="o"/>. 
        /// Raise an exception if the <see cref="UndertaleGameObject"/> named <paramref name="name"/> does not exist.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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