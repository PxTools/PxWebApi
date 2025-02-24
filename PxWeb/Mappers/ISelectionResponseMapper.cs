using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Mappers
{
    public interface ISelectionResponseMapper
    {
        SelectionResponse Map(Selection[] selections, List<string> heading, List<string> stub, PXMeta meta, string tableId, string lang);

        SelectionResponse Map(VariablesSelection selections, string tableId, string lang);
    }
}
