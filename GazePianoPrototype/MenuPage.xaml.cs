namespace GazePianoPrototype
{
    using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
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

    /// <summary>
    /// Menu/options page for My Visual Piano
    /// </summary>
    public sealed partial class MenuPage : Page
    {
        public MenuPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Navigates to home page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeButtonClick(object sender, RoutedEventArgs e)
        {

            this.Frame.Navigate(typeof(MainPage));
        }

        /// <summary>
        /// Navigates to help page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpButtonClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts recording
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordButtonClick(object sender, RoutedEventArgs e)
        {
            if (App.RecordingControlsVisibility == Visibility.Collapsed)
            {
                App.RecordingControlsVisibility = Visibility.Visible;
            }
            else
            {
                App.RecordingControlsVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Navigates to key selection page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeySelectButtonClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(KeySelector));
        }

        /// <summary>
        /// Closes this menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        /// <summary>
        /// Closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            App.Current.Exit();
        }

        /// <summary>
        /// Toggles Gaze Pointer on/off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleGazeDotClick(object sender, RoutedEventArgs e)
        {
            GazeInput.SetIsCursorVisible(this, !GazeInput.GetIsCursorVisible(this));
        }

        private void ClearRec1_Click(object sender, RoutedEventArgs e)
        {
            App.Recording1 = new Recording();
        }

        private void ClearRec2_Click(object sender, RoutedEventArgs e)
        {
            App.Recording2 = new Recording();
        }

        private void ClearRec3_Click(object sender, RoutedEventArgs e)
        {
            App.Recording3 = new Recording();
        }
    }
}
