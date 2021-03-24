// DigitalesSchaufenster (C) 2020 DIH-OST

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;

namespace AndroidApp
{
    /// <summary>
    ///     <para>SplashActivity - Erste Aktivität bei Android</para>
    ///     Klasse SplashActivity. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Activity(MainLauncher = true, Theme = "@style/MainTheme.Splash", NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            Log.Info("BissAndroidSplashActivity", "OnCreate: " + bundle);

            base.OnCreate(bundle);

            //BissPushService.setActToUpdate(this);

            var openMainActivity = new Intent(BaseContext, typeof(MainActivity));
            openMainActivity.SetFlags(ActivityFlags.ReorderToFront);

            if (bundle != null)
            {
                Log.Info("BissAndroidSplashActivity", "Started with Extras");
                openMainActivity.PutExtras(bundle);
            }

            StartActivityIfNeeded(openMainActivity, 0);

            Finish();
        }

        protected override void OnNewIntent(Intent intent)
        {
            Log.Info("BissAndroidSplashActivity", "OnNewIntent: " + intent);
            Log.Info("BissAndroidSplashActivity", "OnNewIntent.Extras: " + intent?.Extras);

            base.OnNewIntent(intent);
        }
    }
}