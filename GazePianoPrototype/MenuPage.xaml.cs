namespace GazePianoPrototype
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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

    class MenuResults
    {
        public static Boolean displayMenu = true;
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MenuPage : Page
    {
        public MenuPage()
        {
            this.InitializeComponent();
        }

        private void HomeButtonClick(object sender, RoutedEventArgs e)
        {
            MenuResults.displayMenu = true;
            this.Frame.Navigate(typeof(MainPage));
        }

        private void HelpButtonClick(object sender, RoutedEventArgs e)
        {
            MenuResults.displayMenu = true;
        }

        private void RecordButtonClick(object sender, RoutedEventArgs e)
        {
            MenuResults.displayMenu = true;
        }

        private void KeySelectButtonClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(KeySelector));
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            MenuResults.displayMenu = true;
            this.Frame.GoBack();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            App.Current.Exit();
        }
    }
}
