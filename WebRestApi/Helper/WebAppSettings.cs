// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebRestApi.Helper
{
    /// <summary>
    ///     Einstellungen für die WebApplikation
    /// </summary>
    public class WebAppSettings
    {
        /// <summary>
        ///     Credentials für Sendgrid
        /// </summary>
        public static SendGridCredentials EmailCredentials = new SendGridCredentials
                                                             {
                                                                 ApiKeyV3 = "[RELEASE]",
                                                             };

        /// <summary>
        ///     Passwort für Sicheren Austausch zwischen WebApp und WebApi
        /// </summary>
        public static string CheckPassword = "[RELEASE]";

        #region Properties

        /// <summary>
        ///     Secret für den JWT (JSON Web Token)
        /// </summary>
        public string Secret { get; set; }

        #endregion
    }
}