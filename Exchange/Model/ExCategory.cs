// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Biss.Apps.Components.Map.Base;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Kategorie</para>
    ///     Klasse ExCategory. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExCategory : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     DB Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Name für UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Icon für UI
        /// </summary>
        public string Glyph { get; set; }

        /// <summary>
        ///     Image für Pin
        /// </summary>
        public BissPinInfo Pin { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }

    public class ExCategoryItem : ExCategory
    {
        #region Properties

        public bool checkboxAnswer { get; set; }

        #endregion
    }
}