// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Biss.Apps.Interfaces;
using Exchange.Resources;

namespace Exchange
{
    public class AppSettings :
        IAppSettings,
        IAppSettingsDataBaseOrNoSql,
        IAppSettingsRest,
        IAppSettingsEMail,
        IAppSettingsNotification,
        ISettingsSignalR,
        IAppSettingsMap,
        IAppSettingsAppCenter,
        IAppSettingsNavigation,
        IAppFiles
    {
        private static AppSettings _current;

        #region Properties

        public string BranchName { get; set; }
        public int AppConfigurationConstants { get; set; }
        public string PackageName { get; set; }
        public string AppVersion { get; set; }
        public string AppName { get; set; }
        public string AssemblyDescription { get; set; }
        public string AssemblyConfiguration { get; set; }
        public string AssemblyCompany { get; set; }
        public string AssemblyCopyright { get; set; }
        public string AssemblyTrademark { get; set; }
        public string AssemblyCulture { get; set; }

        #region IAppFiles

        public VmFiles BaseFiles { get; set; }

        #endregion

        #endregion

        public static AppSettings Current()
        {
            if (_current == null)
            {
                _current = new AppSettings
                           {
                               BranchName = "dev",
                               AppConfigurationConstants = 2,
                               PackageName = "at.dihost.digitalesschaufenster.dev",
                               AppVersion = "1.0.1.5",
                               AppName = "Schau DEV",
                               AssemblyDescription = "DIHOST-Digitales Schaufenster",
                               AssemblyConfiguration = "",
                               AssemblyCompany = "FOTEC Forschungs- und Technologietransfer GmbH",
                               AssemblyCopyright = "Copyright © 1998-2020 FOTEC - Forschungs- und Technologietransfer GmbH",
                               AssemblyTrademark = "",
                               AssemblyCulture = "",
                               DefaultUserId = -1,
                               LanguageContent = null,
                               ProjectWorkUserFolder = "digitalshoppingDev",
                               ConnectionString = "",
                               ConnectionStringDb = "[RELEASEONLY]",
                               ConnectionStringDbServer = "[RELEASEONLY]",
                               ConnectionStringUser = "[RELEASEONLY]",
                               ConnectionStringUserPwd = "[RELEASEONLY]",
                               DefaultWebApp = "[RELEASEONLY]",
                               DefaultWebEndpoint = "[RELEASEONLY]",
                               SendEMailAs = "[RELEASEONLY]",
                               SendEMailAsDisplayName = "Info meinschaufenster.at DEV",
                               NotificationAppNameAndroid = "[RELEASEONLY]",
                               NotificationAppNameIOs = "[RELEASEONLY]",
                               NotificationAppNameUwp = "[RELEASEONLY]",
                               NotificationBaseApiUrl = "[RELEASEONLY]",
                               NotificationOrganizationName = "[RELEASEONLY]",
                               NotificationToken = "[RELEASEONLY]",
                               DeviceIdentifier = "",
                               LogLevel = LogLevel.Warning,
                               SignalRBaseUrl = "[RELEASEONLY]",
                               TimeToReconnectMs = 5000,
                               UserIdentifier = 0,
                               UserTokenGuid = "",
                               BingMapsApplicationId = "[RELEASEONLY]",
                               GoogleMapsApiKey = "[RELEASEONLY]",
                               AndroidSecret = "[RELEASEONLY]",
                               AppCenterServices = null,
                               IosSecret = "[RELEASEONLY]",
                               UwpSecret = "[RELEASEONLY]",
                               BaseNavigator = null,
                               DefaultBackText = ResView.Command_Back,
                               DefaultViewNamespace = "BaseApp.View.",
                               BaseFiles = null,
                           };
            }

            return _current;
        }

        #region IAppSettings

        public int DefaultUserId { get; set; }
        public ExLanguageContent LanguageContent { get; set; }
        public string ProjectWorkUserFolder { get; set; }

        #endregion

        #region IAppSettingsDataBaseOrNoSql

        public string ConnectionString { get; set; }
        public string ConnectionStringDb { get; set; }
        public string ConnectionStringDbServer { get; set; }
        public string ConnectionStringUser { get; set; }
        public string ConnectionStringUserPwd { get; set; }

        #endregion

        #region IAppSettingsRest

        public string DefaultWebApp { get; set; }
        public string DefaultWebEndpoint { get; set; }

        #endregion

        #region IAppSettingsEMail

        public string SendEMailAs { get; set; }
        public string SendEMailAsDisplayName { get; set; }

        #endregion

        #region IAppSettingsNotification

        public string NotificationAppNameAndroid { get; set; }
        public string NotificationAppNameIOs { get; set; }
        public string NotificationAppNameUwp { get; set; }
        public string NotificationBaseApiUrl { get; set; }
        public string NotificationOrganizationName { get; set; }
        public string NotificationToken { get; set; }

        #endregion

        #region ISettingsSignalR

        public string DeviceIdentifier { get; set; }
        public LogLevel LogLevel { get; set; }
        public string SignalRBaseUrl { get; set; }
        public int TimeToReconnectMs { get; set; }
        public int UserIdentifier { get; set; }
        public string UserTokenGuid { get; set; }

        #endregion

        #region IAppSettingsMap

        public string BingMapsApplicationId { get; set; }
        public string GoogleMapsApiKey { get; set; }

        #endregion

        #region IAppSettingsAppCenter

        public string AndroidSecret { get; set; }
        public Type[] AppCenterServices { get; set; }
        public string IosSecret { get; set; }
        public string UwpSecret { get; set; }

        #endregion

        #region IAppSettingsNavigation

        public VmNavigator BaseNavigator { get; set; }
        public string DefaultBackText { get; set; }
        public string DefaultViewNamespace { get; set; }

        #endregion
    }
}