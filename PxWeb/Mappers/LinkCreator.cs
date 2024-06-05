using System.Text;

using Microsoft.Extensions.Options;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Mappers
{
    public class LinkCreator : ILinkCreator
    {
        public enum LinkRelationEnum
        {
            self,
            data,
            describedby,
            metadata,
            next,
            previous,
            last
        }

        private readonly string _urlBase;
        private readonly string _defaultDataFormat;
        private readonly List<string> _metaFormats = new List<string> { "json-px", "json-stat2" };
        //Could not get the strings cleanly from MetadataOutputFormatType. Anybody?

        public LinkCreator(IOptions<PxApiConfigurationOptions> configOptions)
        {
            _urlBase = configOptions.Value.BaseURL;
            _defaultDataFormat = configOptions.Value.DefaultOutputFormat;
        }
        public Link GetTablesLink(LinkRelationEnum relation, string language, string? query, int pagesize, int pageNumber, bool showLangParam = true)
        {
            var link = new Link();
            link.Rel = relation.ToString();
            link.Hreflang = language;
            link.Href = CreatePageURL($"tables/", language, showLangParam, query, pagesize, pageNumber);

            return link;
        }

        public Link GetTableLink(LinkRelationEnum relation, string id, string language, bool showLangParam = true)
        {
            var link = new Link();
            link.Rel = relation.ToString();
            link.Hreflang = language;
            link.Href = CreateURL($"tables/{id}", language, showLangParam, null);

            return link;
        }

        public List<Link> GetTableMetadataJsonLink(LinkRelationEnum relation, string id, string language, bool showLangParam = true)
        {
            List<Link> links = new List<Link>();

            foreach (string outFormat in _metaFormats)
            {
                var link = new Link();
                link.Rel = relation.ToString();
                link.Hreflang = language;

                link.Href = CreateURL($"tables/{id}/metadata", language, showLangParam, outFormat);
                links.Add(link);
            }

            return links;
        }

        public Link GetTableDataLink(LinkRelationEnum relation, string id, string language, bool showLangParam = true)
        {
            var link = new Link();
            link.Rel = relation.ToString();
            link.Hreflang = language;
            link.Href = CreateURL($"tables/{id}/data", language, showLangParam, _defaultDataFormat);

            return link;
        }

        public Link GetCodelistLink(LinkRelationEnum relation, string id, string language, bool showLangParam = true)
        {
            var link = new Link();
            link.Rel = relation.ToString();
            link.Hreflang = language;
            link.Href = CreateURL($"codeLists/{id}", language, showLangParam, null);

            return link;
        }

        public Link GetDefaultSelectionLink(LinkRelationEnum relation, string id, string language, bool showLangParam = true)
        {
            var link = new Link();
            link.Rel = relation.ToString();
            link.Hreflang = language;
            link.Href = CreateURL($"tables/{id}/defaultselection", language, showLangParam, null);

            return link;
        }

        public Link GetFolderLink(LinkRelationEnum relation, string id, string language, bool showLangParam = true)
        {
            var link = new Link();
            link.Rel = relation.ToString();
            link.Hreflang = language;
            link.Href = CreateURL($"navigation/{id}", language, showLangParam, null);

            return link;
        }
        private string CreateURL(string endpointUrl, string language, bool showLangParam, string? outputFormat)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_urlBase);
            sb.Append("/");
            sb.Append(endpointUrl);

            if (showLangParam)
            {
                sb.Append("?lang=");
                sb.Append(language);
            }

            if (!string.IsNullOrEmpty(outputFormat))
            {
                if (showLangParam)
                {
                    sb.Append('&');
                }
                else
                {
                    sb.Append('?');
                }
                sb.Append("outputFormat=");
                sb.Append(outputFormat);
            }

            return sb.ToString();
        }
        private string CreatePageURL(string endpointUrl, string language, bool showLangParam, string? query, int pagesize, int pageNumber)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_urlBase);
            sb.Append("/");
            sb.Append(endpointUrl);

            if (!string.IsNullOrEmpty(query) && showLangParam)
            {
                sb.Append("?lang=");
                sb.Append(language);
                sb.Append("&query=" + query);
                sb.Append("&pagesize=" + pagesize);
            }
            if (!string.IsNullOrEmpty(query) && !showLangParam)
            {
                sb.Append("?");
                sb.Append("query=" + query);
                sb.Append("&pagesize=" + pagesize);
            }
            if (string.IsNullOrEmpty(query) && !showLangParam)
            {
                sb.Append("?");
                sb.Append("pagesize=" + pagesize);
            }
            if (string.IsNullOrEmpty(query) && showLangParam)
            {
                sb.Append("?lang=");
                sb.Append(language);
                sb.Append("&pagesize=" + pagesize);
            }

            sb.Append("&pageNumber=" + pageNumber);

            return sb.ToString();
        }

    }
}
