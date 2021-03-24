// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange;
using Exchange.PostRequests;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>ViewModelLogin - Schritt 1 - nur E-Mail</para>
    ///     Klasse ViewModelUserAccount. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelLogin : ProjectViewModelBase
    {
        private int _userId = -1;
        private string _userName;

        /// <summary>
        ///     ViewModelLogin
        /// </summary>
        public ViewModelLogin() : base(ResViewLogin.Title, subTitle: ResViewLogin.SubTitle)
        {
        }

        #region Properties

        /// <summary>
        ///     Aktueller Build Typ
        /// </summary>
        public string BuildType => AppConfigConstants == null || AppConfigConstants.CurrentBuildType == EnumCurrentBuildType.CustomerRelease ? "" : $"Type: {AppConfigConstants.CurrentBuildType}, Mode: {AppConfigConstants.Mode}";

        /// <summary>
        ///     Bild
        /// </summary>
        public byte[] Image => Images.ReadImage(EmbeddedImages.AppIconTransparent_png).Image;

        /// <summary>
        ///     CommandLogin
        /// </summary>
        public VmCommand CmdContinue { get; set; }

        /// <summary>
        ///     Demo Mode (falls verfügbar) - wird in den Constants definiert
        /// </summary>
        public VmCommand CmdDemo { get; set; }

        /// <summary>
        ///     Demo Mode (falls verfügbar) - wird in den Constants definiert
        /// </summary>
        public bool HasDemoMode => Constants.HasDemoUser;

        /// <summary>
        ///     Check E-Mail erneut senden
        /// </summary>
        public VmCommand CmdResendEmailCheck { get; set; }

        /// <summary>
        ///     Ist der Button zum erneuten versenden sichtbar?
        /// </summary>
        public bool CmdResendEmailVisible { get; set; }

        /// <summary>
        ///     Login (EMail) des Benutzers
        /// </summary>
        public string UserPhone
        {
            get => _userName;
            set
            {
                _userName = value;
                CmdContinue.CanExecute();
            }
        }

        #endregion

        /// <summary>
        ///     View wurde instanziert
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override Task OnActivated(object args = null)
        {
            // if (AppConfigConstants.CurrentBuildType == EnumCurrentBuildType.Developer) UserPhone = "biss@fotec.at";

            CManager.InitAllNotInitialized();
            return Task.CompletedTask;
        }

        /// <summary>
        ///     View wurde wieder aktiv unbedingt beim überschreiben auch base. aufrufen!
        /// </summary>
        /// <returns></returns>
        public override Task OnAppearing(IView view)
        {
            ToastClick += VmBase_ToastClick;
            return base.OnAppearing(view);
        }

        /// <summary>
        ///     View wurde inaktiv
        /// </summary>
        /// <returns></returns>
        public override Task OnDisappearing(IView view)
        {
            ToastClick -= VmBase_ToastClick;
            return base.OnDisappearing(view);
        }

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdContinue = new VmCommand(ResView.Command_Continue, async () =>
            {
                IsBusy = true;

                if (UserPhone.TrimStart().StartsWith("+"))
                {
                    UserPhone = "00" + UserPhone.TrimStart();
                }

                var r = await Sa.UserCheck(UserPhone);
                if (await this.CheckRestStatusResult(r.Status))
                {
                    if (r.Result.IsDemoUser)
                    {
                        Toast.Show(ResViewLogin.ToastDemoUser, ResViewLogin.ToastDemoUserHead);
                    }
                    else if (r.Result.WrongNumberFormat)
                    {
                        Toast.Show("Falsches Format für Telefonnummer", "Error");
                    }
                    else if (r.Result.ErrorFromDb)
                    {
                        Toast.Show("Fehler beim Sichern der Daten", "Error");
                    }
                    else if (r.Result.EMailNotChecked)
                    {
                        _userId = r.Result.UserId;
                        await Toast.ShowAsync(new BissNotificationOptions
                                              {
                                                  Description = ResViewLogin.ToastEmailUnconfirmed,
                                                  Title = ResViewLogin.ToastEmailUnconfirmedHead,
                                                  Commands = new List<VmCommand>
                                                             {
                                                                 CmdResendEmailCheck,
                                                             }
                                              });
                        CmdResendEmailVisible = true;
                    }
                    else if (r.Result.UserIsLocked)
                    {
                        await MsgBox.Show(ResViewLogin.MsgBoxUserLocked, ResViewLogin.MsgBoxUserLockedHead);
                    }
                    else
                    {
                        await Nav.ToViewWithResult("ViewPassword", r.Result);
                    }
                }

                IsBusy = false;
            }, CanCmdContinueExecute);

            CmdDemo = new VmCommand("Demo", async () =>
            {
                IsBusy = true;
                var r = await Sa.UserDemoId();
                if (await this.CheckRestStatusResult(r.Status))
                {
                    if (r.Result < 0)
                    {
                        IsBusy = false;
                        await MsgBox.Show(ResViewLogin.MsgBoxDemoError, ResViewLogin.MsgBoxDemorErrorHead, VmMessageBoxButton.Ok, VmMessageBoxImage.Information);
                    }
                    else
                    {
                        var r2 = await Sa.UserAccountData(new ExPostUserPasswortData
                                                          {
                                                              UserId = r.Result ?? -1,
                                                              PasswordHash = ""
                                                          });

                        if (await this.CheckRestStatusResult(r2.Status))
                        {
                            IsBusy = false;
                            UserAccountData = r2.Result.UserAccountData;
                            //Demo User soll nicht eingeloggt bleiben!
                            //UserAppData.Save(r2.Result.UserAccountData);
                            LaunchFirstView();
                        }
                    }
                }

                IsBusy = false;
            }, "This is the Demo Button");
        }

        private void VmBase_ToastClick(object sender, string e)
        {
            if (!string.IsNullOrWhiteSpace(e))
            {
                if (e == CmdDemo.ToolTip)
                {
                    CmdDemo.Execute(null);
                }
                else if (e == CmdResendEmailCheck.ToolTip)
                {
                    CmdResendEmailCheck.Execute(null);
                }
            }
        }

        private bool CanCmdContinueExecute()
        {
            return !string.IsNullOrWhiteSpace(UserPhone);
        }
    }
}