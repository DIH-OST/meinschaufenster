// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ExtUserDeviceInfo. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExExtendedUserDeviceInfo : DbUserDeviceInfo
    {
        #region Properties

        /// <summary>
        ///     Benutzername
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     BenutzerId
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     GeräteId
        /// </summary>
        public int DeviceId { get; set; }

        #endregion
    }
}