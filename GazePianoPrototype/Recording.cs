namespace GazePianoPrototype
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Timers;
    using Windows.Devices.Midi;

    /// <summary>
    /// Class to hold the details of a recording and assist with playback
    /// </summary>
    public class Recording
    {
        /// <summary>
        /// Defines the states a Recording object can be in
        /// </summary>
        public enum RecordingStatus { Blank, Recording, Recorded, Playing };

        /// <summary>
        /// The current RecordingStatus of this recording instance
        /// </summary>
        public RecordingStatus Status { get; private set; }

        public delegate void PlayNoteEventHandler(IMidiMessage args);

        public delegate void PlaybackCompleteEventHandler();

        /// <summary>
        /// Event to subscribe to for MIDI playback
        /// </summary>
        public event PlayNoteEventHandler PlayNote;

        public event PlaybackCompleteEventHandler PlaybackComplete;

        private readonly List<RecordingItem> recordingItems;
        private Timer playbackTimer;
        private DateTime recordingStart;
        private DateTime playbackStart;

        /// <summary>
        /// Creates a new Recording
        /// </summary>
        public Recording()
        {
            this.recordingItems = new List<RecordingItem>();
            this.Status = RecordingStatus.Blank;
        }

        /// <summary>
        /// Allows object to start recording notes <see cref="AddNote(IMidiMessage)"/>
        /// </summary>
        public void StartRecording()
        {
            this.recordingStart = DateTime.Now;
            this.Status = RecordingStatus.Recording;
        }

        /// <summary>
        /// Add note to the recording *may only be called after <see cref="StartRecording()"/> and before <see cref="StopRecording()"/>*
        /// </summary>
        /// <param name="message">IMidiMessage to record</param>
        public void AddNote(IMidiMessage message)
        {
            if (this.Status == RecordingStatus.Recording)
            {
                this.recordingItems.Add(new RecordingItem(DateTime.Now - this.recordingStart, message));
            }
            else
            {
                throw new Exception("Cannot add note when RecordingStatus is not recording");
            }
        }

        /// <summary>
        /// Stops recording
        /// </summary>
        public void StopRecording()
        {
            this.Status = RecordingStatus.Recorded;
        }

        /// <summary>
        /// Start playback <seealso cref="PlayNote"/>
        /// </summary>
        public void Play()
        {
            if (this.Status == RecordingStatus.Recorded)
            {
                this.Status = RecordingStatus.Playing;
                this.playbackTimer = new Timer(100);
                this.playbackTimer.Elapsed += PlaybackTimer_Elapsed;
                this.playbackStart = DateTime.Now;
                this.playbackTimer.Start();
            }
        }

        private void PlaybackTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeSpan currentPosition = DateTime.Now - this.playbackStart;
            IEnumerable<RecordingItem> toPlay = this.recordingItems.Where(x => !x.Played && x.Timecode < currentPosition);
            foreach (RecordingItem item in toPlay)
            {
                this.PlayNote?.Invoke(item.MidiMessage);
                item.Played = true;
            }

            // If all items have been played, reset
            if (!this.recordingItems.Any(x => !x.Played))
            {
                this.playbackTimer.Stop();
                this.playbackTimer.Dispose();
                this.recordingItems.ForEach(x => x.Played = false);
                this.Status = RecordingStatus.Recorded;
                this.PlaybackComplete?.Invoke();
            }
        }
    }
}
