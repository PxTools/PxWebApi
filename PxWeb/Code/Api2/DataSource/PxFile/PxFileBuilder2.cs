using System.Globalization;

using PCAxis.Paxiom;

namespace PxWeb.Code.Api2.DataSource.PxFile
{
    internal class PxFileBuilder2 : PXFileBuilder
    {
        public override bool BuildForSelection()
        {
            var meta = base.Model.Meta;
            var result = base.BuildForSelection();
            if (meta.ContentVariable is null)
            {
                // If there is no content variable, we create one.

                try
                {
                    // The name of the content variable is localized based on the language of the metadata.
                    var name = PCAxis.Paxiom.Localization.PxResourceManager.GetResourceManager()
                        .GetString("ApiContentsVariableName", new CultureInfo(meta.Language));

                    // Create the content variable
                    var contentVariable = new Variable(name, "CONTENTS", PlacementType.Stub, meta.NumberOfLanguages);
                    contentVariable.IsContentVariable = true;

                    // Create the value for the content variable from the meta contents
                    var value = new Value(meta.Contents, meta.NumberOfLanguages);
                    PaxiomUtil.SetCode(value, "content");
                    // Move the contentInfo from tablelevel to the content value
                    value.ContentInfo = meta.ContentInfo;
                    meta.ContentInfo = null;
                    contentVariable.Values.Add(value);

                    // Insert the content variable as the first variable in the stub
                    meta.Stub.Insert(0, contentVariable);
                    meta.Variables.Insert(0, contentVariable);
                    meta.ContentVariable = contentVariable;

                    // Add text and ContentInfo for all languages
                    var languages = meta.GetAllLanguages();
                    var currentLanguage = meta.CurrentLanguage;
                    for (int i = 0; i < languages.Length; i++)
                    {
                        var lang = languages[i];

                        meta.SetLanguage(i);

                        // Add the label for each language
                        contentVariable.Name = PCAxis.Paxiom.Localization.PxResourceManager.GetResourceManager()
                            .GetString("ApiContentsVariableName", new CultureInfo(lang));

                        // Set the value text in diffrent languages
                        value.Value = meta.Contents;
                    }
                    meta.SetLanguage(currentLanguage);
                }
                catch (Exception)
                {
                    result = false;
                }
            }

            return result;

        }
    }
}
