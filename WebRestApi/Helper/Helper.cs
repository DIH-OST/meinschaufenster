// DigitalesSchaufenster (C) 2020 DIH-OST

namespace WebRestApi.Helper
{
    /// <summary>
    ///     <para>Ilfsfunktionen</para>
    ///     Klasse Helper. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class Common
    {
        /// <summary>
        ///     XML Kommentar-Datei für Swagger
        /// </summary>
        /// <returns></returns>
        public static string GetCommonFile(string fileName)
        {
            var app = PlatformServices.Default.Application;
            return Path.Combine(app.ApplicationBasePath, "Common", fileName);
        }

        /// <summary>
        ///     SMS Schicken
        /// </summary>
        /// <param name="to"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string SendSMS(string to, string msg)
        {
            const string accountSid = "[RELEASEONLY]";
            const string authToken = "[RELEASEONLY]";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                new PhoneNumber("00" + to),
                from: new PhoneNumber("[RELEASEONLY]"),
                body: msg
            );

            return message?.Sid;
        }
    }
}