using System;
using System.Collections.Generic;
using System.Linq;

namespace ModShardLauncher.ModUtils
{
    /* the builder class `AddDE2Dialogue` should lead to the following syntax :
     
     new Msl.AddDE2Dialogue("DialogueName")
    .AddTopic("topic0")
    .AddOptionToLastTopic("sayHi", "scr_myScript", "topic1")
    .AddOptionToLastTopic("Leave", null, null)
    .AddTopic("topic1")
    .AddOptionToLastTopic("back", null, "topic0")
    .Build();
    
    */
    public static partial class Msl
    {
        public class AddDE2Dialogue
        {
            private DE2Dialogue _dialogue;
            
            public AddDE2Dialogue(string name)
            {
                _dialogue = new DE2Dialogue(name);
            }

            public AddDE2Dialogue AddTopic(string name)
            {
                DE2Topic topic = new(name);
                _dialogue.DialogueTopics.Add(topic);
                return this;
            }

            public AddDE2Dialogue AddOptionToLastTopic(string text, string script, string nextTopic)
            {
                DE2Option option = new(text, script, nextTopic);
                DE2Topic? lastTopic = _dialogue.DialogueTopics.LastOrDefault();
                lastTopic?.AddOption(option);
                return this;
            }
            
            public void Build()
            {
                // Creating a HashSet to avoid looping through topics twice
                HashSet<string> topicNames = new(_dialogue.DialogueTopics.Select(t => t.Name));
                
                // Making sure all NextTopic reference valid topics
                foreach (DE2Topic topic in _dialogue.DialogueTopics)
                {
                    foreach (DE2Option option in topic.TopicOptions)
                    {
                        if (option.NextTopic != null && !topicNames.Contains(option.NextTopic))
                            throw new ArgumentException($"Option {option.Text} in topic {topic.Name} references a non-existent topic {option.NextTopic} in dialogue {_dialogue.Name}");
                    }
                }
                
                // Actually build the .de2 file
                /*
                 
                <topic_start>                   // New Topic Marker.
                topic4                          // Topic Name. (unique name of this entire topic)
                arena_cooldown                  // Topic Text.
                ...                             // Topic Script. Called when this topic opens. Empty line if none.
                trade                           // Option Name. (unique name to lookup in tables or regular string message)
                scr_trade_open                  // Option Script. Called when this option is selected. Empty line if none. 
                ...                             // The next topic this dialogue leads to. Empty line if none.
                Leave                           // Option Name. (unique name to lookup in tables or regular string message)
                scr_close_dialog                // Option Script. Called when this option is selected. Empty line if none.
                ...                             // The next topic this dialogue leads to. Empty line if none.
                <de2_opt>88/35/6051070/300      // Topic Closing Marker. (X/Y/OLE COLOR/Width)
                 
                 */
                
                // Write `<topic_start>`
                // Write topic name
                // Write topic text if any
                // Write topic script if any
                // For each option:
                //    Write option name
                //    Write option script if any
                //    Write option next topic if any
                // Write `<de2_opt>X/Y/COLOR/300`
                
                throw new NotImplementedException();
            }
        }
    }
}