using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Code.Api2.DataSelection
{
    public interface IPlacementHandler
    {
        VariablePlacementType? GetPlacment(VariablesSelection variablesSelection, Selection[] selection, PXMeta meta, out Problem? problem);
    }
}
