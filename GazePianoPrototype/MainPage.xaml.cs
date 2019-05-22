namespace GazePianoPrototype
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;
    using Windows.Devices.Input.Preview;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void HomeButtonClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(KeySelector));
        }
    }
}