using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace PxWebApi_Mvc.Tests
{
    internal class Util
    {
        internal static string ExpectedJsonDir()
        {
            return GetFullPathToFile(@"PxWebApi_Mvc.Tests/ExpectedJson/");
        }

        internal static string GetFullPathToFile(string pathRelativeUnitTestingFile)
        {
            string folderProjectLevel = GetPathToPxWebProject();
            string final = System.IO.Path.Combine(folderProjectLevel, pathRelativeUnitTestingFile);
            return final;
        }

        private static string GetPathToPxWebProject()
        {
            string pathAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string directoryName = System.IO.Path.GetDirectoryName(pathAssembly) ?? throw new System.Exception("GetDirectoryName(pathAssembly) is null for:" + pathAssembly);
            string folderAssembly = directoryName.Replace("\\", "/");
            if (folderAssembly.EndsWith("/") == false) folderAssembly = folderAssembly + "/";
            string folderProjectLevel = System.IO.Path.GetFullPath(folderAssembly + "../../../../");
            return folderProjectLevel;
        }

        private static string replaceValueInProblemFields(string inString, string[]? replaceValuesNamed)
        {

            string[] replaceThese = replaceValuesNamed ?? new string[0];

            foreach (var name in replaceThese)
            {
                string pattern = $@"""{name}"":\s*""[^""]*""";
                inString = Regex.Replace(inString, pattern, $@"""{name}"": ""VALUE_HAS_BEEN_REPLACED""");
            }

            return inString;
        }


        internal static void AssertJson(string inExpected, string inActual, string[]? keysExcludedFromTest = null)
        {


            // prettyprints, but changes & to \u0026, so it has to be applied to expected as well
            JsonNode? jsonNodeActual = System.Text.Json.Nodes.JsonNode.Parse(inActual);
            string actual = jsonNodeActual != null ? jsonNodeActual.ToString() : "jsonNodeActual is null";

            JsonNode? jsonNodeExpected = System.Text.Json.Nodes.JsonNode.Parse(inExpected);
            string expected = jsonNodeExpected != null ? jsonNodeExpected.ToString() : "jsonNodeExpected is null";

            actual = replaceValueInProblemFields(actual, keysExcludedFromTest);
            expected = replaceValueInProblemFields(expected, keysExcludedFromTest);

            Assert.AreEqual(expected.Substring(0, 5), actual.Substring(0, 5), "Diff in first 5.");

            for (int i = 0; i < actual.Length; i += 25)
            {
                int lengthToCompare = Math.Min(50, actual.Length - i);
                Assert.AreEqual(expected.Substring(i, lengthToCompare), actual.Substring(i, lengthToCompare));
            }

        }
    }
}
