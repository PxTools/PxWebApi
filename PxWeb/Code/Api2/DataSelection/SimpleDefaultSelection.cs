using System.Linq;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;
using PxWeb.Helper.Api2;

namespace PxWeb.Code.Api2.DataSelection
{
    public class SimpleDefaultSelection : IDefaultSelectionAlgorithm
    {
        public VariablesSelection GetDefaultSelection(IPXModelBuilder builder)
        {
            var meta = builder.Model.Meta;

            var selections = SelectionUtil.CreateEmptyVariablesSelection();

            //Only one variable put it in the placmentStub
            if (meta.Variables.Count == 1)
            {
                selections.AddStubVariable(meta.Variables[0], SelectionUtil.GetCodes);
                return selections;
            }

            var mandatoryClassificationVariables = meta.Variables.Where(v => !v.Elimination).ToList();
            var noneMandatoryClassificationVariables = meta.Variables.Where(v => v.Elimination).ToList();

            if (mandatoryClassificationVariables.Count == 1) //Only one mandantory classification variable
            {
                //Take the mandantory and the last none mandantory classification variable
                // place the one with most values in the placmentStub
                var (stub, heading) = SelectionUtil.StubOrHeading(mandatoryClassificationVariables[0], noneMandatoryClassificationVariables[noneMandatoryClassificationVariables.Count - 1]);
                selections.AddStubVariable(stub, SelectionUtil.GetCodes);
                selections.AddHeadingVariable(heading, SelectionUtil.GetCodes);

                //Eliminate all none mandatory classification variables
                for (int i = 0; i < noneMandatoryClassificationVariables.Count - 1; i++)
                {
                    selections.EliminateVariable(noneMandatoryClassificationVariables[i]);
                }
            }
            else if (mandatoryClassificationVariables.Count > 1) // Two or more mandatory classification variable
            {
                //Take the first and last mandantory classification variable
                //and place the one with most values in the placmentStub
                var (stub, heading) = SelectionUtil.StubOrHeading(mandatoryClassificationVariables[0], mandatoryClassificationVariables[mandatoryClassificationVariables.Count - 1]);

                //select firt value for all remaining mandatory classification variables
                for (int i = 1; i < mandatoryClassificationVariables.Count - 1; i++)
                {
                    selections.AddVariableToHeading(mandatoryClassificationVariables[i], SelectionUtil.GetCodes);
                }

                //Eliminate all none mandatory classification variables
                for (int i = 0; i < noneMandatoryClassificationVariables.Count; i++)
                {
                    selections.EliminateVariable(noneMandatoryClassificationVariables[i]);
                }

                selections.AddStubVariable(stub, SelectionUtil.GetCodes);
                selections.AddHeadingVariable(heading, SelectionUtil.GetCodes);

            }
            else //No mandantory variables and at leat two of them
            {
                //Take the first and last none mandantory classification variable
                //and place the one with most values in the placmentStub
                var (stub, heading) = SelectionUtil.StubOrHeading(noneMandatoryClassificationVariables[0], noneMandatoryClassificationVariables[noneMandatoryClassificationVariables.Count - 1];
                selections.AddStubVariable(stub, SelectionUtil.GetCodes);
                selections.AddHeadingVariable(heading, SelectionUtil.GetCodes);

                //Eliminate all none mandatory classification variables
                for (int i = 1; i < noneMandatoryClassificationVariables.Count - 1; i++)
                {
                    selections.EliminateVariable(noneMandatoryClassificationVariables[i]);
                }
            }

            return selections;
        }
    }
}
