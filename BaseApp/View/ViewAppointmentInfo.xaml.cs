// DigitalesSchaufenster (C) 2020 DIH-OST

namespace BaseApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewAppointmentInfo
    {
        public ViewAppointmentInfo() : this(null)
        {
        }

        public ViewAppointmentInfo(object args = null) : base(args)
        {
            InitializeComponent();
        }
    }
}