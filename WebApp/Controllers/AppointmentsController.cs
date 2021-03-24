// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;
using Exchange;
using Exchange.Enum;
using Exchange.Model;
using Exchange.PostRequests;
using Exchange.ServiceAccess;
using WebApp.Models;
using WebRestApi.Helper;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Shop")]
    public class AppointmentsController : Controller
    {
        private readonly Db _context;

        public AppointmentsController(Db context)
        {
            _context = context;
        }

        // GET: Appointments
        // A simple View is returned
        // The Grid will asyc call the Appointments_Read action on page load
        public async Task<IActionResult> Index()
        {
            AppointmentViewModel model = new AppointmentViewModel();
            model.FilterFrom = DateTime.Now.AddHours(2);

            return View(model);
        }

        public async Task<IActionResult> TblAppointments_Read([DataSourceRequest] DataSourceRequest request, string filterFromDate)
        {
            DateTime datFilterFromDate = DateTime.Now;
            bool success = DateTime.TryParse(filterFromDate, CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.None, out datFilterFromDate);

            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var employs = await _context.TblEmployees.Where(a => a.StoreId == storeId).Select(a => a.Id).ToListAsync();

            List<AppointmentModel> lstApps = new List<AppointmentModel>();

            if (success)
            {
                return Json(await _context.TblAppointments.Include(a => a.User).Where(a => !a.Canceled && employs.Contains(a.EmployeeId) && a.ValidFrom.Date == datFilterFromDate.Date)
                    .Select(a => new AppointmentModel {Id = a.Id, Text = a.Text, Name = $"{a.User.Firstname} {a.User.Lastname}", Phonenumber = a.User.PhoneNumber, ValidFrom = a.ValidFrom.AddHours(2), ValidTo = a.ValidTo.AddHours(2)})
                    .OrderBy(x => x.ValidFrom)
                    .ToDataSourceResultAsync(request));
            }

            return Json(await _context.TblAppointments.Include(a => a.User).Where(a => !a.Canceled && employs.Contains(a.EmployeeId))
                .Select(a => new AppointmentModel {Id = a.Id, Text = a.Text, Name = $"{a.User.Firstname} {a.User.Lastname}", Phonenumber = a.User.PhoneNumber, ValidFrom = a.ValidFrom.AddHours(2), ValidTo = a.ValidTo.AddHours(2)})
                .OrderBy(x => x.ValidFrom).ToDataSourceResultAsync(request));
        }

        public async Task<IActionResult> TblAppointments_ReadJson(string filterFromDate)
        {
            DateTime datFilterFromDate = DateTime.Now;
            bool success = DateTime.TryParse(filterFromDate, CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.None, out datFilterFromDate);

            var storeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var employs = await _context.TblEmployees.Where(a => a.StoreId == storeId).Select(a => a.Id).ToListAsync();

            List<AppointmentModel> lstApps = new List<AppointmentModel>();

            if (success)
            {
                return Json(await _context.TblAppointments.Include(a => a.User).Where(a => !a.Canceled && employs.Contains(a.EmployeeId) && a.ValidFrom.Date == datFilterFromDate.Date)
                    .Select(a => new AppointmentModel {Id = a.Id, Text = a.Text, Name = $"{a.User.Firstname} {a.User.Lastname}", Phonenumber = a.User.PhoneNumber, ValidFrom = a.ValidFrom.AddHours(2), ValidTo = a.ValidTo.AddHours(2)}).OrderBy(x => x.ValidFrom).ToListAsync());
            }

            return Json(await _context.TblAppointments.Include(a => a.User).Where(a => !a.Canceled && employs.Contains(a.EmployeeId))
                .Select(a => new AppointmentModel {Id = a.Id, Text = a.Text, Name = $"{a.User.Firstname} {a.User.Lastname}", Phonenumber = a.User.PhoneNumber, ValidFrom = a.ValidFrom.AddHours(2), ValidTo = a.ValidTo.AddHours(2)}).OrderBy(x => x.ValidFrom).ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var app = await _context.TblAppointments.Include(a => a.User).Include(x => x.Employee).Include(x => x.Employee.Store).FirstOrDefaultAsync(m => m.Id == id);
            if (app == null || app.Employee == null || app.Employee.Store == null)
            {
                return NotFound();
            }

            ExMeeting meeting = new ExMeeting
                                {
                                    Id = app.Id,
                                    ShopId = app.Employee.StoreId,
                                    ShopName = app.User.Firstname + " " + app.User.Lastname,
                                    Start = app.ValidFrom.AddHours(2),
                                    End = app.ValidTo.AddHours(2),
                                    Staff = new ExStaff
                                            {
                                                Id = app.Employee.Id,
                                                Name = app.Text,
                                                WhatsappContact = app.User.PhoneNumber,
                                                //Image = app.Employee.Image
                                            }
                                };

            ViewBag.AdressUser = $"{app.User.PostalCode ?? ""} {app.User.City ?? ""} {app.User.Street ?? ""}";

            return View(meeting);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text,Canceled,Attended,BookedOn,ValidFrom,ValidTo,UserId,EmployeeId")]
            TableAppointment tableAppointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tableAppointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tableAppointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableAppointment = await _context.TblAppointments.FindAsync(id);
            if (tableAppointment == null)
            {
                return NotFound();
            }

            return View(tableAppointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,Canceled,Attended,BookedOn,ValidFrom,ValidTo,UserId,EmployeeId")]
            TableAppointment tableAppointment)
        {
            if (id != tableAppointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tableAppointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableAppointmentExists(tableAppointment.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(tableAppointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableAppointment = await _context.TblAppointments.Include(a => a.User).Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableAppointment == null)
            {
                return NotFound();
            }

            tableAppointment.ValidFrom.AddHours(2);
            tableAppointment.ValidTo.AddHours(2);
            return View(tableAppointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tableAppointment = await _context.TblAppointments.FindAsync(id);

            var tbUser = await _context.TblUsers.AsNoTracking().FirstOrDefaultAsync(a => a.Id == tableAppointment.UserId);
            RestAccess ra = new RestAccess(tbUser.Id.ToString(), tbUser.RestPasswort, Constants.ServiceClientEndPointWithApiPrefix);

            var r = new ExRemoveMeetingRequest
                    {
                        MeetingId = id,
                        UserId = tableAppointment.UserId,
                        UserType = EnumUserType.ShopEmployee,
                        CheckPassword = WebAppSettings.CheckPassword,
                    };

            var xxx = await ra.DeleteMeetingWeb(r);
            return RedirectToAction(nameof(Index));
        }

        private bool TableAppointmentExists(int id)
        {
            return _context.TblAppointments.Any(e => e.Id == id);
        }
    }
}