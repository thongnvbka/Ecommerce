using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cms.Attributes;
using Common.Constant;
using Common.Emums;
using Common.Helper;
using Common.PasswordEncrypt;
using Library.DbContext.Entities;
using Library.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Cms.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult LogOn(string returnUrl = "/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
     
        [HttpPost]
        public async Task<ActionResult> LogOn(LoginMeta login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = UnitOfWork.UserRepo.SingleOrDefault(
                x => (x.UserName.Equals(login.UserName) || x.Email.Equals(login.UserName)) && !x.IsDelete);

            // Tên đăng nhập hoặc Password không đúng
            if (user == null)
            {
                ModelState.AddModelError("Invalid_UserName", @"Username or password incorrect");
                return View();
            }

            // Nhân viên thôi việc
            if (user.Status == 5)
            {
                ModelState.AddModelError("Invalid_UserName", @"This account does not exist");
                return View();
            }

            // Nhân viên thôi việc
            if (user.Status == 6)
            {
                ModelState.AddModelError("Invalid_UserName", @"This account has retired");
                return View();
            }

            var currentDate = DateTime.Now;
            // Lấy hoặc khởi tạo cấu hình lock tài khoản khi đăng nhập lỗi
            var configLoginFailure = await UnitOfWork.ConfigLoginFailureRepo.FirstOrDefaultAsync() ??
                                     new ConfigLoginFailure()
                                     {
                                         LockDuration = 5,
                                         LoginFailureInterval = 10,
                                         MaximumLoginFailure = 5
                                     };

            // Tài khoản đang bị khóa
            if (user.IsLockout)
            {
                if (!user.LastLockoutDate.HasValue)
                {
                    user.LastLockoutDate = currentDate;
                    user.LockoutToDate = currentDate.AddMinutes(configLoginFailure.LockDuration);
                    user.FirstLoginFailureDate = currentDate.AddMinutes(-(configLoginFailure.LoginFailureInterval));
                    user.LoginFailureCount = configLoginFailure.MaximumLoginFailure;


                    await UnitOfWork.UserRepo.UserUpdateLoginFailure(user.Id, user.IsLockout, user.LastLockoutDate, user.FirstLoginFailureDate,
                            user.LoginFailureCount, user.LockoutToDate);

                    ModelState.AddModelError("Lock_Duration", $"Account locked for  {configLoginFailure.LockDuration} minutes");
                    return View();
                }

                var unlockTime = user.LastLockoutDate.Value.AddMinutes(configLoginFailure.LockDuration);
                if (unlockTime.CompareTo(currentDate) > 0)
                {
                    ModelState.AddModelError("Lock_Duration", $"Account locked for  {(int)unlockTime.Subtract(currentDate).TotalMinutes + 1} minutes");
                    return View();
                }
            }

            if (!PasswordEncrypt.EncodePassword(login.Password.Trim(), PasswordSalt.Cms).Equals(user.Password))
            {
                if (await LockUser(user, currentDate, configLoginFailure.MaximumLoginFailure,
                    configLoginFailure.LoginFailureInterval, configLoginFailure.LockDuration))
                {
                    ModelState.AddModelError("Lock_Duration", $"Account locked for  {configLoginFailure.LockDuration} minutes");
                    return View();
                }

                ModelState.AddModelError("Invalid_UserName", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View();
            }

            if (user.LoginFailureCount > 0)
            {
                user.IsLockout = false;
                user.LoginFailureCount = 0;
                user.FirstLoginFailureDate = null;
                user.LastLockoutDate = null;
                user.LockoutToDate = null;

                await UnitOfWork.UserRepo.UserUpdateLoginFailure(user.Id, user.IsLockout, user.LastLockoutDate,
                        user.FirstLoginFailureDate,
                        user.LoginFailureCount, user.LockoutToDate);
            }

            var userPosition = UnitOfWork.UserPositionRepo.SingleOrDefault(x => x.IsDefault && x.UserId == user.Id);
            var office = UnitOfWork.OfficeRepo.SingleOrDefault(x => !x.IsDelete && x.Id == userPosition.OfficeId);

            var userSate = new UserState();
            userSate.FromUser(user, userPosition, office);
            userSate.SessionId = $"{currentDate.Ticks}_{user.Id}";

            //var userSate = new UserState()
            //{
            //    FullName = user.FullName,
            //    OfficeId = userPosition?.OfficeId,
            //    OfficeName = userPosition?.OfficeName,
            //    TitleId = userPosition?.TitleId,
            //    TitleName = userPosition?.TitleName,
            //    UserId = user.Id,
            //    UserName = user.UserName,
            //    SessionId = $"{currentDate.Ticks}_{user.Id}",
            //    Type = userPosition?.Type,
            //    OfficeIdPath = userPosition.OfficeIdPath,
            //    Avatar = user.Avatar,
            //};

            IdentitySignin(userSate, login.RememberMe);

            //set cookie
            HttpCookie langCookie = Request.Cookies["lang"];
            if (langCookie == null)
            {
                langCookie = new HttpCookie("lang")
                {
                    Value = userSate.Culture,
                    Expires = DateTime.Now.AddDays(3d)
                };
                Response.Cookies.Add(langCookie);
            }
            else
            {
                if (langCookie.Value != userSate.Culture)
                {
                    langCookie = new HttpCookie("lang")
                    {
                        Value = userSate.Culture,
                        Expires = DateTime.Now.AddDays(3d)
                    };
                    Response.Cookies.Add(langCookie);
                }
            }

            // Log đăng nhập
            await UnitOfWork.LogLoginRepo.Insert(user.UserName, user.FullName, MyCommon.ClientIp(), userSate.SessionId,
                Request.Browser.Platform, Request.Browser.Browser, Request.Browser.MajorVersion);

            if (!string.IsNullOrEmpty(login.ReturnUrl))
                return Redirect(login.ReturnUrl);

            return Redirect("/");
        }

        [HttpPost]
        public async Task<ActionResult> ReLogOn(LoginMeta login)
        {
            if (!ModelState.IsValid)
                return JsonCamelCaseResult(new {Status = -1, Text = "Tên đăng nhập hoặc mật khẩu không đúng"},
                    JsonRequestBehavior.AllowGet);

            var user = UnitOfWork.UserRepo.SingleOrDefault(
                x => (x.UserName.Equals(login.UserName) || x.Email.Equals(login.UserName)) && !x.IsDelete);

            // Tên đăng nhập hoặc Password không đúng
            if (user == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "Tên đăng nhập hoặc mật khẩu không đúng" },
                        JsonRequestBehavior.AllowGet);

            var currentDate = DateTime.Now;
            // Lấy hoặc khởi tạo cấu hình lock tài khoản khi đăng nhập lỗi
            var configLoginFailure = await UnitOfWork.ConfigLoginFailureRepo.FirstOrDefaultAsync() ??
                                     new ConfigLoginFailure()
                                     {
                                         LockDuration = 5,
                                         LoginFailureInterval = 10,
                                         MaximumLoginFailure = 5
                                     };

            // Tài khoản đang bị khóa
            if (user.IsLockout)
            {
                if (!user.LastLockoutDate.HasValue)
                {
                    user.LastLockoutDate = currentDate;
                    user.LockoutToDate = currentDate.AddMinutes(configLoginFailure.LockDuration);
                    user.FirstLoginFailureDate = currentDate.AddMinutes(-(configLoginFailure.LoginFailureInterval));
                    user.LoginFailureCount = configLoginFailure.MaximumLoginFailure;


                    await UnitOfWork.UserRepo.UserUpdateLoginFailure(user.Id, user.IsLockout, user.LastLockoutDate, 
                        user.FirstLoginFailureDate, user.LoginFailureCount, user.LockoutToDate);

                    return JsonCamelCaseResult(
                            new {Status = -2, Text = $"Account locked for  {configLoginFailure.LockDuration} minutes"},
                            JsonRequestBehavior.AllowGet);
                }

                var unlockTime = user.LastLockoutDate.Value.AddMinutes(configLoginFailure.LockDuration);
                if (unlockTime.CompareTo(currentDate) > 0)
                    return
                        JsonCamelCaseResult(
                            new
                            {
                                Status = -2,
                                Text =
                                $"Account locked for  {(int) unlockTime.Subtract(currentDate).TotalMinutes + 1} minutes"
                            }, JsonRequestBehavior.AllowGet);
            }

            if (!PasswordEncrypt.EncodePassword(login.Password.Trim(), PasswordSalt.Cms).Equals(user.Password))
            {
                if (await LockUser(user, currentDate, configLoginFailure.MaximumLoginFailure,
                    configLoginFailure.LoginFailureInterval, configLoginFailure.LockDuration))
                    return JsonCamelCaseResult(
                            new
                            {
                                Status = -2,
                                Text = $"Account locked for  {configLoginFailure.LockDuration} minutes"
                            }, JsonRequestBehavior.AllowGet);

                return JsonCamelCaseResult(
                        new
                        {
                            Status = -2,
                            Text = "Tên đăng nhập hoặc mật khẩu không đúng"
                        }, JsonRequestBehavior.AllowGet);
            }

            if (user.LoginFailureCount > 0)
            {
                user.IsLockout = false;
                user.LoginFailureCount = 0;
                user.FirstLoginFailureDate = null;
                user.LastLockoutDate = null;
                user.LockoutToDate = null;

                await UnitOfWork.UserRepo.UserUpdateLoginFailure(user.Id, user.IsLockout, user.LastLockoutDate,
                        user.FirstLoginFailureDate,
                        user.LoginFailureCount, user.LockoutToDate);
            }

            var userPosition = UnitOfWork.UserPositionRepo.SingleOrDefault(x => x.IsDefault && x.UserId == user.Id);
            var office = UnitOfWork.OfficeRepo.SingleOrDefault(x => !x.IsDelete && x.Id == userPosition.OfficeId);

            var userSate = new UserState();
            userSate.FromUser(user, userPosition, office);
            userSate.SessionId = $"{currentDate.Ticks}_{user.Id}";

            IdentitySignin(userSate, login.RememberMe);

            // Log đăng nhập
            await UnitOfWork.LogLoginRepo.Insert(user.UserName, user.FullName, MyCommon.ClientIp(), userSate.SessionId,
                Request.Browser.Platform, Request.Browser.Browser, Request.Browser.MajorVersion);

            return JsonCamelCaseResult(
                new
                {
                    Status = 1,
                    Text = "Log in successfully"
                }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> LogOut()
        {
            // Cập nhật Logout time
            await UnitOfWork.LogLoginRepo.UpdateLogoutTime(UserState.UserName, UserState.SessionId, DateTime.Now);

            // Logout
            IdentitySignout();

            return RedirectToAction("LogOn");
        }

        private async Task<bool> LockUser(User user, DateTime currentDate, byte maxLoginFailure, byte loginFailureInterval, int lockDuration)
        {
            if (!user.FirstLoginFailureDate.HasValue || user.LoginFailureCount == 0)
            {
                user.LoginFailureCount = 1;
                user.FirstLoginFailureDate = currentDate;
            }
            else
            {
                if (user.FirstLoginFailureDate.Value.AddMinutes(loginFailureInterval).CompareTo(currentDate) > 0)
                {
                    user.LoginFailureCount = (byte)(user.LoginFailureCount + 1);
                }
                else
                {
                    user.LoginFailureCount = 1;
                    user.FirstLoginFailureDate = currentDate;
                }
            }

            user.IsLockout = user.LoginFailureCount >= maxLoginFailure;
            if (user.IsLockout)
            {
                user.LastLockoutDate = currentDate;
                user.LockoutToDate = currentDate.AddMinutes(lockDuration);
            }

            await UnitOfWork.UserRepo.UserUpdateLoginFailure(user.Id, user.IsLockout, user.LastLockoutDate,
                user.FirstLoginFailureDate, user.LoginFailureCount, user.LockoutToDate);

            return user.IsLockout;
        }

        public void IdentitySignin(UserState userState,  bool isPersistent = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userState.UserId.ToString()),
                new Claim(ClaimTypes.Name, userState.UserName),
                new Claim(ClaimTypes.GivenName, userState.FullName),
                new Claim("userState", userState.ToString())
            };

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignIn(new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            }, identity);
        }

        public void IdentitySignout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                                            DefaultAuthenticationTypes.ExternalCookie);
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        public async Task<ActionResult> ChooseLanguage(string culture)
        {
            var user = UnitOfWork.UserRepo.Entities.FirstOrDefault(x=>x.Id == UserState.UserId);
            if (user != null)
            {
                user.Culture = culture;
                await UnitOfWork.UserRepo.SaveAsync();

                var currentDate = DateTime.Now;
                var userPosition = UnitOfWork.UserPositionRepo.SingleOrDefault(x => x.IsDefault && x.UserId == user.Id);
                var office = UnitOfWork.OfficeRepo.SingleOrDefault(x => !x.IsDelete && x.Id == userPosition.OfficeId);
                var userSate = new UserState();
                userSate.FromUser(user, userPosition, office);
                userSate.SessionId = $"{currentDate.Ticks}_{user.Id}";

                IdentitySignin(userSate, true);
            }

            return JsonCamelCaseResult(
               new
               {
                   Status = 1,
                   Text = "Transfer successful"
               }, JsonRequestBehavior.AllowGet);
        }
    }
}