using System.Linq;

using Microsoft.Extensions.Options;

using Px.Search;

using PxWeb.Api2.Server.Models;
using PxWeb.Converters;

namespace PxWeb.Mappers
{
    public class TableResponseMapper : ITableResponseMapper
    {

        private readonly ILinkCreator _linkCreator;
        private readonly PxApiConfigurationOptions _configOptions;

        public TableResponseMapper(ILinkCreator linkCreator, IOptions<PxApiConfigurationOptions> configOptions)
        {
            _linkCreator = linkCreator;
            _configOptions = configOptions.Value;
        }
        public TableResponse Map(SearchResult searchResult, string lang)
        {
            var linkList = new List<Link>();

            // Links to table
            linkList.Add(_linkCreator.GetTableLink(LinkCreator.LinkRelationEnum.self, searchResult.Id.ToUpper(), lang, true));

            // Links to metadata
            linkList.Add(_linkCreator.GetTableMetadataJsonLink(LinkCreator.LinkRelationEnum.metadata, searchResult.Id.ToUpper(), lang, true));

            // Links to data
            linkList.Add(_linkCreator.GetTableDataLink(LinkCreator.LinkRelationEnum.data, searchResult.Id.ToUpper(), lang, true));

            TableResponse tableResponse = new TableResponse()
            {
                Id = searchResult.Id,
                Label = searchResult.Label,
                Description = searchResult.Description,
                Source = searchResult.Source,
                TimeUnit = Convert(searchResult.TimeUnit),
                Paths = Convert(searchResult.Paths),
                //Tags = item.Tags.ToList(), // TODO: Implement later
                Updated = searchResult.Updated,
                FirstPeriod = searchResult.FirstPeriod,
                LastPeriod = searchResult.LastPeriod,
                Category = EnumConverter.ToEnum<TableCategory>(searchResult.Category),
                Discontinued = searchResult.Discontinued,
                VariableNames = searchResult.VariableNames.ToList(),
                Links = linkList,
                Language = lang,
                SortCode = searchResult.SortCode ?? string.Empty,
                SubjectCode = searchResult.SubjectCode ?? string.Empty

            };
            return tableResponse;

        }

        public static TimeUnit Convert(string timeUnit)
        {
            return timeUnit switch
            {
                "A" => TimeUnit.AnnualEnum,
                "Q" => TimeUnit.QuarterlyEnum,
                "M" => TimeUnit.MonthlyEnum,
                "W" => TimeUnit.WeeklyEnum,
                _ => TimeUnit.OtherEnum
            };
        }

        public static List<List<PathElement>> Convert(List<Level[]> paths)
        {
            var list = new List<List<PathElement>>();
            foreach (var path in paths)
            {
                var p = new List<PathElement>();
                foreach (var level in path)
                {
                    p.Add(new PathElement() { Id = level.Code, Label = level.Text });
                }
                list.Add(p);
            }

            return list;
        }

    }
}
