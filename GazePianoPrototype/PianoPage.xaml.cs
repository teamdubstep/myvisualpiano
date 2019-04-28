using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace GazePianoPrototype
{
    /// <summary>
    /// The home page for the visual piano instrument
    /// </summary>
    public sealed partial class PianoPage : Page
    {
        enum PianoMode { SingleNote, MinorChord, MajorChord }

        private PianoMode CurrentMode
        {
            get;
            set;
        }

        private static Dictionary<String, int> noteToInt;
        private static Dictionary<String, String[]> majorChords;
        private static Dictionary<String, String[]> minorChords;

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
        }

        /// <summary>
        /// Handles midi creation on navigated to
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadPreset(0);

            // Find Microsoft synth and instantiate midi
            var deviceInformationCollection = await DeviceInformation.FindAllAsync(MidiOutPort.GetDeviceSelector());
            var synthObject = deviceInformationCollection.FirstOrDefault(x => x.Name == "Microsoft GS Wavetable Synth");
            synth = await MidiOutPort.FromIdAsync(synthObject.Id);

            if (synth == null)
            {
                await new MessageDialog("Could not find Microsoft GS Wavetable Synth.  Failed to open MIDI port.", "Error Starting App").ShowAsync();
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
                    notes = new byte[] { GetPianoNote(button.Content as string, Octave) };
                }
                else
                {
                    notes = GetPianoChord(button.Content as string, Octave);
                }

                PlayNote(notes);
            }
            else if (e.PointerState == PointerState.Exit)
            {
                StopNotes();
            }
        }

        private void OctaveDown_Click(object sender, RoutedEventArgs e)
        {
            this.Octave--;
        }

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

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

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

        private void OctaveUp_Click(object sender, RoutedEventArgs e)
        {
            this.Octave++;
        }

        /// <summary>
        /// Loads the MIDI mappings for notes and chords
        /// </summary>
        private void LoadNotesAndChords()
        {
            noteToInt = new Dictionary<string, int>();
            noteToInt.Add("C", 0);
            noteToInt.Add("C#", 1);
            noteToInt.Add("Db", 1);
            noteToInt.Add("D", 2);
            noteToInt.Add("D#", 3);
            noteToInt.Add("Eb", 3);
            noteToInt.Add("E", 4);
            noteToInt.Add("F", 5);
            noteToInt.Add("F#", 6);
            noteToInt.Add("Gb", 6);
            noteToInt.Add("G", 7);
            noteToInt.Add("G#", 8);
            noteToInt.Add("Ab", 8);
            noteToInt.Add("A", 9);
            noteToInt.Add("A#", 10);
            noteToInt.Add("Bb", 10);
            noteToInt.Add("B", 11);

            majorChords = new Dictionary<String, String[]>();
            majorChords.Add("C", new String[] { "C", "E", "G" });
            majorChords.Add("C#", new String[] { "C#", "E#", "G#" });
            majorChords.Add("Db", new String[] { "Db", "F", "Ab" });
            majorChords.Add("D", new String[] { "D", "F#", "A" });
            majorChords.Add("Eb", new String[] { "Eb", "G", "Bb" });
            majorChords.Add("E", new String[] { "E", "G#", "B" });
            majorChords.Add("Fb", new String[] { "Fb", "Ab", "Cb" });
            majorChords.Add("F", new String[] { "F", "A", "C" });
            majorChords.Add("F#", new String[] { "F#", "A#", "C#" });
            majorChords.Add("Gb", new String[] { "Gb", "Bb", "Db" });
            majorChords.Add("G", new String[] { "G", "B", "D" });
            majorChords.Add("G#", new String[] { "G#", "B#", "D#" });
            majorChords.Add("Ab", new String[] { "Ab", "C", "Eb" });
            majorChords.Add("A", new String[] { "A", "C#", "E" });
            majorChords.Add("Bb", new String[] { "Bb", "D", "F" });
            majorChords.Add("B", new String[] { "B", "D#", "F#" });
            majorChords.Add("Cb", new String[] { "Cb", "Eb", "Gb" });

            minorChords = new Dictionary<String, String[]>();
            minorChords.Add("C", new String[] { "C", "Eb", "G" });
            minorChords.Add("C#", new String[] { "C#", "E", "G#" });
            minorChords.Add("Db", new String[] { "Db", "Fb", "Ab" });
            minorChords.Add("D", new String[] { "D", "F", "A" });
            minorChords.Add("D#", new String[] { "D#", "F#", "A#" });
            minorChords.Add("Eb", new String[] { "Eb", "Gb", "Bb" });
            minorChords.Add("E", new String[] { "E", "G", "B" });
            minorChords.Add("E#", new String[] { "E#", "G#", "B#" });
            minorChords.Add("F", new String[] { "F", "Ab", "C" });
            minorChords.Add("F#", new String[] { "F#", "A", "C#" });
            minorChords.Add("G", new String[] { "G", "Bb", "D" });
            minorChords.Add("G#", new String[] { "G#", "B", "D#" });
            minorChords.Add("Ab", new String[] { "Ab", "B", "Eb" });
            minorChords.Add("A", new String[] { "A", "C", "E" });
            minorChords.Add("A#", new String[] { "A#", "C#", "E#" });
            minorChords.Add("Bb", new String[] { "Bb", "Db", "F" });
            minorChords.Add("B", new String[] { "B", "D", "F#" });
        }

        /// <summary>
        /// Get the MIDI byte mapping for a chord
        /// </summary>
        /// <param name="note">Desired note (e.g. C or F#)</param>
        /// <param name="octave">Desired Octave</param>
        /// <returns></returns>
        private byte GetPianoNote(String note, int octave)
        {
            int intNote = -1;
            if (noteToInt.ContainsKey(note))
            {
                intNote = noteToInt[note];
                for (var i = 0; i < octave + 2; i++)
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
        /// <param name="octave"></param>
        /// <returns></returns>
        private byte[] GetPianoChord(String note, int octave)
        {
            byte[] intChord = new byte[3];
            int intNote = -1;
            if (noteToInt.ContainsKey(note))
            {
                String[] stringChord = new String[3];
                // Get the chord progression for major / minor modes
                if (CurrentMode == PianoMode.MajorChord)
                {
                    stringChord = majorChords[note];
                }
                else
                {
                    stringChord = minorChords[note];
                }

                // Get all int piano notes for the chord
                for (var i = 0; i < 3; i++)
                {
                    intChord[i] = GetPianoNote(stringChord[i], octave);
                }
            }
            return intChord;
        }
    }
}