using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Code.Api2.DataSelection
{
    public interface IDefaultSelectionAlgorithm
    {
        VariablesSelection GetDefaultSelection(IPXModelBuilder builder);
    }
}
