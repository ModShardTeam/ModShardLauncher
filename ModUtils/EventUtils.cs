using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Models;
using static UndertaleModLib.Models.UndertaleGameObject;

namespace ModShardLauncher
{
    public static partial class Msl
    {
        public static string EventName(string objectName, EventType eventType, uint subtype)
        {
            return "gml_Object_" + objectName + "_" + eventType + "_" + subtype;
        }
        public static void AddNewEvent(string objectName, string eventCode, EventType eventType, uint? nullOrSubtype)
        {
            try
            {
                uint subtype = CheckSubEvent(eventType, nullOrSubtype);
                // find the object
                UndertaleGameObject gameObject = GetObject(objectName);
                // check if the subEvent already exists
                Event? subtypeObj = gameObject.Events[(int)eventType].FirstOrDefault(x => x.EventSubtype == subtype);
                if (subtypeObj != null)
                {
                    throw new ArgumentException(string.Format("Cannot add the event {0}_{1} in {2} since it already exists", eventType, subtype, objectName));
                }

                // create a new code
                string newEventName = EventName(objectName, eventType, subtype);
                AddCode(eventCode, newEventName);
                // add the previous code to the event
                Event newEvent = new() { EventSubtype = subtype };
                newEvent.Actions.Add(new EventAction()
                {
                    CodeId = GetUMTCodeFromFile(newEventName),
                });
                
                gameObject.Events[(int)eventType].Add(newEvent);
                Log.Information(string.Format("Successfully added event {{{0}_{1}}} in object {{{2}}}", eventType, subtype, objectName));
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        private static uint CheckSubEvent(EventType eventType, uint? nullOrSubtype)
        { 
            uint subType = 0;

            switch(eventType)
            {
                case EventType.Create:
                case EventType.Destroy:
                case EventType.Trigger:
                    // test if null or default
                    // default is zero for uint
                    if (!EqualityComparer<uint>.Default.Equals(nullOrSubtype))
                    {
                        throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Null or zero subtype expected for {0}", eventType));
                    }
                break;
                
                case EventType.Alarm:
                    if(nullOrSubtype == null) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Null subtype found for {0}", eventType));
                    if(nullOrSubtype > 11) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subType));
                    subType = (uint)nullOrSubtype;
                break;
                
                case EventType.Step:
                    if(nullOrSubtype == null) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Null subtype found for {0}", eventType));
                    if(nullOrSubtype > 2) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subType));
                    subType = (uint)nullOrSubtype;
                break;

                case EventType.Collision:
                    if(nullOrSubtype == null) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Null subtype found for {0}", eventType));
                    if(nullOrSubtype >= ModLoader.Data.GameObjects.Count) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subType));
                    if(ModLoader.Data.GameObjects[(int)nullOrSubtype] == null) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Invalid subtype {1} found for {0}, gameObject associated is null", eventType, subType));
                    subType = (uint)nullOrSubtype;
                break;

                case EventType.Keyboard:
                case EventType.KeyPress:
                case EventType.KeyRelease:
                    if(nullOrSubtype == null) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Null subtype found for {0}", eventType));
                    if(Enum.IsDefined(typeof(EventSubtypeKey), (uint)nullOrSubtype)) 
                    {
                        throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subType));
                    }
                    subType = (uint)nullOrSubtype;
                break;

                case EventType.Mouse:
                    if(nullOrSubtype == null) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Null subtype found for {0}", eventType));
                    if(Enum.IsDefined(typeof(EventSubtypeMouse), (uint)nullOrSubtype)) 
                    {
                        throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subType));
                    }
                    subType = (uint)nullOrSubtype;
                break;

                case EventType.Other:
                    if(nullOrSubtype == null) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Null subtype found for {0}", eventType));
                    if(Enum.IsDefined(typeof(EventSubtypeOther), (uint)nullOrSubtype)) 
                    {
                        throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subType));
                    }
                    subType = (uint)nullOrSubtype;
                break;

                case EventType.Draw:
                    if(nullOrSubtype == null) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Null subtype found for {0}", eventType));
                    if(Enum.IsDefined(typeof(EventSubtypeDraw), (uint)nullOrSubtype)) 
                    {
                        throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subType));
                    }
                    subType = (uint)nullOrSubtype;
                break;

                default:
                    throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Unknown event {0}", eventType));
                break;
            }
    
            return subType;
        }
    }
}