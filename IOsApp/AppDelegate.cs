// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseApp;
using Biss.Apps.Components;
using Exchange;
using Foundation;
using UIKit;
using UserNotifications;
using Platform = ZXing.Net.Mobile.Forms.iOS.Platform;

namespace IOsApp
{
    /// <summary>
    ///     <para>UWP Main Page - Xamarin Forms und BISS MVVM Initialisieren und Xamarin Forms starten </para>
    ///     The UIApplicationDelegate for the application. This class is responsible for launching the
    ///     User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    ///     Klasse AppDelegate. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        private UIInterfaceOrientationMask _orientation = UIInterfaceOrientationMask.All;

        #region Properties

        public override UIWindow Window { get; set; }

        #endregion

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // General Initialization
            var initializer = new BissInitializer();
            var parameters = new object[]
                             {
                                 AppSettings.Current().ProjectWorkUserFolder
                             };

            // Just throwing an exception for now. 
            if (!initializer.Initialize(parameters))
            {
                throw new ApplicationException("Initialization failed");
            }

            InitializeDisplayOrientations();

            UIApplication.SharedApplication.RegisterForRemoteNotifications();
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

            if (Constants.AppNeedInternet)
            {
                PushExtension.BcInitPush(null);

                //Notifizeirungen global auch wenn die App im Vordergrund läuft => Besser über VmBase. ... .Badge
                //UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
                // Request notification permissions from the user
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, async (approved, err) =>
                {
                    // Handle approval
                    if (!approved)
                    {
                        await Task.Delay(200);
                        // Beim Erststart nach installation ist approved immer False - deshalb nochmal checken, ob wirklich abgelehnt.
                        UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, async (approved2, err2) => { await PushExtension.BcPush(null).EnablePush(approved2); });
                    }
                    else
                    {
                        await PushExtension.BcPush(null).EnablePush(approved);
                    }
                });
            }

            var remoteNotificationInfo = launchOptions?.ObjectForKey(UIApplication.LaunchOptionsRemoteNotificationKey);
            var localNotificationInfo = launchOptions?.ObjectForKey(UIApplication.LaunchOptionsLocalNotificationKey);

            if (remoteNotificationInfo != null)
            {
                Logging.Log.LogInfo("Launch From Remote Notification");
                Console.WriteLine("BissIos: Launch remote Notification: " + remoteNotificationInfo);

                var remoteNotification = remoteNotificationInfo as NSDictionary;

                var aps = remoteNotification.ObjectForKey(new NSString("aps"));

                var apsDict = aps as NSDictionary;

                var alert = apsDict?.ObjectForKey(new NSString("alert"));

                var alertDict = alert as NSDictionary;

                var appCenter = remoteNotification.ObjectForKey(new NSString("mobile_center"));

                var appCenterDict = appCenter as NSDictionary;

                /*
                 * {
                 *      aps =
                 *          {
                 *              alert =
                 *                  {
                 *                      body = "";
                 *                      title = "";
                 *                  };
                 *          };
                 *      "mobile_center"=
                 *          {
                 *              User = "BissAdmin";
                 *              UserID = 1;
                 *          };
                 * }
                 */

                var dict = new Dictionary<string, string>();

                if (alertDict != null)
                {
                    foreach (var custData in alertDict)
                    {
                        if (custData.Key is NSString key && custData.Value is NSString value)
                        {
                            dict.Add(key.ToString(), value.ToString());
                        }
                        else
                        {
                            Logging.Log.LogError("Alert Data ist nicht ein String!");
                        }
                    }
                }

                if (appCenterDict != null)
                {
                    foreach (var custData in appCenterDict)
                    {
                        if (custData.Key is NSString key && custData.Value is NSString value)
                        {
                            dict.Add(key.ToString(), value.ToString());
                        }
                        else
                        {
                            Logging.Log.LogError("Custom Data ist nicht ein String!");
                        }
                    }
                }

                Console.WriteLine("BissIos: remote NotificationInfo: " + string.Join("; ", dict.Select(x => x.Key + ": " + x.Value)));

                try
                {
                    ProjectViewModelBase.PushReceived(dict);
                }
                catch (Exception e)
                {
                    Logging.Log.LogError($"{e}");
                }
            }

            if (localNotificationInfo != null)
            {
                Logging.Log.LogInfo("Launch From Local Notification");
                Console.WriteLine("BissIos: Launch Local Notification: " + localNotificationInfo);

                var localNotification = localNotificationInfo as UILocalNotification;

                var title = localNotification.AlertTitle;
                var body = localNotification.AlertBody;

                var userInfo = localNotification.UserInfo;

                var dict = new Dictionary<string, string>();

                dict.Add("Title", title);
                dict.Add("Body", body);

                foreach (var custData in userInfo)
                {
                    if (custData.Key is NSString key && custData.Value is NSString value)
                    {
                        dict.Add(key.ToString(), value.ToString());
                    }
                    else
                    {
                        Logging.Log.LogError("Custom Data ist nicht ein String!");
                    }
                }

                Console.WriteLine("BissIos: local NotificationInfo: " + string.Join("; ", dict.Select(x => x.Key + ": " + x.Value)));

                try
                {
                    ProjectViewModelBase.PushReceived(dict);
                }
                catch (Exception e)
                {
                    Logging.Log.LogError($"{e}");
                }
            }

            LoadApplication(new App(initializer));

            // Additional Features
            FormsMaps.Init();
            Platform.Init();

            return base.FinishedLaunching(application, launchOptions);
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        /// <summary>
        ///     Notifizierungen via Azure Mobile Center
        /// </summary>
        /// <param name="application"></param>
        /// <param name="deviceToken"></param>
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            if (Constants.AppNeedInternet)
            {
                Push.RegisteredForRemoteNotifications(deviceToken);
            }
        }

        /// <summary>
        ///     Notifizierungen via Azure Mobile Center
        /// </summary>
        /// <param name="application"></param>
        /// <param name="error"></param>
        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            if (Constants.AppNeedInternet)
            {
                Push.FailedToRegisterForRemoteNotifications(error);
            }
        }

        /// <summary>
        ///     Notifizierungen via Azure Mobile Center
        /// </summary>
        /// <param name="application"></param>
        /// <param name="userInfo"></param>
        /// <param name="completionHandler"></param>
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            if (Constants.AppNeedInternet)
            {
                var result = Push.DidReceiveRemoteNotification(userInfo);
                if (result)
                {
                    completionHandler?.Invoke(UIBackgroundFetchResult.NewData);
                }
                else
                {
                    completionHandler?.Invoke(UIBackgroundFetchResult.NoData);
                }
            }
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
        {
            if (_orientation == UIInterfaceOrientationMask.PortraitUpsideDown)
            {
            }

            return _orientation;
        }


        private void InitializeDisplayOrientations()
        {
            MessagingCenter.Subscribe<OrientationSender>(this, EnumDisplayOrientation.Unspecified.ToString(), sender =>
            {
                if (_orientation != UIInterfaceOrientationMask.All) _orientation = UIInterfaceOrientationMask.All;
            });

            MessagingCenter.Subscribe<OrientationSender>(this, EnumDisplayOrientation.Portrait.ToString(), sender =>
            {
                if (_orientation != UIInterfaceOrientationMask.Portrait) _orientation = UIInterfaceOrientationMask.Portrait;
            });

            MessagingCenter.Subscribe<OrientationSender>(this, EnumDisplayOrientation.PortraitReversed.ToString(), sender =>
            {
                if (_orientation != UIInterfaceOrientationMask.Landscape) _orientation = UIInterfaceOrientationMask.PortraitUpsideDown;
            });

            MessagingCenter.Subscribe<OrientationSender>(this, EnumDisplayOrientation.Landscape.ToString(), sender =>
            {
                if (_orientation != UIInterfaceOrientationMask.Landscape) _orientation = UIInterfaceOrientationMask.LandscapeLeft;
            });

            MessagingCenter.Subscribe<OrientationSender>(this, EnumDisplayOrientation.LandscapeReversed.ToString(), sender =>
            {
                if (_orientation != UIInterfaceOrientationMask.Landscape) _orientation = UIInterfaceOrientationMask.LandscapeRight;
            });
        }
    }
}