using PxWeb.Api2.Server.Models;

namespace PxWeb.Mappers
{
    public class SavedQueryResponseMapper : ISavedQueryResponseMapper
    {
        private readonly ILinkCreator _linkCreator;

        public SavedQueryResponseMapper(ILinkCreator linkCreator)
        {
            _linkCreator = linkCreator;
        }
        public SavedQueryResponse Map(SavedQuery savedQuery)
        {
            var response = new SavedQueryResponse
            {
                Id = savedQuery.Id ?? "",
                Language = savedQuery.Language,
                SavedQuery = savedQuery
            };

            response.Links.Add(_linkCreator.GetSavedQueryLink(LinkCreator.LinkRelationEnum.self, response.Id, response.Language, true));

            return response;
        }
    }
}
