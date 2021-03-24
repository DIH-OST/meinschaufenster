// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für Termine zwischen Kunder und Angestellter</para>
    ///     Klasse TableAppointment. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("Appointment")]
    public class TableAppointment
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Anfragetext vom User
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Storniert
        /// </summary>
        public bool Canceled { get; set; }

        /// <summary>
        ///     Wahrgenommen
        /// </summary>
        public bool Attended { get; set; }

        /// <summary>
        ///     Gebucht am
        /// </summary>
        public DateTime BookedOn { get; set; }


        /// <summary>
        ///     Datum ohne Zeitanteil
        /// </summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        ///     Uhrzeit
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        ///     Dauer des Termins in Minuten
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        ///     Umstellung von ValidFrom/Valid To auf neues Format
        /// </summary>
        public bool IsMigrated { get; set; }


        /// <summary>
        ///     Gültig von in UTC
        ///     TODO check UTC!
        /// </summary>
        public DateTime ValidFrom { get; set; }

        /// <summary>
        ///     Gültig bis in UTC
        ///     TODO check UTC!
        /// </summary>
        public DateTime ValidTo { get; set; }

        /// <summary>
        ///     User welcher am Termin teilnimmt
        /// </summary>
        public virtual TableUser User { get; set; }

        /// <summary>
        ///     Id des Users welcher am Termin teilnimmt
        /// </summary>
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        /// <summary>
        ///     Ansgestelter welcher am Termin teilnimmt
        /// </summary>
        public virtual TableEmployee Employee { get; set; }

        /// <summary>
        ///     Id des Angestellten welcher am Termin teilnimmt
        /// </summary>
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        #endregion
    }
}