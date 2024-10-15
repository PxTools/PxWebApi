

namespace PxWeb.UnitTests.Data
{
    [TestClass]
    public class DataSelectionTest
    {

        [TestMethod]
        public void ShouldReturnWildcardStarSelection()
        {
            List<string> valueCodes = new List<string>();

            valueCodes.Add("000*"); // 9 values
            valueCodes.Add("*100"); // 1 value

            var selections = GetSelection(valueCodes);

            if (selections != null)
            {
                var selection = selections.FirstOrDefault(s => s.VariableCode == "var1");
                if (selection != null)
                {
                    Assert.AreEqual(10, selection.ValueCodes.Count);
                }
            }
            else { Assert.Fail(); }
        }

        [TestMethod]
        public void ShouldReturnWildcardQuestionmarkSelection()
        {
            List<string> valueCodes = new List<string>();

            valueCodes.Add("00??"); // 98 values
            valueCodes.Add("0?00"); // 10 values

            var selections = GetSelection(valueCodes);

            if (selections != null)
            {
                var selection = selections.FirstOrDefault(s => s.VariableCode == "var1");
                if (selection != null)
                {
                    Assert.AreEqual(108, selection.ValueCodes.Count);
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ShouldReturnTopSelection()
        {
            List<string> valueCodes = new List<string>();

            valueCodes.Add("TOP(10)"); // 10 values

            var selections = GetSelection(valueCodes);

            if (selections != null)
            {
                var selection = selections.FirstOrDefault(s => s.VariableCode == "var1");

                if (selection != null)
                {
                    Assert.AreEqual(10, selection.ValueCodes.Count);
                    Assert.AreEqual("0001", selection.ValueCodes[0]);
                    Assert.AreEqual("0010", selection.ValueCodes[9]);
                }
            }
            else { Assert.Fail(); }
        }

        [TestMethod]
        public void ShouldReturnTopOffsetSelection()
        {
            List<string> valueCodes = new List<string>();

            valueCodes.Add("TOP(10,995)"); // 5 values

            var selections = GetSelection(valueCodes);

            if (selections != null)
            {
                var selection = selections.FirstOrDefault(s => s.VariableCode == "var1");
                if (selection != null)
                {
                    Assert.AreEqual(5, selection.ValueCodes.Count);
                    Assert.AreEqual("0996", selection.ValueCodes[0]);
                    Assert.AreEqual("1000", selection.ValueCodes[4]);
                }
            }
            else
            { Assert.Fail(); }
        }


        [TestMethod]
        public void ShouldReturnBottomSelection()
        {
            List<string> valueCodes = new List<string>();

            valueCodes.Add("bottom(10)"); // 10 values

            var selections = GetSelection(valueCodes);

            if (selections != null)
            {
                var selection = selections.FirstOrDefault(s => s.VariableCode == "var1");
                if (selection != null)
                {
                    Assert.AreEqual(10, selection.ValueCodes.Count);
                    Assert.AreEqual("0991", selection.ValueCodes[0]);
                    Assert.AreEqual("1000", selection.ValueCodes[9]);
                }
            }
            else { Assert.Fail(); }
        }

        [TestMethod]
        public void ShouldReturnBottomOffsetSelection()
        {
            List<string> valueCodes = new List<string>();

            valueCodes.Add("bottom(10,995)"); // 5 values

            var selections = GetSelection(valueCodes);

            if (selections != null)
            {
                var selection = selections.FirstOrDefault(s => s.VariableCode == "var1");
                if (selection != null)
                {
                    Assert.AreEqual(5, selection.ValueCodes.Count);
                    Assert.AreEqual("0001", selection.ValueCodes[0]);
                    Assert.AreEqual("0005", selection.ValueCodes[4]);
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ShouldReturnRangeSelection()
        {
            List<string> valueCodes = new List<string>();

            valueCodes.Add("RANGE(0120,0139)"); // 20 values

            var selections = GetSelection(valueCodes);

            if (selections != null)
            {
                var selection = selections.FirstOrDefault(s => s.VariableCode == "var1");
                if (selection != null)
                {
                    Assert.AreEqual(20, selection.ValueCodes.Count);
                    Assert.AreEqual("0120", selection.ValueCodes[0]);
                    Assert.AreEqual("0139", selection.ValueCodes[19]);
                }
            }
            else { Assert.Fail(); }
        }

        [TestMethod]
        public void ShouldReturnFromSelection()
        {
            List<string> valueCodes = new List<string>();

            valueCodes.Add("from(0981)"); // 20 values

            var selections = GetSelection(valueCodes);

            if (selections != null)
            {
                var selection = selections.FirstOrDefault(s => s.VariableCode == "var1");
                if (selection != null)
                {
                    Assert.AreEqual(20, selection.ValueCodes.Count);
                    Assert.AreEqual("0981", selection.ValueCodes[0]);
                    Assert.AreEqual("1000", selection.ValueCodes[19]);
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ShouldReturnToSelection()
        {
            List<string> valueCodes = new List<string>();

            valueCodes.Add("TO(0025)"); // 25 values

            var selections = GetSelection(valueCodes);

            if (selections != null)
            {
                var selection = selections.FirstOrDefault(s => s.VariableCode == "var1");
                if (selection != null)
                {
                    Assert.AreEqual(25, selection.ValueCodes.Count);
                    Assert.AreEqual("0001", selection.ValueCodes[0]);
                    Assert.AreEqual("0025", selection.ValueCodes[24]);
                }
            }
            else { Assert.Fail(); }
        }


        // Helper methods

        private PCAxis.Paxiom.Value CreateValue(string code)
        {
            PCAxis.Paxiom.Value value = new PCAxis.Paxiom.Value(code);
            PaxiomUtil.SetCode(value, code);
            return value;
        }

        private VariablesSelection CreateVariablesSelection(VariablesSelection variablesSelection)
        {
            //Add variable
            var variableSelectionObject = new VariableSelection
            {
                VariableCode = "var3",
                ValueCodes = new List<string>()
            };

            variablesSelection.Selection.Add(variableSelectionObject);

            return variablesSelection;

        }

        private VariableSelection CreateVariableSelection(string variableCode, List<string> valueCodes)
        {
            var variableSelectionObject = new VariableSelection
            {
                VariableCode = variableCode,
                ValueCodes = new List<string>()
            };

            variableSelectionObject.ValueCodes.AddRange(valueCodes);

            return variableSelectionObject;
        }


        private Selection[]? GetSelection(List<string> wantedValues)
        {
            PXModel model = GetPxModelForTest();

            SelectionHandler selectionHandler = new SelectionHandler(GetConfigMock().Object);
            VariablesSelection variablesSelection = new VariablesSelection();
            variablesSelection.Selection = new List<VariableSelection>();
            List<string> valueCodes = new List<string>();

            valueCodes.AddRange(wantedValues);

            var varSelection = CreateVariableSelection("var1", valueCodes);
            variablesSelection.Selection.Add(varSelection);

            Problem? problem;
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            Selection[]? selections = selectionHandler.GetSelection(builderMock.Object, variablesSelection, out problem);
            return selections;
        }

        private Mock<IPxApiConfigurationService> GetConfigMock()
        {
            var configMock = new Mock<IPxApiConfigurationService>();
            var testFactory = new TestFactory();
            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            return configMock;
        }

        private PXModel GetPxModelForTest()
        {
            PXModel pxModel = new PXModel();

            Variable var1 = new Variable("var1", PlacementType.Heading);
            var1.Elimination = false;

            for (int i = 1; i <= 1000; i++)
            {
                var1.Values.Add(CreateValue(i.ToString("0000")));
            }

            pxModel.Meta.AddVariable(var1);

            return pxModel;

        }
    }
}
