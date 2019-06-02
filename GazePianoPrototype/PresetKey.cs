namespace GazePianoPrototype
{
    using System;
    using System.Linq;

    public class PresetKey
    {
        /// <summary>
        /// Name of the key
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Notes to play
        /// </summary>
        public string[] Notes { get; private set; }

        /// <summary>
        /// Notes to display on virtual keys
        /// </summary>
        public string[] DisplayNotes { get; private set; }

        public PresetKey(string name, string[] notes)
        {
            if (notes.Length != 8)
            {
                throw new ArgumentException("Preset keys must have an array length of 8 (nulls are acceptable)");
            }
            this.Name = name;
            this.Notes = notes;
            // remove +/- octave indicators from display notes and replace #/b with unicode escape sequences for sharps/flats
            this.DisplayNotes = this.Notes.Select(note => note.Replace("+", string.Empty).Replace("-", string.Empty)
                                                              .Replace("#", "\u266f").Replace("b", "\u266D")).ToArray();
        }

        public PresetKey(string name, string[] notes, string[] displayNotes)
        {
            this.Name = name;
            this.Notes = notes;
            this.DisplayNotes = displayNotes;
        }
    }
}
