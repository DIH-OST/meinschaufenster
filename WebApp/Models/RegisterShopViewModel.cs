// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp.Models
{
    /// <summary>
    ///     <para>Login ViewModel</para>
    ///     Klasse LoginViewModel. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class RegisterShopViewModel
    {
        #region Properties

        [DisplayName("E-Mail")]
        public string UserName { get; set; }

        #endregion
    }

    /// <summary>
    ///     <para>Login ViewModel</para>
    ///     Klasse LoginViewModel. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class RegisterUserViewModel
    {
        #region Properties

        [DisplayName("Telefonnummer")]
        public string PhoneNumber { get; set; }

        #endregion
    }
}