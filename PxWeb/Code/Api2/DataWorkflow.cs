using System.Linq;

using PCAxis.Paxiom;
using PCAxis.Paxiom.Operations;

using Px.Abstractions.Interfaces;

using PxWeb.Api2.Server.Models;
using PxWeb.Code.Api2.DataSelection;
using PxWeb.Helper.Api2;

namespace PxWeb.Code.Api2
{
    public class DataWorkflow : IDataWorkflow
    {
        private readonly IDataSource _dataSource;
        private readonly ISelectionHandler _selectionHandler;
        private readonly IDefaultSelectionAlgorithm _defaultSelectionAlgorithm;
        private readonly IPlacementHandler _placementHandler;

        public DataWorkflow(IDataSource datasource, ISelectionHandler selectionHandler, IDefaultSelectionAlgorithm defaultSelectionAlgorithm, IPlacementHandler placementHandler)
        {
            _dataSource = datasource;
            _selectionHandler = selectionHandler;
            _defaultSelectionAlgorithm = defaultSelectionAlgorithm;
            _placementHandler = placementHandler;
        }
        public PXModel? Run(string tableId, string language, VariablesSelection? variablesSelection, out Problem? problem)
        {
            //If no selection is made, return a problem and exit early
            if (variablesSelection is null)
            {
                problem = ProblemUtility.MissingSelection();
                return null;

            }

            // Create a builder for the table and read in the table metadata
            var builder = _dataSource.CreateBuilder(tableId, language);
            if (builder == null)
            {
                problem = ProblemUtility.NonExistentTable();
                return null;
            }
            builder.BuildForSelection();
            return Run(variablesSelection, builder, out problem);
        }

        public PXModel? Run(string tableId, string language, out Problem? problem)
        {
            var builder = _dataSource.CreateBuilder(tableId, language);
            if (builder == null)
            {
                problem = ProblemUtility.NonExistentTable();
                return null;
            }

            builder.BuildForSelection();

            var variablesSelection = _defaultSelectionAlgorithm.GetDefaultSelection(builder);
            //If no selection is made, return a problem and exit early
            if (variablesSelection is null)
            {
                problem = ProblemUtility.MissingSelection();
                return null;

            }

            // Create a builder for the table and read in the table metadata
            return Run(variablesSelection, builder, out problem);
        }

        private PXModel? Run(VariablesSelection variablesSelection, IPXModelBuilder? builder, out Problem? problem)
        {
            //If no selection is made, return a problem and exit early
            if (variablesSelection is null)
            {
                problem = ProblemUtility.MissingSelection();
                return null;

            }

            if (builder == null)
            {
                problem = ProblemUtility.NonExistentTable();
                return null;
            }


            // Expand and verify the selections
            if (!_selectionHandler.ExpandAndVerfiySelections(variablesSelection, builder, out problem))
            {
                return null;
            }


            Selection[]? selection = null;
            VariablePlacementType? placment = null;

            selection = _selectionHandler.Convert(variablesSelection);

            if (problem is not null)
            {
                return null;
            }

            //Fix selection loses all values after BuildForPresentation
            var selectionCopy = SelectionUtil.Copy(selection);

            builder.BuildForPresentation(selection);

            selection = selectionCopy;

            var model = builder.Model;

            placment = _placementHandler.GetPlacment(variablesSelection, selection, builder.Model.Meta, out problem);

            if (placment is not null)
            {
                var descriptions = new List<PivotDescription>();

                descriptions.AddRange(placment.Heading.Select(h => new PivotDescription()
                {
                    VariableName = model.Meta.Variables.First(v => v.Code.Equals(h, StringComparison.OrdinalIgnoreCase)).Name,
                    VariablePlacement = PlacementType.Heading
                }));

                descriptions.AddRange(placment.Stub.Select(h => new PivotDescription()
                {
                    VariableName = model.Meta.Variables.First(v => v.Code == h).Name,
                    VariablePlacement = PlacementType.Stub
                }));

                var pivot = new PCAxis.Paxiom.Operations.Pivot();
                model = pivot.Execute(model, descriptions.ToArray());
            }

            return model;
        }
    }
}
