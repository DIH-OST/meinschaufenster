// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;

namespace BaseApp.ViewModel.UiModel
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse UiExCategory. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class UiExCategory : INotifyPropertyChanged
    {
        public UiExCategory(ExCategory category)
        {
            Category = category;
        }

        #region Properties

        public ExCategory Category { get; }

        public bool IsSelected { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}