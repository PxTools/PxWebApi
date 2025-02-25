
using PxWeb.Api2.Server.Models;

namespace PxWeb.Code.Api2.DataSelection
{
    public class QueryHandler
    {
        public SavedQuery GetSavedQuery(string id)
        {
            throw new System.NotImplementedException();
        }

        public VariablesSelection GetDefaultSelection(string tableId)
        {
            //TODO: Implement
            // 1. Check if there is a saved query for the table
            // 2. If there is a saved query, return the selection of the saved query
            // 3. Use the algorithm for the table to get the default selection

            throw new System.NotImplementedException();
        }

    }
}
