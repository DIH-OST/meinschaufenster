// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp
{
    /// <summary>
    ///     Input Validation Helper
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        ///     überprüft die Telefonnummer
        /// </summary>
        /// <param name="orgTelNr">originale Eingabe</param>
        /// <param name="telNr">formatierte Telefonnummern-Ausgabe</param>
        /// <returns>true, wenn gültig</returns>
        public static bool ProoveValidPhoneNumber(string orgTelNr, out string telNr)
        {
            if (String.IsNullOrEmpty(orgTelNr))
            {
                telNr = null;
                return false;
            }

            var orgTelWithoutWhitespace = RemoveWhitespaces(orgTelNr);

            telNr = orgTelWithoutWhitespace.Replace("+", "");
            telNr = telNr.Replace(" ", "");
            telNr = telNr.Replace("/", "");
            telNr = telNr.Replace("\\", "");

            if (telNr.Length > 4)
            {
                if (telNr[0] == '0')
                {
                    if (telNr[1] == '0')
                    {
                        if (telNr[2] == '4' && telNr[3] == '3')
                        {
                            telNr = telNr.Substring(2);
                        }
                        else
                        {
                            telNr = "43" + telNr.Substring(2);
                        }
                    }
                    else
                    {
                        telNr = "43" + telNr.Substring(1);
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Removes all white spaces in a string.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>The phone number withouts white spaces.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if phone Number is null.
        /// </exception>
        private static string RemoveWhitespaces(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new ArgumentNullException(nameof(phoneNumber), "Phone number must not be null");
            }

            return new string(phoneNumber.ToCharArray().Where(p => !char.IsWhiteSpace(p)).ToArray());
        }
    }
}