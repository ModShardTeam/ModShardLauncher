using System;
using System.Linq;
using Serilog;
using UndertaleModLib.Models;
using static UndertaleModLib.Models.UndertaleGameObject;

namespace ModShardLauncher
{
    /// <summary>
    /// Abstraction for the event system in GML.
    /// </summary>
    public class MslEvent
    {
        /// <summary>
        /// File where the code of the event is stored.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// The <see cref="EventType"/> of the event.
        /// </summary>
        public EventType EventType { get; set; }
        /// <summary>
        /// The <see cref="EventSubtype"/> of the event.
        /// </summary>
        public uint Subtype { get; set; }
        /// <summary>
        /// Return an complete wrapped event with the name of the file containing its source code.
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="eventType"></param>
        /// <param name="subtype"></param>
        public MslEvent(string code, EventType eventType, uint subtype)
        {
            Code = code;
            EventType = eventType;
            Subtype = subtype;
        }
        /// <summary>
        /// Given an <see cref="UndertaleGameObject"/> named <paramref name="objectName"/>, load the source code of the event and add it in the data.win through the <see cref="AddNewEvent"/> function.
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="modFile"></param>
        public void Apply(string objectName, ModFile modFile)
        {
            try
            {
                Msl.AddNewEvent(objectName, modFile.GetCode(Code), EventType, Subtype);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Given a <paramref name="gameObject"/>, load the source code of the event and add it in the data.win through the <see cref="AddNewEvent"/> function.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="modFile"></param>
        public void Apply(UndertaleGameObject gameObject, ModFile modFile)
        {
            try
            {
                Msl.AddNewEvent(gameObject, modFile.GetCode(Code), EventType, Subtype);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Given an <see cref="UndertaleGameObject"/> named <paramref name="objectName"/>, load the source code of the event and add it in the data.win through the <see cref="AddNewEvent"/> function.
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="modFile"></param>
        public void Apply(string objectName)
        {
            try
            {
                Msl.AddNewEvent(objectName, Code, EventType, Subtype);
            }
            catch
            {
                throw;
            }
        }
        public void Apply(UndertaleGameObject objectName)
        {
            try
            {
                Msl.AddNewEvent(objectName, Code, EventType, Subtype);
            }
            catch
            {
                throw;
            }
        }
    }
    public static partial class Msl
    {
        /// <summary>
        /// Given the <paramref name="objectName"/> of a <see cref="UndertaleGameObject"/>, an <paramref name="eventType"/> and its <paramref name="subtype"/>,
        /// return the name of the script associated.
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="eventType"></param>
        /// <param name="subtype"></param>
        /// <returns></returns>
        public static string EventName(string objectName, EventType eventType, uint subtype)
        {
            return "gml_Object_" + objectName + "_" + eventType + "_" + subtype;
        }
        public static void AddNewEvent(string objectName, string eventCode, EventType eventType, uint subtype)
        {
            AddNewEvent(objectName, eventCode, eventType, subtype, false);
        }
        /// <summary>
        /// Add a new event (<paramref name="eventType"/>, <paramref name="subtype"/>) associated to an <see cref="UndertaleGameObject"/> named <paramref name="objectName"/>
        /// to the data.win.
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="eventCode"></param>
        /// <param name="eventType"></param>
        /// <param name="subtype"></param>
        public static void AddNewEvent(string objectName, string eventCode, EventType eventType, uint subtype, bool asAsm = false)
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
                if (asAsm)
                {
                    AddCodeAsm(eventCode, newEventName);
                }
                else
                {
                    AddCode(eventCode, newEventName);
                }
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
        public static void AddNewEvent(UndertaleGameObject objectName, string eventCode, EventType eventType, uint subtype)
        {
            try
            {
                AddNewEvent(objectName, eventCode, eventType, subtype, false);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Add a new event (<paramref name="eventType"/>, <paramref name="subtype"/>) associated to an <paramref name="gameObject"/>
        /// to the data.win.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="eventCode"></param>
        /// <param name="eventType"></param>
        /// <param name="subtype"></param>
        public static void AddNewEvent(UndertaleGameObject gameObject, string eventCode, EventType eventType, uint subtype, bool asAsm = false)
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

                if (asAsm)
                {
                    AddCodeAsm(eventCode, newEventName);
                }
                else
                {
                    AddCode(eventCode, newEventName);
                }
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
        /// <summary>
        /// Check if the combination <paramref name="eventType"/> and <paramref name="subtype"/> are correct. Raise an exception if not.
        /// <example>
        /// For example:
        /// <code>
        /// CheckSubEvent(EventType.Create, 1);
        /// </code>
        /// will raise an <see cref="ArgumentNullException"/>.
        /// </example>
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="subtype"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private static void CheckSubEvent(EventType eventType, uint subtype)
        { 
            switch(eventType)
            {
                case EventType.PreCreate:
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