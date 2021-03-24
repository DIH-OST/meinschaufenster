// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Erweiterte Shop Infos</para>
    ///     Klasse ExShop. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExShop : ExShopShort
    {
        #region Properties

        /// <summary>
        ///     gerade verfügbar
        /// </summary>
        public bool IsFree { get; set; }

        /// <summary>
        ///     zusätzlicher Name für diesen Standort
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        ///     frei bzw besetzt bis
        /// </summary>
        public DateTime NextSlot { get; set; }

        /// <summary>
        ///     Telefonnummer
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     Whatsapp Kontakt - null wenn keiner vorhanden
        /// </summary>
        public string WhatsappNumber { get; set; }

        /// <summary>
        ///     Link zur Website
        /// </summary>
        public string WebLink { get; set; }

        /// <summary>
        ///     Postleitzahl
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        ///     Ort
        /// </summary>
        public string City { get; set; }

        /// <summary>
        ///     Adresse
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     Land
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        ///     Bundesland
        /// </summary>
        public string FederalState { get; set; }

        /// <summary>
        ///     Öffnungszeiten
        /// </summary>
        public List<ExOpeningHour> OpeningHours { get; set; }

        /// <summary>
        ///     Mitarbeiter
        /// </summary>
        public List<ExStaff> Employees { get; set; }

        /// <summary>
        ///     Verfügbare Timeslots
        /// </summary>
        public List<ExMeeting> FreeSlots { get; set; }

        /// <summary>
        ///     Url zum Bild.
        /// </summary>
        public string ImageUrl { get; set; }

        public string Description { get; set; } = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";

        #endregion
    }
}