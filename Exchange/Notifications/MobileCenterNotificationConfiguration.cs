// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Notifications
{
    /// <summary>
    ///     <para>MobileCenterNotificationData</para>
    ///     Klasse MobileCenterNotificationConfiguration. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class MobileCenterNotificationConfiguration
    {
        #region Properties

        /// <summary>
        ///     Token für die Schnittstelle
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        ///     Name der Organisation in der die Apps in mobile.azure.com liegen
        ///     https://api.mobile.azure.com/v0.1/orgs
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        ///     IOS App Name in mobile.azure.com
        ///     https://api.mobile.azure.com/v0.1/orgs/FOTEC-BISS/apps
        /// </summary>
        public string AppNameIOs { get; set; }

        /// <summary>
        ///     Android App Name in mobile.azure.com
        ///     https://api.mobile.azure.com/v0.1/orgs/FOTEC-BISS/apps
        /// </summary>
        public string AppNameAndroid { get; set; }

        /// <summary>
        ///     Uwp App Name in mobile.azure.com
        ///     https://api.mobile.azure.com/v0.1/orgs/FOTEC-BISS/apps
        /// </summary>
        public string AppNameUwp { get; set; }

        /// <summary>
        ///     API URL für mobile.azure.com
        /// </summary>
        public string BaseApiUrl { get; set; }

        #endregion
    }
}