// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Exchange;

namespace BaseApp.ViewModel
{
    public class CustData
    {
        #region Properties

        public string Key { get; set; }
        public string Value { get; set; }

        #endregion
    }

    /// <summary>
    ///     <para>PushInfo - von Push kommend</para>
    ///     Klasse ViewModelPushInfo. (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelPushInfo : ProjectViewModelBase
    {
        /// <summary>
        ///     ViewModelPushInfo
        /// </summary>
        public ViewModelPushInfo() : base("PushInfo", subTitle: "nix")
        {
            IsBusy = true;
        }

        #region Properties

        public ObservableCollection<CustData> Data { get; set; }

        /// <summary>
        ///     Test Command
        /// </summary>
        public VmCommand CmdPing { get; set; }

        #endregion

        /// <summary>
        ///     Wird aufgerufen sobald die View initialisiert wurde
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override Task OnActivated(object args = null)
        {
            var data = new ObservableCollection<CustData>();

            Logging.Log.LogInfo($"PushInfo OnActivated: {args}");

            if (args is IDictionary<string, string> customData)
            {
                foreach (var custData in customData)
                {
                    data.Add(new CustData
                             {
                                 Key = custData.Key,
                                 Value = custData.Value,
                             });
                }
            }

            Data = data;

            PageSubTitle = data.FirstOrDefault(x => x.Key == "Body")?.Value ?? "Notification";

            Logging.Log.LogInfo("PushInfo OnActivated: " + string.Join("; ", Data.Select(x => x.Key + ": " + x.Value)));

            IsBusy = false;
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdPing = new VmCommand("Ping", async () =>
            {
                var r = await Sa.GetPing();
                if (await this.CheckRestStatusResult(r.Status))
                {
                    ToastShow("Ping Ok!", Constants.MainTitle);
                }
            }, CanExecutCmdPing);
        }

        /// <summary>
        ///     Validierung ob CommandLogin gedrückt werden kann
        /// </summary>
        /// <returns></returns>
        private bool CanExecutCmdPing()
        {
            return true;
        }
    }
}