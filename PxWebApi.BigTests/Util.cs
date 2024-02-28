namespace PxwebApi.BigTests
{
    internal class Util
    {
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

        //From AI :-)
        internal static IOptions<TOptions> GetIOptions<TOptions>(IConfigurationRoot configuration, string section)
             where TOptions : class, new() // This ensures TOptions is a class and has a parameterless constructor
        {
            TOptions options = new TOptions();
            configuration.GetSection(section).Bind(options);
            IOptions<TOptions> myOptions = Options.Create(options);
            return myOptions;
        }
    }
}
