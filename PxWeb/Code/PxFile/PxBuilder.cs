using PCAxis.Paxiom;

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
            throw new NotImplementedException();
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
