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

        public SelectionResponse Map(VariablesSelection selections, string tableOrSavedQueryId, string lang, bool fromSavedQuery)
        {
            var response = new SelectionResponse();
            response.Selection = selections.Selection;

            response.Placement = selections.Placement;

            response.Links = new List<Link>();

            if (fromSavedQuery)
            {
                response.Links.Add(_linkCreator.GetSavedQuerySelectionLink(LinkCreator.LinkRelationEnum.self, tableOrSavedQueryId, lang, true));
            }
            else
            {
                response.Links.Add(_linkCreator.GetDefaultSelectionLink(LinkCreator.LinkRelationEnum.self, tableOrSavedQueryId, lang, true));
            }
            return response;
        }

    }


}
