// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Helper;
using Exchange.PostRequests;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>ViewModel Passwort ändern</para>
    ///     Klasse ViewModelEditPassword. (C) 2016 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelEditPassword : ProjectViewModelBase
    {
        private string _currentPassword = string.Empty;
        private string _password = string.Empty;
        private string _passwordRepeat = string.Empty;
        private bool _showAsPassword;

        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelEditPassword() : base(ResViewEditPassword.Title, subTitle: ResViewEditPassword.Subtitle)
        {
        }

        #region Properties

        /// <summary>
        ///     Passwort
        /// </summary>
        public string CurrentPassword
        {
            get => _currentPassword;
            set
            {
                _currentPassword = value;
                CmdSave.CanExecute();
            }
        }


        /// <summary>
        ///     Passwort
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                CmdSave.CanExecute();
            }
        }

        /// <summary>
        ///     Passwort
        /// </summary>
        public string PasswordRepeat
        {
            get => _passwordRepeat;
            set
            {
                _passwordRepeat = value;
                CmdSave.CanExecute();
            }
        }

        /// <summary>
        ///     Neuen Benutzer sichern
        /// </summary>
        public VmCommand CmdSave { get; set; }

        /// <summary>
        ///     Soll das Passwortfeld verschlüsselt angezeigt werden
        /// </summary>
        public bool ShowAsPassword
        {
            get => _showAsPassword;
            set
            {
                _showAsPassword = value;
                CmdSave.CanExecute();
            }
        }

        #endregion

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdSave = new VmCommand(ResViewEditPassword.CmdSave, async () =>
            {
                IsBusy = true;
                var r = await Sa.UserUpdatePassword(new ExPostUserChangePasswortData
                                                    {
                                                        UserId = UserAccountData.UserId,
                                                        OldPasswordHash = PasswordHelper.CumputeHash(CurrentPassword),
                                                        NewPasswordHash = PasswordHelper.CumputeHash(Password)
                                                    });
                IsBusy = false;


                if (await this.CheckRestStatusResult(r.Status))
                    if (await this.CheckSaveResult(r.Result))
                        if (r.Result.Result == EnumSaveDataResult.Ok)
                            await Nav.Back();
            }, CanCmdSaveExecute);
        }

        private bool CanCmdSaveExecute()
        {
            bool passwordOk = !string.IsNullOrEmpty(Password) &&
                              (ShowAsPassword || Password == PasswordRepeat);

            return !string.IsNullOrEmpty(CurrentPassword) && passwordOk;
        }
    }
}