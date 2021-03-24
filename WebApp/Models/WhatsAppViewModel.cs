// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp.Models
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse WhatsAppViewModel. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class WhatsAppViewModel
    {
        #region Properties

        public string PhoneNumber { get; set; }
        public EnumModalStyle Style { get; set; }
        public string Text { get; set; }

        #endregion
    }

    public enum EnumModalStyle
    {
        /// <summary>
        ///     Stil eins mit body
        /// </summary>
        Style1 = 1,

        /// <summary>
        ///     Stil zwei nur mit content
        /// </summary>
        Style2 = 2
    }
}