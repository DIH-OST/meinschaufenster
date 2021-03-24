// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>ViewModelUser</para>
    ///     Klasse ViewModelUser. (C) 2016 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelUser : ProjectViewModelBase
    {
        private readonly CheckSaveJsonBehavior _saveCheck = new CheckSaveJsonBehavior();
        private bool _dataLoaded;

        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelUser() : base(ResViewUser.Title, subTitle: ResViewUser.Subtitle)
        {
            CheckSaveBehavior = _saveCheck;
        }

        #region Properties

        /// <summary>
        /// </summary>
        public bool HasName => !string.IsNullOrWhiteSpace(FullName);

        /// <summary>
        ///     Logout aktuellen User
        /// </summary>
        public VmCommand CmdLogout { get; set; }

        /// <summary>
        ///     Logout aktuellen User
        /// </summary>
        public VmCommand CmdChangeUserData { get; set; }

        /// <summary>
        ///     Logout aktuellen User
        /// </summary>
        public VmCommand CmdChangePassword { get; set; }

        /// <summary>
        ///     UserAccount löschen
        /// </summary>
        public VmCommand CmdDeleteAccount { get; set; }

        /// <summary>
        ///     Daten zum bearbeiten
        /// </summary>
        public EditUserData Data { get; set; }

        /// <summary>
        ///     Bei Demo Mode sind einige Button nicht sichtbar
        /// </summary>
        public bool CmdVisible => !UserAccountData?.IsDemoUser ?? true;

        /// <summary>
        ///     Name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        ///     Telefonnummer
        /// </summary>
        public string PhoneNumber { get; set; }

        #endregion

        /// <summary>
        ///     Wird aufgerufen sobald die View initialisiert wurde
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override async Task OnActivated(object args = null)
        {
            if (UserAccountData == null)
            {
                ViewAfterLogin = "ViewUser";
                ViewArgsAfterLogin = args;

                await Task.Run(async () =>
                {
                    var elem = new AsyncAutoResetEvent();
                    RunOnMainThread(async () =>
                    {
                        await Nav.ToViewWithResult("ViewLogin");

                        elem.Set();
                    });

                    await elem.WaitOne();
                });
            }

            if (UserAccountData == null)
            {
                Logging.Log.LogError("UserAccount noch immer null?");
                Nav.ToView("ViewMain");
            }
            else
            {
                _dataLoaded = true;
                CmdChangeUserData.CanExecute();
                CmdChangePassword.CanExecute();
                UpdateData();

                CheckSaveBehavior.SetCompareData(Data.ToJson());
                Data.PropertyChanged += (sender, eventArgs) => { CmdChangeUserData.CanExecute(); };
                _saveCheck.CheckSaveComparer += (sender, a) => { a.JsonToCompare = Data.ToJson(); };
            }
        }

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdChangeUserData = new VmCommand(ResViewUser.CmdChangeUserData, async () =>
            {
                IsBusy = true;

                var d = BissClone.Clone(UserAccountData);
                d.FirstName = Data.FirstName;
                d.LastName = Data.LastName;
                d.City = Data.City;
                d.PostalCode = Data.PostalCode;
                d.Street = Data.Street;

                var r = await Sa.UserUpdate(d);
                if (await this.CheckRestStatusResult(r.Status)
                    && await this.CheckSaveResult(r.Result))
                {
                    CheckSaveBehavior = null;
                    UserAccountData = d;
                    FileExtensions.BcFiles(null).UserAppData.Save(d);
                    ViewResult = Data;
                }

                IsBusy = false;
                UpdateData();
            }, () => CheckSaveBehavior != null && CheckSaveBehavior.Check());

            CmdChangePassword = new VmCommand(ResViewUser.CmdChangePassword, async () => { await Nav.ToViewWithResult("ViewEditPassword"); }, () => _dataLoaded);

            CmdLogout = new VmCommand(ResViewUser.CmdLogout, async () =>
            {
                FileExtensions.BcFiles(null).UserAppData.Delete();
                UserAccountData = null;
                await DeviceInfoDelete();
                if (Constants.SupportLogin)
                    AppCenter.UpdateCurrentUser("", "");
                UpdateMenu();
                Nav.ToView("ViewMain");
            });

            CmdDeleteAccount = new VmCommand(ResViewUser.CmdDeleteAccount, async () =>
            {
                var res = await MsgBox.Show(ResViewUser.MsgBoxDeleteAccount, ResViewUser.MsgBoxDeleteAccountCaption, VmMessageBoxButton.YesNo);

                if (res == VmMessageBoxResult.Yes)
                {
                    IsBusy = true;

                    var saRes = await Sa.DeleteUser(UserAccountData.UserId);

                    if (await this.CheckRestStatusResult(saRes.Status))
                    {
                        CheckSaveBehavior = null;
                        CmdLogout.Execute(null);
                    }

                    IsBusy = false;
                }
            });
        }

        private void UpdateData()
        {
            Data = new EditUserData
                   {
                       City = UserAccountData.City,
                       FirstName = UserAccountData.FirstName,
                       LastName = UserAccountData.LastName,
                       PostalCode = UserAccountData.PostalCode,
                       Street = UserAccountData.Street
                   };
        }
    }
}