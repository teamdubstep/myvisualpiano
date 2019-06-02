namespace GazePianoPrototype
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
    using Windows.Devices.Enumeration;
    using Windows.Devices.Midi;
    using Windows.UI;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
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
        private static Dictionary<string, string> enharmonics;

        private static int _octave;

        private int Octave
        {
            get
            {
                return _octave;
            }
            set
            {
                if (value < 1 || value > 7)
                {
                    return;
                }
                else
                {
                    switch (value)
                    {
                        case 1:
                            this.Background = new SolidColorBrush(Color.FromArgb(255, 13, 46, 122));
                            break;
                        case 2:
                            this.Background = new SolidColorBrush(Color.FromArgb(255, 43, 88, 191));
                            break;
                        case 3:
                            this.Background = new SolidColorBrush(Color.FromArgb(255, 48, 99, 212));
                            break;
                        case 4:
                            this.Background = new SolidColorBrush(Color.FromArgb(255, 56, 115, 220));
                            break;
                        case 5:
                            this.Background = new SolidColorBrush(Color.FromArgb(255, 75, 130, 245));
                            break;
                        case 6:
                            this.Background = new SolidColorBrush(Color.FromArgb(255, 112, 150, 239));
                            break;
                        case 7:
                            this.Background = new SolidColorBrush(Color.FromArgb(255, 135, 160, 225));
                            break;
                    }
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
            this.CurrentMode = PianoMode.SingleNote;
            LoadNotesAndChords();
            this.Octave = 3;
            this.CurrentOctave.Text = "Octave " + this.Octave;

        }

        /// <summary>
        /// Handles midi creation on navigated to
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadPreset(0);
            if (e.Parameter is int)
            {
                LoadPreset((int)e.Parameter);
            }

            // Find Microsoft synth and instantiate midi
            DeviceInformationCollection deviceInformationCollection = await DeviceInformation.FindAllAsync(MidiOutPort.GetDeviceSelector());
            DeviceInformation synthObject = deviceInformationCollection.FirstOrDefault(x => x.Name == "Microsoft GS Wavetable Synth");
            this.synth = await MidiOutPort.FromIdAsync(synthObject.Id);

            if (this.synth == null)
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
                this.synth.SendMessage(new MidiNoteOnMessage(0, note, VELOCITY));
            }
            this.playingNotes = notes;
        }

        /// <summary>
        /// Stops all currently playing notes
        /// </summary>
        private void StopNotes()
        {
            if (this.playingNotes != null)
            {
                foreach (byte note in this.playingNotes)
                {
                    this.synth.SendMessage(new MidiNoteOffMessage(0, note, VELOCITY));
                }

                this.playingNotes = null;
            }
        }

        private void LoadPreset(int index)
        {
            if (index < 0 || index >= App.PresetKeys.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be within App.PresetKeys range");
            }
            PresetKey key = App.PresetKeys[index];
            this.L1.Content = key.DisplayNotes[0];
            this.L2.Content = key.DisplayNotes[1];
            this.L3.Content = key.DisplayNotes[2];
            this.B1.Content = key.DisplayNotes[3];
            this.B2.Content = key.DisplayNotes[4];
            this.B3.Content = key.DisplayNotes[5];
            this.R1.Content = key.DisplayNotes[6];
            this.R2.Content = key.DisplayNotes[7];
            //R3.Content = key.DisplayNotes[8];

            this.L1.Tag = key.Notes[0];
            this.L2.Tag = key.Notes[1];
            this.L3.Tag = key.Notes[2];
            this.B1.Tag = key.Notes[3];
            this.B2.Tag = key.Notes[4];
            this.B3.Tag = key.Notes[5];
            this.R1.Tag = key.Notes[6];
            this.R2.Tag = key.Notes[7];
            //R3.Tag = key.Notes[8];

            this.CurrentKey.Text = key.Name.Replace("#", "\u266f").Replace("b", "\u266D");

            // Get keys ready for touch control
            this.L1.AddHandler(PointerPressedEvent, new PointerEventHandler(this.ButtonPressed), true);
            this.L1.AddHandler(PointerReleasedEvent, new PointerEventHandler(this.ButtonReleased), true);
            this.L2.AddHandler(PointerPressedEvent, new PointerEventHandler(this.ButtonPressed), true);
            this.L2.AddHandler(PointerReleasedEvent, new PointerEventHandler(this.ButtonReleased), true);
            this.L3.AddHandler(PointerPressedEvent, new PointerEventHandler(this.ButtonPressed), true);
            this.L3.AddHandler(PointerReleasedEvent, new PointerEventHandler(this.ButtonReleased), true);

            this.B1.AddHandler(PointerPressedEvent, new PointerEventHandler(this.ButtonPressed), true);
            this.B1.AddHandler(PointerReleasedEvent, new PointerEventHandler(this.ButtonReleased), true);
            this.B2.AddHandler(PointerPressedEvent, new PointerEventHandler(this.ButtonPressed), true);
            this.B2.AddHandler(PointerReleasedEvent, new PointerEventHandler(this.ButtonReleased), true);
            this.B3.AddHandler(PointerPressedEvent, new PointerEventHandler(this.ButtonPressed), true);
            this.B3.AddHandler(PointerReleasedEvent, new PointerEventHandler(this.ButtonReleased), true);

            this.R1.AddHandler(PointerPressedEvent, new PointerEventHandler(this.ButtonPressed), true);
            this.R1.AddHandler(PointerReleasedEvent, new PointerEventHandler(this.ButtonReleased), true);
            this.R2.AddHandler(PointerPressedEvent, new PointerEventHandler(this.ButtonPressed), true);
            this.R2.AddHandler(PointerReleasedEvent, new PointerEventHandler(this.ButtonReleased), true);            
        }

        /// <summary>
        /// Handles button events for the notes when used with eye gaze
        /// </summary>
        /// <param name="sender">XAML object that sent the event</param>
        /// <param name="e"></param>
        private void NoteButton_StateChanged(object sender, StateChangedEventArgs e)
        {
            /* Make sure we have a synth before playing anything */
            if (this.synth is null)
            {
                return;
            }

            if (e.PointerState == PointerState.Fixation)
            {
                ButtonNoteTranslator(sender, true);
            }
            else if (e.PointerState == PointerState.Exit)
            {
                ButtonNoteTranslator(sender, false);
            }
        }

        /// <summary>
        /// Handles button pressed 
        /// </summary>
        /// <param name="sender">XAML Button that triggered the event</param>
        /// <param name="e"></param>
        private void ButtonPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is Button)
            {
                ButtonNoteTranslator(sender, true);
            }
        }

        /// <summary>
        /// Handles button released 
        /// </summary>
        /// <param name="sender">XAML Button that triggered the event</param>
        /// <param name="e"></param>
        private void ButtonReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is Button)
            {
                ButtonNoteTranslator(sender, false);
            }
        }

        /// <summary>
        /// Abstracted button handling so either touch or gaze input can be used
        /// </summary>
        /// <param name="sender">Button that was pressed/gazed/whatever</param>
        /// <param name="start">True if starting note, false if stopping note</param>
        private void ButtonNoteTranslator(object sender, bool start)
        {
            Button button = sender as Button;
            if (start)
            {
                byte[] notes;
                string content = button.Tag as string;
                int octave = this.Octave;

                /* Check for empty virtual keys */
                if (string.IsNullOrWhiteSpace(content))
                {
                    return;
                }

                /* Octave Parsing */
                if (content.Contains("+"))
                {
                    octave++;
                    content = content.Replace("+", string.Empty);
                }
                else if (content.Contains("-"))
                {
                    octave--;
                    content = content.Replace("-", string.Empty);
                }

                /* Play Note */
                if (this.CurrentMode == PianoMode.SingleNote)
                {
                    notes = new byte[] { GetPianoNote(content, octave) };
                }
                else
                {
                    notes = GetPianoChord(content, octave);
                }

                PlayNote(notes);
            }
            else
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
            this.CurrentOctave.Text = "Octave " + this.Octave;
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
                this.MajorModeButton.Style = (Style)Application.Current.Resources["ModeButton"];

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
            this.Frame.Navigate(typeof(MainPage));
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
                this.MinorModeButton.Style = (Style)Application.Current.Resources["ModeButton"];
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
            this.CurrentOctave.Text = "Octave " + this.Octave;
        }

        /// <summary>
        /// Handles button click to navigate to the menu page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MenuPage));
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

            enharmonics = new Dictionary<string, string>
            {
                { "Cb", "B" },
                { "Fb", "E" },
                { "E#", "F" },
                { "B#", "C" }
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
            /* 
            *  Get equivalent enharmonic of passed in note 
            *  if note is an enharmonic
            */
            if (enharmonics.ContainsKey(note))
            {
                note = enharmonics[note];
            }
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
        /// <param name="note">Note that represents the chord name</param>
        /// <param name="octave">Octave for bottom chord note</param>
        /// <returns></returns>
        private byte[] GetPianoChord(string note, int octave)
        {
            byte[] byteChord = new byte[3];
            byte baseNote = GetPianoNote(note, octave);
            byteChord[0] = baseNote;
            // major -> baseNote + 4 + 3
            if (this.CurrentMode == PianoMode.MajorChord)
            {
                byteChord[1] = (byte) (baseNote + 4);
            }
            // minor -> baseNote + 3 + 4
            else
            {
                byteChord[1] = (byte) (baseNote + 3);
            }

            byteChord[2] = (byte) (baseNote + 7);
            return byteChord;
        }        
    }
}