// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange;
using Exchange.Model;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ViewModelAppInformation. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelAppInformation : ProjectViewModelBase
    {
        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelAppInformation() : base("Schau Infos!")
        {
        }

        #region Properties

        public ImageSource Logo { get; set; } = ImageSource.FromStream(() => Images.ReadImageAsStream(EmbeddedImages.AppIconTransparent_png));

        public ExAppInfo AppInfo { get; set; }

        public string Version { get; set; } = $"Version: {AppSettings.Current().AppVersion}";

        public string AppText { get; set; } = "Die App, die alle regionalen Geschäfte anzeigt und ein persönliches Verkaufsgespräch über Videotelefonie mit WhatsApp ermöglicht.";

        public VmCommand CmdOpenEula { get; set; }

        public VmCommand CmdOpenEmail { get; set; }

        public VmCommand CmdOpenAdditionalInfo { get; set; }

        public VmCommand CmdOpenYoutube { get; set; }

        #endregion

        /// <summary>
        ///     Wird aufgerufen sobald die View initialisiert wurde
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override async Task OnActivated(object args = null)
        {
            var data = await Sa.GetAppInfo();
            if (data.Ok && data.Result != null)
                AppInfo = data.Result;
        }


        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdOpenAdditionalInfo = new VmCommand("", () =>
            {
                if (!string.IsNullOrWhiteSpace(AppInfo.UrlToInfoMaterial))
                {
                    Browser.OpenAsync(AppInfo.UrlToInfoMaterial, BrowserLaunchMode.External);
                    //OpenUri(AppInfo.UrlToInfoMaterial);
                }
            });

            CmdOpenEmail = new VmCommand("", () =>
            {
                if (!string.IsNullOrWhiteSpace(AppInfo.EMail))
                {
                    if (!AppInfo.EMail.StartsWith("mailto:"))
                        AppInfo.EMail = "mailto:" + AppInfo.EMail;

                    OpenUri(AppInfo.EMail);
                }
            });

            CmdOpenEula = new VmCommand("", () =>
                {
                    if (!string.IsNullOrWhiteSpace(AppInfo.UrlToEula))
                        Browser.OpenAsync(AppInfo.UrlToEula, BrowserLaunchMode.External);
                    //  OpenUri(AppInfo.UrlToEula);
                }
            );

            CmdOpenYoutube = new VmCommand("", () => { Browser.OpenAsync(AppInfo.UrlToCustomerVideo, BrowserLaunchMode.External); });
        }
    }
}