using System.IO;
using System.Linq;

using PCAxis.Paxiom;
using PCAxis.Paxiom.Operations;

using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.ExtensionMethods;
using Px.Utils.PxFile.Data;

using PxWeb.Code.PxFile;

namespace PxWeb.PxFile
{
    public class PxBuilder : PXModelBuilderAdapter
    {

        private sealed record VariableGroupingInfo(Variable Variable, GroupingIncludesType IncludeType);

        private readonly Dictionary<string, VariableGroupingInfo> _originalVariables = [];

        private enum PostProcessingActionType
        {
            None,
            EliminateByValue,
            EliminateBySum
        }

        public override void SetPath(string path)
        {
            base.SetPath(path);
            m_path = path;
            var parser = new PxUtilsProxyParser(path);
            m_parser = parser;
        }

        protected virtual GroupRegistryWrapper GetGroupRegistryProvider()
        {
            return new GroupRegistryWrapper();
        }


        /// <summary>
        /// Builds the Meta part of the model based on the PX file and add groupings from the group registry to each variable.
        /// If any error occurs during parsing of the PX file, an error message is added to the builder's Errors collection and false is returned.
        /// </summary>
        /// <returns></returns>
        public override bool BuildForSelection()
        {
            try
            {
                base.BuildForSelection();
            }
            catch (PXModelParserException ex)
            {
                this.Errors.Add(new BuilderMessage("Error parsing the PX file: " + ex.Message));
                return false;
            }

            var groupRegistry = GetGroupRegistryProvider();

            if (!groupRegistry.IsLoaded) return true;

            var currentLanguageIndex = Model.Meta.CurrentLanguageIndex;

            for (int i = 0; i < Model.Meta.NumberOfLanguages; i++)
            {
                Model.Meta.SetLanguage(i);
                foreach (var variable in Model.Meta.Variables.Where(v => !string.IsNullOrEmpty(v.Domain)))
                {
                    foreach (var groupingInfo in groupRegistry.GetDefaultGroupings(variable.Domain))
                    {
                        variable.AddGrouping(groupingInfo);
                    }
                }
            }

            Model.Meta.SetLanguage(currentLanguageIndex);

            foreach (var variable in Model.Meta.Variables)
            {
                if (!variable.Values.ValuesHaveCodes)
                {
                    variable.Values.SetFictionalCodes();
                }
            }

            return true;
        }

        public override void ApplyGrouping(string variableCode, GroupingInfo groupingInfo, GroupingIncludesType include)
        {

            var groupRegistry = GetGroupRegistryProvider();

            var grouping = groupRegistry.GetGrouping(groupingInfo);
            if (grouping == null)
            {
                return;
            }

            grouping.GroupPres = include;

            var variable = this.Model.Meta.Variables.GetByCode(variableCode);

            // Store the original variable and the grouping include type in the builder. It will be used when the aggregated sum should be calculated in BuildForPresentation.
            var varOriginal = variable.CreateCopyWithValues();
            _originalVariables.TryAdd(variable.Code, new VariableGroupingInfo(varOriginal, include));


            variable.CurrentGrouping = grouping;

            //List of added values. Asserts that two values with the same code is not added. All value codes must be unique!
            var valueCodes = new HashSet<string>();


            //Initializes the Values collection
            variable.RecreateValues();

            // Adds the new values from the grouping
            foreach (var group in grouping.Groups)
            {
                // Add aggregated (group) values
                if (include == GroupingIncludesType.AggregatedValues || include == GroupingIncludesType.All)
                {
                    if (!valueCodes.Contains(group.GroupCode))
                    {
                        valueCodes.Add(group.GroupCode);
                        var val = new Value();
                        PaxiomUtil.SetCode(val, group.GroupCode);
                        val.Value = group.Name;
                        variable.Values.Add(val);
                    }
                }

                // Add single (child) values
                if (include == GroupingIncludesType.SingleValues || include == GroupingIncludesType.All)
                {
                    foreach (var child in group.ChildCodes)
                    {
                        if (!valueCodes.Contains(child.Code))
                        {
                            valueCodes.Add(child.Code);
                            var val = new Value();
                            PaxiomUtil.SetCode(val, child.Code);
                            val.Value = child.Name;
                            variable.Values.Add(val);
                        }
                    }
                }
            }

            // Removed the hierarchical information
            if (variable.Hierarchy.IsHierarchy)
            {
                variable.Hierarchy.RootLevel = null;
            }

            //'Remove elimination for variable 
            variable.Elimination = false;
            variable.EliminationValue = null;


            // Model no longer multilingual after grouping has been selected
            if (Model.Meta.NumberOfLanguages > 1)
            {
                int lang = Model.Meta.CurrentLanguageIndex;

                // Remove languages from model
                Model.Meta.DeleteAllLanguagesExceptCurrent();

                // Also remove languages from original variables stored in builder (if available)
                foreach (var kvp in _originalVariables)
                {
                    var v = kvp.Value.Variable;
                    PCAxis.Paxiom.VariableHelper.DeleteAllLanguagesExceptOne(v, lang);
                }
            }
        }


