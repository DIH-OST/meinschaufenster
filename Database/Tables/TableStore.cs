// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für Geschäfte</para>
    ///     Klasse TableAppointment. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("Store")]
    public class TableStore
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Firmenname
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        ///     Beschreibungstext
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     email Addresse
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        ///     Link zu Website
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        ///     Aktivierungscode
        /// </summary>
        public string ActivationCode { get; set; }

        /// <summary>
        ///     Aktiv
        /// </summary>
        public bool Activated { get; set; }

        /// <summary>
        ///     Erstellungsdatum
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Postleitzahl
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        ///     Stadt
        /// </summary>
        public string City { get; set; }

        /// <summary>
        ///     Straße und Hausnummer
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     Land
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        ///     Bundesland
        /// </summary>
        public string FederalState { get; set; }

        /// <summary>
        ///     Koordinaten Breitengrad
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        ///     Koordinaten Längengrad
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        ///     Geschäftstelefon
        /// </summary>
        public string Telephonenumber { get; set; }

        /// <summary>
        ///     Passworthash
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Token für Rest
        /// </summary>
        public string RestPassword { get; set; }

        /// <summary>
        ///     Angestellten des Geschäfts
        /// </summary>
        public virtual List<TableEmployee> Employees { get; set; }

        /// <summary>
        ///     Öffnungszeiten des Geschäfts
        /// </summary>
        public virtual List<TableOpeningHours> OpeningHours { get; set; }

        /// <summary>
        ///     Sondertage des Geschäfts
        /// </summary>
        public virtual List<TableSpecialDay> SpecialDays { get; set; }

        /// <summary>
        ///     Bilder des Geschäfts
        /// </summary>
        public virtual List<TableImage> Images { get; set; }

        /// <summary>
        ///     Abwesenheiten des Geschäfts
        /// </summary>
        public virtual List<TableAbsence> Absences { get; set; }

        /// <summary>
        ///     Standorte des Geschäfts
        /// </summary>
        public virtual List<TableLocation> TblLocations { get; set; }

        /// <summary>
        ///     Produktkategorien des Geschäfts
        /// </summary>
        public virtual List<TableStoreCategory> TblStoreCategories { get; set; }

        /// <summary>
        ///     Bezahlmöglichkeiten des Geschäfts
        /// </summary>
        public virtual List<TableStorePayment> TblStorePayments { get; set; }

        /// <summary>
        ///     Liefermöglichkeiten des Geschäfts
        /// </summary>
        public virtual List<TableStoreDelivery> TblStoreDelivery { get; set; }

        #endregion
    }
}