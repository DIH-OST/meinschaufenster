// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace BaseApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewAppInformation
    {
        public ViewAppInformation() : this(null)
        {
        }

        public ViewAppInformation(object args = null) : base(args)
        {
            InitializeComponent();
        }
    }
}