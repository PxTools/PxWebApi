
namespace PxWeb.UnitTests.Search
{
    using Px.Abstractions.Interfaces;

    public class MyHost : IPxHost
    {
        public string RootPath { get; set; }

        public MyHost(string inRootPath)
        {
            RootPath = inRootPath;  
        }
   
    }
}

