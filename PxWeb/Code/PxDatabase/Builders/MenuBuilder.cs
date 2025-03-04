﻿using System.Linq;
using System.Text;
using System.Xml.Linq;

using Microsoft.Extensions.Logging;

using PCAxis.Menu;
using PCAxis.Paxiom.Extensions;

using Px.Abstractions.Interfaces;
using Px.Search;

namespace PXWeb.Database
{
    class MenuBuilder : IDatabaseBuilder
    {
        private readonly Dictionary<string, PxMenuItem> _languageRoots = new Dictionary<string, PxMenuItem>();
        private readonly Dictionary<string, PxMenuItem> _currentItems = new Dictionary<string, PxMenuItem>();
        private readonly Dictionary<string, string> _matrixDict = new Dictionary<string, string>();
        private readonly string[] _languages;
        private Func<PCAxis.Paxiom.PXMeta, string, string> _sortOrder;
        private DatabaseLogger? _buildLogger;
        private readonly ILogger _logger;
        private readonly bool _languageDependent;
        private readonly Dictionary<PxMenuItem, List<string>> _links = new Dictionary<PxMenuItem, List<string>>();

        private readonly PxApiConfigurationOptions _configOptions;
        private readonly IPxHost _hostingEnvironment;


        public Func<PCAxis.Paxiom.PXMeta, string, string> SortOrder
        {
            get
            {
                return _sortOrder;
            }
            set
            {
                _sortOrder = value;
            }
        }

        /// <summary>
        /// Constructor for the MenuBuilder. This builder will create a Menu.xml file
        /// for all languages specified by the languages parameter
        /// </summary>
        /// <param name="languages">Languages that the database structure will be created for</param>
        /// <param name="languagDependent">If only file with the specific language should be included in the menu</param>
        public MenuBuilder(PxApiConfigurationOptions configOptions, ILogger logger, IPxHost hostingEnvironment, string[] languages, bool languageDependent)
        {
            _configOptions = configOptions;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _languages = languages;
            _languageDependent = languageDependent;
            _sortOrder = (meta, path) => System.IO.Path.GetFileName(path);
        }

        #region IMenuBuilder Members

        /// <summary>
        /// Creates all the root nodes for each language
        /// </summary>
        /// <param name="path"></param>
        public void BeginBuild(string path, DatabaseLogger logger)
        {
            _logger.LogInformation("Start building menu for {Path}", path);
            _buildLogger = logger;
            //TODO set use Date format
            _buildLogger(new DatabaseMessage() { MessageType = DatabaseMessage.BuilderMessageType.Information, Message = "Menu build started " + DateTime.Now.ToString() });
            string folderName = System.IO.Path.GetFileNameWithoutExtension(path);
            foreach (var language in _languages)
            {
                PxMenuItem root = new PxMenuItem(null, System.IO.Path.GetDirectoryName(path)?.Replace("\\", "/"), folderName, folderName, folderName, path, "");
                _languageRoots.Add(language, root);
                _currentItems.Add(language, root);
                _links.Add(root, new List<string>());
            }

        }

        /// <summary>
        /// Saves the Menu as a XML file
        /// </summary>
        /// <param name="path"></param>
        public void EndBuild(string path)
        {

            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "true"),
                new XElement("Menu",
                    from lang in _languages
                    select new XElement("Language",
                             new XAttribute("lang", lang),
                             new XAttribute("default", lang == _configOptions.DefaultLanguage),
                             (_languageRoots[lang].HasSubItems ? (XNode)_languageRoots[lang].SubItems[0].GetAsXML() : (XNode)new XComment("No items")))));


            // Sort the elements recursively on SortCode attrinute
            if (doc.Root != null)
            {
                SortElements(doc.Root);
            }

