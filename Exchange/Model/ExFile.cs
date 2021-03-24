// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    public class ExFile
    {
        #region Properties

        /// <summary>
        ///     Filename
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Daten
        /// </summary>
        public byte[] Data { get; set; }


        /// <summary>
        ///     DateTyp
        /// </summary>
        public EnumFileType Type { get; set; }

        #endregion
    }

    public enum EnumFileType
    {
        Pdf = 0
    }
}