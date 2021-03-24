// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange;
using Exchange.Model;
using Exchange.Resources;

namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>XXXX</para>
    ///     Klasse XXX. (C) 2016 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelDeveloperInfo : ProjectViewModelBase
    {
        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelDeveloperInfo() : base(ResViewDeveloperInfo.Title, subTitle: ResViewDeveloperInfo.Subtitle)
        {
        }

        #region Properties

        public ExBaseInfo Data { get; set; } = new ExBaseInfo();

        public bool ShowInternetInfo => Constants.AppNeedInternet;

        public VmCommand CmdTestIsBusy { get; set; }

        public VmCommand CmdSendEmail { get; set; }

        #endregion

        /// <summary>
        ///     Wird aufgerufen sobald die View initialisiert wurde
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override Task OnActivated(object args = null)
        {
            Update();

            Connectivity.ConnectivityChanged += (sender, eventArgs) => UpdateConnectivityInfo();
            Connectivity.ConnectivityTypeChanged += (sender, eventArgs) => UpdateConnectivityInfo();

            return Task.CompletedTask;
        }

        public void UpdateDeviceInfo()
        {
            Data.DeviceHardwareId = DeviceInfo.DeviceHardwareId;
            Data.DeviceType = DeviceInfo.DeviceType;
            Data.DeviceIdiom = DeviceInfo.DeviceIdiom.ToString();
            Data.DevicePlattform = DeviceInfo.Plattform.ToString();
            Data.OperatingSystemVersion = DeviceInfo.OperatingSystemVersion;
        }

        public void UpdateInternetInfo()
        {
            if (this.BcPush().PushDeviceGuid != null)
                Data.AppCenterId = this.BcPush().PushDeviceGuid.ToString();

            if (Constants.DefaultWebEndpoint != null)
                Data.RestUrl = Constants.DefaultWebEndpoint;

            if (Constants.DefaulSignalREndpoint != null)
                Data.SignalRBaseUrl = Constants.DefaulSignalREndpoint;
        }

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdTestIsBusy = new VmCommand(ResViewDeveloperInfo.CmdBusyTest, async () =>
            {
                IsBusy = true;
                BusyContent = string.Format(ResViewDeveloperInfo.BusyLoadingText, 25);
                await Task.Delay(1000);
                BusyContent = string.Format(ResViewDeveloperInfo.BusyLoadingText, 50);
                await Task.Delay(1000);
                BusyContent = string.Format(ResViewDeveloperInfo.BusyLoadingText, 75);
                await Task.Delay(1000);
                BusyContent = string.Format(ResViewDeveloperInfo.BusyLoadingText, 100);
                await Task.Delay(1000);
                BusyContent = ResViewDeveloperInfo.BusyCompleteText;
                await Task.Delay(1000);
                IsBusy = false;
            });


            CmdSendEmail = new VmCommand("", async () =>
            {
                try
                {
                    var json = Data.ToJson();
                    var path = Path.Combine(FileSystem.CacheDirectory, "DeviceInfo.Json");
                    File.WriteAllText(path, json);


                    var attachment = new EmailAttachment(path);

                    var message = new EmailMessage
                                  {
                                      Subject = "DeviceInformation Biss Demo App",
                                      Body = "Attached is a json file containing the device information."
                                  };
                    message.To.Add("biss@fotec.at");
                    message.Attachments.Add(attachment);
                    await Email.ComposeAsync(message);
                }
                catch
                {
                    await MsgBox.Show(ResViewDeveloperInfo.MsgBoxEmailErrorText, ResViewDeveloperInfo.MsgBoxEmailErrorHeader);
                }
            });
        }

        private void Update()
        {
            UpdateConnectivityInfo();
            UpdateLanguageInfo();
            UpdateDeviceInfo();
            if (Constants.AppNeedInternet)
                UpdateInternetInfo();
        }

        private void UpdateConnectivityInfo()
        {
            Data.Internet = Connectivity.IsConnected ? ResViewDeveloperInfo.LblConnectivityConnected : ResViewDeveloperInfo.LblConnectivityNotConnected;
            Data.Bandwidths = string.Join(",", Connectivity.Bandwidths);
            Data.ConnectionTypes = string.Join(",", Connectivity.ConnectionTypes);
        }

        private void UpdateLanguageInfo()
        {
            Data.AppLanguage = Language.CurrentCulture.DisplayName;
            Data.DeviceLanguage = Language.CurrentDeviceCulture.DisplayName;
        }
    }
}