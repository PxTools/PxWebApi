using System.Linq;
using System.Text;

using Microsoft.Extensions.Options;

using PCAxis.Metadata;
using PCAxis.Paxiom;
using PCAxis.Paxiom.Extensions;

using PxWeb.Api2.Server.Models;
using PxWeb.Models.Api2;

using Value = PCAxis.Paxiom.Value;

namespace PxWeb.Mappers
{
    public class DatasetMapper : IDatasetMapper
    {
        private readonly ILinkCreator _linkCreator;
        private readonly PxApiConfigurationOptions _configOptions;
        private string _language;

        private readonly MetaLinkManager _metaLinkManager = new MetaLinkManager();

        public DatasetMapper(ILinkCreator linkCreator, IOptions<PxApiConfigurationOptions> configOptions)
        {
            _linkCreator = linkCreator;
            _configOptions = configOptions.Value;
            _language = _configOptions.DefaultLanguage;
        }

        public Dataset Map(PXModel model, string id, string language)
        {

            _language = language;

            DatasetSubclass dataset = new DatasetSubclass();

            AddUpdated(model, dataset);

            //Source
            dataset.AddSource(model.Meta.Source);

            //Label
            dataset.AddLabel(model.Meta.Title);

            //Extension PX
            AddPxToExtension(model, dataset);

            // Dimension
            //Handle Elminated content variable

            if (model.Meta.ContentVariable == null)
            {
                AddInfoForEliminatedContentVariable(model, dataset);
            }

            foreach (var variable in model.Meta.Variables)
            {
                //temporary collector storage
                var metaIdsHelper = new Dictionary<string, string>();

                dataset.AddDimensionValue(variable.Code, variable.Name, out var dimensionValue);

                var indexCounter = 0;

                foreach (var variableValue in variable.Values)
                {
                    dimensionValue.Category.Label.Add(variableValue.Code, variableValue.Value);
                    dimensionValue.Category.Index.Add(variableValue.Code, indexCounter++);

                    CollectMetaIdsForValue(variableValue, ref metaIdsHelper);

                    // ValueNote
                    AddValueNotes(variableValue, dataset, dimensionValue);


                    if (!variable.IsContentVariable) continue;

                    var unitDecimals = (variableValue.HasPrecision()) ? variableValue.Precision : model.Meta.ShowDecimals;
                    dataset.AddUnitValue(dimensionValue.Category, out var unitValue);

                    if (variableValue.ContentInfo != null)
                    {
                        unitValue.Base = variableValue.ContentInfo.Units;
                        unitValue.Decimals = unitDecimals;

                        //refPeriod extension dimension
                        dataset.AddRefPeriod(dimensionValue, variableValue.Code, variableValue.ContentInfo.RefPeriod);

                        // Contact
                        AddContact(dataset, variableValue.ContentInfo);
                    }
                    else
                    {
                        //TODO
                        //  _logger.Warn("Category" + variableValue.Code + " lacks ContentInfo. Unit, refPeriod and contact not set");
                    }

                    dimensionValue.Category.Unit.Add(variableValue.Code, unitValue);
                }

                //elimination
                AddEliminationInfo(dimensionValue, variable);

                //Show
                AddShow(dimensionValue, variable);

                //Variable notes
                AddVariableNotes(variable, dataset, dimensionValue);

                //MetaID
                CollectMetaIdsForVariable(variable, ref metaIdsHelper);

                if (metaIdsHelper.Count > 0)
                {
                    dataset.AddDimensionLink(dimensionValue, metaIdsHelper);
                }


                //Codelists
                var codeLists = new System.Collections.Generic.List<CodeListInformation>();
                MapCodelists(codeLists, variable);
                if (codeLists != null)
                {
                    dataset.AddCodelist(dimensionValue, codeLists);
                }


                dataset.Size.Add(variable.Values.Count);
                dataset.Id.Add(variable.Code);

                //Role
                AddRoles(variable, dataset);
            }

            AddTableNotes(model, dataset);

            List<Link> linksOnRoot = new List<Link>();
            linksOnRoot.AddRange(_linkCreator.GetTableMetadataJsonLink(LinkCreator.LinkRelationEnum.self, id.ToUpper(), language, true));
            linksOnRoot.Add(_linkCreator.GetTableDataLink(LinkCreator.LinkRelationEnum.data, id.ToUpper(), language, true));

            //"type": "application/json"

            // TODO: Links to documentation
            //if (!string.IsNullOrEmpty(model.Meta.MetaId))
            //{
            //}

            dataset.AddLinksOnRoot(linksOnRoot);


            return dataset;
        }


