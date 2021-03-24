// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Infos zu einem Ger�t eines User</para>
    ///     Klasse ExPostUserDeviceInfo. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class DbUserDeviceInfo
    {
        #region Properties

        /// <summary>
        ///     Eindeutiger Token des Ger�res
        ///     APPS: Via Mobile Center => MobileCenter.GetInstallIdAsync
        ///     WPF: Via BissMachineId
        /// </summary>
        public string DeviceToken { get; set; }

        /// <summary>
        ///     Plattform des Ger�tes
        /// </summary>
        public EnumPlattform Plattform { get; set; }

        /// <summary>
        ///     Ger�teart
        /// </summary>
        public EnumDeviceIdiom DeviceIdiom { get; set; }

        /// <summary>
        ///     Version des Os
        /// </summary>
        public string OperatingSystemVersion { get; set; }

        /// <summary>
        ///     Info zum Device/Ger�teart
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        ///     Wann war das Ger�t das letze Mal online
        /// </summary>
        public DateTime LastDateTimeUtcOnline { get; set; }

        /// <summary>
        ///     L�uft auf diesem Ger�t die Applikation gerade (via SignalR)
        /// </summary>
        public bool IsAppRunning { get; set; }

        #endregion
    }

    /// <summary>
    ///     <para>Infos zu einem Ger�t eines User</para>
    ///     Klasse ExPostUserDeviceInfo. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExUserDeviceInfo
    {
        #region Properties

        /// <summary>
        ///     UserId des Benutzers (aus TableUser)
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Daten f�r das Ger�t (aus Db)
        /// </summary>
        public DbUserDeviceInfo Device { get; set; }

        #endregion
    }

    /// <summary>
    ///     <para>Ger�te eines User</para>
    ///     Klasse ExPostUserDeviceInfo. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExUserDevices
    {
        #region Properties

        /// <summary>
        ///     UserId des Benutzers (aus TableUser)
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Ger�te des Users
        /// </summary>
        public List<DbUserDeviceInfo> Devices { get; set; }

        #endregion
    }
}