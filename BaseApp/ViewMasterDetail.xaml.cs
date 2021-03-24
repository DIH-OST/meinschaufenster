// DigitalesSchaufenster (C) 2020 DIH-OST

namespace BaseApp
{
    //https://developer.xamarin.com/guides/xamarin-forms/application-fundamentals/navigation/hierarchical/
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewMasterDetail : MasterDetailPage, IShowMenu
    {
        public ViewMasterDetail()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            //https://developer.xamarin.com/api/type/Xamarin.Forms.MasterBehavior/
            MasterBehavior = MasterBehavior.Popover;
            NavigatorExtensions.BcNavigation(null).Navigator.SetShowMenuPage(this);
            InitializeComponent();
        }

        #region Properties

        public bool MenuGestureEnabled
        {
            get => VmBase.MenuGestureEnabled;
            set => VmBase.MenuGestureEnabled = value;
        }

        #endregion

        /// <summary>
        ///     Soll das Meü ausgeklappt/eingeklappt werden?
        /// </summary>
        /// <param name="show">Anzeigen (true) oder nicht (false)</param>
        /// <returns></returns>
        public bool ShowMenu(bool show)
        {
            IsPresented = show;
            return IsPresented;
        }
    }
}