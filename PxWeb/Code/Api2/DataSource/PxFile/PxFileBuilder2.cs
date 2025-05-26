using PCAxis.Paxiom;

namespace PxWeb.Code.Api2.DataSource.PxFile
{
    internal class PxFileBuilder2 : PXFileBuilder
    {
        public override bool BuildForSelection()
        {
            var meta = base.Model.Meta;
            var retVAlue = base.BuildForSelection();
            if (meta.ContentVariable is null)
            {
                // TODO: If there is no content variable add one
                Console.WriteLine("No content variable found in the model. This should be handled properly.");

                var contentVariable = new Variable("Metrics", "CONTENTS", PlacementType.Stub, meta.NumberOfLanguages);
                contentVariable.IsContentVariable = true;
                var value = new Value(meta.Contents, meta.NumberOfLanguages);
                PaxiomUtil.SetCode(value, "content");
                value.ContentInfo = meta.ContentInfo;
                meta.ContentInfo = null;

                contentVariable.Values.Add(value);
                meta.Stub.Insert(0, contentVariable);
                meta.Variables.Insert(0, contentVariable);
                meta.ContentVariable = contentVariable;

            }

            return retVAlue;

        }
    }
}
