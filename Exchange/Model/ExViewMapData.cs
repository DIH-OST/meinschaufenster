// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Biss.Apps.Components.Map.Base;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Position eines Messgerätes</para>
    ///     Klasse ExViewPositionData. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExViewMapData : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     Id
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        ///     Adresse und Zeit der Position
        /// </summary>
        [JsonIgnore]
        [DependsOn(nameof(Address), nameof(DateTimeUtc))]
        public string AddressAndTime => $"{Address}\r\nam {DateTimeUtc.ToLocalTime().ToString("d")} um {DateTimeUtc.ToLocalTime().ToString("t")} Uhr";

        /// <summary>
        ///     Adresse und Zeit der Position
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     Zeitpunkt der Position
        /// </summary>
        public DateTime DateTimeUtc { get; set; }

        /// <summary>
        ///     Breitengrad
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        ///     Längengrad
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        ///     Genauigkeit (Abweichung: Radius in Meter)
        /// </summary>
        public double Accuracy { get; set; }

        /// <summary>
        ///     Breitengrad Zone
        /// </summary>
        public double ZoneLatitude { get; set; }

        /// <summary>
        ///     Längengrad Zone
        /// </summary>
        public double ZoneLongitude { get; set; }

        /// <summary>
        ///     Radius in Meter
        /// </summary>
        public double ZoneRadius { get; set; }

        /// <summary>
        ///     Position
        /// </summary>
        [JsonIgnore]
        [DependsOn(nameof(ZoneLatitude), nameof(ZoneLongitude), nameof(AddressAndTime))]
        public BissPosition ZonePosition => PositionOk ? new BissPosition(ZoneLatitude, ZoneLongitude) : null;

        /// <summary>
        ///     Position des Pin
        /// </summary>
        [JsonIgnore]
        [DependsOn(nameof(Latitude), nameof(Longitude))]
        public BissPosition PinPosition => new BissPosition(Latitude, Longitude);


        /// <summary>
        ///     Aktiv?
        /// </summary>
        public bool ZoneIsEnable { get; set; }

        /// <summary>
        ///     Ist die angegebene Positon vorhanden
        /// </summary>
        [JsonIgnore]
        [DependsOn(nameof(AddressAndTime))]
        public bool PositionOk => !string.IsNullOrEmpty(Address);

        /// <summary>
        ///     Gerätename für Pin
        /// </summary>
        public string DeviceName { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}