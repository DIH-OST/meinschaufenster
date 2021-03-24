// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using BaseApp;

namespace AndroidApp
{
    /// <summary>
    ///     <para>
    ///         CrossCurrentActivity - Initialisieren
    ///         Do not delete thise file! It was here because plugins depend on it.
    ///         If you have an existing Application class you can merge the two together
    ///         if you have existing assembly:Application, you can remove them.
    ///     </para>
    ///     Klasse MainApplication. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
#if DEBUG
    [Application(Debuggable = true)]
#else
	[Application(Debuggable = false)]
#endif
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
            Log.Info("BissAndroidMainApp", "Ctor");
        }

        public override void OnCreate()
        {
            Log.Info("BissAndroidMainApp", "OnCreate");
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
        }

        public override void OnTerminate()
        {
            Log.Info("BissAndroidMainApp", "OnTerminate");
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            Log.Info("BissAndroidMainApp", "OnActivityCreate: " + activity?.Intent?.Extras + "; " + savedInstanceState);
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
            // Keine Ahnung warum, aber wenn die App von einem Toast wieder in den Vordergrund gebracht wird, kommen die Extras mit den Notification infos nur hierher

            Log.Info("BissAndroidMainApp", "OnActivityDestroyed: " + activity?.Intent?.Extras);

            var intent = activity?.Intent;

            if (intent?.Extras != null)
            {
                var dict = new Dictionary<string, string>();

                foreach (var key in intent.Extras.KeySet())
                {
                    if (key.StartsWith("com.microsoft.intune") || key == "profile")
                    {
                        continue;
                    }

                    var value = intent.Extras.Get(key);
                    Log.Debug("BissAndroidMainActivity", "Key: {0} Value: {1}", key, value);

                    dict.Add(key, value.ToString());
                }

                if (dict.Any())
                    // Launched from Notification or Toast
                    ProjectViewModelBase.PushReceived(dict);
            }
        }

        public void OnActivityPaused(Activity activity)
        {
            Log.Info("BissAndroidMainApp", "OnActivityPaused: " + activity?.Intent?.Extras);
        }

        public void OnActivityResumed(Activity activity)
        {
            Log.Info("BissAndroidMainApp", "OnActivityResumed: " + activity?.Intent?.Extras);
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            Log.Info("BissAndroidMainApp", "OnActivitySaveInstanceState: " + activity?.Intent?.Extras + "; " + outState);
        }

        public void OnActivityStarted(Activity activity)
        {
            Log.Info("BissAndroidMainApp", "OnActivityStarted: " + activity?.Intent?.Extras);
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
            Log.Info("BissAndroidMainApp", "OnActivityStopped: " + activity?.Intent?.Extras);
        }
    }
}