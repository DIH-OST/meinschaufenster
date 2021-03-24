// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp.Models
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse AppointmentEntryViewModel. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class AppointmentEntryViewModel
    {
        #region Properties

        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string PreviewText { get; set; }
        public string OptionalText { get; set; }
        public int ShopId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int StaffId { get; set; }

        public string StartTimeTime => StartTime.ToString("HH:mm");
        public string EndTimeTime => EndTime.ToString("HH:mm");
        public string ImageUrl { get; set; }

        #endregion
    }
}