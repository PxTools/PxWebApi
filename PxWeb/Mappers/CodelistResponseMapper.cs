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

        public CodelistResponse Map(Codelist codelist, string language)
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

            var codeListResponse = new CodelistResponse();
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
            foreach (string alternateLang in codelist.AvailableLanguages)
            {
                if (alternateLang != language)
                {
                    codeListResponse.Links.Add(_linkCreator.GetCodelistLink(LinkCreator.LinkRelationEnum.alternate, id, alternateLang, true));
                }
            }

            codeListResponse.Languages = codelist.AvailableLanguages;
            codeListResponse.Elimination = codelist.Elimination;
            codeListResponse.EliminationValueCode = codelist.EliminationValue;

            return codeListResponse;
        }

        private CodelistType GetCodelistType(Codelist.CodelistTypeEnum type)
        {
            switch (type)
            {
                case Codelist.CodelistTypeEnum.ValueSet:
                    return CodelistType.ValuesetEnum;
                case Codelist.CodelistTypeEnum.Aggregation:
                    return CodelistType.AggregationEnum;
                default:
                    return CodelistType.AggregationEnum;
            }
        }

        private ValueMap Map(CodelistValue val)
        {
            ValueMap map = new ValueMap();

            map.Code = val.Code;
            map.Label = val.Label;

            map.VarValueMap = new System.Collections.Generic.List<string>();

            foreach (var v in val.ValueMap)
            {
                map.VarValueMap.Add(v.ToString());
            }

            // TODO: Add later?
            //map.Notes = new System.Collections.Generic.List<Note>();

            return map;
        }
    }
}
