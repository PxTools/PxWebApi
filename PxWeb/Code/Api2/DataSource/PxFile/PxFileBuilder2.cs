using PCAxis.Paxiom;

using PxWeb.Code.PxFile;

namespace PxWeb.Code.Api2.DataSource.PxFile
{


    internal class PxFileBuilder2 : PXFileBuilder
    {
        public override bool BuildForSelection()
        {
            var meta = base.Model.Meta;
            var result = base.BuildForSelection();
            ContentsUtil.AssertContentsVariableExists(meta);

            return result;

        }
    }
}
