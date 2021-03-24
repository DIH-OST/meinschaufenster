// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle mit den druchgeführten Notifizierungen via Azure Mobile Center</para>
    ///     Klasse TableNotifications. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("TableNotifications")]
    public class TableNotifications
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        ///     RegistrationId aus AzurePushNotifications
        /// </summary>
        public string AzureTag { get; set; }

        /// <summary>
        ///     Wenn die Notifizeirung an einen bestimmten Benutzer gegangen ist => wenn null dann an alle registrierten Devices
        /// </summary>
        public virtual TableUser TblUser { get; set; }

        /// <summary>
        ///     Fremdschlüssel Benutzer-ID
        /// </summary>
        [ForeignKey(nameof(TblUser))]
        public int TblUserId { get; set; }

        #endregion
    }
}