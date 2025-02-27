using System.Linq;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;
using PxWeb.Code.Api2.DataSelection.SelectionExpressions;
using PxWeb.Helper.Api2;

namespace PxWeb.Code.Api2.DataSelection
{
    public class SelectionHandler : ISelectionHandler
    {
        private readonly PxApiConfigurationOptions _configOptions;

        public SelectionHandler(IPxApiConfigurationService configOptionsService)
        {
            _configOptions = configOptionsService.GetConfiguration();
        }


        public bool ExpandAndVerfiySelections(VariablesSelection variablesSelection, IPXModelBuilder builder, out Problem? problem)
        {
            if (SelectionUtil.UseDefaultSelection(variablesSelection))
            {
                problem = ProblemUtility.MissingSelection();
                return false;
            }

            if (!FixVariableRefsAndApplyCodelists(builder, variablesSelection, out problem))
            {
                return false;
            }

            if (!VerifyMandatoryVariables(builder.Model, variablesSelection, out problem))
            {
                return false;
            }

            //Add variables that the user did not post
            variablesSelection = AddVariables(variablesSelection, builder.Model);


            if (!ExpandSelectionExpressionsAndVerifyValues(builder, variablesSelection, out problem))
            {
                return false;
            }


            if (!CheckNumberOfCells(variablesSelection, _configOptions.MaxDataCells))
            {
                problem = ProblemUtility.TooManyCellsSelected();
                return false;
            }

            return true;
        }

        public Selection[] Convert(VariablesSelection variablesSelection)
        {
            var selections = new List<Selection>();

            foreach (var varSelection in variablesSelection.Selection)
            {
                var selection = new Selection(varSelection.VariableCode);
                selection.ValueCodes.AddRange(varSelection.ValueCodes.ToArray());
                selections.Add(selection);
            }

            return selections.ToArray();
        }

        private static bool CheckNumberOfCells(VariablesSelection selections, int threshold)
        {
            int cells = 1;

            //Calculate number of cells
            foreach (var s in selections.Selection)
            {
                if (s.ValueCodes.Count > 0)
                {
                    cells *= s.ValueCodes.Count;
                }
            }

            return cells <= threshold;
        }


