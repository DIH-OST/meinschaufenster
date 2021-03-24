// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using BaseApp.ViewModel.UiModel;
using Biss.Apps.Components.Map;
using Biss.Apps.Components.Map.Base;
using Exchange;
using Exchange.Enum;

namespace BaseApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class ViewMain
    {
        private BissMap _map;

        public ViewMain() : this(null)
        {
        }

        public ViewMain(object args = null) : base(args)
        {
            InitializeComponent();

            //ViewModel.RetryMapPermissions += ViewModel_RetryMapPermissions;
            Task.Run(InitMap);
        }


        /// <summary>
        ///     Load Map for UI
        /// </summary>
        /// <returns></returns>
        public Task InitMap()
        {
            try
            {
                if (ViewModel.BcBissMap().State != EnumComponentState.Ok)
                {
                    BissMapExtension.BcInitBissMap(null, AppSettings.Current());

                    ViewModel.BcBissMap().Initialize().Wait();
                }

                _map = ViewModel.BcBissMap().BissMap;
            }
            catch (Exception e)
            {
                Logging.Log.LogError($"{e}");
            }

            if (MapGrid.Children.Any())
                return Task.CompletedTask;

            if (_map == null)
            {
                if (_map == null)
                {
                    ViewModel.OnNoPermission();
                    return Task.CompletedTask;
                }
            }


            ViewModel.RunOnDispatcher(() =>
            {
                // _map.SetBinding(Xamarin.Forms.Maps.Map.IsShowingUserProperty, new Binding("ViewModel.NoAccess", BindingMode.TwoWay, new InvertedBooleanConverter()));
                ViewModel.NoAccess = false;
                MapGrid.Children.Add(_map);
                ViewModel.CmdZoomMyPosition.Execute(null);
                //ViewModel.InitialZoom();
                ViewModel.OnPermissionGranted();
            });

            return Task.CompletedTask;
        }

        /// <summary>
        ///     When overridden, allows application developers to customize behavior immediately prior to the
        ///     <see cref="T:Xamarin.Forms.Page" /> becoming visible.
        /// </summary>
        /// <remarks>To be added.</remarks>
        protected override async void OnAppearing()
        {
            ViewModel.ResetSearchAvailableButton();

            ((App) Application.Current).OnLifeCycleChanged += ViewMain_OnLifeCycleChanged;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            await ViewModel.CheckPermission();

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            ((App) Application.Current).OnLifeCycleChanged -= ViewMain_OnLifeCycleChanged;
            base.OnDisappearing();
        }

        private void ViewModel_RetryMapPermissions(object sender, EventArgs e)
        {
            //ViewModel.RetryMapPermissions -= ViewModel_RetryMapPermissions;
            Task.Run(InitMap);
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(ViewModel.NoAccess)) && _map != null)
            {
                _map.IsShowingUser = !ViewModel.NoAccess;
            }
        }

        private void ViewMain_OnLifeCycleChanged(object sender, EnumLifeCycle e)
        {
            try
            {
                if (e == EnumLifeCycle.Resumed)
                    ViewModel.CmdUpdateShops.Execute(null);
            }
            catch (Exception exception)
            {
                Logging.Log.LogError($"OnLifeCycle Callback Exception occured (ViewMain) {exception.Message}. LifeCycle Enum was {e}");
                throw;
            }
        }

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            //if (e.Item is UiFilterType type)
            //{
            //    if (ViewModel.SelectedFilterType != null)
            //    {
            //        ViewModel.SelectedFilterType.IsSelected = false;
            //    }

            //    type.IsSelected = true;
            //    ViewModel.SelectedFilterType = type;
            //}

            if (e.Item is UiExCategory category)
            {
                if (ViewModel.SelectedCategory != null)
                {
                    ViewModel.SelectedCategory.IsSelected = false;
                }

                category.IsSelected = true;
                ViewModel.SelectedCategory = category;
            }
        }
    }
}