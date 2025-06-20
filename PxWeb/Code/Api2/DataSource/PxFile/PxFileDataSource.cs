﻿using System.IO;
using System.Xml;
using System.Xml.Linq;

using PCAxis.Menu;
using PCAxis.Menu.Implementations;
using PCAxis.Paxiom;

using Px.Abstractions;
using Px.Abstractions.Interfaces;

using PxWeb.Mappers;

namespace PxWeb.Code.Api2.DataSource.PxFile
{
    public class PxFileDataSource : IDataSource
    {
        private readonly IPxFileConfigurationService _pxFileConfigurationService;
        private readonly IItemSelectionResolver _itemSelectionResolver;
        private readonly ITablePathResolver _tablePathResolver;
        private readonly IPxHost _hostingEnvironment;
        private readonly ICodelistMapper _codelistMapper;

        public PxFileDataSource(IPxFileConfigurationService pxFileConfigurationService, IItemSelectionResolver itemSelectionResolver, ITablePathResolver tablePathResolver, IPxHost hostingEnvironment, ICodelistMapper codelistMapper)
        {
            _pxFileConfigurationService = pxFileConfigurationService;
            _itemSelectionResolver = itemSelectionResolver;
            _tablePathResolver = tablePathResolver;
            _hostingEnvironment = hostingEnvironment;
            _codelistMapper = codelistMapper;
        }

        /// <summary>
        /// Create a PxFileBuilder
        /// </summary>
        /// <param name="id">Table id</param>
        /// <param name="language">Language</param>
        /// <returns>Builder object, null if builder could not be created</returns>
        public IPXModelBuilder? CreateBuilder(string id, string language)
        {
            var builder = new PxFileBuilder2();

            var path = _tablePathResolver.Resolve(language, id, out bool selectionExists);

            if (selectionExists)
            {
                builder.SetPath(path);
                builder.SetPreferredLanguage(language);
                return builder;
            }
            else
            {
                return null;
            }
        }

        public TableLink? CreateMenuTableLink(string id, string language)
        {
            ItemSelection itmSel = _itemSelectionResolver.ResolveTable(language, id, out bool selectionExists);
            if (!selectionExists)
            {
                return null;
            }

            Item? outItem = CreateMenu(language, itmSel);

            return (TableLink?)outItem;
        }

        public Item? CreateMenu(string id, string language, out bool selectionExists)
        {
            ItemSelection itmSel = _itemSelectionResolver.ResolveFolder(language, id, out selectionExists);
            if (!selectionExists)
            {
                return null;
            }

            Item? outItem = CreateMenu(language, itmSel);

            return outItem;

        }

        private Item? CreateMenu(string language, ItemSelection itmSel)
        {

            MenuXmlFile menuXmlFile = new MenuXmlFile(_hostingEnvironment);
            XmlDocument xmlDocument = menuXmlFile.GetAsXmlDocument();

            var xNavigator = xmlDocument.CreateNavigator();
            XDocument xDocument = xNavigator != null ? XDocument.Load(xNavigator.ReadSubtree()) : new XDocument();


            XmlMenu menu = new XmlMenu(xDocument, language,
                    m =>
                    {
                        m.Restriction = item =>
                        {
                            return true;
                        };
                    });

            menu.SetCurrentItemBySelection(itmSel.Menu, itmSel.Selection);

            // Fix selection for subitems - we only want the last part...
            if (menu.CurrentItem is PxMenuItem)
            {
                foreach (var item in ((PxMenuItem)(menu.CurrentItem)).SubItems)
                {
                    if ((item is PxMenuItem) || (item is TableLink))
                    {
                        //Hmm, doesnt this mean that a TableLink for TAB004  will differ when it is root and in a folder? It that ok?
                        //Yes, it seems. item.ID.Selection is not used by CreateMenuTableLinnk client.  
                        item.ID.Selection = GetIdentifierWithoutPath(item.ID.Selection);
                    }
                }
            }

            return menu.CurrentItem;
        }

        public Codelist? GetCodelist(string id, string language)
        {
            Codelist? codelist = null;

            if (string.IsNullOrEmpty(id))
            {
                return codelist;
            }

            if (id.StartsWith("agg_", System.StringComparison.InvariantCultureIgnoreCase))
            {
                // Remove leading "agg_" from id
                id = id.Substring(4);
            }

            Grouping grouping = PCAxis.Paxiom.GroupRegistry.GetRegistry().GetGrouping(id);

            if (grouping != null)
            {
                codelist = _codelistMapper.Map(grouping);
                codelist.AvailableLanguages.Add(language);
            }

            return codelist;
        }

        public bool TableExists(string tableId, string language)
        {
            bool selectionExists;

            _itemSelectionResolver.ResolveTable(language, tableId, out selectionExists);
            return selectionExists;

        }

        private string GetIdentifierWithoutPath(string id)
        {
            if (id.Contains('/'))
            {
                return Path.GetFileName(id);
            }
            else
            {
                return id;
            }
        }

        public List<string> GetTablesPublishedBetween(DateTime from, DateTime to)
        {
            MenuXmlFile menuXmlFile = new MenuXmlFile(_hostingEnvironment);
            var doc = menuXmlFile.GetAsXmlDocument();

            var tableIds = new List<string>();
            var nodes = doc.SelectNodes("//Link[LastUpdated]");

            if (nodes is null)
            {
                return tableIds;
            }

            foreach (XmlNode link in nodes)
            {
                var lastUpdatedNode = link.SelectSingleNode("LastUpdated");
                if (lastUpdatedNode != null)
                {
                    if (DateTime.TryParse(lastUpdatedNode.InnerText, out DateTime lastUpdated))
                    {
                        if (lastUpdated >= from && lastUpdated <= to)
                        {
                            string? tableId = link.Attributes?["tableId"]?.Value;

                            if (tableId is not null && !tableIds.Contains(tableId))
                            {
                                tableIds.Add(tableId);
                            }
                        }
                    }
                }
            }

            return tableIds;
        }
    }
}
