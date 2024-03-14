using System;
using System.Runtime.InteropServices;

using PxWeb.Code.PxDatabase;

namespace PxWebApi.BigTests.Other
{
    [TestClass]
    public class OSTest
    {
        [TestMethod]
        public void AliasTxtFirstComparerTest()
        {
            string[] XSorted = new[] { "BAlias_en", "balias.txt", "Alias.txt", "Alias_sv" };

            Array.Sort(XSorted, new AliasTxtFirstComparer());

            Assert.AreEqual("balias.txt", XSorted[0]);
            Assert.AreEqual("Alias.txt", XSorted[1]);
            Assert.AreEqual("BAlias_en", XSorted[2]);
            Assert.AreEqual("Alias_sv", XSorted[3]);


            string path = Util.GetFullPathToFile(@"PxWeb/wwwroot/Database/EN");
            string[] filesInDir = System.IO.Directory.GetFiles(path);
            Array.Sort(filesInDir, new AliasTxtFirstComparer());
            Assert.IsTrue(filesInDir[0].EndsWith("Alias.txt"));

        }

        [Ignore]
        [TestMethod]
        public void GetFilesTest()
        {
            string path = Util.GetFullPathToFile(@"PxWeb/wwwroot/Database/EN");

            string[] filesInDir = System.IO.Directory.GetFiles(path);
            //https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles?view=net-6.0
            // says: The order of the returned file names is not guaranteed; use the Sort method if a specific sort order is required.



            string[] actual = new string[filesInDir.Length];

            for (int i = 0; i < filesInDir.Length; i++)
            {
                Console.WriteLine(filesInDir[i]);
                actual[i] = System.IO.Path.GetFileNameWithoutExtension(filesInDir[i]);
            }

            string[] stringsNonMac = new[] { "Alias", "Alias_en", "Alias_sv" };
            string[] stringsMac = new[] { "Alias_en", "Alias_sv", "Alias", };
            string[] stringsUbuntu = new[] { "Alias_sv", "Alias", "Alias_en" };


            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("Running on Windows");
                MyDiff(stringsNonMac, actual);

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Console.WriteLine("Running on macOS");
                MyDiff(stringsMac, actual);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                MyDiff(stringsUbuntu, actual);

                // Linux is reported for all distributions; there's no straightforward way to differentiate between them (like Ubuntu) using only .NET APIs.
                // You may need to check specific files or use other methods to distinguish between Linux distributions.
                Console.WriteLine("Running on Linux");

                // Example of a basic check for Ubuntu by looking for a specific file
                if (System.IO.File.Exists("/etc/ubuntu-release"))
                {
                    Console.WriteLine("This is likely Ubuntu.");
                }
                MyDiff(stringsUbuntu, actual);
            }
            else
            {
                Console.WriteLine("Running on an unknown platform.");
                MyDiff(stringsNonMac, actual);
            }
        }

        private void MyDiff(string[] aExcepted, string[] aActual)
        {
            var expected = string.Join(" ", aExcepted);
            var actual = string.Join(" ", aActual);

            Assert.AreEqual(expected, actual);
        }
    }
}
