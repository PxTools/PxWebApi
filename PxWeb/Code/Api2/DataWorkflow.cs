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

        public DataWorkflow(IDataSource datasource, ISelectionHandler selectionHandler)
        {
            _dataSource = datasource;
            _selectionHandler = selectionHandler;
        }
        public PXModel? Run(string tableId, string language, VariablesSelection variablesSelection, out Problem? problem)
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

            builder.BuildForPresentation(selection);

            var model = builder.Model;

            // TODO: Fix this
            //placment = savedQuery.Selection.Placement;

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
