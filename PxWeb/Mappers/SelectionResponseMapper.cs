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

        public SelectionResponse Map(VariablesSelection selections, string tableId, string lang)
        {
            var response = new SelectionResponse();
            response.Selection = selections.Selection;

            response.Placement = selections.Placement;

            response.Links = new List<Link>();

            response.Links.Add(_linkCreator.GetDefaultSelectionLink(LinkCreator.LinkRelationEnum.self, tableId, lang, true));

            return response;
        }

    }


}
