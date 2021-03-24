// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace BaseApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewWhatsAppTutorial
    {
        public ViewWhatsAppTutorial() : this(null)
        {
        }

        public ViewWhatsAppTutorial(object args = null) : base(args)
        {
            InitializeComponent();
        }
    }
}