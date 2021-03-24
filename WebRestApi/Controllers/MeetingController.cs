// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;
using Exchange;
using Exchange.Enum;
using Exchange.Model;
using Exchange.PostRequests;
using Exchange.Resources;
using WebRestApi.Helper;
using WebRestApi.Services;

namespace WebRestApi.Controllers
{
    /// <summary>
    ///     <para>Termincontroller</para>
    ///     Klasse MeetingController. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ApiController]
    public class MeetingController : Controller
    {
        private readonly HtmlGenerator _mailgenerator;

        public MeetingController(ViewRender view)
        {
            _mailgenerator = new HtmlGenerator(view);
        }

        /// <summary>
        ///     Platzhaltertext für Terminabfrage
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/GetDefaultText/{staffId}")]
        public async Task<ExMeetingDefault> GetDefaultText(int staffId)
        {
            using (var db = new Db())
            {
                var employee = db.TblEmployees.FirstOrDefault(e => e.Id == staffId);
                ExMeetingDefault res = null;

                if (employee == null)
                {
                    res = new ExMeetingDefault
                          {
                              PlaceholderText = string.Empty,
                              FreeEmployeeId = -1
                          };
                }
                else
                {
                    res = new ExMeetingDefault
                          {
                              PlaceholderText = !string.IsNullOrWhiteSpace(employee?.DefaultAnnotation)
                                  ? employee.DefaultAnnotation
                                  : string.Empty,
                              FreeEmployeeId = -1
                          };
                }

                return res;
            }
        }

        /// <summary>
        ///     Termine eines Shops holen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/GetMeetingsForDate")]
        public async Task<List<ExMeeting>> GetMeetingsForDate([FromBody] ExGetMeetingsForDateRequest request)
        {
            if (!request.LocationId.HasValue)
                return new List<ExMeeting>();

            var res = MeetingSlots.GetSlots(request.LocationId.Value, request.Date);

            if (!request.ShowTakenSlots)
            {
                // Nur freie Termine
                res = res.Where(x => x.Id < 0).ToList();
            }

            return res.OrderBy(x => x.Start).ToList();
        }

        /// <summary>
        ///     Bild für einen Mitarbeiter laden
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/GetStaffImage/{staffId}")]
        public ActionResult GetStaffImage(int staffId)
        {
            using (var db = new Db())
            {
                var res = db.TblEmployees.FirstOrDefault(x => x.Id == staffId)?.Image
                          ?? Images.ReadImage(EmbeddedImages.DefaultAvatar_png).Image;

                return File(res, "image/png");
            }
        }

        #region MyMeetings

        /// <summary>
        ///     Termine eines Users für einen Tag holen für Apps
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/GetMyMeetingsForDate")]
        public async Task<List<ExMeeting>> GetMyMeetingsForDate([FromBody] ExGetMeetingsForDateRequest request)
        {
            if (!request.UserId.HasValue)
            {
                HttpContext.Response.StatusCode = BadRequest().StatusCode;
                return new List<ExMeeting>();
            }

            ClaimsIdentity identity = null;

            try
            {
                identity = HttpContext.User.Identity as ClaimsIdentity;
            }
            catch (Exception e)
            {
                Logging.Log.LogError("No Claims identity");
            }

            if (identity != null)
            {
                var claims = identity.Claims;

                if (!identity.HasClaim(c => c.Type == "UserID"))
                {
                    HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                    return new List<ExMeeting>();
                }

                var userId = identity.HasClaim(c => c.Type == "UserID")
                    ? identity.FindFirst("UserID").Value
                    : "a"; //BENUTZER ID

                if (request.UserId.Value.ToString() != userId)
                {
                    HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                    return new List<ExMeeting>();
                }
            }
            else
            {
                HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                return new List<ExMeeting>();
            }

            return await GetMyMeetingsForDateInternal(request);
        }

        /// <summary>
        ///     Termine eines Users für einen Tag holen für WebApp
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/GetMyMeetingsForDateWeb")]
        public async Task<List<ExMeeting>> GetMyMeetingsForDateWeb([FromBody] ExGetMeetingsForDateRequest request)
        {
            if (!request.UserId.HasValue)
            {
                HttpContext.Response.StatusCode = BadRequest().StatusCode;
                return new List<ExMeeting>();
            }

            if (request.CheckPassword != WebAppSettings.CheckPassword)
            {
                HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                return new List<ExMeeting>();
            }

            return await GetMyMeetingsForDateInternal(request);
        }

