// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using BaseApp;
using Biss.Apps.Components.AzurePush.Droid;
using Exchange;
using Platform = ZXing.Net.Mobile.Forms.Android.Platform;

namespace AndroidApp
{
    /// <summary>
    ///     <para>MainActivity - Wird durch SplashActivity aufgerufen</para>
    ///     Klasse MainActivity. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Activity(Label = "AndroidApp", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : FormsAppCompatActivity
    {
        //AzurePush

        //bool IsPlayServiceAvailable()
        //{
        //    int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
        //    if (resultCode != ConnectionResult.Success)
        //    {
        //        if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
        //            Log.Debug(AzurePushSettings.DebugTag, GoogleApiAvailability.Instance.GetErrorString(resultCode));
        //        else
        //        {
        //            Log.Debug(AzurePushSettings.DebugTag, "This device is not supported");
        //        }
        //        return false;
        //    }
        //    return true;
        //}

        //void CreateNotificationChannel()
        //{
        //    // Notification channels are new as of "Oreo".
        //    // There is no need to create a notification channel on older versions of Android.
        //    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        //    {
        //        var channelName = AzurePushSettings.NotificationChannelName;
        //        var channelDescription = String.Empty;
        //        var channel = new NotificationChannel(channelName, channelName, NotificationImportance.Default)
        //                      {
        //                          Description = channelDescription
        //                      };

        //        var notificationManager = (NotificationManager)GetSystemService(NotificationService);
        //        notificationManager.CreateNotificationChannel(channel);
        //    }
        //}

        //END


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle bundle)
        {
            var parameters = new object[]
                             {
                                 AppSettings.Current().ProjectWorkUserFolder,
                                 this,
                                 bundle,
                                 new PlatformOptions {SmallIconDrawable = Resource.Drawable.ic_notification},
                             };

            var initializer = new BissInitializer();
            if (!initializer.Initialize(parameters))
            {
                throw new ApplicationException("Initialization failed");
            }

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            FormsMaps.Init(this, bundle);

            InitializeDisplayOrientations();

            Platform.Init();
            LoadApplication(new App(initializer));

            if (Intent?.Extras != null)
            {
                var dict = new Dictionary<string, string>();

                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.Get(key);
                    Log.Debug("BissAndroidMainActivity", "Key: {0} Value: {1}", key, value);

                    dict.Add(key, value.ToString());
                }

                ProjectViewModelBase.PushReceived(dict);
            }

            var pushOk = PushAzure.Init(this, NotificationService);
        }

        protected override void OnNewIntent(Intent intent)
        {
            Log.Info("BissAndroidMainActivity", "OnNewIntent: " + intent?.Extras);

            // Fix für doppelten Notification Empfang falls auf die Push Nachricht getapt worden ist. 
            intent.AddFlags(ActivityFlags.ClearTop);

            base.OnNewIntent(intent);
            //if (Constants.AppNeedInternet)
            //{
            //    Push.CheckLaunchedFromNotification(this, intent);
            //}

            if (intent?.Extras != null)
            {
                var dict = new Dictionary<string, string>();

                foreach (var key in intent.Extras.KeySet())
                {
                    var value = intent.Extras.Get(key);
                    Log.Debug("BissAndroidMainActivity", "Key: {0} Value: {1}", key, value);

                    dict.Add(key, value.ToString());
                }

                ProjectViewModelBase.PushReceived(dict);
            }
        }

        private void Close()
        {
            MoveTaskToBack(true);
        }

        private void InitializeDisplayOrientations()
        {
            MessagingCenter.Subscribe<OrientationSender>(this, EnumDisplayOrientation.Unspecified.ToString(), sender => { RequestedOrientation = ScreenOrientation.Unspecified; });

            MessagingCenter.Subscribe<OrientationSender>(this, EnumDisplayOrientation.Portrait.ToString(), sender => { RequestedOrientation = ScreenOrientation.Portrait; });

            MessagingCenter.Subscribe<OrientationSender>(this, EnumDisplayOrientation.PortraitReversed.ToString(), sender => { RequestedOrientation = ScreenOrientation.ReversePortrait; });

            MessagingCenter.Subscribe<OrientationSender>(this, EnumDisplayOrientation.Landscape.ToString(), sender => { RequestedOrientation = ScreenOrientation.Landscape; });

            MessagingCenter.Subscribe<OrientationSender>(this, EnumDisplayOrientation.LandscapeReversed.ToString(), sender => { RequestedOrientation = ScreenOrientation.ReverseLandscape; });
        }
    }
}