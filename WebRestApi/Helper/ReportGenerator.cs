// DigitalesSchaufenster (C) 2020 DIH-OST

using Database.Tables;
using Exchange;
using Exchange.Model;

namespace WebRestApi.Helper
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ReportGenerator. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class ReportGenerator
    {
        public static ExEmailRegistration GetRegistrationEmail(TableUser user)
        {
            var url = $"{Constants.ServiceClientEndPointWithApiPrefix}UserValidateEMail/{user.Id}/{user.RestPasswort}";
            Logging.Log.LogInfo($"SendCheckEMail: {url}");

            return new ExEmailRegistration
                   {
                       Message = "Um die Registrierung abzuschließen, drücken Sie den folgenden Link:",
                       ApproveLink = url,
                       AlternativeText = $"Alternativ kopieren sie den folgenden Link und rufen Sie ihn einem Browser ihrer Wahl auf: {url}",
                   };
        }

        public static ExEmailNewPassword GetNewPasswordEmail(string newPwd)
        {
            return new ExEmailNewPassword
                   {
                       Message = "Ihr Passwort wurde erfolgreich zurückgesetzt, Sie können sich jetzt mit folgendem Passwort anmelden:",
                       NewPassword = newPwd,
                       AlternativeText = $"Falls Sie die Änderung nicht gestartet haben, kontaktieren Sie uns unter {Constants.SendEMailAs}",
                   };
        }

        public static ExEmailResetPassword GetPasswordResetEmail(TableUser data)
        {
            //Einladung versenden
            var url = $"{Constants.ServiceClientEndPointWithApiPrefix}UserResetPassword/{data.Id}/{data.RestPasswort}";
            Logging.Log.LogInfo($"Send StartResetPassword: {url}");

            return new ExEmailResetPassword
                   {
                       Message = "Um ihr Passwort zurück zu setzen, drücken Sie den folgenden Link:",
                       ApproveLink = url,
                       AlternativeText = $"Alternativ kopieren sie den folgenden Link und rufen Sie ihn einem Browser ihrer Wahl auf: {url}",
                   };
        }
    }
}