﻿using Microsoft.Extensions.Options;
using Px.Search;
using PxWeb.Api2.Server.Models;
using PxWeb.Config.Api2;
using PxWeb.Converters;
using System.Collections.Generic;
using System.Linq;

namespace PxWeb.Mappers
{
    public class TableResponseMapper : ITableResponseMapper
    {

        private ILinkCreator _linkCreator;
        private PxApiConfigurationOptions _configOptions;

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

                Type = FolderContentItemTypeEnum.TableEnum,
                Id = searchResult.Id,
                Label = searchResult.Label,
                Description = searchResult.Description,
                //Tags = item.Tags.ToList(), // TODO: Implement later
                Updated = searchResult.Updated,
                FirstPeriod = searchResult.FirstPeriod,
                LastPeriod = searchResult.LastPeriod,
                Category = EnumConverter.ToCategoryEnum(searchResult.Category),
                Discontinued = searchResult.Discontinued,
                VariableNames = searchResult.VariableNames.ToList(),
                Links = linkList,
                Language = lang,
                SortCode = searchResult.SortCode ?? string.Empty

            };
            return tableResponse;

        }

    }
}
