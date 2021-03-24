// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;
using WebRestApi.Services;

namespace WebRestApi.Helper
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse HtmlGenerator. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class HtmlGenerator
    {
        public ViewRender ViewRenderer;

        public HtmlGenerator(ViewRender viewRenderer)
        {
            ViewRenderer = viewRenderer;
        }

        /// <summary>
        ///     Html für EMail Registrierung generieren
        /// </summary>
        /// <param name="request">UserInfos</param>
        /// <returns></returns>
        public string GetMessageOnlyEmail(ExEMailMessageOnly request)
        {
            var htmlEMailOrder = ViewRenderer.Render("EMail/EMailMessageOnly", request);
            return htmlEMailOrder;
        }


        /// <summary>
        ///     Html für EMail Registrierung generieren
        /// </summary>
        /// <param name="request">UserInfos</param>
        /// <returns></returns>
        public string GetRegistrationEmail(ExEmailRegistration request)
        {
            var htmlEMailOrder = ViewRenderer.Render("EMail/EMailRegistration", request);
            return htmlEMailOrder;
        }

        /// <summary>
        ///     HTML Meldung wenn ein Kunden den Termin bei einem Geschäft löscht
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetStornoAppointmentShopEmail(ExEMailStornoAppointmentShop request)
        {
            var htmlEMailOrder = ViewRenderer.Render("EMail/EMailStornoAppointmentShop", request);
            return htmlEMailOrder;
        }


        /// <summary>
        ///     Html für EMail neues Passwort generieren
        /// </summary>
        /// <param name="request">UserInfos</param>
        /// <returns></returns>
        public string GetNewPasswordEmail(ExEmailNewPassword request)
        {
            var htmlEMailOrder = ViewRenderer.Render("EMail/EMailNewPassword", request);
            return htmlEMailOrder;
        }

        /// <summary>
        ///     Html für EMail Passwort reset generieren
        /// </summary>
        /// <param name="request">UserInfos</param>
        /// <returns></returns>
        public string GetPasswordResetEmail(ExEmailResetPassword request)
        {
            var htmlEMailOrder = ViewRenderer.Render("EMail/EMailResetPassword", request);
            return htmlEMailOrder;
        }
    }
}