using System.Reflection;

namespace PxWeb.UnitTests.Helpers
{
    [TestClass]
    public class NotesFixTests
    {
        [TestMethod]
        public void GetNotes_WhenContent_ShouldReturnEmptyDictonary()
        {
            // Arrange
            var variable = ModelStore.CreateContentVariable("", PlacementType.Stub, 2);

            // Act
            var methodInfo = typeof(Bjarte3).GetMethod("GetNotes", BindingFlags.NonPublic | BindingFlags.Static);
            var result = methodInfo?.Invoke(null, new object[] { variable });

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Dictionary<string, Notes>));
            Assert.HasCount(0, ((Dictionary<string, Notes>)result));
        }

        [TestMethod]
        public void GetNotes_WhenOneNoteOnFirstValue_ShouldReturDictonaryWithOneKey()
        {
            // Arrange
            var variable = ModelStore.CreateTimeVariable("", PlacementType.Stub, 10);
            variable.Values[0].AddNote(new PCAxis.Paxiom.Note("A note!", NoteType.Value, false));
            var valueCode = variable.Values[0].Code;

            // Act
            var methodInfo = typeof(Bjarte3).GetMethod("GetNotes", BindingFlags.NonPublic | BindingFlags.Static);
            var result = methodInfo?.Invoke(null, new object[] { variable });

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Dictionary<string, Notes>));
            Assert.HasCount(1, ((Dictionary<string, Notes>)result));
            Assert.IsTrue(((Dictionary<string, Notes>)result).ContainsKey(valueCode));
        }


        [TestMethod]
        public void GetReapply_WhenOneNoteOnFirstValue_ShouldHaveANotOnFirstValue()
        {
            // Arrange
            const string noteText = "A note!";
            var variable = ModelStore.CreateTimeVariable("", PlacementType.Stub, 10);
            variable.Values[0].AddNote(new PCAxis.Paxiom.Note(noteText, NoteType.Value, false));
            var valueCode = variable.Values[0].Code;
            var variable2 = ModelStore.CreateTimeVariable("", PlacementType.Stub, 10);
            var methodInfo = typeof(Bjarte3).GetMethod("GetNotes", BindingFlags.NonPublic | BindingFlags.Static);
            var notes = methodInfo?.Invoke(null, new object[] { variable }) as Dictionary<string, Notes> ?? new Dictionary<string, Notes>();

            // Act
            var methodInfo2 = typeof(Bjarte3).GetMethod("ReapplyNotes", BindingFlags.NonPublic | BindingFlags.Static);
            var result = methodInfo2?.Invoke(null, new object[] { variable2, notes });

            // Assert
            Assert.IsTrue(variable2.Values[0].HasNotes());
            Assert.AreEqual(noteText, variable2.Values[0].Notes[0].Text);
            Assert.HasCount(1, variable2.Values[0].Notes);

        }
    }
}
