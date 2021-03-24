// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace BaseApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewUser
    {
        public ViewUser(object args) : base(args)
        {
            InitializeComponent();
        }

        public ViewUser()
        {
            InitializeComponent();
        }
    }
}