namespace GazePianoPrototype
{
    using System;
    using System.Collections.Generic;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Preset Keys for the visual piano
        /// </summary>
        public static List<PresetKey> PresetKeys { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            PresetKeys = new List<PresetKey>
            {
                new PresetKey("C major", new string[] { "C", "D", "E", "F", "G", "A", "B", "C+", string.Empty }),
                new PresetKey("A minor", new string[] { "A-", "B-", "C", "D", "E", "F", "G", "A", string.Empty }),
                new PresetKey("G major", new string[] { "G", "A", "B", "C+", "D+", "E+", "F#+", "G+", string.Empty }),
                new PresetKey("E minor", new string[] { "E", "F#", "G", "A", "B", "C+", "D+", "E+", string.Empty }),
                new PresetKey("D major", new string[] { "D", "E", "F#", "G", "A", "B", "C#+", "D+", string.Empty }),
                new PresetKey("B minor", new string[] { "B-", "C#", "D", "E", "F#", "G", "A", "B", string.Empty }),
                new PresetKey("A major", new string[] { "A-", "B-", "C#", "D", "E", "F#", "G#", "A", string.Empty }),
                new PresetKey("F# minor", new string[] { "F#", "G#", "A", "B", "C#+", "D+", "E+", "F#+", string.Empty }),
                new PresetKey("E major", new string[] { "E", "F#", "G#", "A", "B", "C#+", "D#+", "E+", string.Empty }),
                new PresetKey("C# minor", new string[] { "C#", "D#", "E", "F#", "G#", "A", "B", "C#+", string.Empty }),
                new PresetKey("B major", new string[] { "B-", "C#", "D#", "E", "F#", "G#", "A#", "B", string.Empty }),
                new PresetKey("G# minor", new string[] { "G#", "A#", "B", "C#+", "D#+", "E+", "F#+", "G#+", string.Empty }),
                new PresetKey("F# major", new string[] { "F#", "G#", "A#", "B", "C#+", "D#+", "E#+", "F#+", string.Empty }),
                new PresetKey("D# minor", new string[] { "D#", "E#", "F#", "G#", "A#", "B", "C#+", "D#+", string.Empty }),
                new PresetKey("C# major", new string[] { "C#", "D#", "E#", "F#", "G#", "A#", "B#", "C#+", string.Empty }),
                new PresetKey("A# minor", new string[] { "A#-", "B#", "C#", "D#", "E#", "F#", "G#", "A#", string.Empty }),

                new PresetKey("F major", new string[] { "F", "G", "A", "Bb", "C+", "D+", "E+", "F+", string.Empty }),
                new PresetKey("D minor", new string[] { "D", "E", "F", "G", "A", "Bb", "C+", "D+", string.Empty }),
                new PresetKey("Bb major", new string[] { "Bb-", "C", "D", "Eb", "F", "G", "A", "Bb", string.Empty }),
                new PresetKey("G minor", new string[] { "G", "A", "Bb", "C+", "D+", "Eb+", "F+", "G+", string.Empty }),
                new PresetKey("Eb major", new string[] { "Eb", "F", "G", "Ab", "Bb", "C", "D", "Eb", string.Empty }),
                new PresetKey("C minor", new string[] { "C", "D", "Eb", "F", "G", "Ab", "Bb", "C+", string.Empty }),
                new PresetKey("Ab major", new string[] { "Ab", "Bb", "C+", "Db+", "Eb+", "F+", "G+", "Ab+", string.Empty }),
                new PresetKey("F minor", new string[] { "F", "G", "Ab", "Bb", "C+", "Db+", "Eb+", "F+", string.Empty }),
                new PresetKey("Db major", new string[] { "Db", "Eb", "F", "Gb", "Ab", "Bb", "C+", "Db+", string.Empty }),
                new PresetKey("Bb minor", new string[] { "Bb-", "C", "Db", "Eb", "F", "Gb", "Ab", "Bb", string.Empty }),
                new PresetKey("Gb major", new string[] { "Gb", "Ab", "Bb", "Cb", "Db+", "Eb+", "F+", "Gb+", string.Empty }),
                new PresetKey("Eb minor", new string[] { "Eb", "F", "Gb", "Ab", "Bb", "Cb", "Db+", "Eb+", string.Empty }),
                new PresetKey("Cb major", new string[] { "Cb-", "Db", "Eb", "Fb", "Gb", "Ab", "Bb", "Cb", string.Empty }),
                new PresetKey("Ab minor", new string[] { "Ab", "Bb", "Cb", "Db+", "Eb+", "Fb+", "Gb+", "Ab+", string.Empty }),
            };
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}