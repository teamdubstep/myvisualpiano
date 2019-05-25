namespace GazePianoPrototype
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RecordingLayer
    {
        public List<RecordingItem> RecordingItems { get; private set; }
        public RecordingLayer()
        {
            this.RecordingItems = new List<RecordingItem>();
        }

        /// <summary>
        /// Sets all items to not played
        /// </summary>
        public void ResetPlayedStatus()
        {
            this.RecordingItems.ForEach(x => x.Played = false);
        }
    }
}
