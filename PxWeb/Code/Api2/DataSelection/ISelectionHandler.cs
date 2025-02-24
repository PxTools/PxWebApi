using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Code.Api2.DataSelection
{
    public interface ISelectionHandler
    {
        //Selection[]? GetSelection(IPXModelBuilder builder, VariablesSelection variablesSelection, out Problem? problem);
        bool ExpandAndVerfiySelections(VariablesSelection variablesSelection, IPXModelBuilder builder, out Problem? problem);
        Selection[] Convert(VariablesSelection variablesSelection);
    }
}
