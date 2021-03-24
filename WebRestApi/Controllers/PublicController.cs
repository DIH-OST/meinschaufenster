// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Exchange;
using Exchange.Model;

namespace WebRestApi.Controllers
{
    /// <summary>
    ///     <para>PublicController</para>
    ///     Klasse PublicController. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [AllowAnonymous]
    public class PublicController : Controller
    {
        /// <summary>
        ///     Infos für die Apps holen
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/GetAppInfos/")]
        [HttpGet]
        public async Task<ExAppInfo> GetAppInfos()
        {
            return new ExAppInfo
                   {
                       EMail = "info@meinschaufenster.at",
                       UrlToEula = $"{Constants.WebAppBaseUrl}assets/AGB_Datenschutz.pdf",
                       UrlToInfoMaterial = $"{Constants.WebAppBaseUrl}assets/Info_Kunden.pdf",
                       UrlToMerchantVideo = "https://www.youtube.com/watch?v=eEqPVhu3IhI",
                       UrlToCustomerVideo = "https://www.youtube.com/watch?v=Y9Bh93UPlFU"
                   };
        }

        /// <summary>
        ///     Infos für die Apps holen
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/GetMaintenanceInfo/")]
        [HttpGet]
        public async Task<string> GetMaintenanceInfo()
        {
            using (var db = new Db())
            {
                string result;
                try
                {
                    result = await db.TblStaticSettings.Select(s => s.MaintenanceInfo).FirstOrDefaultAsync();
                }
                catch
                {
                    return "";
                }

                return result;
            }
        }
    }
}