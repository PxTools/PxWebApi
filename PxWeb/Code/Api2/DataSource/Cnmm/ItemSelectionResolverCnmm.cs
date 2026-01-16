using Microsoft.Extensions.Options;

using PCAxis.Menu;

using Px.Abstractions.Interfaces;

using PxWeb.Code.Api2.Cache;

namespace PxWeb.Code.Api2.DataSource.Cnmm
{
    public class ItemSelectionResolverCnmm : IItemSelectionResolver
    {
        //TODO this class and ItemSelectionResolverPxFile are the same except for name and namespace 

        private readonly IPxCache _pxCache;
        private readonly IItemSelectionResolverFactory _itemSelectionResolverFactory;
        private readonly IPxApiConfigurationService _pxApiConfigurationService;
        private readonly string? _rootNode;

        public ItemSelectionResolverCnmm(IPxCache pxCache, IItemSelectionResolverFactory itemSelectionResolverFactory, IPxApiConfigurationService pxApiConfigurationService, IOptions<CnmmConfigurationOptions> cnmmConfigOptions)
        {
            _pxCache = pxCache;
            _itemSelectionResolverFactory = itemSelectionResolverFactory;
            _pxApiConfigurationService = pxApiConfigurationService;
            _rootNode = cnmmConfigOptions.Value.RootNode;
        }

        public ItemSelection ResolveFolder(string language, string selection, out bool selectionExists)
        {
            selectionExists = true;
            ItemSelection itemSelection = new ItemSelection();

            string lookupTableName = "LookUpTableCache_Folder" + language;
            var lookupTable = _pxCache.Get<Dictionary<string, ItemSelection>>(lookupTableName);
            if (lookupTable is null)
            {
                lookupTable = _itemSelectionResolverFactory.GetMenuLookupFolders(language);
                if (!string.IsNullOrEmpty(_rootNode))
                {
                    lookupTable = RemoveUnrootedEntries(lookupTable, _rootNode);
                }
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

        public ItemSelection ResolveTable(string language, string selection, out bool selectionExists)
        {
            selectionExists = false;
            ItemSelection itemSelection = new ItemSelection();

            string lookupTableName = "LookUpTableCache_Table_" + language;
            var lookupTable = _pxCache.Get<Dictionary<string, ItemSelection>>(lookupTableName);
            if (lookupTable is null)
            {
                lookupTable = _itemSelectionResolverFactory.GetMenuLookupTables(language);
                var newLookupTable = new Dictionary<string, ItemSelection>();
                if (!string.IsNullOrEmpty(_rootNode))
                {
                    foreach (var kvp in lookupTable)
                    {
                        ResolveFolder(language, kvp.Value.Menu, out bool folderExists);
                        if (folderExists)
                        {
                            newLookupTable.Add(kvp.Key, kvp.Value);
                        }
                    }
                    lookupTable = newLookupTable;
                }
                _pxCache.Set(lookupTableName, lookupTable);
            }

            if (!string.IsNullOrEmpty(selection) && lookupTable.ContainsKey(selection.ToUpper()))
            {
                var itmSel = lookupTable[selection.ToUpper()];
                itemSelection.Menu = itmSel.Menu;
                itemSelection.Selection = itmSel.Selection;
                selectionExists = true;
            }

            return itemSelection;
        }


        private static Dictionary<string, ItemSelection> RemoveUnrootedEntries(Dictionary<string, ItemSelection> lookupTable, string rootItem)
        {
            var newLookup = new Dictionary<string, ItemSelection>();
            var filter = new Dictionary<string, List<ItemSelection>>();

            foreach (var selection in lookupTable.Values)
            {
                if (filter.ContainsKey(selection.Menu))
                {
                    filter[selection.Menu].Add(selection);
                }
                else
                {
                    filter[selection.Menu] = new List<ItemSelection>() { selection };
                }
            }

            if (filter.ContainsKey(rootItem))
            {
                newLookup.Add(rootItem, lookupTable[rootItem]);
                AddRecursive(rootItem, newLookup, filter);
            }

            return newLookup;
        }

        private static void AddRecursive(string item, Dictionary<string, ItemSelection> result, Dictionary<string, List<ItemSelection>> lookup)
        {
            if (!lookup.ContainsKey(item))
            {
                return;
            }

            var selections = lookup[item];

            if (selections != null && selections.Count > 0)
            {
                foreach (var selection in selections)
                {
                    result.Add(selection.Selection, selection);
                    AddRecursive(selection.Selection, result, lookup);
                }
            }
        }
    }
}
