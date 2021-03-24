// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace WebApp.Models
{
    /// <summary>
    ///     <para>Login ViewModel</para>
    ///     Klasse LoginViewModel. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class LoginViewModel
    {
        #region Properties

        [DisplayName("Benutzername")]
        public string UserName { get; set; }

        [DisplayName("Passwort")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        #endregion
    }
}