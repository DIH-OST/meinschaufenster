// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Einzelner Datensatz für Telefonliste</para>
    ///     Klasse ExPhoneList. (C) 2016 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExPhoneList : IComparable, INotifyPropertyChanged, IDbList
    {
        #region Properties

        /// <summary>
        ///     Fullname of employee
        /// </summary>
        //[XmlElement(ElementName = "Mitarbeiter")]
        public string Fullname { get; set; }

        /// <summary>
        ///     FOTEC function
        /// </summary>
        //[XmlElement(ElementName = "Funktion")]
        public string Function { get; set; }

        /// <summary>
        ///     FOTEC Extension
        /// </summary>
        //[XmlElement(ElementName = "Durchwahl")]
        public string Extension { get; set; }

        /// <summary>
        ///     Smartphone number
        /// </summary>
        //[XmlIgnore]
        public string PrivateMobile { get; set; }

        /// <summary>
        ///     DESCRIPTION
        /// </summary>
        //[XmlIgnore]
        public bool ShowPrivateMobile { get; set; }

        /// <summary>
        ///     Smartphone Number for UI only
        /// </summary>
        //[XmlElement(ElementName = "Mobiltelefon")]
        [JsonIgnore]
        public string PrivateMobileUi
        {
            get { return ShowPrivateMobile ? PrivateMobile : String.Empty; }
            set { PrivateMobile = value; }
        }


        /// <summary>
        ///     Datenbank Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Datensatz in Db löschen
        /// </summary>
        public bool DeleteInDb { get; set; }

        /// <summary>
        ///     Datensatz in Db aktualisieren
        /// </summary>
        public bool UpdateInDb { get; set; }

        #endregion

        public int CompareTo(object obj)
        {
            var o = obj as ExPhoneList;
            return Fullname.CompareTo(o.Fullname);
        }

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }


    /// <summary>
    ///     <para>Daten für UI Phonelist</para>
    ///     Klasse ExPhoneListData. (C) 2016 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExPhoneListData : INotifyPropertyChanged, INotifyCollectionChanged
    {
        #region Properties

        /// <summary>
        ///     $DESCRIPTION$
        /// </summary>
        public List<ExPhoneList> PhoneListEntries { get; set; }

        /// <summary>
        ///     DESCRIPTION
        /// </summary>
        public string CompanyPhoneNumber { get; set; }

        /// <summary>
        ///     DESCRIPTION
        /// </summary>
        public DateTime CreationDate => DateTime.Now;

        [JsonIgnore]
        public string CreationDateFormated => CreationDate.ToString("d");

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Occurs when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
#pragma warning restore 0067
    }
}