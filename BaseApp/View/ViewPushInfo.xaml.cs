// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace BaseApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewPushInfo
    {
        public ViewPushInfo() : this(null)
        {
        }

        public ViewPushInfo(object args = null) : base(args)
        {
            InitializeComponent();
        }
    }
}