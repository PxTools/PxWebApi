using System.Linq;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Helper.Api2
{
    public class SelectionUtil
    {
        public static VariablesSelection GetDefaultSelection(IPXModelBuilder builder)
        {

            //TODO look for saved query 
            var meta = builder.Model.Meta;
            // Default groupings and value sets are applied in the SQL parser by default
            // Only apply first valueset if no grouping or valueset is applied and multiple valuesets exists

            ReapplyCodelist(builder, meta);

            var contents = meta.Variables.FirstOrDefault(v => v.IsContentVariable);
            var time = meta.Variables.FirstOrDefault(v => v.IsTime);
            VariablesSelection selections;

            if (contents is not null && time is not null)
            {
                //PX file using good practice or CNMM datasource
                selections = GetDefaultSelectionByAlgorithm(meta, contents, time);
            }
            else
            {
                selections = GetDefaultSelectionByAlgorithmFallback(meta);
            }

            if (selections is null)
            {
                throw new System.Exception("Could not create default selection");
            }

            return selections;
        }


        #region "Default selection algorithm"


        private static VariablesSelection GetDefaultSelectionByAlgorithm(PXMeta meta, Variable contents, Variable time)
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

            //TODO : Should not happen, throw exception?
            return new VariablesSelection();
        }

        /// <summary>
        /// Case A according to the algorithm when oly contents and time variables are present
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static VariablesSelection OnlyContentsAndTime(Variable contents, Variable time)
        {
            var selections = new VariablesSelection();
            selections.Placement = new VariablePlacementType();

            if (contents.Values.Count < 6)
            {
                selections.AddHeadingVariable(contents, GetCodes);
                selections.AddStubVariable(time, GetTimeCodes, 13);
            }
            else
            {
                selections.AddStubVariable(contents, GetCodes);
                selections.AddHeadingVariable(time, GetTimeCodes, 13);
            }

            return selections;
        }

        /// <summary>
        /// Case B according to the algorithm when three variables are present and where contents and time are two of them
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="contents"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static VariablesSelection WithThreeDimensions(Variable contents, Variable time, Variable variable)
        {
            var selections = new VariablesSelection();
            selections.Placement = new VariablePlacementType();

            if (contents.Values.Count == 1)
            {
                // select the contents and 13 latest time values
                selections.AddVariableToHeading(contents, GetCodes);
                selections.AddHeadingVariable(time, GetTimeCodes, 13);
                selections.AddStubVariable(variable, GetCodes);
            }
            else
            {
                //Add the latest time value
                selections.AddVariableToHeading(time, GetTimeCodes);

                //Check if contents of classification should be in placmentStub or placmentHeading
                var (stub, heading) = StubOrHeading(contents, variable);
                selections.AddStubVariable(stub, GetCodes);
                selections.AddHeadingVariable(heading, GetCodes);
            }

            return selections;
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
        private static VariablesSelection WithMoreThenTreeDimensions(Variable contents, Variable time, List<Variable> classificationVariables, List<Variable> mandatoryClassificationVariables, List<Variable> noneMandatoryClassificationVariables)
        {

            if (mandatoryClassificationVariables.Count > 1)
            {
                return WithContentsAndTimeAndMoreThenOneMandatoryClassificationVariables(contents, time, mandatoryClassificationVariables, noneMandatoryClassificationVariables);
            }
            else if (mandatoryClassificationVariables.Count == 1)
            {
                return WithContentsAndTimeAndOneMandatoryClassificationVariables(contents, time, mandatoryClassificationVariables, noneMandatoryClassificationVariables);
            }

            return WithContentsAndTimeAndNoMandatoryClassificationVariables(contents, time, classificationVariables, noneMandatoryClassificationVariables);

        }

        /// <summary>
        /// Case C according to the algorithm when more then three variables are present and where contents and time are two of them
        /// and more then one mandatory variable
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="time"></param>
        /// <param name="mandatoryClassificationVariables"></param>
        /// <param name="noneMandatoryClassificationVariables"></param>
        /// <returns></returns>
        private static VariablesSelection WithContentsAndTimeAndMoreThenOneMandatoryClassificationVariables(Variable contents, Variable time, List<Variable> mandatoryClassificationVariables, List<Variable> noneMandatoryClassificationVariables)
        {
            var selections = new VariablesSelection();
            selections.Placement = new VariablePlacementType();

            //First content and lastNoneMandantoryClassificationVariable time period
            selections.AddVariableToHeading(contents, GetCodes);
            selections.AddVariableToHeading(time, GetTimeCodes);

            for (int i = 1; i < (mandatoryClassificationVariables.Count - 1); i++)
            {
                selections.AddVariableToHeading(mandatoryClassificationVariables[i], GetCodes);
            }

            //The variable with the most values should be in the placmentHeading
            var (stub, heading) = StubOrHeading(mandatoryClassificationVariables[0], mandatoryClassificationVariables[mandatoryClassificationVariables.Count - 1]);

            selections.AddStubVariable(stub, GetCodes);
            selections.AddHeadingVariable(heading, GetCodes);

            //Add the none mandatory classification variables without any selected values
            foreach (var variable in noneMandatoryClassificationVariables)
            {
                selections.EliminateVariable(variable);
            }

            return selections;
        }

        /// <summary>
        /// Case C according to the algorithm when more then three variables are present and where contents and time are two of them
        /// and only one mandatory variable
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="time"></param>
        /// <param name="mandatoryClassificationVariables"></param>
        /// <param name="noneMandatoryClassificationVariables"></param>
        /// <returns></returns>
        private static VariablesSelection WithContentsAndTimeAndOneMandatoryClassificationVariables(Variable contents, Variable time, List<Variable> mandatoryClassificationVariables, List<Variable> noneMandatoryClassificationVariables)
        {
            var selections = new VariablesSelection();
            selections.Placement = new VariablePlacementType();

            //First content and lastNoneMandantoryClassificationVariable time period
            selections.AddVariableToHeading(contents, GetCodes);
            selections.AddVariableToHeading(time, GetTimeCodes);

            var lastNoneMandantoryClassificationVariable = noneMandatoryClassificationVariables.Last();
            var (stub, heading) = StubOrHeading(mandatoryClassificationVariables[0], lastNoneMandantoryClassificationVariable);
            selections.AddStubVariable(stub, GetCodes);
            selections.AddHeadingVariable(heading, GetCodes);

            foreach (var variable in noneMandatoryClassificationVariables)
            {
                if (variable != lastNoneMandantoryClassificationVariable)
                {
                    selections.EliminateVariable(variable);
                }
            }

            return selections;
        }

        /// <summary>
        /// Case C according to the algorithm when more then three variables are present and where contents and time are two of them
        /// and there are no mandatory classification variables
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="time"></param>
        /// <param name="classificationVariables"></param>
        /// <param name="noneMandatoryClassificationVariables"></param>
        /// <returns></returns>
        private static VariablesSelection WithContentsAndTimeAndNoMandatoryClassificationVariables(Variable contents, Variable time, List<Variable> classificationVariables, List<Variable> noneMandatoryClassificationVariables)
        {
            var selections = new VariablesSelection();
            selections.Placement = new VariablePlacementType();

            //First content and lastNoneMandantoryClassificationVariable time period
            selections.AddVariableToHeading(contents, GetCodes);
            selections.AddVariableToHeading(time, GetTimeCodes);

            var firstNoneMandantoryClassificationVariable = classificationVariables.First();
            var lastNoneMandantoryClassificationVariable = classificationVariables.Last();
            var (stub, heading) = StubOrHeading(firstNoneMandantoryClassificationVariable, lastNoneMandantoryClassificationVariable);
            selections.AddStubVariable(stub, GetCodes);
            selections.AddHeadingVariable(heading, GetCodes);


            foreach (var variable in noneMandatoryClassificationVariables)
            {
                if (variable != firstNoneMandantoryClassificationVariable && variable != lastNoneMandantoryClassificationVariable)
                {
                    selections.EliminateVariable(variable);
                }
            }

            return selections;
        }

        #endregion


        #region "Default selection by fallback algorithm"

        /// <summary>
        /// Fallback method for getting default selection when contents and time are not present
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        private static VariablesSelection GetDefaultSelectionByAlgorithmFallback(PXMeta meta)
        {
            var selections = new VariablesSelection();
            selections.Placement = new VariablePlacementType();

            //Only one variable put it in the placmentStub
            if (meta.Variables.Count == 1)
            {
                selections.AddStubVariable(meta.Variables[0], GetCodes);
                return selections;
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

                //select firt value for all remaining mandatory classification variables
                for (int i = 1; i < mandatoryClassificationVariables.Count - 1; i++)
                {
                    selections.AddVariableToHeading(mandatoryClassificationVariables[i], GetCodes);
                }

                //Eliminate all none mandatory classification variables
                for (int i = 0; i < noneMandatoryClassificationVariables.Count; i++)
                {
                    selections.EliminateVariable(noneMandatoryClassificationVariables[i]);
                }

                selections.AddStubVariable(stub, GetCodes);
                selections.AddHeadingVariable(heading, GetCodes);

            }
            else //No mandantory variables and at leat two of them
            {
                //Take the first and last none mandantory classification variable
                //and place the one with most values in the placmentStub
                var (stub, heading) = StubOrHeading(noneMandatoryClassificationVariables[0], noneMandatoryClassificationVariables.Last());
                selections.AddStubVariable(stub, GetCodes);
                selections.AddHeadingVariable(heading, GetCodes);

                //Eliminate all none mandatory classification variables
                for (int i = 1; i < noneMandatoryClassificationVariables.Count - 1; i++)
                {
                    selections.EliminateVariable(noneMandatoryClassificationVariables[i]);
                }
            }

            return selections;
        }


        #endregion



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



        private static string[] GetCodes(Variable variable, int count)
        {
            var codes = variable.Values.Take(count).Select(value => value.Code).ToArray();

            return codes;
        }


        private static string[] GetTimeCodes(Variable variable, int count)
        {
            var lstCodes = variable.Values.TakeLast(count).Select(value => value.Code).ToList();
            var codes = lstCodes.ToArray();

            return codes;
        }

        private static void ReapplyCodelist(IPXModelBuilder builder, PXMeta meta)
        {
            foreach (var variable in meta.Variables)
            {
                if (variable.HasValuesets() && variable.CurrentGrouping is null && variable.CurrentValueSet is null)
                {
                    builder.ApplyValueSet(variable.Code, variable.ValueSets[0]);
                }
                else if (variable.CurrentGrouping != null)
                {
                    builder.ApplyGrouping(variable.Code, variable.GetGroupingInfoById(variable.CurrentGrouping.ID), GroupingIncludesType.AggregatedValues);
                }
                else if (variable.CurrentValueSet != null)
                {
                    builder.ApplyValueSet(variable.Code, variable.CurrentValueSet);
                }
            }
        }
    }

    public static class VariablesSelectionExtensions
    {
        public static void AddStubVariable(this VariablesSelection selections, Variable variable, Func<Variable, int, string[]> valuesFunction, int numberOfValues = 1500)
        {
            var selection = new VariableSelection();
            selection.VariableCode = variable.Code;
            selection.CodeList = GetCodeList(variable);
            selection.ValueCodes.AddRange(valuesFunction(variable, numberOfValues));
            selections.Selection.Add(selection);
            selections.Placement?.Stub.Add(variable.Code);
        }

        public static void AddHeadingVariable(this VariablesSelection selections, Variable variable, Func<Variable, int, string[]> valuesFunction, int numberOfValues = 11)
        {
            var selection = new VariableSelection();
            selection.VariableCode = variable.Code;
            selection.CodeList = GetCodeList(variable);
            selection.ValueCodes.AddRange(valuesFunction(variable, numberOfValues));
            selections.Selection.Add(selection);
            selections.Placement?.Heading.Add(variable.Code);
        }

        public static void AddVariableToHeading(this VariablesSelection selections, Variable variable, Func<Variable, int, string[]> valuesFunction)
        {
            var selection = new VariableSelection();
            selection.VariableCode = variable.Code;
            selection.CodeList = GetCodeList(variable);
            selection.ValueCodes.AddRange(valuesFunction(variable, 1));
            selections.Selection.Add(selection);
            selections.Placement?.Stub.Add(variable.Code);
        }

        public static void EliminateVariable(this VariablesSelection selections, Variable variable)
        {
            var selection = new VariableSelection();
            selection.VariableCode = variable.Code;
            selection.CodeList = GetCodeList(variable);
            selections.Selection.Add(selection);

        }

        private static string? GetCodeList(Variable? variable)
        {
            if (variable == null)
            {
                return null;
            }

            if (variable.CurrentGrouping != null)
            {
                return "agg_" + variable.CurrentGrouping.ID;
            }

            if (variable.CurrentValueSet != null)
            {
                return "vs_" + variable.CurrentValueSet.ID;
            }

            return null;

        }

    }
}
