// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using System;
using Exchange.Resources;


namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ViewModelWhatsAppTutorial. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelWhatsAppTutorial : ProjectViewModelBase
    {
        private string _number;

        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelWhatsAppTutorial() : base("WhatsApp")
        {
        }

        #region Properties

        public ImageSource WhatsApp => ImageSource.FromStream(() => Images.ReadImageAsStream(EmbeddedImages.WhatsAppAnleitung_png));

        public VmCommand CmdContinue { get; set; }

        #endregion

        /// <summary>
        ///     Wird aufgerufen sobald die View initialisiert wurde
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override Task OnActivated(object args = null)
        {
            if (!(args is string number))
            {
                Nav.Back();
                return Task.CompletedTask;
            }

            _number = number;
            return Task.CompletedTask;
        }


        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdContinue = new VmCommand("", () => { OpenUri(string.Format(ResView.CmdOpenWhatsAppUrl, _number)); });
        }
    }
}