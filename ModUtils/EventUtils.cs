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
        public static void AddNewEvent(string objectName, string eventCode, EventType eventType, uint subtype)
        {
            try
            {
                CheckSubEvent(eventType, subtype);
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
        public static void AddNewEvent(UndertaleGameObject gameObject, string eventCode, EventType eventType, uint subtype)
        {
            try
            {
                CheckSubEvent(eventType, subtype);
                // check if the subEvent already exists
                Event? subtypeObj = gameObject.Events[(int)eventType].FirstOrDefault(x => x.EventSubtype == subtype);
                if (subtypeObj != null)
                {
                    throw new ArgumentException(string.Format("Cannot add the event {0}_{1} in {2} since it already exists", eventType, subtype, gameObject.Name.Content));
                }

                // create a new code
                string newEventName = EventName(gameObject.Name.Content, eventType, subtype);
                AddCode(eventCode, newEventName);
                // add the previous code to the event
                Event newEvent = new() { EventSubtype = subtype };
                newEvent.Actions.Add(new EventAction()
                {
                    CodeId = GetUMTCodeFromFile(newEventName),
                });
                
                gameObject.Events[(int)eventType].Add(newEvent);
                Log.Information(string.Format("Successfully added event {{{0}_{1}}} in object {{{2}}}", eventType, subtype, gameObject.Name.Content));
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        private static void CheckSubEvent(EventType eventType, uint subtype)
        { 
            switch(eventType)
            {
                case EventType.Create:
                case EventType.Destroy:
                case EventType.Trigger:
                case EventType.CleanUp:
                    // default is zero for uint
                    if (subtype != 0)
                    {
                        throw new ArgumentNullException(subtype.ToString(), string.Format("Null or zero subtype expected for {0}", eventType));
                    }
                break;
                
                case EventType.Alarm:
                    if(subtype > 11) throw new ArgumentNullException(subtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subtype));
                break;
                
                case EventType.Step:
                    if(subtype > 2) throw new ArgumentNullException(subtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subtype));

                break;

                case EventType.Collision:
                    if(subtype >= ModLoader.Data.GameObjects.Count) throw new ArgumentNullException(subtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subtype));
                    if(ModLoader.Data.GameObjects[(int)subtype] == null) throw new ArgumentNullException(subtype.ToString(), string.Format("Invalid subtype {1} found for {0}, gameObject associated is null", eventType, subtype));

                break;

                case EventType.Keyboard:
                case EventType.KeyPress:
                case EventType.KeyRelease:
                    if(!Enum.IsDefined(typeof(EventSubtypeKey), subtype)) 
                    {
                        throw new ArgumentNullException(subtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subtype));
                    }
                break;

                case EventType.Mouse:
                    if(!Enum.IsDefined(typeof(EventSubtypeMouse), subtype)) 
                    {
                        throw new ArgumentNullException(subtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subtype));
                    }
                break;

                case EventType.Other:
                    if(!Enum.IsDefined(typeof(EventSubtypeOther), subtype)) 
                    {
                        throw new ArgumentNullException(subtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subtype));
                    }
                break;

                case EventType.Draw:
                    if(!Enum.IsDefined(typeof(EventSubtypeDraw), subtype)) 
                    {
                        throw new ArgumentNullException(subtype.ToString(), string.Format("Invalid subtype {1} found for {0}", eventType, subtype));
                    }
                break;

                default:
                    throw new ArgumentNullException(subtype.ToString(), string.Format("Unknown event {0}", eventType));
            }
        }
    }
}