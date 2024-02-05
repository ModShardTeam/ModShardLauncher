using System;
using System.Linq;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public static partial class Msl
    {
        public static UndertaleGameObject AddObject(string name)
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

                // doesnt exist so it can be added
                UndertaleGameObject obj = new()
                {
                    Name = ModLoader.Data.Strings.MakeString(name)
                };
                ModLoader.Data.GameObjects.Add(obj);
                Log.Information(string.Format("Successfully created gameObject: {0}", name.ToString()));
                return obj;
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
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