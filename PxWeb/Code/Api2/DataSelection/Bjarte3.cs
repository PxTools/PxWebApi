using System.Linq;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;
using PxWeb.Helper.Api2;

namespace PxWeb.Code.Api2.DataSelection
{
    public class Bjarte3 : IDefaultSelectionAlgorithm
    {
        public VariablesSelection GetDefaultSelection(IPXModelBuilder builder)
        {
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
                var fallback = new SimpleDefaultSelection();
                selections = fallback.GetDefaultSelection(builder);
            }

            if (selections is null)
            {
                throw new NotSupportedException("Could not create default selection");
            }

            return selections;
        }

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
            // Case C according to algorithm
            var classificationVariables = meta.Variables.Where(v => v.Code != contents.Code && v.Code != time.Code).ToList();
            var mandatoryClassificationVariables = classificationVariables.Where(v => !v.Elimination).ToList();
            var noneMandatoryClassificationVariables = classificationVariables.Where(v => v.Elimination).ToList();
            return WithMoreThenTreeDimensions(contents, time, classificationVariables, mandatoryClassificationVariables, noneMandatoryClassificationVariables);
        }

        /// <summary>
        /// Case A according to the algorithm when oly contents and time variables are present
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static VariablesSelection OnlyContentsAndTime(Variable contents, Variable time)
        {
            var selections = SelectionUtil.CreateEmptyVariablesSelection();
            selections.Selection = new List<VariableSelection>();
            selections.Placement = new VariablePlacementType() { Heading = new List<string>(), Stub = new List<string>() };

            if (contents.Values.Count < 6)
            {
                selections.AddHeadingVariable(contents, SelectionUtil.GetCodes);
                selections.AddStubVariable(time, SelectionUtil.GetTimeCodes, 13);
            }
            else
            {
                selections.AddStubVariable(contents, SelectionUtil.GetCodes);
                selections.AddHeadingVariable(time, SelectionUtil.GetTimeCodes, 13);
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
            var selections = SelectionUtil.CreateEmptyVariablesSelection();

            if (contents.Values.Count == 1)
            {
                // select the contents and 13 latest time values
                selections.AddVariableToHeading(contents, SelectionUtil.GetCodes);
                selections.AddHeadingVariable(time, SelectionUtil.GetTimeCodes, 13);
                selections.AddStubVariable(variable, SelectionUtil.GetCodes);
            }
            else
            {
                //Add the latest time value
                selections.AddVariableToHeading(time, SelectionUtil.GetTimeCodes);

                //Check if contents of classification should be in placmentStub or placmentHeading
                var (stub, heading) = SelectionUtil.StubOrHeading(contents, variable);
                selections.AddStubVariable(stub, SelectionUtil.GetCodes);
                selections.AddHeadingVariable(heading, SelectionUtil.GetCodes);
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
            var selections = SelectionUtil.CreateEmptyVariablesSelection();

            //First content and lastNoneMandantoryClassificationVariable time period
            selections.AddVariableToHeading(contents, SelectionUtil.GetCodes);
            selections.AddVariableToHeading(time, SelectionUtil.GetTimeCodes);

            for (int i = 1; i < (mandatoryClassificationVariables.Count - 1); i++)
            {
                selections.AddVariableToHeading(mandatoryClassificationVariables[i], SelectionUtil.GetCodes);
            }

            //The variable with the most values should be in the placmentHeading
            var (stub, heading) = SelectionUtil.StubOrHeading(mandatoryClassificationVariables[0], mandatoryClassificationVariables[mandatoryClassificationVariables.Count - 1]);

            selections.AddStubVariable(stub, SelectionUtil.GetCodes);
            selections.AddHeadingVariable(heading, SelectionUtil.GetCodes);

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
            var selections = SelectionUtil.CreateEmptyVariablesSelection();

            //First content and lastNoneMandantoryClassificationVariable time period
            selections.AddVariableToHeading(contents, SelectionUtil.GetCodes);
            selections.AddVariableToHeading(time, SelectionUtil.GetTimeCodes);

            var lastNoneMandantoryClassificationVariable = noneMandatoryClassificationVariables[noneMandatoryClassificationVariables.Count - 1];
            var (stub, heading) = SelectionUtil.StubOrHeading(mandatoryClassificationVariables[0], lastNoneMandantoryClassificationVariable);
            selections.AddStubVariable(stub, SelectionUtil.GetCodes);
            selections.AddHeadingVariable(heading, SelectionUtil.GetCodes);

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
            var selections = SelectionUtil.CreateEmptyVariablesSelection();

            //First content and lastNoneMandantoryClassificationVariable time period
            selections.AddVariableToHeading(contents, SelectionUtil.GetCodes);
            selections.AddVariableToHeading(time, SelectionUtil.GetTimeCodes);

            var firstNoneMandantoryClassificationVariable = classificationVariables[0];
            var lastNoneMandantoryClassificationVariable = classificationVariables[classificationVariables.Count - 1];
            var (stub, heading) = SelectionUtil.StubOrHeading(firstNoneMandantoryClassificationVariable, lastNoneMandantoryClassificationVariable);
            selections.AddStubVariable(stub, SelectionUtil.GetCodes);
            selections.AddHeadingVariable(heading, SelectionUtil.GetCodes);


            foreach (var variable in noneMandatoryClassificationVariables)
            {
                if (variable != firstNoneMandantoryClassificationVariable && variable != lastNoneMandantoryClassificationVariable)
                {
                    selections.EliminateVariable(variable);
                }
            }

            return selections;
        }

        /// <summary>
        /// Reapply the codelist to the variables
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="meta"></param>
        private static void ReapplyCodelist(IPXModelBuilder builder, PXMeta meta)
        {
            foreach (var variable in meta.Variables)
            {

                var valueNotes = GetNotes(variable);

                if (variable.HasValuesets() && variable.CurrentGrouping is null && variable.CurrentValueSet is null)
                {
                    builder.ApplyValueSet(variable.Code, variable.ValueSets[0]);
                }
                else if (variable.CurrentGrouping != null)
                {
                    var grouping = variable.GetGroupingInfoById(variable.CurrentGrouping.ID);

                    if (grouping is null)
                    {
                        throw new Exception($"Could not find grouping {variable.CurrentGrouping.ID} for variable {variable.Code}");
                    }

                    builder.ApplyGrouping(variable.Code, variable.GetGroupingInfoById(variable.CurrentGrouping.ID), grouping.GroupPres);
                }
                else if (variable.CurrentValueSet != null)
                {
                    builder.ApplyValueSet(variable.Code, variable.CurrentValueSet);
                }
                var newVariable = builder.Model.Meta.Variables.FirstOrDefault(v => v.Code == variable.Code);
                if (newVariable is not null)
                {
                    ReapplyNotes(newVariable, valueNotes);
                }
            }
        }

        private static Dictionary<string, Notes> GetNotes(Variable variable)
        {
            var valueNotes = new Dictionary<string, Notes>();
            //Skip Content variables since they have not other valueset or grouping
            if (variable.IsContentVariable)
            {
                return valueNotes;
            }

            foreach (var value in variable.Values.Where(v => v.HasNotes()))
            {
                valueNotes.Add(value.Code, value.Notes);
            }
            return valueNotes;
        }

        private static void ReapplyNotes(Variable variable, Dictionary<string, Notes> valueNotes)
        {

            foreach (var valueCode in valueNotes.Keys)
            {
                var value = variable.Values.FirstOrDefault(v => v.Code == valueCode);
                if (value is not null && !value.HasNotes())
                {
                    foreach (var note in valueNotes[valueCode])
                    {
                        value.AddNote(note);
                    }
                }
            }
        }
    }
}
