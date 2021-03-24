// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>ExCheckUser</para>
    ///     Klasse ExUserPasswordHash. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExCheckUser
    {
        #region Properties

        /// <summary>
        ///     Datenbank Id des User
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Der User ist in der DB gesperrt und kann sich daher nicht anmelden
        /// </summary>
        public bool UserIsLocked { get; set; }

        /// <summary>
        ///     E-Mail ist nicht im System => neuer User
        /// </summary>
        public bool IsNewUser { get; set; }

        /// <summary>
        ///     E-Mail wurde noch nicht validiert ob auch der Besitzer
        /// </summary>
        public bool EMailNotChecked { get; set; }

        /// <summary>
        ///     Dieser Account ist der Demo Account - "Normales" Login ist nicht möglich!
        /// </summary>
        public bool IsDemoUser { get; set; }

        /// <summary>
        ///     Fehler beim Speichern in die DB
        /// </summary>
        public bool ErrorFromDb { get; set; }

        /// <summary>
        ///     Nummer konnte nicht richtig konvertiert werden
        /// </summary>
        public bool WrongNumberFormat { get; set; }

        #endregion
    }
}