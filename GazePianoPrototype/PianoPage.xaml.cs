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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GazePianoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PianoPage : Page
    {
        enum PageState { Normal, ChordConfirm, RetNormal }

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

        public PianoPage()
        {
            this.InitializeComponent();
            GazeInput.SetIsCursorVisible(this, true);
            currentState = PageState.Normal;
        }

        private string queuedNote = string.Empty;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var UIE = sender as Button;
            if (UIE.Content as string == "PAUSE") 
            {
                // TODO: Navigate to pause menu
                return;
            }
            if (currentState == PageState.Normal)
            {
                queuedNote = UIE.Content as string;
                switch (UIE.Name)
                {
                    case "ML":
                        BlankButtons();
                        TL.Content = "Chord";
                        BL.Content = "Cancel";
                        break;
                    case "TL":
                        BlankButtons();
                        ML.Content = "Chord";
                        TM.Content = "Cancel";
                        break;
                    case "TM":
                        BlankButtons();
                        TL.Content = "Chord";
                        TR.Content = "Cancel";
                        break;
                    case "TR":
                        BlankButtons();
                        TM.Content = "Chord";
                        MR.Content = "Cancel";
                        break;
                    case "MR":
                        BlankButtons();
                        TR.Content = "Chord";
                        BR.Content = "Cancel";
                        break;
                    case "BR":
                        BlankButtons();
                        MR.Content = "Chord";
                        BM.Content = "Cancel";
                        break;
                    case "BM":
                        BlankButtons();
                        BL.Content = "Chord";
                        BR.Content = "Cancel";
                        break;
                }

                currentState = PageState.ChordConfirm;
            }
            else if (currentState == PageState.ChordConfirm)
            {
                if (UIE.Content as string == "Chord")
                {
                    PlayNote(queuedNote + ".chord");
                    SetStateRetNormal();
                }
                else if (UIE.Content as string == "Cancel")
                {
                    queuedNote = String.Empty;
                    SetStateRetNormal();
                }
            }
        }

        private void PlayNote(string note)
        {
            MediaSource pianoNote = MediaSource.CreateFromUri(new Uri($"ms-appx:///Notes/{note}.mp3"));
            PianoPlayer.SetPlaybackSource(pianoNote);
            PianoPlayer.Play();

        }

        private void SetStateRetNormal()
        {
            //GazeInput.SetDwellDuration(this, new TimeSpan(0, 0, 0, 0, 200));
            //GazeInput.SetDwellDuration(MidGrid, TimeSpan.Zero);
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

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (currentState == PageState.ChordConfirm)
            {
                PlayNote(queuedNote);
                queuedNote = String.Empty;
                SetStateRetNormal();
            }
            if (currentState == PageState.RetNormal)
            {
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
