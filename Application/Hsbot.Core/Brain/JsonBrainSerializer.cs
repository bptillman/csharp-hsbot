using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hsbot.Core.Brain
{
    public class JsonBrainSerializer : IBotBrainSerializer<InMemoryBrain>
    {
        public InMemoryBrain Deserialize(string serializedBrain)
        {
            var brainAsDictionary = new Dictionary<string, string>();
            var jObject = JObject.Parse(serializedBrain);
            foreach (var property in jObject.Properties())
            {
                brainAsDictionary[property.Name] = property.Value.ToString();
            }

            return new InMemoryBrain(brainAsDictionary);
        }

        public string Serialize(InMemoryBrain brain)
        {
            var brainObject = new JObject();

            foreach (var key in brain.Keys)
            {
                brainObject.Add(new JProperty(key, brain.GetItem<object>(key)));
            }

            return JsonConvert.SerializeObject(brainObject, Formatting.Indented);
        }
    }
}