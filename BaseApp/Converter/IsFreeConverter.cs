// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;
using Exchange.Resources;

namespace BaseApp.Converter
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse IsFreeConverter. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class IsFreeConverter : IValueConverter
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
            if (!(value is ExShop shopData))
                return null;

            if (shopData.IsFree)
                return ResViewMain.LblConsultingAvailable;

            var dueTime = (shopData.NextSlot.ToUniversalTime() - DateTime.Now);

            if (dueTime.Days > 1)
                return $"{ResViewMain.LblDueIn} {dueTime.Days} {ResViewMain.LblDays}";

            if (dueTime.Hours > 1)
                return $"{ResViewMain.LblDueIn} {dueTime.Hours} {ResViewMain.LblHours}";

            if (dueTime.Minutes > 5)
                return $"{ResViewMain.LblDueIn} {dueTime.Minutes} {ResViewMain.LblMinutes}";

            return ResViewMain.LblSoon;
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