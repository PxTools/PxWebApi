﻿using System.Linq;

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
                // Check if there is a condition for the variable with a value that is no longer in the list of values
                foreach (var condition in cellNote.Conditions.Where(c => string.Equals(c.VariableCode, pxVariable.Code, StringComparison.OrdinalIgnoreCase) &&
                                                                    pxVariable.Values.FirstOrDefault(x => x.Code.Equals(c.ValueCode, StringComparison.InvariantCultureIgnoreCase)) is null))
                {
                    meta.CellNotes.RemoveAt(i);
                    removed++;
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
                        RetoreNote(value, note);
                    }
                }
            }
        }

        private static void RetoreNote(Value value, Note note)
        {
            if (value.Notes is null)
            {
                value.AddNote(note);
            }
            else
            {
                if (value.Notes.Any(n => n.Text == note.Text && n.Type == note.Type && n.Mandantory == note.Mandantory))
                {
                    return; // Note already exists
                }
                value.AddNote(note);
            }
        }

        public static Dictionary<string, Notes> ExtractNotes(Variable variable)
        {

            // Extract notes
            var notes = new Dictionary<string, Notes>();
            foreach (var value in variable.Values.Where(v => v.HasNotes()))
            {
                notes.Add(value.Code, value.Notes);
            }

            return notes;
        }
    }
}
