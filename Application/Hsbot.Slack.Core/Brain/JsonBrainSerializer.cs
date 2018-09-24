using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hsbot.Slack.Core.Brain
{
    public class JsonBrainSerializer : IBotBrainSerializer<HsbotBrain>
    {
        public HsbotBrain Deserialize(string serializedBrain)
        {
            var brainAsDictionary = new Dictionary<string, string>();
            var jObject = JObject.Parse(serializedBrain);
            foreach (var property in jObject.Properties())
            {
                brainAsDictionary[property.Name] = property.Value.ToString();
            }

            return new HsbotBrain(brainAsDictionary);
        }

        public string Serialize(HsbotBrain brain)
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