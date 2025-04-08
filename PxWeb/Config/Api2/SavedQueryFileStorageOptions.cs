namespace PxWeb.Config.Api2
{
    public class SavedQueryFileStorageOptions
    {
        public const string SectionName = "FileStorage";
        public SavedQueryFileStorageOptions()
        {
            Path = "saved-queries";
        }

        public string Path { get; set; }

    }
}
