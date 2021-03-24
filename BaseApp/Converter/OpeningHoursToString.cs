// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;

namespace BaseApp.Converter
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse OpeningHoursToString. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class OpeningHoursToString : IValueConverter
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
            if (!(value is ExOpeningHour opening))
            {
                return string.Empty;
            }

            var uiString = string.Empty;

            switch (opening.Day.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    uiString = "Mo: ";
                    break;
                case DayOfWeek.Tuesday:
                    uiString = "Di: ";
                    break;
                case DayOfWeek.Wednesday:
                    uiString = "Mi: ";
                    break;
                case DayOfWeek.Thursday:
                    uiString = "Do: ";
                    break;
                case DayOfWeek.Friday:
                    uiString = "Fr: ";
                    break;
                case DayOfWeek.Saturday:
                    uiString = "Sa: ";
                    break;
                case DayOfWeek.Sunday:
                    uiString = "So: ";
                    break;
            }

            if (!opening.TimeFrom.HasValue || !opening.TimeTo.HasValue)
            {
                uiString += "Geschlossen";
            }
            else
                uiString += $"{opening.TimeFrom.Value.ToLocalTime().ToString("HH:mm")} - {opening.TimeTo.Value.ToLocalTime().ToString("HH:mm")}";

            return uiString;
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