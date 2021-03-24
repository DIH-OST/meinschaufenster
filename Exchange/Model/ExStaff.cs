// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Mitarbeiter</para>
    ///     Klasse ExStaff. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExStaff : IBissSerialize, INotifyPropertyChanged
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
        ///     Whatsapp Nummer für Kontakt
        /// </summary>
        public string WhatsappContact { get; set; }

        /// <summary>
        ///     Bild
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        ///     Link zum Bild
        /// </summary>
        public string ImageUrl { get; set; }

        #endregion

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
                return Name;

            return base.ToString();
        }

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}