// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using BaseApp.ViewModel;
using Biss.Apps.Components;
using Biss.Apps.Components.Map.Base;
using Exchange;
using Exchange.Model;
using Exchange.PostRequests;
using Exchange.Resources;
using Exchange.ServiceAccess;

namespace BaseApp
{
    /// <summary>
    ///     <para>Basis View Model Projektspezifisch</para>
    ///     Klasse ViewModelBase. (C) 2016 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public abstract class ProjectViewModelBase : VmBase
    {
        public static BissPosition DefaultPosition = new BissPosition(48.25, 15.75);
        public static double DefaultZoomLevel = 100000;
        public static double MyZoomLevel = 5000;
        private static ExUserAccountData _userAccountData;
        public static ViewModelMenu _mainMenu;
        private static bool _deviceDataUpdatedInSession;

        protected static IDictionary<string, string> LaunchOptions;

        protected static bool AppIsAlreadyLaunched;


        protected ProjectViewModelBase(string pageTitle, object args = null, string subTitle = "") : base(pageTitle, args, subTitle)
        {
        }

        #region Properties

        /// <summary>
        ///     Aktueller User
        /// </summary>
        public static ExUserAccountData UserAccountData
        {
            get => _userAccountData;
            set
            {
                _userAccountData = value;
                if (_userAccountData != null)
                {
                    Sa.Initialize(_userAccountData.UserId.ToString(), _userAccountData.RestPasswort, Constants.ServiceClientEndPointWithApiPrefix);
                    SignalRExtension.BcSignalR(null).Initialize(_userAccountData.UserId, _userAccountData.RestPasswort, PushExtension.BcPush(null).PushDeviceGuid.ToString());
                    if (string.IsNullOrEmpty(_userAccountData.DefaultUserLanguage))
                        _userAccountData.DefaultUserLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                    UpdateLanguage(_userAccountData.DefaultUserLanguage);
                }
                else
                {
                    Sa.Initialize("", "", Constants.ServiceClientEndPointWithApiPrefix);
                    UpdateLanguage();
                }
            }
        }

        /// <summary>
        ///     Zugriff auf das Hauptmenü
        /// </summary>
        public ViewModelMenu MainMenu => _mainMenu;

        /// <summary>
        ///     View die nach Login/Registrierung geöffnet werden soll
        /// </summary>
        public static string ViewAfterLogin { get; set; } = "ViewMain";

        /// <summary>
        ///     Argumente die der View nach Login/Registrierung mitgegeben werden sollen
        /// </summary>
        public static object ViewArgsAfterLogin { get; set; }

        #endregion

        /// <summary>
        ///     Welche View soll initial getartet weren
        /// </summary>
        public static void LaunchFirstView()
        {
            var localData = FileExtensions.BcFiles(null).UserAppData.Load<ExUserAccountData>();
            UserAccountData = localData ?? null;

            var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            if (UserAccountData != null)
                lang = UserAccountData.DefaultUserLanguage;

            UpdateLanguage(lang, true);

            InvokeDispatcher(() =>
            {
                if (UserAccountData == null)
                {
                    NavigatorExtensions.BcNavigation(null).ToView("ViewMain");
                }
                else
                {
                    if (Constants.SupportLogin && !AppIsAlreadyLaunched)
                        AppCenterExtension.BcAppCenter(null).AppCenter.UpdateCurrentUser(UserAccountData.FullName, UserAccountData.PhoneNumber, UserAccountData.UserId.ToString());

                    AppIsAlreadyLaunched = true;

                    if (LaunchOptions != null && LaunchOptions.Any())
                    {
                        if (ToastExtension.BcToast(null) == null || ToastExtension.BcToast(null).State != EnumComponentState.Ok)
                        {
                            CManager.ComponetSetups.BcInitToast();
                        }

                        if (ToastExtension.BcToast(null) != null && ToastExtension.BcToast(null).State == EnumComponentState.Ok)
                        {
                            ToastExtension.BcToast(null).Show("Started from Notification: " + string.Join("; ", LaunchOptions.Select(x => x.Key + ": " + x.Value)));
                        }

                        NavigatorExtensions.BcNavigation(null).ToView("ViewPushInfo", LaunchOptions);
                        LaunchOptions = null;
                    }
                    else
                    {
                        NavigatorExtensions.BcNavigation(null).ToView("ViewMain");
                    }
                }
            });
        }

        public static void PushReceived(IDictionary<string, string> customData)
        {
            Logging.Log.LogInfo("PVMB: PushReceived: " + string.Join("; ", customData.Select(x => x.Key + ": " + x.Value)));

            LaunchOptions = customData;

            if (AppIsAlreadyLaunched)
            {
                LaunchFirstView();
            }
        }

        /// <summary>
        ///     Projekt Initialisieren
        /// </summary>
        public static void InitializeApp()
        {
            var localData = FileExtensions.BcFiles(null).UserAppData.Load<ExUserAccountData>();
            UserAccountData = localData ?? null;
        }

        /// <summary>
        ///     Systemsprache aktualisieren nachdem sie beim User verändert wurde
        /// </summary>
        public static void UpdateLanguage(string languageCode = "de", bool fistInit = false)
        {
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName != languageCode || fistInit)
            {
                var newCulture = new CultureInfo(languageCode);

                CultureInfo.CurrentCulture = newCulture;
                CultureInfo.CurrentUICulture = newCulture;

                Language.SetLanguage(newCulture);

                if (_mainMenu != null)
                    _mainMenu.UnSubNotifications();

                _mainMenu = new ViewModelMenu();
                _mainMenu.SetInitialVmCommand(_mainMenu.CmdAllMenuCommands.FirstOrDefault());
                _mainMenu.SubToNotifications();

                NavigatorExtensions.BcNavigation(null).Navigator.UpdateMenu();
                CManager.UptdateLanguage(Language.GetText());
            }
        }

