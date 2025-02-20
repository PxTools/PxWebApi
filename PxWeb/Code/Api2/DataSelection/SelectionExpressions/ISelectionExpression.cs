using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Code.Api2.DataSelection.SelectionExpressions
{
    public interface ISelectionExpression
    {
        bool CanHandle(string expression);
        bool Verfiy(string expression);
        void AddToSelection(Variable variable, VariableSelection selection, string expression);
    }
}
