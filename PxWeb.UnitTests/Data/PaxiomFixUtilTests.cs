using Note = PCAxis.Paxiom.Note;

namespace PxWeb.UnitTests.Data
{
    [TestClass]
    public class PaxiomFixUtilTests
    {
        [TestMethod]
        public void ExtractNotes_WhenNoNotes_ReturnsEmptyDictionary()
        {
            // Arrange
            var variable = new Variable();

            // Act
            var result = PaxiomFixUtil.ExtractNotes(variable);
            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ExtractNotes_WhenNotesExist_ReturnsNotes()
        {
            // Arrange
            var variable = new Variable("VAR1", "VAR1", PlacementType.Heading, 1);
            var value = new Value("v1");
            PaxiomUtil.SetCode(value, "v1");
            variable.Values.Add(value);
            value = new Value("v2");
            PaxiomUtil.SetCode(value, "v2");
            variable.Values.Add(value);
            value = new Value("v3");
            PaxiomUtil.SetCode(value, "v3");
            variable.Values.Add(value);
            variable.Values[0].AddNote(new Note("My note", NoteType.Value, true));

            // Act
            var result = PaxiomFixUtil.ExtractNotes(variable);
            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.ContainsKey("v1"));
            Assert.AreEqual(1, result["v1"].Count);
        }

        [TestMethod]
        public void RestoreNotes_WhenNotesExist_ReturnsNotes()
        {
            // Arrange
            var variable = new Variable("VAR1", "VAR1", PlacementType.Heading, 1);
            var value = new Value("v1");
            PaxiomUtil.SetCode(value, "v1");
            variable.Values.Add(value);
            value = new Value("v2");
            PaxiomUtil.SetCode(value, "v2");
            variable.Values.Add(value);
            value = new Value("v3");
            PaxiomUtil.SetCode(value, "v3");
            variable.Values.Add(value);
            var notes = new Dictionary<string, Notes>();
            var list = new Notes();
            list.Add(new Note("My note", NoteType.Value, true));
            notes.Add("v1", list);

            // Act
            PaxiomFixUtil.RestoreNotes(variable, notes);

            // Assert
            Assert.AreEqual(1, variable.Values[0].Notes.Count);
        }

        [TestMethod]
        public void RestoreNotes_WhenNotesNotApplicable_ReturnsNotes()
        {
            // Arrange
            var variable = new Variable("VAR1", "VAR1", PlacementType.Heading, 1);
            var value = new Value("v1");
            PaxiomUtil.SetCode(value, "v1");
            variable.Values.Add(value);
            value = new Value("v2");
            PaxiomUtil.SetCode(value, "v2");
            variable.Values.Add(value);
            value = new Value("v3");
            PaxiomUtil.SetCode(value, "v3");
            variable.Values.Add(value);
            var notes = new Dictionary<string, Notes>();
            var list = new Notes();
            list.Add(new Note("My note", NoteType.Value, true));
            notes.Add("v4", list);

            // Act
            PaxiomFixUtil.RestoreNotes(variable, notes);

            // Assert
            Assert.IsFalse(variable.Values[0].HasNotes());
        }

        [TestMethod]
        public void CleanCellnotes_WhenNoCellNotes_NoException()
        {
            // Arrange
            var variable = new Variable("VAR1", "VAR1", PlacementType.Heading, 1);
            var value = new Value("v1");
            PaxiomUtil.SetCode(value, "v1");
            variable.Values.Add(value);
            value = new Value("v2");
            PaxiomUtil.SetCode(value, "v2");
            variable.Values.Add(value);
            value = new Value("v3");
            PaxiomUtil.SetCode(value, "v3");
            variable.Values.Add(value);

            var meta = new PXMeta();

            // Act
            var result = PaxiomFixUtil.CleanCellnotes(meta, variable);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CleanCellnotes_WhenCellNotes_MatchingWillBeRemoved()
        {
            // Arrange
            var variable = new Variable("VAR1", "VAR1", PlacementType.Heading, 1);
            var value = new Value("v1");
            PaxiomUtil.SetCode(value, "v1");
            variable.Values.Add(value);
            value = new Value("v2");
            PaxiomUtil.SetCode(value, "v2");
            variable.Values.Add(value);
            value = new Value("v3");
            PaxiomUtil.SetCode(value, "v3");
            variable.Values.Add(value);

            var meta = new PXMeta();

            var cellNote = new CellNote();
            cellNote.Conditions.Add(new VariableValuePair("VAR1", "v4"));
            cellNote.Conditions.Add(new VariableValuePair("VAR2", "v3"));
            meta.CellNotes.Add(cellNote);

            cellNote = new CellNote();
            cellNote.Conditions.Add(new VariableValuePair("VAR2", "v3"));
            meta.CellNotes.Add(cellNote);

            cellNote = new CellNote();
            cellNote.Conditions.Add(new VariableValuePair("VAR1", "v3"));
            meta.CellNotes.Add(cellNote);

            // Act
            var result = PaxiomFixUtil.CleanCellnotes(meta, variable);

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(2, meta.CellNotes.Count);
        }
    }
}
