using System.Collections.Generic;
using System.Text.RegularExpressions;
using Hsbot.Slack.Core.Brain;
using Shouldly;

namespace Hsbot.Slack.Core.Tests.Brain
{
    public class JsonBrainSerializerTests
    {
        public class SimpleObject
        {
            public string Text { get; set; }
            public int TimesRecalled { get; set; }
            public bool IsPublic { get; set; }
        }

        public class ComplexObject
        {
            public string StringProperty { get; set; }
            public List<SimpleObject> ListOfSimpleObjects { get; set; }
        }

        public void ShouldSerializeSimpleObject()
        {
            var brain = new HsbotBrain();
            var simpleObject = new SimpleObject
            {
                Text = "Foo",
                TimesRecalled = 9,
                IsPublic = true
            };

            brain.SetItem("test", simpleObject);

            var sut = new JsonBrainSerializer();
            var serializedBrain = sut.Serialize(brain);

            const string expected = @"{
    ""test"": {
        ""Text"": ""Foo"",
        ""TimesRecalled"": 9,
        ""IsPublic"": true
    }
}";
            AssertMatchIgnoringWhitespace(serializedBrain, expected);
        }

        public void ShouldSerializeComplexObject()
        {
            var brain = new HsbotBrain();

            var simpleObject = new SimpleObject
            {
                Text = "Bar",
                TimesRecalled = 9,
                IsPublic = true
            };

            var otherSimpleObject = new SimpleObject
            {
                Text = "Baz",
                TimesRecalled = 0,
                IsPublic = false
            };

            var complexObject = new ComplexObject
            {
                StringProperty = "Foo",
                ListOfSimpleObjects = new List<SimpleObject> { simpleObject, otherSimpleObject }
            };

            brain.SetItem("otherTest", complexObject);

            var sut = new JsonBrainSerializer();
            var serializedBrain = sut.Serialize(brain);

            const string expected = @"{
    ""otherTest"": {
        ""StringProperty"": ""Foo"",
        ""ListOfSimpleObjects"": [
            {
                ""Text"": ""Bar"",
                ""TimesRecalled"": 9,
                ""IsPublic"": true
            },
            {
                ""Text"": ""Baz"",
                ""TimesRecalled"": 0,
                ""IsPublic"": false
            }
        ]
    }
}";
            AssertMatchIgnoringWhitespace(serializedBrain, expected);
        }

        public void ShouldDeserializeSimpleObject()
        {
            const string serializedBrain = @"{
    ""test"": {
        ""Text"": ""Foo"",
        ""TimesRecalled"": 9,
        ""IsPublic"": true
    }
}";

            var sut = new JsonBrainSerializer();
            var brain = sut.Deserialize(serializedBrain);

            brain.Keys.Count.ShouldBe(1);

            var testObject = brain.GetItem<SimpleObject>("test");
            testObject.Text.ShouldBe("Foo");
            testObject.TimesRecalled.ShouldBe(9);
            testObject.IsPublic.ShouldBeTrue();
        }

        public void ShouldDeserializeComplexObject()
        {
            const string serializedBrain = @"{
    ""otherTest"": {
        ""StringProperty"": ""Foo"",
        ""ListOfSimpleObjects"": [
            {
                ""Text"": ""Bar"",
                ""TimesRecalled"": 9,
                ""IsPublic"": true
            },
            {
                ""Text"": ""Baz"",
                ""TimesRecalled"": 0,
                ""IsPublic"": false
            }
        ]
    }
}";

            var sut = new JsonBrainSerializer();
            var brain = sut.Deserialize(serializedBrain);

            brain.Keys.Count.ShouldBe(1);

            var testObject = brain.GetItem<ComplexObject>("otherTest");
            testObject.StringProperty.ShouldBe("Foo");
            testObject.ListOfSimpleObjects.Count.ShouldBe(2);
            testObject.ListOfSimpleObjects[0].Text.ShouldBe("Bar");
            testObject.ListOfSimpleObjects[0].TimesRecalled.ShouldBe(9);
            testObject.ListOfSimpleObjects[0].IsPublic.ShouldBe(true);
            testObject.ListOfSimpleObjects[1].Text.ShouldBe("Baz");
            testObject.ListOfSimpleObjects[1].TimesRecalled.ShouldBe(0);
            testObject.ListOfSimpleObjects[1].IsPublic.ShouldBe(false);
        }

        private void AssertMatchIgnoringWhitespace(string actual, string expected)
        {
            var actualWithoutWhitespace = Regex.Replace(actual, @"\s+", "");
            var expectedWithoutWhitespace = Regex.Replace(expected, @"\s+", "");

            actualWithoutWhitespace.ShouldBe(expectedWithoutWhitespace);
        }
    }
}
