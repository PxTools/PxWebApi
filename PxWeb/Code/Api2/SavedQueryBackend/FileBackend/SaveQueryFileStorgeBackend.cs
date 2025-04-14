using System.IO;
using System.Text.Json;

using Microsoft.Extensions.Options;

using Px.Abstractions.Interfaces;

namespace PxWeb.Code.Api2.SavedQueryBackend.FileBackend
{
    public class SaveQueryFileStorgeBackend : ISavedQueryStorageBackend
    {

        private readonly string _path;
        private readonly int _subDirectoryLength = 1;

        public SaveQueryFileStorgeBackend(IOptions<SavedQueryFileStorageOptions> options, IPxHost host)
        {
            var path = options.Value.Path;
            if (!Path.IsPathFullyQualified(path))
            {
                path = Path.Combine(host.RootPath, path);
            }
            _path = path;
        }

        public string Load(string id)
        {
            id = SavedQueryBackendProxy.SanitizeName(id);
            var statisticsFilePath = Path.Combine(_path, id.Substring(0, _subDirectoryLength), id + ".sqa");

            if (File.Exists(statisticsFilePath))
            {
                return File.ReadAllText(statisticsFilePath);
            }

            return string.Empty;
        }

        public string LoadDefaultSelection(string tableId)
        {
            return Load(tableId);
        }

        public string Save(string savedQuery, string tableId, string language)
        {
            var name = Guid.NewGuid().ToString();

            //Make sure the directory exists
            var fileDirectory = Path.Combine(_path, name.Substring(0, _subDirectoryLength));
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            // Saves the saved query to file
            var filePath = Path.Combine(fileDirectory, name + ".sqa");
            File.WriteAllText(filePath, savedQuery);

            // Create a statistics file
            var statisticsFilePath = Path.Combine(fileDirectory, name + ".sqs");
            var statistics = new SavedQueryStats() { Created = DateTime.Now };
            File.WriteAllText(statisticsFilePath, JsonSerializer.Serialize(statistics));
            return name;
        }

        public bool UpdateRunStatistics(string id)
        {
            id = SavedQueryBackendProxy.SanitizeName(id);
            // Load the statistics file
            var statisticsFilePath = Path.Combine(_path, id.Substring(0, _subDirectoryLength), id + ".sqs");
            SavedQueryStats? statistics;
            if (File.Exists(statisticsFilePath))
            {
                statistics = JsonSerializer.Deserialize<SavedQueryStats>(File.ReadAllText(statisticsFilePath));
            }
            else if (!File.Exists(Path.Combine(_path, id.Substring(0, _subDirectoryLength), id + ".sqa")))
            {
                // No saved query file exists, so we can't update the statistics
                return false;
            }
            else
            {
                statistics = new SavedQueryStats() { Created = DateTime.Now };
            }

            if (statistics is not null)
            {
                statistics.UsageCount++;
                statistics.LastUsed = DateTime.Now;
            }

            File.WriteAllText(statisticsFilePath, JsonSerializer.Serialize(statistics));
            return true;
        }
    }
}
