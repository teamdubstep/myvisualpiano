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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GazePianoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MajorMinorSelector : Page
    {
        private string note;

        public MajorMinorSelector()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.note = e.Parameter as string;
            if (!App.PresetKeys.Any(x => x.Name.Equals($"{this.note} minor")))
            {
                this.MinorButton.IsEnabled = false;
            }
            if (!App.PresetKeys.Any(x=>x.Name.Equals($"{this.note} major")))
            {
                this.MajorButton.IsEnabled = false;
            }

            base.OnNavigatedTo(e);
        }

        private void Minor_Clicked(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PianoPage), App.PresetKeys.IndexOf(App.PresetKeys.First(x => x.Name.Equals($"{this.note} minor"))));
        }

        private void Major_Clicked(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PianoPage), App.PresetKeys.IndexOf(App.PresetKeys.First(x => x.Name.Equals($"{this.note} major"))));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
