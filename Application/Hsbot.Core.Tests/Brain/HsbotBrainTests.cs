using System.Collections.Generic;
using Hsbot.Slack.Core.Brain;
using Shouldly;

namespace Hsbot.Core.Tests.Brain
{
    public class HsbotBrainTests
    {
        public class TestObject
        {
            public string Text { get; set; }
            public int TimesRecalled { get; set; }
            public bool IsPublic { get; set; }
        }

        private Dictionary<string, string> DefaultTestBrainContents => new Dictionary<string, string>
        {
            {"test", @"{""Text"": ""Foo"", ""TimesRecalled"": 9, ""IsPublic"": true}"}
        };

        private HsbotBrain TestBrain => new HsbotBrain(DefaultTestBrainContents);

        public void ShouldBeEmptyByDefault()
        {
            var brain = new HsbotBrain();
            brain.Keys.Count.ShouldBe(0);
        }

        public void ShouldGetPopulatedByConstructorArgument()
        {
            var brain = TestBrain;
            brain.Keys.Count.ShouldBe(1);

            var item = brain.GetItem<TestObject>("test");
            item.ShouldNotBeNull();

            item.Text.ShouldBe("Foo");
            item.TimesRecalled.ShouldBe(9);
            item.IsPublic.ShouldBe(true);
        }

        public void GetItemShouldReturnNullForNonExistentKey()
        {
            var brain = TestBrain;
            brain.Keys.Count.ShouldBe(1);

            var item = brain.GetItem<TestObject>("fakeKey");
            item.ShouldBeNull();
        }

        public void SetItemShouldReplaceExistingItemWithSameKey()
        {
            var brain = TestBrain;
            brain.Keys.Count.ShouldBe(1);

            var newItem = new TestObject { IsPublic = false, Text = "Bar", TimesRecalled = 0};
            brain.SetItem("test", newItem);

            brain.Keys.Count.ShouldBe(1);

            var item = brain.GetItem<TestObject>("test");
            item.ShouldNotBeNull();

            item.Text.ShouldBe("Bar");
            item.TimesRecalled.ShouldBe(0);
            item.IsPublic.ShouldBe(false);
        }

        public void SetItemShouldCreateNewItemWithDifferentKey()
        {
            var brain = TestBrain;
            brain.Keys.Count.ShouldBe(1);

            var newItem = new TestObject { IsPublic = false, Text = "Bar", TimesRecalled = 0};
            brain.SetItem("test2", newItem);

            brain.Keys.Count.ShouldBe(2);

            var item = brain.GetItem<TestObject>("test2");
            item.ShouldNotBeNull();

            item.Text.ShouldBe("Bar");
            item.TimesRecalled.ShouldBe(0);
            item.IsPublic.ShouldBe(false);
        }

        public void SetItemShouldAllowNull()
        {
            var brain = TestBrain;
            brain.SetItem("test2", (TestObject)null);

            brain.Keys.Count.ShouldBe(2);

            var item = brain.GetItem<TestObject>("test2");
            item.ShouldBeNull();
        }
    }
}
