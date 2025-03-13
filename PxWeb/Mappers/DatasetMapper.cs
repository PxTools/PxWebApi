using System.Globalization;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        private string _language;

        private readonly MetaLinkManager _metaLinkManager = new MetaLinkManager();

        public DatasetMapper(ILinkCreator linkCreator, IOptions<PxApiConfigurationOptions> configOptions, ILogger<DatasetMapper> logger)
        {
            _linkCreator = linkCreator;
            _configOptions = configOptions.Value;
            _language = _configOptions.DefaultLanguage;
            _logger = logger;
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
                    if (dimensionValue.Category is not null)
                    {
                        dimensionValue.Category.Label.Add(variableValue.Code, variableValue.Value);
                        dimensionValue.Category.Index.Add(variableValue.Code, indexCounter++);
                    }

                    CollectMetaIdsForValue(variableValue, ref metaIdsHelper);

                    // ValueNote
                    AddValueNotes(variableValue, dimensionValue);

                    if (!variable.IsContentVariable) continue;

                    var unitDecimals = (variableValue.HasPrecision()) ? variableValue.Precision : model.Meta.ShowDecimals;
                    if (dimensionValue.Category is not null)
                    {
                        DatasetSubclass.AddUnitValue(dimensionValue.Category, out var unitValue);

                        if (variableValue.ContentInfo != null)
                        {
                            unitValue.Base = variableValue.ContentInfo.Units;
                            unitValue.Decimals = unitDecimals;

                            //refPeriod extension dimension
                            DatasetSubclass.AddRefPeriod(dimensionValue, variableValue.Code, variableValue.ContentInfo.RefPeriod);

                            //measuringType extension dimension
                            DatasetSubclass.AddMeasuringType(dimensionValue, variableValue.Code, GetMeasuringType(variableValue.ContentInfo.StockFa));

                            //priceType extension dimension
                            DatasetSubclass.AddPriceType(dimensionValue, variableValue.Code, GetPriceType(variableValue.ContentInfo.CFPrices));

                            //adjustment extension dimension
                            DatasetSubclass.AddAdjustment(dimensionValue, variableValue.Code, GetAdjustment(variableValue.ContentInfo.DayAdj, variableValue.ContentInfo.SeasAdj));

                            //basePeriod extension dimension
                            DatasetSubclass.AddBasePeriod(dimensionValue, variableValue.Code, variableValue.ContentInfo.Baseperiod);

                            // Contact
                            AddContact(dataset, variableValue.ContentInfo);
                        }
                        else
                        {
                            _logger.LogWarning("Category {CategoryCode} lacks ContentInfo. Unit, refPeriod and contact not set", variableValue.Code);
                        }

                        dimensionValue.Category.Unit.Add(variableValue.Code, unitValue);
                    }

                }

                //elimination
                AddEliminationInfo(dimensionValue, variable);

                //Show
                AddShow(dimensionValue, variable);

                //Variable notes
                AddVariableNotes(variable, dimensionValue);

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
                    DatasetSubclass.AddCodelist(dimensionValue, codeLists);
                }


                dataset.Size.Add(variable.Values.Count);
                dataset.Id.Add(variable.Code);

                //Role
                AddRoles(variable, dataset);
            }

            AddTableNotes(model, dataset);

            List<Link> linksOnRoot = new List<Link>();
            linksOnRoot.Add(_linkCreator.GetTableMetadataJsonLink(LinkCreator.LinkRelationEnum.self, id.ToUpper(), language, true));
            linksOnRoot.Add(_linkCreator.GetTableDataLink(LinkCreator.LinkRelationEnum.data, id.ToUpper(), language, true));

            //"type": "application/json"

            // TODO: Links to documentation
            //if (!string.IsNullOrEmpty(model.Meta.MetaId))

            dataset.AddLinksOnRoot(linksOnRoot);


            return dataset;
        }

        private static PriceType GetPriceType(string cfprices)
        {
            string cfp = cfprices != null ? cfprices.ToUpper() : "";

            switch (cfp)
            {
                case "C":
                    return PriceType.CurrentEnum;
                case "F":
                    return PriceType.FixedEnum;
                default:
                    return PriceType.NotApplicableEnum;
            }
        }

        private static Adjustment GetAdjustment(string dayAdj, string seasAdj)
        {
            string dadj = dayAdj != null ? dayAdj.ToUpper() : "";
            string sadj = seasAdj != null ? seasAdj.ToUpper() : "";

            if (dadj.Equals("YES") && sadj.Equals("YES"))
            {
                return Adjustment.WorkAndSesEnum;
            }
            else if (sadj.Equals("YES"))
            {
                return Adjustment.SesOnlyEnum;
            }
            else if (dadj.Equals("YES"))
            {
                return Adjustment.WorkOnlyEnum;
            }
            else
            {
                return Adjustment.NoneEnum;
            }
        }

        private static MeasuringType GetMeasuringType(string stockfa)
        {
            if (stockfa == null)
            {
                return MeasuringType.OtherEnum;
            }
            switch (stockfa.ToUpper())
            {
                case "S":
                    return MeasuringType.StockEnum;
                case "F":
                    return MeasuringType.FlowEnum;
                case "A":
                    return MeasuringType.AverageEnum;
                default:
                    return MeasuringType.OtherEnum;
            }
        }

        private void AddInfoForEliminatedContentVariable(PXModel model, DatasetSubclass dataset)
        {
            var eliminatedValue = "EliminatedValue";
            dataset.AddDimensionValue("ContentsCode", "EliminatedContents", out var dimensionValue);
            if (dimensionValue.Category is not null)
            {
                dimensionValue.Category.Label.Add(eliminatedValue, model.Meta.Contents);
                dimensionValue.Category.Index.Add(eliminatedValue, 0);

                DatasetSubclass.AddUnitValue(dimensionValue.Category, out var unitValue);
                unitValue.Base = model.Meta.ContentInfo.Units;
                unitValue.Decimals = model.Meta.Decimals;

                dimensionValue.Category.Unit.Add(eliminatedValue, unitValue);
            }
            if (dimensionValue.Extension is not null)
            {
                dimensionValue.Extension.Elimination = true;
            }

            //refPeriod extension dimension
            DatasetSubclass.AddRefPeriod(dimensionValue, eliminatedValue, model.Meta.ContentInfo.RefPeriod);

            //measuringType extension dimension
            DatasetSubclass.AddMeasuringType(dimensionValue, eliminatedValue, GetMeasuringType(model.Meta.ContentInfo.StockFa));

            //priceType extension dimension
            DatasetSubclass.AddPriceType(dimensionValue, eliminatedValue, GetPriceType(model.Meta.ContentInfo.CFPrices));

            //adjustment extension dimension
            DatasetSubclass.AddAdjustment(dimensionValue, eliminatedValue, GetAdjustment(model.Meta.ContentInfo.DayAdj, model.Meta.ContentInfo.SeasAdj));

            //basePeriod extension dimension
            DatasetSubclass.AddBasePeriod(dimensionValue, eliminatedValue, model.Meta.ContentInfo.Baseperiod);

            // Contact
            AddContact(dataset, model.Meta.ContentInfo);

            dataset.AddToMetricRole("ContentsCode");
            dataset.Size.Add(1);
            dataset.Id.Add("ContentsCode");
        }

        private static void AddUpdated(PXModel model, DatasetSubclass dataset)
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

            dataset.Updated = DateTimeAsUtcString(tempDateTime);
        }

        public static string DateTimeAsUtcString(DateTime datetime)
        {
            return datetime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        }

        private static void AddNextUpdate(Dataset dataset, string nextUpdate)
        {
            if (nextUpdate != null)
            {
                DateTime tempDatetime;
                tempDatetime = nextUpdate.PxDateStringToDateTime();

                dataset.Extension ??= new ExtensionRoot();
                dataset.Extension.Px ??= new ExtensionRootPx();
                dataset.Extension.Px.NextUpdate = DateTimeAsUtcString(tempDatetime);
            }
        }

        private static void AddPxToExtension(PXModel model, DatasetSubclass dataset)
        {
            // TODO should we have included both Decimals and ShowDecimals?
            var decimals = model.Meta.ShowDecimals < 0 ? model.Meta.Decimals : model.Meta.ShowDecimals;

            dataset.AddInfoFile(model.Meta.InfoFile);
            dataset.AddTableId(model.Meta.TableID);
            dataset.AddDecimals(decimals);
            dataset.AddContents(model.Meta.Contents);
            dataset.AddDescription(model.Meta.Description);
            dataset.AddDescriptiondefault(model.Meta.DescriptionDefault);
            dataset.AddStub(model.Meta.Stub.Select(v => v.Code).ToList());
            dataset.AddHeading(model.Meta.Heading.Select(v => v.Code).ToList());
            dataset.AddLanguage(model.Meta.Language);
            dataset.AddOfficialStatistics(model.Meta.OfficialStatistics);
            dataset.AddCopyright(model.Meta.Copyright);
            dataset.AddMatrix(model.Meta.Matrix);
            dataset.AddSubjectCode(model.Meta.SubjectCode);
            dataset.AddSubjectArea(model.Meta.SubjectArea);
            dataset.AddAggRegAllowed(model.Meta.AggregAllowed);
            dataset.AddSurvey(model.Meta.Survey);
            dataset.AddLink(model.Meta.Link);
            dataset.AddUpdateFrequency(model.Meta.UpdateFrequency);
            AddNextUpdate(dataset, model.Meta.NextUpdate);
        }

        private static void AddTableNotes(PXModel model, DatasetSubclass dataset)
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

        private static void AddEliminationInfo(DimensionValue dimensionValue, Variable variable)
        {
            if (dimensionValue.Extension is null)
            {
                dimensionValue.Extension = new ExtensionDimension();
            }
            dimensionValue.Extension.Elimination = variable.Elimination;

            if (!variable.Elimination || variable.EliminationValue == null) return;

            if (string.IsNullOrEmpty(variable.EliminationValue.Code)) return;
            dimensionValue.Extension.EliminationValueCode = variable.EliminationValue.Code;
        }

        private static void AddShow(DimensionValue dimensionValue, Variable variable)
        {
            if (Enum.TryParse(variable.PresentationText.ToString(), out PresentationFormType presentationForm))
            {
                if (dimensionValue.Extension is null)
                {
                    dimensionValue.Extension = new ExtensionDimension();
                }
                dimensionValue.Extension.Show = presentationForm.ToString().ToLower();
            }
        }

        private static void AddValueNotes(Value variableValue, DimensionValue dimensionValue)
        {
            if (variableValue.Notes == null) return;

            var index = 0;
            foreach (var note in variableValue.Notes)
            {
                DatasetSubclass.AddValueNoteToCategory(dimensionValue, variableValue.Code, note.Text);

                if (note.Mandantory)
                    DatasetSubclass.AddIsMandatoryForCategoryNote(dimensionValue, variableValue.Code, index.ToString());

                index++;
            }
        }

        private static void AddVariableNotes(Variable variable, DimensionValue dimensionValue)
        {
            if (variable.Notes == null) return;

            var noteIndex = 0;
            foreach (var note in variable.Notes)
            {
                DatasetSubclass.AddNoteToDimension(dimensionValue, note.Text);

                if (note.Mandantory)
                    DatasetSubclass.AddIsMandatoryForDimensionNote(dimensionValue, noteIndex.ToString());

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

            if (dataset.Extension is not null && dataset.Extension.Contact == null)
            {
                dataset.Extension.Contact = new List<Api2.Server.Models.Contact>();
            }

            Api2.Server.Models.Contact jsonContact = new Api2.Server.Models.Contact
            {
                Name = GetFullName(contact),
                Mail = contact.Email,
                Phone = contact.PhoneNo,
                Organization = contact.OrganizationName
            };

            if (contInfo.Contact != null)
            {
                var contacts = contInfo.Contact.Split(new[] { "##" }, StringSplitOptions.RemoveEmptyEntries);
                var res = contacts.FirstOrDefault(x => x.Contains(contact.Forname) &&
                                                       x.Contains(contact.Surname) &&
                                                       x.Contains(contact.Email) &&
                                                       x.Contains(contact.PhoneNo) &&
                                                       x.Contains(contact.OrganizationName));

                if (res != null)
                {
                    jsonContact.Raw = res;
                }
            }

            // Only display unique contact once
            if (dataset.Extension is null)
            {
                dataset.Extension = new ExtensionRoot();
            }
            if (!dataset.Extension.Contact.Exists(x => x.Mail is not null &&
                                                       x.Name is not null &&
                                                       x.Phone is not null &&
                                                       x.Organization is not null &&
                                                       x.Mail.Equals(jsonContact.Mail) &&
                                                       x.Name.Equals(jsonContact.Name) &&
                                                       x.Phone.Equals(jsonContact.Phone) &&
                                                       x.Organization.Equals(jsonContact.Organization)))
            {
                dataset.Extension.Contact.Add(jsonContact);
            }
        }

        private void MapContact(DatasetSubclass dataset, string contactString)
        {
            if (contactString != null)
            {
                if (dataset.Extension is null)
                {
                    dataset.Extension = new ExtensionRoot();
                }

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

        private static string GetFullName(PCAxis.Paxiom.Contact contact)
        {
            if (string.IsNullOrEmpty(contact.Forname) && string.IsNullOrEmpty(contact.Surname))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(contact.Forname))
            {
                return contact.Surname;
            }

            if (string.IsNullOrEmpty(contact.Surname))
            {
                return contact.Forname;
            }

            return $"{contact.Forname} {contact.Surname}";
        }

        private static void AddRoles(Variable variable, DatasetSubclass dataset)
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



        private CodeListInformation Map(PCAxis.Paxiom.GroupingInfo grouping)
        {
            CodeListInformation codelist = new CodeListInformation();

            codelist.Id = "agg_" + grouping.ID;
            codelist.Label = grouping.Name;
            codelist.Type = CodeListType.AggregationEnum;
            codelist.Links = new System.Collections.Generic.List<Link>();
            codelist.Links.Add(_linkCreator.GetCodelistLink(LinkCreator.LinkRelationEnum.metadata, codelist.Id, _language));

            return codelist;
        }
        private CodeListInformation Map(PCAxis.Paxiom.ValueSetInfo valueset)
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
