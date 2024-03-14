﻿using PCAxis.Menu;

using Px.Abstractions.Interfaces;

using PxWeb.Code.Api2.Cache;
using PxWeb.Code.Api2.DataSource.Cnmm;

namespace PxWeb.Code.Api2.DataSource.PxFile
{
    public class ItemSelectionResolverPxFile : IItemSelectionResolver
    {

        private readonly IPxCache _pxCache;
        private readonly IItemSelectionResolverFactory _itemSelectionResolverFactory;
        private readonly IPxApiConfigurationService _pxApiConfigurationService;

        public ItemSelectionResolverPxFile(IPxCache pxCache, IItemSelectionResolverFactory itemSelectionResolverFactory, IPxApiConfigurationService pxApiConfigurationService)
        {
            _pxCache = pxCache;
            _itemSelectionResolverFactory = itemSelectionResolverFactory;
            _pxApiConfigurationService = pxApiConfigurationService;
        }
        public ItemSelection Resolve(string language, string selection, out bool selectionExists)
        {

            selectionExists = true;
            ItemSelection itemSelection = new ItemSelection();

            string lookupTableName = "LookUpTableCache_" + language;
            var lookupTable = _pxCache.Get<Dictionary<string, ItemSelection>>(lookupTableName);
            if (lookupTable is null)
            {
                lookupTable = _itemSelectionResolverFactory.GetMenuLookup(language);
                _pxCache.Set(lookupTableName, lookupTable);
            }

            if (!string.IsNullOrEmpty(selection))
            {
                if (lookupTable.ContainsKey(selection.ToUpper()))
                {
                    var itmSel = lookupTable[selection.ToUpper()];
                    itemSelection.Menu = itmSel.Menu;
                    itemSelection.Selection = itmSel.Selection;
                }
                else
                {
                    selectionExists = false;
                }
            }
            return itemSelection;
        }
    }
}