        public static void UpdateMenu()
        {
            if (_mainMenu != null)
                _mainMenu.UnSubNotifications();

            _mainMenu = new ViewModelMenu();
            _mainMenu.SetInitialVmCommand(_mainMenu.CmdAllMenuCommands.FirstOrDefault());
            _mainMenu.SubToNotifications();

            NavigatorExtensions.BcNavigation(null).Navigator.UpdateMenu();
        }

        #region NUGET Biss.Apps.*

        #region Biss.Apps.AppCenter

        /// <summary>
        ///     Zugriff auf das AppCenter (sollte das AppCenter (via Biss.Apps.Appcenter nuget) nich verwendet werden - löschen)
        /// </summary>
        /// <returns></returns>
        protected override VmBissAppCenter GetAppCenter()
        {
            if (Nav.AppCenter == null) Nav.AppCenter = AppCenterExtension.BcAppCenter(null).AppCenter;
            return AppCenterExtension.BcAppCenter(null).AppCenter;
        }

        #endregion

        #region Biss.Apps.Rest => DeviceInfo

        /// <summary>
        ///     Zugriff auf die (Web)Services
        /// </summary>
        public static RestAccess Sa = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);

        /// <summary>
        ///     Daten über das aktuelle Device an die Cloud senden. Wird unter anderem für die Notifizierungen benötigt.
        ///     Sollte aufgerufen werden wenn der User eingeloggt ist. Wenn die App keine User unterstützt:
        ///     Db und Funktionen umbauen (das die Devices ohne User angelegt werden) ODER
        ///     Einen "ALLUSER" anlegen => wenn erslichtlich das eventuell mal ein Login hinzukommt
        /// </summary>
        public async Task DeviceInfoUpdate()
        {
            if (_deviceDataUpdatedInSession)
                return;

            if (Constants.SupportLogin == false || UserAccountData == null || UserAccountData.IsDemoUser || UserAccountData.UserId < 0)
                return;

            var r = await Sa.UserDeviceUpdate(new ExUserDeviceInfo
                                              {
                                                  UserId = UserAccountData.UserId,
                                                  Device = new DbUserDeviceInfo
                                                           {
                                                               Plattform = DeviceInfo.Plattform,
                                                               DeviceIdiom = DeviceInfo.DeviceIdiom,
                                                               DeviceType = DeviceInfo.DeviceType,
                                                               OperatingSystemVersion = DeviceInfo.OperatingSystemVersion,
                                                               DeviceToken = this.BcPush().PushDeviceGuid.ToString()
                                                           }
                                              });

            if (!r.Ok || r.Result != true)
                Logging.Log.LogWarning("DeviceInfoUpdate fail!");
            else
                _deviceDataUpdatedInSession = true;
        }

        /// <summary>
        ///     Device Infos vom Server löschen => Beim ausloggen aufrufen!
        /// </summary>
        public async Task DeviceInfoDelete()
        {
            if (Constants.SupportLogin == false || UserAccountData == null || UserAccountData.IsDemoUser || UserAccountData.UserId < 0)
                return;

            var r = await Sa.UserDeviceDelete(new ExPostUserDeviceDelete
                                              {
                                                  UserId = UserAccountData.UserId,
                                                  Plattform = DeviceInfo.Plattform,
                                                  DeviceToken = this.BcPush().PushDeviceGuid.ToString()
                                              });

            if (!r.Ok || r.Result != true)
                Logging.Log.LogWarning("DeviceInfoDelete fail!");

            _deviceDataUpdatedInSession = false;
        }

        #endregion

        #region Biss.Apps.SignalR

        /// <summary>
        ///     SignalR Zugriff für alle Apps
        /// </summary>
        public IVmSignalR SignalR => SignalRExtension.BcSignalR(null);

        #endregion

        #endregion
    }
}