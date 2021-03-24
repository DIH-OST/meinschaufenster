// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Helper;

namespace Database.Tables
{
    /// <summary>
    ///     <para>TableUser</para>
    ///     Klasse TableUser. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("Users")]
    public class TableUser
    {
        public TableUser()
        {
            RestPasswort = PasswordHelper.GeneratePassword();
            CreatedAtUtc = DateTime.UtcNow;
        }

        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Benutzer ist gesperrt => kann sich nicht anmelden!
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        ///     Telefonnummer = UserName => Gültige E-Mail!
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     Überprüfung der Telefonnummer war erfolgreich
        /// </summary>
        public bool PhoneChecked { get; set; }

        /// <summary>
        ///     DefaultUserLanguage
        /// </summary>
        public string DefaultUserLanguage { get; set; }

        /// <summary>
        ///     Passwort Hash
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Passwort für REST "[Authorize]" Kommunikation. Dieses Passwort wird automatisch erzeugt wenn der User angelegt wird
        /// </summary>
        public string RestPasswort { get; set; }

        /// <summary>
        ///     Erzeugungsdatum (zB um unnötige User zu löschen)
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        ///     Dieser Account ist der "Demo" Account - nur ein Datensatz darf true sein
        /// </summary>
        public bool IsDemoUser { get; set; }

        /// <summary>
        ///     Dieser Account ist ein "Admin" Account
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        ///     Vorname
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        ///     Nachname
        /// </summary>
        public string Lastname { get; set; }

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

        /// <summary>
        ///     Geräte des Benutzers
        /// </summary>
        public virtual List<TableUserDevice> TblUserDevices { get; set; }

        /// <summary>
        ///     Termine des Benutzers
        /// </summary>
        public virtual List<TableAppointment> TblAppointments { get; set; }

        #endregion
    }
}