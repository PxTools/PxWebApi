using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Code.Api2
{
    public interface IDataWorkflow
    {
        PXModel? Run(string tableId, string language, VariablesSelection variablesSelection, out Problem? problem);
    }
}
