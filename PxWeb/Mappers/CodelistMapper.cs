using PCAxis.Paxiom;

using Px.Abstractions;

namespace PxWeb.Mappers
{
    public class CodelistMapper : ICodelistMapper
    {
        public Codelist Map(PCAxis.Paxiom.Grouping grouping)
        {
            Codelist codelist = new Codelist(grouping.ID, grouping.Name);
            codelist.CodelistType = Codelist.CodelistTypeEnum.Aggregation;
            codelist.Elimination = false;
            codelist.EliminationValue = null;

            foreach (Group group in grouping.Groups)
            {
                codelist.Values.Add(Map(group));
            }

            return codelist;
        }

        public Codelist Map(PCAxis.Sql.Models.Grouping grouping)
        {
            Codelist codelist = new Codelist(grouping.Id, grouping.Label);
            codelist.CodelistType = Codelist.CodelistTypeEnum.Aggregation;
            codelist.Elimination = false;
            codelist.EliminationValue = null;
            codelist.AvailableLanguages = grouping.AvailableLanguages;

            foreach (PCAxis.Sql.Models.GroupedValue group in grouping.Values)
            {
                codelist.Values.Add(Map(group));
            }

            return codelist;
        }

        public Codelist Map(PCAxis.Sql.Models.ValueSet valueset)
        {
            Codelist codelist = new Codelist(valueset.Id, valueset.Label);
            codelist.CodelistType = Codelist.CodelistTypeEnum.ValueSet;
            codelist.Elimination = valueset.Elimination;
            codelist.EliminationValue = valueset.EliminationValueCode;
            codelist.AvailableLanguages = valueset.AvailableLanguages;

            foreach (var value in valueset.Values)
            {
                codelist.Values.Add(MapValuesetValue(value));
            }

            return codelist;
        }

        private CodelistValue Map(PCAxis.Paxiom.Group group)
        {
            CodelistValue codelistValue = new CodelistValue(group.GroupCode, group.Name);

            foreach (GroupChildValue child in group.ChildCodes)
            {
                codelistValue.ValueMap.Add(child.Code);
            }

            return codelistValue;
        }

        private CodelistValue Map(PCAxis.Sql.Models.GroupedValue group)
        {
            CodelistValue codelistValue = new CodelistValue(group.Code, group.Text);

            foreach (string code in group.Codes)
            {
                codelistValue.ValueMap.Add(code);
            }

            return codelistValue;
        }

        private CodelistValue MapValuesetValue(PCAxis.Sql.Models.Value value)
        {
            CodelistValue codelistValue = new CodelistValue(value.Code, value.Text);
            codelistValue.ValueMap.Add(value.Code);

            return codelistValue;
        }
    }
}
