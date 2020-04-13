using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Cms.Attributes;
using Common.Constant;
using Common.Emums;
using Common.Helper;
using Common.Items;
using Common.PasswordEncrypt;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Library.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Web;

namespace Cms.Controllers
{
   // [Authorize]
    public class UserController : BaseController
    {
        // GET: User
        [LogTracker(EnumAction.Add, EnumPage.Staff)]
        public async Task<ActionResult> Index(ModelView<UserResult, UserFilterViewModel> model)
        {
            if (model.SearchInfo == null)
                model.SearchInfo = new UserFilterViewModel() { HasChilds = true, OfficeId = 1, OfficeIdPath = "1" };

            int totalRecord;

            model.SearchInfo.Keyword = MyCommon.Ucs2Convert(model.SearchInfo.Keyword);

            model.Items = await UnitOfWork.UserRepo.Search(model.SearchInfo, model.PageInfo.CurrentPage, model.PageInfo.PageSize,
                    out totalRecord);

            model.PageInfo.TotalRecord = totalRecord;
            model.PageInfo.Name = "Staff";
            model.PageInfo.Url = Url.Action("Index", "User");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", model);
            }

            ViewBag.officeJsTree = await GetOfficeJsTree();

            var groupPermission =
                await UnitOfWork.GroupPermissionRepo.FindAsNoTrackingAsync(x => !x.IsDefault && !x.IsDelete);
            ViewBag.GroupPermission = groupPermission.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).OrderBy(x=> x.Text).ToList();

