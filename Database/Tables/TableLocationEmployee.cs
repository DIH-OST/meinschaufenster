// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Database.Tables
{
    /// <summary>
    ///     <para>Kreuztabelle Mitarbeiter und Store Location</para>
    ///     Klasse TableLocationEmployee. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("LocationEmployee")]
    public class TableLocationEmployee
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Standort ID
        /// </summary>
        [ForeignKey(nameof(TblLocation))]
        public int TblLocationId { get; set; }

        /// <summary>
        ///     Standort
        /// </summary>
        public virtual TableLocation TblLocation { get; set; }

        /// <summary>
        ///     Mitarbeiter ID
        /// </summary>
        [ForeignKey(nameof(TblEmployee))]
        public int TblEmployeeId { get; set; }

        /// <summary>
        ///     Mitarbeiter
        /// </summary>
        public virtual TableEmployee TblEmployee { get; set; }

        #endregion
    }
}