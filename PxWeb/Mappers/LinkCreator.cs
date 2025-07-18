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
            alternate,
            data,
            describedby,
            metadata,
            next,
            previous,
            last
        }

        private readonly string _urlPrefix;
        private readonly string _defaultDataFormat;

        public LinkCreator(IOptions<PxApiConfigurationOptions> configOptions)
        {
            _urlPrefix = configOptions.Value.BaseURL + configOptions.Value.RoutePrefix;
            _defaultDataFormat = configOptions.Value.DefaultOutputFormat;
        }
        public Link GetTablesLink(LinkRelationEnum relation, string language, string? query, int? pastDays, int pagesize, int pageNumber, bool showLangParam = true)
        {
            var link = new Link();
            link.Rel = relation.ToString();
            link.Hreflang = language;
            link.Href = CreatePageURL($"tables/", language, showLangParam, query, pastDays, pagesize, pageNumber);

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

        public Link GetTableMetadataJsonLink(LinkRelationEnum relation, string id, string language, bool showLangParam = true)
        {
            Link link = new Link();
            link.Rel = relation.ToString();
            link.Hreflang = language;
            link.Href = CreateURL($"tables/{id}/metadata", language, showLangParam, null);

            return link;
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

            sb.Append(_urlPrefix);
            sb.Append('/');
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
        private string CreatePageURL(string endpointUrl, string language, bool showLangParam, string? query, int? pastDays, int pagesize, int pageNumber)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_urlPrefix);
            sb.Append('/');
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
                sb.Append('?');
                sb.Append("query=" + query);
                sb.Append("&pagesize=" + pagesize);
            }
            if (string.IsNullOrEmpty(query) && !showLangParam)
            {
                sb.Append('?');
                sb.Append("pagesize=" + pagesize);
            }
            if (string.IsNullOrEmpty(query) && showLangParam)
            {
                sb.Append("?lang=");
                sb.Append(language);
                sb.Append("&pagesize=" + pagesize);
            }

            sb.Append("&pageNumber=" + pageNumber);

            if (pastDays is not null)
            {
                sb.Append("&pastDays=" + pastDays.Value);
            }

            return sb.ToString();
        }

    }
}
