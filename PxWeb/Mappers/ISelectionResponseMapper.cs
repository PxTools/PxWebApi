using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Mappers
{
    public interface ISelectionResponseMapper
    {
        SelectionResponse Map(Selection[] selections, PXMeta meta, string tableId, string lang);
    }
}
