// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using BaseApp.BissMvvm;
using Exchange;
using Exchange.Enum;
using Exchange.Resources;

namespace BaseApp
{
    // ReSharper disable once RedundantExtendsListEntry
    public class App : Application
    {
        /// <summary>
        ///     Nie aufrufen! Wird nur von Xamarin.Forms Previewer benötigt!
        /// </summary>
        public App()
        {
            ThreadHelper.Initialize(Environment.CurrentManagedThreadId);
            InitializeComponent();
            //throw new Exception("Invalid Initializion of BISS MVVM!");
        }

        /// <summary>
        ///     Xamarin BISS MvvM initialisieren und starten IBissAppPlattform
        /// </summary>
        //public App(string deviceId, VmFiles vmFiles)
        public App(IBissAppPlattform plattform)
        {
            ThreadHelper.Initialize(Environment.CurrentManagedThreadId);
            InitializeComponent();

            Language.CurrentDeviceCulture = CultureInfo.CurrentCulture;
            var setting = AppSettings.Current();

            setting.BaseNavigator = new ViewModelXamarinPlattformNavigation((App) Current, setting);
            setting.BaseFiles = plattform.Files;
            //setting.AppCenterServices = new[] {typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push)};
            setting.AppCenterServices = new[] {typeof(Analytics), typeof(Crashes)};
            setting.LanguageContent = Language.GetText();

            VmBase.InitComponentManager(setting);

            //ToDo: MKo Umbau finalisieren
            VmBase.AppConfigConstants = new AppConfigurationConstants(setting.AppConfigurationConstants);
            XamarinDeviceInfo.DeviceHardwareId = plattform.DeviceId;

            VmBase.CManager.InitHighest();
            MainPage = new ViewSplash(plattform.Files);
        }

        public event EventHandler<EnumLifeCycle> OnLifeCycleChanged;

        #region OnStart/Sleep/Resume

        protected override void OnStart()
        {
            base.OnStart();
            ProjectViewModelBase._mainMenu?.SubToNotifications();
            // Handle when your app starts
            Logging.Log.LogInfo("XamarinForms: OnStart");
            OnLifeCycleChanged?.Invoke(this, EnumLifeCycle.Started);
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            // Handle when your app sleeps
            Logging.Log.LogInfo("XamarinForms: OnSleep");
            ProjectViewModelBase._mainMenu?.UnSubNotifications();
            //SignalRExtension.BcSignalR(null).DisConnect();
            OnLifeCycleChanged?.Invoke(this, EnumLifeCycle.FellAsleep);
        }

        protected override void OnResume()
        {
            base.OnResume();
            // Handle when your app resumes
            Logging.Log.LogInfo("XamarinForms: OnResume");
            ProjectViewModelBase._mainMenu?.SubToNotifications();
            //SignalRExtension.BcSignalR(null).ReConnect();
            OnLifeCycleChanged?.Invoke(this, EnumLifeCycle.Resumed);
        }

        #endregion
    }
}