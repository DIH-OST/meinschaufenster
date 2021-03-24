// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace BaseApp.Converter
{
    /// <summary>
    ///     <para>Einen Boolean in einen Wert umwandeln</para>
    ///     Klasse InvertedBooleanConverter. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Preserve(AllMembers = true)]
    public class BooleanValueConverter : IValueConverter
    {
        #region Properties

        public object TrueValue { get; set; }

        public object FalseValue { get; set; }

        #endregion

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            return (bool) value ? TrueValue : FalseValue;
        }


        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}