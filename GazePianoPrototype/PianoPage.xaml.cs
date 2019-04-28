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
                // TODO: Map from button label to midi notes
                // TODO: Send midi notes to be played
            }
            else if (e.PointerState == PointerState.Exit)
            {
                StopNotes();
            }
        }

        private void OctaveDown_Click(object sender, RoutedEventArgs e)
        {
            this.Octave++;
        }

        private void MinorMode_Click(object sender, RoutedEventArgs e)
        {
            if (this.CurrentMode != PianoMode.MinorChord)
            {
                this.CurrentMode = PianoMode.MinorChord;
                (sender as Button).Style = (Style)Application.Current.Resources["SelectedModeButton"];

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
            }
            else
            {
                this.CurrentMode = PianoMode.SingleNote;
                (sender as Button).Style = (Style)Application.Current.Resources["ModeButton"];
            }
        }

        private void OctaveUp_Click(object sender, RoutedEventArgs e)
        {
            this.Octave--;
        }
    }
}