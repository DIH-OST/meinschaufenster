// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Exchange.Model
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ExShopRegistration. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ExShopForgotPassword
    {
        #region Properties

        public string EMail { get; set; }
        public EnumShopForgotPassword Step { get; set; }

        #endregion
    }

    public enum EnumShopForgotPassword
    {
        //LINK VERSENDEN
        Step1 = 1,

        //PASSWORT VERSENDEN
        Step2 = 2
    }
}