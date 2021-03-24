// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace BaseApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewEditUser
    {
        public ViewEditUser()
        {
            InitializeComponent();
        }

        public ViewEditUser(object args) : base(args)
        {
            InitializeComponent();

            if (args is bool fromRegistration && fromRegistration)
            {
                ControlTemplate = (ControlTemplate) new StyPageTemplates()["EmptyPageTemplate"];
            }
        }
    }
}