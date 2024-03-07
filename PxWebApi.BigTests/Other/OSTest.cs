using System;
using System.Runtime.InteropServices;

namespace PxWebApi.BigTests.Other
{
    [TestClass]
    public class OSTest
    {
        [TestMethod]
        public void FileOrderingTest()
        {
            string path = Util.GetFullPathToFile(@"PxWeb/wwwroot/Database/EN");



            string[] filesInDir = System.IO.Directory.GetFiles(path);
            string[] actual = new string[filesInDir.Length];

            for (int i = 0; i < filesInDir.Length; i++)
            {
                Console.WriteLine(filesInDir[i]);
                actual[i] = System.IO.Path.GetFileNameWithoutExtension(filesInDir[i]);
            }

            string[] stringsNonMac = new[] { "Alias", "Alias_en", "Alias_sv" };
            string[] stringsMac = new[] { "Alias_en", "Alias_sv", "Alias", };

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
                MyDiff(stringsNonMac, actual);

                // Linux is reported for all distributions; there's no straightforward way to differentiate between them (like Ubuntu) using only .NET APIs.
                // You may need to check specific files or use other methods to distinguish between Linux distributions.
                Console.WriteLine("Running on Linux");

                // Example of a basic check for Ubuntu by looking for a specific file
                if (System.IO.File.Exists("/etc/ubuntu-release"))
                {
                    Console.WriteLine("This is likely Ubuntu.");
                }
            }
            else
            {
                Console.WriteLine("Running on an unknown platform.");
                MyDiff(stringsNonMac, actual);
            }
        }

        private void MyDiff(string[] aExcepted, string[] aActual)
        {
            var expected= string.Join(" ", aExcepted);
            var actual = string.Join(" ", aActual);

            Assert.AreEqual(expected, actual);
        }
    }
}
