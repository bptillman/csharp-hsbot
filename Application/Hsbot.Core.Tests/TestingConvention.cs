using Fixie;

namespace Hsbot.Core.Tests
{
    public class TestingConvention : Discovery, Execution
    {
        public TestingConvention()
        {
            Methods
                .Where(x => x.Name != "SetUp");
        }

        public void Execute(TestClass testClass)
        {
            testClass.RunCases(@case =>
            {
                var instance = testClass.Construct();

                SetUp(instance);

                @case.Execute(instance);
            });
        }

        static void SetUp(object instance)
        {
            instance.GetType().GetMethod("SetUp")?.Execute(instance);
        }
    }
}
