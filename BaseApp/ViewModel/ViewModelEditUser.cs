// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    #region Hilfsklassen

    /// <summary>
    ///     <para>Daten die editiert werden können</para>
    ///     Klasse EditUserData. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class EditUserData : INotifyPropertyChanged, IBissSerialize
    {
        #region Properties

        /// <summary>
        ///     Vorname
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Nachname
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Straße und Hausnummer
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        ///     PLZ
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        ///     Ort
        /// </summary>
        public string City { get; set; }

        #endregion

#pragma warning disable 0067
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }

    #endregion

    /// <summary>
    ///     <para>ViewModelUser</para>
    ///     Klasse ViewModelUser. (C) 2016 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelEditUser : ProjectViewModelBase
    {
        private readonly CheckSaveJsonBehavior _saveCheck = new CheckSaveJsonBehavior();

        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelEditUser() : base(ResViewEditUser.Title, subTitle: ResViewEditUser.Subtitle)
        {
            CheckSaveBehavior = _saveCheck;
        }

        #region Properties

        public bool FromRegistration { get; set; }

        /// <summary>
        ///     Daten zum bearbeiten
        /// </summary>
        public EditUserData Data { get; set; }

        /// <summary>
        ///     Sichern
        /// </summary>
        public VmCommand CmdSave { get; set; }

        /// <summary>
        ///     Überspringen
        /// </summary>
        public VmCommand CmdSkip { get; set; }

        #endregion

        /// <summary>
        ///     Wird aufgerufen sobald die View initialisiert wurde
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override Task OnActivated(object args = null)
        {
            if (UserAccountData == null)
            {
                Logging.Log.LogError("UserAccount null?");
                Nav.ToView("ViewMain");
            }

            if (args is bool fromRegistration)
            {
                FromRegistration = fromRegistration;
            }

            Data = new EditUserData
                   {
                       LastName = UserAccountData.LastName,
                       FirstName = UserAccountData.FirstName,
                   };

            CheckSaveBehavior.SetCompareData(Data.ToJson());
            Data.PropertyChanged += (sender, eventArgs) => { CmdSave.CanExecute(); };
            _saveCheck.CheckSaveComparer += (sender, a) => { a.JsonToCompare = Data.ToJson(); };

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdSave = new VmCommand(ResViewEditUser.CmdSaveName, async () =>
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

                    ContinueNavigation();
                }

                IsBusy = false;
            }, () => CheckSaveBehavior != null && CheckSaveBehavior.Check(), ResViewEditUser.CmdSaveTooltip);

            CmdSkip = new VmCommand(ResViewEditUser.CmdSkipName, async () =>
            {
                CheckSaveBehavior = null;
                ContinueNavigation();
            });
        }

        private async void ContinueNavigation()
        {
            if (FromRegistration)
            {
                Nav.ToView(ViewAfterLogin, ViewArgsAfterLogin);
            }
            else
            {
                await Nav.Back();
            }
        }
    }
}