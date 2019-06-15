namespace GazePianoPrototype
{
    using System;
    using Windows.Devices.Midi;

    public class RecordingItem
    {
        public IMidiMessage MidiMessage { get; private set; }

        public TimeSpan Timecode { get; private set; }

        public bool Played { get; set; }

        public RecordingItem(TimeSpan time, IMidiMessage midiMessage, bool played = false)
        {
            this.MidiMessage = midiMessage;
            this.Timecode = time;
            this.Played = played;
        }
    }
}
