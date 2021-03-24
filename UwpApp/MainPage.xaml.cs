// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Exchange;

namespace UwpApp
{
    /// <summary>
    ///     UWP Main Page - Xamarin Forms und BISS MVVM Initialisieren und Xamarin Forms starten
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            var parameters = new object[] {AppSettings.Current().ProjectWorkUserFolder};

            var initializer = new BissInitializer();
            if (!initializer.Initialize(parameters))
                throw new ApplicationException("Initialization failed");

            ZXingScannerViewRenderer.Init();

            FormsMaps.Init(AppSettings.Current().BingMapsApplicationId);
            NativeCustomize();
            LoadApplication(new BaseApp.App(initializer));
        }

        /// <summary>
        ///     Styles richtig setzen für UWP
        /// </summary>
        private void NativeCustomize()
        {
            // PC Customization
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    var color = (Color) Application.Current.Resources["NativeAccentColor"];
                    titleBar.BackgroundColor = (Color) Application.Current.Resources["NativeAccentColor"];
                    titleBar.ButtonBackgroundColor = (Color) Application.Current.Resources["NativeAccentColor"];
                    titleBar.ForegroundColor = color;
                }
            }

            // Mobile Customization
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = (Color) Application.Current.Resources["NativeAccentColor"];
                }
            }

            // (Default)TitleBar komplett deaktivieren
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;

            // Launch in Window Mode
            var currentView = ApplicationView.GetForCurrentView();
            if (currentView.IsFullScreenMode)
                currentView.ExitFullScreenMode();
        }
    }
}