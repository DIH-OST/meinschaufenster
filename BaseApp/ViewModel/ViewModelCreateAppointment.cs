// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using BaseApp.ViewModel.UiModel;
using Exchange.Model;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ViewModelScheduleAppointment. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelCreateAppointment : ProjectViewModelBase
    {
        private readonly List<UiMeeting> _loadedAppointments = new List<UiMeeting>();
        private readonly List<ExStaff> _loadedStaff = new List<ExStaff>();
        private DateTime _selectedDate;
        private ExStaff _selectedStaff;
        private ExShop _shop;

        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelCreateAppointment() : base(ResViewCreateAppointment.PageTitle)
        {
            MinimumDate = DateTime.Today;
            SelectedDate = DateTime.Today;
            MaximumDate = DateTime.Today + new TimeSpan(365, 0, 0, 0);
        }

        #region Properties

        public DateTime MinimumDate { get; set; }

        public DateTime MaximumDate { get; set; }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                if (value != null)
                    LoadAppointments();
            }
        }


        public ObservableCollection<UiMeeting> Appointments { get; set; } = new ObservableCollection<UiMeeting>();

        public string ShopName { get; set; }

        public ObservableCollection<ExStaff> Staff { get; set; } = new ObservableCollection<ExStaff>();

        public ExStaff SelectedStaff
        {
            get => _selectedStaff;
            set
            {
                _selectedStaff = value;
                FilterAppointments();
            }
        }

        [DependsOn(nameof(Staff))]
        public bool StaffAvailable => Staff.Count > 2;

        public bool AppointmentsAvailable { get; set; }

        public VmCommand CmdBack { get; set; }

        #endregion

        /// <summary>
        ///     Wird aufgerufen sobald die View initialisiert wurde
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override async Task OnActivated(object args = null)
        {
            if (args is ExShop shop && !string.IsNullOrWhiteSpace(shop.Name) && shop.FreeSlots != null)
            {
                _shop = shop;
                ShopName = _shop.Name;
                _loadedStaff.Add(new ExStaff {Id = -1, Name = "Alle"});

                foreach (var appointment in _shop.FreeSlots.OrderByDescending(a => a.Start).Reverse())
                {
                    if (!_loadedStaff.Any(s => s.Id == appointment.Staff.Id))
                    {
                        _loadedStaff.Add(appointment.Staff);
                    }

                    _loadedAppointments.Add(new UiMeeting(appointment));

                    Appointments = new ObservableCollection<UiMeeting>(_loadedAppointments);
                    Staff = new ObservableCollection<ExStaff>(_loadedStaff);
                }

                _selectedStaff = _loadedStaff[0];
                SelectedDate = DateTime.Now;
                FilterAppointments();
            }
            else
                await Nav.Back();
        }

        public async Task NavigateToAppointmentDetails(UiMeeting appointment)
        {
            if (UserAccountData != null)
            {
                await Nav.ToViewWithResult("ViewAppointmentInfo", new object[] {appointment, _shop});
            }
            else
            {
                var userResponse = await MsgBox.Show(ResViewCreateAppointment.MsgBoxNotLoggedIn, ResViewCreateAppointment.MsgBoxNotLoggedInCaption,
                    ResViewCreateAppointment.MsgBoxNotLoggedInLoginButton, VmMessageBoxResult.Yes,
                    ResViewCreateAppointment.MsgBoxNotLoggedInNoButton, VmMessageBoxResult.No
                );

                if (userResponse == VmMessageBoxResult.Yes)
                {
                    ViewAfterLogin = "ViewCreateAppointment";
                    ViewArgsAfterLogin = _shop;

                    await Nav.ToViewWithResult("ViewLogin");
                }
                else
                {
                    Nav.ToView("ViewMain");
                }
            }
        }

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdBack = new VmCommand("", () => { Nav.Back(); }, glyph: "\uE90F");
        }

        private async Task LoadAppointments()
        {
            if (_shop == null)
                return;

            var date = SelectedDate;

            if (date == null)
            {
                date = DateTime.Now;
            }

            var result = await Sa.GetMeetingsForDate(_shop.Id, date);

            if (result.Ok && result.Result != null)
            {
                var appointments = result.Result;
                _loadedAppointments.Clear();
                _loadedStaff.Clear();
                _loadedStaff.Add(new ExStaff {Id = -1, Name = "Alle"});

                foreach (var appointment in appointments.OrderByDescending(a => a.Start).Reverse())
                {
                    if (!_loadedStaff.Any(s => s.Id == appointment.Staff.Id))
                    {
                        _loadedStaff.Add(appointment.Staff);
                    }

                    _loadedAppointments.Add(new UiMeeting(appointment));
                }

                _selectedStaff = _loadedStaff[0];
            }

            FilterAppointments();
        }

        private void FilterAppointments()
        {
            var filtered = new List<UiMeeting>();

            if (SelectedStaff != null && SelectedStaff.Id >= 0)
            {
                filtered = _loadedAppointments.Where(a => a.Meeting.Start.Date.Equals(SelectedDate.Date) && a.Meeting.Staff.Id == SelectedStaff.Id).OrderByDescending(m => m.Meeting.Start).Reverse().ToList();
            }
            else
            {
                filtered = _loadedAppointments.Where(a => a.Meeting.Start.Date.Equals(SelectedDate.Date)).OrderByDescending(m => m.Meeting.Start).Reverse().ToList();
            }

            Appointments.Clear();

            foreach (var item in filtered)
            {
                Appointments.Add(item);
            }

            AppointmentsAvailable = Appointments.Count > 0;

            Staff = new ObservableCollection<ExStaff>(_loadedStaff);
        }
    }
}