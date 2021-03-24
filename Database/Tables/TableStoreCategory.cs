// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Database.Tables
{
    /// <summary>
    ///     <para>Kreuztabelle Store Kategorie</para>
    ///     Klasse TableStoreCategory. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("StoreCategory")]
    public class TableStoreCategory
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Ist Hauptkategorie
        /// </summary>
        public bool IsMainStoreCategory { get; set; }

        /// <summary>
        ///     Shop Id
        /// </summary>
        [ForeignKey(nameof(TblStore))]
        public int TblStoreId { get; set; }

        /// <summary>
        ///     Shop
        /// </summary>
        public virtual TableStore TblStore { get; set; }

        /// <summary>
        ///     Kategorie Id
        /// </summary>
        [ForeignKey(nameof(TblProductCategory))]
        public int TblProductCategoryId { get; set; }

        /// <summary>
        ///     Kategorie
        /// </summary>
        public virtual TableProductCategory TblProductCategory { get; set; }

        #endregion
    }
}