// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>ExUserAccountData - Diese Daten werden lokal (verschlüsselt) gesichert um sich das Login zu merken</para>
    ///     Klasse ExUserAccountData. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExUserAccountData : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     User Id (aus TableUser)
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Passwort für REST "[Authorize]" Kommunikation. Dieses Passwort wird automatisch erzeugt wenn der User angelegt wird
        /// </summary>
        public string RestPasswort { get; set; }

        /// <summary>
        ///     Name des eingeloggten User
        /// </summary>
        [JsonIgnore]
        [DependsOn(nameof(FirstName), nameof(LastName))]
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        ///     Vorname
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Nachname
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Nummer des eingeloggten User
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     App ist mit DemoUser eingeloggt
        /// </summary>
        public bool IsDemoUser { get; set; }

        /// <summary>
        ///     Benutzer ist Administrator
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        ///     Standardsprache des Benutzers
        /// </summary>
        public string DefaultUserLanguage { get; set; }

        /// <summary>
        ///     Straße und Hausnummer
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        ///     PLZ
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        ///     Ort
        /// </summary>
        public string City { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }

    /// <summary>
    ///     <para>Daten welche beim GetUserAccount zurückgegeben werden</para>
    ///     Klasse ExUserAccountDataResult. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExUserAccountDataResult
    {
        #region Properties

        /// <summary>
        ///     Account ist gerade gesperrt
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        ///     Passwort ist falsch
        /// </summary>
        public bool PasswordWrong { get; set; }

        /// <summary>
        ///     Benutzer wenn erlaubt sonst null
        /// </summary>
        public ExUserAccountData UserAccountData { get; set; }

        #endregion
    }
}