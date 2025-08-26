using PxWeb.Api2.Server.Models;

namespace PxWeb.Mappers
{
    public interface ISavedQueryResponseMapper
    {
        SavedQueryResponse Map(SavedQuery savedQuery);
    }
}
