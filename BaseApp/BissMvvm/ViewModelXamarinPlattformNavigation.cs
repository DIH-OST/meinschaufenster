// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange.Resources;

namespace BaseApp.BissMvvm
{
    /// <summary>
    ///     <para>Navigator für Xamarin Forms</para>
    ///     Klasse ViewModelXamarinPlattformNavigation. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelXamarinPlattformNavigation : VmNavigator
    {
        private readonly NavigationService _navService;

        #region Nested Types

        /// <summary>
        ///     <para>Abstaktion der Navigation für Xamarin Forms</para>
        ///     Klasse NavigationService. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
        /// </summary>
        internal class NavigationService
        {
            private readonly App _app;
            private readonly string _backButtonText;

            private readonly List<PopHandler> _popHandlers = new List<PopHandler>();

            public NavigationService(App app, VmNavigator navigator)
            {
                _app = app;
                _backButtonText = navigator.DefaultBackText;
                navigator.SwitchPage += OnSwitchPage;
            }

            #region Properties

            /// <summary>
            ///     NavigationPage
            /// </summary>
            public ViewNavigation NavPage { get; set; }

            /// <summary>
            ///     MasterDetailPage
            /// </summary>
            public ViewMasterDetail MasterDetailPage { get; set; }

            /// <summary>
            ///     MasterPage
            /// </summary>
            public ViewMaster MasterPage { get; set; }

            #endregion

            /// <summary>
            ///     Navigationpage setzten, mit MasterDetailPage als parent oder eben nicht
            /// </summary>
            public void SetNavPage(ViewNavigation page, bool? useMasterDetail = null)
            {
                //destroy all pophandlers
                foreach (var handler in _popHandlers)
                    handler.Cancel();
                _popHandlers.Clear();

                //Alte Page muss sich wieder aus Events austragen
                //NavPage?.Dispose();

                //letzte navpage merken
                NavPage = page;

                //Master detail hinzufügen
                if (MasterDetailPage == null && useMasterDetail.HasValue && useMasterDetail.Value)
                {
                    MasterPage = new ViewMaster();
                    MasterDetailPage = new ViewMasterDetail
                                       {
                                           Master = MasterPage,
                                           Detail = page
                                       };
                    _app.MainPage = MasterDetailPage;
                } //master detail entfernen
                else if (MasterDetailPage != null && useMasterDetail.HasValue && !useMasterDetail.Value)
                {
                    _app.MainPage = page;
                    MasterDetailPage = null;
                    MasterPage = null;
                }
                else
                {
                    //keine Änderung an MasterDetail Setting
                    if (MasterDetailPage != null)
                        MasterDetailPage.Detail = page;
                    else
                        _app.MainPage = page;
                }
            }

            /// <summary>
            ///     Master Page neu laden (zB. dyn. Menüeinträgen bzw. Sprache)
            /// </summary>
            public void UpdateMaster()
            {
                if (MasterDetailPage != null)
                {
                    MasterPage = new ViewMaster();
                    MasterDetailPage.Master = MasterPage;
                }
            }

            /// <summary>
            ///     Eine Page zurück
            /// </summary>
            /// <returns></returns>
            public async Task PopPage()
            {
                var navPage = NavPage;

                if (navPage.InternalChildren.Count < 1)
                {
                }

                try
                {
                    await navPage.PopAsync(true);
                }
                catch (Exception e)
                {
                    Logging.Log.LogError($"Nav Error: {e}");
                }
            }

            /// <summary>
            ///     Eine Page auf den Stack pushen
            /// </summary>
            /// <param name="nextPage">die Page</param>
            /// <returns>Ergebnis</returns>
            public async Task<object> PushPage(object nextPage)
            {
                if (MasterDetailPage != null)
                    MasterDetailPage.IsPresented = false;

                var vm = (nextPage as IView).GetViewModel();
                var page = nextPage as Page;

                var navPage = NavPage;

                await navPage.PushAsync(page, true);

                var handler = new PopHandler(navPage, page);
                _popHandlers.Add(handler);
                var task = handler.WaitTillItPops();
                await task;

                if (!handler.HasBeenCanceled)
                    return vm.ViewResult;
                return null;
            }

            /// <summary>
            ///     Seite wechseln, Neue Nav page anlegen
            /// </summary>
            /// <param name="p">seite</param>
            /// <param name="showMenuPage"></param>
            public async void SwitchPage(Page p, bool? showMenuPage = null)
            {
                if (MasterDetailPage != null && MasterDetailPage.Detail != null && showMenuPage == null)
                    await PushPage(p);
                else
                    SetNavPage(CreateNavPage(p), showMenuPage);

                if (MasterDetailPage != null)
                {
                    MasterPage.UnsetMenueListItems();
                    MasterDetailPage.IsPresented = false;
                }
            }

            /// <summary>
            ///     View (einzige dann am Stack) wird geladen
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            private void OnSwitchPage(object sender, ViewModelNavigatorNextPageArgs args)
            {
                SwitchPage((Page) args.NextPage, args.UseMasterDetail);
            }

            /// <summary>
            ///     Hilfsfunktion zum Erzeugen einer Navigationseite
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            private ViewNavigation CreateNavPage(Page p)
            {
                NavigationPage.SetHasBackButton(p, false);
                NavigationPage.SetHasNavigationBar(p, false);
                NavigationPage.SetBackButtonTitle(p, _backButtonText);

                return new ViewNavigation(p);
            }
        }

        /// <summary>
        ///     Warten bis die eine angegebene Page von der NavigationPage gepoppt wurde und anschließendes unsubscriben auf den
        ///     Eventhandler
        ///     (Hilfsklasse notwendig um sich nur für die spezifische Page ein und auszuhängen)
        /// </summary>
        private class PopHandler
        {
            readonly AutoResetEvent _are = new AutoResetEvent(false);
            readonly NavigationPage _navPage;
            private readonly Page _p;

            public PopHandler(NavigationPage navPage, Page p)
            {
                _navPage = navPage;
                _p = p;
                _navPage.Popped += NavPageOnPopped;
            }

            #region Properties

            /// <summary>
            ///     HasBeenCanceled (bei einer Cancelllation null zurückgeben)
            /// </summary>
            public bool HasBeenCanceled { get; set; }

            #endregion

            /// <summary>
            ///     Warten bis die eine angegebene Page von der NavigationPage gepoppt wurde und anschließendes unsubscriben auf den
            ///     Eventhandler
            /// </summary>
            /// <returns>Task</returns>
            public async Task WaitTillItPops()
            {
                var waitingTask = Task.Run(() => _are.WaitOne());
                await waitingTask;
                _navPage.Popped -= NavPageOnPopped;
            }


            /// <summary>
            ///     Task abbrechen
            /// </summary>
            public void Cancel()
            {
                HasBeenCanceled = true;
                _are.Set();
            }

            private void NavPageOnPopped(object sender, NavigationEventArgs navigationEventArgs)
            {
                if (navigationEventArgs.Page == _p)
                    _are.Set();
            }
        }

        #endregion

        /// <summary>
        ///     Navigator für Xamarin Forms
        /// </summary>
        /// <param name="app"></param>
        /// <param name="settings"></param>
        public ViewModelXamarinPlattformNavigation(App app, IAppSettingsNavigation settings) : base(settings)
        {
            _navService = new NavigationService(app, this);
        }

        /// <summary>
        ///     Typ der View für neue Instanz des Objekts
        /// </summary>
        /// <param name="viewClassName"></param>
        /// <returns></returns>
        public override Type GetViewType(string viewClassName)
        {
            var fullViewClassName = DefaultViewNamespace + viewClassName;
            var type = Type.GetType(fullViewClassName);

            return type;
        }

        /// <summary>
        ///     Menü (zB. Sprache) hat sich geändert => neu erzeugen
        /// </summary>
        public override void UpdateMenu()
        {
            _navService.UpdateMaster();
        }

        /// <summary>
        ///     Ein Eigabefenster starten und die eingegeben Daten zurück liefern
        ///     WPF: Ein neues Fenster wird erzeugt (kann gebunden asModal angezeigt werden)
        ///     APPS: Eine neue View wird auf den aktuellen NaviagtionStack gepusht
        /// </summary>
        /// <param name="view">Instanzierte View</param>
        /// <param name="asModal"></param>
        /// <returns></returns>
        public override async Task<object> NavToWindow(object view, bool asModal)
        {
            var r = await _navService.PushPage(view);

            //Workaround für iOs/Android Software Back Button (eventuell in Zukunft nicht notwendig)!
            //Nur noch notwendig wenn BuildIn NavBar verwendet wird!
            var v = view as IView;
            if (v != null)
            {
                var check = await v.GetViewModel().CheckForSave();
                while (!check)
                {
                    r = await _navService.PushPage(view);
                    check = await v.GetViewModel().CheckForSave();
                }
            }

            return r;
        }

        /// <summary>
        ///     Eine View laden
        ///     WPF: View wird im CONTENT FRAME des MainWindow geladen
        ///     APPS: View wird mit Menü angezeigt und ist dann die einzige View am Stack
        /// </summary>
        /// <param name="viewInstance"></param>
        /// <param name="showMenu">Menü Seite anzueigen</param>
        public override void NavToPage(object viewInstance, bool? showMenu = null)
        {
            OnSwitchPage(new ViewModelNavigatorNextPageArgs {NextPage = viewInstance, UseMasterDetail = showMenu});
        }

        /// <summary>
        ///     WPF: Fenster schließen
        ///     Apps: Zurück navigieren
        /// </summary>
        /// <returns></returns>
        public override async Task BackOrClose()
        {
            if (GetItemsOnPageStack() > 1)
            {
                await _navService.PopPage();
            }
            else if (_navService.NavPage.Navigation.NavigationStack.FirstOrDefault().Title == ResViewMain.Title)
            {
                var closer = DependencyService.Get<IQuitApplication>();
                if (closer == null)
                {
                    Logging.Log.LogWarning("App kann nicht geschlossen werden!");
                    return;
                }

                closer.Quit();
            }
            else
            {
                NavigatorExtensions.BcNavigation(null).ToView("ViewMain");
            }
        }

        /// <summary>
        ///     Anzahl der Seiten am Stack
        /// </summary>
        /// <returns></returns>
        public override int GetItemsOnPageStack()
        {
            int count = 0;
            try
            {
                count = _navService.NavPage.Navigation.NavigationStack.Count;
            }
            catch
            {
                count = 0;
            }

            return count;
        }
    }
}