// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp.Models
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse AppointmentModel. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class AppointmentModel
    {
        #region Properties

        public int Id { get; set; }

        public string Text { get; set; }

        public string Name { get; set; }

        /// <summary>
        ///     Gültig von
        /// </summary>
        public DateTime ValidFrom { get; set; }

        /// <summary>
        ///     Gültig bis
        /// </summary>
        public DateTime ValidTo { get; set; }


        public string Phonenumber { get; set; }

        public string ValidFromTime
        {
            get => ValidFrom.ToShortTimeString();
        }

        public string ValidToTime
        {
            get => ValidTo.ToShortTimeString();
        }

        public int Duration
        {
            get => (int) (ValidTo - ValidFrom).TotalMinutes;
        }

        #endregion
    }
}