        /// <summary>
        ///     Termine eines Users für einen Tag holen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<List<ExMeeting>> GetMyMeetingsForDateInternal(ExGetMeetingsForDateRequest request)
        {
            using (var db = new Db())
            {
                var res = new List<ExMeeting>();

                var meetings = db.TblAppointments
                    .Include(x => x.Employee)
                    .Include(x => x.Employee).ThenInclude(x => x.Store)
                    .Include(x => x.Employee).ThenInclude(x => x.TblLocationEmployee)
                    .AsNoTracking()
                    .Where(x => x.UserId == request.UserId.Value
                                && !x.Canceled
                                && (x.ValidFrom.Date == request.Date.Date || x.ValidTo.Date == request.Date.Date));

                foreach (var dbMeeting in meetings)
                {
                    var meeting = new ExMeeting
                                  {
                                      Id = dbMeeting.Id,

                                      // TODO ShopId ist eigentlich auf Location und ein MA kann bei mehreren Locations sein, also beim Meeting Location auch dazu?
                                      ShopId = dbMeeting.Employee?.TblLocationEmployee?.FirstOrDefault()?.TblLocationId ??
                                               dbMeeting.Employee?.StoreId ?? -1,

                                      ShopName = dbMeeting.Employee?.Store?.CompanyName ?? "-",
                                      Staff = Staff.GetExStaff(dbMeeting.Employee),
                                      UserId = dbMeeting.UserId,
                                      Start = dbMeeting.ValidFrom,
                                      End = dbMeeting.ValidTo,
                                  };

                    res.Add(meeting);
                }

                return res.OrderBy(x => x.Start).ToList();
            }
        }

        #endregion

        #region SetMeeting

        /// <summary>
        ///     Termin vereinbaren Apps
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/SetMeeting")]
        public async Task<ExSaveDataResult<ExMeeting>> SetMeeting([FromBody] ExSaveMeetingRequest request)
        {
            ClaimsIdentity identity = null;

            try
            {
                identity = HttpContext.User.Identity as ClaimsIdentity;
            }
            catch (Exception e)
            {
                Logging.Log.LogError("No Claims identity");
            }

            if (identity != null)
            {
                var claims = identity.Claims;

                if (!identity.HasClaim(c => c.Type == "UserID"))
                {
                    HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                    return null;
                }

                var userId = identity.HasClaim(c => c.Type == "UserID")
                    ? identity.FindFirst("UserID").Value
                    : "a"; //BENUTZER ID

                if (request.UserId.ToString() != userId)
                {
                    HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                    return null;
                }
            }
            else
            {
                HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                return null;
            }

            return await SetMeetingInternal(request);
        }

        /// <summary>
        ///     Termin vereinbaren WebApp
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/SetMeetingWeb")]
        public async Task<ExSaveDataResult<ExMeeting>> SetMeetingWeb([FromBody] ExSaveMeetingRequest request)
        {
            if (request.CheckPassword != WebAppSettings.CheckPassword)
            {
                HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                return null;
            }

            return await SetMeetingInternal(request);
        }