        private void AddInfoForEliminatedContentVariable(PXModel model, DatasetSubclass dataset)
        {
            dataset.AddDimensionValue("ContentsCode", "EliminatedContents", out var dimensionValue);
            dimensionValue.Category.Label.Add("EliminatedValue", model.Meta.Contents);
            dimensionValue.Category.Index.Add("EliminatedValue", 0);

            dataset.AddUnitValue(dimensionValue.Category, out var unitValue);
            unitValue.Base = model.Meta.ContentInfo.Units;
            unitValue.Decimals = model.Meta.Decimals;

            dimensionValue.Category.Unit.Add("EliminatedValue", unitValue);

            dimensionValue.Extension.Elimination = true;

            //refPeriod extension dimension
            dataset.AddRefPeriod(dimensionValue, "EliminatedValue", model.Meta.ContentInfo.RefPeriod);

            // Contact
            AddContact(dataset, model.Meta.ContentInfo);
        }

        private void AddUpdated(PXModel model, DatasetSubclass dataset)
        {
            DateTime tempDateTime;
            if (model.Meta.ContentVariable != null && model.Meta.ContentVariable.Values.Count > 0)
            {
                var lastUpdatedContentsVariable = model.Meta.ContentVariable.Values
                    .OrderByDescending(x => x.ContentInfo.LastUpdated)
                    .FirstOrDefault();

                // ReSharper disable once PossibleNullReferenceException
                if (lastUpdatedContentsVariable != null)
                {
                    tempDateTime = lastUpdatedContentsVariable.ContentInfo.LastUpdated.PxDateStringToDateTime();
                }
                else
                {
                    tempDateTime = model.Meta.CreationDate.PxDateStringToDateTime();
                }
            }
            else if (model.Meta.ContentInfo.LastUpdated != null)
            {
                tempDateTime = model.Meta.ContentInfo.LastUpdated.PxDateStringToDateTime();
            }
            else
            {
                tempDateTime = model.Meta.CreationDate.PxDateStringToDateTime();
            }

            dataset.SetUpdatedAsUtcString(tempDateTime);

            /*
            if (contInfo.LastUpdated.IsPxDate())
            {
                DateTime tryDate = contInfo.LastUpdated.PxDateStringToDateTime().ToUniversalTime();
                if (tm.Updated == null || tryDate > tm.Updated)
                {
                    tm.Updated = tryDate;
                }
            }
             */
        }

        private void AddPxToExtension(PXModel model, DatasetSubclass dataset)
        {
            // TODO should we have included both Decimals and ShowDecimals?
            var decimals = model.Meta.ShowDecimals < 0 ? model.Meta.Decimals : model.Meta.ShowDecimals;

            dataset.AddInfoFile(model.Meta.InfoFile);
            dataset.AddTableId(model.Meta.TableID);
            dataset.AddDecimals(decimals);
            dataset.AddContents(model.Meta.Contents);
            dataset.AddDescription(model.Meta.Description);
            dataset.AddDescriptiondefault(model.Meta.DescriptionDefault);
            dataset.AddStub(model.Meta.Stub.Select(v => v.Name).ToList());
            dataset.AddHeading(model.Meta.Heading.Select(v => v.Name).ToList());
            dataset.AddLanguage(model.Meta.Language);
            dataset.AddOfficialStatistics(model.Meta.OfficialStatistics);
            dataset.AddMatrix(model.Meta.Matrix);
            dataset.AddSubjectCode(model.Meta.SubjectCode);
            dataset.AddSubjectArea(model.Meta.SubjectArea);
            dataset.AddAggRegAllowed(model.Meta.AggregAllowed);
        }

