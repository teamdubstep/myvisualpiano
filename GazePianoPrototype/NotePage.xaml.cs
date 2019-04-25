using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GazePianoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotePage : Page
    {
        public NotePage()
        {
            this.InitializeComponent();
            actionSelected = false;
        }

        private bool actionSelected;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button UIE = sender as Button;
            string buttonText = UIE.Content as string;

            switch (buttonText.ToLower())
            {
                case "major":
                    App.PlayNote = App.QueuedNote + ".major";
                    break;
                case "minor":
                    App.PlayNote = App.QueuedNote + ".minor";
                    break;
                case "eigth":
                    App.PlayNote = App.QueuedNote + ".eigth";
                    break;
                case "quarter":
                    App.PlayNote = App.QueuedNote + ".quarter";
                    break;
                case "note":
                    App.PlayNote = App.QueuedNote;
                    break;
                case "o+":
                    App.Octave++;
                    break;
                case "o-":
                    App.Octave--;
                    break;
                default:
                    break;
            }

            BlankButtons();
            actionSelected = true;
            App.QueuedNote = null;
        }

        private void Center_Tapped(object sender, RoutedEventArgs e)
        {
            if (actionSelected)
            {
                Frame.GoBack(new SuppressNavigationTransitionInfo());
            }
            
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
    }
}