        /// <summary>
        ///     Termin vereinbaren
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<ExSaveDataResult<ExMeeting>> SetMeetingInternal([FromBody] ExSaveMeetingRequest request)
        {
            using (var db = new Db())
            {
                var freeEmployeeId = request.StaffId;

                if (freeEmployeeId == null)
                {
                    var employees = db.TblLocations
                        .Include(x => x.TblLocationEmployee)
                        .Include(x => x.TblLocationEmployee).ThenInclude(x => x.TblEmployee)
                        .AsNoTracking()
                        .FirstOrDefault(x => x.Id == request.LocationId)?.TblLocationEmployee;

                    // TODO Freien Mitarbeiter suchen
                    var employeesFree = employees?.Where(x => x.TblEmployee.Active);

                    freeEmployeeId = employeesFree?.FirstOrDefault()?.Id;
                }

                if (freeEmployeeId == null)
                {
                    return new ExSaveDataResult<ExMeeting>
                           {
                               Description = "Leider wurde kein freier Verkäufer gefunden!",
                               Caption = "Kein freier Termin",
                               Result = EnumSaveDataResult.Error,
                               Data = null,
                           };
                }

                // prüfen ob der Mitarbeiter (noch) Zeit hat
                var locationEmployee = db.TblEmployees
                    .Include(x => x.TblVirtualWorkTimes)
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == freeEmployeeId.Value);

                var slotTime = locationEmployee.TblVirtualWorkTimes.FirstOrDefault(x => x.Weekday == request.StartTime.Date.DayOfWeek);

                if (slotTime == null)
                {
                    return new ExSaveDataResult<ExMeeting>
                           {
                               Description = "Leider hat der Verkäufer heute frei.",
                               Caption = "Kein Arbeitstag",
                               Result = EnumSaveDataResult.Error,
                               Data = null,
                           };
                }

                var meeting = db.TblAppointments.AsNoTracking()
                    .FirstOrDefault(x => x.EmployeeId == locationEmployee.Id
                                         && x.ValidTo > request.StartTime
                                         && x.ValidFrom <= request.StartTime
                                         && !x.Canceled);

                if (meeting != null)
                {
                    return new ExSaveDataResult<ExMeeting>
                           {
                               Description = "Der gewünschte Verkäufer hat zu diesem Zeitpunkt leider bereits einen Termin.",
                               Caption = "Terminkonflikt",
                               Result = EnumSaveDataResult.Error,
                               Data = null,
                           };
                }

                var dbMeeting = new TableAppointment
                                {
                                    UserId = request.UserId,
                                    EmployeeId = freeEmployeeId.Value,
                                    Text = request.OptionalText,
                                    BookedOn = DateTime.UtcNow,
                                    ValidFrom = request.StartTime,
                                    ValidTo = request.StartTime.AddMinutes(slotTime?.TimeSlot ?? 1),
                                    Attended = false,
                                    Canceled = false,

                                    AppointmentDate = request.StartTime.Date,
                                };

                db.TblAppointments.Add(dbMeeting);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Logging.Log.LogError($"{e}");
                    return new ExSaveDataResult<ExMeeting>
                           {
                               Description = ExSaveDataResult.GetDefaultSaveError().Description,
                               Caption = ExSaveDataResult.GetDefaultSaveError().Caption,
                               Result = EnumSaveDataResult.Error,
                               Data = null,
                           };
                }

                var shop = db.TblLocations
                    .Include(x => x.Store)
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == request.LocationId);

                var res = new ExSaveDataResult<ExMeeting>
                          {
                              Result = EnumSaveDataResult.Ok,
                              Caption = ExSaveDataResult.GetDefaultSuccess().Caption,
                              Description = ExSaveDataResult.GetDefaultSuccess().Description,
                              Data = new ExMeeting
                                     {
                                         Id = dbMeeting.Id,
                                         Start = dbMeeting.ValidFrom,
                                         End = dbMeeting.ValidTo,
                                         ShopId = shop.Id,
                                         ShopName = shop.Store.CompanyName,
                                         Staff = Staff.GetExStaff(db, dbMeeting.EmployeeId),
                                         UserId = dbMeeting.UserId,
                                     }
                          };

                //Shop per E-Mail über neuen Termin informieren
                try
                {
                    var meetingInfo = db.TblAppointments
                        .Include(x => x.Employee)
                        .Include(x => x.Employee).ThenInclude(x => x.Store)
                        .Include(x => x.User)
                        .FirstOrDefault(x => x.Id == dbMeeting.Id);

                    string shopEMail = meetingInfo.Employee.Store.EMail;
                    string userName = $"{meetingInfo.User.Firstname} {meetingInfo.User.Lastname}";
                    string emailContent = $"Neuer Termin mit {(String.IsNullOrEmpty(userName.Trim()) ? "UNBEKANNT" : userName)} am {meetingInfo.ValidFrom.AddHours(2):dd.MM.yyy} um {meetingInfo.ValidFrom.AddHours(2):HH:mm}.";

                    string email = _mailgenerator.GetMessageOnlyEmail(new ExEMailMessageOnly
                                                                      {
                                                                          Message = emailContent
                                                                      });
                    BissEMail bm = new BissEMail(WebAppSettings.EmailCredentials);
                    await bm.SendHtmlEMail(Constants.SendEMailAs, new List<string> {shopEMail}, "Neuer Termin", email, Constants.SendEMailAsDisplayName);
                }
                catch (Exception e)
                {
                    Logging.Log.LogWarning($"E-Mail über neuen Termin konnte nicht gesendet werden: {e}");
                    throw;
                }