        private void AddTableNotes(PXModel model, DatasetSubclass dataset)
        {
            var notes = model.Meta.Notes.Where(note => note.Type == NoteType.Table);

            var noteIndex = 0;
            foreach (var note in notes)
            {

                dataset.AddTableNote(note.Text);

                if (note.Mandantory)
                    dataset.AddIsMandatoryForTableNote(noteIndex.ToString());

                noteIndex++;

            }
        }

        private void AddEliminationInfo(DatasetDimensionValue dimensionValue, Variable variable)
        {
            dimensionValue.Extension.Elimination = variable.Elimination;

            if (!variable.Elimination || variable.EliminationValue == null) return;

            if (string.IsNullOrEmpty(variable.EliminationValue.Code)) return;
            dimensionValue.Extension.EliminationValueCode = variable.EliminationValue.Code;
        }

        private void AddShow(DatasetDimensionValue dimensionValue, Variable variable)
        {
            if (Enum.TryParse(variable.PresentationText.ToString(), out PresentationFormType presentationForm))
            {
                dimensionValue.Extension.Show = presentationForm.ToString().ToLower();
            }
        }

        private void AddValueNotes(Value variableValue, DatasetSubclass dataset, DatasetDimensionValue dimensionValue)
        {
            if (variableValue.Notes == null) return;

            var index = 0;
            foreach (var note in variableValue.Notes)
            {
                //dataset.AddValueNoteToDimension(dimensionValue, variableValue.Code, note.Mandantory, note.Text);
                dataset.AddValueNoteToCategory(dimensionValue, variableValue.Code, note.Text);

                if (note.Mandantory)
                    dataset.AddIsMandatoryForCategoryNote(dimensionValue, variableValue.Code, index.ToString());

                index++;
            }
        }

        private void AddVariableNotes(Variable variable, DatasetSubclass dataset, DatasetDimensionValue dimensionValue)
        {
            if (variable.Notes == null) return;

            var noteIndex = 0;
            foreach (var note in variable.Notes)
            {
                dataset.AddNoteToDimension(dimensionValue, note.Text);

                if (note.Mandantory)
                    dataset.AddIsMandatoryForDimensionNote(dimensionValue, noteIndex.ToString());

                noteIndex++;
            }
        }

        private void AddContact(DatasetSubclass dataset, ContInfo contInfo)
        {
            if (contInfo.ContactInfo != null && contInfo.ContactInfo.Count > 0)
            {
                foreach (var contact in contInfo.ContactInfo)
                {
                    MapContact(dataset, contact, contInfo);
                }
            }
            else
            {
                MapContact(dataset, contInfo.Contact);
            }
        }

