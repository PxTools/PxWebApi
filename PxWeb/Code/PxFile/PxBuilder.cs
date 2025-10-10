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
            var map = new MatrixMap(Model.Meta.Variables.Select(
                    v => (IDimensionMap)(new DimensionMap(
                            v.Code, v.Values.Select(val => val.Code).ToList()))).ToList());

            using Stream fileStream = new FileStream(m_path, FileMode.Open, FileAccess.Read);
            fileStream.Position = 0;


            SetMatrixSize();

            // Read data & build the matrix
            var buffer = new double[map.GetSize()];
            using PxFileStreamDataReader dataReader = new(fileStream);
            var missingEncoding = new double[] { PXConstant.DATASYMBOL_NIL, PXConstant.DATASYMBOL_1, PXConstant.DATASYMBOL_2, PXConstant.DATASYMBOL_3, PXConstant.DATASYMBOL_4, PXConstant.DATASYMBOL_5 };
            dataReader.ReadUnsafeDoubles(buffer, 0, map, map, missingEncoding);

            var count = m_model.Data.Write(buffer, 0, buffer.Length - 1);
            return true;
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
