using System.Linq;

using Microsoft.Extensions.Options;

using Px.Search;

using PxWeb.Api2.Server.Models;
using PxWeb.Converters;

namespace PxWeb.Mappers
{
    public class TablesResponseMapper : ITablesResponseMapper
    {
        private readonly ILinkCreator _linkCreator;
        private readonly PxApiConfigurationOptions _configOptions;
        public TablesResponseMapper(ILinkCreator linkCreator, IOptions<PxApiConfigurationOptions> configOptions)
        {
            _linkCreator = linkCreator;
            _configOptions = configOptions.Value;
        }

        public TablesResponse Map(SearchResultContainer searchResultContainer, string lang, string? query, int? pastDays)
        {
            var tablesResponse = new TablesResponse();
            var linkPageList = new List<Link>();
            var pageNumber = searchResultContainer.pageNumber;
            var pageSize = searchResultContainer.pageSize;
            var totalElements = searchResultContainer.totalElements;
            var totalPages = searchResultContainer.totalPages;



            if (pageNumber < totalPages)
            {
                // Links to next page 
                linkPageList.Add(_linkCreator.GetTablesLink(LinkCreator.LinkRelationEnum.next, lang, query, pastDays, pageSize, pageNumber + 1, true));
            }

            if (pageNumber <= totalPages && pageNumber != 1)
            {
                // Links to previous page 
                linkPageList.Add(_linkCreator.GetTablesLink(LinkCreator.LinkRelationEnum.previous, lang, query, pastDays, pageSize, pageNumber - 1, true));
            }

            if (totalPages > 1)
            {
                // Links to last page 
                linkPageList.Add(_linkCreator.GetTablesLink(LinkCreator.LinkRelationEnum.last, lang, query, pastDays, pageSize, totalPages, true));
            }

            PageInfo page = new PageInfo
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalElements = totalElements,
                TotalPages = totalPages == 0 ? totalPages + 1 : totalPages,
                Links = linkPageList
            };

            var tableList = new List<Table>();

            tablesResponse.Page = page;
            tablesResponse.Language = lang;

            foreach (var item in searchResultContainer.searchResults)
            {
                var linkList = new List<Link>();

                // Links to table
                linkList.Add(_linkCreator.GetTableLink(LinkCreator.LinkRelationEnum.self, item.Id.ToUpper(), lang, true));

                // Links to table in other languages
                foreach (var language in _configOptions.Languages)
                {
                    if (language.Id != lang && item.Languages.Contains(language.Id))
                    {
                        linkList.Add(_linkCreator.GetTableLink(LinkCreator.LinkRelationEnum.alternate, item.Id.ToUpper(), language.Id, true));
                    }
                }

                // Links to metadata
                linkList.Add(_linkCreator.GetTableMetadataJsonLink(LinkCreator.LinkRelationEnum.metadata, item.Id.ToUpper(), lang, true));

                // Links to data
                linkList.Add(_linkCreator.GetTableDataLink(LinkCreator.LinkRelationEnum.data, item.Id.ToUpper(), lang, true));

                var tb = new Table()
                {
                    Type = FolderContentItemTypeEnum.TableEnum,
                    Id = item.Id,
                    Label = item.Label,
                    Source = item.Source,
                    TimeUnit = TableResponseMapper.Convert(item.TimeUnit),
                    Paths = TableResponseMapper.Convert(item.Paths),
                    Description = item.Description,
                    //Tags = item.Tags.ToList(), // TODO: Implement later
                    Updated = item.Updated,
                    FirstPeriod = item.FirstPeriod,
                    LastPeriod = item.LastPeriod,
                    Category = EnumConverter.ToCategoryEnum(item.Category),
                    Discontinued = item.Discontinued,
                    VariableNames = item.VariableNames.ToList(),
                    Links = linkList,
                    SubjectCode = item.SubjectCode ?? string.Empty,
                };
                tableList.Add(tb);
            }

            tablesResponse.Tables = tableList;

            var linkListTableResponse = new List<Link>();

            // Links to tablesResponse
            linkListTableResponse.Add(_linkCreator.GetTablesLink(LinkCreator.LinkRelationEnum.self, lang, query, pastDays, page.PageSize, page.PageNumber, true));

            tablesResponse.Links = linkListTableResponse;

            return tablesResponse;
        }


    }
}
