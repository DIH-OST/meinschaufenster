// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;
using Exchange;
using Exchange.Model;
using WebRestApi.Controllers;

namespace WebRestApi.Helper
{
    /// <summary>
    ///     <para>MA Funktionen</para>
    ///     Klasse Staff. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class Staff
    {
        public static ExStaff GetExStaff(Db db, int staffId, bool withImage = false)
        {
            return GetExStaff(db.TblEmployees.AsNoTracking().FirstOrDefault(x => x.Id == staffId), withImage);
        }

        public static ExStaff GetExStaff(TableEmployee employee, bool withImage = false)
        {
            if (employee == null) return null;

            return new ExStaff
                   {
                       Id = employee.Id,
                       Name = employee.FirstName + " " + employee.LastName,
                       WhatsappContact = employee.TelephoneNumber,
                       Image = withImage ? employee.Image : null,
                       ImageUrl = Constants.ServiceClientEndPointWithApiPrefix + nameof(MeetingController.GetStaffImage) + "/" + employee.Id,
                   };
        }
    }
}