            try
            {
                doc.Save(System.IO.Path.Combine(path, "Menu.xml"));
                _logger.LogInformation("Finished building menu for {Path}", path);
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("Cannot create file {0}. {1}", path, e.Message);

                _logger.LogError(e, errorMessage);

                if (_buildLogger != null)
                {
                    _buildLogger(new DatabaseMessage()
                    {
                        MessageType = DatabaseMessage.BuilderMessageType.Error,
                        Message = errorMessage
                    });
                }

            }

            //TODO set use Date format
            if (_buildLogger != null)
            {
                _buildLogger(new DatabaseMessage()
                {
                    MessageType = DatabaseMessage.BuilderMessageType.Information,
                    Message = "Menu build ended " + DateTime.Now.ToString()
                });
            }

        }

        //Crude sorting, from ChatGPT.
        static void SortElements(XElement element)
        {
            if (!element.HasElements)
            { return; }
            var dsfsd = element.Name;

            var sortedChildren = element.Elements()
                                        .OrderBy(el => GetOrderByValue(el))
                                        .ToList();

            // Replace existing children with sorted ones
            element.ReplaceNodes(sortedChildren);

            // Recursively sort the children of each child
            foreach (var child in sortedChildren)
            {
                SortElements(child);
            }
        }

        static string GetOrderByValue(XElement element)
        {
            string myOut = "A"; //Elements like <Description> goes first.
            var sortOrder = element.Attribute("sortCode");
            if (sortOrder != null)
            {
                myOut = sortOrder.Value;
            }

            return myOut;
        }

        public void BeginNewLevel(string id)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(id);
            foreach (var language in _languages)
            {
                var idFromRoot = id.Substring(_hostingEnvironment.RootPath.Length + 1);

                ItemSelection cid = new ItemSelection(System.IO.Path.GetDirectoryName(idFromRoot)?.Replace("\\", "/"), idFromRoot);
                string sortOrderField = name;
                PxMenuItem newItem = new PxMenuItem(null, name, "", sortOrderField, cid.Menu, cid.Selection.Replace("\\", "/"), "");
                _currentItems[language].AddSubItem(newItem);
                _currentItems[language] = newItem;
                _links.Add(newItem, new List<string>());
            }
        }

        private bool HasLinks(PxMenuItem item)
        {
            for (int i = 0; i < item.SubItems.Count; i++)
            {
                Item itm = item.SubItems[i];
                if (itm is Link)
                {
                    return true;
                }
                else if (itm is PxMenuItem)
                {
                    if (HasLinks((PxMenuItem)itm))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void EndNewLevel(string id)
        {
            foreach (var language in _languages)
            {
                PxMenuItem item = _currentItems[language];
                if (item.Parent == null)
                {
                    _currentItems[language] = _languageRoots[language];
                }
                else
                {
                    _currentItems[language] = (PxMenuItem)item.Parent;
                }
                //there is no tables or other links remove node
                if (!HasLinks(item))
                {
                    _currentItems[language].SubItems.Remove(item);
                }
            }
        }

        public void NewItem(object item, string path)
        {
            if (item is AliasItem)
            {
                AliasItem alias = (AliasItem)item;
                if (Array.IndexOf(_languages, alias.Language) >= 0)
                {
                    if (_currentItems[alias.Language].Text == _currentItems[alias.Language].SortCode)
                    {
                        _currentItems[alias.Language].SortCode = alias.Alias;
                    }
                    _currentItems[alias.Language].Text = alias.Alias;

                }
            }
            else if (item is LinkItem)
            {
                LinkItem itm = (LinkItem)item;
                if (Array.IndexOf(_languages, itm.Language) >= 0)
                {
                    if (CheckIfLinkShallBeAdded(path, _currentItems[itm.Language]))
                    {
                        Url url = new Url(itm.Text, "", "MENU_TEST", itm.Location, "", PresCategory.NotSet, itm.Location, LinkPres.NotSet);
                        _currentItems[itm.Language].SubItems.Add(url);
                    }
                }
            }
            else if (item is MenuSortItem)
            {
                MenuSortItem sort = (MenuSortItem)item;
                if (Array.IndexOf(_languages, sort.Language) >= 0)
                {
                    _currentItems[sort.Language].SortCode = sort.SortString;
                }
            }
            else if (item is PCAxis.Paxiom.PXMeta)
            {
                PCAxis.Paxiom.PXMeta meta = (PCAxis.Paxiom.PXMeta)item;

                // Check if table with this MATRIX is already added
                if (_matrixDict.ContainsKey(meta.Matrix))
                {
                    if (_buildLogger != null)
                    {
                        _buildLogger(new DatabaseMessage()
                        {
                            MessageType = DatabaseMessage.BuilderMessageType.Information,
                            Message = "Duplicate MATRIX " + meta.Matrix + " adding " + path + " already exists at " + _matrixDict[meta.Matrix]
                        });
                    }
                }
                else
                {
                    _matrixDict.Add(meta.Matrix, path);
                }

                foreach (var language in _languages)
                {
                    if (meta.HasLanguage(language))
                    {
                        meta.SetLanguage(language);
                        TableLink tbl = CreateTableLink(meta, path);
                        _currentItems[language].AddSubItem(tbl);
                    }
                    else
                    {
                        if (!_languageDependent)
                        {
                            if (meta.HasLanguage(_configOptions.DefaultLanguage))
                            {
                                meta.SetLanguage(_configOptions.DefaultLanguage);
                            }
                            else
                            {
                                meta.SetLanguage("default");
                            }
                            TableLink tbl = CreateTableLink(meta, path);
                            _currentItems[language].AddSubItem(tbl);
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Checks if a link shall be added or not. If this check is not done a link may be added
        /// twice for the default language - which is wrong.
        /// </summary>
        /// <param name="path">Path to the .link-file</param>
        /// <param name="itm">MenuItem object that the link shall be added to</param>
        /// <returns></returns>
        private bool CheckIfLinkShallBeAdded(string path, PxMenuItem itm)
        {
            string filename = System.IO.Path.GetFileNameWithoutExtension(path);
            int index = filename.IndexOf("_");


            if ((index != -1) && (index > 0))
            {
                //Remove the language-specific part of the filename (for example: "_en" or "_sv")
                filename = filename.Substring(0, index);
            }

            if (_links[itm].Contains(filename))
            {
                return false;
            }
            else
            {
                _links[itm].Add(filename);
                return true;
            }
        }


        private TableLink CreateTableLink(PCAxis.Paxiom.PXMeta meta, string path)
        {
            ItemSelection cid = new ItemSelection(System.IO.Path.GetDirectoryName(path.Substring(_hostingEnvironment.RootPath.Length + 1))?.Replace("\\", "/"), path.Substring(_hostingEnvironment.RootPath.Length + 1));

            DateTime? initPublished = null;
            DateTime? initLastUpdated = null;

            TableLink tbl = new TableLink(!string.IsNullOrEmpty(meta.Description) ? meta.Description : meta.Title,
                meta.Matrix, _sortOrder(meta, path), cid.Menu, cid.Selection.Replace("\\", "/"), meta.Description ?? "", LinkType.PX,
                TableStatus.AccessibleToAll, initPublished, initLastUpdated, meta.GetFirstTimeValue(), meta.GetLastTimeValue(), meta.Matrix ?? "", PresCategory.Official);

            int cellCount = 1;
            for (int i = 0; i < meta.Variables.Count; i++)
            {
                tbl.SetAttribute("Var" + (i + 1) + "Name", meta.Variables[i].Name);
                tbl.SetAttribute("Var" + (i + 1) + "Values", GetNames(meta.Variables[i]));
                tbl.SetAttribute("Var" + (i + 1) + "NumberOfValues", meta.Variables[i].Values.Count.ToString());
                cellCount *= meta.Variables[i].Values.Count;
            }

            System.IO.FileInfo info = new System.IO.FileInfo(path);
            tbl.SetAttribute("size", info.Length);
            tbl.SetAttribute("cells", cellCount.ToString());

            if (meta.AutoOpen)
            {
                tbl.SetAttribute("autoOpen", "true");
            }

            // Store dates in the PC-Axis date format
            tbl.SetAttribute("updated", info.LastWriteTime.ToString(PCAxis.Paxiom.PXConstant.PXDATEFORMAT));
            tbl.SetAttribute("modified", GetLastModified(meta));

            string lastUpdated = GetLastModified(meta);
            if (PxDate.IsPxDate(lastUpdated))
            {
                tbl.LastUpdated = PxDate.PxDateStringToDateTime(lastUpdated);
            }
            tbl.Published = info.LastWriteTime;

            return tbl;
        }

        private static string GetLastModified(PCAxis.Paxiom.PXMeta meta)
        {
            string date = "";
            DateTime maxDate = DateTime.MinValue;
            if (meta.ContentVariable != null)
            {
                foreach (var value in meta.ContentVariable.Values)
                {
                    if (value.ContentInfo != null)
                    {
                        if (value.ContentInfo.LastUpdated != "")
                        {
                            DateTime d = GetDate(value.ContentInfo.LastUpdated);
                            maxDate = maxDate > d ? maxDate : d;
                        }
                    }
                }
                if (meta.ContentInfo != null)
                {
                    if (meta.ContentInfo.LastUpdated != "")
                    {
                        DateTime d = GetDate(meta.ContentInfo.LastUpdated);
                        maxDate = maxDate > d ? maxDate : d;
                    }
                }
                if (maxDate != DateTime.MinValue)
                {
                    // Store date in the PC-Axis date format
                    date = maxDate.ToString(PCAxis.Paxiom.PXConstant.PXDATEFORMAT);
                }
            }
            else
            {
                if (meta.ContentInfo != null)
                {
                    date = meta.ContentInfo.LastUpdated;
                }
            }

            if (string.IsNullOrEmpty(date))
            {
                date = "";
            }

            return date;
        }

        private static DateTime GetDate(string modified)
        {
            if (string.IsNullOrEmpty(modified)) return DateTime.MinValue;

            //supposed to have the format "CCYYMMDD hh:mm"
            if (modified.Length == 14)
            {
                try
                {
                    return new DateTime(Convert.ToInt32(modified.Substring(0, 4)), Convert.ToInt32(modified.Substring(4, 2)), Convert.ToInt32(modified.Substring(6, 2)), Convert.ToInt32(modified.Substring(9, 2)), Convert.ToInt32(modified.Substring(12, 2)), 0);
                }
                catch (Exception)
                {

                }
            }
            return DateTime.MinValue;
        }

        private static string GetNames(PCAxis.Paxiom.Variable variable)
        {
            StringBuilder names = new StringBuilder();

            if (variable.Values.Count <= 5)
            {
                // Gets the five first values of the variable in the form of
                // value 1, Value 2, value 3
                // if the varible have five or less values
                int limit = Math.Min(5, variable.Values.Count);
                for (int i = 0; i < limit; i++)
                {
                    names.Append(variable.Values[i].Value);
                    if (i < 4 && i <= (limit - 1))
                    {
                        names.Append(", ");
                    }
                }
            }
            else
            {
                // Gets the five first values of the variable in the form of
                // value 1, Value 2, value 3, value 4, ..., value N
                // if the varible have more than five values
                for (int i = 0; i < 4; i++)
                {
                    names.Append(variable.Values[i].Value);
                    names.Append(", ");
                }
                names.Append("..., ");
                names.Append(variable.Values[variable.Values.Count - 1].Value);
            }

            return names.ToString();
        }

        #region IDatabaseBuilder Members

        /// <summary>
        /// Return the priority of the builder which is set to 1
        /// </summary>
        public int Priority
        {
            get { return 1; }
        }

        #endregion
    }
}
