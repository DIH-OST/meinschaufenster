// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Database.Tables
{
    /// <summary>
    ///     <para>Tabelle für Produktkategorien</para>
    ///     Klasse TableProductCategory. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Table("ProductCategory")]
    public class TableProductCategory
    {
        #region Properties

        /// <summary>
        ///     DB ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Beschreibung
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///     Zwischentabelle für Geschäft und Produktkategorie
        /// </summary>
        public virtual List<TableStoreCategory> TblStoreCategory { get; set; }

        #endregion
    }
}