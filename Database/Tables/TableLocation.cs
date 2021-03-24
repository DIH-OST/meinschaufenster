// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für nebenstandorte</para>
    ///     Klasse TableLocation. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("Location")]
    public class TableLocation
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
        public string Name { get; set; }

        /// <summary>
        ///     Email
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        ///     Postleitzahl
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        ///     Ort
        /// </summary>
        public string City { get; set; }

        /// <summary>
        ///     Adresse
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
        ///     Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        ///     Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        ///     Telefonnummer
        /// </summary>
        public string Telephonenumber { get; set; }

        /// <summary>
        ///     Geschäft welches den Nebenstandort hat
        /// </summary>
        public virtual TableStore Store { get; set; }

        /// <summary>
        ///     Id des Geschäfts welches den Nebenstandort hat
        /// </summary>
        [ForeignKey(nameof(Store))]
        public int StoreId { get; set; }

        /// <summary>
        ///     Angestellte des Nebenstandorts
        /// </summary>
        public virtual List<TableLocationEmployee> TblLocationEmployee { get; set; }

        #endregion
    }
}