// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Helper;
using Exchange.Model;
using Exchange.PostRequests;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>ViewModel Template</para>
    ///     Klasse ViewModelUserAccount. (C) 2016 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelPassword : ProjectViewModelBase
    {
        private VmCommand _cmdLogin;
        private string _password;
        private int _userId;

        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelPassword() : base(ResViewPassword.Title)
        {
        }

        #region Properties

        /// <summary>
        ///     Password
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                CmdLogin.CanExecute();
            }
        }

        public VmCommand CmdResendPassword { get; set; }

        public VmCommand CmdLogin
        {
            get => _cmdLogin;
            set
            {
                _cmdLogin = value;
                CmdLogin.CanExecute();
            }
        }


        /// <summary>
        ///     Soll das Passwortfeld verschlüsselt angezeigt werden
        /// </summary>
        public bool ShowAsPassword { get; set; }

        /// <summary>
        ///     neu registrierter User
        /// </summary>
        public bool IsNewUser { get; set; }

        #endregion

        /// <summary>
        ///     Wird aufgerufen sobald die View initialisiert wurde
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override async Task OnActivated(object args = null)
        {
            if (args is ExCheckUser user)
            {
                _userId = user.UserId;
                IsNewUser = user.IsNewUser;

                PageSubTitle = IsNewUser ? ResViewPassword.SubTitleRegistration : ResViewPassword.SubTitle;
            }
            else
            {
                await Nav.Back();
            }
        }


        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdLogin = new VmCommand(ResView.Command_Login, async () =>
            {
                IsBusy = true;
                BusyContent = ResViewPassword.BusyCheckingPassword;

                string hash = PasswordHelper.CumputeHash(Password);

                var r = await Sa.UserAccountData(new ExPostUserPasswortData
                                                 {
                                                     UserId = _userId,
                                                     PasswordHash = hash
                                                 });

                if (await this.CheckRestStatusResult(r.Status))
                {
                    if (r.Result.PasswordWrong)
                    {
                        await MsgBox.Show(ResViewPassword.MsgBoxWrongPW, ResViewPassword.MsgBoxWrongPWHead);
                    }
                    else if (r.Result.IsLocked)
                    {
                        await MsgBox.Show(ResViewPassword.MsgBoxUserLocked, ResViewPassword.MsgBoxUserLockedHead);
                    }
                    else if (r.Result.UserAccountData == null)
                    {
                        await MsgBox.Show(ResViewPassword.MsgBoxDataInvalid, ResViewPassword.MsgBoxDataInvalidHead);
                    }
                    else
                    {
                        UserAccountData = r.Result.UserAccountData;
                        FileExtensions.BcFiles(null).UserAppData.Save(r.Result.UserAccountData);
                        await DeviceInfoUpdate();
                        UpdateMenu();

                        if (IsNewUser)
                        {
                            Nav.ToView("ViewEditUser", true);
                        }
                        else
                        {
                            Nav.ToView(ViewAfterLogin, ViewArgsAfterLogin);
                            ViewArgsAfterLogin = null;
                            //Nav.ToView("ViewMain");
                        }
                    }
                }

                IsBusy = false;
            }, CanCmdLoginExecute);

            CmdResendPassword = new VmCommand(ResView.Command_ResendPassword, async () =>
            {
                IsBusy = true;
                var r = await Sa.UserStartResetPassword(_userId);
                IsBusy = false;
                if (await this.CheckRestStatusResult(r.Status))
                {
                    if (r.Result == true)
                        Toast.Show(ResViewPassword.MsgBoxNewPasswordSent, ResViewPassword.MsgBoxNewPasswordSentHead);
                    else
                        await MsgBox.Show(ResViewPassword.MsgBoxErrorOccurred, ResViewPassword.MsgBoxErrorOccurredHead, VmMessageBoxButton.Ok, VmMessageBoxImage.Error);
                }
            });
        }

        private bool CanCmdLoginExecute()
        {
            //!String.IsNullOrEmpty(Password) && Regex.IsMatch(Password, ".+@.+\\..+");
            return !string.IsNullOrEmpty(Password);
        }
    }
}