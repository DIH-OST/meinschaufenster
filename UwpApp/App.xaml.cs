// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BaseApp;

namespace UwpApp
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        ///     Initializes the singleton application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                // beim Store Build werden die Assemblies aus Nuget Packages nicht richtig mitgeladen, hiermit behoben
                var rendererAssemblies = new[]
                                         {
                                             // Die nicht im Projekt benötigten Renderer können entfernt werden
                                             typeof(CartesianChartRenderer).GetTypeInfo().Assembly,
                                             typeof(LegendRenderer).GetTypeInfo().Assembly,
                                             typeof(PieChartRenderer).GetTypeInfo().Assembly,
                                             typeof(PieLabelRenderer).GetTypeInfo().Assembly,
                                             typeof(CardActionViewRenderer).GetTypeInfo().Assembly,
                                             typeof(ChatListViewRenderer).GetTypeInfo().Assembly,
                                             typeof(ListViewRenderer).GetTypeInfo().Assembly,
                                             typeof(TreeViewRenderer).GetTypeInfo().Assembly,
                                             typeof(ItemsControlRenderer).GetTypeInfo().Assembly,
                                             typeof(AutoCompleteLabelRenderer).GetTypeInfo().Assembly,
                                             typeof(AutoCompleteRenderer).GetTypeInfo().Assembly,
                                             typeof(ButtonRenderer).GetTypeInfo().Assembly,
                                             typeof(CalendarRenderer).GetTypeInfo().Assembly,
                                             typeof(DataFormRenderer).GetTypeInfo().Assembly,
                                             typeof(EntryRenderer).GetTypeInfo().Assembly,
                                             typeof(MaskedInputRenderer).GetTypeInfo().Assembly,
                                             typeof(SegmentedControlRenderer).GetTypeInfo().Assembly,
                                             typeof(TimePickerItemViewRenderer).GetTypeInfo().Assembly,
                                             typeof(BorderRenderer).GetTypeInfo().Assembly,
                                             typeof(CheckBoxRenderer).GetTypeInfo().Assembly,
                                             typeof(ScrollViewRenderer).GetTypeInfo().Assembly,
                                             typeof(SideDrawerRenderer).GetTypeInfo().Assembly,
                                             typeof(SlideViewLabelRenderer).GetTypeInfo().Assembly,
                                             typeof(SlideViewRenderer).GetTypeInfo().Assembly,
                                             typeof(TabViewHeaderItemRenderer).GetTypeInfo().Assembly,
                                             typeof(ZXingBarcodeImageViewRenderer).GetTypeInfo().Assembly,
                                             typeof(ZXingScannerViewRenderer).GetTypeInfo().Assembly,
                                             typeof(WriteableBitmapRenderer).GetTypeInfo().Assembly,
                                             typeof(PixelDataRenderer).GetTypeInfo().Assembly,
                                             typeof(SvgRenderer).GetTypeInfo().Assembly,
                                             typeof(SKCanvasViewRenderer).GetTypeInfo().Assembly,
                                             typeof(SKGLViewRenderer).GetTypeInfo().Assembly
                                         };

                Forms.Init(e, rendererAssemblies);
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null) rootFrame.Navigate(typeof(MainPage), e.Arguments);
                // Ensure the current window is active
                Window.Current.Activate();
            }

            // Azure Mobile Center Push Notifizierungen
            Push.CheckLaunchedFromNotification(e);

            if (e.Arguments != null)
            {
                var customData = ParseLaunchString(e.Arguments);

                if (customData != null)
                {
                    foreach (var data in customData)
                    {
                        Logging.Log.LogInfo("NotificationData: " + data.Key + ": " + data.Value);
                    }

                    ProjectViewModelBase.PushReceived(customData);
                }
            }
        }

        internal static Dictionary<string, string> ParseLaunchString(string launchString)
        {
            try
            {
                if (!string.IsNullOrEmpty(launchString))
                {
                    var launchJObject = JObject.Parse(launchString);

                    if (launchJObject?["appCenter"] is JObject appCenterData)
                    {
                        return ParseCustomData(appCenterData);
                    }

                    if (launchJObject?["mobile_center"] is JObject mobileCenterData)
                    {
                        return ParseCustomData(mobileCenterData);
                    }

                    var toastArgs = JsonConvert.DeserializeObject<Dictionary<string, string>>(launchString);

                    if (toastArgs != null)
                    {
                        return toastArgs;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                try
                {
                    if (!string.IsNullOrEmpty(launchString))
                    {
                        var toastArgs = JsonConvert.DeserializeObject<Dictionary<string, string>>(launchString);

                        if (toastArgs != null)
                        {
                            return toastArgs;
                        }
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private static Dictionary<string, string> ParseCustomData(JObject appCenterData)
        {
            var customData = new Dictionary<string, string>();

            foreach (var pair in appCenterData)
            {
                customData.Add(pair.Key, pair.Value.ToString());
            }

            return customData;
        }

        /// <summary>
        ///     Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //Save application state and stop any background activity if you want ... ;-) MKo
            deferral.Complete();
        }
    }
}