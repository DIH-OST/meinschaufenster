// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Enum;

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für Bilder</para>
    ///     Klasse TableImage. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("Image")]
    public class TableImage
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Typ des Bildes
        /// </summary>
        public EnumImageType ImageType { get; set; }

        /// <summary>
        ///     Dateiname
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     Dateidaten
        /// </summary>
        public byte[] File { get; set; }

        /// <summary>
        ///     GEspeihert am
        /// </summary>
        public DateTime SavedAt { get; set; }

        /// <summary>
        ///     Geschäft welches diese Bilder hat.
        /// </summary>
        public virtual TableStore Store { get; set; }

        /// <summary>
        ///     Id des Geschäfts welches diese Bilder hat.
        /// </summary>
        [ForeignKey(nameof(Store))]
        public int StoreId { get; set; }

        #endregion
    }
}