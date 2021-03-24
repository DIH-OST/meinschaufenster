// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;

namespace Exchange.PostRequests
{
    /// <summary>
    ///     <para>Ein bestimmtes Gerät eines Users löschen</para>
    ///     Klasse ExPostUserDeviceInfo. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExPostUserDevice
    {
        #region Properties

        /// <summary>
        ///     UserId des Benutzers (aus TableUser)
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Infos zum Device
        /// </summary>
        public DbUserDeviceInfo DeviceInfo { get; set; }

        #endregion
    }


    /// <summary>
    ///     <para>Ein bestimmtes Gerät eines Users löschen</para>
    ///     Klasse ExPostUserDeviceInfo. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExPostUserDeviceDelete
    {
        #region Properties

        /// <summary>
        ///     UserId des Benutzers (aus TableUser)
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Eindeutiger Token des Geräres
        ///     APPS: Via Mobile Center => MobileCenter.GetInstallIdAsync
        ///     WPF: Via BissMachineId
        /// </summary>
        public string DeviceToken { get; set; }

        /// <summary>
        ///     Plattform des Gerätes
        /// </summary>
        public EnumPlattform Plattform { get; set; }

        /// <summary>
        ///     Passwortcheck für Auth bei WebApp
        /// </summary>
        public string CheckPassword { get; set; }

        #endregion
    }
}