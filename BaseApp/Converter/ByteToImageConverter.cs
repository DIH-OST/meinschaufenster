// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace BaseApp.Converter
{
    /// <summary>
    ///     <para>Bytes in eine ImageSource konvertieren</para>
    ///     Klasse ByteToImageConverter. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ByteToImageConverter : IValueConverter
    {
        /// <summary>
        ///     Bild in Imagesource umwandeln
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource retSource = null;
            if (value != null)
            {
                byte[] imageAsBytes = (byte[]) value;
                retSource = ImageSource.FromStream(() => new MemoryStream(imageAsBytes));
            }

            return retSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}