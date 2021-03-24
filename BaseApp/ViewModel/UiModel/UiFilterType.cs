// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Enum;

namespace BaseApp.ViewModel.UiModel
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse UiFilterType. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class UiFilterType : INotifyPropertyChanged
    {
        public UiFilterType(EnumFilterType filterType)
        {
            Type = filterType;
            var fieldInfo = Type.GetType().GetField(Type.ToString());
            var descriptionAttributes = (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            Name = descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : Type.ToString();
        }

        #region Properties

        public EnumFilterType Type { get; }

        public string Name { get; }

        public bool IsSelected { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}