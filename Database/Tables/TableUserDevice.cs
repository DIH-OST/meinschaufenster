// DigitalesSchaufenster (C) 2020 DIH-OST

using Exchange.Model;

namespace Database.Tables
{
    /// <summary>
    ///     <para>TableUserDevice</para>
    ///     Klasse TableUserDevice. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("UserDevices")]
    public class TableUserDevice
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Daten für das Gerät
        /// </summary>
        public virtual DbUserDeviceInfo Device { get; set; }

        /// <summary>
        ///     User des Devices
        /// </summary>
        public virtual TableUser TblUser { get; set; }

        /// <summary>
        ///     User Id des Devices
        /// </summary>
        [ForeignKey(nameof(TblUser))]
        public int TblUserId { get; set; }

        #endregion
    }
}