// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;

namespace WebApp.Models
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ExShopViewModel. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExShopViewModel
    {
        #region Properties

        public ExShop Shop { get; set; }
        public string AllHours { get; set; }
        public string HoursToday { get; set; }

        #endregion
    }
}