            var listAccSub = UnitOfWork.AccountantSubjectRepo.Find(m => !m.IsDelete && !m.IsIdSystem && m.SubjectName != "Customer").Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.SubjectName }).ToList();
            ViewBag.ListAccSub = listAccSub;

            return View(model);
        }

        // GET: User/Details/5
        [LogTracker(EnumAction.View, EnumPage.Staff)]
        public ActionResult Details(int id)
        {
            return View();
        }

        [LogTracker(EnumAction.View, EnumPage.Profile)]
        public new async Task<ActionResult> Profile()
        {
            var user = await UnitOfWork.UserRepo.GetUser(UserState.UserId);
            return View(user);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.Profile)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordMeta model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(", ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList());

                return JsonCamelCaseResult(new { Status = -1, Text = $"Error: {errorMessage}" },
                    JsonRequestBehavior.AllowGet);
            }

            var oldPassword = PasswordEncrypt.EncodePassword(model.OldPassword.Trim(), PasswordSalt.Cms);

            var user = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => x.Id == UserState.UserId && x.Password == oldPassword);

            if (user == null)
                return JsonCamelCaseResult(new {Status = -1, Text = $"Old password is not correct" },
                    JsonRequestBehavior.AllowGet);

            user.Password = PasswordEncrypt.EncodePassword(model.NewPassword.Trim(), PasswordSalt.Cms);
            user.Updated = DateTime.Now;

            await UnitOfWork.UserRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = 1, Text = $"Password update successful" },
                    JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.Profile)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeAvatar(string avatar)
        {
            var user = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => x.Id == UserState.UserId);

            if (user == null)
                return JsonCamelCaseResult(new { Status = -1, Text = $"Old password is not correct" },
                    JsonRequestBehavior.AllowGet);

            user.Avatar = avatar;
            user.Updated = DateTime.Now;

            await UnitOfWork.UserRepo.SaveAsync();

            // Cập nhật lại Claim
            UserState.Avatar = avatar;
            AddUpdateClaim("userState", UserState.ToString());

            return JsonCamelCaseResult(new { Status = 1, Text = $"Password update successful" },
                    JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.Profile)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeFullname(string fullname)
        {
            var user = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => x.Id == UserState.UserId);

            if (user == null)
                return JsonCamelCaseResult(new { Status = -1, Text = $"User account does not exist or has been deleted!" },
                    JsonRequestBehavior.AllowGet);

            user.FullName = fullname;
            user.Updated = DateTime.Now;

            await UnitOfWork.UserRepo.SaveAsync();

            var currentDate = DateTime.Now;
            var userPosition = UnitOfWork.UserPositionRepo.SingleOrDefault(x => x.IsDefault && x.UserId == user.Id);
            var office = UnitOfWork.OfficeRepo.SingleOrDefault(x => !x.IsDelete && x.Id == userPosition.OfficeId);
            var userSate = new UserState();
            userSate.FromUser(user, userPosition, office);
            userSate.SessionId = $"{currentDate.Ticks}_{user.Id}";

            IdentitySignin(userSate, true);

            return JsonCamelCaseResult(new { Status = 1, Text = $"User name update successful" },
                    JsonRequestBehavior.AllowGet);
        }

        public void IdentitySignin(UserState userState, bool isPersistent = false)
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

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        // GET: User/Create
       // [LogTracker(EnumAction.Add, EnumPage.Staff)]
        public async Task<ActionResult> Create()
        {
            ViewBag.MaxFileLength = GetMaxFileLength();
            ViewBag.officeJsTree = await GetOfficeJsTree();
            var listAccSub = UnitOfWork.AccountantSubjectRepo.Find(m => !m.IsDelete && !m.IsIdSystem && m.SubjectName != "KHACHHANG").Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.SubjectName }).ToList();
            ViewBag.ListAccSub = listAccSub;

            var groupPermission =
                await UnitOfWork.GroupPermissionRepo.FindAsNoTrackingAsync(x => !x.IsDefault && !x.IsDelete);

            ViewBag.GroupPermission = groupPermission.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).OrderBy(x => x.Text).ToList();

            
            return View();
        }

        // POST: User/QuickAdd
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.Staff)]
        public async Task<ActionResult> QuickAdd(QuickAddUserMeta model)
        {
            ModelState.Remove("IsCompany");
            if (!ModelState.IsValid)
            {
                return JsonCamelCaseResult(ModelState.Values.SelectMany(v => v.Errors), JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra tài khoản nhân viên
            if (await UnitOfWork.UserRepo.AnyAsync(x => x.UserName.Equals(model.UserName) && !x.IsDelete))
            {
                return JsonCamelCaseResult("This account already exists in system", JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra đơn vị tồn tại
            var office = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == model.OfficeId);
            if (office == null)
            {
                return JsonCamelCaseResult("Unit does not exist or has been deleted", JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra Position tồn tại
            var title = await UnitOfWork.TitleRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == model.TitleId);
            if (title == null)
            {
                return JsonCamelCaseResult("position does not exist or has been deleted", JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra đơn vị tồn tại trong Position
            if (!await UnitOfWork.PositionRepo.AnyAsync(x => x.OfficeId == office.Id && x.TitleId == title.Id))
            {
                return JsonCamelCaseResult("Position is not declared in this unit", JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra groupPermission
            GroupPermision groupPermision = null;
            if (model.GroupPermisionId.HasValue)
            {
                groupPermision = await UnitOfWork.GroupPermissionRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == model.GroupPermisionId.Value);

                if (groupPermision == null)
                {
                    return JsonCamelCaseResult("Right to access does not exist or has been deleted", JsonRequestBehavior.AllowGet);
                }
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var user = Mapper.Map<User>(model);

                    user.LastUpdateUserId = UserState.UserId;
                    user.IsDelete = false;
                    user.Created = DateTime.Now;
                    user.Updated = DateTime.Now;
                    user.Gender = default(byte);
                    user.Status = 1;
                    user.IsLockout = false;
                    user.IsSystem = false;
                    user.Email = string.Empty;
                    user.FullName = user.FullName.Trim();
                    user.LastName = user.FullName.Substring(user.FullName.LastIndexOf(' ') + 1);
                    user.UnsignName = MyCommon.Ucs2Convert($"{user.FullName} {user.UserName} {user.Email}");
                    user.Password = PasswordEncrypt.EncodePassword(user.Password.Trim(), PasswordSalt.Cms);
                    var accSub = UnitOfWork.AccountantSubjectRepo.Find(m => m.Id == model.TypeId).FirstOrDefault();
                    if (accSub != null)
                    {
                        user.TypeId = accSub.Id;
                        user.TypeIdd = accSub.Idd;
                        user.TypeName = accSub.SubjectName;
                    }
                    // Tách FullName
                    var splitFullName = user.FullName.Split(' ');
                    switch (splitFullName.Length)
                    {
                        case 1:
                            user.FirstName = model.FullName;
                            break;
                        case 2:
                            user.FirstName = splitFullName[1];
                            user.LastName = splitFullName[0];
                            break;
                        default:
                            if (splitFullName.Length > 2)
                            {
                                user.FirstName = splitFullName[splitFullName.Length - 1];
                                user.LastName = splitFullName[0];
                                for (var i = 1; i < splitFullName.Length - 1; i++)
                                {
                                    if (string.IsNullOrWhiteSpace(splitFullName[i]))
                                    {
                                        continue;
                                    }
                                    user.MidleName += splitFullName[i] + " ";
                                }
                                user.MidleName = user.MidleName.Trim();
                            }
                            break;
                    }

                    // Tạo Avatar
                    var name = $"{DateTime.Now:ddMMyyyyssfffffff}_{Path.GetFileName(model.UserName)}";

                    // build path folder
                    var path = $"/upload/{DateTime.Now:yyyy/MM/dd}/";

                    // create folder
                    var mapPath = Server.MapPath(path);
                    if (!Directory.Exists(mapPath))
                        Directory.CreateDirectory(mapPath);

                    var fullName = name + ".png";

                    var url = Encryptor.Base64Encode(path + fullName);

                    // buidl full path on sẻver
                    MyCommon.GenerateAvatar(user.FirstName, user.LastName, mapPath + fullName);

                    user.Avatar = url;

                    UnitOfWork.UserRepo.Add(user);

                    var rs = await UnitOfWork.UserRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
                    }

                    // Insert thông tin Position
                    var userPossion = new UserPosition()
                    {
                        UserId = user.Id,
                        OfficeId = office.Id,
                        OfficeName = office.Name,
                        OfficeIdPath = office.IdPath,
                        OfficeNamePath = office.NamePath,
                        TitleId = title.Id,
                        TitleName = title.Name,
                        Type = model.Type,
                        IsDefault = true
                    };

                    if (groupPermision != null)
                    {
                        userPossion.GroupPermisionId = groupPermision.Id;
                        userPossion.GroupPermissionName = groupPermision.Name;
                    }

                    UnitOfWork.UserPositionRepo.Add(userPossion);
                    await UnitOfWork.UserPositionRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
        }


        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.Staff)]
        public async Task<ActionResult> Create(UserMeta model)
        {
            ModelState.Remove("Id");
            var listAccSub = UnitOfWork.AccountantSubjectRepo.Find(m => !m.IsDelete && !m.IsIdSystem && m.SubjectName != "KHACHHANG").Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.SubjectName }).ToList();
            ViewBag.ListAccSub = listAccSub;
            ViewBag.MaxFileLength = GetMaxFileLength();
            ViewBag.officeJsTree = await GetOfficeJsTree();

            var groupPermission =
                await UnitOfWork.GroupPermissionRepo.FindAsNoTrackingAsync(x => !x.IsDefault && !x.IsDelete);

            ViewBag.GroupPermission = groupPermission.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).OrderBy(x => x.Text).ToList();

            if (!ModelState.IsValid)
                return View();

            // Kiểm tra tài khoản nhân viên
            if (await UnitOfWork.UserRepo.AnyAsync(x => x.UserName.Equals(model.UserName) && !x.IsDelete))
            {
                ModelState.AddModelError("UserName", @"This account already exists in the system");
                return View();
            }

            // Kiểm tra email nhân viên
            if (await UnitOfWork.UserRepo.AnyAsync(x => x.Email.Equals(model.Email) && !x.IsDelete))
            {
                ModelState.AddModelError("Email", @"This email already exists in the system");
                return View();
            }

            // Kiểm tra đủ độ tuổi lao động
            if (model.Birthday.HasValue && DateTime.Now.AddYears(-16).Subtract(model.Birthday.Value).TotalDays < 0)
            {
                ModelState.AddModelError("Birthday", @"Date of birth not enough working age");
                return View();
            }

            // check Date start work phải lớn hơn Birthday + 16
            if (model.Birthday.HasValue && (model.StartDate.HasValue && model.StartDate.Value.Subtract(model.Birthday.Value).TotalDays < 16))
            {
                ModelState.AddModelError("StartDate", @"The date of commencement of work must be greater than the date of birth and must be of working age >= 16 age");
                return View();
            }

            // Kiểm tra đơn vị tồn tại
            var office = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == model.OfficeId);
            if (office == null)
            {
                ModelState.AddModelError("OfficeId", @"unit does not exist or has been deleted");
                return View();
            }

            // Kiểm tra Position tồn tại
            var title = await UnitOfWork.TitleRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == model.TitleId);
            if (title == null)
            {
                ModelState.AddModelError("TitleId", @"Position does not exist or has been deleted");
                return View();
            }

            // Kiểm tra đơn vị tồn tại trong Position
            if (!await UnitOfWork.PositionRepo.AnyAsync(x => x.OfficeId == office.Id && x.TitleId == title.Id))
            {
                ModelState.AddModelError("TitleId", @"Position is not declared in this unit");
                return View();
            }

            // Kiểm tra groupPermission
            GroupPermision groupPermision = null;
            if (model.GroupPermisionId.HasValue)
            {
                groupPermision = await UnitOfWork.GroupPermissionRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == model.GroupPermisionId.Value);

                if (groupPermision == null)
                {
                    ModelState.AddModelError("GroupPermisionId", @"right to access does not exist or has been deleted");
                    return View();
                }
            }

            if (string.IsNullOrWhiteSpace(model.Avatar))
            {
                var name = $"{DateTime.Now:ddMMyyyyssfffffff}_{Path.GetFileName(model.UserName)}";

                // build path folder
                var path = $"/upload/{DateTime.Now:yyyy/MM/dd}/";

                // create folder
                var mapPath = Server.MapPath(path);
                if (!Directory.Exists(mapPath))
                    Directory.CreateDirectory(mapPath);

                var fullName = name + ".png";

                var url = Encryptor.Base64Encode(path + fullName);

                // buidl full path on sẻver
                MyCommon.GenerateAvatar(model.FirstName, model.LastName, mapPath + fullName);

                model.Avatar = url;
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var user = Mapper.Map<User>(model);

                    user.LastUpdateUserId = UserState.UserId;
                    user.IsDelete = false;
                    user.Created = DateTime.Now;
                    user.Updated = DateTime.Now;
                    user.FullName = string.IsNullOrWhiteSpace(user.MidleName)
                        ? $"{user.LastName} {user.FirstName}"
                        : $"{user.LastName} {user.MidleName} {user.FirstName}";

                    user.UnsignName = MyCommon.Ucs2Convert($"{user.FullName} {user.UserName} {user.Email}");
                    user.Password = PasswordEncrypt.EncodePassword(user.Password.Trim(), PasswordSalt.Cms);
                    user.IsLockout = false;
                    user.IsSystem = false;
                    var accSub = UnitOfWork.AccountantSubjectRepo.Find(m => m.Id == model.TypeId).FirstOrDefault();
                    if (accSub != null)
                    {
                        user.TypeId = accSub.Id;
                        user.TypeIdd = accSub.Idd;
                        user.TypeName = accSub.SubjectName;
                    }
                    UnitOfWork.UserRepo.Add(user);

                    var rs = await UnitOfWork.UserRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View();
                    }

                    var userPosition = new UserPosition()
                    {
                        UserId = user.Id,
                        OfficeId = office.Id,
                        OfficeName = office.Name,
                        OfficeIdPath = office.IdPath,
                        OfficeNamePath = office.NamePath,
                        TitleId = title.Id,
                        TitleName = title.Name,
                        Type = model.Type,
                        IsDefault = true
                    };

                    if (groupPermision != null)
                    {
                        userPosition.GroupPermisionId = groupPermision.Id;
                        userPosition.GroupPermissionName = groupPermision.Name;
                    }

                    UnitOfWork.UserPositionRepo.Add(userPosition);
                    await UnitOfWork.UserPositionRepo.SaveAsync();

                    TempData["Msg"] = $"More successful staff \"<b>{user.FullName}</b>\"";

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return RedirectToAction("Create");
        }

        // GET: User/Edit/5
        [LogTracker(EnumAction.Update, EnumPage.Staff)]
        public async Task<ActionResult> Edit(int id)
        {
            var user = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (user == null)
                return HttpNotFound($"No staff has Id {id}");

            ViewBag.MaxFileLength = GetMaxFileLength();
            ViewBag.officeJsTree = await GetOfficeJsTree();

            var model = Mapper.Map<UserMeta>(user);

            var userPossion = await UnitOfWork.UserPositionRepo.SingleOrDefaultAsync(x => x.IsDefault && x.UserId == user.Id);
            model.OfficeId = userPossion.OfficeId;
            model.OfficeName = userPossion.OfficeName;
            model.TitleId = userPossion.TitleId;
            model.TitleName = userPossion.TitleName;
            model.Type = userPossion.Type;
            model.GroupPermisionId = userPossion.GroupPermisionId;
            model.GroupPermissionName = userPossion.GroupPermissionName;

            var groupPermission =
                await UnitOfWork.GroupPermissionRepo.FindAsNoTrackingAsync(x => !x.IsDefault && !x.IsDelete);
            ViewBag.GroupPermission = groupPermission.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).OrderBy(x => x.Text).ToList();

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            ViewBag.GroupPermissionJson = JsonConvert.SerializeObject(groupPermission.Select(x => new { x.Id, x.Name }).OrderBy(x=> x.Name), jsonSerializerSettings);

            var listAccSub = UnitOfWork.AccountantSubjectRepo.Find(m => !m.IsDelete && !m.IsIdSystem && m.SubjectName != "Customer").Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.SubjectName }).ToList();
            ViewBag.ListAccSub = listAccSub;
            return View(model);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Update, EnumPage.Staff)]
        public async Task<ActionResult> Edit(UserMeta model)
        {
            var listAccSub = UnitOfWork.AccountantSubjectRepo.Find(m => !m.IsDelete && !m.IsIdSystem && m.SubjectName != "KHACHHANG").Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.SubjectName }).ToList();
            ViewBag.ListAccSub = listAccSub;
            ViewBag.MaxFileLength = GetMaxFileLength();
            ViewBag.officeJsTree = await GetOfficeJsTree();
            var groupPermission =
                await UnitOfWork.GroupPermissionRepo.FindAsNoTrackingAsync(x => !x.IsDefault && !x.IsDelete);

            ViewBag.GroupPermission = groupPermission.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).OrderBy(x => x.Text).ToList();

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            ViewBag.GroupPermissionJson = JsonConvert.SerializeObject(groupPermission.Select(x => new { x.Id, x.Name }).OrderBy(x=> x.Name), jsonSerializerSettings);

            ModelState.Remove("IsCompany");
            if (!ModelState.IsValid)
                return View(model);

            var user = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            if (user == null)
            {
                ModelState.AddModelError("NotExist", @"Staff does not exist or has been deleted");
                return View(model);
            }

            // Kiểm tra tài khoản nhân viên
            if (user.UserName != model.UserName &&
                await
                    UnitOfWork.UserRepo.AnyAsync(
                        x => x.UserName.Equals(model.UserName) && !x.IsDelete && x.Id != model.Id))
            {
                ModelState.AddModelError("UserName", @"This account already exists in system");
                return View(model);
            }

            // Kiểm tra email nhân viên
            if (user.Email != model.Email &&
                await UnitOfWork.UserRepo.AnyAsync(x => x.Email.Equals(model.Email) && !x.IsDelete && x.Id != model.Id))
            {
                ModelState.AddModelError("Email", @"This email already exists in system");
                return View(model);
            }

            // Kiểm tra đủ độ tuổi lao động
            if (model.Birthday.HasValue && DateTime.Now.AddYears(-16).Subtract(model.Birthday.Value).TotalDays < 0)
            {
                ModelState.AddModelError("Birthday", @"Date of birth not enough working age");
                return View(model);
            }

            // check Date start work phải lớn hơn Birthday + 16
            if (model.Birthday.HasValue &&
                (model.StartDate.HasValue && model.StartDate.Value.Subtract(model.Birthday.Value).TotalDays < 16))
            {
                ModelState.AddModelError("StartDate",
                    @"The date of commencement of work must be greater than the date of birth and must be of working age >= 16 age");
                return View(model);
            }

            // Kiểm tra đơn vị tồn tại
            var office = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == model.OfficeId);
            if (office == null)
            {
                ModelState.AddModelError("OfficeId", @"Unit does not exist or has been deleted");
                return View(model);
            }

            // Kiểm tra Position tồn tại
            var title = await UnitOfWork.TitleRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == model.TitleId);
            if (title == null)
            {
                ModelState.AddModelError("TitleId", @"Position does not exist or has been deleted");
                return View(model);
            }

            // Kiểm tra đơn vị tồn tại trong Position
            if (!await UnitOfWork.PositionRepo.AnyAsync(x => x.OfficeId == office.Id && x.TitleId == title.Id))
            {
                ModelState.AddModelError("TitleId", @"Position is not declared in this unit");
                return View(model);
            }

            // Kiểm tra nhân viên đã có kiêm nhiệm này
            if (await UnitOfWork.UserPositionRepo.AnyAsync(x => x.UserId == model.Id && x.TitleId == title.Id && x.OfficeId == office.Id && x.IsDefault == false))
            {
                ModelState.AddModelError("TitleId", @"This position was held in the part-time position of the staff");
                return View(model);
            }

            // Kiểm tra groupPermission
            GroupPermision groupPermision = null;
            if (model.GroupPermisionId.HasValue)
            {
                groupPermision = await UnitOfWork.GroupPermissionRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == model.GroupPermisionId.Value);

                if (groupPermision == null)
                {
                    ModelState.AddModelError("GroupPermisionId", @"Right to access does not exist or has been deleted");
                    return View();
                }
            }


            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldPass = user.Password;
                    user = Mapper.Map(model, user);

                    user.LastUpdateUserId = UserState.UserId;
                    user.Updated = DateTime.Now;
                    user.FullName = string.IsNullOrWhiteSpace(user.MidleName)
                        ? $"{user.LastName} {user.FirstName}"
                        : $"{user.LastName} {user.MidleName} {user.FirstName}";

                    user.UnsignName = MyCommon.Ucs2Convert($"{user.FullName} {user.UserName} {user.Email}");
                    user.Password = oldPass;

                    var rs = await UnitOfWork.UserRepo.SaveAsync();

                    if (rs <= 0)
                        return View(model);

                    // Cập nhật lại Position kiêm nhiệm
                    var userPossition =
                        UnitOfWork.UserPositionRepo.SingleOrDefault(x => x.IsDefault && x.UserId == user.Id);

                    userPossition.OfficeId = office.Id;
                    userPossition.OfficeName = office.Name;
                    userPossition.OfficeIdPath = office.IdPath;
                    userPossition.OfficeNamePath = office.NamePath;
                    userPossition.TitleId = title.Id;
                    userPossition.TitleName = title.Name;
                    userPossition.Type = model.Type;

                    if (groupPermision != null)
                    {
                        userPossition.GroupPermisionId = groupPermision.Id;
                        userPossition.GroupPermissionName = groupPermision.Name;
                    }
                    else
                    {
                        userPossition.GroupPermisionId = null;
                        userPossition.GroupPermissionName = null;
                    }

                    var accSub = UnitOfWork.AccountantSubjectRepo.Find(m => m.Id == model.TypeId).FirstOrDefault();

                    if (accSub != null)
                    {
                        user.TypeId = accSub.Id;
                        user.TypeIdd = accSub.Idd;
                        user.TypeName = accSub.SubjectName;
                    }

                    await UnitOfWork.UserPositionRepo.SaveAsync();

                    TempData["Msg"] = $"Successful update staff \"<b>{user.FullName}</b>\"";

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            
            return RedirectToAction("Index");
        }

        // POST: User/Delete/5
        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.Staff)]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            // Staff does not exist or has been deleted
            if (user == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);

            // Không được xóa tài khoản đang đăng nhập
            if (user.Id == UserState.UserId)
                return JsonCamelCaseResult(-2, JsonRequestBehavior.AllowGet);

            user.IsDelete = true;

            var rs = await UnitOfWork.UserRepo.SaveAsync();

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserSearch(string keyword, int? page)
        {
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);

            var listUser = UnitOfWork.UserRepo.Find(
                   out totalRecord,
                   x => !x.IsDelete && (x.FullName.Contains(keyword) || x.Email.Contains(keyword) || x.Phone.Contains(keyword) || x.UnsignName.Contains(keyword)),
                   x => x.OrderByDescending(y => y.FullName),
                   page ?? 1,
                   10
              ).ToList();

            return Json(new { incomplete_results = true, total_count = totalRecord, items = listUser.Select(x => new { id = x.Id, text = x.FullName, email = x.Email, phone = x.Phone, avatar = x.Avatar }) }, JsonRequestBehavior.AllowGet);
        }

        //public async Task<JsonResult> GetUserSearch(string keyword, int? page)
        //{
        //    var listUser = await UnitOfWork.UserRepo.GetUserToOffice(UserState.UserId, UserState.Type.Value, UserState.OfficeIdPath, UserState.OfficeId.Value);
        //    return Json(new { incomplete_results = true, total_count = listUser.Count(), items = listUser.Where(x => x.FullName.Contains(keyword) || x.Email.Contains(keyword) || x.Phone.Contains(keyword)).Select(x => new { id = x.Id, text = x.FullName, email = x.Email, phone = x.Phone, avatar = x.Avatar }) }, JsonRequestBehavior.AllowGet);

        //}

        [CheckPermission(EnumAction.Add, EnumPage.Delivery)]
        public async Task<ActionResult> Suggetion(string term, int pageIndex = 1, int recordPerPage = 20)
        {
            term = MyCommon.Ucs2Convert(term);

            int totalRecord;
            var items = await UnitOfWork.UserRepo.Suggetion(term, pageIndex, recordPerPage, out totalRecord);

            return JsonCamelCaseResult(new { totalRecord, items }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchFullName(string term)
        {
            int totalRecord;

            var items = UnitOfWork.UserRepo.GetShortInfoByFullName(MyCommon.Ucs2Convert(term), 1, 20, out totalRecord);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SearchUserTag(string term, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrEmpty(term))
                return Json("", JsonRequestBehavior.AllowGet);

            int totalRecord;

            term = MyCommon.Ucs2Convert(term).ToLower();
            var items = await UnitOfWork.UserRepo.Suggetion(term.Replace("@", ""), page, pageSize, out totalRecord);

            return Json(items.Select(x => new GroupChatGetListTagUserResult
            {
                FullName = x.FullName,
                Id = x.Id,
                Image = x.Avatar,
                OfficeName = x.OfficeName,
                TitleName = x.TitleName,
                UserName = x.UserName
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetUserPosition(int userId)
        {
            return JsonCamelCaseResult(await UnitOfWork.UserPositionRepo.FindAsync(x => x.UserId == userId),
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUserPosition(int userId, int officeId, int titleId, byte type, int? groupPermissionId)
        {
            var user = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => x.Id == userId && x.IsDelete == false);

            if (user == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "Staff does not exist or has been deleted" },
                         JsonRequestBehavior.AllowGet);

            // Kiểm tra đơn vị tồn tại
            var office = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == officeId);
            if (office == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "Unit does not exist or has been deleted" },
                         JsonRequestBehavior.AllowGet);

            // Kiểm tra Position tồn tại
            var title = await UnitOfWork.TitleRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == titleId);
            if (title == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "Position does not exist or has been deleted" },
                         JsonRequestBehavior.AllowGet);

            // Kiểm tra đơn vị tồn tại trong Position
            if (!await UnitOfWork.PositionRepo.AnyAsync(x => x.OfficeId == office.Id && x.TitleId == title.Id))
                return JsonCamelCaseResult(new { Status = -1, Text = "Position is not declared in this unit" },
                         JsonRequestBehavior.AllowGet);

            // Kiểm tra groupPermission
            GroupPermision groupPermision = null;
            if (groupPermissionId.HasValue)
            {
                groupPermision = await UnitOfWork.GroupPermissionRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == groupPermissionId.Value);

                if (groupPermision == null)
                    return JsonCamelCaseResult(new { Status = -1, Text = "Right to access does not exist or has been deleted" },
                         JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra nhân viên đã có kiêm nhiệm này
            if(await UnitOfWork.UserPositionRepo.AnyAsync(x=> x.UserId == userId && x.TitleId == titleId && x.OfficeId == officeId))
                return JsonCamelCaseResult(new { Status = -1, Text = "Staff have this position" },
                         JsonRequestBehavior.AllowGet);

            var userPosition = new UserPosition()
            {
                UserId = user.Id,
                OfficeId = office.Id,
                OfficeName = office.Name,
                OfficeIdPath = office.IdPath,
                OfficeNamePath = office.NamePath,
                TitleId = title.Id,
                TitleName = title.Name,
                Type = type,
                IsDefault = false
            };

            if (groupPermision != null)
            {
                userPosition.GroupPermisionId = groupPermision.Id;
                userPosition.GroupPermissionName = groupPermision.Name;
            }

            UnitOfWork.UserPositionRepo.Add(userPosition);
            await UnitOfWork.UserPositionRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = 1, Text = "Add concurrently success" },
                         JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveUserPosition(int id)
        {
            var userPosition =
                await UnitOfWork.UserPositionRepo.SingleOrDefaultAsync(x => x.Id == id);
           
            if (userPosition == null)
                return JsonCamelCaseResult(new { Status = -1, Text = " This concurrently does not exist or has been deleted" },
                         JsonRequestBehavior.AllowGet);

            if (userPosition.IsDefault)
                return JsonCamelCaseResult(new { Status = -1, Text = "You can not delete the primary staff" },
                         JsonRequestBehavior.AllowGet);
            
            UnitOfWork.UserPositionRepo.Remove(userPosition);
            await UnitOfWork.UserPositionRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = 1, Text = "Successful deletion of concurrently" },
                         JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateUserPosition(int id, int? groupPermissionId)
        {
            var userPosition =
                await UnitOfWork.UserPositionRepo.SingleOrDefaultAsync(x => x.Id == id);

            if (userPosition == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "This concurrently does not exist or has been deleted" },
                         JsonRequestBehavior.AllowGet);

            // Kiểm tra groupPermission
            GroupPermision groupPermision = null;
            if (groupPermissionId.HasValue)
            {
                groupPermision = await UnitOfWork.GroupPermissionRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == groupPermissionId.Value);

                if (groupPermision == null)
                    return JsonCamelCaseResult(new { Status = -1, Text = "Right to access does not exist or has been deleted" },
                         JsonRequestBehavior.AllowGet);
            }

            if (groupPermision != null)
            {
                userPosition.GroupPermisionId = groupPermision.Id;
                userPosition.GroupPermissionName = groupPermision.Name;
            }
            else
            {
                userPosition.GroupPermisionId = null;
                userPosition.GroupPermissionName = null;
            }


            await UnitOfWork.UserPositionRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = 1, Text = "Update access successful" },
                         JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ChangeTitleConcurent(int id)
        {
            var userPosition =
                await UnitOfWork.UserPositionRepo.SingleOrDefaultAsync(x => x.Id == id);

            if (userPosition == null)
                return Redirect(Request.UrlReferrer == null ? "/" : Request.UrlReferrer.PathAndQuery);

            var office = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.Id == userPosition.OfficeId);

            var title = await UnitOfWork.TitleRepo.SingleOrDefaultAsync(x => x.Id == userPosition.TitleId);

            if (office == null || title == null)
                return Redirect(Request.UrlReferrer == null ? "/" : Request.UrlReferrer.PathAndQuery);

            UserState.OfficeName = office.Name;
            UserState.OfficeType = office.Type;
            UserState.OfficeAddress = office.Address;
            UserState.OfficeId = office.Id;
            UserState.OfficeIdPath = office.IdPath;
            UserState.TitleId = title.Id;
            UserState.TitleName = title.Name;

            AddUpdateClaim("userState", UserState.ToString());

            return Redirect(Request.UrlReferrer == null ? "/" : Request.UrlReferrer.PathAndQuery);
        }

        [CheckPermission(EnumAction.View, EnumPage.Setting)]
        public async Task<ActionResult> SuggettionRecentSearch()
        {
            var items = await UnitOfWork.UserRepo.RecentSuggetion(UserState.UserId, RecentMode.User);
            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Setting)]
        public async Task<ActionResult> SuggettionSearch(string keyword, int page = 1, int pageSize = 20)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            int totalRecord;

            var items = await UnitOfWork.UserRepo.SearchUser(keyword, page, pageSize, out totalRecord);

            return JsonCamelCaseResult(new { totalRecord, items }, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.Setting)]
        [HttpPost]
        public async Task<ActionResult> SuggettionRecentSave(int userId)
        {
            var recent = await
                    UnitOfWork.RecentRepo.SingleOrDefaultAsync(
                        x => x.RecordId == userId && x.Mode == (byte)RecentMode.User && x.UserId == UserState.UserId);

            if (recent != null)
            {
                recent.CountNo += 1;
                await UnitOfWork.RecentRepo.SaveAsync();

                return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
            }

            UnitOfWork.RecentRepo.Add(new Recent()
            {
                CountNo = 1,
                Mode = (byte)RecentMode.User,
                RecordId = userId,
                UserId = UserState.UserId
            });

            await UnitOfWork.RecentRepo.SaveAsync();

            return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
        }
    }
}
