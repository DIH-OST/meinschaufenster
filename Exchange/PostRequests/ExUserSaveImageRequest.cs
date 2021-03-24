// DigitalesSchaufenster (C) 2020 DIH-OST

namespace Exchange.PostRequests
{
    /// <summary>
    ///     <para>Save Image Request</para>
    ///     Klasse ExUserSaveImageRequest. (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExUserSaveImageRequest : IBissSerialize, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     User
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Neues Profilbild
        /// </summary>
        public List<byte> Image { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}