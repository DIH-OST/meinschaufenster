// DigitalesSchaufenster (C) 2020 DIH-OST

using BaseApp.ViewModel.UiModel;

namespace BaseApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewCreateAppointment
    {
        public ViewCreateAppointment() : this(null)
        {
        }

        public ViewCreateAppointment(object args = null) : base(args)
        {
            InitializeComponent();
        }

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is UiMeeting appointment)
                ViewModel.NavigateToAppointmentDetails(appointment);
        }
    }
}