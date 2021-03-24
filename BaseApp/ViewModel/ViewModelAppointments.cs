// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using BaseApp.ViewModel.UiModel;
using Exchange.Model;
using Exchange.PostRequests;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ViewModelScheduleAppointment. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelAppointments : ProjectViewModelBase
    {
        private readonly List<UiMeeting> _loadedAppointments = new List<UiMeeting>();
        private UiMeeting _selectedAppointment;
        private DateTime _selectedDate;
        private ExShop _shop;

        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelAppointments() : base(ResViewAppointments.PageTitle)
        {
            MinimumDate = DateTime.Now.Date.AddDays(-30);
            SelectedDate = DateTime.Now.Date;
            MaximumDate = DateTime.Now.Date + new TimeSpan(365, 0, 0, 0);
        }

        #region Properties

        public bool AppointmentsAvailable { get; set; }

        public bool InfoPopupOpen { get; set; }

        public DateTime MinimumDate { get; set; }

        public DateTime MaximumDate { get; set; }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                if (value != null)
                    Task.Run(async () => { await LoadAppointments(value); });
            }
        }

        public UiMeeting SelectedAppointment { get; set; }

        public ObservableCollection<UiMeeting> Appointments { get; set; } = new ObservableCollection<UiMeeting>();

        public VmCommand CmdStartWhatsapp { get; set; }

        public VmCommand CmdDelete { get; set; }

        public VmCommand CmdCall { get; set; }

        #endregion

        /// <summary>
        ///     Wird aufgerufen sobald die View initialisiert wurde
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override async Task OnActivated(object args = null)
        {
            await LoadAppointments(DateTime.UtcNow);
        }

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdDelete = new VmCommand("", async param =>
            {
                var response = await MsgBox.Show(ResViewAppointments.MsgBoxDelete, ResViewAppointments.MsgBoxDeleteCaption, VmMessageBoxButton.YesNo);
                if (response == VmMessageBoxResult.No)
                    return;

                if (param is UiMeeting appointment)
                {
                    var r = await Sa.DeleteMeeting(new ExRemoveMeetingRequest {MeetingId = appointment.Meeting.Id, UserId = UserAccountData.UserId});
                    if (r.Ok && r.Result.Result == EnumSaveDataResult.Ok)
                    {
                        _loadedAppointments.Remove(appointment);
                        Appointments.Remove(appointment);

                        if (!_loadedAppointments.Any())
                            AppointmentsAvailable = false;
                    }

                    if (r.Ok && r.Result.Result != EnumSaveDataResult.Ok && !string.IsNullOrWhiteSpace(r.Result.Description))
                    {
                        var error = string.IsNullOrWhiteSpace(r.Result.Caption) ? "Fehler" : r.Result.Caption;
                        await MsgBox.Show(r.Result.Description, error);
                    }
                }
            }, glyph: "\ue912");

            CmdStartWhatsapp = new VmCommand("", async param =>
            {
                if (!(param is UiMeeting appointment))
                {
                    return;
                }

                if (await MsgBox.Show(ResViewMain.MsgBoxOpenWhatsApp, ResViewMain.MsgBoxOpenWhatsAppCaption, VmMessageBoxButton.YesNo) == VmMessageBoxResult.Yes)
                {
                    await Nav.ToViewWithResult("ViewWhatsAppTutorial", appointment.Meeting.Staff.WhatsappContact);
                    //OpenUri(string.Format(ResView.CmdOpenWhatsAppWithMessageUrl, appointment.Meeting.Staff.WhatsappContact, ResView.LblReadyForVideoCall));
                }
            }, glyph: "\ue93D");

            CmdCall = new VmCommand("", async param =>
            {
                if (!(param is UiMeeting appointment))
                    return;

                var shopDetails = await Sa.GetShopInfo(appointment.Meeting.ShopId);

                if (shopDetails.Ok && shopDetails.Result != null)
                {
                    if (!shopDetails.Result.IsOpen)
                    {
                        var response = await MsgBox.Show(ResViewMain.LblOutsideOpeningHours, ResViewMain.LblHint, VmMessageBoxButton.YesNo);
                        if (response != VmMessageBoxResult.Yes)
                            return;
                    }

                    PhoneDialer.Open($"00{shopDetails.Result.PhoneNumber}");
                }
            }, glyph: "\ue948");
        }

        private void FilterAppointments()
        {
            var filtered = new List<UiMeeting>();
            filtered = _loadedAppointments.Where(a => a.Meeting.Start.Date.Equals(SelectedDate.Date)).OrderByDescending(m => m.Meeting.Start).Reverse().ToList();

            Appointments.Clear();

            foreach (var item in filtered)
            {
                Appointments.Add(item);
            }
        }

        private async Task LoadAppointments(DateTime? date = null)
        {
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }
            else
            {
                var formatted = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                date = formatted;
            }

            var response = await Sa.GetMyMeetingsForDate(UserAccountData.UserId, date.Value);

            if (response.Ok && response.Result != null)
            {
                _loadedAppointments.Clear();

                foreach (var item in response.Result)
                {
                    _loadedAppointments.Add(new UiMeeting(item));
                }

                AppointmentsAvailable = _loadedAppointments.Count > 0;

                FilterAppointments();
            }
        }
    }
}