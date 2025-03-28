﻿using System.IO;
using System.Xml;

using Microsoft.Extensions.Logging;

using PCAxis.Menu;

using Px.Abstractions.Interfaces;

namespace PxWeb.Code.Api2.DataSource.PxFile
{
    public class ItemSelectorResolverPxFactory : IItemSelectionResolverFactory
    {
        private readonly IPxFileConfigurationService _pxFileConfigurationService;
        private readonly IPxHost _hostingEnvironment;
        private readonly ILogger _logger;

        public ItemSelectorResolverPxFactory(IPxFileConfigurationService pxFileConfigurationService, IPxHost hostingEnvironment, ILogger<ItemSelectorResolverPxFactory> logger)
        {
            _pxFileConfigurationService = pxFileConfigurationService;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public Dictionary<string, ItemSelection> GetMenuLookupFolders(string language)
        {
            var menuLookup = new Dictionary<string, ItemSelection>();

            if (!LanguageUtil.HasValidLanguageCodePattern(language))
            {
                _logger.LogWarning("Unsupported language: {Code}", LanguageUtil.SanitizeLangueCode(language));
                return menuLookup;
            }

            language = LanguageUtil.SanitizeLangueCode(language);

            try
            {
                var menuXmlFile = new MenuXmlFile(_hostingEnvironment);
                XmlDocument xdoc = menuXmlFile.GetLanguageAsXmlDocument(language);

                // Add Menu levels to lookup table
                string xpath = "//MenuItem";
                AddMenuItemsToMenuLookup(xdoc, menuLookup, xpath);
            }

            catch (Exception e)
            {
                _logger.LogError(e, "Error loading MenuLookup table for language {Code}", LanguageUtil.SanitizeLangueCode(language));
            }

            return menuLookup;
        }

        private void AddMenuItemsToMenuLookup(XmlDocument xdoc, Dictionary<string, ItemSelection> menuLookup, string xpath)
        {
            var nodeList = xdoc.SelectNodes(xpath);

            if (nodeList != null)
            {
                foreach (XmlElement childEl in nodeList)
                {
                    string selection = childEl.GetAttribute("selection");
                    var menu = Path.GetDirectoryName(selection)?.Replace("\\", "/");
                    var sel = Path.GetFileName(selection).ToUpper();
                    if (!menuLookup.ContainsKey(sel))
                    {
                        ItemSelection itemSelection = new ItemSelection(menu, selection);
                        menuLookup.Add(sel, itemSelection);
                    }
                }
            }
        }


        public Dictionary<string, ItemSelection> GetMenuLookupTables(string language)
        {
            var menuLookup = new Dictionary<string, ItemSelection>();

            if (!LanguageUtil.HasValidLanguageCodePattern(language))
            {
                _logger.LogWarning("Unsupported language: {Code}", LanguageUtil.SanitizeLangueCode(language));
                return menuLookup;
            }

            language = LanguageUtil.SanitizeLangueCode(language);

            try
            {
                var menuXmlFile = new MenuXmlFile(_hostingEnvironment);
                XmlDocument xdoc = menuXmlFile.GetLanguageAsXmlDocument(language);

                // Add Tables to lookup table
                string xpath = "//Link";
                AddTablesToMenuLookup(xdoc, menuLookup, xpath);
            }

            catch (Exception e)
            {
                _logger.LogError(e, "Error loading MenuLookup table for language {Code}", LanguageUtil.SanitizeLangueCode(language));
            }

            return menuLookup;
        }

        private void AddTablesToMenuLookup(XmlDocument xdoc, Dictionary<string, ItemSelection> menuLookup, string xpath)
        {
            var nodeList = xdoc.SelectNodes(xpath);

            if (nodeList != null)
            {
                foreach (XmlElement childEl in nodeList)
                {
                    string selection = childEl.GetAttribute("selection");
                    string tableId = childEl.GetAttribute("tableId");
                    var menu = Path.GetDirectoryName(selection)?.Replace("\\", "/");
                    if (!menuLookup.ContainsKey(tableId))
                    {
                        ItemSelection itemSelection = new ItemSelection(menu, selection);
                        menuLookup.Add(tableId, itemSelection);
                    }
                }
            }
        }

    }
}
