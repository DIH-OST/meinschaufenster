// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Database.Tables;
using Exchange;
using Exchange.Enum;
using Exchange.Helper;
using Exchange.Model;
using Exchange.Notifications;
using Exchange.PostRequests;
using WebRestApi.Helper;
using WebRestApi.Services;


namespace WebRestApi.Controllers
{
    /// <summary>
    ///     <para>Controller für Login, User erzeugen, Passwort zurücksetzen, ...</para>
    ///     Klasse UserController. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class UserController : Controller
    {
        private readonly HtmlGenerator _mailgenerator;
        private readonly ViewRender _view;

        public UserController(ViewRender view)
        {
            _view = view;
            _mailgenerator = new HtmlGenerator(view);
        }

        /// <summary>
        ///     Benutzerstammdaten ändern
        /// </summary>
        /// <param name="user">Benutzerdaten</param>
        /// <returns></returns>
        [Authorize]
        [Route("api/UserUpdate/")]
        [HttpPost]
        public async Task<ExSaveDataResult> UserUpdate([FromBody] ExUserAccountData user)
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

                if (user.UserId.ToString() != userId)
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

            Logging.Log.LogInfo($"UserUpdate {user.UserId}");
            using (var db = new Db())
            {
                if (user.IsDemoUser)
                    return new ExSaveDataResult
                           {
                               Result = EnumSaveDataResult.Information,
                               Description = "Daten können nicht geändert werden.",
                               Caption = "Nicht möglich"
                           };

                var data = await db.TblUsers.FirstOrDefaultAsync(u => u.Id == user.UserId);

                if (data == null)
                    return new ExSaveDataResult
                           {
                               Result = EnumSaveDataResult.Error,
                               Description = "Account ungültig!",
                               Caption = "Fehler"
                           };

                data.Firstname = user.FirstName;
                data.Lastname = user.LastName;
                data.Street = user.Street;
                data.PostalCode = user.PostalCode;
                data.City = user.City;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Logging.Log.LogWarning($"UserDeviceUpdate Save: {e}");
                    return ExSaveDataResult.GetDefaultSaveError();
                }

                return ExSaveDataResult.GetDefaultSuccess();
            }
        }

        /// <summary>
        ///     Passwort ändern
        /// </summary>
        /// <param name="password">Altes und neues Passwort</param>
        /// <returns></returns>
        [Authorize]
        [Route("api/UserUpdatePassword/")]
        [HttpPost]
        public async Task<ExSaveDataResult> UserUpdatePassword([FromBody] ExPostUserChangePasswortData password)
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

                if (password.UserId.ToString() != userId)
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

            Logging.Log.LogInfo($"UserUpdatePassword {password.UserId}");
            using (var db = new Db())
            {
                var data = await db.TblUsers.FirstOrDefaultAsync(u => u.Id == password.UserId);

                if (data == null)
                    return new ExSaveDataResult
                           {
                               Result = EnumSaveDataResult.Error,
                               Description = "Account ungültig!",
                               Caption = "Fehler"
                           };

                if (data.Password != password.OldPasswordHash)
                    return new ExSaveDataResult
                           {
                               Result = EnumSaveDataResult.Warning,
                               Description = "Aktuelles Passwort falsch!",
                               Caption = "Nicht möglich"
                           };

                data.Password = password.NewPasswordHash;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Logging.Log.LogWarning($"UserDeviceUpdate Save: {e}");
                    return ExSaveDataResult.GetDefaultSaveError();
                }

                return ExSaveDataResult.GetDefaultSuccess();
            }
        }

        /// <summary>
        ///     Einen bestimmten User zum Demo User machen
        /// </summary>
        /// <param name="userId">User Id (Datenbank)</param>
        /// <returns></returns>
        [Authorize]
        [Route("api/UserSetAsDemoUser/{userId}")]
        [HttpGet]
        public async Task<bool> UserSetAsDemoUser(int userId)
        {
            Logging.Log.LogInfo($"UserSetAsDemoUser {userId}");
            using (var db = new Db())
            {
                var data = await db.TblUsers.FirstOrDefaultAsync(u => u.Id == userId);

                if (data == null)
                    return false;

                var allOldDemos = db.TblUsers.Where(u => u.IsDemoUser);
                foreach (var user in allOldDemos)
                    user.IsDemoUser = false;
                data.IsDemoUser = true;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Logging.Log.LogError($"UserSetAsDemoUser: {e}");
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        ///     Liefert den authentifizierten Benutzer zurück
        /// </summary>
        /// <returns>Authentifizierter Benutzer</returns>
        [Authorize]
        [Route("api/GetAuthenticatedUser")]
        [HttpGet]
        public ExAuthenticatedUser GetAuthenticatedUser()
        {
            var username = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;

            //Beispiel um die BenutzerID abzufragen
            var userId = HttpContext.User.Claims.First(c => c.Type == "UserID").Value;

            ExAuthenticatedUser authUser = new ExAuthenticatedUser();
            authUser.Name = username;
            authUser.UserId = long.Parse(userId);

            return authUser;
        }

        /// <summary>
        ///     Bestätigungs-Token senden ...
        /// </summary>
        /// <param name="user">User Daten</param>
        /// <returns></returns>
        private async Task<bool> SendPassword(TableUser user, string password, bool isNewUser = false)
        {
            return !string.IsNullOrWhiteSpace(Common.SendSMS(user.PhoneNumber, "Dein Passwort für meinschaufenster.at lautet: " + password));
        }

        #region UserDevices

        /// <summary>
        ///     Ein Gerät eines User anlegen bzw. aktualisieren
        /// </summary>
        /// <param name="userDeviceData"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/UserDeviceUpdate/")]
        [HttpPost]
        public async Task<bool> UserDeviceUpdate([FromBody] ExUserDeviceInfo userDeviceData)
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
                    return false;
                }

                var userId = identity.HasClaim(c => c.Type == "UserID")
                    ? identity.FindFirst("UserID").Value
                    : "a"; //BENUTZER ID

                if (userDeviceData.UserId.ToString() != userId)
                {
                    HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                    return false;
                }
            }
            else
            {
                HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                return false;
            }

            Logging.Log.LogInfo($"UserDeviceUpdate {userDeviceData.Device.DeviceToken}");
            using (var db = new Db())
            {
                //Eventuell hängt das Gerät an einem anderen User Account => löschen!
                var userDevices = db.TblUserDevices.Where(d => d.Device.DeviceToken == userDeviceData.Device.DeviceToken && d.TblUserId != userDeviceData.UserId);
                if (userDevices != null)
                    db.TblUserDevices.RemoveRange(userDevices);

                //Daten aktualisieren
                bool isNew = false;

                if (userDeviceData.UserId < 0)
                    return false;

                var user = await db.TblUsers.FirstOrDefaultAsync(u => u.Id == userDeviceData.UserId);
                if (user == null)
                    return false;

                var data = await db.TblUserDevices.FirstOrDefaultAsync(d => d.Device.DeviceToken == userDeviceData.Device.DeviceToken);
                if (data == null)
                {
                    data = new TableUserDevice
                           {
                               TblUser = user
                           };
                    isNew = true;
                }

                if (isNew)
                    db.TblUserDevices.Add(data);

                if (data.Device == null)
                    data.Device = new DbUserDeviceInfo();

                data.Device.DeviceIdiom = userDeviceData.Device.DeviceIdiom;
                data.Device.DeviceToken = userDeviceData.Device.DeviceToken;
                data.Device.DeviceType = userDeviceData.Device.DeviceType;
                data.Device.IsAppRunning = userDeviceData.Device.IsAppRunning;
                data.Device.OperatingSystemVersion = userDeviceData.Device.OperatingSystemVersion;
                data.Device.Plattform = userDeviceData.Device.Plattform;
                data.Device.LastDateTimeUtcOnline = DateTime.UtcNow;
                data.Device.IsAppRunning = true;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Logging.Log.LogWarning($"UserDeviceUpdate Save: {e}");
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        ///     Ein Gerät eines Benutzers aus dessen Geräteliste entfernen für Apps
        /// </summary>
        /// <param name="userDeviceData"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/UserDeviceDelete/")]
        [HttpPost]
        public async Task<bool> UserDeviceDelete([FromBody] ExPostUserDeviceDelete userDeviceData)
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
                    return false;
                }

                var userId = identity.HasClaim(c => c.Type == "UserID")
                    ? identity.FindFirst("UserID").Value
                    : "a"; //BENUTZER ID

                if (userDeviceData.UserId.ToString() != userId)
                {
                    HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                    return false;
                }
            }
            else
            {
                HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                return false;
            }

            return await UserDeviceDeleteInternal(userDeviceData);
        }

        /// <summary>
        ///     Ein Gerät eines Benutzers aus dessen Geräteliste entfernen für WebApp
        /// </summary>
        /// <param name="userDeviceData"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/UserDeviceDeleteWeb/")]
        [HttpPost]
        public async Task<bool> UserDeviceDeleteWeb([FromBody] ExPostUserDeviceDelete userDeviceData)
        {
            if (userDeviceData.CheckPassword != WebAppSettings.CheckPassword)
            {
                HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                return false;
            }

            return await UserDeviceDeleteInternal(userDeviceData);
        }

        private async Task<bool> UserDeviceDeleteInternal(ExPostUserDeviceDelete userDeviceData)
        {
            Logging.Log.LogInfo($"UserDeviceDelete {userDeviceData.DeviceToken}");
            using (var db = new Db())
            {
                var data = db.TblUserDevices.Where(d => d.Device.DeviceToken == userDeviceData.DeviceToken);
                db.TblUserDevices.RemoveRange(data);

                try
                {
                    var service = new MobileCenterNotification(Constants.MobileCenterNotification);
                    if (data.Any())
                        await service.RemoveDevice(data.First().Device.DeviceToken, data.First().Device.Plattform);

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Logging.Log.LogWarning($"UserDeviceDelete SaveChanges: {e}");
                    return false;
                }

                return true;
            }
        }

        #endregion

        #region Anonymus für Login

        /// <summary>
        ///     1. Schritt der Anmeldung => UserName (Telefonnummer) wird empfangen. Wenn User existiert wird Id zurück gegeben.
        ///     Wenn neuer
        ///     User dann IsNewUser = true und Passwort per SMS schicken.
        ///     Es wird auch mitgegeben ob der Account gerade gesperrt ist.
        /// </summary>
        /// <param name="userPhone">User Telefonnummer (Login Name)</param>
        /// <returns> Passwort Hash oder String.Empty</returns>
        [AllowAnonymous]
        [Route("api/UserCheck/{userPhone}")]
        [HttpGet]
        public async Task<ExCheckUser> UserCheck(string userPhone)
        {
            Logging.Log.LogInfo($"UserCheck {userPhone}");
            using (var db = new Db())
            {
                if (!PhoneHelper.IsNumber(userPhone))
                {
                    return new ExCheckUser
                           {
                               WrongNumberFormat = true,
                           };
                }

                var numberOk = PhoneHelper.ProoveValidPhoneNumber(userPhone, out var num);

                if (!numberOk)
                {
                    return new ExCheckUser
                           {
                               WrongNumberFormat = true,
                           };
                }

                var data = await db.TblUsers.FirstOrDefaultAsync(u => u.PhoneNumber == num);

                if (data == null)
                {
                    var newPwd = PasswordHelper.GeneratePassword(5);
                    var newPassword = PasswordHelper.CumputeHash(newPwd);

                    data = new TableUser
                           {
                               PhoneNumber = num,
                               Locked = false,
                               Password = newPassword,
                               DefaultUserLanguage = "de",
                               PhoneChecked = true,
                           };

                    db.TblUsers.Add(data);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Logging.Log.LogWarning($"Datenbank Initialwerte konnten nicht erzeugt werden: {e}");
                        return new ExCheckUser
                               {
                                   UserId = -1,
                                   ErrorFromDb = true,
                               };
                    }

                    //SMS senden
                    try
                    {
                        await SendPassword(data, newPwd);

                        return new ExCheckUser
                               {
                                   IsNewUser = true,
                                   UserId = data.Id,
                               };
                    }
                    catch (Exception e)
                    {
                        Logging.Log.LogError($"{e}");
                        return new ExCheckUser
                               {
                                   ErrorFromDb = true,
                                   WrongNumberFormat = true,
                                   IsNewUser = true,
                                   UserId = data.Id,
                               };
                    }
                }

                if (data.IsDemoUser)
                    return new ExCheckUser
                           {
                               UserIsLocked = true,
                               IsDemoUser = true
                           };

                return new ExCheckUser
                       {
                           UserIsLocked = data.Locked,
                           UserId = data.Id,
                           EMailNotChecked = !data.PhoneChecked
                       };
            }
        }

        /// <summary>
        ///     Demo User Id abfragen (wenn Demo User verfügbar)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/UserDemoId/")]
        [HttpGet]
        public async Task<int> UserDemoId()
        {
            Logging.Log.LogInfo("UserDemoId");
            using (var db = new Db())
            {
                var data = await db.TblUsers.FirstOrDefaultAsync(u => u.IsDemoUser);
                if (data == null)
                    return -1;
                return data.Id;
            }
        }

        /// <summary>
        ///     2. Schritt Anmeldung => Passwort lokal gehashed und wird nun überprüft.
        /// </summary>
        /// <returns>Userdaten oder null wenn nicht verfügbar oder Passwort</returns>
        [AllowAnonymous]
        [Route("api/UserAccountData/")]
        [HttpPost]
        public async Task<ExUserAccountDataResult> UserAccountData([FromBody] ExPostUserPasswortData userData)
        {
            Logging.Log.LogInfo($"UserAccountData {userData.UserId},{userData.PasswordHash}");
            using (var db = new Db())
            {
                var data = await db.TblUsers.FirstOrDefaultAsync(u => u.Id == userData.UserId);

                if (data == null)
                    return null;

                if (data.Locked)
                    return new ExUserAccountDataResult
                           {
                               IsLocked = true,
                               PasswordWrong = false
                           };

                if (data.Password != userData.PasswordHash && data.IsDemoUser == false)
                    return new ExUserAccountDataResult
                           {
                               IsLocked = false,
                               PasswordWrong = true
                           };

                return new ExUserAccountDataResult
                       {
                           IsLocked = false,
                           PasswordWrong = false,
                           UserAccountData = new ExUserAccountData
                                             {
                                                 FirstName = data.Firstname,
                                                 LastName = data.Lastname,
                                                 UserId = data.Id,
                                                 RestPasswort = data.RestPasswort,
                                                 DefaultUserLanguage = data.DefaultUserLanguage,
                                                 IsDemoUser = data.IsDemoUser,
                                                 IsAdmin = data.IsAdmin,
                                                 PhoneNumber = data.PhoneNumber,
                                                 Street = data.Street,
                                                 PostalCode = data.PostalCode,
                                                 City = data.City,
                                             }
                       };
            }
        }

        /// <summary>
        ///     Passwort zurücksetzen - Email mit Link zum zurücksetzen senden
        /// </summary>
        /// <param name="userId">Id des User aus TableUser</param>
        /// <returns>Ja - true</returns>
        [AllowAnonymous]
        [Route("api/UserStartResetPassword/{userId}")]
        [HttpGet]
        public async Task<bool> UserStartResetPassword(int userId)
        {
            Logging.Log.LogInfo($"UserStartResetPassword {userId}");
            using (var db = new Db())
            {
                var data = await db.TblUsers.FirstOrDefaultAsync(u => u.Id == userId);
                if (data == null)
                    return false;

                var newPwd = PasswordHelper.GeneratePassword(5);
                data.Password = PasswordHelper.CumputeHash(newPwd);
                data.RestPasswort = PasswordHelper.GeneratePassword();

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Logging.Log.LogError($"UserResetPassword: {e}");
                    return false;
                }

                Logging.Log.LogInfo("Send UserResetPassword");

                //SMS senden
                return await SendPassword(data, newPwd);
            }
        }

        /// <summary>
        ///     EMail eines User ist gültig
        /// </summary>
        /// <param name="userId">User Id (Datenbank)</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/UserValidateEMail/{userId}/{token}")]
        [HttpGet]
        public async Task<bool> UserValidateEMail(int userId, string token)
        {
            Logging.Log.LogInfo($"UserValidateEMail {userId},{token}");
            using (var db = new Db())
            {
                var data = await db.TblUsers.FirstOrDefaultAsync(u => u.Id == userId);

                if (data == null)
                    return false;

                if (data.PhoneChecked)
                    return true;

                if (token != data.RestPasswort)
                    return false;

                data.PhoneChecked = true;
                data.RestPasswort = PasswordHelper.GeneratePassword();

                //Könnte auch später passieren in einem WebFrontend zb (Beispiel smartflower)
                data.Locked = false;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Logging.Log.LogError($"ValidateEMail: {e}");
                    return false;
                }

                return true;
            }
        }

        #endregion

        #region Userdaten löschen

        /// <summary>
        ///     Userdaten löschen für die Apps
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/DeleteUser/")]
        [HttpPost]
        public async Task<ExSaveDataResult> DeleteUser([FromBody] ExDeleteRequest request)
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

                var userClaimId = identity.HasClaim(c => c.Type == "UserID")
                    ? identity.FindFirst("UserID").Value
                    : "a"; //BENUTZER ID

                if (request.Id.ToString() != userClaimId)
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

            return await DeleteUsereInternal(request.Id);
        }

        /// <summary>
        ///     Userdaten löschen für die WebApp
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/DeleteUserWeb/")]
        [HttpPost]
        public async Task<ExSaveDataResult> DeleteUserWeb([FromBody] ExDeleteRequest request)
        {
            if (request.CheckPassword != WebAppSettings.CheckPassword)
            {
                HttpContext.Response.StatusCode = Unauthorized().StatusCode;
                return null;
            }

            return await DeleteUsereInternal(request.Id);
        }

        /// <summary>
        ///     Userdaten löschen
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<ExSaveDataResult> DeleteUsereInternal(int userId)
        {
            using (var db = new Db())
            {
                var user = db.TblUsers.Include(i => i.TblAppointments).Include(i => i.TblUserDevices).FirstOrDefault(x => x.Id == userId);

                if (user.IsAdmin || user.IsDemoUser)
                {
                    return new ExSaveDataResult
                           {
                               Description = "Admins oder Demouser können nicht gelöscht werden!",
                               Caption = "Nicht möglich!",
                               Result = EnumSaveDataResult.Information,
                           };
                }

                //Termine absagen und löschen
                var mc = new MeetingController(_view);

                var appointmentsToInform = user.TblAppointments.Where(a => a.ValidFrom > DateTime.UtcNow && a.Canceled == false);
                foreach (var appointment in appointmentsToInform)
                {
                    await mc.DeleteMeetingWeb(new ExRemoveMeetingRequest
                                              {
                                                  UserType = EnumUserType.Customer,
                                                  MeetingId = appointment.Id,
                                                  UserId = userId,
                                                  CheckPassword = WebAppSettings.CheckPassword,
                                              });
                }

                db.TblAppointments.RemoveRange(user.TblAppointments);
                db.TblUserDevices.RemoveRange(user.TblUserDevices);
                //ToDo: Geräte auch aus AzurePush entfernen!

                //Admins informieren 
                List<string> eMails2Inform;
                if (Constants.CurrentAppSettings.AppConfigurationConstants == 0) //master
                {
                    eMails2Inform = new List<string>
                                    {
                                        "info@meinschaufenster.at",
                                        "biss@fotec.at",
                                    }; //ToDo: Wenn die Settings Tabelle existiert dann über die Settings Tabelle
                }
                else
                {
                    eMails2Inform = new List<string>
                                    {
                                        "biss@fotec.at"
                                    }; //ToDo: Wenn die Settings Tabelle existiert dann über die Settings Tabelle
                }

                string userName = $"{user.Firstname} {user.Lastname}";
                string emailContent = $"Leider hat {(String.IsNullOrEmpty(userName.Trim()) ? "UNBEKANNT" : userName)} mit der Telefonnummer {user.PhoneNumber} seinen Account gelöscht.";
                string email = _mailgenerator.GetMessageOnlyEmail(new ExEMailMessageOnly
                                                                  {
                                                                      Message = emailContent
                                                                  });
                BissEMail bm = new BissEMail(WebAppSettings.EmailCredentials);
                var res = await bm.SendHtmlEMail(Constants.SendEMailAs, eMails2Inform, "Kunde gelöscht.", email, Constants.SendEMailAsDisplayName);

                //Löschen
                db.TblUsers.Remove(user);

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