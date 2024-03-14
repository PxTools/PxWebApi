using System;

namespace PxWebApi.BigTests.Other
{
    [TestClass]
    public class TestLoggerTest
    {
        [TestMethod]
        public void LogSomething()
        {
            var testLogger = new TestLogger<TestLoggerTest>();

            string[] strings = new[] { "XXX", "b" };

            testLogger.LogError(new NotImplementedException("String in exception constructor"), "String formt in LogError call 2.nd param {0},{1}", strings[0], strings[1]);

            testLogger.LogDebug("Just a debug message");

            testLogger.LogError(new NotImplementedException("String in exception constructor"), "String in LogError call 2.nd param");

            var logged = testLogger.LogMessages;

            Assert.IsNotNull(logged);
            Assert.IsTrue(logged.Count == 3);
            Assert.IsTrue(logged[0].Contains("XXX"));
        }

    }
}
