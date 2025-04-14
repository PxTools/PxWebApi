namespace PxWeb.Config.Api2
{
    public class SavedQueryDatabaseStorageOptions
    {
        public const string SectionName = "DatabaseStorage";

        public SavedQueryDatabaseStorageOptions()
        {
            ConnectionString = string.Empty;
            TargetDatabase = "default";
            TableOwner = "dbo";
            DatabaseVendor = "Microsoft";
        }

        public string ConnectionString { get; set; }
        public string TargetDatabase { get; set; }
        public string TableOwner { get; set; }
        public string DatabaseVendor { get; set; }

    }
}
