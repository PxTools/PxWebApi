using System.Linq;
using System.Text.RegularExpressions;

using Lucene.Net.Util;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;
using PxWeb.Helper.Api2;

namespace PxWeb.Code.Api2.DataSelection
{
    public class SelectionHandler : ISelectionHandler
    {
        private readonly PxApiConfigurationOptions _configOptions;

        // Regular expressions for selection expression validation

        // TOP(xxx), TOP(xxx,yyy), top(xxx) and top(xxx,yyy)
        private static readonly string REGEX_TOP = "^(TOP\\([1-9]\\d*\\)|TOP\\([1-9]\\d*,[0-9]\\d*\\))$";

        // BOTTOM(xxx), BOTTOM(xxx,yyy), bottom(xxx) and bottom(xxx,yyy)
        private static readonly string REGEX_BOTTOM = "^(BOTTOM\\([1-9]\\d*\\)|BOTTOM\\([1-9]\\d*,[0-9]\\d*\\))$";

        // RANGE(xxx,yyy) and range(xxx,yyy)
        private static readonly string REGEX_RANGE = "^(RANGE\\(([^,]+)\\d*,([^,)]+)\\d*\\))$";

        // FROM(xxx) and from(xxx)
        private static readonly string REGEX_FROM = "^(FROM\\(([^,]+)\\d*\\))$";

        // TO(xxx) and to(xxx)
        private static readonly string REGEX_TO = "^(TO\\(([^,]+)\\d*\\))$";

        public SelectionHandler(IPxApiConfigurationService configOptionsService)
        {
            _configOptions = configOptionsService.GetConfiguration();
        }

        /// <summary>
        /// Get Selection-array for the wanted variables and values
        /// </summary>
        /// <param name="builder">Paxiom model builder</param>
        /// <param name="variablesSelection">VariablesSelection object describing wanted variables and values</param>
        /// <param name="problem">Null if everything is ok, otherwise it describes whats wrong</param>
        /// <returns>If everything was ok, an array of selection objects, else null</returns>
        public Selection[]? GetSelection(IPXModelBuilder builder, VariablesSelection variablesSelection, out Problem? problem)
        {
            if (!VerifyAndApplyCodelists(builder, variablesSelection, out problem))
            {
                return null;
            }

            Selection[]? selections;

            if (!UseDefaultSelection(variablesSelection))
            {
                //Add variables that the user did not post
                variablesSelection = AddVariables(variablesSelection, builder.Model);

                //Map VariablesSelection to PCaxis.Paxiom.Selection[] 
                selections = MapCustomizedSelection(builder, builder.Model, variablesSelection).ToArray();
            }
            else
            {
                problem = ProblemUtility.IllegalSelection();
                return null;
            }

            //Verify that valid selections could be made for mandatory variables
            if (!VerifyMadeSelection(builder, selections))
            {
                problem = ProblemUtility.IllegalSelection();
                return null;
            }

            if (!CheckNumberOfCells(selections))
            {
                selections = null;
                problem = ProblemUtility.TooManyCellsSelected();
            }

            return selections;

        }

