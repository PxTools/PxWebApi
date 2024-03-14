using System.IO;
using System.Xml;

using Microsoft.Extensions.Logging;

using Px.Abstractions.Interfaces;

using PxWeb.Code.Api2.Cache;

namespace PxWeb.Code.Api2.DataSource.PxFile
{
    public class TablePathResolverPxFile : ITablePathResolver
    {
        private readonly IPxCache _pxCache;
        private readonly IPxHost _hostingEnvironment;
        private readonly IPxApiConfigurationService _pxApiConfigurationService;
        private readonly ILogger _logger;

        public TablePathResolverPxFile(IPxCache pxCache, IPxHost hostingEnvironment, IPxApiConfigurationService pxApiConfigurationService, ILogger<TablePathResolverPxFile> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _pxCache = pxCache;
            _pxApiConfigurationService = pxApiConfigurationService;
        }

        public string Resolve(string language, string id, out bool selectionExists)
        {
            string tablePath = "";
            selectionExists = true;

            string lookupTableName = "LookUpTablePathCache_" + language;
            var lookupTable = _pxCache.Get<Dictionary<string, string>>(lookupTableName);
            if (lookupTable is null)
            {
                lookupTable = GetPxTableLookup(language);
                _pxCache.Set(lookupTableName, lookupTable);
            }

            if (!string.IsNullOrEmpty(id))
            {
                if (lookupTable.ContainsKey(id.ToUpper()))
                {
                    tablePath = Path.Combine(_hostingEnvironment.RootPath, lookupTable[id.ToUpper()]);
                }
                else
                {
                    selectionExists = false;
                }
            }

            return tablePath;
        }

        private Dictionary<string, string> GetPxTableLookup(string language)
        {
            var tableLookup = new Dictionary<string, string>();

            if (!LanguageUtil.HasValidLanguageCodePattern(language))
            {
                _logger.LogWarning($"Unsupported language: {LanguageUtil.SanitizeLangueCode(language)}");
                return tableLookup;
            }

            language = LanguageUtil.SanitizeLangueCode(language);

            try
            {
                var menuXmlFile = new MenuXmlFile(_hostingEnvironment);
                XmlDocument xdoc = menuXmlFile.GetLanguageAsXmlDocument(language);

                string xpath = "//Link";
                var nodeList = xdoc.SelectNodes(xpath);

                if (nodeList != null)
                {
                    foreach (XmlElement childEl in nodeList)
                    {
                        string selection = childEl.GetAttribute("selection");
                        string tableId = childEl.GetAttribute("tableId").ToUpper();
                        if (!tableLookup.ContainsKey(tableId))
                        {
                            tableLookup.Add(tableId, selection);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error loading TablePathLookup table for language {LanguageUtil.SanitizeLangueCode(language)}");
            }

            return tableLookup;
        }

    }
}