                return res;
            }
        }

        #endregion

        #region DeleteMeeting

        /// <summary>
        ///     Termin entfernen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/DeleteMeeting")]
        public async Task<ExSaveDataResult> DeleteMeeting([FromBody] ExRemoveMeetingRequest request)
        {
            var authfailed = true;

            ClaimsIdentity identity = null;

            try
            {
                identity = HttpContext.User.Identity as ClaimsIdentity;
            }
            catch (Exception e)
            {
                Logging.Log.LogError("No Claims identity");
            }

            if (identity != null)
            {
                var claims = identity.Claims;

                if (identity.HasClaim(c => c.Type == "UserID"))
                {
                    var userId = identity.HasClaim(c => c.Type == "UserID")
                        ? identity.FindFirst("UserID").Value
                        : "a"; //BENUTZER ID

                    if (request.UserId.ToString() == userId)
                    {
                        authfailed = false;
                    }
                }
            }

            if (authfailed)
            {
                if (request.CheckPassword != WebAppSettings.CheckPassword)
                {
                    HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                    return null;
                }
            }

            return await DeleteMeetingInternal(request);
        }

        /// <summary>
        ///     Termin entfernen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/DeleteMeetingWeb")]
        public async Task<ExSaveDataResult> DeleteMeetingWeb([FromBody] ExRemoveMeetingRequest request)
        {
            if (request.CheckPassword != WebAppSettings.CheckPassword)
            {
                HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                return null;
            }

            return await DeleteMeetingInternal(request);
        }

        /// <summary>
        ///     Termin entfernen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<ExSaveDataResult> DeleteMeetingInternal([FromBody] ExRemoveMeetingRequest request)
        {
            using (var db = new Db())
            {
                var meeting = db.TblAppointments
                    .Include(x => x.Employee)
                    .Include(x => x.Employee).ThenInclude(x => x.Store)
                    .Include(x => x.User)
                    .FirstOrDefault(x => x.Id == request.MeetingId);

                if (meeting.UserId != request.UserId && request.UserType == EnumUserType.Customer)
                {
                    return new ExSaveDataResult
                           {
                               Result = EnumSaveDataResult.Error,
                               Caption = "Nicht möglich!",
                               Description = "Nur der Ersteller des Termins kann diesen auch wieder löschen!"
                           };
                }

                meeting.Canceled = true;
                try
                {
                    switch (request.UserType)
                    {
                        //Kunde hat den Termin storniert => E-Mail an Shop als Info
                        case EnumUserType.Customer:
                            string shopEMail = meeting.Employee.Store.EMail;
                            string userName = $"{meeting.User.Firstname} {meeting.User.Lastname}";
                            string emailContent = $"Leider hat {(String.IsNullOrEmpty(userName.Trim()) ? "UNBEKANNT" : userName)} den Termin am {meeting.ValidFrom.AddHours(2):dd.MM.yyy} um {meeting.ValidFrom.AddHours(2):HH:mm} absagen müssen.";
                            string employeeTel = meeting.Employee.TelephoneNumber;

                            string email = _mailgenerator.GetStornoAppointmentShopEmail(new ExEMailStornoAppointmentShop
                                                                                        {
                                                                                            Message = emailContent
                                                                                        });
                            BissEMail bm = new BissEMail(WebAppSettings.EmailCredentials);
                            var res = await bm.SendHtmlEMail(Constants.SendEMailAs, new List<string> {shopEMail}, "Termin-Storno", email, Constants.SendEMailAsDisplayName);

                            //ToDo: Umstellung auf Push sobald möglich
                            //Common.SendSMS(employeeTel, $"{emailContent} Auf diese SMS kann nicht geantwortet werden.");

                            break;


                        //Shop musst den Termin stonieren => SMS an Kunden als Info
                        //ToDo: Umstellung auf Push sobald möglich
                        case EnumUserType.ShopEmployee:
                            string telCustomer = meeting.User.PhoneNumber;
                            string employeeName = $"{meeting.Employee.FirstName} {meeting.Employee.LastName}";
                            string smsContent = $"Leider hat {(String.IsNullOrEmpty(employeeName.Trim()) ? "UNBEKANNT" : employeeName)} von {meeting.Employee.Store.CompanyName} den Termin am {meeting.ValidFrom.AddHours(2):dd.MM.yyy} um {meeting.ValidFrom.AddHours(2):HH:mm} absagen müssen. Du kannst gerne wieder einen neuen Termin über die App vereinbaren. Auf diese SMS kann nicht geantwortet werden.";
                            Common.SendSMS(telCustomer, smsContent);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception e)
                {
                    Logging.Log.LogWarning($"Could not inform customer or shop about chanceled appointment: {e}");
                }

                try
                {
                    await db.SaveChangesAsync();
                    return ExSaveDataResult.GetDefaultSuccess();
                }
                catch (Exception e)
                {
                    Logging.Log.LogError($"{e}");
                    return ExSaveDataResult.GetDefaultSaveError();
                }
            }
        }

        #endregion
    }
}