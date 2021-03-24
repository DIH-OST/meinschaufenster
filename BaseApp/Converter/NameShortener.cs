// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace BaseApp.Converter
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse NameShortener. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class NameShortener : IValueConverter
    {
        /// <summary>
        ///     Konvertiert ein Objekt für XAML
        /// </summary>
        /// <param name="value">Wert zum konvertieren für das UI</param>
        /// <param name="targetType">Zieltyp des Werts</param>
        /// <param name="parameter">Zusätzlicher Parameter aus XAML</param>
        /// <param name="culture">Aktuelle Kultur</param>
        /// <returns>Konvertierter Wert oder null</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string name))
                return null;

            if (name.Length > 20)
            {
                name = name.Substring(0, 18) + "...";
            }

            return name;
        }

        /// <summary>
        ///     Konvertiert ein Objekt von XAML
        /// </summary>
        /// <param name="value">Wert zum konvertieren für das Datenobjekt</param>
        /// <param name="targetType">Zieltyp des Werts</param>
        /// <param name="parameter">Zusätzlicher Parameter aus XAML</param>
        /// <param name="culture">Aktuelle Kultur</param>
        /// <returns>Konvertierter Wert oder UnsetValue</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}