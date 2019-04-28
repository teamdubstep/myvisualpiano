using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazePianoPrototype
{
    public class PresetKey
    {
        public string Name { get; private set; }

        public string[] Notes { get; private set; }

        public PresetKey(string name, string[] notes)
        {
            if (notes.Length != 9)
            {
                throw new ArgumentException("Preset keys must have an array length of 9 (nulls are acceptable)");
            }
            Name = name;
            Notes = notes;
        }
    }
}
