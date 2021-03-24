// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.PostRequests
{
    /// <summary>
    ///     <para>Post Daten für UserAccount abfragen</para>
    ///     Klasse ExPostUserAccountData. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExPostUserPasswortData
    {
        #region Properties

        /// <summary>
        ///     Datenbank Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Passwort gehashed
        /// </summary>
        public string PasswordHash { get; set; }

        #endregion
    }

    /// <summary>
    ///     <para>Post Daten für UserAccount abfragen</para>
    ///     Klasse ExPostUserAccountData. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExPostUserChangePasswortData
    {
        #region Properties

        /// <summary>
        ///     Datenbank Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Passwort gehashed
        /// </summary>
        public string OldPasswordHash { get; set; }

        /// <summary>
        ///     Passwort gehashed
        /// </summary>
        public string NewPasswordHash { get; set; }

        #endregion
    }
}