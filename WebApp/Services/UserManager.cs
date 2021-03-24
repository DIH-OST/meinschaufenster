// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database;
using Exchange;
using Exchange.Helper;
using Exchange.Model;
using Exchange.PostRequests;
using Exchange.ServiceAccess;

namespace WebApp.Services
{
    /// <summary>
    ///     <para>DESCRIPTION</para>
    ///     Klasse UserManager. (C) 2018 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class UserManager
    {
        private readonly string _connectionString;

        /// <summary>
        ///     Konstruktor
        /// </summary>
        /// <param name="connectionString"></param>
        public UserManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        ///     Anmelden
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        public async Task<bool> SignInAdmin(HttpContext httpContext, string username, string password, bool isPersistent = false)
        {
            try
            {
                RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);
                var user = await ra.UserCheck(username);

                //if (user.Ok && user.Result != null && !user.Result.UserIsLocked && user.Result.UserId > 0)
                if (username.ToLower() == "admin" && password == "[PASSWORT]")
                {
                    //var hash = PasswordHelper.CumputeHash(password);
                    //var userAccountData = await ra.UserAccountData(new ExPostUserPasswortData {UserId = user.Result.UserId, PasswordHash = hash});

                    //if (userAccountData.Ok && userAccountData.Result != null && userAccountData.Result.UserAccountData != null)
                    var tmp = new ExUserAccountData
                              {
                                  IsAdmin = true, FirstName = "Admin", LastName = "Admin", UserId = 1, PhoneNumber = "+43"
                              };
                    {
                        ClaimsIdentity identity = new ClaimsIdentity(GetUserClaims(tmp), CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public async Task<bool> SignInUserForAdmin(HttpContext httpContext, int userId)
        {
            await httpContext.SignOutAsync();
            RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);
            using (Db db = new Db())
            {
                var user = await db.TblUsers.FirstOrDefaultAsync(a => a.Id == userId);

                if (user == null)
                    return false;

                ExUserAccountData ud = new ExUserAccountData {UserId = user.Id, FirstName = user.Firstname, LastName = user.Lastname, IsAdmin = false, PhoneNumber = user.PhoneNumber};

                ClaimsIdentity identity = new ClaimsIdentity(GetUserClaims(ud), CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }

            return true;
        }

        /// <summary>
        ///     Anmelden
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SignInShopForAdmin(HttpContext httpContext, int shopId)
        {
            try
            {
                using (var db = new Db())
                {
                    var data = await db.TblStores.FirstOrDefaultAsync(u => u.Id == shopId);

                    if (data == null)
                    {
                        return false;
                    }

                    if (data.Activated == false)
                    {
                        //TODO: Nachfragen ob das so sein soll
                        data.Activated = true;
                        await db.SaveChangesAsync();
                    }


                    var lstClaims = new List<Claim>();

                    var claims = new List<Claim>
                                 {
                                     new Claim(ClaimTypes.NameIdentifier, data.Id.ToString()),
                                     new Claim(ClaimTypes.Name, data.CompanyName ?? ""),
                                     new Claim(ClaimTypes.Email, data.EMail),
                                     new Claim(ClaimTypes.Role, "Shop")
                                 };

                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        ///     Anmelden
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        public async Task<bool> SignInUser(HttpContext httpContext, string username, string password, bool isPersistent = false)
        {
            try
            {
                RestAccess ra = new RestAccess(Constants.ServiceClientEndPointWithApiPrefix);
                var user = await ra.UserCheck(username);
                if (user.Ok && user.Result != null && !user.Result.UserIsLocked && user.Result.UserId > 0)
                {
                    var hash = PasswordHelper.CumputeHash(password);
                    var userAccountData = await ra.UserAccountData(new ExPostUserPasswortData {UserId = user.Result.UserId, PasswordHash = hash});

                    if (userAccountData.Ok && userAccountData.Result != null && userAccountData.Result.UserAccountData != null)
                    {
                        ClaimsIdentity identity = new ClaimsIdentity(GetUserClaims(userAccountData.Result.UserAccountData), CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        ///     Anmelden
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        public async Task<int> SignInShop(HttpContext httpContext, string username, string password, bool isPersistent = false)
        {
            try
            {
                using (var db = new Db())
                {
                    var email = username.ToLower();

                    string pwd = PasswordHelper.CumputeHash(password.Trim());

                    var data = await db.TblStores.FirstOrDefaultAsync(u => u.EMail == email && u.Password == pwd); //TODO: Passwort check

                    if (data == null)
                    {
                        return 1;
                    }

                    if (data.Activated == false)
                    {
                        return 2;
                    }

                    var lstClaims = new List<Claim>();

                    var claims = new List<Claim>
                                 {
                                     new Claim(ClaimTypes.NameIdentifier, data.Id.ToString()),
                                     new Claim(ClaimTypes.Name, data.CompanyName ?? ""),
                                     new Claim(ClaimTypes.Email, data.EMail),
                                     new Claim(ClaimTypes.Role, "Shop")
                                 };


                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return 0;
                }
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public async void SignOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync();
        }

        private IEnumerable<Claim> GetUserClaims(ExUserAccountData user)
        {
            var claims = new List<Claim>
                         {
                             new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                             new Claim(ClaimTypes.Name, user.FullName),
                             new Claim(ClaimTypes.Email, user.PhoneNumber)
                         };

            claims.AddRange(GetUserRoleClaims(user));
            return claims;
        }

        private IEnumerable<Claim> GetUserRoleClaims(ExUserAccountData user)
        {
            var claims = new List<Claim>
                         {
                             new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                             new Claim(ClaimTypes.Role, user.IsAdmin
                                 ? "Admin"
                                 : user.IsDemoUser
                                     ? "Demo"
                                     : "User")
                         };

            return claims;
        }
    }
}