        private static bool VerifyMandatoryVariables(PXModel model, VariablesSelection variablesSelection, out Problem? problem)
        {
            problem = null;
            foreach (var mandatoryVariable in GetAllMandatoryVariables(model))
            {
                if (!variablesSelection.Selection.Any(x => x.VariableCode.Equals(mandatoryVariable.Code, System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    problem = ProblemUtility.MissingSelection();
                    return false;
                }
            }
            return true;
        }

        private static bool ExpandSelectionExpressionsAndVerifyValues(IPXModelBuilder builder, VariablesSelection variablesSelection, out Problem? problem)
        {
            var model = builder.Model;
            problem = null;

            foreach (var variable in variablesSelection.Selection)
            {

                var modelVariable = model.Meta.Variables.GetByCode(variable.VariableCode);

                for (int i = 0; i < variable.ValueCodes.Count; i++)
                {
                    var valueCode = variable.ValueCodes[i];
                    // Try to get the value using the code specified by the API user
                    PCAxis.Paxiom.Value? pxValue = pxValue = modelVariable.Values.FirstOrDefault(x => x.Code.Equals(valueCode, System.StringComparison.InvariantCultureIgnoreCase));

                    if (pxValue is null)
                    {
                        return ExpandSelectionExpression(variable, modelVariable, valueCode, out problem);
                    }
                    else
                    {
                        variable.ValueCodes[i] = pxValue.Code;
                    }
                }

                //Verify that variables have at least one value selected for mandatory varibles
                var mandatory = SelectionUtil.IsMandatory(model, variable);
                if (variable.ValueCodes.Count() == 0 && mandatory)
                {
                    problem = ProblemUtility.MissingSelection();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Expand selection expression and add to selection
        /// This method should only be 
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="modelVariable"></param>
        /// <param name="valueCode"></param>
        /// <param name="problem"></param>
        /// <returns></returns>
        private static bool ExpandSelectionExpression(VariableSelection variable, Variable modelVariable, string valueCode, out Problem? problem)
        {
            for (int j = 0; j < ExpressionUtil.SelectionExpressions.Count; j++)
            {
                if (ExpressionUtil.SelectionExpressions[j].CanHandle(valueCode))
                {
                    if (!ExpressionUtil.SelectionExpressions[j].Verfiy(valueCode, out problem))
                    {
                        problem = ProblemUtility.IllegalSelectionExpression();
                        return false;
                    }

                    if (!ExpressionUtil.SelectionExpressions[j].AddToSelection(modelVariable, variable, valueCode, out problem))
                    {
                        problem = ProblemUtility.NonExistentValue();
                        return false;
                    }
                    problem = null;
                    return true;
                }
            }
            problem = ProblemUtility.NonExistentValue();
            return false;
        }

        /// <summary>
        /// Verify that VariablesSelection object has valid variables and values. Also applies codelists.
        /// </summary>
        /// <param name="builder">Paxiom model builder</param>
        /// <param name="variablesSelection">The VariablesSelection object to verify and apply codelists for</param>
        /// <param name="problem">Null if everything is ok, otherwise it describes whats wrong</param>
        /// <returns>True if everything was ok, else false</returns>
        private static bool FixVariableRefsAndApplyCodelists(IPXModelBuilder builder, VariablesSelection variablesSelection, out Problem? problem)
        {
            problem = null;

            //Verify that variable exists
            foreach (var variable in variablesSelection.Selection)
            {
                //Check if time is used as identifier for the time variable in variable selection
                //If the time variable is named differently in the metadata, change to it
                ReplaceTimeAlias(builder, variable);
                // Try to get variable using the code specified by the API user
                var pxVariable = FindAndFixCurrectVariableCode(builder, variable);

                if (pxVariable is null)
                {
                    problem = ProblemUtility.NonExistentVariable();
                    return false;
                }

                if (!ApplyCodelist(builder, pxVariable, variable, out problem))
                {
                    return false;
                }
            }

            return true;
        }

        private static Variable? FindAndFixCurrectVariableCode(IPXModelBuilder builder, VariableSelection variable)
        {
            Variable? pxVariable = builder.Model.Meta.Variables.GetByCode(variable.VariableCode);

            if (pxVariable is null)
            {
                // Is it a case sensitivity problem?
                pxVariable = builder.Model.Meta.Variables.FirstOrDefault(x => x.Code.Equals(variable.VariableCode, System.StringComparison.InvariantCultureIgnoreCase));

                if (pxVariable is not null)
                {
                    // Use the correct variable code for making the selection
                    variable.VariableCode = pxVariable.Code;
                }
            }
            return pxVariable;
        }

        private static void ReplaceTimeAlias(IPXModelBuilder builder, VariableSelection variable)
        {
            if (variable.VariableCode.ToLower().Equals("time"))
            {
                Variable? pxVariableTime = builder.Model.Meta.Variables.FirstOrDefault(x => x.IsTime);

                if (pxVariableTime is not null && (variable.VariableCode.ToLower() != pxVariableTime.Code.ToLower()))
                {
                    variable.VariableCode = pxVariableTime.Code.ToLower();
                }
            }
        }



        private static bool ApplyCodelist(IPXModelBuilder builder, Variable pxVariable, VariableSelection variable, out Problem? problem)
        {
            problem = null;

            if (!string.IsNullOrWhiteSpace(variable.CodeList))
            {
                if (variable.CodeList.StartsWith("agg_"))
                {
                    if (!ApplyGrouping(builder, pxVariable, variable, out problem))
                    {
                        return false;
                    }
                }
                else if (variable.CodeList.StartsWith("vs_"))
                {
                    if (!ApplyValueset(builder, pxVariable, variable, out problem))
                    {
                        return false;
                    }
                }
                else
                {
                    problem = ProblemUtility.NonExistentCodelist();
                    return false;
                }
            }

            return true;
        }

        private static bool ApplyGrouping(IPXModelBuilder builder, Variable pxVariable, VariableSelection variable, out Problem? problem)
        {
            problem = null;

            if (string.IsNullOrWhiteSpace(variable.CodeList))
            {
                problem = ProblemUtility.NonExistentCodelist();
                return false;
            }

            GroupingInfo grpInfo = pxVariable.GetGroupingInfoById(variable.CodeList.Replace("agg_", ""));

            if (grpInfo is null)
            {
                problem = ProblemUtility.NonExistentCodelist();
                return false;
            }

            GroupingIncludesType include = GroupingIncludesType.AggregatedValues; // Always build for aggregated values


            try
            {
                builder.ApplyGrouping(variable.VariableCode, grpInfo, include);
            }
            catch (Exception)
            {
                problem = ProblemUtility.NonExistentCodelist();
                return false;
            }

            return true;
        }

        private static bool ApplyValueset(IPXModelBuilder builder, Variable pxVariable, VariableSelection variable, out Problem? problem)
        {
            problem = null;

            if (string.IsNullOrWhiteSpace(variable.CodeList))
            {
                problem = ProblemUtility.NonExistentCodelist();
                return false;
            }

            ValueSetInfo vsInfo = pxVariable.GetValuesetById(variable.CodeList.Replace("vs_", ""));

            if (vsInfo is null)
            {
                problem = ProblemUtility.NonExistentCodelist();
                return false;
            }

            try
            {
                builder.ApplyValueSet(variable.VariableCode, vsInfo);
            }
            catch (Exception)
            {
                problem = ProblemUtility.NonExistentCodelist();
                return false;
            }

            return true;
        }


        /// <summary>
        /// Add all varibles for a table
        /// </summary>
        /// <param name="variablesSelection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static VariablesSelection AddVariables(VariablesSelection variablesSelection, PXModel model)
        {
            foreach (var variable in model.Meta.Variables)
            {
                if (!variablesSelection.Selection.Any(x => x.VariableCode.Equals(variable.Code, System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    //Add variable
                    var variableSelectionObject = new VariableSelection
                    {
                        VariableCode = variable.Code,
                        ValueCodes = new List<string>()
                    };

                    variablesSelection.Selection.Add(variableSelectionObject);
                }
            }

            return variablesSelection;
        }

        private static List<Variable> GetAllMandatoryVariables(PXModel model)
        {
            var mandatoryVariables = model.Meta.Variables.Where(x => x.Elimination.Equals(false)).ToList();
            return mandatoryVariables;
        }



    }
}
