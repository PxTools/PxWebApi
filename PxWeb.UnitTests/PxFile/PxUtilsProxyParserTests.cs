using System;
using System.Collections.Specialized;
using System.Text;

using PxWeb.PxFile;
using PxWeb.UnitTests.Fixtures;

namespace PxWeb.UnitTests.PxFile
{
    [TestClass]
    public class PxUtilsProxyParserTests
    {
        public sealed class TestablePxUtilsProxyParser(string fixture) : PxUtilsProxyParser(filePath: "in-memory")
        {
            internal readonly struct HandlerCallInfo
            {
                public string Keyword { get; init; }
                public string? Language { get; init; }
                public string SubKey { get; init; }
                public StringCollection Values { get; init; }
            }

            internal List<HandlerCallInfo> HandlerCalls { get; } = [];

            internal override Stream OpenFileStream()
            {
                var bytes = Encoding.UTF8.GetBytes(fixture);
                return new MemoryStream(bytes, writable: false);
            }

            internal bool TestMetaHandler(string key, string lang, string subkey, StringCollection values)
            {
                HandlerCalls.Add(new HandlerCallInfo
                {
                    Keyword = key,
                    Language = lang,
                    SubKey = subkey,
                    Values = values
                });
                return true;
            }
        }


        [TestMethod]
        public void ParseMeta_ParsesLanguageEntry_WithAllLanguages()
        {
            var parser = new TestablePxUtilsProxyParser(PxFileFixtures.GenericPxFile);
            parser.ParseMeta(parser.TestMetaHandler, preferredLanguage: "en");

            Assert.AreEqual(1, parser.HandlerCalls.Count(call => call.Keyword == "LANGUAGE"));
            Assert.AreEqual(1, parser.HandlerCalls.Count(call => call.Keyword == "LANGUAGES"));
            Assert.HasCount(3, parser.HandlerCalls.First(call => call.Keyword == "LANGUAGES").Values);
        }

        [TestMethod]
        public void ParseMeta_ParseCodePage_ReturnUtf8()
        {
            var parser = new TestablePxUtilsProxyParser(PxFileFixtures.GenericPxFile);
            parser.ParseMeta(parser.TestMetaHandler, preferredLanguage: "en");

            Assert.AreEqual(1, parser.HandlerCalls.Count(call => call.Keyword == "CODEPAGE"));
            Assert.AreEqual("utf-8", parser.HandlerCalls.First(call => call.Keyword == "CODEPAGE").Values[0]);
        }

        [TestMethod]
        public void ParseMeta_StubAndHeading_ReadsCorrectly()
        {
            var parser = new TestablePxUtilsProxyParser(PxFileFixtures.GenericPxFile);
            parser.ParseMeta(parser.TestMetaHandler, preferredLanguage: "en");

            Assert.AreEqual(3, parser.HandlerCalls.Count(call => call.Keyword == "STUB"));
            Assert.HasCount(1, parser.HandlerCalls.First(call => call.Keyword == "STUB").Values);
            Assert.AreEqual("Year", parser.HandlerCalls.First(call => call.Keyword == "STUB" && call.Language == "en").Values[0]);
            Assert.AreEqual(3, parser.HandlerCalls.Count(call => call.Keyword == "HEADING"));
            Assert.HasCount(1, parser.HandlerCalls.First(call => call.Keyword == "HEADING").Values);
            Assert.AreEqual("Information", parser.HandlerCalls.First(call => call.Keyword == "HEADING" && call.Language == "en").Values[0]);
        }

        [TestMethod]
        public void ParseMeta_YearValues_ParsedCorrectly()
        {
            var parser = new TestablePxUtilsProxyParser(PxFileFixtures.GenericPxFile);
            parser.ParseMeta(parser.TestMetaHandler, preferredLanguage: "en");

            var yearValuesCalls = parser.HandlerCalls
                .Where(call => call.Keyword == "VALUES" && call.Language == "en" && call.SubKey == "Year")
                .ToList();

            Assert.HasCount(1, yearValuesCalls);

            var values = yearValuesCalls[0].Values;
            Assert.HasCount(54, values);
            Assert.AreEqual("1970", values[0]);
            Assert.AreEqual("2023*", values[^1]);
        }

        [TestMethod]
        public void ParseMeta_YearNotes_ParsedCorrectly_InAllLanguages()
        {
            var parser = new TestablePxUtilsProxyParser(PxFileFixtures.GenericPxFile);
            parser.ParseMeta(parser.TestMetaHandler, preferredLanguage: "en");

            var yearNoteCalls = parser.HandlerCalls
                .Where(call => call.Keyword == "NOTE" && (
                    call.SubKey == "Vuosi" ||
                    call.SubKey == "År" ||
                    call.SubKey == "Year"))
                .ToList();

            Assert.HasCount(1, yearNoteCalls.Where(c => c.Language is null), "Default language note not found!");
            Assert.HasCount(1, yearNoteCalls.Where(c => c.Language == "sv"), "Swedish note not found!");
            Assert.HasCount(1, yearNoteCalls.Where(c => c.Language == "en"), "English note not found!");

            Assert.AreEqual("*=ennakkotieto", yearNoteCalls.Single(c => c.Language is null).Values[0]);
            Assert.AreEqual("*=preliminär uppgift", yearNoteCalls.Single(c => c.Language == "sv").Values[0]);
            Assert.AreEqual("*=preliminary data", yearNoteCalls.Single(c => c.Language == "en").Values[0]);
        }

        [TestMethod]
        public void ParseMeta_CallbackCount_ShouldBeCorrect()
        {
            var parser = new TestablePxUtilsProxyParser(PxFileFixtures.GenericPxFile);
            parser.ParseMeta(parser.TestMetaHandler, preferredLanguage: "en");
            // Total number of expected handler calls based on the fixture content
            const int expectedHandlerCallCount = 116;
            Assert.HasCount(expectedHandlerCallCount, parser.HandlerCalls, $"Expected {expectedHandlerCallCount} handler calls, but got {parser.HandlerCalls.Count}.");
        }
    }
}
