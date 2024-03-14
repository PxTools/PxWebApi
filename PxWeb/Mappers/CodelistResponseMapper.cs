﻿using Microsoft.Extensions.Options;

using Px.Abstractions;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Mappers
{
    public class CodelistResponseMapper : ICodelistResponseMapper
    {
        private readonly ILinkCreator _linkCreator;
        private readonly PxApiConfigurationOptions _configOptions;
        private readonly string _language;

        public CodelistResponseMapper(ILinkCreator linkCreator, IOptions<PxApiConfigurationOptions> configOptions)
        {
            _linkCreator = linkCreator;
            _configOptions = configOptions.Value;
            _language = _configOptions.DefaultLanguage;
        }

        public CodeListResponse Map(Codelist codelist, string language)
        {
            string id = codelist.Id;

            if (codelist.CodelistType == Codelist.CodelistTypeEnum.Aggregation && !id.StartsWith("agg_"))
            {
                id = "agg_" + id;
            }
            else if (codelist.CodelistType == Codelist.CodelistTypeEnum.ValueSet && !id.StartsWith("vs_"))
            {
                id = "vs_" + id;
            }

            CodeListResponse codeListResponse = new CodeListResponse();
            codeListResponse.Id = id;
            codeListResponse.Label = codelist.Label;
            codeListResponse.Language = language;
            codeListResponse.Type = GetCodelistType(codelist.CodelistType);

            codeListResponse.Values = new System.Collections.Generic.List<ValueMap>();

            foreach (var val in codelist.Values)
            {
                codeListResponse.Values.Add(Map(val));
            }

            codeListResponse.Links = new System.Collections.Generic.List<Link>();

            // Links 
            codeListResponse.Links.Add(_linkCreator.GetCodelistLink(LinkCreator.LinkRelationEnum.self, id, language, true));

            return codeListResponse;
        }

        private CodeListType GetCodelistType(Codelist.CodelistTypeEnum type)
        {
            switch (type)
            {
                case Codelist.CodelistTypeEnum.ValueSet:
                    return CodeListType.ValuesetEnum;
                case Codelist.CodelistTypeEnum.Aggregation:
                    return CodeListType.AggregationEnum;
                default:
                    return CodeListType.AggregationEnum;
            }
        }

        private ValueMap Map(CodelistValue val)
        {
            ValueMap map = new ValueMap();

            map.Code = val.Code;
            map.Label = val.Label;

            map._ValueMap = new System.Collections.Generic.List<string>();

            foreach (var v in val.ValueMap)
            {
                map._ValueMap.Add(v.ToString());
            }

            // TODO: Add later?
            //map.Notes = new System.Collections.Generic.List<Note>();

            return map;
        }
    }
}
