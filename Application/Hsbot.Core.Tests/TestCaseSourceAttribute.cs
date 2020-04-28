using System;

namespace Hsbot.Core.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseSourceAttribute : Attribute
    {
        public TestCaseSourceAttribute(string sourceName, Type sourceType)
        {
            SourceName = sourceName;
            SourceType = sourceType;
        }

        public TestCaseSourceAttribute(string sourceName)
        {
            SourceName = sourceName;
            SourceType = null;
        }

        public Type SourceType { get; set; }
        public string SourceName { get; private set; }
    }
}