        /// <summary>
        /// Verify that VariablesSelection object has valid variables and values. Also applies codelists.
        /// </summary>
        /// <param name="builder">Paxiom model builder</param>
        /// <param name="variablesSelection">The VariablesSelection object to verify and apply codelists for</param>
        /// <param name="problem">Null if everything is ok, otherwise it describes whats wrong</param>
        /// <returns>True if everything was ok, else false</returns>
        private bool VerifyAndApplyCodelists(IPXModelBuilder builder, VariablesSelection? variablesSelection, out Problem? problem)
        {
            problem = null;

            if (!UseDefaultSelection(variablesSelection) && variablesSelection is not null)
            {
                //Verify that variable exists
                foreach (var variable in variablesSelection.Selection)
                {
                    //Check if time is used as identifier for the time variable in variable selection
                    //If the time variable is named differently in the metadata, change to it
                    if (variable.VariableCode.ToLower().Equals("time"))
                    {
                        Variable? pxVariableTime = builder.Model.Meta.Variables.FirstOrDefault(x => x.IsTime);

                        if (pxVariableTime is not null && (variable.VariableCode.ToLower() != pxVariableTime.Code.ToLower()))
                        {
                            variable.VariableCode = pxVariableTime.Code.ToLower();
                        }
                    }
                    // Try to get variable using the code specified by the API user
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

                //Verify that all the mandatory variables exists
                foreach (var mandatoryVariable in GetAllMandatoryVariables(builder.Model))
                {
                    if (!variablesSelection.Selection.Any(x => x.VariableCode.Equals(mandatoryVariable.Code, System.StringComparison.InvariantCultureIgnoreCase)))
                    {
                        problem = ProblemUtility.MissingSelection();
                        return false;
                    }
                }

                //Verify variable values
                if (!VerifyVariableValues(builder.Model, variablesSelection, out problem))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ApplyCodelist(IPXModelBuilder builder, Variable pxVariable, VariableSelection variable, out Problem? problem)
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

        private bool ApplyGrouping(IPXModelBuilder builder, Variable pxVariable, VariableSelection variable, out Problem? problem)
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

        private bool ApplyValueset(IPXModelBuilder builder, Variable pxVariable, VariableSelection variable, out Problem? problem)
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
        /// Verify that the wanted variable values has valid codes
        /// </summary>
        /// <param name="model">Paxiom model</param>
        /// <param name="variablesSelection">VariablesSelection with the wanted variables and values</param>
        /// <param name="problem">Will be null if everything is ok, oterwise it will describe the problem</param>
        /// <returns></returns>
        private bool VerifyVariableValues(PXModel model, VariablesSelection variablesSelection, out Problem? problem)
        {
            problem = null;

            foreach (var variable in variablesSelection.Selection)
            {
                //Verify that variables have at least one value selected for mandatory varibles
                var mandatory = Mandatory(model, variable);
                if (variable.ValueCodes.Count().Equals(0) && mandatory)
                {
                    problem = ProblemUtility.NonExistentValue();
                    return false;
                }

                //Check variable values if they exists in model.Metadata
                if (!variable.ValueCodes.Count().Equals(0))
                {
                    var modelVariable = model.Meta.Variables.GetByCode(variable.VariableCode);

                    for (int i = 0; i < variable.ValueCodes.Count; i++)
                    {
                        if (!IsSelectionExpression(variable.ValueCodes[i]))
                        {
                            // Try to get the value using the code specified by the API user
                            PCAxis.Paxiom.Value? pxValue = modelVariable.Values.GetByCode(variable.ValueCodes[i]);

                            if (pxValue is null)
                            {
                                // Is it a problem with case sensitivity?
                                pxValue = modelVariable.Values.FirstOrDefault(x => x.Code.Equals(variable.ValueCodes[i], System.StringComparison.InvariantCultureIgnoreCase));

                                if (pxValue is not null)
                                {
                                    // Use the correct value code for making the selection
                                    variable.ValueCodes[i] = pxValue.Code;
                                }
                                else
                                {
                                    problem = ProblemUtility.NonExistentValue();
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if (!VerifySelectionExpression(variable.ValueCodes[i]))
                            {
                                problem = ProblemUtility.IllegalSelectionExpression();
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Verifies that a selection expression is valid
        /// </summary>
        /// <param name="expression">The selection expression to verify</param>
        /// <returns>True if the expression is valid, else false</returns>
        private bool VerifySelectionExpression(string expression)
        {
            if (expression.Contains('*'))
            {
                return VerifyWildcardStarExpression(expression);
            }
            else if (expression.Contains('?'))
            {
                return VerifyWildcardQuestionmarkExpression(expression);
            }
            else if (expression.StartsWith("TOP(", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return VerifyTopExpression(expression);
            }
            else if (expression.StartsWith("BOTTOM(", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return VerifyBottomExpression(expression);
            }
            else if (expression.StartsWith("RANGE(", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return VerifyRangeExpression(expression);
            }
            else if (expression.StartsWith("FROM(", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return VerifyFromExpression(expression);
            }
            else if (expression.StartsWith("TO(", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return VerifyToExpression(expression);
            }

            return false;
        }

        /// <summary>
        /// Verifies that the wildcard * selection expression is valid
        /// </summary>
        /// <param name="expression">The wildcard selection expression to validate</param>
        /// <returns>True if the expression is valid, else false</returns>
        private bool VerifyWildcardStarExpression(string expression)
        {
            if (expression.Equals("*"))
            {
                return true;
            }

            int count = expression.Count(c => c == '*');

            if (count > 2)
            {
                // More than 2 * is not allowed
                return false;
            }

            if ((count == 1) && !(expression.StartsWith('*') || expression.EndsWith('*')))
            {
                // * must be in the beginning or end of the value
                return false;
            }

            if ((count == 2) && !(expression.StartsWith('*') && expression.EndsWith('*')))
            {
                // The * must be in the beginning and the end of the value
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies that the wildcard ? selection expression is valid
        /// </summary>
        /// <param name="expression">The wildcard selection expression to validate</param>
        /// <returns>True if the expression is valid, else false</returns>
        private bool VerifyWildcardQuestionmarkExpression(string expression)
        {
            // What could be wrong?
            return true;
        }

        /// <summary>
        /// Verifies that the TOP(xxx) or TOP(xxx,yyy) selection expression is valid
        /// </summary>
        /// <param name="expression">The TOP selection expression to validate</param>
        /// <returns>True if the expression is valid, else false</returns>
        private bool VerifyTopExpression(string expression)
        {
            return Regex.IsMatch(expression, REGEX_TOP, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Verifies that the BOTTOM(xxx) or BOTTOM(xxx,yyy) selection expression is valid
        /// </summary>
        /// <param name="expression">The BOTTOM selection expression to validate</param>
        /// <returns>True if the expression is valid, else false</returns>
        private bool VerifyBottomExpression(string expression)
        {
            return Regex.IsMatch(expression, REGEX_BOTTOM, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Verifies that the RANGE(xxx,yyy) selection expression is valid
        /// </summary>
        /// <param name="expression">The RANGE selection expression to validate</param>
        /// <returns>True if the expression is valid, else false</returns>
        private bool VerifyRangeExpression(string expression)
        {
            return Regex.IsMatch(expression, REGEX_RANGE, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Verifies that the FROM(xxx) selection expression is valid
        /// </summary>
        /// <param name="expression">The FROM selection expression to validate</param>
        /// <returns>True if the expression is valid, else false</returns>
        private bool VerifyFromExpression(string expression)
        {
            return Regex.IsMatch(expression, REGEX_FROM, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Verifies that the TO(xxx) selection expression is valid
        /// </summary>
        /// <param name="expression">The TO selection expression to validate</param>
        /// <returns>True if the expression is valid, else false</returns>
        private bool VerifyToExpression(string expression)
        {
            return Regex.IsMatch(expression, REGEX_TO, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Returns true if the value string is a selection expression, else false.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsSelectionExpression(string value)
        {
            return value.Contains('*') ||
                   value.Contains('?') ||
                   value.StartsWith("TOP(", System.StringComparison.InvariantCultureIgnoreCase) ||
                   value.StartsWith("BOTTOM(", System.StringComparison.InvariantCultureIgnoreCase) ||
                   value.StartsWith("RANGE(", System.StringComparison.InvariantCultureIgnoreCase) ||
                   value.StartsWith("FROM(", System.StringComparison.InvariantCultureIgnoreCase) ||
                   value.StartsWith("TO(", System.StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Add all varibles for a table
        /// </summary>
        /// <param name="variablesSelection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private VariablesSelection AddVariables(VariablesSelection variablesSelection, PXModel model)
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

        /// <summary>
        /// Map VariablesSelection to PCaxis.Paxiom.Selection[]
        /// </summary>
        /// <param name="variablesSelection"></param>
        /// <returns></returns>
        private Selection[] MapCustomizedSelection(IPXModelBuilder builder, PXModel model, VariablesSelection variablesSelection)
        {
            var selections = new List<Selection>();

            foreach (var varSelection in variablesSelection.Selection)
            {
                var variable = model.Meta.Variables.GetByCode(varSelection.VariableCode);
                selections.Add(GetSelection(builder, variable, varSelection));
            }

            return selections.ToArray();
        }

        /// <summary>
        /// Add all values for variable
        /// </summary>
        /// <param name="variable">Paxiom variable</param>
        /// <param name="varSelection">VariableSelection object with wanted values from user</param>
        /// <returns></returns>
        private Selection GetSelection(IPXModelBuilder builder, Variable variable, VariableSelection varSelection)
        {
            var selection = new Selection(varSelection.VariableCode);
            var values = new List<string>();
            bool aggregatedSingle = false;

            if (variable.CurrentGrouping is not null && varSelection.OutputValues == CodeListOutputValuesType.SingleEnum)
            {
                // Single values from aggregation groups shall be added
                aggregatedSingle = true;
            }

            foreach (var value in varSelection.ValueCodes)
            {
                if (value.Contains('*'))
                {
                    AddWildcardStarValues(variable, aggregatedSingle, values, value);
                }
                else if (value.Contains('?'))
                {
                    AddWildcardQuestionmarkValues(variable, aggregatedSingle, values, value);
                }
                else if (value.StartsWith("TOP(", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    AddTopValues(variable, aggregatedSingle, values, value);
                }
                else if (value.StartsWith("BOTTOM(", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    AddBottomValues(variable, aggregatedSingle, values, value);
                }
                else if (value.StartsWith("RANGE(", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    AddRangeValues(variable, aggregatedSingle, values, value);
                }
                else if (value.StartsWith("FROM(", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    AddFromValues(variable, aggregatedSingle, values, value);
                }
                else if (value.StartsWith("TO(", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    AddToValues(variable, aggregatedSingle, values, value);
                }
                else
                {
                    AddValue(variable, aggregatedSingle, values, value);
                }
            }

            if (!aggregatedSingle)
            {
                var sortedValues = SortValues(variable, values);
                selection.ValueCodes.AddRange(sortedValues.ToArray());
            }
            else
            {
                selection.ValueCodes.AddRange(values.ToArray());
            }

            if (aggregatedSingle)
            {
                // Need to restore original values before trying to get data
                ValueSetInfo vsInfo = new ValueSetInfo();
                vsInfo.ID = "_ALL_";
                builder.ApplyValueSet(selection.VariableCode, vsInfo);
            }

            return selection;
        }

        private void AddValue(Variable variable, bool aggregatedSingle, List<string> values, string value)
        {
            if (!aggregatedSingle)
            {
                if (!values.Contains(value))
                {
                    values.Add(value);
                }
            }
            else
            {
                if (variable.CurrentGrouping is not null)
                {
                    PCAxis.Paxiom.Group? group = variable.CurrentGrouping.Groups.FirstOrDefault(x => x.GroupCode == value);

                    if (group is not null)
                    {
                        foreach (var child in group.ChildCodes)
                        {
                            if (!values.Contains(child.Code))
                            {
                                values.Add(child.Code);
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Sort selected values so that they appear in the same order as in the Paxiom variable
        /// </summary>
        /// <param name="variable">The Paxiom variable</param>
        /// <param name="values">Unsorted list of selected values</param>
        /// <returns>Sorted list of selected values</returns>
        private List<string> SortValues(Variable variable, List<string> values)
        {
            var sortedValues = new List<string>();

            foreach (var value in variable.Values)
            {
                if (values.Contains(value.Code))
                {
                    sortedValues.Add(value.Code);
                }
            }
            return sortedValues;
        }

        /// <summary>
        /// Add values for variable based on wildcard * selection. * represents 0 to many characters.
        /// </summary>
        /// <param name="variable">Paxiom variable</param>
        /// <param name="aggregatedSingle">Indicates if single values from aggregation groups shall be added</param>
        /// <param name="values">List that the values shall be added to</param>
        /// <param name="wildcard">The wildcard string</param>
        private void AddWildcardStarValues(Variable variable, bool aggregatedSingle, List<string> values, string wildcard)
        {
            if (wildcard.Equals("*"))
            {
                // Select all values
                var variableValues = variable.Values.Select(v => v.Code);
                foreach (var variableValue in variableValues)
                {
                    AddValue(variable, aggregatedSingle, values, variableValue);
                }
            }
            else if (wildcard.StartsWith("*") && wildcard.EndsWith("*"))
            {
                var variableValues = variable.Values.Where(v => v.Code.Contains(wildcard.Substring(1, wildcard.Length - 2), StringComparison.InvariantCultureIgnoreCase)).Select(v => v.Code);
                foreach (var variableValue in variableValues)
                {
                    AddValue(variable, aggregatedSingle, values, variableValue);
                }
            }
            else if (wildcard.StartsWith("*"))
            {
                var variableValues = variable.Values.Where(v => v.Code.EndsWith(wildcard.Substring(1), StringComparison.InvariantCultureIgnoreCase)).Select(v => v.Code);
                foreach (var variableValue in variableValues)
                {
                    AddValue(variable, aggregatedSingle, values, variableValue);
                }
            }
            else if (wildcard.EndsWith("*"))
            {
                var variableValues = variable.Values.Where(v => v.Code.StartsWith(wildcard.Substring(0, wildcard.Length - 1), StringComparison.InvariantCultureIgnoreCase)).Select(v => v.Code);
                foreach (var variableValue in variableValues)
                {
                    AddValue(variable, aggregatedSingle, values, variableValue);
                }
            }
        }

        /// <summary>
        /// Add values for variable based on wildcard ? selection. ? reperesent any 1 character.
        /// </summary>
        /// <param name="variable">Paxiom variable</param>
        /// <param name="aggregatedSingle">Indicates if single values from aggregation groups shall be added</param>
        /// <param name="values">List that the values shall be added to</param>
        /// <param name="wildcard">The wildcard string</param>
        private void AddWildcardQuestionmarkValues(Variable variable, bool aggregatedSingle, List<string> values, string wildcard)
        {
            string regexPattern = string.Concat("^", Regex.Escape(wildcard).Replace("\\?", "."), "$");
            var variableValues = variable.Values.Where(v => Regex.IsMatch(v.Code, regexPattern, RegexOptions.IgnoreCase)).Select(v => v.Code);
            foreach (var variableValue in variableValues)
            {
                AddValue(variable, aggregatedSingle, values, variableValue);
            }
        }

        /// <summary>
        /// Add values for variable based on TOP(xxx) and TOP(xxx,yyy) selection expression. 
        /// </summary>
        /// <param name="variable">Paxiom variable</param>
        /// <param name="aggregatedSingle">Indicates if single values from aggregation groups shall be added</param>
        /// <param name="values">List that the values shall be added to</param>
        /// <param name="expression">The TOP selection expression string</param>
        private void AddTopValues(Variable variable, bool aggregatedSingle, List<string> values, string expression)
        {
            int count;
            int offset;

            if (!GetCountAndOffset(expression, out count, out offset))
            {
                return; // Something went wrong
            }

            var codes = variable.Values.Select(value => value.Code).ToArray();

            if (variable.IsTime)
            {
                codes.Sort((a, b) => b.CompareTo(a)); // Descending sort
            }

            for (int i = (0 + offset); i < (count + offset); i++)
            {
                if (i < codes.Length)
                {
                    AddValue(variable, aggregatedSingle, values, codes[i]);
                }
            }
        }

        /// <summary>
        /// Add values for variable based on BOTTOM(xxx) and BOTTOM(xxx,yyy) selection expression. 
        /// </summary>
        /// <param name="variable">Paxiom variable</param>
        /// <param name="aggregatedSingle">Indicates if single values from aggregation groups shall be added</param>
        /// <param name="values">List that the values shall be added to</param>
        /// <param name="expression">The BOTTOM selection expression string</param>
        private void AddBottomValues(Variable variable, bool aggregatedSingle, List<string> values, string expression)
        {
            int count;
            int offset;

            if (!GetCountAndOffset(expression, out count, out offset))
            {
                return; // Something went wrong
            }

            var codes = variable.Values.Select(value => value.Code).ToArray();

            if (variable.IsTime)
            {
                codes.Sort((a, b) => b.CompareTo(a)); // Descending sort
            }

            if (codes.Length - offset > 0)
            {
                int startIndex = codes.Length - offset - 1;
                int endIndex = codes.Length - offset - count;

                for (int i = startIndex; i >= endIndex; i--)
                {
                    if (i >= 0)
                    {
                        AddValue(variable, aggregatedSingle, values, codes[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Add values for variable based on RANGE(xxx,yyy) selection expression. 
        /// </summary>
        /// <param name="variable">Paxiom variable</param>
        /// <param name="aggregatedSingle">Indicates if single values from aggregation groups shall be added</param>
        /// <param name="values">List that the values shall be added to</param>
        /// <param name="expression">The RANGE selection expression string</param>
        private void AddRangeValues(Variable variable, bool aggregatedSingle, List<string> values, string expression)
        {
            string code1 = "";
            string code2 = "";

            if (!GetRangeCodes(expression, out code1, out code2))
            {
                return; // Something went wrong
            }

            var codes = variable.Values.Select(value => value.Code).ToArray();

            if (variable.IsTime)
            {
                codes.Sort((a, b) => a.CompareTo(b)); // Ascending sort
            }

            int index1 = GetCodeIndex(codes, code1);
            int index2 = GetCodeIndex(codes, code2);
            int indexTemp;

            if (index1 > index2)
            {
                // Handle indexes in wrong order
                indexTemp = index1;
                index1 = index2;
                index2 = indexTemp;
            }

            if (index1 > -1 && index2 > -1 && index2 >= index1)
            {
                for (int i = index1; i <= index2; i++)
                {
                    AddValue(variable, aggregatedSingle, values, codes[i]);
                }
            }
        }

        /// <summary>
        /// Add values for variable based on FROM(xxx) selection expression. 
        /// </summary>
        /// <param name="variable">Paxiom variable</param>
        /// <param name="aggregatedSingle">Indicates if single values from aggregation groups shall be added</param>
        /// <param name="values">List that the values shall be added to</param>
        /// <param name="expression">The FROM selection expression string</param>
        private void AddFromValues(Variable variable, bool aggregatedSingle, List<string> values, string expression)
        {
            string code = "";

            if (!GetSingleCode(expression, out code))
            {
                return; // Something went wrong
            }

            var codes = variable.Values.Select(value => value.Code).ToArray();

            if (variable.IsTime)
            {
                codes.Sort((a, b) => a.CompareTo(b)); // Ascending sort
            }

            int index1 = GetCodeIndex(codes, code);

            if (index1 > -1)
            {
                for (int i = index1; i < codes.Length; i++)
                {
                    AddValue(variable, aggregatedSingle, values, codes[i]);
                }
            }
        }

        /// <summary>
        /// Add values for variable based on TO(xxx) selection expression. 
        /// </summary>
        /// <param name="variable">Paxiom variable</param>
        /// <param name="aggregatedSingle">Indicates if single values from aggregation groups shall be added</param>
        /// <param name="values">List that the values shall be added to</param>
        /// <param name="expression">The TO selection expression string</param>
        private void AddToValues(Variable variable, bool aggregatedSingle, List<string> values, string expression)
        {
            string code = "";

            if (!GetSingleCode(expression, out code))
            {
                return; // Something went wrong
            }

            var codes = variable.Values.Select(value => value.Code).ToArray();

            if (variable.IsTime)
            {
                codes.Sort((a, b) => a.CompareTo(b)); // Ascending sort
            }

            int index = GetCodeIndex(codes, code);

            if (index > -1)
            {
                for (int i = 0; i <= index; i++)
                {
                    AddValue(variable, aggregatedSingle, values, codes[i]);
                }
            }
        }

        /// <summary>
        /// Extracts the count and offset from selection expressions like TOP(count), TOP(count,offset), BOTTOM(count), BOTTOM(count,offset)
        /// </summary>
        /// <param name="expression">The selection expression to extract count and offset from</param>
        /// <param name="count">Set to the count value if it could be extracted, else 0</param>
        /// <param name="offset">Set to the offset value if it could be extracted, else 0</param>
        /// <returns>True if values could be extracted, false if something went wrong</returns>
        private bool GetCountAndOffset(string expression, out int count, out int offset)
        {
            count = 0;
            offset = 0;

            try
            {
                int firstParanteses = expression.IndexOf('(');

                if (firstParanteses == -1)
                {
                    return false;
                }

                string strNumbers = expression.Substring(firstParanteses + 1, expression.Length - (firstParanteses + 2)); // extract the numbers part of TOP(xxx) or TOP(xxx,yyy)
                string[] numbers = strNumbers.Split(',', System.StringSplitOptions.RemoveEmptyEntries);

                if (!int.TryParse(numbers[0], out count))
                {
                    return false; // Something went wrong
                }

                if (numbers.Length == 2)
                {
                    if (!int.TryParse(numbers[1], out offset))
                    {
                        return false; // Something went wrong
                    }
                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Extracts code1 and code2 from RANGE selection expressions like RANGE(xxx,yyy)
        /// </summary>
        /// <param name="expression">The Range selection expression to extract codes from</param>
        /// <param name="code1">The firts code</param>
        /// <param name="code2">The second code</param>
        /// <returns>True if the codes could be extracted, false if something went wrong</returns>
        private bool GetRangeCodes(string expression, out string code1, out string code2)
        {
            code1 = "";
            code2 = "";

            try
            {
                int firstParanteses = expression.IndexOf('(');

                if (firstParanteses == -1)
                {
                    return false;
                }

                string strCodes = expression.Substring(firstParanteses + 1, expression.Length - (firstParanteses + 2)); // extract the codes
                string[] codes = strCodes.Split(',', System.StringSplitOptions.RemoveEmptyEntries);

                if (codes.Length != 2)
                {
                    return false;
                }

                code1 = codes[0];
                code2 = codes[1];

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Find index of code in code array.
        /// First tries to find code as specified. If it is not found the method tries to find the code in a case insensitive way. 
        /// </summary>
        /// <param name="codes">Array of codes</param>
        /// <param name="code">Code to find index for</param>
        /// <returns>Index of the specified code within the codes array. If not found -1 is returned.</returns>
        private int GetCodeIndex(string[] codes, string code)
        {
            // Try to get the value using the code specified by the API user
            int index = Array.IndexOf(codes, code);

            if (index == -1)
            {
                // Is it a problem with case sensitivity?
                string? nonCaseCode = codes.FirstOrDefault(x => x.Equals(code, System.StringComparison.InvariantCultureIgnoreCase));

                if (nonCaseCode is not null)
                {
                    // Use non case sensitivy index
                    index = Array.IndexOf(codes, nonCaseCode);
                }
            }

            return index;
        }

        /// <summary>
        /// Extracts the code from selection expressions like FROM(xxx) or TO(xxx)
        /// </summary>
        /// <param name="expression">The Range selection expression to extract the code from</param>
        /// <param name="code">The code</param>
        /// <returns>True if teh code could be extracted, false if something went wrong</returns>
        private bool GetSingleCode(string expression, out string code)
        {
            code = "";

            try
            {
                int firstParanteses = expression.IndexOf('(');

                if (firstParanteses == -1)
                {
                    return false;
                }

                code = expression.Substring(firstParanteses + 1, expression.Length - (firstParanteses + 2)); // extract the code

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

    
        private List<Variable> GetAllMandatoryVariables(PXModel model)
        {
            var mandatoryVariables = model.Meta.Variables.Where(x => x.Elimination.Equals(false)).ToList();
            return mandatoryVariables;
        }

        private bool Mandatory(PXModel model, VariableSelection variable)
        {
            bool mandatory = false;
            var mandatoryVariable = model.Meta.Variables.Where(x => x.Code.Equals(variable.VariableCode) && x.Elimination.Equals(false));

            if (mandatoryVariable.Count() != 0)
            {
                mandatory = true;
            }
            return mandatory;
        }



        private string[] GetCodes(Variable variable, int count)
        {
            var codes = variable.Values.Take(count).Select(value => value.Code).ToArray();

            return codes;
        }


        private string[] GetTimeCodes(Variable variable, int count)
        {
            var lstCodes = variable.Values.TakeLast(count).Select(value => value.Code).ToList();
            var codes = lstCodes.ToArray();

            return codes;
        }

        private bool HasSelection(VariablesSelection selection)
        {
            if (selection.Selection.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Verifies that at least one valid value has been selected for mandatory variables
        /// </summary>
        /// <param name="builder">Paxiom model builder</param>
        /// <param name="selections">Selections made by the SelectionHandler</param>
        /// <returns>True if all mandatory variables have at least one selected value, else false</returns>
        private bool VerifyMadeSelection(IPXModelBuilder builder, Selection[]? selections)
        {
            if (selections is null)
            {
                return false;
            }

            //Verify that all the mandatory variables have at least one value
            foreach (var mandatoryVariable in GetAllMandatoryVariables(builder.Model))
            {
                if (!selections.Any(x => x.VariableCode.Equals(mandatoryVariable.Code, System.StringComparison.InvariantCultureIgnoreCase) && x.ValueCodes.Count > 0))
                {
                    return false;
                }
            }

            return true;
        }


        private bool CheckNumberOfCells(Selection[] selections)
        {
            int cells = CalculateCells(selections);

            if (cells > _configOptions.MaxDataCells)
            {
                return false;
            }

            return true;
        }


        private int CalculateCells(Selection[] selection)
        {
            int cells = 1;

            foreach (var s in selection)
            {
                if (s.ValueCodes.Count > 0)
                {
                    cells *= s.ValueCodes.Count;
                }
            }

            return cells;
        }


        public bool UseDefaultSelection(VariablesSelection? variablesSelection)
        {
            return variablesSelection is null || !HasSelection(variablesSelection);
        }

        public (Selection[]?, List<string>, List<string>) GetDefaultSelection(IPXModelBuilder builder, out Problem? problem)
        {
            var meta = builder.Model.Meta;
            // Default groupings and value sets are applied in the SQL parser by default
            // Only apply first valueset if no grouping or valueset is applied and multiple valuesets exists
            foreach (var variable in meta.Variables)
            {
                if (variable.HasValuesets() && variable.CurrentGrouping is null && variable.CurrentValueSet is null)
                {
                    builder.ApplyValueSet(variable.Code, variable.ValueSets[0]);
                }
            }

            var contents = meta.Variables.FirstOrDefault(v => v.IsContentVariable);
            var time = meta.Variables.FirstOrDefault(v => v.IsTime);
            List<Selection> selections;
            List<string> heading;
            List<string> stub;

            if (contents is not null && time is not null)
            {
                //PX file using good practice or CNMM datasource
                (selections, heading, stub) = GetDefaultSelectionByAlgorithm(meta, contents, time);
            }
            else
            {
                (selections, heading, stub) = GetDefaultSelectionByAlgorithmFallback(meta);
            }

            //Verify that valid selections could be made for mandatory variables
            if (!VerifyMadeSelection(builder, selections.ToArray()))
            {
                problem = ProblemUtility.IllegalSelection();
                return (null, new List<string>(), new List<string>());
            }

            if (!CheckNumberOfCells(selections.ToArray()))
            {
                problem = ProblemUtility.TooManyCellsSelected();
                return (null, new List<string>(), new List<string>());
            }

            problem = null;
            return (selections.ToArray(), heading, stub);

        }

        private (List<Selection>, List<string>, List<string>) GetDefaultSelectionByAlgorithmFallback(PXMeta meta)
        {
            var selections = new List<Selection>();
            List<string> placmentHeading = new List<string>();
            List<string> placmentStub = new List<string>();

            //Only one variable put it in the placmentStub
            if (meta.Variables.Count == 1)
            {
                selections.AddStubVariable(meta.Variables[0], GetCodes);
                placmentStub.Add(meta.Variables[0].Code);
                return (selections, placmentHeading, placmentStub);
            }

            var mandatoryClassificationVariables = meta.Variables.Where(v => v.Elimination == false).ToList();
            var noneMandatoryClassificationVariables = meta.Variables.Where(v => v.Elimination == true).ToList();

            if (mandatoryClassificationVariables.Count == 1) //Only one mandantory classification variable
            {
                //Take the mandantory and the last none mandantory classification variable
                // place the one with most values in the placmentStub
                var (stub, heading) = StubOrHeading(mandatoryClassificationVariables[0], noneMandatoryClassificationVariables.Last());
                selections.AddStubVariable(stub, GetCodes);
                selections.AddHeadingVariable(heading, GetCodes);

                placmentHeading.Add(heading.Code);
                placmentStub.Add(stub.Code);

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
                var (stub, heading) = StubOrHeading(mandatoryClassificationVariables[0], mandatoryClassificationVariables.Last());
                selections.AddStubVariable(stub, GetCodes);
                selections.AddHeadingVariable(heading, GetCodes);

                //select firt value for all remaining mandatory classification variables
                for (int i = 1; i < mandatoryClassificationVariables.Count - 1; i++)
                {
                    selections.AddVariableToHeading(mandatoryClassificationVariables[i], GetCodes);
                    placmentHeading.Add(mandatoryClassificationVariables[i].Code);
                }

                //Eliminate all none mandatory classification variables
                for (int i = 0; i < noneMandatoryClassificationVariables.Count; i++)
                {
                    selections.EliminateVariable(noneMandatoryClassificationVariables[i]);
                }

                placmentHeading.Add(heading.Code);
                placmentStub.Add(stub.Code);
            }
            else //No mandantory variables and at leat two of them
            {
                //Take the first and last none mandantory classification variable
                //and place the one with most values in the placmentStub
                var (stub, heading) = StubOrHeading(noneMandatoryClassificationVariables[0], noneMandatoryClassificationVariables.Last());
                placmentHeading.Add(heading.Code);
                placmentStub.Add(stub.Code);

                //Eliminate all none mandatory classification variables
                for (int i = 1; i < noneMandatoryClassificationVariables.Count - 1; i++)
                {
                    selections.EliminateVariable(noneMandatoryClassificationVariables[i]);
                }
            }

            return (selections, placmentHeading, placmentStub);
        }

        private (List<Selection>, List<string>, List<string>) GetDefaultSelectionByAlgorithm(PXMeta meta, Variable contents, Variable time)
        {
            if (meta.Variables.Count == 2) 
            {
                // Case A according to algorithm
                return OnlyContentsAndTime(contents, time);
            }
            else if (meta.Variables.Count == 3) 
            {
                // Case B according to algorithm
                var variable = meta.Variables.FirstOrDefault(v => v.Code != contents.Code && v.Code != time.Code);
                if (variable is not null)
                {
                    return WithThreeDimensions(contents, time, variable);
                }
            }
            else 
            {
                // Case C according to algorithm
                var classificationVariables = meta.Variables.Where(v => v.Code != contents.Code && v.Code != time.Code).ToList();
                var mandatoryClassificationVariables = classificationVariables.Where(v => v.Elimination == false).ToList();
                var noneMandatoryClassificationVariables = classificationVariables.Where(v => v.Elimination == true).ToList();
                return WithMoreThenTreeDimensions(contents, time, classificationVariables, mandatoryClassificationVariables, noneMandatoryClassificationVariables);
            }

            return (new List<Selection>(), new List<string>(), new List<string>());
        }

        /// <summary>
        /// Case C according to the algorithm when more then three variables are present and where contents and time are two of them
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="time"></param>
        /// <param name="classificationVariables"></param>
        /// <param name="mandatoryClassificationVariables"></param>
        /// <param name="noneMandatoryClassificationVariables"></param>
        /// <returns></returns>
        private (List<Selection>, List<string>, List<string>) WithMoreThenTreeDimensions(Variable contents, Variable time, List<Variable> classificationVariables, List<Variable> mandatoryClassificationVariables, List<Variable> noneMandatoryClassificationVariables)
        {
            var selections = new List<Selection>();
            List<string> placmentHeading = new List<string>();
            List<string> placmentStub = new List<string>();

            //First content and lastNoneMandantoryClassificationVariable time period
            selections.AddVariableToHeading(contents, GetCodes);
            selections.AddVariableToHeading(time, GetTimeCodes);
            placmentHeading.Add(contents.Code);

            if (mandatoryClassificationVariables.Count > 1)
            {
                for (int i = 1; i < (mandatoryClassificationVariables.Count - 1); i++)
                {
                    selections.AddVariableToHeading(mandatoryClassificationVariables[i], GetCodes);
                    placmentHeading.Add(mandatoryClassificationVariables[i].Code);
                }

                //The variable with the most values should be in the placmentHeading
                var (stub, heading) = StubOrHeading(mandatoryClassificationVariables[0], mandatoryClassificationVariables[mandatoryClassificationVariables.Count - 1]);
                placmentHeading.Add(heading.Code);
                placmentStub.Add(stub.Code);

                selections.AddStubVariable(stub, GetCodes);
                selections.AddHeadingVariable(heading, GetCodes);

                //Add the none mandatory classification variables without any selected values
                foreach (var variable in noneMandatoryClassificationVariables)
                {
                    selections.EliminateVariable(variable);
                }
            }
            else if (mandatoryClassificationVariables.Count == 1)
            {
                var lastNoneMandantoryClassificationVariable = noneMandatoryClassificationVariables.Last();
                var (stub, heading) = StubOrHeading(mandatoryClassificationVariables[0], lastNoneMandantoryClassificationVariable);
                selections.AddStubVariable(stub, GetCodes);
                selections.AddHeadingVariable(heading, GetCodes);

                placmentHeading.Add(heading.Code);
                placmentStub.Add(stub.Code);


                foreach (var variable in noneMandatoryClassificationVariables)
                {
                    if (variable != lastNoneMandantoryClassificationVariable)
                    {
                        selections.EliminateVariable(variable);
                    }
                }
            }
            else //No mandatory classification variables
            {
                var firstNoneMandantoryClassificationVariable = classificationVariables.First();
                var lastNoneMandantoryClassificationVariable = classificationVariables.Last();
                var (stub, heading) = StubOrHeading(firstNoneMandantoryClassificationVariable, lastNoneMandantoryClassificationVariable);
                selections.AddStubVariable(stub, GetCodes);
                selections.AddHeadingVariable(heading, GetCodes);
                placmentHeading.Add(heading.Code);
                placmentStub.Add(stub.Code);


                foreach (var variable in noneMandatoryClassificationVariables)
                {
                    if (variable != firstNoneMandantoryClassificationVariable && variable != lastNoneMandantoryClassificationVariable)
                    {
                        selections.EliminateVariable(variable);
                    }
                }
            }
            //place time as last variable in heading
            placmentHeading.Add(time.Code);
            return (selections, placmentHeading, placmentStub);
        }

        /// <summary>
        /// Case B according to the algorithm when three variables are present and where contents and time are two of them
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="contents"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private (List<Selection>, List<string>, List<string>) WithThreeDimensions(Variable contents, Variable time, Variable variable)
        {
            var selections = new List<Selection>();
            List<string> placmentHeading = new List<string>();
            List<string> placmentStub = new List<string>();

            if (contents.Values.Count == 1)
            {
                // select the contents and 13 latest time values
                selections.AddVariableToHeading(contents, GetCodes);
                selections.AddHeadingVariable(time, GetTimeCodes, 13);
                placmentHeading.Add(contents.Code);
                placmentHeading.Add(time.Code);
                selections.AddStubVariable(variable, GetCodes);
                placmentStub.Add(variable.Code);

            }
            else
            {
                //Add the latest time value
                selections.AddVariableToHeading(time, GetTimeCodes);
                placmentHeading.Add(time.Code);

                //Check if contents of classification should be in placmentStub or placmentHeading
                var (stub, heading) = StubOrHeading(contents, variable);
                selections.AddStubVariable(stub, GetCodes);
                selections.AddHeadingVariable(heading, GetCodes);

                placmentHeading.Add(heading.Code);
                placmentStub.Add(stub.Code);
            }

            return (selections, placmentHeading, placmentStub); 
        }

        /// <summary>
        /// Case A according to the algorithm when oly contents and time variables are present
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private (List<Selection>, List<string>, List<string>) OnlyContentsAndTime(Variable contents, Variable time)
        {
            var selections = new List<Selection>();
            List<string> placmentHeading = new List<string>();
            List<string> placmentStub = new List<string>();

            if (contents.Values.Count < 6)
            {
                selections.AddHeadingVariable(contents, GetCodes);
                selections.AddStubVariable(time, GetTimeCodes, 13);
                placmentHeading.Add(contents.Code);
                placmentStub.Add(time.Code);
            }
            else
            {
                selections.AddStubVariable(contents, GetCodes);
                selections.AddHeadingVariable(time, GetTimeCodes, 13);
                placmentHeading.Add(time.Code);
                placmentStub.Add(contents.Code);
            }

            return (selections, placmentHeading, placmentStub);
        }

        /// <summary>
        /// Helper function that determis which variable should to the Stub or Heading
        /// </summary>
        /// <param name="one">first variable</param>
        /// <param name="two">second variable</param>
        /// <returns>variable that should go to the stub and the variable hat should go to the heading</returns>
        private static (Variable, Variable) StubOrHeading(Variable one, Variable two)
        {
            if (one.Values.Count > two.Values.Count)
            {
                return (one, two);
            }
            else
            {
                return (two, one);
            }
        }
    }


    public static class SelectionsExtensions
    {
        public static void AddStubVariable(this List<Selection> selections, Variable variable, Func<Variable, int, string[]> valuesFunction, int numberOfValues = 1500)
        {
            var selection = new Selection(variable.Code);
            selection.ValueCodes.AddRange(valuesFunction(variable, numberOfValues));
            selections.Add(selection);
        }

        public static void AddHeadingVariable(this List<Selection> selections, Variable variable, Func<Variable, int, string[]> valuesFunction, int numberOfValues = 30)
        {
            var selection = new Selection(variable.Code);
            selection.ValueCodes.AddRange(valuesFunction(variable, numberOfValues));
            selections.Add(selection);
        }

        public static void AddVariableToHeading(this List<Selection> selections, Variable variable, Func<Variable, int, string[]> valuesFunction)
        {
            var selection = new Selection(variable.Code);
            selection.ValueCodes.AddRange(valuesFunction(variable, 1));
            selections.Add(selection);
        }

        public static void EliminateVariable(this List<Selection> selections, Variable variable)
        {
            var selection = new Selection(variable.Code);
            selections.Add(selection);
        }

    }
}
