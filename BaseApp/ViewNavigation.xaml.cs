// DigitalesSchaufenster (C) 2020 DIH-OST

namespace BaseApp
{
    /// <summary>
    ///     <para>Xamarin Master/Detail Initialisierung</para>
    ///     Klasse ViewNavigation. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewNavigation : NavigationPage
    {
        public ViewNavigation()
        {
            InitializeComponent();
        }

        public ViewNavigation(Page root) : base(root)
        {
            InitializeComponent();
        }
    }
}