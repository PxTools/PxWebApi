﻿using System.Text.Json;

namespace Px.Search.Lucene
{

    public class LuceneIndex : IIndex
    {
        private readonly string _indexDirectoryBase = "";
        private string _indexDirectoryCurrent = "";
        private IndexWriter? _writer;
        private IndexReader? _reader;
        private IndexSearcher? _indexSearcher;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="indexDirectory">Index directory</param>
        public LuceneIndex(string indexDirectory)
        {
            if (string.IsNullOrWhiteSpace(indexDirectory))
            {
                throw new ArgumentNullException("Index directory not defined for Lucene");
            }

            _indexDirectoryBase = indexDirectory;
        }

        /// <summary>
        /// Get Lucene.Net IndexWriter object
        /// </summary>
        /// <param name="create">
        /// If true, the existing index will be overwritten
        /// If false, the existing index will be appended
        /// </param>
        /// <returns>IndexWriter object. If the Index directory is locked, null is returned</returns>
        private IndexWriter CreateIndexWriter(bool create, string language)
        {
            FSDirectory fsDir = FSDirectory.Open(_indexDirectoryCurrent);
            if (IndexWriter.IsLocked(fsDir))
            {
                throw new Exception("Could not create IndexWriter. Index directory may be locked by another IndexWriter");
            }

            Analyzer analyzer = LuceneAnalyzer.GetAnalyzer(language);

            IndexWriterConfig config = new IndexWriterConfig(LuceneAnalyzer.luceneVersion, analyzer)
            {
                // Overwrite or append existing index
                OpenMode = create ? OpenMode.CREATE : OpenMode.CREATE_OR_APPEND
            };

            IndexWriter writer = new IndexWriter(fsDir, config);

            return writer;
        }

        private void CreateIndexReader()
        {
            try
            {
                FSDirectory fsDir = FSDirectory.Open(_indexDirectoryCurrent);
                IndexReader reader = DirectoryReader.Open(fsDir);
                _indexSearcher = new IndexSearcher(reader);
                _reader = reader;
            }
            catch (Exception e)
            {
                _reader = null;
                Console.WriteLine(e.ToString());
            }
        }

        public void BeginUpdate(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                throw new ArgumentNullException("Language not specified");
            }

            _indexDirectoryCurrent = Path.Combine(_indexDirectoryBase, language);
            _writer = CreateIndexWriter(false, language);
            CreateIndexReader();

            if (_writer == null)
            {
                throw new Exception("Could not create IndexWriter. Index directory may be locked by another IndexWriter");
            }
        }

        public void BeginWrite(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                throw new ArgumentNullException("Language not specified");
            }

            _indexDirectoryCurrent = Path.Combine(_indexDirectoryBase, language);
            _writer = CreateIndexWriter(true, language);

