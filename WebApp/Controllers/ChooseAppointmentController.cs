// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;
using Exchange;
using Exchange.Enum;
using Exchange.PostRequests;
using Exchange.ServiceAccess;
using WebApp.Models;
using WebRestApi.Helper;

namespace WebApp.Controllers
{
    [Authorize(Roles = "User")]
    public class ChooseAppointmentController : Controller
    {
        private readonly Db _context;

        public ChooseAppointmentController(Db context)
        {
            _context = context;
        }

        // GET: ChooseAppointment
        // A simple View is returned
        // The Grid will asyc call the ChooseAppointment_Read action on page load
        public async Task<IActionResult> Index(int storeId)
        {
            if (TempData["message"] != null) ViewData["message"] = TempData["message"];
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (String.IsNullOrEmpty(role) || role != "User")
            {
                return RedirectToAction("LogInUser", "Account");
            }

            ChooseAppointmentModel model = new ChooseAppointmentModel();
            model.StoreId = storeId;
            model.FilterFrom = DateTime.Now.AddHours(2);

            var store = (await _context.TblLocations.Include("Store").FirstOrDefaultAsync(x => x.Id == storeId));
            ViewData["CompanyName"] = store?.Store?.CompanyName ?? "";
            ViewData["storeId"] = storeId;
            return View(model);
        }

        public async Task<IActionResult> TblAppointments_Read([DataSourceRequest] DataSourceRequest request, string storeId, string filterFromDate)
        {
            int sId = int.Parse(storeId);

            DateTime datFilterFromDate = DateTime.Now;
            bool success = DateTime.TryParse(filterFromDate, CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.None, out datFilterFromDate);

            RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);

            var shop = await ra.GetMeetingsForDate(sId, datFilterFromDate.Date);
            var slots = shop.Result.Where(a => a.Id == -1).ToList();

            List<AppointmentEntryViewModel> lstAppointmentEntryViewModels = new List<AppointmentEntryViewModel>();
            int id = 1;

            foreach (var exMeeting in slots)
            {
                var employee = _context.TblEmployees.First(x => x.Id == exMeeting.Staff.Id);

                AppointmentEntryViewModel vm = new AppointmentEntryViewModel();
                vm.Id = id++;
                vm.EmployeeName = exMeeting.Staff.Name;
                vm.EndTime = exMeeting.End.AddHours(2);
                vm.OptionalText = "";
                vm.PreviewText = employee.DefaultAnnotation;
                vm.ShopId = sId;
                vm.StaffId = exMeeting.Staff.Id;
                vm.StartTime = exMeeting.Start.AddHours(2);
                vm.ImageUrl = exMeeting.Staff.ImageUrl ?? "";

                lstAppointmentEntryViewModels.Add(vm);
            }

            //IEnumerable<AppointmentEntryViewModel> tmpLst = lstAppointmentEntryViewModels;

            return Json(await lstAppointmentEntryViewModels.ToDataSourceResultAsync(request));
        }

        //[HttpPost]
        public async Task<IActionResult> TblAppointments_ReadJson(string storeId, string filterFromDate) //string storeId, string filterFromDate)
        {
            int sId = int.Parse(storeId);

            DateTime datFilterFromDate = DateTime.Now;
            bool success = DateTime.TryParse(filterFromDate, CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.None, out datFilterFromDate);

            RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);

            var shop = await ra.GetMeetingsForDate(sId, datFilterFromDate.Date);
            var slots = shop.Result.Where(a => a.Id == -1).ToList();

            List<AppointmentEntryViewModel> lstAppointmentEntryViewModels = new List<AppointmentEntryViewModel>();
            int id = 1;

            foreach (var exMeeting in slots)
            {
                var employee = _context.TblEmployees.First(x => x.Id == exMeeting.Staff.Id);
                AppointmentEntryViewModel vm = new AppointmentEntryViewModel();
                vm.Id = id++;
                vm.EmployeeName = exMeeting.Staff.Name;
                vm.StartTime = exMeeting.Start.AddHours(2);
                vm.EndTime = exMeeting.End.AddHours(2);
                vm.ShopId = sId;
                vm.OptionalText = "";
                vm.PreviewText = employee.DefaultAnnotation;
                vm.StaffId = exMeeting.Staff.Id;
                vm.ImageUrl = string.IsNullOrEmpty(exMeeting.Staff.ImageUrl) ? "" : exMeeting.Staff.ImageUrl;
                lstAppointmentEntryViewModels.Add(vm);
            }

            return Json(lstAppointmentEntryViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> TblAppointments_ReadJson(ChooseAppointmentModelV2 model) //string storeId, string filterFromDate)
        {
            int sId = model.storeId;

            DateTime datFilterFromDate = DateTime.Now;
            bool success = DateTime.TryParse(model.filterFromDate, CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.None, out datFilterFromDate);

            RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);

            var shop = await ra.GetMeetingsForDate(sId, datFilterFromDate.Date);
            var slots = shop.Result.Where(a => a.Id == -1).ToList();

            List<AppointmentEntryViewModel> lstAppointmentEntryViewModels = new List<AppointmentEntryViewModel>();
            int id = 1;

            foreach (var exMeeting in slots)
            {
                AppointmentEntryViewModel vm = new AppointmentEntryViewModel();
                vm.Id = id++;
                vm.EmployeeName = exMeeting.Staff.Name;
                vm.EndTime = exMeeting.End.AddHours(2);
                vm.OptionalText = "";
                vm.PreviewText = "";
                vm.ShopId = sId;
                vm.StaffId = exMeeting.Staff.Id;
                vm.StartTime = exMeeting.Start.AddHours(2);
                vm.ImageUrl = exMeeting.Staff.ImageUrl ?? "";

                lstAppointmentEntryViewModels.Add(vm);
            }

            return Json(lstAppointmentEntryViewModels);
        }

