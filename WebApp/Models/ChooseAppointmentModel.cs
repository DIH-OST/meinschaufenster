// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp.Models
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ChooseAppointmentModel. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ChooseAppointmentModel
    {
        #region Properties

        public int StoreId { get; set; }
        public DateTime FilterFrom { get; set; }

        #endregion
    }

    public class ChooseAppointmentModelV2
    {
        #region Properties

        public int storeId { get; set; }
        public string filterFromDate { get; set; }

        #endregion
    }
}