            if (_writer == null)
            {
                throw new Exception("Could not create IndexWriter. Index directory may be locked by another IndexWriter");
            }
        }

        public void EndUpdate(string language)
        {
            if (_reader is not null)
            {
                _reader.Dispose();
                _indexSearcher = null;
            }
            EndWrite(language);
        }

        public void EndWrite(string language)
        {
            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }
        }

        public void AddEntry(TableInformation tbl, PXMeta meta)
        {
            Document doc = GetDocument(tbl, meta);
            if (_writer != null)
            {
                _writer.AddDocument(doc);
            }
        }

        public void UpdateEntry(TableInformation tbl, PXMeta meta)
        {
            var oldDoc = FindDocument(tbl.Id);
            Document doc = GetDocument(tbl, meta);

            if (oldDoc is not null)
            {
                var paths = oldDoc.GetBinaryValue(SearchConstants.SEARCH_FIELD_PATHS);
                if (paths is not null)
                {
                    doc.Add(new StoredField(SearchConstants.SEARCH_FIELD_PATHS, paths));
                }
                return;
            }
            if (_writer != null)
            {
                _writer.UpdateDocument(new Term(SearchConstants.SEARCH_FIELD_DOCID, doc.Get(SearchConstants.SEARCH_FIELD_DOCID)), doc);
            }
        }

        public void RemoveEntry(string id)
        {
            //check if document exists, if true deletes existing
            var searchQuery = new TermQuery(new Term(SearchConstants.SEARCH_FIELD_DOCID, id));
            if (_writer != null)
            {
                _writer.DeleteDocuments(searchQuery);
            }
        }


        /// <summary>
        /// Get Document object representing the table
        /// </summary>
        /// <param name="tbl">TableInformation object</param>
        /// <param name="meta">PxMeta object</param>
        /// <returns>Document object representing the table</returns>
        private Document GetDocument(TableInformation tbl, PXMeta meta)
        {
            Document doc = new Document();
            DateTime updated2;
            string strUpdated = "";

            if (tbl != null && meta != null)
            {
                if (string.IsNullOrEmpty(tbl.Label) || string.IsNullOrEmpty(meta.Matrix) || meta.Variables.Count == 0)
                {
                    return doc;
                }

                if (tbl.Updated != null)
                {
                    updated2 = tbl.Updated.Value;
                    strUpdated = DateTools.DateToString(updated2, DateResolution.SECOND);
                }

                doc.Add(new StringField(SearchConstants.SEARCH_FIELD_DOCID, tbl.Id, Field.Store.YES)); // Used as id when updating a document - NOT searchable!!!
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_SEARCHID, tbl.Id, Field.Store.NO)); // Used for finding a document by id - will be used for generating URL from just the tableid - Searchable!!!
                doc.Add(new StringField(SearchConstants.SEARCH_FIELD_UPDATED, strUpdated, Field.Store.YES));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_MATRIX, meta.Matrix, Field.Store.YES));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_TITLE, tbl.Label, Field.Store.YES));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_DESCRIPTION, tbl.Description, Field.Store.YES));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_SORTCODE, tbl.SortCode, Field.Store.YES));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_CATEGORY, tbl.Category, Field.Store.YES));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_FIRSTPERIOD, tbl.FirstPeriod, Field.Store.YES));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_LASTPERIOD, tbl.LastPeriod, Field.Store.YES));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_VARIABLES, string.Join("|", tbl.VariableNames), Field.Store.YES));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_PERIOD, meta.GetTimeValues(), Field.Store.NO));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_VALUES, meta.GetAllValues(), Field.Store.NO));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_CODES, meta.GetAllCodes(), Field.Store.NO));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_GROUPINGS, meta.GetAllGroupings(), Field.Store.NO));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_GROUPINGCODES, meta.GetAllGroupingCodes(), Field.Store.NO));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_VALUESETS, meta.GetAllValuesets(), Field.Store.NO));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_VALUESETCODES, meta.GetAllValuesetCodes(), Field.Store.NO));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_DISCONTINUED, tbl.Discontinued == null ? "Unknown" : tbl.Discontinued.ToString(), Field.Store.YES));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_TAGS, GetAllTags(tbl.Tags), Field.Store.YES));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_SOURCE, tbl.Source, Field.Store.YES));
                doc.Add(new TextField(SearchConstants.SEARCH_FIELD_TIME_UNIT, tbl.TimeUnit, Field.Store.YES));
                doc.Add(new StoredField(SearchConstants.SEARCH_FIELD_PATHS, GetBytes(tbl.Paths)));
                if (!string.IsNullOrEmpty(meta.Synonyms))
                {
                    doc.Add(new TextField(SearchConstants.SEARCH_FIELD_SYNONYMS, meta.Synonyms, Field.Store.NO));
                }
            }

            return doc;
        }

        public Document? FindDocument(string tableId)
        {
            if (_indexSearcher is null)
            {
                return null;
            }

            string[] field = new[] { SearchConstants.SEARCH_FIELD_SEARCHID };
            LuceneVersion luceneVersion = LuceneAnalyzer.luceneVersion;
            Query luceneQuery;
            QueryParser queryParser = new MultiFieldQueryParser(luceneVersion,
                                                       field,
                                                       new StandardAnalyzer(luceneVersion));
            luceneQuery = queryParser.Parse(tableId);

            TopDocs topDocs = _indexSearcher.Search(luceneQuery, 1);
            if (topDocs.TotalHits == 0)
            {
                return null;
            }

            return _indexSearcher.Doc(topDocs.ScoreDocs[0].Doc);
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Rollback();
                _writer = null;
            }
        }

        public static string GetAllTags(string[] tags)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string tag in tags)
            {
                builder.Append(tag);
                builder.Append(' ');
            }
            return builder.ToString();
        }

        private static byte[] GetBytes(List<Level[]> paths)
        {
            string jsonString = JsonSerializer.Serialize(paths);
            return Encoding.UTF8.GetBytes(jsonString);
        }
    }
}