        // GET: ChooseAppointment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableAppointment = await _context.TblAppointments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableAppointment == null)
            {
                return NotFound();
            }

            return View(tableAppointment);
        }

        // GET: ChooseAppointment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChooseAppointment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text,Canceled,Attended,BookedOn,ValidFrom,ValidTo,UserId,EmployeeId")]
            TableAppointment tableAppointment)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(tableAppointment);
                //await _context.SaveChangesAsync();

                //var response = await Sa.SetMeeting(new ExSaveMeetingRequest
                //                                   {
                //                                       //OptionalText = InfoText,
                //                                       //ShopId = _shop.Id,
                //                                       //StartTime = Appointment.Meeting.Start,
                //                                       //StaffId = Appointment.Meeting.Staff.Id,
                //                                       //UserId = 1
                //                                   });


                return RedirectToAction(nameof(Index));
            }

            return View(tableAppointment);
        }

        // GET: ChooseAppointment/Edit/5
        public async Task<IActionResult> Edit(int? id, string storeId, string selectedDate)
        {
            if (TempData["message"] != null) ViewData["message"] = TempData["message"];

            if (id == null)
            {
                return NotFound();
            }

            int sId = int.Parse(storeId); // this is the location id... 

            DateTime datSelectedDate = DateTime.Now.ToLocalTime();
            try
            {
                var ddatSelectedDate = DateTime.Parse(selectedDate, CultureInfo.GetCultureInfo("de-DE"));
                datSelectedDate = ddatSelectedDate;
            }
            catch (Exception e)
            {
                return Json("false");
            }

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var tbUser = await _context.TblUsers.AsNoTracking().FirstOrDefaultAsync(a => a.Id == userId);
            RestAccess ra = new RestAccess(tbUser.Id.ToString(), tbUser.RestPasswort, Constants.ServiceClientEndPointWithApiPrefix);

            var shop = await ra.GetMeetingsForDate(sId, datSelectedDate.Date.AddHours(2));
            var slots = shop.Result.Where(a => a.Id == -1).ToList();

            List<AppointmentEntryViewModel> lstAppointmentEntryViewModels = new List<AppointmentEntryViewModel>();

            var exMeeting = slots[id.Value - 1];

            AppointmentEntryViewModel vm = new AppointmentEntryViewModel();
            vm.Id = 0;
            vm.EmployeeName = exMeeting.Staff.Name;
            vm.EndTime = exMeeting.End;
            vm.OptionalText = "";
            var res = await ra.GetDefaultText(exMeeting.Staff.Id);
            if (res != null && res.Result != null)
            {
                vm.PreviewText = res.Result.PlaceholderText;
            }
            else vm.PreviewText = "";

            vm.ShopId = sId;
            vm.StaffId = exMeeting.Staff.Id;
            vm.StartTime = exMeeting.Start;

            return View(vm);
        }

        // POST: ChooseAppointment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OptionalText,ShopId,StartTime,EndTime,StaffId")]
            AppointmentEntryViewModel app)
        {
            //if (id != tableAppointment.Id)
            //{
            //    return NotFound();
            //}

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var tbUser = await _context.TblUsers.AsNoTracking().FirstOrDefaultAsync(a => a.Id == userId);
            RestAccess ra = new RestAccess(tbUser.Id.ToString(), tbUser.RestPasswort,
                Constants.ServiceClientEndPointWithApiPrefix);

            var response = await ra.SetMeetingWeb(new ExSaveMeetingRequest
                                                  {
                                                      OptionalText = app.OptionalText,
                                                      LocationId = app.ShopId,
                                                      StartTime = app.StartTime.ToUniversalTime(),
                                                      StaffId = app.StaffId,
                                                      UserId = userId,
                                                      CheckPassword = WebAppSettings.CheckPassword,
                                                  });

            if (response.Ok && response.Result != null)
            {
                if (response.Result.Result == EnumSaveDataResult.Error)
                {
                    ViewData["message"] = response.Result.Description;
                    return View(app);
                }

                TempData["message"] = "Dein Termin wurde erfolgreich bestätigt.";
                return RedirectToAction("Index", "MyAppointments");
            }

            ViewData["message"] = "Es konnte keine Verbindung zum Server hergestellt werden.";
            return View(app);
        }

        // GET: ChooseAppointment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableAppointment = await _context.TblAppointments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableAppointment == null)
            {
                return NotFound();
            }

            return View(tableAppointment);
        }

        // POST: ChooseAppointment/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var tbUser = await _context.TblUsers.AsNoTracking().FirstOrDefaultAsync(a => a.Id == userId);
            RestAccess ra = new RestAccess(tbUser.Id.ToString(), tbUser.RestPasswort, Constants.ServiceClientEndPointWithApiPrefix);
            await ra.DeleteMeetingWeb(new ExRemoveMeetingRequest {MeetingId = id, UserType = EnumUserType.Customer, UserId = userId, CheckPassword = WebAppSettings.CheckPassword});

            return RedirectToAction(nameof(Index));
        }

        private bool TableAppointmentExists(int id)
        {
            return _context.TblAppointments.Any(e => e.Id == id);
        }
    }
}