// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Helper
{
    /// <summary>
    ///     <para>Erweiterungsmethoden</para>
    ///     Klasse Extensions. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        ///     Grad in Radianten umrechnen
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double ToRadians(this double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        /// <summary>
        ///     Radianten inGrad umrechen
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double ToDegrees(this double radians)
        {
            return radians * (180 / Math.PI);
        }
    }
}