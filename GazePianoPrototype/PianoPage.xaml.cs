using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
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
        enum PageState { Normal, RetNormal }
        private PageState _currentState;

        private PageState currentState
        {
            get { return _currentState; }
            set
            {
                if (value != _currentState)
                {
                    _currentState = value;
                }
            }
        }

        /// <summary>
        /// Creates a new PianoPage (home page for playing music)
        /// </summary>
        public PianoPage()
        {
            InitializeComponent();
            GazeInput.SetIsCursorVisible(this, true);
            currentState = PageState.Normal;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Checks if a note has been queued for playback & plays it
            if (!string.IsNullOrEmpty(App.PlayNote))
            {
                PlayNote(App.PlayNote);
                App.PlayNote = null;
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Handles button clicks for cardinal buttons
        /// </summary>
        /// <param name="sender">One of the cardinal buttons</param>
        /// <param name="e">Event arguments</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var UIE = sender as Button;
            if (UIE.Content as string == "PAUSE")
            {
                Frame.GoBack();
                return;
            }
            else if (string.IsNullOrWhiteSpace(UIE.Content as string))
            {
                return;
            }
            else if (currentState == PageState.Normal)
            {
                App.QueuedNote = UIE.Content as string;
                SetStateRetNormal();
            }
        }

        /// <summary>
        /// Code that handles playing notes
        /// </summary>
        /// <param name="note">String description of note (e.g. A or A.major)</param>
        private void PlayNote(string note)
        {
            MediaSource pianoNote = MediaSource.CreateFromUri(new Uri($"ms-appx:///Notes/{note}.mp3"));
            PianoPlayer.SetPlaybackSource(pianoNote);
            PianoPlayer.Play();
        }

        /// <summary>
        /// Blanks buttons after note selection
        /// </summary>
        private void SetStateRetNormal()
        {
            BlankButtons();
            currentState = PageState.RetNormal;
        }

        private void BlankButtons()
        {
            ML.Content = "";
            TL.Content = "";
            TM.Content = "";
            TR.Content = "";
            MR.Content = "";
            BR.Content = "";
            BM.Content = "";
            BL.Content = "";
        }

        /// <summary>
        /// Handles recentering
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Center_Tapped(object sender, RoutedEventArgs e)
        {
            // Check if a note was selected, if so handle fixing the page & navigation
            if (currentState == PageState.RetNormal)
            {
                Frame.Navigate(typeof(NotePage), null, new SuppressNavigationTransitionInfo());

                ML.Content = "A";
                TL.Content = "B";
                TM.Content = "C";
                TR.Content = "D";
                MR.Content = "E";
                BR.Content = "F";
                BM.Content = "G";
                BL.Content = "PAUSE";

                currentState = PageState.Normal;
            }
        }
    }
}