        private void MapContact(DatasetSubclass dataset, PCAxis.Paxiom.Contact contact, ContInfo contInfo)
        {

            if (dataset.Extension.Contact == null)
            {
                dataset.Extension.Contact = new List<Api2.Server.Models.Contact>();
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(contact.Forname);
            sb.Append(" ");
            sb.Append(contact.Surname);

            Api2.Server.Models.Contact jsonContact = new Api2.Server.Models.Contact
            {
                Name = sb.ToString(),
                Mail = contact.Email,
                Phone = contact.PhoneNo
            };

            if (contInfo.Contact != null)
            {
                var contacts = contInfo.Contact.Split(new[] { "##" }, StringSplitOptions.RemoveEmptyEntries);
                var res = contacts.Where(x => x.Contains(contact.Forname) && x.Contains(contact.Surname) && x.Contains(contact.Email) && x.Contains(contact.PhoneNo)).FirstOrDefault();

                if (res != null)
                {
                    jsonContact.Raw = res;
                }
            }

            // Only display unique contact once
            if (!dataset.Extension.Contact.Exists(x => x.Mail.Equals(jsonContact.Mail) && x.Name.Equals(jsonContact.Name) && x.Phone.Equals(jsonContact.Phone)))
            {
                dataset.Extension.Contact.Add(jsonContact);
            }
        }

        private void MapContact(DatasetSubclass dataset, string contactString)
        {
            if (contactString != null)
            {
                if (dataset.Extension.Contact == null)
                {
                    dataset.Extension.Contact = new List<Api2.Server.Models.Contact>();
                }

                var contacts = contactString.Split(new[] { "##" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var contact in contacts)
                {
                    if (!dataset.Extension.Contact.Exists(x => x.Raw.Equals(contact)))
                    {
                        dataset.Extension.Contact.Add(new Api2.Server.Models.Contact
                        {
                            Raw = contact
                        });
                    }
                }
            }
        }

        private void AddRoles(Variable variable, DatasetSubclass dataset)
        {
            if (variable.IsTime)
            {
                dataset.AddToTimeRole(variable.Code);
            }

            if (variable.IsContentVariable)
            {
                dataset.AddToMetricRole(variable.Code);
            }

            if (variable.VariableType == null) return;
            if (variable.VariableType.ToUpper() == "G" || (variable.Map != null))
            {
                dataset.AddToGeoRole(variable.Code);
            }
        }

        private void CollectMetaIdsForVariable(Variable variable, ref Dictionary<string, string> metaIds)
        {
            if (!string.IsNullOrWhiteSpace(variable.MetaId))
            {
                metaIds.Add(variable.Code, SerializeMetaIds(variable.MetaId));
            }
        }

        private void CollectMetaIdsForValue(Value value, ref Dictionary<string, string> metaIds)
        {
            if (!string.IsNullOrWhiteSpace(value.MetaId))
            {
                metaIds.Add(value.Code, SerializeMetaIds(value.MetaId));
            }
        }

        private string SerializeMetaIds(string metaId)
        {
            var metaIds = metaId.Split(_metaLinkManager.GetSystemSeparator(), StringSplitOptions.RemoveEmptyEntries);
            var metaIdsAsString = new List<string>();
            foreach (var meta in metaIds)
            {
                var metaLinks = meta.Split(_metaLinkManager.GetParamSeparator(), StringSplitOptions.RemoveEmptyEntries);
                if (metaLinks.Length > 0)
                {
                    metaIdsAsString.Add(meta);
                }
            }
            return (string.Join(" ", metaIdsAsString));
        }



        // TODO: HIERARCHIES(“Country”)="parent","parent":"child",  (I think child in dimention for jsonstat)

        // TODO: the below is cloned from TableMetadataResponseMapper, should be moved to common class



        private Api2.Server.Models.CodeListInformation Map(PCAxis.Paxiom.GroupingInfo grouping)
        {
            CodeListInformation codelist = new CodeListInformation();

            codelist.Id = "agg_" + grouping.ID;
            codelist.Label = grouping.Name;
            codelist.Type = CodeListType.AggregationEnum;
            codelist.Links = new System.Collections.Generic.List<Link>();
            codelist.Links.Add(_linkCreator.GetCodelistLink(LinkCreator.LinkRelationEnum.metadata, codelist.Id, _language));

            return codelist;
        }
        private Api2.Server.Models.CodeListInformation Map(PCAxis.Paxiom.ValueSetInfo valueset)
        {
            CodeListInformation codelist = new CodeListInformation();

            codelist.Id = "vs_" + valueset.ID;
            codelist.Label = valueset.Name;
            codelist.Type = CodeListType.ValuesetEnum;
            codelist.Links = new System.Collections.Generic.List<Link>();
            codelist.Links.Add(_linkCreator.GetCodelistLink(LinkCreator.LinkRelationEnum.metadata, codelist.Id, _language));

            return codelist;
        }


        private void MapCodelists(System.Collections.Generic.List<CodeListInformation> codelists, Variable variable)
        {
            if (variable.HasGroupings())
            {
                foreach (var grouping in variable.Groupings)
                {
                    codelists.Add(Map(grouping));
                }
            }

            if (variable.HasValuesets())
            {
                foreach (var valueset in variable.ValueSets)
                {
                    if (!valueset.ID.Equals("_ALL_"))
                    {
                        codelists.Add(Map(valueset));
                    }
                }
            }
        }

    }
}
