// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Database
{
    /// <summary>
    ///     Constants die nicht die nicht an den Client verteilt werden dürfen (Serverkeys)
    ///     -> schützen vor dekomplilierung
    /// </summary>
    public class ServerConstants
    {
        #region Properties

        /// <summary>
        ///     Instance
        /// </summary>
        public static ServerConstants Instance { get; } = new ServerConstants();


        /// <summary>
        ///     GoogleServerKey für Push
        /// </summary>
        public string GoogleServerKey { get; set; } = "AAAA75WuxMo:APA91bFpnONabhnUBekKivqRpFN0PgGQQhStGq34i9qtGFvMKmx4p8dcPi5zXEPwunJcVkECxaVgJJzhNPUFU402BI_IDw1R7HglcURvOGy7Z3mjft7yuZb2CN6ycF_L9c0zQYT0RKa6";

        public string UwpAppPackageName { get; set; } = "BISS.Biss.Mvvm";
        public string UwpAppSid { get; set; } = "ms-app://s-1-15-2-2223498298-3243272845-2137246800-1054884044-3574107808-380004894-111009338";
        public string UwpPushSecret { get; set; } = "LsqqoejiSkdjVVF5n0nGeSjtqcsVZrx3";

        #endregion
    }
}