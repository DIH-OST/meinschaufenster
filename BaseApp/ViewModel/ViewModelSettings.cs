// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Model;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>Verfügbare Sprachen für die Auswahl</para>
    ///     Klasse LanguageSetting. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class LanguageSetting
    {
        #region Properties

        /// <summary>
        ///     2 Buchstaben ISO Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Name der Sprache für Auswahl
        /// </summary>
        public string Name { get; set; }

        #endregion
    }


    /// <summary>
    ///     <para>ViewModel Template</para>
    ///     Klasse ViewModelUserAccount. (C) 2016 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelSettings : ProjectViewModelBase
    {
        private readonly CheckSaveJsonBehavior _saveCheck = new CheckSaveJsonBehavior();
        private LanguageSetting _selectedLanguage;

        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelSettings() : base(ResViewSettings.Title, null, ResViewSettings.SubTitle)
        {
            CheckSaveBehavior = _saveCheck;

            var tmp = new ObservableCollection<LanguageSetting>();
            foreach (var l in Language.SupportedLanguages)
            {
                var ci = new CultureInfo(l);
                tmp.Add(new LanguageSetting
                        {
                            Name = ci.DisplayName,
                            Code = l
                        });
            }

            Data = BissClone.Clone(UserAccountData);
            Languages = tmp;
        }

        #region Properties

        /// <summary>
        ///     Daten zum bearbeiten
        /// </summary>
        public ExUserAccountData Data { get; set; }

        /// <summary>
        ///     Verfügbare Sprachen
        /// </summary>
        public ObservableCollection<LanguageSetting> Languages { get; set; }

        /// <summary>
        ///     Ausgewählte Sprache
        /// </summary>
        public LanguageSetting SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                Data.DefaultUserLanguage = _selectedLanguage.Code;
                CmdSave.CanExecute();
            }
        }

        /// <summary>
        ///     CommandLogin
        /// </summary>
        public VmCommand CmdSave { get; set; }

        public VmCommand CmdPushSettingOn { get; set; }
        public VmCommand CmdPushSettingOff { get; set; }
        public VmCommand CmdPushSettingClear { get; set; }

        #endregion

        /// <summary>
        ///     Wird aufgerufen sobald die View initialisiert wurde
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override Task OnActivated(object args = null)
        {
            if (Languages.Any(l => l.Code == UserAccountData.DefaultUserLanguage))
                SelectedLanguage = Languages.First(l => l.Code == UserAccountData.DefaultUserLanguage);
            else
                SelectedLanguage = Languages.First(l => l.Code == "de");


            CheckSaveBehavior.SetCompareData(UserAccountData.ToJson());
            Data.PropertyChanged += (sender, eventArgs) => { CmdSave.CanExecute(); };
            _saveCheck.CheckSaveComparer += (sender, a) => { a.JsonToCompare = Data.ToJson(); };


            //if (args != null)
            //{
            //    UpdateMenu();
            //}

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdSave = new VmCommand(ResViewSettings.CmdSaveTitle, async () =>
            {
                var r = await Sa.UserUpdate(Data);
                if (await this.CheckRestStatusResult(r.Status))
                    if (await this.CheckSaveResult(r.Result))
                    {
                        CheckSaveBehavior = null;
                        UserAccountData = Data;
                        FileExtensions.BcFiles(null).UserAppData.Save(Data);

                        var msgBoxResult = await MsgBox.Show(ResViewSettings.MsgBoxAppClose, "App schließen", VmMessageBoxButton.YesNo, VmMessageBoxImage.Question);
                        if (msgBoxResult == VmMessageBoxResult.Yes)
                            QuitApp();
                        else
                            await Nav.Back();
                    }
            }, CanExecuteSave, ResViewSettings.CmdSaveToolTip);


            CmdPushSettingOn = new VmCommand("an", () => { AppCenter.UpdateProperties("Test", true); });

            CmdPushSettingOff = new VmCommand("aus", () => { AppCenter.UpdateProperties("Test", false); });

            CmdPushSettingClear = new VmCommand("löschen", () => { AppCenter.UpdateProperties("Test", null); });
        }

        /// <summary>
        ///     Validierung ob CommandLogin gedrückt werden kann
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteSave()
        {
            return CheckSaveBehavior != null && CheckSaveBehavior.Check();
        }
    }
}