using System.Linq;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;
using PxWeb.Helper.Api2;

namespace PxWeb.Code.Api2.DataSelection
{
    public class PlacementHandler : IPlacementHandler
    {
        VariablePlacementType? IPlacementHandler.GetPlacment(VariablesSelection variablesSelection, Selection[] selection, PXMeta meta, out Problem? problem)
        {
            var p = variablesSelection.Palcement;

            //No placement is specified
            if (p is null)
            {
                problem = null;
                return null;
            }

            //If not placement is specified, return null
            if (p.Heading is null || p.Stub is null)
            {
                problem = null;
                return null;
            }

            //If not placement is specified, return null
            if (p.Heading.Count == 0 && p.Stub.Count == 0)
            {
                problem = null;
                return null;
            }

            //Replace the text TIME with tid in list
            ReplaceTimeConstant(meta, p);

            var selectedVariablesCode = selection.Where(x => x.ValueCodes.Count > 0).Select(x => x.VariableCode).ToList();

            //Check if all variables are in the model
            if (!AllVariablesExistsInSelection(p, selectedVariablesCode))
            {
                problem = ProblemUtility.IllegalPlacementSelection();
                return null;
            }


            //Check if all variables have a placement
            if (p.Heading.Count + p.Stub.Count == selectedVariablesCode.Count)
            {
                //Check for duplicates
                if (p.Heading.Intersect(p.Stub).Any())
                {
                    problem = ProblemUtility.IllegalPlacementSelection();
                    return null;
                }

                problem = null;
                return p;
            }

            //Check if user have specified stub or heading
            if (OnlyHeadOrStubIsSpecified(p))
            {

                List<string> usedVariables = new List<string>();
                usedVariables.AddRange(p.Heading);
                usedVariables.AddRange(p.Stub);
                var unusedVariables = selectedVariablesCode.Except(usedVariables, StringComparer.OrdinalIgnoreCase).ToList();

                if (p.Heading.Count == 0)
                {
                    problem = null;
                    return new VariablePlacementType() { Heading = unusedVariables, Stub = p.Stub };
                }
                else
                {
                    problem = null;
                    return new VariablePlacementType() { Heading = p.Heading, Stub = unusedVariables };
                }
            }

            problem = ProblemUtility.IllegalPlacementSelection();
            return null;

        }

        private static bool OnlyHeadOrStubIsSpecified(VariablePlacementType p)
        {
            return (p.Heading.Count > 0 && p.Stub.Count == 0) ||
                             (p.Heading.Count == 0 && p.Stub.Count > 0);
        }

        private static bool AllVariablesExistsInSelection(VariablePlacementType p, List<string> selectedVariablesCode)
        {
            return (p.Stub.TrueForAll(variableCode => selectedVariablesCode.Exists(code =>
                                code.Equals(variableCode, StringComparison.OrdinalIgnoreCase))) &&
                              p.Heading.TrueForAll(variableCode => selectedVariablesCode.Exists(code =>
                                code.Equals(variableCode, StringComparison.OrdinalIgnoreCase))));
        }

        private static void ReplaceTimeConstant(PXMeta meta, VariablePlacementType p)
        {
            var time = meta.Variables.Find(x => x.IsTime);
            if (time != null)
            {
                p.Stub = p.Stub.Select(x => x.Equals("TIME", System.StringComparison.OrdinalIgnoreCase) ? time.Code : x).ToList();
                p.Heading = p.Heading.Select(x => x.Equals("TIME", System.StringComparison.OrdinalIgnoreCase) ? time.Code : x).ToList();
            }
        }
    }
}
