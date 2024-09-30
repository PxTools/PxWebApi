namespace PxWeb.UnitTests
{
    internal static class ModelStore
    {
        public static PXModel GetModelWithOnlyOneVariable(int numberOfValues)
        {

            var variable = CreateClassificationVariable("1", PlacementType.Stub, numberOfValues);

            PXModel model = new PXModel();
            PXMeta meta = new PXMeta();
            meta.AddVariable(variable);

            model.Meta = meta;
            return model;
        }

        public static PXModel GetModelWith2NoneMandantoryVariables(int numberOfValues1, int numberOfValues2)
        {

            PXModel model = new PXModel();
            PXMeta meta = new PXMeta();

            var variable = CreateClassificationVariable("1", PlacementType.Stub, numberOfValues1);
            meta.AddVariable(variable);

            variable = CreateClassificationVariable("2", PlacementType.Stub, numberOfValues2);
            meta.AddVariable(variable);

            model.Meta = meta;
            return model;
        }

        public static PXModel GetModelWith1MandantoryAnd1NoneMandantoryVariables(int mandantoryVariableNumberOfValues, int noneMandantoryVariableNuberOfValues)
        {

            PXModel model = new PXModel();
            PXMeta meta = new PXMeta();

            var variable = CreateClassificationVariable("1", PlacementType.Stub, mandantoryVariableNumberOfValues, false);
            meta.AddVariable(variable);

            variable = CreateClassificationVariable("2", PlacementType.Stub, noneMandantoryVariableNuberOfValues);
            meta.AddVariable(variable);

            model.Meta = meta;
            return model;
        }

        public static PXModel GetModelWith1MandantoryAnd3NoneMandantoryVariables(int mandantoryVariableNumberOfValues, int noneMandantoryVariableNuberOfValues)
        {

            PXModel model = new PXModel();
            PXMeta meta = new PXMeta();

            var variable = CreateClassificationVariable("1", PlacementType.Stub, mandantoryVariableNumberOfValues, false);
            meta.AddVariable(variable);

            variable = CreateClassificationVariable("2", PlacementType.Stub, noneMandantoryVariableNuberOfValues);
            meta.AddVariable(variable);

            variable = CreateClassificationVariable("3", PlacementType.Stub, noneMandantoryVariableNuberOfValues);
            meta.AddVariable(variable);

            variable = CreateClassificationVariable("4", PlacementType.Stub, noneMandantoryVariableNuberOfValues);
            meta.AddVariable(variable);

            model.Meta = meta;
            return model;
        }

        public static PXModel GetModelWith2MandantoryAnd2NoneMandantoryVariables(int mandantoryVariableNumberOfValues, int noneMandantoryVariableNuberOfValues)
        {

            PXModel model = new PXModel();
            PXMeta meta = new PXMeta();

            var variable = CreateClassificationVariable("1", PlacementType.Stub, mandantoryVariableNumberOfValues, false);
            meta.AddVariable(variable);

            variable = CreateClassificationVariable("2", PlacementType.Stub, mandantoryVariableNumberOfValues, false);
            meta.AddVariable(variable);

            variable = CreateClassificationVariable("3", PlacementType.Stub, noneMandantoryVariableNuberOfValues);
            meta.AddVariable(variable);

            variable = CreateClassificationVariable("4", PlacementType.Stub, noneMandantoryVariableNuberOfValues);
            meta.AddVariable(variable);

            model.Meta = meta;
            return model;
        }

        public static PXModel GetModelWith3MandantoryAnd1NoneMandantoryVariables(int mandantoryVariableNumberOfValues, int noneMandantoryVariableNuberOfValues)
        {

            PXModel model = new PXModel();
            PXMeta meta = new PXMeta();

            var variable = CreateClassificationVariable("1", PlacementType.Stub, mandantoryVariableNumberOfValues, false);
            meta.AddVariable(variable);

            variable = CreateClassificationVariable("2", PlacementType.Stub, mandantoryVariableNumberOfValues, false);
            meta.AddVariable(variable);

            variable = CreateClassificationVariable("3", PlacementType.Stub, mandantoryVariableNumberOfValues, false);
            meta.AddVariable(variable);

            variable = CreateClassificationVariable("4", PlacementType.Stub, noneMandantoryVariableNuberOfValues);
            meta.AddVariable(variable);

            model.Meta = meta;
            return model;
        }

        public static PXModel GetModelWithContentsAndTime(int numberOfContentsValues, int numberOfTimeValues, int startTime = 1968)
        {

            PXModel model = new PXModel();
            PXMeta meta = new PXMeta();

            var variable = CreateContentVariable("1", PlacementType.Stub, numberOfContentsValues);
            meta.AddVariable(variable);

            variable = CreateTimeVariable("2", PlacementType.Stub, numberOfTimeValues, startTime);
            meta.AddVariable(variable);

            model.Meta = meta;
            return model;
        }

        public static PXModel GetModelWithContentsTimeAnd1ClassificationVariable(int numberOfContentsValues, int numberOfTimeValues, int numberOfClassificationVariableValues, int startTime = 1968)
        {

            PXModel model = new PXModel();
            PXMeta meta = new PXMeta();

            var variable = CreateContentVariable("1", PlacementType.Stub, numberOfContentsValues);
            meta.AddVariable(variable);

            variable = CreateTimeVariable("2", PlacementType.Stub, numberOfTimeValues, startTime);
            meta.AddVariable(variable);

            variable = CreateClassificationVariable("2", PlacementType.Stub, numberOfClassificationVariableValues);
            meta.AddVariable(variable);

            model.Meta = meta;
            return model;
        }

        public static PXModel GetModelWithContentsTimeAndXClassificationVariable(int numberOfContentsValues, int numberOfTimeValues, int numberOfClassificationVariableValues, int numberOfNoneMandantoryClassificationVariables, int numberOfMandantoryClassificationVariables, int startTime = 1968)
        {

            PXModel model = new PXModel();
            PXMeta meta = new PXMeta();

            var variable = CreateContentVariable("1", PlacementType.Stub, numberOfContentsValues);
            meta.AddVariable(variable);

            variable = CreateTimeVariable("2", PlacementType.Stub, numberOfTimeValues, startTime);
            meta.AddVariable(variable);

            for (int i = 0; i < numberOfNoneMandantoryClassificationVariables; i++)
            {
                variable = CreateClassificationVariable($"{i + 1}", PlacementType.Stub, numberOfClassificationVariableValues);
                meta.AddVariable(variable);
            }

            for (int i = 0; i < numberOfMandantoryClassificationVariables; i++)
            {
                variable = CreateClassificationVariable($"{i + numberOfNoneMandantoryClassificationVariables + 1}", PlacementType.Stub, numberOfClassificationVariableValues, false);
                meta.AddVariable(variable);
            }
            model.Meta = meta;
            return model;
        }


        private static PCAxis.Paxiom.Variable CreateClassificationVariable(string suffix, PlacementType placementType, int numberOfValues, bool elimination = true)
        {
            var name = $"clsv_{suffix}";
            Variable variable = new Variable(name, placementType);

            for (int i = 0; i < numberOfValues; i++)
            {
                variable.Values.Add(CreateValue($"Code_{i}_{name}"));
            }

            variable.Elimination = elimination;

            return variable;
        }

        private static PCAxis.Paxiom.Variable CreateContentVariable(string suffix, PlacementType placementType, int numberOfValues)
        {
            var name = $"cont_{suffix}";
            Variable variable = new Variable(name, placementType);

            for (int i = 0; i < numberOfValues; i++)
            {
                variable.Values.Add(CreateValue($"Code_{i}_{name}"));
            }

            variable.Elimination = false;
            variable.IsContentVariable = true;

            return variable;
        }

        private static PCAxis.Paxiom.Variable CreateTimeVariable(string suffix, PlacementType placementType, int numberOfValues, int startYear = 1968)
        {
            var name = $"time_{suffix}";
            Variable variable = new Variable(name, placementType);

            for (int i = startYear; i < startYear + numberOfValues; i++)
            {
                variable.Values.Add(CreateValue($"{i}"));
            }

            variable.TimeValue = $"TLIST(A, \"{startYear}\"-\"{startYear + numberOfValues - 1}\")";
            variable.IsTime = true;

            return variable;
        }


        private static PCAxis.Paxiom.Value CreateValue(string code)
        {
            PCAxis.Paxiom.Value value = new PCAxis.Paxiom.Value(code);
            PaxiomUtil.SetCode(value, code);
            return value;
        }
    }
}
