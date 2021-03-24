// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für Öffnungszeiten einers Geschäfts</para>
    ///     Klasse TableAppointment. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("OpeningHours")]
    public class TableOpeningHours
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Wochentag
        /// </summary>
        public DayOfWeek Weekday { get; set; }

        /// <summary>
        ///     Zeit von
        /// </summary>
        public string From { get; set; }

        /// <summary>
        ///     Zeit bis
        /// </summary>
        public string To { get; set; }

        /// <summary>
        ///     Umstellung von DateTime  auf Time (From/To) MKo
        /// </summary>
        public bool IsMigrated { get; set; }

        /// <summary>
        ///     Zeit von UTC!
        /// </summary>
        public DateTime? TimeFrom { get; set; }

        /// <summary>
        ///     Zeit von non UTC
        /// </summary>
        [NotMapped]
        public DateTime? TimeFromLocal
        {
            get => TimeFrom != null ? (DateTime?) TimeFrom.Value.AddHours(2) : null;
            set => TimeFrom = value != null ? (DateTime?) value.Value.AddHours(-2) : null;
        }

        /// <summary>
        ///     Zeit bis UTC!
        /// </summary>
        public DateTime? TimeTo { get; set; }

        /// <summary>
        ///     Zeit bis non UTC
        /// </summary>
        [NotMapped]
        public DateTime? TimeToLocal
        {
            get => TimeTo != null ? (DateTime?) TimeTo.Value.AddHours(2) : null;
            set => TimeTo = value != null ? (DateTime?) value.Value.AddHours(-2) : null;
        }

        /// <summary>
        ///     Geschäft welches diese Öffnungszeiten hat.
        /// </summary>
        public virtual TableStore Store { get; set; }

        /// <summary>
        ///     Id des Geschäfts welches diese Öffnungszeiten hat.
        /// </summary>
        [ForeignKey(nameof(Store))]
        public int StoreId { get; set; }

        #endregion
    }
}