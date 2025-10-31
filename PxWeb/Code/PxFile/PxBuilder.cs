using System.IO;
using System.Linq;

using PCAxis.Paxiom;

using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.ExtensionMethods;
using Px.Utils.PxFile.Data;

namespace PxWeb.PxFile
{
    public class PxBuilder : PXModelBuilderAdapter
    {

        public override void SetPath(string path)
        {
            base.SetPath(path);
            m_path = path;
            var parser = new PxUtilsProxyParser(path);
            m_parser = parser;
        }

        public override bool BuildForPresentation(Selection[] selection)
        {
            if (selection == null || selection.Length != Model.Meta.Variables.Count)
            {
                throw new PXException("Selection is null or selection contains all variables.");
            }

            var totalMap = new MatrixMap(Model.Meta.Variables.Select(
                    v => (IDimensionMap)(new DimensionMap(
                            v.Code, v.Values.Select(val => val.Code).ToList()))).ToList());

            // TODO Handle aggregations
            // Create the matrix map add selections for eliminated values
            RemoveUnselectedValues(selection);
            var targetMap = new MatrixMap(Model.Meta.Variables.Select(
                    v => (IDimensionMap)(new DimensionMap(
                            v.Code, v.Values.Select(val => val.Code).ToList()))).ToList());

            using Stream fileStream = new FileStream(m_path, FileMode.Open, FileAccess.Read);
            fileStream.Position = 0;


            SetMatrixSize();

            // Read data & build the matrix
            var buffer = new double[targetMap.GetSize()];
            using PxFileStreamDataReader dataReader = new(fileStream);
            var missingEncoding = new double[] { PXConstant.DATASYMBOL_NIL, PXConstant.DATASYMBOL_1, PXConstant.DATASYMBOL_2, PXConstant.DATASYMBOL_3, PXConstant.DATASYMBOL_4, PXConstant.DATASYMBOL_5 };
            dataReader.ReadUnsafeDoubles(buffer, 0, targetMap, totalMap, missingEncoding);

            var count = m_model.Data.Write(buffer, 0, buffer.Length - 1);

            // TODO Handle eliminations
            // TODO Handle aggregations
            // TODO Trim notes etc

            return true;
        }

        private void RemoveUnselectedValues(Selection[] selection)
        {
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
                        }
                        else
                        {
                            // Elimination is done by sum all values for the variable
                            // We need to add all values for the variable
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
                }
            }
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
