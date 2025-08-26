using PxWeb.Api2.Server.Models;

namespace PxWeb.Mappers
{
    public interface ISelectionResponseMapper
    {
        SelectionResponse Map(VariablesSelection selections, string tableId, string lang, bool fromSavedQuery);
    }
}
