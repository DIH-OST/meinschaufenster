// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für wichtige Einstellungen die oft verwendet werden</para>
    ///     Klasse TableAbsence. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("StaticSetting")]
    public class TableStaticSetting
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     An wen sollen E-Mails gesandt werden wenn Kunde oder Geschäft gelöscht wurde
        ///     Mit ; getrennt
        /// </summary>
        public string EMailsToInform { get; set; }

        /// <summary>
        ///     Welcher Text soll in WebApp und App angezeigt werden falls ein Service geplant ist
        ///     Leer lassen im Normalbetrieb
        /// </summary>
        public string MaintenanceInfo { get; set; }

        #endregion
    }
}