// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für Angestellte</para>
    ///     Klasse TableAppointment. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("Employee")]
    public class TableEmployee
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Vorname
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Nachanme
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Telefonnummer
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        ///     Standard Anmerkung für Anfragen
        /// </summary>
        public string DefaultAnnotation { get; set; }

        /// <summary>
        ///     Aktiv
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///     Bild
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        ///     Termine des Angestellten
        /// </summary>
        public virtual List<TableAppointment> TblAppointments { get; set; }

        /// <summary>
        ///     Arbeitszeiten des Angestellten
        /// </summary>
        public virtual List<TableVirtualWorkTime> TblVirtualWorkTimes { get; set; }

        /// <summary>
        ///     Zwischentabelle zwischen Angestellten und Nebenstandorten
        /// </summary>
        public virtual List<TableLocationEmployee> TblLocationEmployee { get; set; }

        /// <summary>
        ///     Geschäft and dem der Angestellte arbeitet
        /// </summary>
        public virtual TableStore Store { get; set; }

        /// <summary>
        ///     Id des Geschäfts and dem der Angestellte arbeitet
        /// </summary>
        [ForeignKey(nameof(Store))]
        public int StoreId { get; set; }

        #endregion
    }
}