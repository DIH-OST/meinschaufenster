// DigitalesSchaufenster (C) 2020 DIH-OST

namespace BaseApp
{
    /// <summary>
    ///     <para>Hauptmenü für App</para>
    ///     Klasse ViewMaster. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewMaster : ContentPage, IMasterPage
    {
        #region Nested Types

        class ViewMenuHelper : ProjectViewModelBase
        {
            /// <summary>
            ///     Basisklasse für FOTEC MVVM.
            ///     In jedem ViewModel Projekt sollte genau eine Basisklasse für View Models von dieser Klasse ableiten.
            /// </summary>
            public ViewMenuHelper(string pageTitle = "", object args = null, string subTitle = "") : base(pageTitle, args, subTitle)
            {
            }

            /// <summary>
            ///     View wurde geladen
            /// </summary>
            public override Task OnActivated(object args = null)
            {
                return Task.CompletedTask;
            }
        }

        #endregion

        public ViewMaster()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            BindingContext = ViewModel;
            Appearing += (sender, args) => { ViewModel.MainMenu.SetInitialVmCommand(ViewModel.MainMenu.CmdAllMenuCommands.First()); };
        }

        #region Properties

        /// <summary>
        ///     ViewModel
        /// </summary>
        public ProjectViewModelBase ViewModel { get; set; } = new ViewMenuHelper();

        #endregion

        public void UnsetMenueListItems()
        {
            if (ListViewMenuItems != null)
            {
                ListViewMenuItems.SelectedItem = null;
                ListViewMenuItems.SelectedItems.Clear();
            }
        }

        /// <summary>
        ///     Work around damit im Menü immer ein Eintrag auch selektiert ist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewMenuItems_OnSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null ||
                e.NewItems != null ||
                !e.OldItems.Contains(ListViewMenuItems.SelectedItem)) return;

            if (!ListViewMenuItems.SelectedItems.Contains(ListViewMenuItems.SelectedItem))
            {
                var elem = ViewModel.MainMenu.CmdAllMenuCommands.FirstOrDefault(x => x == ListViewMenuItems.SelectedItem);
                Task.Run(async () =>
                {
                    await Task.Delay(1);
                    MainThread.BeginInvokeOnMainThread(() => ListViewMenuItems.SelectedItems.Add(elem));
                });
            }

            ViewModel.BcNavigation().Navigator.ShowMenu(false);
        }
    }
}