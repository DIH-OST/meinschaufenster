// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle dynamische Einstellungen Key/Value</para>
    ///     Klasse TableAbsence. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("DynamicSetting")]
    public class TableDynamicSetting
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        #endregion
    }
}