using System;
using System.Linq;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Models;
using static UndertaleModLib.Models.UndertaleGameObject;

namespace ModShardLauncher
{
    public enum MslEventType
    {
        Create,
        Destroy,
        Step0,
        StepBegin,
        StepEnd,
        Alarm0,
        Alarm1,
        Alarm2,
        Alarm3,
        Alarm4,
        Alarm5,
        Alarm6,
        Alarm7,
        Alarm8,
        Alarm9,
        Alarm10,
        Alarm11,
        KeyBoard, //TODO: check here
        LeftButton,
        RightButton,
        MiddleButton,
        NoButton,
        LeftPress,
        RightPress,
        MiddlePress,
        LeftRelease,
        RightRelease,
        MiddleRelease,
        MouseEnter,
        MouseLeave,
        MouseWheelUp,
        MouseWheelDown,
        GlobalLeftButton,
        GlobalRightButton,
        GlobalMiddleButton,
        GlobalLeftPress,
        GlobalRightPress,
        GlobalMiddlePress,
        GlobalLeftRelease,
        GlobalRightRelease,
        GlobalMiddleRelease,
        DrawGui,
        
    }
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
                if(nullOrSubtype != null) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Null subtype expected for {0}", eventType));
                break;
                
                case EventType.Alarm:
                if(nullOrSubtype == null) throw new ArgumentNullException(nullOrSubtype.ToString(), string.Format("Null subtype expected for {0}", eventType));
                break;
                case EventType.Step:
                case EventType.Collision:
                case EventType.Keyboard:
                case EventType.Mouse:
                case EventType.Other:
                case EventType.Draw:
                case EventType.KeyPress:
                case EventType.KeyRelease:
                case EventType.Trigger:
                default:
                break; 
            }

            return subType;
        }
        public static void AddNewEvent(string objectName, string eventCode, MslEventType mslEventType)
        {
            switch(mslEventType)
            {
                case MslEventType.Create:
                    AddNewEvent(objectName, eventCode, EventType.Create, 0);
                    break;
                case MslEventType.Destroy:
                    AddNewEvent(objectName, eventCode, EventType.Destroy, 0);
                    break;
                case MslEventType.Step0:
                    AddNewEvent(objectName, eventCode, EventType.Step, 0);
                    break;
                case MslEventType.Alarm0:
                    AddNewEvent(objectName, eventCode, EventType.Alarm, 0);
                    break;
                case MslEventType.DrawGui:
                    AddNewEvent(objectName, eventCode, EventType.Draw, 0);
                    break;
                default:
                    Log.Error(string.Format("Invalid MslEventType found in AddNewEvent. Found: {0}", mslEventType));
                    break;
            }
        }
    }
}