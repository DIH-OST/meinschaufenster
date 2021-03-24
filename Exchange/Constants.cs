// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Notifications;


namespace Exchange
{
    /// <summary>
    ///     <para>Konstanten für alle Projekte</para>
    ///     Klasse Constants. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class Constants
    {
        /// <summary>
        ///     Benötigt die App Zugriff auf das Internet, bei false wird auch der empfang von Notifizierungen deaktiviert
        /// </summary>
        public const bool AppNeedInternet = true;

        /// <summary>
        ///     Besitz die App einen User (oder komplett ohne Userbezug)
        /// </summary>
        public const bool SupportLogin = true;

        /// <summary>
        ///     Gibt es einen Demo User
        /// </summary>
        public const bool HasDemoUser = true;

        /// <summary>
        ///     Bing Maps Key für die FMS Applikation
        /// </summary>
        public const string BingMapsApplicationId = "AiVpeNHs8eSiqGvkWZ6cxkvA3AbwFfBuRSaagWNs8zyOrUclm6XiRWIjUn4KqkU1";

        /// <summary>
        ///     Gültigkeit des Authentication Tokens
        /// </summary>
        public const double AuthTokenExpiry = 120;

        /// <summary>
        ///     Name der Variable (für mobile.azure.com) um die User Id zu sichern für Notifizierung und Telemetrie
        /// </summary>
        public const string AppCenterUserIdVarName = "userId";


        public static AppSettings CurrentAppSettings;


        /// <summary>
        ///     Konfiguration für das versenden von Notifizierungen durch mobile.azure.com
        /// </summary>
        public static MobileCenterNotificationConfiguration MobileCenterNotification;

        /// <summary>
        ///     Hockey App Id für den WPF Client
        /// </summary>
        public static string HockeyAppId = "[ID]";

        /// <summary>
        ///     mobile.azure.com Analytics Id für UWP
        /// </summary>
        public static string UwpAnalyticsId = "[ID]";

        /// <summary>
        ///     mobile.azure.com Analytics Id für Android
        /// </summary>
        public static string AndroidAnalyticsId = "[ID]";

        /// <summary>
        ///     mobile.azure.com Analytics Id für iOS
        /// </summary>
        public static string IosAnalyticsId = "[ID]";

        /// <summary>
        ///     Titel für die Apps
        /// </summary>
        public static string MainTitle = "BISS Demo ";

        /// <summary>
        ///     E-Mail Sender
        /// </summary>
        public static string SendEMailAs = "biss@fotec.at";

        /// <summary>
        ///     E-Mail Sender Anzeigename
        /// </summary>
        public static string SendEMailAsDisplayName = "MeinSchaufenster.at";

        /// <summary>
        ///     Endpunkt für Web
        /// </summary>
        public static string DefaulSignalREndpoint;

        /// <summary>
        ///     Endpunkt für Web
        /// </summary>
        public static string DefaultWebEndpoint;

        /// <summary>
        ///     Endpunkt für REST
        /// </summary>
        public static string ServiceClientEndPointWithApiPrefix;

        /// <summary>
        ///     Endpunkt für SingalRProxy
        /// </summary>
        public static string SignalRProxyEndPointWithApiPrefix;

        /// <summary>
        ///     DB Connection String
        /// </summary>
        public static string ConnectionString;

        /// <summary>
        ///     UserId für Tests => Default ist -1 für den Life Betrieb, -2 für den Developer Mode bzw. die EmployeeId
        /// </summary>
        public static int DefaultUserId = -1;

        /// <summary>
        ///     Aktueller USer Ordner, Abhängig von DEV/BETA/RELEASE
        /// </summary>
        public static string ProjectWorkUserFolder;

        /// <summary>
        ///     Wo läuft die WebApp
        /// </summary>
        public static string WebAppBaseUrl = "";


        /// <summary>
        ///     Aktuelle App Settings für verschiedene Versionen (Release, Beta, Dev)
        /// </summary>
        public static AppConfigurationConstants AppConfiguration = new AppConfigurationConstants(AppSettings.Current().AppConfigurationConstants);

        static Constants()
        {
            Logging.Init(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));

            CurrentAppSettings = AppSettings.Current();

            if (string.IsNullOrEmpty(AppSettings.Current().ConnectionString))
                ConnectionString = new CsBuilderSql(AppSettings.Current().ConnectionStringDbServer, AppSettings.Current().ConnectionStringDb, AppSettings.Current().ConnectionStringUser, AppSettings.Current().ConnectionStringUserPwd, SqlCommonStandardApplicationName.EntityFramework).ToString();
            else
                ConnectionString = AppSettings.Current().ConnectionString;

            DefaultWebEndpoint = AppSettings.Current().DefaultWebEndpoint;
            DefaulSignalREndpoint = AppSettings.Current().SignalRBaseUrl;
            DefaultUserId = AppSettings.Current().DefaultUserId;
            SendEMailAs = AppSettings.Current().SendEMailAs;
            SendEMailAsDisplayName = AppSettings.Current().SendEMailAsDisplayName;
            ProjectWorkUserFolder = AppSettings.Current().ProjectWorkUserFolder;
            UwpAnalyticsId = AppSettings.Current().UwpSecret;
            AndroidAnalyticsId = AppSettings.Current().AndroidSecret;
            IosAnalyticsId = AppSettings.Current().IosSecret;
            MobileCenterNotification = new MobileCenterNotificationConfiguration
                                       {
                                           BaseApiUrl = AppSettings.Current().NotificationBaseApiUrl,
                                           Token = AppSettings.Current().NotificationToken,
                                           OrganizationName = AppSettings.Current().NotificationOrganizationName,
                                           //AppNameIOs = "biss.demo.inhouse-iOs",
                                           AppNameIOs = AppSettings.Current().NotificationAppNameIOs,
                                           AppNameAndroid = AppSettings.Current().NotificationAppNameAndroid,
                                           AppNameUwp = AppSettings.Current().NotificationAppNameUwp
                                       };

            WebAppBaseUrl = AppSettings.Current().DefaultWebApp;

            ServiceClientEndPointWithApiPrefix = $"{DefaultWebEndpoint}api/";
            SignalRProxyEndPointWithApiPrefix = $"{DefaulSignalREndpoint}api/";
        }
    }
}