// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Helper
{
    /// <summary>
    ///     <para>Hilfsfunktonen für Passwörter für lokale User </para>
    ///     Klasse PasswordHelper. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        ///     Aus einem Passwort einen Hash erzeugen
        /// </summary>
        /// <param name="password">Passwort</param>
        /// <returns></returns>
        public static string CumputeHash(string password)
        {
            //var bytes = new UTF8Encoding().GetBytes(password);
            //byte[] hashBytes;
            //using (var algorithm = new HMACSHA512())
            //{
            //    hashBytes = algorithm.ComputeHash(bytes);
            //}
            //return Convert.ToBase64String(hashBytes);

            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                // Get the hashed string.  
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        ///     GUID Passwort erzeugen
        /// </summary>
        /// <param name="maxChars"></param>
        /// <returns></returns>
        public static string GeneratePassword(int maxChars = -1)
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "");
            if (maxChars > 0)
            {
                if (maxChars < guid.Length)
                    return guid.Substring(0, maxChars);
            }

            return guid;
        }
    }
}