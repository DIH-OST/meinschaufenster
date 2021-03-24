// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class ModalController : Controller
    {
        public async Task<IActionResult> Whatsapp(int style, string phone)
        {
            WhatsAppViewModel vm = new WhatsAppViewModel();
            vm.PhoneNumber = phone;
            vm.Style = (EnumModalStyle) style;
            vm.Text = "Drücke auf Videoberatung und starte anschließend das Videotelefonat wie im Bild ersichtlich. Bitte beachte, dass du hierfür die App \"WhatsApp\" auf deinem Telefon installiert haben musst.";

            return PartialView(vm);
        }
    }
}