        protected virtual Stream GetStream()
        {
            return new FileStream(m_path, FileMode.Open, FileAccess.Read);
        }


        public override bool BuildForPresentation(Selection[] selection)
        {
            if (selection == null || selection.Length != Model.Meta.Variables.Count)
            {
                throw new PXException("Selection is null or selection contains all variables.");
            }


            var totalList = new List<IDimensionMap>();
            foreach (var variable in Model.Meta.Variables)
            {
                if (_originalVariables.TryGetValue(variable.Code, out VariableGroupingInfo? orgVariable))
                {
                    totalList.Add(new DimensionMap(
                            orgVariable.Variable.Code, orgVariable.Variable.Values.Select(val => val.Code).ToList()));
                }
                else
                {
                    totalList.Add(new DimensionMap(
                            variable.Code, variable.Values.Select(val => val.Code).ToList()));
                }
            }
            var totalMap = new MatrixMap(totalList);



            // Restore variables and modyfy selection for groupings
            var groupedVariables = RestoreOriginalVariablesForDataRetrival(selection);
            // Create the matrix map add selections for eliminated values
            var actions = RemoveUnselectedValues(selection);

            var targetMap = new MatrixMap(Model.Meta.Variables.Select(
                v => (IDimensionMap)(new DimensionMap(
                    v.Code, v.Values.Select(val => val.Code).ToList()))).ToList());

            using Stream fileStream = GetStream();


            SetMatrixSize();

            // Read data & build the matrix
            var buffer = new double[targetMap.GetSize()];
            using PxFileStreamDataReader dataReader = new(fileStream);
            var missingEncoding = new double[] { PXConstant.DATASYMBOL_NIL, PXConstant.DATASYMBOL_1, PXConstant.DATASYMBOL_2, PXConstant.DATASYMBOL_3, PXConstant.DATASYMBOL_4, PXConstant.DATASYMBOL_5 };
            dataReader.ReadUnsafeDoubles(buffer, 0, targetMap, totalMap, missingEncoding);

            var _ = m_model.Data.Write(buffer, 0, buffer.Length - 1);
            var elimOper = new Elimination();

            EliminationDescription[] elimDescriptions = actions.Where(
                a => a.Value == PostProcessingActionType.EliminateByValue ||
                a.Value == PostProcessingActionType.EliminateBySum)
             .Select(e => new EliminationDescription(e.Key, e.Value == PostProcessingActionType.EliminateByValue))
             .ToArray();
            m_model = elimOper.Execute(m_model, elimDescriptions);

            if (groupedVariables.Count > 0)
            {
                var sumGroupOp = new SumGrouping();

                var groupIncludes = groupedVariables.Select(v => _originalVariables[v.Code].IncludeType).ToList();
                var sumGroupDescriptions = new SumGroupingDescription()
                {
                    GroupVariables = groupedVariables,
                    KeepValues = groupIncludes
                };
                try
                {
                    m_model = sumGroupOp.Execute(m_model, sumGroupDescriptions);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during sum grouping: " + ex.Message);
                    throw;
                }

            }

            this.m_builderState = ModelBuilderStateType.BuildForPresentation;

            var time = Model.Meta.Variables.FirstOrDefault(v => v.IsTime);
            if (time != null)
            {
                time.BuildTimeValuesString();
            }

            Model.Meta.Prune();

            // TODO Trim notes etc
            return true;
        }

        // Substitutes variables in variables collection
        private void SubstituteVariables(Variables variables, Variable oldVar, Variable newVar)
        {
            int index = variables.IndexOf(oldVar);
            variables.RemoveAt(index);
            variables.Insert(index, newVar);
        }

        private void SubstituteVariablesInModel(Variable oldVar, Variable newVar)
        {
            // Substitute variables
            if (oldVar.Placement == PlacementType.Heading)
            {
                SubstituteVariables(Model.Meta.Heading, oldVar, newVar);
            }
            else
            {
                SubstituteVariables(Model.Meta.Stub, oldVar, newVar);
            }

            SubstituteVariables(Model.Meta.Variables, oldVar, newVar);
        }

