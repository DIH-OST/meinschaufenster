// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für virtualle Arbeitszeit der Angestellten</para>
    ///     Klasse TableVirtualWorkTime. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("VirtualWorkTime")]
    public class TableVirtualWorkTime
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

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
        ///     Wochentag
        /// </summary>
        public DayOfWeek Weekday { get; set; }

        /// <summary>
        ///     Startzeit
        /// </summary>
        public TimeSpan TimeFrom { get; set; }

        /// <summary>
        ///     Endzeit
        /// </summary>
        public TimeSpan TimeTo { get; set; }

        /// <summary>
        ///     Dauer der einzelnen Beratungstermine in Minuten
        /// </summary>
        public int TimeSlot { get; set; }

        /// <summary>
        ///     Betroffener Angestellter
        /// </summary>
        public virtual TableEmployee Employee { get; set; }

        /// <summary>
        ///     ID des betroffenen Angestellten
        /// </summary>
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        #endregion
    }
}