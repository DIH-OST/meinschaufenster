// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Biss.Apps.Components.Base;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <inheritdoc />
    /// <summary>
    ///     <para>ViewModelMenu</para>
    ///     Klasse ViewModelUserAccount. (C) 2016 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelMenu : ProjectViewModelBase
    {
        private bool _dontNavigate;
        private VmCommand _selectedVmCommand;

        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelMenu() : base(ResViewMenu.Title)
        {
        }

        #region Properties

        /// <summary>
        ///     Soll das Menü gerade angezeigt werden (für WPF)
        /// </summary>
        public bool IsMenuVisible { get; set; }

        /// <summary>
        ///     Bild
        /// </summary>
        public byte[] Image => Images.ReadImage(EmbeddedImages.AppIconTransparent_png).Image;

        /// <summary>
        ///     Alle Menüeinträge für seitliches Menü
        /// </summary>

        public ObservableCollection<VmCommand> CmdAllMenuCommands { get; set; } = new ObservableCollection<VmCommand>();

        /// <summary>
        ///     SelectedVmCommand
        /// </summary>
        public VmCommand SelectedVmCommand
        {
            get => _selectedVmCommand;
            set
            {
                if (_selectedVmCommand == value) return;

                _selectedVmCommand = value;
                if (_selectedVmCommand != null
                    && _selectedVmCommand.CanExecute()
                    && !_dontNavigate)
                    _selectedVmCommand.Execute(null);
            }
        }

        /// <summary>
        ///     Home Button
        /// </summary>
        public VmCommand CmdHome { get; set; }

        public VmCommand CmdUser { get; set; }

        public VmCommand CmdAppointments { get; set; }

        public VmCommand CmdLogin { get; set; }

        public VmCommand CmdAppInformation { get; set; }

        #endregion

        public async void SubToNotifications()
        {
            UnSubNotifications();

            if (this.BcPush().State != EnumComponentState.Ok || !this.BcPush().CanReceivePush)
            {
                await this.BcPush().Initialize();
            }

            if (this.BcPush().State == EnumComponentState.Ok && this.BcPush().CanReceivePush)
            {
                this.BcPush().PushNotification += OnPushNotification;
            }

            if (DeviceInfo.Plattform == EnumPlattform.XamarinIos)
            {
                if (this.BcSignalR().State != ConnectionState.Open)
                    this.BcSignalR().ReConnect();
                SignalR.NewData += OnSignalRData;
            }

            if (SignalR != null)
            {
                if (!SignalR.IsConnected)
                    SignalR.ReConnect();

                SignalR.NewData += OnSignalRData;
            }

            VmBase.PushReceived += OnPushReceived;
        }

        public void UnSubNotifications()
        {
            if (this.BcPush() != null)
            {
                this.BcPush().PushNotification -= OnPushNotification;
            }

            if (SignalR != null)
            {
                SignalR.NewData -= OnSignalRData;
                SignalR.DisConnect();
            }

            VmBase.PushReceived -= OnPushReceived;
        }

        public void SetInitialVmCommand(VmCommand command)
        {
            if (SelectedVmCommand != null)
                return;

            _dontNavigate = true;
            SelectedVmCommand = command;
            _dontNavigate = false;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdHome = new VmCommand(ResViewMenu.CmdHome, () => { Nav.ToView("ViewMain", showMenu: true); }, ResViewMenu.CmdHomeToolTip, "\uE94E");
            CmdUser = new VmCommand(ResViewMenu.CmdUser, () => { Nav.ToView("ViewUser", showMenu: true); }, ResViewMenu.CmdUserToolTip, "\uE93B");
            CmdAppointments = new VmCommand("Termine", () => { Nav.ToView("ViewAppointments"); }, glyph: "\ue915");
            CmdLogin = new VmCommand("Anmelden", () => { Nav.ToViewWithResult("ViewLogin"); }, glyph: "\ue918");
            CmdAppInformation = new VmCommand("Informationen", () => { Nav.ToView("ViewAppInformation"); }, glyph: "\ue92f");

            CmdAllMenuCommands.Add(CmdHome);

            if (UserAccountData != null)
            {
                CmdAllMenuCommands.Add(CmdUser);
                CmdAllMenuCommands.Add(CmdAppointments);
            }
            else
            {
                CmdAllMenuCommands.Add(CmdLogin);
            }

            CmdAllMenuCommands.Add(CmdAppInformation);
        }

        private void OnPushReceived(object sender, IDictionary<string, string> e)
        {
            Logging.Log.LogInfo("VMMain: OnPushReceived: " + string.Join("; ", e.Select(x => x.Key + ": " + e.Values)));
            PushReceived(e);
        }

        /// <summary>
        ///     Methode von Ereignis für PushNotification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventData"></param>
        private void OnPushNotification(object sender, PushNotificationEventArgs eventData)
        {
            Logging.Log.LogInfo($"PUSH Notifizierung empfangen:\r\n{eventData}");

            RunOnDispatcher(async () =>
            {
                // Muss auf Dispatcher laufen, damit man den Toast bekommt...

                if (Toast.State != EnumComponentState.Ok)
                {
                    await Toast.Initialize();
                }

                await Toast.ShowAsync(eventData.Message, eventData.Title);
            });
        }

        private void OnSignalRData(object sender, NotificationDataEventArgs e)
        {
            var dataString = e?.Data?.CustomData != null ? ("\r\n" + string.Join(",", e?.Data?.CustomData?.Select(x => $"{x.Key} : {x.Value}"))) : string.Empty;
            Logging.Log.LogInfo($"SignalR Daten empfangen:\r\n{e?.Data}{dataString}");

            RunOnDispatcher(async () =>
            {
                // Muss auf Dispatcher laufen, damit man den Toast bekommt...

                if (Toast.State != EnumComponentState.Ok)
                {
                    await Toast.Initialize();
                }

                await Toast.ShowAsync(e.Data.Body, e.Data.Title);
            });
        }
    }
}