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
        private readonly Dictionary<string, List<Level[]>> _breadcrumbs;

        public Indexer(IDataSource dataSource, ISearchBackend backend, ILogger logger)
        {
            _source = dataSource;
            _backend = backend;
            _logger = logger;
            _indexedTables = new List<string>();
            _breadcrumbs = new Dictionary<string, List<Level[]>>();
        }

        /// <summary>
        /// Creates or recreates a search index for the database
        /// </summary>
        /// <param name="languages">list of languages codes that the search index will be able to be searched for</param>
        public void IndexDatabase(List<string> languages)
        {
            using (var index = _backend.GetIndex())
            {
                foreach (var language in languages)
                {

                    index.BeginWrite(language);
                    _indexedTables = new List<string>();
                    _logger.LogIndexingStarted(language);

                    //Get the root item from the database
                    var item = _source.LoadDatabaseStructure(language);


                    if (item == null)
                    {
                        _logger.LogNoRootLevel();
                        return;
                    }

                    if (item is PxMenuItem)
                    {
                        var path = new List<Level>();
                        _breadcrumbs.Clear();
                        GenerateBreadcrumbs(item, language, index, path);
                        TraverseDatabase(item, language, index);
                    }

                    _logger.LogIndexingEnded(language, _indexedTables.Count);
                    index.EndWrite(language);
                }
            }
        }

        private void GenerateBreadcrumbs(Item? item, string language, IIndex index, List<Level> path)
        {

            try
            {
                if (item is null)
                {
                    return;
                }

                if (item is PxMenuItem menuItem)
                {
                    foreach (var subitem in menuItem.SubItems)
                    {
                        if (subitem is null)
                        {
                            continue;
                        }
                        if (subitem is PxMenuItem)
                        {
                            path.Add(new Level(subitem.ID.Selection, subitem.Text));
                            GenerateBreadcrumbs(subitem, language, index, path);
                            path.RemoveAt(path.Count - 1);
                        }
                        else if (subitem is TableLink tblLink)
                        {
                            AddBreadcrumbPath(path, tblLink);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCouldNotCreateBreadcrumb(language, ex);
            }

        }

        private void AddBreadcrumbPath(List<Level> path, TableLink tblLink)
        {
            if (!_breadcrumbs.TryGetValue(tblLink.TableId, out var paths))
            {
                paths = new List<Level[]>();
                _breadcrumbs.Add(tblLink.TableId, paths);
            }
            paths.Add(path.ToArray());
        }


        /// <summary>
        /// Traverses the database and looks for tables to add in the index.
        /// </summary>
        /// <param name="id">current node id</param>
        /// <param name="language">current processing language</param>
        /// <param name="index">the index to use</param>
        private void TraverseDatabase(Item? item, string language, IIndex index)
        {

            if (item == null)
            {
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
                        TraverseDatabase(subitem, language, index);
                    }
                    else if (subitem is TableLink)
                    {
                        AddTableToIndex(language, index, (TableLink)subitem);
                    }
                }
            }

        }

        private void AddTableToIndex(string language, IIndex index, TableLink tableLink)
        {
            string tableId = tableLink.TableId;
            if (!_indexedTables.Contains(tableId))
            {
                IndexTable(tableId, tableLink, language, index);

                _indexedTables.Add(tableId);
                if (_indexedTables.Count % 100 == 0)
                {
                    _logger.LogProgression(_indexedTables.Count);
                }
            }
            else
            {
                _logger.LogTableAlreadyIndex(tableId);
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

        private void IndexTable(string id, TableLink tblLink, string language, IIndex index)
        {
            IPXModelBuilder? builder = _source.CreateBuilder(id, language);

            if (builder != null)
            {
                try
                {
                    builder.BuildForSelection();
                    var model = builder.Model;
                    TableInformation tbl = GetTableInformation(id, tblLink, model.Meta);
                    tbl.Paths = _breadcrumbs[id];

                    index.AddEntry(tbl, model.Meta);
                }
                catch (Exception ex)
                {
                    _logger.LogCouldNotBuildTable(id, language, ex);
                }
            }
            else
            {
                _logger.LogCouldNotCreateBuilder(id, language);
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
                    _logger.LogCouldNotBuildTable(id, language, ex);
                }
            }
            else
            {
                _logger.LogCouldNotCreateBuilder(id, language);
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
            tbl.SubjectCode = meta.SubjectCode ?? string.Empty;

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
