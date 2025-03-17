namespace Px.Search
{
    /// <summary>
    /// Moves data from IDataSource to ISearchBackend.
    /// </summary>
    public class Indexer
    {
        private readonly IDataSource _source;
        private readonly ISearchBackend _backend;
        private readonly ILogger _logger;
        private List<string> _indexedTables;

        public Indexer(IDataSource dataSource, ISearchBackend backend, ILogger logger)
        {
            _source = dataSource;
            _backend = backend;
            _logger = logger;
            _indexedTables = new List<string>();
        }

        /// <summary>
        /// Creates or recreates a search index for the database
        /// </summary>
        /// <param name="languages">list of languages codes that the search index will be able to be searched for</param>
        public void IndexDatabase(List<string> languages)
        {
            bool selectionExisits;

            using (var index = _backend.GetIndex())
            {
                foreach (var language in languages)
                {

                    index.BeginWrite(language);
                    _indexedTables = new List<string>();
                    _logger.LogInformation("Indexing starting for {Language}.", language);

                    //Get the root item from the database
                    var item = _source.CreateMenu("", language, out selectionExisits);
                    if (selectionExisits)
                    {

                        if (item == null)
                        {
                            _logger.LogError("IndexDatabase : Could not get root level for database");
                            return;
                        }

                        if (item is PxMenuItem)
                        {
                            var path = new List<Level>();
                            TraverseDatabase(item.ID.Selection, language, index, path);
                        }
                    }
                    _logger.LogInformation("Done for {Language}. Indexed total of {Count} tables.", language, _indexedTables.Count);
                    index.EndWrite(language);
                }
            }
        }


        /// <summary>
        /// Traverses the database and looks for tables to add in the index.
        /// </summary>
        /// <param name="id">current node id</param>
        /// <param name="language">current processing language</param>
        /// <param name="index">the index to use</param>
        private void TraverseDatabase(string id, string language, IIndex index, List<Level> path)
        {
            bool exists;
            Item? item;

            try
            {
                item = _source.CreateMenu(id, language, out exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TraverseDatabase : Could not CreateMenu for id {Id} for language {Language}", id, language);

                return;
            }

            if (item == null || !exists)
            {
                _logger.LogError("TraverseDatabase : Could not get database level with id {Id} for language {Language}", id, language);
                return;
            }

            if (item is PxMenuItem)
            {
                foreach (var subitem in ((PxMenuItem)item).SubItems)
                {
                    if (subitem is null)
                    {
                        continue;
                    }
                    if (subitem is PxMenuItem)
                    {
                        path.Add(new Level(subitem.ID.Selection, subitem.Text));
                        TraverseDatabase(subitem.ID.Selection, language, index, path);
                    }
                    else if (subitem is TableLink)
                    {
                        AddTableToIndex(language, index, (TableLink)subitem, path);
                    }
                }
            }

        }

        private void AddTableToIndex(string language, IIndex index, TableLink tableLink, List<Level> path)
        {
            string tableId = tableLink.TableId;
            if (!_indexedTables.Contains(tableId))
            {
                IndexTable(tableId, tableLink, language, index, path);

                _indexedTables.Add(tableId);
                if (_indexedTables.Count % 100 == 0)
                {
                    _logger.LogInformation("Indexed {Count} tables ...", _indexedTables.Count);
                }
            }
            else
            {
                _logger.LogDebug("Table {TableId} is already indexed.", tableId);
            }
        }


        /// <summary>
        /// Updates the entries in the search index for the list of specified tables.
        /// </summary>
        /// <param name="tables">List of tables that the search index</param>
        /// <param name="languages">list of languages codes that the search index will be able to be searched for</param>
        public void UpdateTableEntries(List<string> tables, List<string> languages)
        {
            using (var index = _backend.GetIndex())
            {
                foreach (var language in languages)
                {
                    index.BeginUpdate(language);

                    foreach (var table in tables)
                    {
                        TableLink? tableLinkItem = _source.CreateMenuTableLink(table, language);

                        if (tableLinkItem != null)
                        {
                            UpdateTable(table, tableLinkItem, language, index);
                        }
                        else
                        {
                            index.RemoveEntry(table);
                        }
                    }

                    index.EndUpdate(language);
                }
            }
        }

        private void IndexTable(string id, TableLink tblLink, string language, IIndex index, List<Level> path)
        {
            IPXModelBuilder? builder = _source.CreateBuilder(id, language);

            if (builder != null)
            {
                try
                {
                    builder.BuildForSelection();
                    var model = builder.Model;
                    TableInformation tbl = GetTableInformation(id, tblLink, model.Meta);
                    tbl.Paths.Add(path.ToArray());

                    index.AddEntry(tbl, model.Meta);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "IndexTable : Could not build table with id {Id} for language {Language}", id, language);
                }
            }
            else
            {
                _logger.LogError("IndexTable : Could not build table with id {Id} for language {Language}", id, language);
            }

        }
        private void UpdateTable(string id, TableLink tblLink, string language, IIndex index)
        {
            IPXModelBuilder? builder = _source.CreateBuilder(id, language);

            if (builder != null)
            {
                try
                {
                    builder.BuildForSelection();
                    var model = builder.Model;
                    TableInformation tbl = GetTableInformation(id, tblLink, model.Meta);

                    index.UpdateEntry(tbl, model.Meta);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "UpdateTable : Could not build table with id {Id} for language {Language}", id, language);
                }
            }
            else
            {
                _logger.LogError("UpdateTable : Could not build table with id {Id} for language {Language}", id, language);
            }
        }

        private TableInformation GetTableInformation(string id, TableLink tblLink, PXMeta meta)
        {
            TableInformation tbl = new TableInformation(id, tblLink.Text, GetCategory(tblLink), meta.GetFirstTimeValue(), meta.GetLastTimeValue(), (from v in meta.Variables select v.Name).ToArray());
            tbl.Source = meta.Source;
            tbl.TimeUnit = meta.GetTimeUnit();
            tbl.Description = tblLink.Description;
            tbl.SortCode = tblLink.SortCode;
            tbl.Updated = tblLink.LastUpdated;
            tbl.Discontinued = null; // TODO: Implement later

            return tbl;
        }

        private string GetCategory(TableLink tblLink)
        {
            switch (tblLink.Category)
            {
                case PresCategory.NotSet:
                    return "";
                case PresCategory.Official:
                    return "public";
                case PresCategory.Internal:
                    return "internal";
                case PresCategory.Private:
                    return "private";
                default:
                    return "";
            }
        }
    }
}
