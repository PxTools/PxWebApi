using System.Linq;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Mappers
{
    public class SelectionResponseMapper : ISelectionResponseMapper
    {

        private readonly ILinkCreator _linkCreator;

        public SelectionResponseMapper(ILinkCreator linkCreator)
        {
            _linkCreator = linkCreator;
        }

        public SelectionResponse Map(Selection[] selections, PXMeta meta, string tableId, string lang)
        {
            var response = new SelectionResponse();
            response.Selection = new VariablesSelection();
            response.Selection.Selection = selections.Select(selection => new VariableSelection()
            {
                VariableCode = selection.VariableCode,
                CodeList = GetCodeList(meta.Variables.FirstOrDefault(v => string.Compare(v.Code, selection.VariableCode, true) == 0)),
                OutputValues = CodeListOutputValuesType.SingleEnum,
                ValueCodes = selection.ValueCodes.Cast<string>().ToList(),
            }).ToList();

            response.Links = new List<Link>();

            response.Links.Add(_linkCreator.GetDefaultSelectionLink(LinkCreator.LinkRelationEnum.self, tableId, lang, true));

            return response;
        }

        private static string? GetCodeList(Variable? variable)
        {
            if (variable == null)
            {
                return null;
            }

            if (variable.CurrentGrouping != null)
            {
                return variable.CurrentGrouping.ID;
            }

            if (variable.CurrentValueSet != null)
            {
                return variable.CurrentValueSet.ID;
            }

            return null;

        }
    }


}
