// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>Ein generierter Glyph</para>
    ///     Klasse ExGlyphData. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExGlyphData
    {
        #region Properties

        /// <summary>
        ///     Glyph Name zB Red_E994.png
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Bild
        /// </summary>
        public byte[] ImageData { get; set; }


        /// <summary>
        ///     Link zum Bild
        /// </summary>
        public string ImageUri { get; set; }

        #endregion
    }

    /// <summary>
    ///     <para>Post Daten für die Generierung eines Glyph</para>
    ///     Klasse ExGlyphData. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExGlyphDataPost
    {
        #region Properties

        /// <summary>
        ///     Glyph Font Code zB "\uE994"
        /// </summary>
        public string FontCode { get; set; }

        /// <summary>
        ///     Glyph Font Code zB "E994"
        /// </summary>
        public string FontCodeNumber { get; set; }

        /// <summary>
        ///     Farbe Pin
        /// </summary>
        public Color PinColor { get; set; }

        /// <summary>
        ///     Farbe Glyph
        /// </summary>
        public Color GlyphColor { get; set; }

        /// <summary>
        ///     Farbe Hintergund (Color.Transparent)
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        ///     Größe Quadratisch 32
        /// </summary>
        public int SquaredSize { get; set; }

        /// <summary>
        ///     Kein Glyph - nur Pin in einer Farbe
        /// </summary>
        public bool PinOnly { get; set; }

        #endregion
    }
}