// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using BaseApp.ViewModel.UiModel;
using Biss.Apps.Components.Map.Base;
using Exchange.Enum;
using Exchange.Model;
using Exchange.PostRequests;
using Exchange.Resources;
using Position = Plugin.Geolocator.Abstractions.Position;


namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>BISS Template Start View(Model)</para>
    ///     Klasse ViewModelUserAccount. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelMain : ProjectViewModelBase
    {
        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelMain() : base(ResViewMain.Title, subTitle: ResViewMain.Subtitle)
        {
        }

        #region Helpers

        ///// <summary>
        ///// Gets the location.
        ///// </summary>
        ///// <returns></returns>
        //private void GetLocation()
        //{

        //    if (!CrossGeolocator.IsSupported || !CrossGeolocator.Current.IsGeolocationEnabled)
        //    {
        //        NoAccess = true;
        //        return;
        //    }

        //    if (CrossGeolocator.Current.IsGeolocationAvailable)
        //    {
        //        NoAccess = false;
        //    }

        //    if (_map != null && _map.VisibleRegion != null)
        //        _lastSearchUpdatePosition = _map.VisibleRegion.Center.ToBissPosition();
        //}


        /// <summary>
        ///     Checks if the permissions are granted.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckPermission()
        {
            var permissions = CrossPermissions.Current;
            var granted = await permissions.CheckPermissionStatusAsync(Permission.Location);
            if (granted != PermissionStatus.Granted)
            {
                granted = await permissions.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
            }

            RunOnDispatcher(() => { NoAccess = (granted != PermissionStatus.Granted); });


            return granted == PermissionStatus.Granted;
        }

        #endregion

        #region fields

        private bool _categoriesLoaded;
        private bool _mapInitialized;
        private UiExCategory _selectedCategory;
        private bool _annotationSelected;
        private double _lastSearchUpdateDistance;
        private BissPosition _lastSearchUpdatePosition;
        private BissMap _map;
        private UiFilterType _selectedFilter = new UiFilterType(EnumFilterType.All);
        private string _searchTerm;
        private List<ExShopShort> _shops;
        private bool _permissionsFinished;
        private readonly object _locker = new object();
        private readonly double pinLocationResolution = 0.0001;
        private IGeolocator _locator;

        #endregion

        #region Data sources

        /// <summary>
        ///     Shops
        /// </summary>
        public ObservableCollection<ExShopShort> Shops { get; set; } = new ObservableCollection<ExShopShort>();

        /// <summary>
        ///     Categories
        /// </summary>
        public ObservableCollection<UiExCategory> Categories { get; set; } = new ObservableCollection<UiExCategory>();

        /// <summary>
        ///     Available Filter Types.
        /// </summary>
        public List<UiFilterType> FilterTypes { get; set; } = new List<UiFilterType>
                                                              {
                                                                  new UiFilterType(EnumFilterType.All),
                                                                  new UiFilterType(EnumFilterType.Categories),
                                                                  new UiFilterType(EnumFilterType.Deliveryoptions),
                                                                  new UiFilterType(EnumFilterType.PaymentMethods)
                                                              };

        #endregion

        #region UI related

        /// <summary>
        ///     Text für "Kein Zugriff auf Position erlaubt" anzeigen
        /// </summary>
        public bool NoAccess { get; set; } = true;

        /// <summary>
        ///     The zoom factor or the center of the map has been changed by more than a margin value.
        ///     Indicates that the "update search" button should be visible.
        /// </summary>
        public bool UpdateSearchAvailable { get; set; }

        /// <summary>
        ///     Indicates whether a pin is selected.
        /// </summary>
        public bool AnnotationSelected
        {
            get { return _annotationSelected; }
            set
            {
                _annotationSelected = value;
                CmdWhatsAppShop.CanExecute();
            }
        }

        /// <summary>
        ///     UiData for the selected shop.
        /// </summary>
        public UiExShopData SelectedShop { get; set; }

        /// <summary>
        ///     The selected Filtertype.
        /// </summary>
        public UiFilterType SelectedFilterType
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                if (value != null)
                {
                    CmdShowFilters?.Execute(null);
                    SearchShops(SearchTerm);
                }
            }
        }

        public UiExCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;

                if (_categoriesLoaded)
                {
                    CmdShowFilters?.Execute(null);
                    SearchShops(SearchTerm);
                }
            }
        }

        /// <summary>
        ///     The search term used to search the shops.
        /// </summary>
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                if (value != null)
                {
                    SearchShops(_searchTerm);
                }
            }
        }

        /// <summary>
        ///     The current distance visible on the map.
        /// </summary>
        public string CurrentVisibleDistance { get; set; } = "- km";

        #endregion

        #region Popups

        /// <summary>
        ///     Whether the filter popup should be visible.
        /// </summary>
        public bool IsFilterTypePopupOpen { get; set; }

        public bool IsOpeningHoursPopupOpen { get; set; }

        public bool IsAdditionalPopupOpen { get; set; }

        public string MaintenanceString { get; set; }

        public bool MaintenanceActive { get; set; }

        #endregion

        #region Commands

        /// <summary>
        ///     Zooms the map to the device position.
        /// </summary>
        public VmCommand CmdZoomMyPosition { get; set; }

        /// <summary>
        ///     Shows or hides the filter selection popup.
        /// </summary>
        public VmCommand CmdShowFilters { get; set; }

        /// <summary>
        ///     Updates the shop list if necessary.
        /// </summary>
        public VmCommand CmdUpdateShops { get; set; }

        public VmCommand CmdCallShop { get; set; }

        public VmCommand CmdWhatsAppShop { get; set; }

        public VmCommand CmdCreateAppointment { get; set; }

        public VmCommand CmdShopAdditionalInfoPopup { get; set; }

        public VmCommand CmdShowOpenHours { get; set; }

        public VmCommand CmdRequestPermission { get; set; }

        public VmCommand CmdOpenWebsite { get; set; }

        #endregion

        #region Events

        ///// <summary>
        ///// Ereignis für iOs zum erneuten probieren der Mappermissions
        ///// </summary>
        //public event EventHandler<EventArgs> RetryMapPermissions;

        #endregion

        #region Overrides

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            if (CrossGeolocator.IsSupported)
                _locator = CrossGeolocator.Current;

            CmdZoomMyPosition = new VmCommand("My Position", async () =>
            {
                if (!_locator.IsGeolocationEnabled || !_locator.IsGeolocationAvailable || !await CheckPermission())
                {
                    if (this.BcBissMap().BissMap != null)
                    {
                        this.BcBissMap().BissMap.ZoomLevel = DefaultZoomLevel;
                        this.BcBissMap().BissMap.NewCenter = DefaultPosition;
                        NoAccess = true;
                    }

                    return;
                }

                NoAccess = false;
                var position = new Position();

                IsBusy = true;
                BusyContent = "Suche deine Position ...";

                try
                {
                    position = await _locator.GetLastKnownLocationAsync();

                    if (position == null)
                        position = await _locator.GetPositionAsync(new TimeSpan(0, 0, 5));
                }
                catch (TaskCanceledException)
                {
                    position = new Position(DefaultPosition.Latitude, DefaultPosition.Longitude);
                }

                if (position != null)
                {
                    try
                    {
                        var pos = new BissPosition(position.Latitude, position.Longitude);

                        if (this.BcBissMap().BissMap != null)
                        {
                            this.BcBissMap().BissMap.ZoomLevel = MyZoomLevel;
                            this.BcBissMap().BissMap.NewCenter = pos;
                        }

                        if (_map?.VisibleRegion != null)
                        {
                            // Works but not logic for a user i think.
                            // var ownDist = GeoHelper.Distance(new BissPosition(0, 0), new BissPosition(_map.VisibleRegion.LatitudeDegrees, _map.VisibleRegion.LongitudeDegrees));

                            ResetSearchAvailableButton();
                        }
                    }
                    catch (Exception e)
                    {
                        Logging.Log.LogError($"Error Map: {e}");
                    }
                }

                IsBusy = false;
            }, glyph: "\ue963");

            CmdShowFilters = new VmCommand("", () => { IsFilterTypePopupOpen = !IsFilterTypePopupOpen; });

            CmdWhatsAppShop = new VmCommand("", async () =>
                {
                    var response = await Sa.GetShopInfo(SelectedShop.Shop.Id);

                    if (response.Ok && response.Result != null)
                    {
                        SelectedShop = new UiExShopData(response.Result);
                    }

                    if (string.IsNullOrWhiteSpace(SelectedShop.Shop.WhatsappNumber))
                    {
                        await MsgBox.Show(ResViewMain.MsgBoxWhatsAppContactUnavailable, ResViewMain.MsgBoxWhatsAppContactUnavailableTitle, VmMessageBoxButton.Ok, VmMessageBoxImage.Warning);
                        return;
                    }

                    if (!SelectedShop.Shop.IsFree)
                    {
                        var userResponse = await MsgBox.Show(ResViewMain.LblWhatsAppNotAvailable, ResViewMain.LblHint, VmMessageBoxButton.YesNo);

                        if (userResponse != VmMessageBoxResult.Yes)
                            return;
                    }
                    else
                    {
                        if (await MsgBox.Show(ResViewMain.MsgBoxOpenWhatsApp, ResViewMain.MsgBoxOpenWhatsAppCaption, VmMessageBoxButton.YesNo) != VmMessageBoxResult.Yes)
                            return;
                    }

                    await Nav.ToViewWithResult("ViewWhatsAppTutorial", SelectedShop.Shop.WhatsappNumber);
                }
            );

            CmdOpenWebsite = new VmCommand("", () =>
            {
                if (!string.IsNullOrWhiteSpace(SelectedShop.Shop.WebLink))
                    try
                    {
                        OpenUri(SelectedShop.Shop.WebLink);
                    }
                    catch (Exception e)
                    {
                        Logging.Log.LogError(e.Message);
                    }
            });

            CmdShowOpenHours = new VmCommand("", () => { IsOpeningHoursPopupOpen = !IsOpeningHoursPopupOpen; }, () => { return true; });

            CmdCreateAppointment = new VmCommand("", async () =>
            {
                if (SelectedShop?.Shop != null)
                    await Nav.ToViewWithResult("ViewCreateAppointment", SelectedShop.Shop);
            });

            CmdShopAdditionalInfoPopup = new VmCommand("", () => { IsAdditionalPopupOpen = !IsAdditionalPopupOpen; });

            CmdCallShop = new VmCommand("", async () =>
            {
                if (SelectedShop.Shop.PhoneNumber != null)
                {
                    if (!SelectedShop.Shop.IsOpen)
                    {
                        var response = await MsgBox.Show(ResViewMain.LblOutsideOpeningHours, ResViewMain.LblHint, VmMessageBoxButton.YesNo);
                        if (response != VmMessageBoxResult.Yes)
                            return;
                    }

                    PhoneDialer.Open($"00{SelectedShop.Shop.PhoneNumber}");
                }
            });

            CmdUpdateShops = new VmCommand("", async () =>
            {
                if (_map?.VisibleRegion == null)
                    return;

                await CheckPermission();
                var request = new ExGetShopsRequest
                              {
                                  MyPosition = new BissPosition(_map.VisibleRegion.Center.Latitude, _map.VisibleRegion.Center.Longitude),
                                  Range = _map.VisibleRegion.Radius.Meters
                              };

                _lastSearchUpdateDistance = _map.VisibleRegion.Radius.Meters;
                _lastSearchUpdatePosition = request.MyPosition;
                UpdateSearchAvailable = false;
                await LoadShops(request);
            });

            CmdRequestPermission = new VmCommand(ResViewMain.CmdRequestPermission, async () =>
            {
                if (await CheckPermission())
                {
                    //await GetLocation();
                    try
                    {
                        CmdZoomMyPosition.Execute(null);
                    }
                    catch (Exception e)
                    {
                        Logging.Log.LogError(e.Message);
                    }
                }
            });
        }

        public void InitialZoom()
        {
            if (this.BcBissMap().BissMap != null)
            {
                this.BcBissMap().BissMap.ZoomLevel = DefaultZoomLevel;
                this.BcBissMap().BissMap.NewCenter = DefaultPosition;
            }
        }

        /// <summary>
        ///     View wurde erzeugt
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override async Task OnActivated(object args = null)
        {
            await DeviceInfoUpdate();
            CManager.InitAllNotInitialized();
            if (this.BcBissMap().State == EnumComponentState.Ok && this.BcBissMap().BissMap != null)
            {
                this.BcBissMap().BissMap.MapType = (int) EnumMapType.Street;

                foreach (var mapPin in this.BcBissMap().BissMap.Pins)
                {
                    mapPin.MarkerClicked -= ShopClicked;
                    mapPin.InfoWindowClicked -= ShopClicked;
                }

                this.BcBissMap().BissMap.Pins.Clear();

                await Task.Run(() =>
                {
                    Parallel.ForEach(Shops, async shop =>
                    {
                        try
                        {
                            var newPin = DeviceInfo.Plattform == EnumPlattform.XamarinAndroid
                                ? await shop.MainCategory.Pin.GetDroidPin()
                                : DeviceInfo.Plattform == EnumPlattform.XamarinIos
                                    ? await shop.MainCategory.Pin.GetIosPin()
                                    : await shop.MainCategory.Pin.GetPin();

                            newPin.MarkerClicked += ShopClicked;
                            newPin.InfoWindowClicked += ShopClicked;

                            RunOnMainThread(() => { this.BcBissMap().BissMap.Pins.Add(newPin); });
                        }
                        catch (Exception e)
                        {
                            Logging.Log.LogError("Error while adding Pin " + e);
                        }
                    });
                });
            }

            var timer = new Timer(3000);
            timer.Elapsed += (sender, t) =>
            {
                if (_map != null || _permissionsFinished)
                {
                    _mapInitialized = true;
                    timer.Stop();
                    timer.Dispose();
                }
                else
                {
                    if (this.BcBissMap().State != EnumComponentState.Ok && DeviceInfo.Plattform == EnumPlattform.XamarinIos)
                    {
                        //RetryMapPermissions?.Invoke(this, null);
                    }
                }
            };
            timer.Start();
            var maintenance = await Sa.GetMaintenanceInfo();
            if (maintenance.Ok)
            {
                if (!string.IsNullOrWhiteSpace(maintenance.Result))
                {
                    await MsgBox.Show(maintenance.Result, ResViewMain.LblMaintenance);
                }
            }

            await LoadFilters();
        }

        #endregion

        #region Callbacks

        public void OnNoPermission()
        {
            if (_permissionsFinished) return;

            _permissionsFinished = true;

            RunOnMainThread(async () =>
            {
                IsBusy = false;
                await MsgBox.Show(ResView.MsgBoxLocationAccessInfo, ResView.MsgBoxLocationAccessInfoTitle);
                RunOnDispatcher(() => { NoAccess = true; });
            });
        }

        public async void OnPermissionGranted()
        {
            _permissionsFinished = true;

            if (this.BcBissMap().State == EnumComponentState.Ok && this.BcBissMap().BissMap != null)
            {
                _map = this.BcBissMap().BissMap;
                _map.MapClicked += MapClicked;
                _map.PropertyChanged += MapPropertyChanged;
                _map.MapType = (int) EnumMapType.Street;

                _map.Pins.Clear();
                await LoadShops();
            }
        }

        public void ResetSearchAvailableButton()
        {
            if (_map?.VisibleRegion != null)
            {
                var distance = _map.VisibleRegion.Radius.Kilometers;
                CurrentVisibleDistance = $"{Math.Round(distance)} km";
                _lastSearchUpdateDistance = distance;
                _lastSearchUpdatePosition = _map.VisibleRegion.Center.ToBissPosition();
                UpdateSearchAvailable = false;
            }
        }

        private void MapPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (!_mapInitialized)
                return;

            if (args.PropertyName.Equals(nameof(_map.VisibleRegion)))
            {
                var distance = _map.VisibleRegion.Radius.Kilometers;
                CurrentVisibleDistance = $"{Math.Round(distance)} km";
                if (_lastSearchUpdateDistance <= 0.001)
                {
                    _lastSearchUpdateDistance = _map.VisibleRegion.Radius.Kilometers;
                }
                else if (Math.Abs(_lastSearchUpdateDistance - _map.VisibleRegion.Radius.Kilometers) > 1.5)
                {
                    UpdateSearchAvailable = true;
                }

                if (_lastSearchUpdatePosition == null)
                {
                    _lastSearchUpdatePosition = _map.VisibleRegion.Center.ToBissPosition();
                }
                else
                {
                    var dist = GeoHelper.Distance(_map.VisibleRegion.Center.ToBissPosition(), _lastSearchUpdatePosition);
                    if (dist > 5000)
                    {
                        UpdateSearchAvailable = true;
                    }
                }
            }
        }

        /// <summary>
        ///     A Map pin or the annotation near it has been clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ShopClicked(object sender, PinClickedEventArgs e)
        {
            if (!(sender is BissPin pin))
                return;

            ExShopShort selected = null;

            var shops = Shops.Where(s => s != null && s.Name.ToLower().Equals(pin.Label.ToLower())).ToList();

            if (shops.Any())
            {
                selected = shops[0];
            }

            if (shops.Count > 1)
            {
                var nearest = shops.FirstOrDefault(s => GeoHelper.Distance(pin.Position.ToBissPosition(), s.Position) < 300);
                if (nearest != null)
                    selected = nearest;
            }

            if (selected == null)
                return;

            BusyContent = "Lade ...";
            IsBusy = true;
            var response = await Sa.GetShopInfo(selected.Id);

            if (response.Ok && response.Result != null)
            {
                SelectedShop = new UiExShopData(response.Result);
                AnnotationSelected = true;
            }
            else
            {
                await MsgBox.Show(ResViewMain.MsgBoxLoadError, ResViewMain.MsgBoxLoadErrorCaption);
            }

            IsBusy = false;
        }

        private void MapClicked(object sender, MapClickedEventArgs e)
        {
            AnnotationSelected = false;
        }

        #endregion

        #region Loading

        /// <summary>
        ///     Loads the available filters.
        /// </summary>
        /// <returns></returns>
        private async Task LoadFilters()
        {
            var response = await Sa.GetFilterInfos();

            if (response.Result != null && response.Result.Categories != null)
            {
                Categories.Clear();
                Categories.Add(new UiExCategory(new ExCategory
                                                {
                                                    Name = "Alle"
                                                }));

                response.Result.Categories = response.Result.Categories.OrderBy(c => c.Name).ToList();

                foreach (var item in response.Result.Categories)
                {
                    Categories.Add(new UiExCategory(item));
                }

                SelectedCategory = Categories.FirstOrDefault(c => c.Category.Name.ToLower().Equals("alle"));
                _selectedCategory.IsSelected = true;
                _categoriesLoaded = true;
            }
        }

        /// <summary>
        ///     Loads the shops.
        /// </summary>
        /// <param name="request">The necessary data. If null a default value will be used.</param>
        /// <returns>nothing</returns>
        private async Task LoadShops(ExGetShopsRequest request = null)
        {
            if (request == null)
            {
                if (_lastSearchUpdatePosition == null)
                {
                    if (_map?.VisibleRegion != null)
                    {
                        _lastSearchUpdateDistance = _map.VisibleRegion.Radius.Meters;
                        _lastSearchUpdatePosition = _map.VisibleRegion.Center.ToBissPosition();
                    }
                }

                var distance = _lastSearchUpdateDistance > 0 ? _lastSearchUpdateDistance : 1000;

                request = new ExGetShopsRequest
                          {
                              MyPosition = _lastSearchUpdatePosition,
                              Range = distance
                          };
            }

            var shopResult = await Sa.GetShops(request);

            if (shopResult.Result != null && shopResult.Result.Count > 0)
            {
                _shops = shopResult.Result;
            }

            SearchShops(SearchTerm);
        }

        #endregion

        #region Search

        /// <summary>
        ///     Searches the shops.
        /// </summary>
        /// <param name="searchTerm">The tearm to look for.</param>
        private async void SearchShopsByAllAttributes(string searchTerm)
        {
            var results = new List<ExShopShort>();

            if (string.IsNullOrWhiteSpace(searchTerm) && _shops != null)
            {
                results = _shops;
            }
            else
            {
                searchTerm = searchTerm.ToLower();

                switch (SelectedFilterType.Type)
                {
                    case EnumFilterType.Categories:
                        results = _shops.Where(s => s.MainCategory.Name.ToLower().Contains(searchTerm) ||
                                                    s.Categories.Any(c => c.Name.ToLower().Contains(searchTerm))).ToList();
                        break;
                    case EnumFilterType.Deliveryoptions:
                        results = _shops.Where(s => s.DeliveryMethods.Any(c => c.Name.ToLower().Contains(searchTerm))).ToList();
                        break;
                    case EnumFilterType.PaymentMethods:
                        results = _shops.Where(s => s.PaymentMethods.Any(c => c.Name.ToLower().Contains(searchTerm))).ToList();
                        break;
                    default:
                        results = _shops.Where(s => s.Name.ToLower().Contains(searchTerm) ||
                                                    s.MainCategory.Name.ToLower().Contains(searchTerm) ||
                                                    s.Categories.Any(c => c.Name.ToLower().Contains(searchTerm) ||
                                                                          s.PaymentMethods.Any(p => p.Name.ToLower().Contains(searchTerm)))).ToList();
                        break;
                }
            }

            if (results.Any())
            {
                var deletions = Shops.Where(s => !results.Any(r => s.Id == r.Id)).ToList();

                foreach (var toDelete in deletions)
                {
                    Shops.Remove(toDelete);
                    var pin = _map.Pins.FirstOrDefault(p => Math.Abs(p.Position.Latitude - toDelete.Position.Latitude) < pinLocationResolution && Math.Abs(p.Position.Longitude - toDelete.Position.Longitude) < pinLocationResolution);
                    if (pin != null)
                    {
                        _map.Pins.Remove(pin);
                        pin.InfoWindowClicked -= ShopClicked;
                        pin.MarkerClicked -= ShopClicked;
                    }
                }
            }
            else
            {
                foreach (var pin in _map.Pins)
                {
                    pin.InfoWindowClicked -= ShopClicked;
                    pin.MarkerClicked -= ShopClicked;
                }

                _map.Pins.Clear();
                Shops.Clear();
            }

            AnnotationSelected = false;

            var shopsToAdd = results.Where(x => x != null && !Shops.Any(s => s != null && s.Id == x.Id)).ToList();

            Parallel.ForEach(shopsToAdd, async result =>
            {
                lock (_locker)
                {
                    Shops.Add(result);
                }

                var pin = _map.Pins.FirstOrDefault(p => Math.Abs(p.Position.Latitude - result.Position.Latitude) < pinLocationResolution && Math.Abs(p.Position.Longitude - result.Position.Longitude) < pinLocationResolution);
                if (pin == null)
                {
                    try
                    {
                        pin = DeviceInfo.Plattform == EnumPlattform.XamarinAndroid
                            ? await result.MainCategory.Pin.GetDroidPin()
                            : DeviceInfo.Plattform == EnumPlattform.XamarinIos
                                ? await result.MainCategory.Pin.GetIosPin()
                                : await result.MainCategory.Pin.GetPin();

                        pin.MarkerClicked += ShopClicked;
                        pin.InfoWindowClicked += ShopClicked;

                        RunOnMainThread(() => _map.Pins.Add(pin));
                    }
                    catch (Exception e)
                    {
                        Logging.Log.LogError("Error while adding Pin " + e);
                    }
                }
            });
        }

        /// <summary>
        ///     Searches the shops.
        /// </summary>
        /// <param name="searchTerm">The tearm to look for.</param>
        private async void SearchShops(string searchTerm = "")
        {
            if (_shops == null)
                return;

            var results = new List<ExShopShort>();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                if (SelectedCategory == null)
                {
                    results = _shops;
                }
                else
                {
                    if (!SelectedCategory.Category.Name.ToLower().Equals("alle"))
                    {
                        results = _shops.Where(s => s.MainCategory.Name.ToLower().Equals(SelectedCategory.Category.Name.ToLower())).ToList();
                    }
                    else
                    {
                        results = _shops;
                    }
                }
            }
            else
            {
                searchTerm = searchTerm.ToLower();

                if (SelectedCategory != null)
                {
                    if (!SelectedCategory.Category.Name.ToLower().Equals("alle"))
                    {
                        results = _shops.Where(s => s.Name.ToLower().Contains(searchTerm) && s.MainCategory.Name.ToLower().Equals(searchTerm)).ToList();
                    }
                    else
                    {
                        results = _shops.Where(s => s.Name.ToLower().Contains(searchTerm) ||
                                                    s.MainCategory.Name.ToLower().Contains(searchTerm) ||
                                                    s.Categories.Any(c => c.Name.ToLower().Contains(searchTerm))).ToList();
                    }
                }
            }

            if (results.Any())
            {
                var deletions = Shops.Where(s => !results.Any(r => s.Id == r.Id)).ToList();

                foreach (var toDelete in deletions)
                {
                    Shops.Remove(toDelete);
                    var pin = _map.Pins.FirstOrDefault(p => Math.Abs(p.Position.Latitude - toDelete.Position.Latitude) < pinLocationResolution && Math.Abs(p.Position.Longitude - toDelete.Position.Longitude) < pinLocationResolution);
                    if (pin != null)
                    {
                        _map.Pins.Remove(pin);
                        pin.InfoWindowClicked -= ShopClicked;
                        pin.MarkerClicked -= ShopClicked;
                    }
                }
            }
            else
            {
                foreach (var pin in _map.Pins)
                {
                    pin.InfoWindowClicked -= ShopClicked;
                    pin.MarkerClicked -= ShopClicked;
                }

                _map.Pins.Clear();
                Shops.Clear();
            }

            AnnotationSelected = false;

            var shopsToAdd = results.Where(x => x != null && !Shops.Any(s => s != null && s.Id == x.Id)).ToList();

            Parallel.ForEach(shopsToAdd, async result =>
            {
                lock (_locker)
                {
                    Shops.Add(result);
                }

                var pin = _map.Pins.FirstOrDefault(p => Math.Abs(p.Position.Latitude - result.Position.Latitude) < pinLocationResolution && Math.Abs(p.Position.Longitude - result.Position.Longitude) < pinLocationResolution);
                if (pin == null)
                {
                    try
                    {
                        pin = DeviceInfo.Plattform == EnumPlattform.XamarinAndroid
                            ? await result.MainCategory.Pin.GetDroidPin()
                            : DeviceInfo.Plattform == EnumPlattform.XamarinIos
                                ? await result.MainCategory.Pin.GetIosPin()
                                : await result.MainCategory.Pin.GetPin();

                        pin.MarkerClicked += ShopClicked;
                        pin.InfoWindowClicked += ShopClicked;

                        RunOnMainThread(() => _map.Pins.Add(pin));
                    }
                    catch (Exception e)
                    {
                        Logging.Log.LogError("Error while adding Pin " + e);
                    }
                }
            });

            try // Could not thoroughly test.
            {
                if (Shops.Any(s => s == null))
                {
                    var shopList = new List<ExShopShort>(Shops);
                    for (int i = shopList.Count - 1; i > 0; i--)
                    {
                        if (shopList[i] == null)
                            Shops.RemoveAt(i);
                    }
                }
            }
            catch (Exception e)
            {
                Logging.Log.LogError("Null Element in Shops");
            }
        }

        #endregion
    }
}