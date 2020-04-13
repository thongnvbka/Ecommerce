using Common.Constant;
using Common.Emums;
using Common.Helper;
using Common.Host;
using Common.MailHelper;
using Library.DbContext.Entities;
using Library.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using ProjectV.LikeOrderThaiLan.com.Items;
using ResourcesLikeOrderThaiLan;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

//using Common.Items;

namespace ProjectV.LikeOrderThaiLan.com.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        [Route("{culture}/Account/Login")]
        public ActionResult Login(string returnUrl)
        {
            if (CustomerState.Email != null)
                return Redirect("/");
            if (Request.IsAuthenticated)
                return Redirect(returnUrl);

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [Route("{culture}/Account/Login")]
        public async Task<ActionResult> Login(CustomerLoginMeta login)
        {
            //lấy id của website từ webconfig
            if (!ModelState.IsValid)
            {
                return View();
            }

            var customer = UnitOfWork.CustomerRepo.SingleOrDefault(
                x => x.Email.Equals(login.Email) && x.SystemId == SystemId);

            // Tên đăng nhập hoặc mật khẩu không đúng
            if (customer == null)
            {
                ModelState.AddModelError("CustomError", Resource.AccoumtLogin_ErrorEmailPass);
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
            if (customer.IsLockout)
            {
                if (!customer.LastLockoutDate.HasValue)
                {
                    customer.LastLockoutDate = currentDate;
                    customer.LockoutToDate = currentDate.AddMinutes(configLoginFailure.LockDuration);
                    customer.FirstLoginFailureDate = currentDate.AddMinutes(-(configLoginFailure.LoginFailureInterval));
                    customer.LoginFailureCount = configLoginFailure.MaximumLoginFailure;

                    await UnitOfWork.CustomerRepo.CustomerUpdateLoginFailure(customer.Id, customer.IsLockout, customer.LastLockoutDate, customer.FirstLoginFailureDate,
                            customer.LoginFailureCount, customer.LockoutToDate);
                    ModelState.AddModelError("CustomError", $"บัญชีถูกล็อก {configLoginFailure.LockDuration} นาที");
                    //ModelState.AddModelError("CustomError", $"Tài khoản bị khóa {configLoginFailure.LockDuration} phút");
                    return View();
                }

                var unlockTime = customer.LastLockoutDate.Value.AddMinutes(configLoginFailure.LockDuration);
                if (unlockTime.CompareTo(currentDate) > 0)
                {
                    ModelState.AddModelError("CustomError", $"บัญชีถูกล็อก {(int)unlockTime.Subtract(currentDate).TotalMinutes + 1} นาที");
                    //ModelState.AddModelError("CustomError", $"Tài khoản bị khóa {(int)unlockTime.Subtract(currentDate).TotalMinutes + 1} phút");
                    return View();
                }
            }

            if (!PasswordEncrypt.PasswordEncrypt.EncodePassword(login.Password.Trim(), PasswordSalt.FinGroupApiCustomer).Equals(customer.Password))
            {
                if (await LockCustomer(customer, currentDate, configLoginFailure.MaximumLoginFailure,
                    configLoginFailure.LoginFailureInterval, configLoginFailure.LockDuration))
                {
                    ModelState.AddModelError("CustomError", $"บัญชีถูกล็อก {configLoginFailure.LockDuration} นาที");
                    //ModelState.AddModelError("CustomError", $"Tài khoản bị khóa {configLoginFailure.LockDuration} phút");
                    return View();
                }

                ModelState.AddModelError("CustomError", Resource.AccoumtLogin_ErrorEmailPass);
                return View();
            }

            //check xác thực tài khoản
            if (customer.IsActive == false)
            {
                ModelState.AddModelError("CustomError", Resource.AccoumtLogin_ErrorAccountNotActive);
                return View();
            }

            if (customer.LoginFailureCount > 0)
            {
                customer.IsLockout = false;
                customer.LoginFailureCount = 0;
                customer.FirstLoginFailureDate = null;
                customer.LastLockoutDate = null;
                customer.LockoutToDate = null;

                await UnitOfWork.CustomerRepo.CustomerUpdateLoginFailure(customer.Id, customer.IsLockout, customer.LastLockoutDate, customer.FirstLoginFailureDate, customer.LoginFailureCount, customer.LockoutToDate);
            }

            var customerState = new CustomerState();
            customerState.FromUser(customer);
            customerState.SessionId = $"{currentDate.Ticks}_{customer.Id}";

            IdentitySignin(customerState, login.RememberMe);

            // Log đăng nhập
            await UnitOfWork.CustomerLogLoginRepo.Insert(customer.Email, customer.FullName, MyCommon.ClientIp(), customerState.SessionId,
                Request.Browser.Platform, Request.Browser.Browser, Request.Browser.MajorVersion, SystemId, SystemName);

            if (!string.IsNullOrEmpty(login.ReturnUrl))
                return Redirect(login.ReturnUrl);

            return RedirectToAction("Index", "AccountCMS", new { Area = "CMS" });
            //return Redirect("{culture}/CMS/AccountCMS/Index");
        }

        private async Task<bool> LockCustomer(Customer customer, DateTime currentDate, byte maxLoginFailure, byte loginFailureInterval, int lockDuration)
        {
            if (!customer.FirstLoginFailureDate.HasValue || customer.LoginFailureCount == 0)
            {
                customer.LoginFailureCount = 1;
                customer.FirstLoginFailureDate = currentDate;
            }
            else
            {
                if (customer.FirstLoginFailureDate.Value.AddMinutes(loginFailureInterval).CompareTo(currentDate) > 0)
                {
                    customer.LoginFailureCount = (byte)(customer.LoginFailureCount + 1);
                }
                else
                {
                    customer.LoginFailureCount = 1;
                    customer.FirstLoginFailureDate = currentDate;
                }
            }

            customer.IsLockout = customer.LoginFailureCount >= maxLoginFailure;
            if (customer.IsLockout)
            {
                customer.LastLockoutDate = currentDate;
                customer.LockoutToDate = currentDate.AddMinutes(lockDuration);
            }

            await UnitOfWork.CustomerRepo.CustomerUpdateLoginFailure(customer.Id, customer.IsLockout, customer.LastLockoutDate,
                customer.FirstLoginFailureDate, customer.LoginFailureCount, customer.LockoutToDate);

            return customer.IsLockout;
        }

        private void IdentitySignin(CustomerState customerState, bool isPersistent = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customerState.Id.ToString()),
                new Claim(ClaimTypes.Name, customerState.Email),
                new Claim(ClaimTypes.GivenName, customerState.FullName),
                new Claim("customerStateString", customerState.ToString())
            };

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignIn(new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            }, identity);
        }

        private void IdentitySignout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        /// <summary>
        /// Dang xuat
        /// </summary>
        /// <returns></returns>
        [Route("{culture}/Account/LogOut")]
        public async Task<ActionResult> LogOut()
        {
            // Cập nhật Logout time
            await UnitOfWork.CustomerLogLoginRepo.UpdateLogoutTime(CustomerState.Email, CustomerState.SessionId, DateTime.Now);

            // Logout
            IdentitySignout();

            return RedirectToAction("Login");
        }

        [Route("{culture}/Account/Register")]
        public ActionResult Register()
        {
            ViewBag.ListProvince = UnitOfWork.ProvinceRepo.Find(x => x.Culture == CountryName.TL.ToString()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            //ViewBag.ListProvince = UnitOfWork.ProvinceRepo.Find(x => x.Culture == CountryName.VN.ToString()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            if (Request.IsAuthenticated)
                return Redirect("/");

            return View();
        }

        [HttpPost]
        [Route("{culture}/Account/Register")]
        public async Task<ActionResult> Register(CustomerRegisterMeta register)
        {
            ViewBag.ListProvince = UnitOfWork.ProvinceRepo.Find(x => x.Culture == CountryName.TL.ToString()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            //ViewBag.ListProvince = UnitOfWork.ProvinceRepo.Find(x => x.Culture == CountryName.VN.ToString()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            //lấy id của website từ webconfig
            if (!ModelState.IsValid)
            {
                return View();
            }

            var customer = UnitOfWork.CustomerRepo.SingleOrDefault(
                x => x.Email.Equals(register.Email) && x.SystemId == SystemId);

            // Tên đăng nhập hoặc mật khẩu không đúng
            if (customer != null)
            {
                ModelState.AddModelError("CustomError", Resource.AccountRegister_DuplicateEmail);
                return View();
            }

            //2. Lấy thông tin định khoản để lưu
            var subjectDetail = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => x.Idd == (int)EnumAccountantSubject.Customer);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    customer = new Customer
                    {
                        FullName = register.FullName,
                        Email = register.Email,
                        Phone = register.Phone,
                        Avatar = "/Content/img/no-avatar.png",
                        Balance = 0,
                        BalanceAvalible = 0,
                        Code = string.Empty,
                        SystemId = SystemId,
                        SystemName = SystemName,
                        Password = PasswordEncrypt.PasswordEncrypt.EncodePassword(register.Password.Trim(), PasswordSalt.FinGroupApiCustomer),
                        CountryId = CountryName.TL.ToString(),
                        //CountryId = CountryName.VN.ToString(),
                        ProvinceId = register.ProvinceId,
                        ProvinceName = register.ProvinceName,
                        DistrictId = register.DistrictId,
                        DistrictName = register.DistrictName,
                        WardId = register.WardId,
                        WardsName = register.WardsName,
                        Address = register.Address
                    };
                    //customer SystemName;
                    customer.Nickname = MyCommon.Ucs2Convert($"{customer.FullName}");
                    var level = await UnitOfWork.CustomerLevelRepo.FirstOrDefaultAsync(x => x.StartMoney >= 0 && 0 < x.EndMoney);

                    //Lây danh sách nhân viên kinh doanh công ty
                    var listUser = await UnitOfWork.UserRepo.GetUserToOfficeTypeCompany(OfficeType.Business);
                    //Lấy trưởng phòng hoặc quản lý
                    var user = listUser.FirstOrDefault(x => x.Type != 0);
                    //Gắn khách hàng đăng ký mới theo ông trưởng phòng kinh doanh công ty
                    if (user != null)
                    {
                        customer.UserId = user.Id;
                        customer.UserFullName = user.FullName;
                        customer.OfficeId = user.OfficeId;
                        customer.OfficeName = user.OfficeName;
                        customer.OfficeIdPath = user.OfficeIdPath;
                    }

                    customer.LevelId = (byte)level.Id;
                    customer.LevelName = level.Name;
                    customer.Point = 0;
                    customer.Created = DateTime.Now;
                    customer.Updated = DateTime.Now;
                    customer.Birthday = DateTime.Now;
                    customer.LoginFailureCount = 0;
                    customer.HashTag = customer.FullName + "," + customer.Nickname + "," + customer.Email;
                    customer.Balance = 0;
                    customer.BalanceAvalible = 0;
                    customer.IsActive = false;
                    customer.IsLockout = false;
                    customer.IsDelete = false;

                    if (subjectDetail != null)
                    {
                        customer.TypeId = subjectDetail.Id;
                        customer.TypeIdd = subjectDetail.Idd;
                        customer.TypeName = subjectDetail.SubjectName;
                    }

                    UnitOfWork.CustomerRepo.Add(customer);
                    await UnitOfWork.CustomerRepo.SaveAsync();

                    var tempCode = UnitOfWork.CustomerRepo.Count(x =>
                        x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                        x.Created.Day == DateTime.Now.Day && x.Id <= customer.Id);

                    customer.Code = $"{tempCode}{DateTime.Now:ddMMyy}";

                    await UnitOfWork.CustomerRepo.SaveAsync();

                    var host = Request.Url.Scheme + "://" + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    //Khong gui email xac thuc
                    //Phần gửi mail xác thực tài khoản
                    var domain = ConfigurationManager.AppSettings["Domain"];
                    var filePath = AppDomain.CurrentDomain.BaseDirectory + @"/EmailTemplates/account/activeAccount.html";
                    var html = System.IO.File.ReadAllText(filePath);
                    var activeEmail = new ActiveEmail(customer.Email, customer.SystemId, DateTime.Now, domain);
                    html = html.Replace("{{linkActive}}", activeEmail.Link);

                    MailHelper.SendMail(customer.Email, Resource.XacThucTaiKhoan + domain, html, false);
                    //MailHelper.SendMail(customer.Email, "Xác thực tài khoản trên " + domain, html, false);

                    customer.CodeActive = activeEmail.Code;
                    customer.CreateDateActive = activeEmail.SendDate;

                    UnitOfWork.CustomerRepo.Update(customer);
                    await UnitOfWork.CustomerRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    OutputLog.WriteOutputLog(ex);
                    transaction.Rollback();
                    ModelState.AddModelError("CustomError", Resource.AccountRegister_ErrorRegister);
                    return View();
                    //throw;
                }
            }

            return View("RegisterSuccess");
        }

        /// <summary>
        /// Xac thuc link kich hoat
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>

        [Route("{culture}/Account/Active-{url}")]
        public ActionResult Active(string url)
        {
            var activeEmail = new ActiveEmail();
            activeEmail.FromString(url);

            var customer = UnitOfWork.CustomerRepo.SingleOrDefault(
               x => x.Email.Equals(activeEmail.Email) && x.SystemId == activeEmail.SystemId && x.CodeActive.Equals(activeEmail.Code));

            if (customer == null || (customer.CreateDateActive.HasValue && Math.Abs(customer.CreateDateActive.Value.Subtract(activeEmail.SendDate).TotalMilliseconds) < 0.001))
            {
                ViewBag.Mess = Resource.AccountActive_NotLink;
                ViewBag.Status = "0";
                return View();
            }

            if (customer.IsActive)
            {
                // ViewBag.Mess = Resource.Currency "Tài khoản đã được xác thực!";
                ViewBag.Mess = Resource.AccountActive_AccountIsActive;
                ViewBag.Status = "0";
                return View();
            }

            //using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            //{
            //    try
            //    {
            customer.DateActive = DateTime.Now;
            customer.IsActive = true;

            UnitOfWork.CustomerRepo.Update(customer);
            UnitOfWork.CustomerRepo.SaveAsync();
            //transaction.Commit();

            ViewBag.Mess = Resource.AccountActive_AccountActiveComplete;
            ViewBag.Status = "1";
            //    }
            //    catch (Exception)
            //    {
            //        transaction.Rollback();

            //        throw;
            //    }
            //}

            return View();
        }

        [Route("{culture}/Account/PassForget")]
        public ActionResult PassForget()
        {
            if (Request.IsAuthenticated)
                return Redirect("/");
            if (CustomerState.Email != null)
                return Redirect("/");
            ViewBag.Status = "0";
            return View();
        }

        [HttpPost]
        [Route("{culture}/Account/PassForget")]
        public ViewResult PassForget(CustomerForgotMeta forgot)
        {
            if (Request.Url != null)
            {
                var host = Request.Url.Scheme + "://" + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            }
            //lấy id của website từ webconfig
            if (!ModelState.IsValid)
            {
                ViewBag.Status = "0";
                return View();
            }

            var customer = UnitOfWork.CustomerRepo.SingleOrDefault(x => x.Email.Equals(forgot.Email) && x.SystemId == SystemId && x.IsActive);

            //{
            //    ViewBag.Status = "0";
            //    ViewBag.MessError = "Tài khoản không tồn tại hoặc chưa được đăng ký vui lòng kiểm tra lại";
            //}
            if (customer != null)
            {
                //Phần gửi mail link nhập mới mật khẩu
                var domain = ConfigurationManager.AppSettings["Domain"];
                var filePath = AppDomain.CurrentDomain.BaseDirectory + @"/EmailTemplates/account/forgetAccount.html";
                var html = System.IO.File.ReadAllText(filePath);
                var forgetEmail = new Items.ForgetEmail(customer.Email, customer.SystemId, DateTime.Now, domain);
                html = html.Replace("{{linkActive}}", forgetEmail.Link);

                MailHelper.SendMail(customer.Email, Resource.YCLayLaiMK + domain, html, false);
                //MailHelper.SendMail(customer.Email, "Yêu cầu lấy lại mật khẩu trên " + domain, html, false);

                customer.Url = forgetEmail.Url;
                UnitOfWork.CustomerRepo.Save();
            }
            else
            {
                TempData["MsgErr"] = Resource.EmailKhongTonTai ;
                //TempData["MsgErr"] = $"Email không tồn tại hoặc bị khóa xin vui lòng liên hệ với chúng tôi để được giải đáp!";
                ViewBag.Status = "0";
                return View("PassForget");
            }
            ViewBag.Status = "1";
            ViewBag.Mess = Resource.AccountPassForget_EmailChangePass;
            TempData["Msg"] = Resource.EmailChinhXac;
            //TempData["Msg"] = $"Email chính xác!";

            return View();
        }

        /// <summary>
        /// tạo mới mật khẩu
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>

        [Route("{culture}/Account/NewPass-{url}")]
        public ActionResult NewPass(string url)
        {
            var forgetEmail = new Common.MailHelper.ForgetEmail();
            forgetEmail.FromString(url);

            var customerNewPassMeta = new CustomerNewPassMeta();
            customerNewPassMeta.Email = forgetEmail.Email;

            //1. Kiểm tra thông tin khách hàng hiện tại
            var customer = UnitOfWork.CustomerRepo.SingleOrDefault(x => x.Url == url && x.SystemId == SystemId && x.IsActive);
            if (customer == null || string.IsNullOrEmpty(url))
            {
                ViewBag.Mess = Resource.LinkDaDuocSuDung ;
                //ViewBag.Mess = "Link đã được sử dụng!";
            }

            ViewBag.Status = "0";
            return View(customerNewPassMeta);
        }

        [HttpPost]
        [Route("{culture}/Account/NewPass-{url}")]
        public async Task<ActionResult> NewPass(CustomerNewPassMeta newPass, string url)
        {
            //lấy id của website từ webconfig
            if (!ModelState.IsValid)
            {
                ViewBag.Status = "0";
                return View(newPass);
            }

            //1. Kiểm tra thông tin khách hàng hiện tại
            var customer = UnitOfWork.CustomerRepo.SingleOrDefault(x => x.Url == url && x.SystemId == SystemId && x.IsActive);
            if (customer == null || string.IsNullOrEmpty(url))
            {
                ViewBag.Mess = Resource.ChangePassError;
                //ViewBag.Mess = "Thay đổi mật khẩu không thành công!";
                ViewBag.Status = "0";
                return View(newPass);
            }
            else
            {
                customer.Password = PasswordEncrypt.PasswordEncrypt.EncodePassword(newPass.Password.Trim(), PasswordSalt.FinGroupApiCustomer);
                customer.Url = "";
                UnitOfWork.CustomerRepo.Update(customer);
                await UnitOfWork.CustomerRepo.SaveAsync();

                ViewBag.Mess = Resource.AccountPassForget_ChangePassComplete;
                //"Thay đổi mật khẩu thành công!";
                ViewBag.Status = "1";
            }

            return View();
        }

        //Lay danh sach huyen tinh thanh pho quoc gia
        public ActionResult GetCityByCountry(string countryId)
        {
            countryId = countryId.ToUpper();
            var provinces = UnitOfWork.ProvinceRepo.Find(x => x.Culture == countryId).ToList();
            return PartialView(provinces);
        }

        public ActionResult GetDistrictByCity(int provinceId)
        {
            var listDistrict = UnitOfWork.DistrictRepo.Find(x => x.ProvinceId == provinceId).ToList();
            return PartialView(listDistrict);
        }

        public ActionResult GetDistrict(int? provinceId)
        {
            var listDistrict = UnitOfWork.DistrictRepo.Find(x => x.ProvinceId == provinceId).ToList();
            return PartialView(listDistrict);
        }

        public ActionResult GetWard(int? districtId)
        {
            var listWard = UnitOfWork.WardRepo.Find(x => x.DistrictId == districtId).ToList();
            return PartialView(listWard);
        }

        //todo thay đổi mật khẩu
        [Route("{culture}/Account/ChangePass")]
        public ActionResult ChangePass()
        {
            ViewBag.ChangePass = "cl_on";
            ViewBag.Status = "0";
            return View();
        }

        [HttpPost]
        [Route("{culture}/Account/ChangePass")]
        public async Task<ActionResult> ChangePass(CustomerChangePassMeta item)
        {
            ViewBag.ChangePass = "cl_on";
            if (!ModelState.IsValid)
            {
                ViewBag.Status = "0";
                return View("ChangePass", item);
            }
            var passOld = PasswordEncrypt.PasswordEncrypt.EncodePassword(item.PasswordOld.Trim(), PasswordSalt.FinGroupApiCustomer);
            var customer = UnitOfWork.CustomerRepo.SingleOrDefault(x => x.Id == CustomerState.Id && x.Password == passOld);

            if (customer != null)
            {
                customer.Password = PasswordEncrypt.PasswordEncrypt.EncodePassword(item.Password.Trim(), PasswordSalt.FinGroupApiCustomer);
                UnitOfWork.CustomerRepo.Update(customer);
                await UnitOfWork.CustomerRepo.SaveAsync();
            }
            else
            {
                TempData["MsgErr"] =Resource.AccountPassForget_ChangePassError;
                //TempData["MsgErr"] = $"Xảy ra lỗi trong quá trình thay đổi mật khẩu. Bạn vui lòng thử lại trong giây lát!";
                ViewBag.Status = "0";
                return View("ChangePass", item);
            }
            TempData["Msg"] = Resource.AccountPassForget_ChangePassComplete;
            //TempData["Msg"] = $"Thay đổi mật khẩu thành công!";
            ViewBag.Status = "1";

            return View("ChangePass", item);
        }

        //todo thông tin tài khoản
        [Route("{culture}/Account/Infor")]
        public ActionResult Infor()
        {
            ViewBag.Infor = "cl_on";
            var list = new List<Office>();
            list.Add(new Office { Id = -1, Name = Resource.InforAccount_Storage });
            //list.Add(new Office { Id = -1, Name = "--Chọn kho hàng về--" });
            var allWarehouseDelivery = UnitOfWork.OfficeRepo.FindAsNoTracking(
                 x => !x.IsDelete && x.Type == (byte)OfficeType.Warehouse && x.Status == (byte)OfficeStatus.Use && x.Culture == "VN").ToList();
            list.AddRange(allWarehouseDelivery);
            ViewBag.ListWardDelivery = list;

            var tmpCustomer = UnitOfWork.CustomerRepo.SingleOrDefault(m => m.Id == CustomerState.Id);
            var model = new CustomerUpdateMeta()
            {
                Email = tmpCustomer.Email,
                Address = tmpCustomer.Address,
                Avatar = tmpCustomer.Avatar,
                CountryId = tmpCustomer.CountryId,
                DistrictId = tmpCustomer.DistrictId,
                DistrictName = tmpCustomer.DistrictName,
                FullName = tmpCustomer.FullName,
                Id = tmpCustomer.Id,
                Nickname = tmpCustomer.Nickname,
                Phone = tmpCustomer.Phone,
                ProvinceId = tmpCustomer.ProvinceId,
                ProvinceName = tmpCustomer.ProvinceName,
                CardBank = tmpCustomer.CardBank,
                CardBranch = tmpCustomer.CardBranch,
                CardId = tmpCustomer.CardId,
                CardName = tmpCustomer.CardName,
                GenderName = tmpCustomer.GenderName,
                GenderId = tmpCustomer.GenderId,
                Birthday = tmpCustomer.Birthday,
                WarehouseId = tmpCustomer.WarehouseId
            };
            if (!string.IsNullOrEmpty(tmpCustomer.CountryId) || tmpCustomer.CountryId != "0")
            {
                ViewBag.listProvince = UnitOfWork.ProvinceRepo.Find(x => x.Culture == tmpCustomer.CountryId);
                ViewBag.listDistrict = UnitOfWork.DistrictRepo.Find(x => x.Culture == tmpCustomer.CountryId);
            }
            return View(model);
        }

        [HttpPost]
        [Route("{culture}/Account/Infor")]
        public int Infor(CustomerUpdateMeta item)
        {
            // var host = Request.Url.Scheme + "://" + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            var result = 0;
            ViewBag.listProvince = new List<Province>();
            ViewBag.listDistrict = new List<District>();

            //Lấy về khách hàng cần cập nhật thông tin
            var tmpCustomer = UnitOfWork.CustomerRepo.SingleOrDefault(m => m.Id == CustomerState.Id);

            if (tmpCustomer != null)
            {
                //Lấy về kho nhận hàng
                var allWarehouseDelivery = UnitOfWork.OfficeRepo.FirstOrDefault(
                     x => !x.IsDelete && x.Id == item.WarehouseId && x.Culture == "VN");
                if (allWarehouseDelivery != null)
                {
                    tmpCustomer.WarehouseId = item.WarehouseId;
                    tmpCustomer.WarehouseName = allWarehouseDelivery.Name;
                }

                //if (!string.IsNullOrEmpty(tmpCustomer.FullName))
                //{
                //tmpCustomer.FullName = item.FullName;
                //}
                tmpCustomer.FullName = item.FullName;
                tmpCustomer.ProvinceId = item.ProvinceId;
                tmpCustomer.ProvinceName = item.ProvinceName;
                tmpCustomer.DistrictId = item.DistrictId;
                tmpCustomer.DistrictName = item.DistrictName;
                tmpCustomer.Avatar = item.Avatar;
                tmpCustomer.Address = item.Address;
                tmpCustomer.Phone = item.Phone;
                tmpCustomer.CountryId = item.CountryId;
                tmpCustomer.CardName = item.CardName;
                tmpCustomer.CardId = item.CardId;
                tmpCustomer.CardBranch = item.CardBranch;
                tmpCustomer.CardBank = item.CardBank;
                tmpCustomer.GenderId = item.GenderId;
                tmpCustomer.GenderName = item.GenderName;
                tmpCustomer.Birthday = item.Birthday;

                UnitOfWork.CustomerRepo.Update(tmpCustomer);
                result = UnitOfWork.CustomerRepo.SaveNoCheck();
                if (!string.IsNullOrEmpty(tmpCustomer.CountryId) || tmpCustomer.CountryId != "0")
                {
                    ViewBag.listProvince = UnitOfWork.ProvinceRepo.Find(x => x.Culture == tmpCustomer.CountryId);
                    ViewBag.listDistrict = UnitOfWork.DistrictRepo.Find(x => x.Culture == tmpCustomer.CountryId);
                }
                else
                {
                    ViewBag.listProvince = new List<Province>();
                    ViewBag.listDistrict = new List<District>();
                }
            }
            return result;
        }

        [HttpPost]
        public JsonResult FileUploadHandler()
        {
            var result = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                if (file != null)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var firstName = Path.GetFileNameWithoutExtension(file.FileName);
                    string fileName = file.FileName;
                    string path = @"/Areas/CMS/Content/avatar/";
                    if (!Directory.Exists(Server.MapPath(path)))
                    {
                        Directory.CreateDirectory(Server.MapPath(path));
                    }
                    string filePath = path + fileName;
                    if (System.IO.File.Exists(Server.MapPath(filePath)))
                    {
                        Random rnd = new Random();
                        int number = rnd.Next(1000);
                        filePath = path + firstName + number.ToString() + extension;
                    }
                    fileName = Server.MapPath(filePath);
                    file.SaveAs(fileName);
                    result = filePath;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Tạo tài khoản chính thức cho khách hàng tiềm năng
        [Route("{culture}/CreateAccount-{url}")]
        public ActionResult CreateAccount(string url)
        {
            var createAccount = new Common.Items.CreateAccount();
            createAccount.FromString(url);

            var potentialCustomer = UnitOfWork.PotentialCustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Status == (byte)PotentialCustomerStatus.Await && x.Email == createAccount.Email);

            if (potentialCustomer == null)
            {
                return View("CreateAccountError");
            }

            var cus = new CustomerRegisterMeta()
            {
                Id = potentialCustomer.Id,
                Email = potentialCustomer.Email,
                Phone = potentialCustomer.Phone,
                FullName = potentialCustomer.FullName,
                ProvinceId = potentialCustomer.ProvinceId.Value,
                ProvinceName = potentialCustomer.ProvinceName,
                DistrictId = potentialCustomer.DistrictId.Value,
                WardId = potentialCustomer.WardId.Value,
                WardsName = potentialCustomer.WardsName,
                Address = potentialCustomer.Address
            };

            return View(cus);
        }

        [Route("{culture}/CreateAccount-{url}")]
        [HttpPost]
        public async Task<ActionResult> CreateAccount(CustomerRegisterMeta cusCreate)
        {
            //lấy id của website từ webconfig
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("CustomError", $"Không thể tạo được tài khoản");
                return View(cusCreate);
            }

            //2. Kiểm tra lại đối tượng người dùng
            var subjectType = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => x.Idd == (int)EnumAccountantSubject.Customer);

            var potentialCustomer = await UnitOfWork.PotentialCustomerRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Status == (byte)PotentialCustomerStatus.Await && x.Id == cusCreate.Id);
            if (potentialCustomer != null)
            {
                using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var timeNow = DateTime.Now;
                        //Xóa bản ghi khách hàng tiềm năng

                        potentialCustomer.IsDelete = true;
                        potentialCustomer.Updated = timeNow;

                        await UnitOfWork.PotentialCustomerRepo.SaveAsync();

                        //Tạo khách hàng chính thức gắn với nhân viên phụ trách khách hàng tiềm năng này

                        var customer = new Customer
                        {
                            FullName = potentialCustomer.FullName,
                            Email = potentialCustomer.Email,
                            Phone = potentialCustomer.Phone,
                            Avatar = "/Content/img/no-avatar.png",
                            Balance = 0,
                            BalanceAvalible = 0,
                            Code = string.Empty,
                            SystemId = SystemId,
                            Password = PasswordEncrypt.PasswordEncrypt.EncodePassword(cusCreate.Password.Trim(), PasswordSalt.FinGroupApiCustomer)
                        };
                        //customer SystemName;
                        customer.Nickname = potentialCustomer.Nickname;

                        var level = await UnitOfWork.CustomerLevelRepo.FirstOrDefaultAsync(x => x.StartMoney >= 0 && 0 < x.EndMoney);

                        customer.LevelId = (byte)level.Id;
                        customer.LevelName = level.Name;
                        customer.Point = 0;
                        customer.CountryId = CountryName.TL.ToString();
                        //customer.CountryId = CountryName.VN.ToString();
                        customer.ProvinceId = potentialCustomer.ProvinceId;
                        customer.ProvinceName = potentialCustomer.ProvinceName;
                        customer.DistrictId = potentialCustomer.DistrictId;
                        customer.DistrictName = potentialCustomer.DistrictName;
                        customer.WardId = potentialCustomer.WardId;
                        customer.WardsName = potentialCustomer.WardsName;
                        customer.Address = potentialCustomer.Address;
                        customer.Created = timeNow;
                        customer.Updated = timeNow;
                        customer.LoginFailureCount = 0;
                        customer.HashTag = customer.FullName + "," + customer.Nickname + "," + customer.Email;
                        customer.Balance = 0;
                        customer.BalanceAvalible = 0;
                        customer.IsActive = true;
                        customer.IsLockout = false;
                        customer.IsDelete = false;
                        customer.UserId = potentialCustomer.UserId;
                        customer.UserFullName = potentialCustomer.UserFullName;
                        customer.OfficeId = potentialCustomer.OfficeId;
                        customer.OfficeName = potentialCustomer.OfficeName;
                        customer.OfficeIdPath = potentialCustomer.OfficeIdPath;

                        if (subjectType != null)
                        {
                            customer.TypeId = subjectType.Id;
                            customer.TypeIdd = subjectType.Idd;
                            customer.TypeName = subjectType.SubjectName;
                        }

                        UnitOfWork.CustomerRepo.Add(customer);
                        await UnitOfWork.CustomerRepo.SaveAsync();

                        //Cập nhật mã code khách hàng
                        var tempCode = UnitOfWork.CustomerRepo.Count(x =>
                           x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                           x.Created.Day == DateTime.Now.Day && x.Id <= customer.Id);

                        customer.Code = $"{tempCode}{DateTime.Now:ddMMyy}";

                        await UnitOfWork.CustomerRepo.SaveAsync();

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("CustomError", $"Không thể tạo được tài khoản");
                        return View();
                        //throw;
                    }
                }
            }
            else
            {
                ModelState.AddModelError("CustomError", $"Không thể tạo được tài khoản");
                return View();
            }

            return View("CreateAccountSuccess");
        }
    }
}