// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using BaseApp.ViewModel.UiModel;
using Exchange.Model;
using Exchange.PostRequests;
using Exchange.Resources;


namespace BaseApp.ViewModel
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse ViewModelAppointments. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ViewModelAppointmentInfo : ProjectViewModelBase
    {
        private ExShop _shop;

        /// <summary>
        ///     ViewModel Template
        /// </summary>
        public ViewModelAppointmentInfo() : base(ResViewAppointmentInfo.PageTitle)
        {
        }

        #region Properties

        public UiMeeting Appointment { get; private set; }

        public string InfoText { get; set; }

        public string Placeholder { get; set; }

        public VmCommand CmdAbort { get; set; }

        public VmCommand CmdScheduleAppointment { get; set; }

        #endregion

        /// <summary>
        ///     Wird aufgerufen sobald die View initialisiert wurde
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override async Task OnActivated(object args = null)
        {
            if (args is object[] argsArr)
            {
                if (argsArr[0] is UiMeeting appointment && argsArr[1] is ExShop shop)
                {
                    Appointment = appointment;
                    _shop = shop;
                    PageTitle = shop.Name;
                    var response = await Sa.GetDefaultText(appointment.Meeting.Staff.Id);

                    if (response.Ok && response.Result != null && !string.IsNullOrWhiteSpace(response.Result.PlaceholderText))
                    {
                        Placeholder = response.Result.PlaceholderText;
                    }
                    else
                    {
                        Placeholder = ResViewAppointmentInfo.LblShopAdditionalInfo;
                    }
                }
            }
            else
            {
                await Nav.Back();
            }
        }


        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdAbort = new VmCommand("", () => { Nav.ToView("ViewMain"); });
            CmdScheduleAppointment = new VmCommand("", async () =>
            {
                var response = await Sa.SetMeeting(new ExSaveMeetingRequest
                                                   {
                                                       OptionalText = InfoText,
                                                       LocationId = _shop.Id,
                                                       StartTime = Appointment.Meeting.Start,
                                                       StaffId = Appointment.Meeting.Staff.Id,
                                                       UserId = UserAccountData.UserId
                                                   });

                if (response.Ok && response.Result.Result == EnumSaveDataResult.Ok)
                {
                    Nav.ToView("ViewAppointments", response.Result.Data);
                }
                else
                {
                    await MsgBox.Show(ResViewAppointmentInfo.MsgBoxCreateAppointmentError, ResViewAppointmentInfo.MsgBoxCreateAppointmentCaption);
                }
            });
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