// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Exchange Model für Device Infos</para>
    ///     Klasse ExBaseInfo. (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExBaseInfo : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        public string Internet { get; set; } = "unavailable";

        public string Bandwidths { get; set; } = "unavailable";

        public string ConnectionTypes { get; set; } = "unavailable";

        public string DeviceLanguage { get; set; } = "unavailable";

        public string AppLanguage { get; set; } = "unavailable";

        public string DeviceHardwareId { get; set; } = "unavailable";

        public string DeviceType { get; set; } = "unavailable";

        public string DeviceIdiom { get; set; }

        public string DevicePlattform { get; set; }

        public string OperatingSystemVersion { get; set; } = "unavailable";

        public string AppCenterId { get; set; } = "unavailable";

        public string RestUrl { get; set; } = "unavailable";

        public string SignalRBaseUrl { get; set; } = "unavailable";

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}