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
    [Authorize(Roles = "User")]
    public class MyAppointmentsController : Controller
    {
        private readonly Db _context;

        public MyAppointmentsController(Db context)
        {
            _context = context;
        }

        // GET: Appointments
        // A simple View is returned
        // The Grid will asyc call the Appointments_Read action on page load
        public async Task<IActionResult> Index()
        {
            if (TempData["message"] != null) ViewData["message"] = TempData["message"];
            AppointmentViewModel model = new AppointmentViewModel();
            model.FilterFrom = DateTime.Now.AddHours(2);

            return View(model);
        }

        public async Task<IActionResult> TblAppointments_Read([DataSourceRequest] DataSourceRequest request, string filterFromDate)
        {
            DateTime datFilterFromDate = DateTime.Now;
            bool success = DateTime.TryParse(filterFromDate, CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.None, out datFilterFromDate);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            List<AppointmentModel> lstApps = new List<AppointmentModel>();

            if (success)
            {
                return Json(await _context.TblAppointments.Include(a => a.User).Include(a => a.Employee).ThenInclude(a => a.Store).Where(a => !a.Canceled && a.UserId == userId && a.ValidFrom.Date == datFilterFromDate.Date)
                    .Select(a => new AppointmentModel {Id = a.Id, Text = a.Text, Name = $"{a.Employee.Store.CompanyName} - {a.Employee.FirstName} {a.Employee.LastName}", Phonenumber = a.Employee.TelephoneNumber, ValidFrom = a.ValidFrom.AddHours(2), ValidTo = a.ValidTo.AddHours(2)}).ToDataSourceResultAsync(request));
            }

            //TODO: Was anderes machen
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> TblAppointments_ReadJson(string filterFromDate)
        {
            DateTime datFilterFromDate = DateTime.Now;
            bool success = DateTime.TryParse(filterFromDate, CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.None, out datFilterFromDate);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            List<AppointmentModel> lstApps = new List<AppointmentModel>();

            if (success)
            {
                return Json(await _context.TblAppointments.Include(a => a.User).Include(a => a.Employee).ThenInclude(a => a.Store)
                    .Where(a => !a.Canceled && a.UserId == userId && a.ValidFrom.Date == datFilterFromDate.Date)
                    .Select(a => new AppointmentModel {Id = a.Id, Text = a.Text, Name = $"{a.Employee.Store.CompanyName} - {a.Employee.FirstName} {a.Employee.LastName}", Phonenumber = a.Employee.TelephoneNumber, ValidFrom = a.ValidFrom.AddHours(2), ValidTo = a.ValidTo.AddHours(2)})
                    .OrderBy(x => x.ValidFrom)
                    .ToListAsync());
            }

            return Json(null);
        }


        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var app = await _context.TblAppointments.Include(x => x.Employee).Include(x => x.Employee.Store).FirstOrDefaultAsync(m => m.Id == id);
            if (app == null || app.Employee == null || app.Employee.Store == null)
            {
                return NotFound();
            }

            ExMeeting meeting = new ExMeeting
                                {
                                    Id = app.Id,
                                    ShopId = app.Employee.StoreId,
                                    ShopName = app.Employee.Store.CompanyName,
                                    Start = app.ValidFrom.AddHours(2),
                                    End = app.ValidTo.AddHours(2),
                                    Staff = new ExStaff
                                            {
                                                Id = app.Employee.Id,
                                                Name = app.Employee.FirstName + " " + app.Employee.LastName,
                                                WhatsappContact = app.Employee.TelephoneNumber,
                                                Image = app.Employee.Image
                                            }
                                };

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

            var tableAppointment = await _context.TblAppointments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableAppointment == null)
            {
                return NotFound();
            }

            tableAppointment.ValidFrom = tableAppointment.ValidFrom.AddHours(2);
            tableAppointment.ValidTo = tableAppointment.ValidTo.AddHours(2);
            return View(tableAppointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tableAppointment = await _context.TblAppointments.FindAsync(id);

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var tbUser = await _context.TblUsers.AsNoTracking().FirstOrDefaultAsync(a => a.Id == userId);
            RestAccess ra = new RestAccess(tbUser.Id.ToString(), tbUser.RestPasswort, Constants.ServiceClientEndPointWithApiPrefix);

            var r = new ExRemoveMeetingRequest
                    {
                        MeetingId = id,
                        UserId = tableAppointment.UserId,
                        UserType = EnumUserType.Customer,
                        CheckPassword = WebAppSettings.CheckPassword,
                    };

            await ra.DeleteMeetingWeb(r);
            return RedirectToAction(nameof(Index));
        }

        private bool TableAppointmentExists(int id)
        {
            return _context.TblAppointments.Any(e => e.Id == id);
        }
    }
}