namespace GazePianoPrototype
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
    using Windows.Devices.Enumeration;
    using Windows.Devices.Midi;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// The home page for the visual piano instrument
    /// </summary>
    public sealed partial class PianoPage : Page
    {
        /// <summary>
        /// Modes that the virtual piano keys can operate in
        /// </summary>
        private enum PianoMode { SingleNote, MinorChord, MajorChord }

        /// <summary>
        /// Gets the mode that the piano is currently in
        /// </summary>
        private PianoMode CurrentMode { get; set; }

        private static Dictionary<string, int> noteToInt;
        private static Dictionary<string, string[]> majorChords;
        private static Dictionary<string, string[]> minorChords;
        private static Dictionary<string, int> notesToMidi;

        private static int _octave;

        private int Octave
        {
            get
            {
                return _octave;
            }
            set
            {
                if (value < 0 || value > 7)
                {
                    return;
                }
                else
                {
                    _octave = value;
                }
            }
        }

        private IMidiOutPort synth;

        /// <summary>
        /// Creates a new PianoPage (home page for playing music)
        /// </summary>
        public PianoPage()
        {
            InitializeComponent();
            GazeInput.SetIsCursorVisible(this, true);
            CurrentMode = PianoMode.SingleNote;
            LoadNotesAndChords();
            Octave = 3;
            CurrentOctave.Text = "Octave " + Octave;
            LoadKeyNoteToMidi(App.PresetKeys[0]);
        }

        /// <summary>
        /// Handles midi creation on navigated to
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadPreset(0);

            // Find Microsoft synth and instantiate midi
            DeviceInformationCollection deviceInformationCollection = await DeviceInformation.FindAllAsync(MidiOutPort.GetDeviceSelector());
            DeviceInformation synthObject = deviceInformationCollection.FirstOrDefault(x => x.Name == "Microsoft GS Wavetable Synth");
            synth = await MidiOutPort.FromIdAsync(synthObject.Id);

            if (synth == null)
            {
                _ = await new MessageDialog("Could not find Microsoft GS Wavetable Synth.  Failed to open MIDI port.", "Error Starting App").ShowAsync();
            }

            base.OnNavigatedTo(e);
        }

        private static readonly byte VELOCITY = 100;
        private byte[] playingNotes;

        /// <summary>
        /// Plays the specified midi notes w/ preset velocity
        /// </summary>
        /// <param name="notes">Array of note values</param>
        private void PlayNote(byte[] notes)
        {
            foreach (byte note in notes)
            {
                synth.SendMessage(new MidiNoteOnMessage(0, note, VELOCITY));
            }
            playingNotes = notes;
        }

        /// <summary>
        /// Stops all currently playing notes
        /// </summary>
        private void StopNotes()
        {
            if (playingNotes != null)
            {
                foreach (byte note in playingNotes)
                {
                    synth.SendMessage(new MidiNoteOffMessage(0, note, VELOCITY));
                }

                playingNotes = null;
            }
        }

        private void LoadPreset(int index)
        {
            if (index < 0 || index >= App.PresetKeys.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be within App.PresetKeys range");
            }
            PresetKey key = App.PresetKeys[index];
            L1.Content = key.Notes[0];
            L2.Content = key.Notes[1];
            L3.Content = key.Notes[2];
            B1.Content = key.Notes[3];
            B2.Content = key.Notes[4];
            B3.Content = key.Notes[5];
            R1.Content = key.Notes[6];
            R2.Content = key.Notes[7];
            R3.Content = key.Notes[8];
        }

        /// <summary>
        /// Handles button events for the notes
        /// </summary>
        /// <param name="sender">XAML object that sent the event</param>
        /// <param name="e"></param>
        private void NoteButton_StateChanged(object sender, StateChangedEventArgs e)
        {
            Button button = sender as Button;
            if (e.PointerState == PointerState.Fixation)
            {
                byte[] notes;
                if (CurrentMode == PianoMode.SingleNote)
                {
                    //notes = new byte[] { GetPianoNote(button.Content as string, Octave) };
                    notes = new byte[] { (byte) notesToMidi[button.Content as string] };
                }
                else
                {
                    notes = GetPianoChord(button.Content as string);
                }

                PlayNote(notes);
            }
            else if (e.PointerState == PointerState.Exit)
            {
                StopNotes();
            }
        }

        /// <summary>
        /// Handles octave down button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OctaveDown_Click(object sender, RoutedEventArgs e)
        {
            this.Octave--;
            CurrentOctave.Text = "Octave " + Octave;
            // Todo: preset keys as a field
            LoadKeyNoteToMidi(App.PresetKeys[0]);
        }

        /// <summary>
        /// Handles button clicks for the minor mode button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinorMode_Click(object sender, RoutedEventArgs e)
        {
            if (this.CurrentMode != PianoMode.MinorChord)
            {
                this.CurrentMode = PianoMode.MinorChord;
                (sender as Button).Style = (Style)Application.Current.Resources["SelectedModeButton"];
                MajorModeButton.Style = (Style)Application.Current.Resources["ModeButton"];

            }
            else
            {
                this.CurrentMode = PianoMode.SingleNote;
                (sender as Button).Style = (Style)Application.Current.Resources["ModeButton"];
            }

        }

        /// <summary>
        /// Handles button clicks for home button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        /// <summary>
        /// Handles button clicks for major mode button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MajorClick_Click(object sender, RoutedEventArgs e)
        {
            if (this.CurrentMode != PianoMode.MajorChord)
            {
                this.CurrentMode = PianoMode.MajorChord;
                (sender as Button).Style = (Style)Application.Current.Resources["SelectedModeButton"];
                MinorModeButton.Style = (Style)Application.Current.Resources["ModeButton"];
            }
            else
            {
                this.CurrentMode = PianoMode.SingleNote;
                (sender as Button).Style = (Style)Application.Current.Resources["ModeButton"];
            }
        }

        /// <summary>
        /// Handles button clicks for octave up button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OctaveUp_Click(object sender, RoutedEventArgs e)
        {
            this.Octave++;
            CurrentOctave.Text = "Octave " + Octave;
            // Todo: preset keys as a field
            LoadKeyNoteToMidi(App.PresetKeys[0]);
        }

        /// <summary>
        /// Loads the MIDI mappings for notes and chords
        /// </summary>
        private void LoadNotesAndChords()
        {
            noteToInt = new Dictionary<string, int>
            {
                { "C", 0 },
                { "C#", 1 },
                { "Db", 1 },
                { "D", 2 },
                { "D#", 3 },
                { "Eb", 3 },
                { "E", 4 },
                { "F", 5 },
                { "F#", 6 },
                { "Gb", 6 },
                { "G", 7 },
                { "G#", 8 },
                { "Ab", 8 },
                { "A", 9 },
                { "A#", 10 },
                { "Bb", 10 },
                { "B", 11 }
            };

            majorChords = new Dictionary<string, string[]>
            {
                { "C", new string[] { "C", "E", "G" } },
                { "C#", new string[] { "C#", "E#", "G#" } },
                { "Db", new string[] { "Db", "F", "Ab" } },
                { "D", new string[] { "D", "F#", "A" } },
                { "Eb", new string[] { "Eb", "G", "Bb" } },
                { "E", new string[] { "E", "G#", "B" } },
                { "Fb", new string[] { "Fb", "Ab", "Cb" } },
                { "F", new string[] { "F", "A", "C" } },
                { "F#", new string[] { "F#", "A#", "C#" } },
                { "Gb", new string[] { "Gb", "Bb", "Db" } },
                { "G", new string[] { "G", "B", "D" } },
                { "G#", new string[] { "G#", "B#", "D#" } },
                { "Ab", new string[] { "Ab", "C", "Eb" } },
                { "A", new string[] { "A", "C#", "E" } },
                { "Bb", new string[] { "Bb", "D", "F" } },
                { "B", new string[] { "B", "D#", "F#" } },
                { "Cb", new string[] { "Cb", "Eb", "Gb" } }
            };

            minorChords = new Dictionary<string, string[]>
            {
                { "C", new string[] { "C", "Eb", "G" } },
                { "C#", new string[] { "C#", "E", "G#" } },
                { "Db", new string[] { "Db", "Fb", "Ab" } },
                { "D", new string[] { "D", "F", "A" } },
                { "D#", new string[] { "D#", "F#", "A#" } },
                { "Eb", new string[] { "Eb", "Gb", "Bb" } },
                { "E", new string[] { "E", "G", "B" } },
                { "E#", new string[] { "E#", "G#", "B#" } },
                { "F", new string[] { "F", "Ab", "C" } },
                { "F#", new string[] { "F#", "A", "C#" } },
                { "G", new string[] { "G", "Bb", "D" } },
                { "G#", new string[] { "G#", "B", "D#" } },
                { "Ab", new string[] { "Ab", "B", "Eb" } },
                { "A", new string[] { "A", "C", "E" } },
                { "A#", new string[] { "A#", "C#", "E#" } },
                { "Bb", new string[] { "Bb", "Db", "F" } },
                { "B", new string[] { "B", "D", "F#" } }
            };
        }

        /// <summary>
        /// Get the MIDI byte mapping for a chord
        /// </summary>
        /// <param name="note">Desired note (e.g. C or F#)</param>
        /// <param name="octave">Desired Octave</param>
        /// <returns>byte code for the MIDI note</returns>
        private byte GetPianoNote(string note, int octave)
        {
            int intNote = -1;
            if (noteToInt.ContainsKey(note))
            {
                intNote = noteToInt[note];
                for (int i = 0; i < octave + 2; i++)
                {
                    intNote += 12;
                }
            }
            return (byte)intNote;
        }

        /// <summary>
        /// Get a byte array of notes for a chord
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        private byte[] GetPianoChord(string note)
        {
            byte[] byteChord = new byte[3];
            if (noteToInt.ContainsKey(note))
            {
                string[] stringChord = new string[3];
                // Get the chord progression for major / minor modes
                if (CurrentMode == PianoMode.MajorChord)
                {
                    stringChord = majorChords[note];
                } else
                {
                    stringChord = minorChords[note];
                }

                // Get all byte piano notes for the chord
                for (int i = 0; i < 3; i++)
                {
                    byteChord[i] = (byte) notesToMidi[stringChord[i]];
                }
            }
            return byteChord;
        }

        /// <summary>
        /// Loads up all notes to their midi numbers in the current preset key
        /// </summary>
        /// <param name="key">Current PresetKey</param>
        private void LoadKeyNoteToMidi(PresetKey key)
        {
            notesToMidi = new Dictionary<string, int>();
            int tempOctave = Octave;
            if (key.Name.Contains("A") || key.Name.Contains("A#") || 
                key.Name.Contains("Bb") || key.Name.Contains("B"))
            {
                tempOctave--; // Octave starts one below if key begins with these conditions
            }
            int baseMidiNote = GetPianoNote(key.Notes[0], tempOctave);
            // Assign different midi numbers depending on major / minor
            if (key.Name.Contains("major"))
            {
                notesToMidi.Add(key.Notes[0], baseMidiNote);
                notesToMidi.Add(key.Notes[1], baseMidiNote + 2);
                notesToMidi.Add(key.Notes[2], baseMidiNote + 4);
                notesToMidi.Add(key.Notes[3], baseMidiNote + 5);
                notesToMidi.Add(key.Notes[4], baseMidiNote + 7);
                notesToMidi.Add(key.Notes[5], baseMidiNote + 9);
                notesToMidi.Add(key.Notes[6], baseMidiNote + 11);
                notesToMidi.Add(key.Notes[7], baseMidiNote + 12);
            } else
            {
                notesToMidi.Add(key.Notes[0], baseMidiNote);
                notesToMidi.Add(key.Notes[1], baseMidiNote + 2);
                notesToMidi.Add(key.Notes[2], baseMidiNote + 3);
                notesToMidi.Add(key.Notes[3], baseMidiNote + 5);
                notesToMidi.Add(key.Notes[4], baseMidiNote + 7);
                notesToMidi.Add(key.Notes[5], baseMidiNote + 8);
                notesToMidi.Add(key.Notes[6], baseMidiNote + 10);
                notesToMidi.Add(key.Notes[7], baseMidiNote + 12);
            }
        }
    }
}