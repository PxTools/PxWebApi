using System.Linq;

using PCAxis.Paxiom;

namespace PxWeb.Code.Api2.DataSelection
{
    public static class PaxiomFixUtil
    {
        public static int CleanCellnotes(PXMeta meta, Variable pxVariable)
        {
            int removed = 0;
            for (int i = meta.CellNotes.Count - 1; i >= 0; i--)
            {
                var cellNote = meta.CellNotes[i];
                foreach (var condition in cellNote.Conditions)
                {
                    // Check if there is a condition for the variable with a value that is no longer in the list of values
                    if (string.Equals(condition.VariableCode, pxVariable.Code, StringComparison.OrdinalIgnoreCase))
                    {
                        if (pxVariable.Values.FirstOrDefault(x => x.Code.Equals(condition.ValueCode, StringComparison.InvariantCultureIgnoreCase)) is null)
                        {
                            meta.CellNotes.RemoveAt(i);
                            removed++;
                        }
                    }
                }
            }
            return removed;
        }

        public static void RestoreNotes(Variable variable, Dictionary<string, Notes> notes)
        {
            foreach (var valueCode in notes.Keys)
            {
                if (variable.Values.FirstOrDefault(x => x.Code.Equals(valueCode, System.StringComparison.InvariantCultureIgnoreCase)) is Value value)
                {
                    foreach (var note in notes[valueCode])
                    {
                        value.AddNote(note);
                    }
                }
            }
        }

        public static Dictionary<string, Notes> ExtractNotes(Variable variable)
        {

            // Extract notes
            var notes = new Dictionary<string, Notes>();
            foreach (var value in variable.Values)
            {
                if (value.HasNotes())
                {
                    notes.Add(value.Code, value.Notes);
                }
            }

            return notes;
        }
    }
}
