// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace BaseApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewSettings
    {
        public ViewSettings()
        {
            InitializeComponent();
        }


        public ViewSettings(object args) : base(args)
        {
            InitializeComponent();
        }
    }
}