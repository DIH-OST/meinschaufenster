// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using BaseApp.ViewModel.UiModel;

namespace BaseApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewAppointments
    {
        public ViewAppointments() : this(null)
        {
        }

        public ViewAppointments(object args = null) : base(args)
        {
            InitializeComponent();
        }

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is UiMeeting appointment)
            {
                ViewModel.SelectedAppointment = appointment;
            }

            ViewModel.InfoPopupOpen = true;
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            ViewModel.InfoPopupOpen = false;
        }
    }
}