        private Variables RestoreOriginalVariablesForDataRetrival(Selection[] selection)
        {
            var groupedVariables = new Variables();

            if (!Model.Meta.Variables.HasGroupingsApplied())
            {
                return groupedVariables;
            }


            var groupingValues = new Dictionary<string, List<Value>>();

            foreach (var variable in Model.Meta.Variables.Where(v => v.CurrentGrouping != null).ToList())
            {
                // 1. Substitue the grouped variable with the stored original variable
                var orgVariable = _originalVariables[variable.Code].Variable;

                orgVariable.Domain = null; // Remove domain for the variable - cannot be grouped as before
                orgVariable.Map = string.IsNullOrEmpty(variable.CurrentGrouping.Map) ? null : variable.CurrentGrouping.Map; // Set the map of the original variable to the map of the grouping (if any)
                SubstituteVariablesInModel(variable, orgVariable);

                var s = selection.First(sel => sel.VariableCode == variable.Code);

                groupedVariables.Add(variable);


                var copyOfGroupedValues = new List<Value>(variable.Values);
                groupingValues.Add(variable.Code, copyOfGroupedValues);

                variable.Values.RemoveAll(val => !s.ValueCodes.Contains(val.Code));

                var newSelectedValues = new HashSet<string>();

                foreach (var selectedCode in s.ValueCodes)
                {
                    if (selectedCode == null) continue;

                    var children = variable.CurrentGrouping.Groups.Where(g => g.GroupCode == selectedCode).SelectMany(g => g.ChildCodes);

                    if (children.Any())
                    {
                        foreach (var child in children)
                        {
                            newSelectedValues.Add(child.Code);
                        }
                    }
                    else
                    {
                        newSelectedValues.Add(selectedCode);
                    }
                }

                if (newSelectedValues.Count == 0)
                {
                    throw new ArgumentException($"Grouping, {variable.CurrentGrouping.ID}, could not match any values");
                }

                s.ValueCodes.Clear();
                s.ValueCodes.AddRange([.. newSelectedValues]);

            }

            return groupedVariables;
        }




        private Dictionary<string, PostProcessingActionType> RemoveUnselectedValues(Selection[] selection)
        {
            var actions = new Dictionary<string, PostProcessingActionType>();

            foreach (var s in selection)
            {
                var variable = Model.Meta.Variables.GetByCode(s.VariableCode);
                //var s = selection.FirstOrDefault(sel => sel.VariableCode == variable.Code);

                if (variable == null)
                {
                    throw new PXException("Variable not found for selection.");
                }

                if (s.ValueCodes.Count == 0)
                {
                    if (variable.Elimination)
                    {
                        if (variable.EliminationValue != null)
                        {


                            // Elimination is done by a specific value
                            // We need to add only the elimination value for the variable
                            var unusedValues = variable.Values.Where(val => val != variable.EliminationValue).ToList();
                            foreach (var val in unusedValues)
                            {
                                variable.Values.Remove(val);
                            }
                            actions.Add(variable.Code, PostProcessingActionType.EliminateByValue);
                        }
                        else
                        {
                            // Elimination is done by sum all values for the variable
                            // We need to add all values for the variable
                            actions.Add(variable.Code, PostProcessingActionType.EliminateBySum);
                        }
                    }
                    else
                    {
                        throw new PXException("No values selected for non-eliminated variable.");
                    }
                }
                else
                {
                    var codes = new List<string>();
                    // Check that the selected values exist
                    foreach (var valCode in s.ValueCodes)
                    {
                        var value = variable.Values.GetByCode(valCode);
                        if (value == null)
                        {
                            throw new PXException("Value not found for selection.");
                        }
                        if (valCode != null)
                        {
                            codes.Add(valCode);
                        }
                    }
                    var unselectedValues = variable.Values.Select(v => v.Code).Except(codes).ToList();
                    //Removed unselected values from the variable
                    foreach (var val in unselectedValues)
                    {
                        var value = variable.Values.GetByCode(val);
                        variable.Values.Remove(value);
                    }
                    actions.Add(variable.Code, PostProcessingActionType.None);
                }
            }
            return actions;
        }

        private void SetMatrixSize()
        {
            //The real number of columns in the entire data matrix includes unselected data
            int lDataColumnLength = 1;
            //The real number of rows in the entire data matrix includes unselected data
            int lDataRowLength = 1;
            foreach (Variable var in m_model.Meta.Heading)
            {
                lDataColumnLength *= var.Values.Count;
            }

            foreach (Variable var in m_model.Meta.Stub)
            {
                //lDataRowLength *= var.Values.Count;
                lDataRowLength *= Math.Max(1, var.Values.Count);
            }


            m_model.Data.SetMatrixSize(lDataRowLength, lDataColumnLength);
        }

        protected override Value FindValue(Variable variable, string findId)
        {
            return variable.Values.GetByName(findId);
        }

        protected override Variable FindVariable(PXMeta meta, string findId)
        {
            return meta.Variables.GetByName(findId, meta.CurrentLanguageIndex);

        }

        protected override Variable FindVariable(PXMeta meta, string findId, int lang)
        {
            return meta.Variables.GetByName(findId, lang);
        }
    }
}
