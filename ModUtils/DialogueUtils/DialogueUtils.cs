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
                throw new NotImplementedException();
            }
        }
    }
}