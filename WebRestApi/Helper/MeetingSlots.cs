// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;
using Exchange.Model;

namespace WebRestApi.Helper
{
    /// <summary>
    ///     <para>Slots für Meetings</para>
    ///     Klasse MeetingSlots. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class MeetingSlots
    {
        /// <summary>
        ///     Meetingslots für einen Standort suchen für den gegebenen Tag bzw  nach der gegebenen Zeit
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<ExMeeting> GetSlots(int locationId, DateTime date)
        {
            using (var db = new Db())
            {
                return GetSlots(db, locationId, date);
            }
        }

        /// <summary>
        ///     alle Meetingslots für einen Standort suchen für den gegebenen Tag bzw  nach der gegebenen Zeit
        /// </summary>
        /// <param name="db"></param>
        /// <param name="locationId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<ExMeeting> GetSlots(Db db, int locationId, DateTime date)
        {
            var slots = new List<ExMeeting>();

            var location = db.TblLocations
                .Include(x => x.TblLocationEmployee)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == locationId);


            if (location?.TblLocationEmployee == null || !location.TblLocationEmployee.Any())
                return slots;

            foreach (var locationEmployee in location.TblLocationEmployee)
            {
                var employee = db.TblEmployees
                    .Where(x => x.Id == locationEmployee.TblEmployeeId)
                    .Include(x => x.TblVirtualWorkTimes)
                    .Select(x => new TableEmployee
                                 {
                                     Id = x.Id,
                                     FirstName = x.FirstName,
                                     LastName = x.LastName,
                                     TelephoneNumber = x.TelephoneNumber,
                                     TblVirtualWorkTimes = x.TblVirtualWorkTimes,
                                 })
                    .AsNoTracking()
                    .FirstOrDefault();

                var workTimeToday = employee.TblVirtualWorkTimes.FirstOrDefault(x => x.Weekday == date.Date.DayOfWeek);

                if (workTimeToday == null)
                    continue;

                var slotJump = workTimeToday.TimeSlot <= 5 ? 5 : workTimeToday.TimeSlot;

                for (var slot = workTimeToday.TimeFrom; slot < workTimeToday.TimeTo; slot = slot.Add(new TimeSpan(0, slotJump, 0)))
                {
                    var from = date.Date.AddTicks(slot.Ticks);

                    //var fromLocal = DateTime.Now;
                    //var success = DateTime.TryParse(from.ToString("yyyy-MM-ddTHH:mm:ss.000Z"), 
                    //     CultureInfo.GetCultureInfo("de-DE"), 
                    //    DateTimeStyles.None, 
                    //    out fromLocal);

                    var fromLocal = from.AddHours(-2);

                    var fromUtc = fromLocal.ToUniversalTime();

                    var to = fromUtc.AddMinutes(slotJump);

                    if (to < date) continue;

                    var store = db.TblStores.Where(x => x.Id == location.StoreId)
                        .Select(x => new {x.Id, x.CompanyName})
                        .AsNoTracking()
                        .FirstOrDefault();

                    var meetings = db.TblAppointments.AsNoTracking()
                        .Where(x => x.EmployeeId == employee.Id && x.ValidFrom == fromUtc && !x.Canceled);

                    var meeting = meetings.FirstOrDefault();

                    slots.Add(new ExMeeting
                              {
                                  Id = meeting?.Id ?? -1,
                                  ShopId = location.Id,
                                  ShopName = store.CompanyName,
                                  Staff = Staff.GetExStaff(employee),
                                  UserId = meeting?.UserId,
                                  Start = fromUtc,
                                  End = to,
                              });
                }
            }

            return slots;
        }
    }
}