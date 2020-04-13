using AutoMapper;
using Cms.Attributes;
using Common.Constant;
using Common.Emums;
using Common.Helper;
using Common.Items;
using Common.MailHelper;
using Common.PasswordEncrypt;
using Library.DbContext.Entities;
using Library.Models;
using Library.ViewModels;
using Library.ViewModels.Complains;
using Library.ViewModels.Customer;
using Library.ViewModels.Report;
using Library.ViewModels.Report.Revenue;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Globalization;
using System.Runtime.ExceptionServices;
using Cms.Jobs;
using Hangfire;
//using ResourcesCMS;
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    [Authorize]
    public class CustomerOfStaffController : BaseController
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// GET: CustomerOfStaff
        [LogTracker(EnumAction.View, EnumPage.PotentialCustomer, EnumPage.PotentialCustomerByStaff, EnumPage.Customer, EnumPage.CustomerbyStaff, EnumPage.BussinessSupport, EnumPage.BussinessReportCustomer, EnumPage.BussinessReportRevenue)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Lấy ra danh sách khách hàng của phòng ban đổ dữ liệu lên searchData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetRenderSystem()
        {
            var listStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listSystem = new List<dynamic>() { new { Text = Resource.All, Value = -1, Class = "active", ClassChild = "label-danger" } };
            var listSexCustomer = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listUser = new List<SearchMeta>();
            var listWarehouse = new List<SearchMeta>();
            var listProvince = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listWarehouseStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listVip = new List<dynamic>();
            var listOrderCustomer = new List<dynamic>() { new { Text = "- All -", Value = -1 } };

            //1. Lấy các trạng thái Status của customer
            foreach (CustomerOfStaffStatus customerOfStaffStatus in Enum.GetValues(typeof(CustomerOfStaffStatus)))
            {
                if (customerOfStaffStatus >= 0)
                {
                    listStatus.Add(
                        new
                        {
                            Text = customerOfStaffStatus.GetAttributeOfType<DescriptionAttribute>().Description,
                            Value = (int)customerOfStaffStatus
                        });
                }
            }

            //2. Lấy danh sách System trên hệ thống
            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);
            foreach (var item in listSystemDb)
            {
                listSystem.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                    Class = "",
                    ClassChild = "label-primary"
                });
            }

            //3. Lấy danh sách Warehouse trên hệ thống
            // TODO: Cần lấy danh sách từ bảng Warehouse
            var listWarehouseTemp = new List<SearchMeta>();
            var allWarehouse = await
               UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                   x => !x.IsDelete && (x.Type == (byte)OfficeType.Warehouse) && (x.Status == (byte)OfficeStatus.Use));

            var warehouseDb = allWarehouse.Select(x => new { x.Id, x.Name, x.IdPath, x.Address }).ToList();
            //var warehouseDb = allWarehouse.Select(x => new SearchMeta() { Text = x.Name, Value = x.Id }).ToList();
            foreach (var item in warehouseDb)
            {
                listWarehouseTemp.Add(new SearchMeta() { Text = item.Name, Value = item.Id });
            }

            listWarehouse.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listWarehouse.AddRange(listWarehouseTemp.ToList());

            //4. Lấy danh sách giới tính
            foreach (SexCustomer sexCustomer in Enum.GetValues(typeof(SexCustomer)))
            {
                if (sexCustomer >= 0)
                {
                    listSexCustomer.Add(
                        new
                        {
                            Text = sexCustomer.GetAttributeOfType<DescriptionAttribute>().Description,
                            Value = (int)sexCustomer
                        });
                }
            }

            //5. Lấy ra danh sách nhân viên
            if (UserState.Type != null && UserState.OfficeId != null)
            {
                var listUserTemp = await UnitOfWork.UserRepo.GetUserToOffice(UserState.UserId, (byte)UserState.Type, UserState.OfficeIdPath, (int)UserState.OfficeId);
                var tempUserList = from p in listUserTemp
                                   select new SearchMeta() { Text = p.UserName + " - " + p.FullName, Value = p.Id };

                listUser.Add(new SearchMeta() { Text = "- All -", Value = -1 });
                listUser.AddRange(tempUserList.ToList());
            }
            //6. Laays ra danh sach thanh pho
            foreach (var item in UnitOfWork.DbContext.Provinces)
            {
                listProvince.Add(new
                {
                    Text = item.Name,
                    Value = item.Id
                });
            }

            //7. Lấy ra các trạng thái khách hàng liên quan tới  kho
            foreach (WarehouseCustomerStatus warehouseCustomerStatus in Enum.GetValues(typeof(WarehouseCustomerStatus)))
            {
                if (warehouseCustomerStatus >= 0)
                {
                    listWarehouseStatus.Add(
                        new
                        {
                            Text = warehouseCustomerStatus.GetAttributeOfType<DescriptionAttribute>().Description,
                            Value = (int)warehouseCustomerStatus
                        });
                }
            }
            //1. Lấy các cấp độ VIP của khách hàng
            foreach (CustomerLevel level in UnitOfWork.CustomerLevelRepo.FindAsNoTracking(s => !s.IsDelete))
            {
                listVip.Add(
                    new
                    {
                        Text = level.Name,
                        Value = level.Id
                    });
            }


            return Json(new { listStatus, listSystem, listWarehouse, listUser, listSexCustomer, listProvince, listWarehouseStatus, listVip }, JsonRequestBehavior.AllowGet);
        }

        // Khởi tạo form
        [HttpPost]
        public async Task<JsonResult> GetInit()
        {
            var totalPotentialCustomer = 0;
            var totalPotentialCustomerByUser = 0;
            var totalCustomer = 0;
            var totalCustomerByUser = 0;
            var totalTicketSupport = 0;

            //Check user có phải là nhân viên kinh doanh công ty ko?
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeTypeCompany(OfficeType.Business);
            var checkUser = listUser.FirstOrDefault(x => x.Id == UserState.UserId);
            var check = checkUser != null;

            // Tính toán tổng số khách hàng chính thức của phòng
            var listCustomer = await UnitOfWork.CustomerRepo.FindAsNoTrackingAsync(
                    x => !x.IsDelete
                         //&& (UserState.Type == 0 || x.OfficeId == UserState.OfficeId)
                         && (UserState.Type != 0 || x.UserId == UserState.UserId)
                         && (check == false || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".") || x.UserId == null)))
                         && (check == true || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + "."))))
                );

            totalCustomer = listCustomer.Count();

            // Tính toán tổng số khách hàng chính thức đang phụ trách
            foreach (var item in listCustomer)
            {
                if (item.UserId == UserState.UserId)
                {
                    totalCustomerByUser++;
                }
            }

            // Tính toán tổng số khách hàng tiềm năng của phòng
            var listPotentialCustomer = await UnitOfWork.PotentialCustomerRepo.FindAsNoTrackingAsync(
                x => !x.IsDelete
                         //&& (UserState.Type == 0 || x.OfficeId == UserState.OfficeId)
                         && (UserState.Type != 0 || x.UserId == UserState.UserId)
                         && (check == false || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".") || x.UserId == null)))
                         && (check == true || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + "."))))
                );

            totalPotentialCustomer = listPotentialCustomer.Count();

            // Tính toán tổng số khách hàng tiềm năng đang phụ trách
            foreach (var item in listPotentialCustomer)
            {
                if (item.UserId == UserState.UserId)
                {
                    totalPotentialCustomerByUser++;
                }
            }

            //Tính toán tổng số ticket nhân viên cần hỗ trợ
            totalTicketSupport = await UnitOfWork.ComplainRepo.TicketSupportCountAsync(UserState);

            return Json(new { totalPotentialCustomer, totalPotentialCustomerByUser, totalCustomer, totalCustomerByUser, totalTicketSupport }, JsonRequestBehavior.AllowGet);
        }

        #region Department's official customer - Customer

        /// <summary>
        /// Lấy danh sách khách hàng chính thức
        /// POST: /CustomerOfStaff/GetAllCustomerList
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[CheckPermission(EnumAction.View, EnumPage.Customer)]
        public async Task<JsonResult> GetAllCustomerList(int page, int pageSize, CustomerSearchModal searchModal)
        {
            List<Customer> customerModal;
            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new CustomerSearchModal();
            }
            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();

            //Check user có phải là nhân viên kinh doanh công ty ko?
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeTypeCompany(OfficeType.Business);
            var checkUser = listUser.FirstOrDefault(x => x.Id == UserState.UserId);
            var check = checkUser != null;

            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);

                customerModal = await UnitOfWork.CustomerRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.Email.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && !x.IsDelete
                         && (searchModal.GenderId == -1 || x.GenderId == searchModal.GenderId)
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                         && (UserState.Type != 0 || x.UserId == UserState.UserId)
                         && (searchModal.WarehouseId == -1 || x.WarehouseId == searchModal.WarehouseId)
                         && (searchModal.WarehouseCustomer == -1 || (searchModal.WarehouseCustomer == 0 && x.WarehouseId == null) || (searchModal.WarehouseCustomer == 1 && x.WarehouseId != null))
                         && (check == false || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".") || x.UserId == null)))
                         && (check == true || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + "."))))
                         && x.Created >= dateStart && x.Created <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            else
            {
                customerModal = await UnitOfWork.CustomerRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.Email.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && !x.IsDelete
                         && (searchModal.GenderId == -1 || x.GenderId == searchModal.GenderId)
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                         && (searchModal.WarehouseId == -1 || x.WarehouseId == searchModal.WarehouseId)
                         && (searchModal.WarehouseCustomer == -1 || (searchModal.WarehouseCustomer == 0 && x.WarehouseId == null) || (searchModal.WarehouseCustomer == 1 && x.WarehouseId != null))
                         && (UserState.Type != 0 || x.UserId == UserState.UserId)
                         && (check == false || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".") || x.UserId == null)))
                         && (check == true || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }

            return Json(new { totalRecord, customerModal }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Chi tiết khách hàng
        /// POST: /CustomerOfStaff/GetAllCustomerList
        /// </summary>
        /// <param-name>customerId</param-name>
        /// lay ra danh sach order
        /// <returns></returns>
        [HttpPost]
        //[CheckPermission(EnumAction.View, EnumPage.Customer)]
        public async Task<JsonResult> GetCustomerOfStaffDetail(int customerId)
        {
            var result = true;
            var customerModal = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(p => p.Id == customerId);
            if (customerModal == null)
            {
                result = false;
            }
            else
            {
                customerModal.Password = string.Empty;
            }

            return Json(new { result, customerModal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetListOrderByCustomer(int customerId, int page, int pageSize)
        {
            long totalRecord;
            int userId = UserState.UserId;
            var orderByCustomer = new List<Order>();
            orderByCustomer = await UnitOfWork.OrderRepo.FindAsync(
                  out totalRecord,
                  s => s.CustomerId == customerId && !s.IsDelete,
                    x => x.OrderByDescending(y => y.Id),
                    page,
                    pageSize);
            return Json(new { totalRecord, orderByCustomer }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// danh sach khieu nai theo khach hang
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.BussinessSupport)]
        public async Task<JsonResult> GetListComplainByCustomer(int customerId)
        {
            var result = true;

            var hascomplainByCustomer =
                await UnitOfWork.ComplainRepo.FindAsync(p => p.CustomerId == customerId, null, 0, 4);
            var complainByCustomer = new List<Complain>();
            if (hascomplainByCustomer == null)
            {
                result = false;
            }
            else
            {
                complainByCustomer = hascomplainByCustomer;
            }
            return Json(new { result, complainByCustomer }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.BussinessSupport)]
        public async Task<JsonResult> GetListOrderMoneyByCustomer(int customerId, int page, int pageSize)
        {
            int userId = UserState.UserId;
            long totalRecord;
            var orderMoneyByCustomer = new List<RechargeBill>();
            orderMoneyByCustomer = await UnitOfWork.RechargeBillRepo.FindAsync(
                 out totalRecord,
                 s => s.CustomerId == customerId && !s.IsDelete,
                   x => x.OrderByDescending(y => y.Id),
                   page,
                   pageSize);
            return Json(new { totalRecord, orderMoneyByCustomer }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thêm mới khách hàng
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.Customer)]
        public async Task<JsonResult> CreateNewCustomer(CustomerMeta model)
        {
            ModelState.Remove("Id");
            ModelState.Remove("LevelId");
            ModelState.Remove("LevelName");
            ModelState.Remove("ProvinceId");
            ModelState.Remove("DistrictId");
            ModelState.Remove("WardId");
            ModelState.Remove("DepositPrice");
            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = "Incomplete, inaccurate date or misformatted date of birth!" },
                    JsonRequestBehavior.AllowGet);
            }

            //1. Kiểm tra Email khach hang chinh thuc đã tồn tại hay chưa
            var customer = UnitOfWork.CustomerRepo.FirstOrDefaultAsNoTracking(x => x.Email.Equals(model.Email));

            // Tên đăng nhập hoặc mật khẩu không đúng
            if (customer != null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.EmailCustomerIsValid },
                    JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra thông tin System
            var systemDetail = UnitOfWork.SystemRepo.FirstOrDefault(x => x.Id == model.SystemId);
            if (systemDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.SystemIsNotValid },
                   JsonRequestBehavior.AllowGet);
            }

            //3. Kiểm tra lại thông tin nhân viên kinh doanh quản lý khách hàng.
            if (model.UserId == null || model.UserId < 0)
            {
                return Json(new { status = Result.Failed, msg = "Please select this customer service officer !" },
                   JsonRequestBehavior.AllowGet);
            }
            else
            {
                var userDetail = UnitOfWork.UserRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && (x.Id == model.UserId));
                if (userDetail != null)
                {
                    model.UserId = userDetail.Id;
                    model.UserFullName = userDetail.FullName;
                    // model.OfficeId = userDetail
                    // Lấy thông tin phòng ban
                    var userPostion =
                        await
                            UnitOfWork.UserPositionRepo.FirstOrDefaultAsNoTrackingAsync(x => x.UserId == userDetail.Id && x.IsDefault);
                    if (userPostion != null)
                    {
                        model.OfficeId = userPostion.OfficeId;
                        model.OfficeName = userPostion.OfficeName;
                        model.OfficeIdPath = userPostion.OfficeIdPath;
                    }
                }
            }

            //4. Kiểm tra lại định khoản tự động trên hệ thống
            var subjectType = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => x.Idd == (int)EnumAccountantSubject.Customer);
            if (subjectType != null)
            {
                model.TypeId = subjectType.Id;
                model.TypeIdd = subjectType.Idd;
                model.TypeName = subjectType.SubjectName;
            }

            // Lấy lại List of warehouses
            var warehouse = UnitOfWork.OfficeRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && (x.Id == model.WarehouseId) && (x.Type == (byte)OfficeType.Warehouse) && (x.Status == (byte)OfficeStatus.Use));
            if (warehouse == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.WarehouseIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }

            //7. Lay ve thanh pho
            var province = UnitOfWork.ProvinceRepo.FirstOrDefault(s => s.Id == model.ProvinceId);
            //if (province == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "City does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //8. Lay ve huyen
            var district = UnitOfWork.DistrictRepo.FirstOrDefault(s => s.Id == model.DistrictId);
            //if (district == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "District does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //9. Lay ve xa
            var ward = UnitOfWork.WardRepo.FirstOrDefault(s => s.Id == model.WardId);
            //if (ward == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "Commune does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}

            // Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var customerDetail = new Customer();
                    Mapper.Map(model, customerDetail);

                    if (model.DepositPrice == null)
                    {
                        customerDetail.DepositPrice = 0;
                    }
                    customerDetail.Password = PasswordEncrypt.EncodePassword(model.Password.Trim(),
                        PasswordSalt.FinGroupApiCustomer);

                    customerDetail.Code = string.Empty;
                    customerDetail.Avatar = "/Content/img/no-avatar.png";
                    //if((int)model.LevelId == -1)
                    //{
                    //    customerDetail.LevelId = 1;
                    //    var level = UnitOfWork.CustomerLevelRepo.FirstOrDefault(s => s.Id == model.LevelId && !s.IsDelete);
                    //    customerDetail.LevelName = level.Name;
                    //}
                    //else
                    //{
                    //    var level = UnitOfWork.CustomerLevelRepo.FirstOrDefault(s=>s.Id == model.LevelId && !s.IsDelete);
                    //    customerDetail.LevelName = level.Name;
                    //}

                    var level = UnitOfWork.CustomerLevelRepo.FirstOrDefault(s => s.Id == model.LevelId && !s.IsDelete);
                    if (level == null)
                    {
                        return Json(new { status = Result.Failed, msg = "Selected level does not exist!" },
                        JsonRequestBehavior.AllowGet);
                    }
                    customerDetail.LevelName = level.Name;
                    customerDetail.Point = 0;
                    customerDetail.DistrictId = -1;
                    customerDetail.DistrictName = "Thailand";
                    customerDetail.ProvinceId = -1;
                    customerDetail.ProvinceName = "Thailand";
                    customerDetail.WardId = -1;
                    customerDetail.WardsName = "Thailand";

                    customerDetail.SystemId = systemDetail.Id;
                    customerDetail.SystemName = systemDetail.Domain;

                    customerDetail.HashTag = model.FullName + "," + model.Nickname + "," + model.Email;
                    //customerDetail.Balance = 0;
                    //customerDetail.BalanceAvalible = 0;

                    //customerDetail.UserId = 4;
                    customerDetail.IsDelete = false;
                    customerDetail.CountryId = "VN";

                    customerDetail.IsActive = true;
                    customerDetail.IsLockout = false;

                    customerDetail.WarehouseId = warehouse.Id;
                    customerDetail.WarehouseName = warehouse.Name;

                    customerDetail.Created = DateTime.Now;
                    customerDetail.Updated = DateTime.Now;
                    customerDetail.LoginFailureCount = 0;

                    customerDetail.GenderName = EnumHelper.GetEnumDescription<SexCustomer>((int)customerDetail.GenderId);

                    UnitOfWork.CustomerRepo.Add(customerDetail);
                    UnitOfWork.CustomerRepo.Save();

                    var customerNo = UnitOfWork.CustomerRepo.Count(x => x.Id <= customerDetail.Id);
                    customerDetail.Code = MyCommon.GenCode(customerNo);
                    UnitOfWork.CustomerRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError },
                //        JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.CreateNewCustomerSuccess },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sửa thông tin khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.Update, EnumPage.Customer)]
        public async Task<JsonResult> EditCustomer(CustomerMeta model)
        {
            //check form
            ModelState.Remove("Id");
            ModelState.Remove("LevelId");
            ModelState.Remove("LevelName");
            ModelState.Remove("ProvinceId");
            ModelState.Remove("DistrictId");
            ModelState.Remove("WardId");
            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = "Incomplete, inaccurate date or misformatted date of birth!" },
                    JsonRequestBehavior.AllowGet);
            }
            //1. Kiểm tra su ton tai cua customer
            var customer = UnitOfWork.CustomerRepo.SingleOrDefault(x => !x.IsDelete && x.Id == model.Id);
            if (customer == null)
            {
                return Json(new { status = Result.Failed, msg = "Customer's account does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra lại đối tượng người dùng
            var subjectType = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => x.Idd == (int)EnumAccountantSubject.Customer);
            if (subjectType != null)
            {
                model.TypeId = subjectType.Id;
                model.TypeIdd = subjectType.Idd;
                model.TypeName = subjectType.SubjectName;
            }

            //2. Lấy về thông tin System
            var system = UnitOfWork.SystemRepo.FirstOrDefault(s => s.Id == model.SystemId);
            if (system == null)
            {
                return Json(new { status = Result.Failed, msg = "Customer's system does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            model.SystemName = system.Domain;

            //7. Lay ve thanh pho
            var province = UnitOfWork.ProvinceRepo.FirstOrDefault(s => s.Id == model.ProvinceId);
            //if (province == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "City does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //8. Lay ve huyen
            var district = UnitOfWork.DistrictRepo.FirstOrDefault(s => s.Id == model.DistrictId);
            //if (district == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "District does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //9. Lay ve xa
            var ward = UnitOfWork.WardRepo.FirstOrDefault(s => s.Id == model.WardId);
            //if (ward == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "Commune does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}

            //3. Kiểm tra lại thông tin nhân viên kinh doanh quản lý khách hàng.
            if (model.UserId != null)
            {
                var userDetail = UnitOfWork.UserRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && (x.Id == model.UserId));
                if (userDetail != null)
                {
                    model.UserId = userDetail.Id;
                    model.UserFullName = userDetail.FullName;
                    // model.OfficeId = userDetail
                    // Lấy thông tin phòng ban
                    var userPostion =
                        await
                            UnitOfWork.UserPositionRepo.FirstOrDefaultAsNoTrackingAsync(x => x.UserId == userDetail.Id && x.IsDefault);
                    if (userPostion != null)
                    {
                        model.OfficeId = userPostion.OfficeId;
                        model.OfficeName = userPostion.OfficeName;
                        model.OfficeIdPath = userPostion.OfficeIdPath;
                    }
                }
            }

            //4. Lấy lại List of warehouses
            var warehouse = UnitOfWork.OfficeRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && (x.Id == model.WarehouseId) && (x.Type == (byte)OfficeType.Warehouse) && (x.Status == (byte)OfficeStatus.Use));
            if (warehouse == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.WarehouseIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                model.WarehouseId = warehouse.Id;
                model.WarehouseName = warehouse.Name;
            }

            // Kiểm tra lại pasword người dùng
            var customerPassword = customer.Password;

            if (!string.IsNullOrEmpty(model.Password))
            {
                var newPassword = PasswordEncrypt.EncodePassword(model.Password.Trim(),
                    PasswordSalt.FinGroupApiCustomer);
                if (customerPassword != newPassword)
                {
                    model.Password = newPassword;
                }
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {

                    Mapper.Map(model, customer);
                    //Mapper.Map<CustomerMeta, Customer>(model);
                    var level = UnitOfWork.CustomerLevelRepo.FirstOrDefault(s => s.Id == model.LevelId && !s.IsDelete);
                    if (level == null)
                    {
                        return Json(new { status = Result.Failed, msg = "Selected level does not exist!" },
                        JsonRequestBehavior.AllowGet);
                    }
                    customer.LevelName = level.Name;

                    if (string.IsNullOrEmpty(model.Password))
                    {
                        customer.Password = customerPassword;
                    }
                    customer.GenderName = EnumHelper.GetEnumDescription<SexCustomer>((int)customer.GenderId);
                    customer.DistrictId = -1;
                    customer.DistrictName = "Thailand";
                    customer.ProvinceId = -1;
                    customer.ProvinceName = "Thailand";
                    customer.WardId = -1;
                    customer.WardsName = "Thailand";

                    customer.Updated = DateTime.Now;

                    UnitOfWork.CustomerRepo.Save();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError },
                //        JsonRequestBehavior.AllowGet);
                //}
            }
            return Json(new { status = Result.Succeed, msg = ConstantMessage.EditCustomerSuccess },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xóa khách hàng
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.Customer)]
        public async Task<ActionResult> DeleteCustomer(int customerId)
        {
            var customer =
                await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(x => x.Id == customerId && x.IsDelete == false);
            if (customer == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);
            customer.IsDelete = true;
            UnitOfWork.CustomerRepo.Update(customer);
            var rs = await UnitOfWork.CustomerRepo.SaveAsync();
            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        #endregion Department's official customer - Customer

        #region Official customer being in charge - CustomerByStaff

        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.CustomerbyStaff)]
        public async Task<JsonResult> GetAllCustomerByStaffList(int page, int pageSize, CustomerSearchModal searchModal)
        {
            List<Customer> customerByStaffModal;
            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new CustomerSearchModal();
            }

            searchModal.Keyword = MyCommon.Ucs2Convert(string.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim());

            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);

                customerByStaffModal = await UnitOfWork.CustomerRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.Email.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && !x.IsDelete
                         && (x.UserId == UserState.UserId)
                         && (searchModal.GenderId == -1 || x.GenderId == searchModal.GenderId)
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         //&& (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                         && (searchModal.WarehouseId == -1 || x.WarehouseId == searchModal.WarehouseId)
                        && (searchModal.WarehouseCustomer == -1 || (searchModal.WarehouseCustomer == 0 && x.WarehouseId == null) || (searchModal.WarehouseCustomer == 1 && x.WarehouseId != null))
                         && x.Created >= dateStart && x.Created <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            else
            {
                customerByStaffModal = await UnitOfWork.CustomerRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.Email.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && !x.IsDelete
                         && (x.UserId == UserState.UserId)
                         && (searchModal.GenderId == -1 || x.GenderId == searchModal.GenderId)
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.WarehouseId == -1 || x.WarehouseId == searchModal.WarehouseId)
                         && (searchModal.WarehouseCustomer == -1 || (searchModal.WarehouseCustomer == 0 && x.WarehouseId == null) || (searchModal.WarehouseCustomer == 1 && x.WarehouseId != null)),
                    //&& (searchModal.UserId == -1 || x.UserId == searchModal.UserId),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            return Json(new { totalRecord, customerByStaffModal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.CustomerbyStaff)]
        public async Task<JsonResult> CreateNewCustomerByStaff(CustomerMeta model)
        {
            ModelState.Remove("Id");
            ModelState.Remove("LevelId");
            ModelState.Remove("LevelName");
            ModelState.Remove("Balance");
            ModelState.Remove("BalanceAvalible");
            ModelState.Remove("DepositPrice");
            ModelState.Remove("DistrictId");
            ModelState.Remove("ProvinceId");
            ModelState.Remove("WardId");
            ModelState.Remove("UserId");


            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = "Incomplete, inaccurate date or misformatted date of birth!" },
                    JsonRequestBehavior.AllowGet);
            }

            //1. Kiểm tra Email khach hang chinh thuc đã tồn tại hay chưa
            var customer = UnitOfWork.CustomerRepo.FirstOrDefaultAsNoTracking(x => x.Email.Equals(model.Email));

            // Tên đăng nhập hoặc mật khẩu không đúng
            if (customer != null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.EmailCustomerIsValid },
                    JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra thông tin System
            var systemDetail = UnitOfWork.SystemRepo.FirstOrDefault(x => x.Id == model.SystemId);
            if (systemDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.SystemIsNotValid },
                   JsonRequestBehavior.AllowGet);
            }

            //3. Kiểm tra lại thông tin nhân viên kinh doanh quản lý khách hàng.
            var userDetail = UnitOfWork.UserRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && (x.Id == UserState.UserId));
            if (userDetail != null)
            {
                model.UserId = userDetail.Id;
                model.UserFullName = userDetail.FullName;
                // model.OfficeId = userDetail
                // Lấy thông tin phòng ban
                var userPostion =
                    await
                        UnitOfWork.UserPositionRepo.FirstOrDefaultAsNoTrackingAsync(x => x.UserId == userDetail.Id && x.IsDefault);
                if (userPostion != null)
                {
                    model.OfficeId = userPostion.OfficeId;
                    model.OfficeName = userPostion.OfficeName;
                    model.OfficeIdPath = userPostion.OfficeIdPath;
                }
            }

            //4. Kiểm tra lại định khoản tự động trên hệ thống
            var subjectType = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => x.Idd == (int)EnumAccountantSubject.Customer);
            if (subjectType != null)
            {
                model.TypeId = subjectType.Id;
                model.TypeIdd = subjectType.Idd;
                model.TypeName = subjectType.SubjectName;
            }

            // Lấy lại List of warehouses
            var warehouse = UnitOfWork.OfficeRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && (x.Id == model.WarehouseId) && (x.Type == (byte)OfficeType.Warehouse) && (x.Status == (byte)OfficeStatus.Use));
            if (warehouse == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.WarehouseIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }

            //7. Lay ve thanh pho
            var province = UnitOfWork.ProvinceRepo.FirstOrDefault(s => s.Id == model.ProvinceId);
            //if (province == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "City does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //8. Lay ve huyen
            var district = UnitOfWork.DistrictRepo.FirstOrDefault(s => s.Id == model.DistrictId);
            //if (district == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "District does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //9. Lay ve xa
            var ward = UnitOfWork.WardRepo.FirstOrDefault(s => s.Id == model.WardId);
            //if (ward == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "Commune does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}

            // Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var customerDetail = new Customer();
                    Mapper.Map(model, customerDetail);

                    if (model.DepositPrice == null)
                    {
                        customerDetail.DepositPrice = 0;
                    }
                    customerDetail.Password = PasswordEncrypt.EncodePassword(model.Password.Trim(),
                        PasswordSalt.FinGroupApiCustomer);

                    customerDetail.Code = string.Empty;

                    customerDetail.Avatar = "/Content/img/no-avatar.png";
                    //if((int)model.LevelId == -1)
                    //{
                    //    customerDetail.LevelId = 1;
                    //    var level = UnitOfWork.CustomerLevelRepo.FirstOrDefault(s => s.Id == model.LevelId && !s.IsDelete);
                    //    customerDetail.LevelName = level.Name;
                    //}
                    //else
                    //{
                    //    var level = UnitOfWork.CustomerLevelRepo.FirstOrDefault(s=>s.Id == model.LevelId && !s.IsDelete);
                    //    customerDetail.LevelName = level.Name;
                    //}

                    var level = UnitOfWork.CustomerLevelRepo.FirstOrDefault(s => s.Id == model.LevelId && !s.IsDelete);
                    if (level == null)
                    {
                        return Json(new { status = Result.Failed, msg = "Selected level does not exist!" },
                        JsonRequestBehavior.AllowGet);
                    }
                    customerDetail.LevelName = level.Name;
                    customerDetail.Point = 0;

                    customerDetail.DistrictId = -1;
                    customerDetail.DistrictName = "Thailand";
                    customerDetail.ProvinceId = -1;
                    customerDetail.ProvinceName = "Thailand";
                    customerDetail.WardId = -1;
                    customerDetail.WardsName = "Thailand";

                    customerDetail.SystemId = systemDetail.Id;
                    customerDetail.SystemName = systemDetail.Domain;

                    customerDetail.HashTag = model.FullName + "," + model.Nickname + "," + model.Email;
                    //customerDetail.Balance = 0;
                    //customerDetail.BalanceAvalible = 0;

                    //customerDetail.UserId = 4;
                    customerDetail.IsDelete = false;
                    customerDetail.CountryId = "VN";

                    customerDetail.IsActive = true;
                    customerDetail.IsLockout = false;

                    customerDetail.WarehouseId = warehouse.Id;
                    customerDetail.WarehouseName = warehouse.Name;

                    customerDetail.Created = DateTime.Now;
                    customerDetail.Updated = DateTime.Now;
                    customerDetail.LoginFailureCount = 0;

                    customerDetail.GenderName = EnumHelper.GetEnumDescription<SexCustomer>((int)customerDetail.GenderId);

                    UnitOfWork.CustomerRepo.Add(customerDetail);
                    UnitOfWork.CustomerRepo.Save();

                    var customerNo = UnitOfWork.CustomerRepo.Count(x => x.Id <= customerDetail.Id);
                    customerDetail.Code = MyCommon.GenCode(customerNo);
                    UnitOfWork.CustomerRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError },
                //        JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.CreateNewCustomerSuccess },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Update, EnumPage.CustomerbyStaff)]
        public async Task<JsonResult> EditCustomerByStaff(CustomerMeta model)
        {
            //check form
            ModelState.Remove("Id");
            ModelState.Remove("LevelId");
            ModelState.Remove("LevelName");
            ModelState.Remove("Balance");
            ModelState.Remove("BalanceAvalible");
            ModelState.Remove("UserId");
            ModelState.Remove("ProvinceId");
            ModelState.Remove("DistrictId");
            ModelState.Remove("WardId");
            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = "Incomplete, inaccurate date or misformatted date of birth!" },
                    JsonRequestBehavior.AllowGet);
            }
            //1. Kiểm tra su ton tai cua customer
            var customer = UnitOfWork.CustomerRepo.SingleOrDefault(x => !x.IsDelete && x.Id == model.Id);
            if (customer == null)
            {
                return Json(new { status = Result.Failed, msg = "Customer's account does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra lại đối tượng người dùng
            var subjectType = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => x.Idd == (int)EnumAccountantSubject.Customer);
            if (subjectType != null)
            {
                model.TypeId = subjectType.Id;
                model.TypeIdd = subjectType.Idd;
                model.TypeName = subjectType.SubjectName;
            }

            //2. Lấy về thông tin System
            var system = UnitOfWork.SystemRepo.FirstOrDefault(s => s.Id == model.SystemId);
            if (system == null)
            {
                return Json(new { status = Result.Failed, msg = "Customer's system does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            model.SystemName = system.Domain;
            //foreach (SystemConfig systemConfig in Enum.GetValues(typeof(SystemConfig)))
            //{
            //    if ((int)systemConfig == model.SystemId)
            //    {
            //        model.SystemName = systemConfig.GetAttributeOfType<DescriptionAttribute>().Description;
            //    }
            //}
            //7. Lay ve thanh pho
            var province = UnitOfWork.ProvinceRepo.FirstOrDefault(s => s.Id == model.ProvinceId);
            //if (province == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "City does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //8. Lay ve huyen
            var district = UnitOfWork.DistrictRepo.FirstOrDefault(s => s.Id == model.DistrictId);
            //if (district == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "District does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //9. Lay ve xa
            var ward = UnitOfWork.WardRepo.FirstOrDefault(s => s.Id == model.WardId);
            //if (ward == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "Commune does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}

            //3. Kiểm tra lại thông tin nhân viên kinh doanh quản lý khách hàng.
            if (model.UserId != null)
            {
                var userDetail = UnitOfWork.UserRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && (x.Id == model.UserId));
                if (userDetail != null)
                {
                    model.UserId = userDetail.Id;
                    model.UserFullName = userDetail.FullName;
                    // model.OfficeId = userDetail
                    // Lấy thông tin phòng ban
                    var userPostion =
                        await
                            UnitOfWork.UserPositionRepo.FirstOrDefaultAsNoTrackingAsync(x => x.UserId == userDetail.Id && x.IsDefault);
                    if (userPostion != null)
                    {
                        model.OfficeId = userPostion.OfficeId;
                        model.OfficeName = userPostion.OfficeName;
                        model.OfficeIdPath = userPostion.OfficeIdPath;
                    }
                }
            }

            //4. Lấy lại List of warehouses
            var warehouse = UnitOfWork.OfficeRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && (x.Id == model.WarehouseId) && (x.Type == (byte)OfficeType.Warehouse) && (x.Status == (byte)OfficeStatus.Use));
            if (warehouse == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.WarehouseIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                model.WarehouseId = warehouse.Id;
                model.WarehouseName = warehouse.Name;
            }

            // Kiểm tra lại pasword người dùng
            var customerPassword = customer.Password;

            if (!string.IsNullOrEmpty(model.Password))
            {
                var newPassword = PasswordEncrypt.EncodePassword(model.Password.Trim(),
                    PasswordSalt.FinGroupApiCustomer);
                if (customerPassword != newPassword)
                {
                    model.Password = newPassword;
                }
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {

                    Mapper.Map(model, customer);
                    //Mapper.Map<CustomerMeta, Customer>(model);
                    var level = UnitOfWork.CustomerLevelRepo.FirstOrDefault(s => s.Id == model.LevelId && !s.IsDelete);
                    if (level == null)
                    {
                        return Json(new { status = Result.Failed, msg = "Selected level does not exist!" },
                        JsonRequestBehavior.AllowGet);
                    }
                    customer.LevelName = level.Name;

                    if (string.IsNullOrEmpty(model.Password))
                    {
                        customer.Password = customerPassword;
                    }
                    customer.GenderName = EnumHelper.GetEnumDescription<SexCustomer>((int)customer.GenderId);
                    customer.DistrictId = -1;
                    customer.DistrictName = "Thailand";
                    customer.ProvinceId = -1;
                    customer.ProvinceName = "Thailand";
                    customer.WardId = -1;
                    customer.WardsName = "Thailand";

                    customer.Updated = DateTime.Now;

                    UnitOfWork.CustomerRepo.Save();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError },
                //        JsonRequestBehavior.AllowGet);
                //}
            }
            return Json(new { status = Result.Succeed, msg = ConstantMessage.EditCustomerSuccess },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.CustomerbyStaff)]
        public JsonResult GetAllCustomerByStaffSearchData()
        {
            var listStatusByStaff = new List<dynamic>() { new { Text = Resource.All, Value = -1 } };
            var listSystemByStaff = new List<dynamic>() { new { Text = Resource.All, Value = -1 } };
            var listSexCustomerByStaff = new List<dynamic>() { new { Text = Resource.All, Value = -1 } };
            var listUserByStaff = new List<SearchMeta>();

            // Lấy các trạng thái Status của customer
            foreach (CustomerOfStaffStatus customerOfStaffStatus in Enum.GetValues(typeof(CustomerOfStaffStatus)))
            {
                if (customerOfStaffStatus >= 0)
                {
                    listStatusByStaff.Add(
                        new
                        {
                            Text = customerOfStaffStatus.GetAttributeOfType<DescriptionAttribute>().Description,
                            Value = (int)customerOfStaffStatus
                        });
                }
            }

            // Lấy danh sách System
            var listSystem = UnitOfWork.SystemRepo.Find(s => s.Id > 0);
            foreach (var item in listSystem)
            {
                listSystemByStaff.Add(
                    new
                    {
                        Text = item.Domain,
                        Value = item.Id
                    });
            }

            //lấy ra danh sách nhân viên
            var user = UnitOfWork.UserRepo.FindAsNoTracking(x => x.Id > 0).ToList();
            var tempUserList = from p in user
                               select new SearchMeta() { Text = p.FullName, Value = p.Id };

            listUserByStaff.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listUserByStaff.AddRange(tempUserList.ToList());

            //lấy danh sách giới tính
            foreach (SexCustomer sexCustomer in Enum.GetValues(typeof(SexCustomer)))
            {
                if (sexCustomer >= 0)
                {
                    listSexCustomerByStaff.Add(
                        new
                        {
                            Text = sexCustomer.GetAttributeOfType<DescriptionAttribute>().Description,
                            Value = (int)sexCustomer
                        });
                }
            }

            return Json(new { listStatusByStaff, listSystemByStaff, listUserByStaff, listSexCustomerByStaff }, JsonRequestBehavior.AllowGet);
        }

        #endregion Official customer being in charge - CustomerByStaff

        #region Khách hàng tiềm năng - PotentialCustomer

        /// <summary>
        /// <para>Id==3 giá trị thay đổi khi giá trị id của phòng chăm sóc khách hàng thay đổi </para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetAllPotentialCustomerSearchData()
        {
            var listSystemPotentialCustomer = new List<SearchMeta>();
            var listUserPotentialCustomer = new List<SearchMeta>();
            var listPotentialCustomerSex = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listUserOfPosition = new List<SearchMeta>();
            var listCustomerType = new List<SearchMeta>();

            //1. Lấy danh sách giới tính
            foreach (SexCustomer sexCustomer in Enum.GetValues(typeof(SexCustomer)))
            {
                if (sexCustomer >= 0)
                {
                    listPotentialCustomerSex.Add(
                        new
                        {
                            Text = sexCustomer.GetAttributeOfType<DescriptionAttribute>().Description,
                            Value = (int)sexCustomer
                        });
                }
            }

            //2. Lấy danh sách System trên hệ thống
            // TODO: Cần lấy danh sách từ bảng System
            var systemDb = UnitOfWork.SystemRepo.FindAsNoTracking(x => x.Id > 0).ToList();
            var tempSystemDb = from p in systemDb
                               select new SearchMeta() { Text = p.Domain, Value = p.Id };

            listSystemPotentialCustomer.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listSystemPotentialCustomer.AddRange(tempSystemDb.ToList());

            //3.lấy ra danh sách nhân viên
            var user = UnitOfWork.UserRepo.FindAsNoTracking(x => x.Id > 0).ToList();
            var userPosition = UnitOfWork.UserPositionRepo.FindAsNoTracking(x => x.UserId > 0).ToList();//LAY RA DANH SACH VI TRI NHAN VIEN
            var office = UnitOfWork.OfficeRepo.FindAsNoTracking(x => x.Id > 0).ToList();

            var tempUserList = from p in user
                               select new SearchMeta() { Text = p.FullName, Value = p.Id };

            listUserPotentialCustomer.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listUserPotentialCustomer.AddRange(tempUserList.ToList());

            //4. Lấy ra danh sách nhân viên thuộc phòng này
            var tempUserCskhList = from u in user
                                   from p in userPosition
                                   from o in office
                                   where u.Id == p.UserId && p.OfficeId == o.Id && o.Id == UserState.OfficeId
                                   select new SearchMeta()
                                   {
                                       Text = "(" + u.UserName + ")" + " - " + u.FullName,
                                       Value = u.Id
                                   };

            listUserOfPosition.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listUserOfPosition.AddRange(tempUserCskhList.ToList());

            //5.Lấy ra danh sách loại khách hàng(CustomerStype)
            var customerStype = UnitOfWork.CustomerTypeRepo.FindAsNoTracking(x => !x.IsDelete && x.Status == 1 && x.Id > 0).ToList();
            var tempCustomerTypeDb = from p in customerStype
                                     select new SearchMeta() { Text = p.NameType, Value = p.Id };

            listCustomerType.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listCustomerType.AddRange(tempCustomerTypeDb.ToList());

            return Json(new
            {
                listSystemPotentialCustomer,
                listUserPotentialCustomer,
                listCustomerType,
                listPotentialCustomerSex,
                listUserOfPosition
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[CheckPermission(EnumAction.View, EnumPage.PotentialCustomer)]
        public async Task<JsonResult> GetAllPotentialCustomerList(int page, int pageSize,
            CustomerSearchModal searchModal)
        {
            List<PotentialCustomer> potentialCustomerModal;
            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new CustomerSearchModal();
            }
            searchModal.Keyword = string.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();

            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);

                potentialCustomerModal = await UnitOfWork.PotentialCustomerRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.Email.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && !x.IsDelete
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.GenderId == -1 || x.GenderId == searchModal.GenderId)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                         && (searchModal.CustomerType == -1 || x.CustomerTypeId == searchModal.CustomerType)
                         && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                         && (UserState.Type != 0 || x.UserId == UserState.UserId)
                         && x.Created >= dateStart && x.Created <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            else
            {
                potentialCustomerModal = await UnitOfWork.PotentialCustomerRepo.FindAsync(
                    out totalRecord,
                       x => (x.Code.Contains(searchModal.Keyword) || x.Email.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && !x.IsDelete
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.GenderId == -1 || x.GenderId == searchModal.GenderId)
                         && (UserState.Type != 0 || x.UserId == UserState.UserId)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                         && (searchModal.CustomerType == -1 || x.CustomerTypeId == searchModal.CustomerType)
                         && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                         && (UserState.Type != 0 || x.UserId == UserState.UserId),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }

            return Json(new { totalRecord, potentialCustomerModal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[CheckPermission(EnumAction.View, EnumPage.PotentialCustomer)]
        public async Task<JsonResult> GetPotentialCustomerDetail(int potentialCustomerId)
        {
            var result = true;
            //danh sach khach hang
            var potentialCustomerModal =
                await UnitOfWork.PotentialCustomerRepo.FirstOrDefaultAsync(p => p.Id == potentialCustomerId);
            if (potentialCustomerModal == null)
            {
                result = false;
            }
            //danh sach tinh thanh
            var listProvince = UnitOfWork.DbContext.Provinces;
            return Json(new { result, potentialCustomerModal, listProvince }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetDistrict(int provinceId)
        {
            var listDistrict = new List<dynamic>() { new { Text = "- All -", Value = -1 } };

            var province = await UnitOfWork.ProvinceRepo.FirstOrDefaultAsync(s => s.Id == provinceId);
            if (province != null)
            {
                foreach (var item in UnitOfWork.DistrictRepo.Find(s => s.ProvinceId == provinceId).ToList())
                {
                    listDistrict.Add(new
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                }
            }
            return Json(listDistrict, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetWard(int districtId)
        {
            var listWard = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var district = await UnitOfWork.DistrictRepo.FirstOrDefaultAsync(s => s.Id == districtId);
            if (district != null)
            {
                foreach (var item in UnitOfWork.WardRepo.Find(s => s.DistrictId == districtId).ToList())
                {
                    listWard.Add(new
                    {
                        Text = item.Name,
                        Value = item.Id
                    });
                }
            }
            return Json(listWard, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Sửa khách hàng tiềm năng
        /// </summary>
        /// <param name="active"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.Update, EnumPage.PotentialCustomer)]
        public async Task<JsonResult> EditPotentialCustomer(PotentialCustomerMeta model)
        {
            //check form
            ModelState.Remove("Id");
            ModelState.Remove("Created");
            ModelState.Remove("Updated");
            ModelState.Remove("WardId");
            ModelState.Remove("ProvinceId");
            ModelState.Remove("DistrictId");
            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = "Incomplete, inaccurate date or misformatted date of birth!" },
                    JsonRequestBehavior.AllowGet);
            }

            if (model.Birthday >= DateTime.Now)
            {
                return Json(new { status = Result.Failed, msg = " Birthday exceeds current date!" },
                    JsonRequestBehavior.AllowGet);
            }

            //1. Kiểm tra sự tồn tại của potentialCustomer
            var potentialCustomer = UnitOfWork.PotentialCustomerRepo.SingleOrDefault(x => !x.IsDelete && x.Id == model.Id);
            if (potentialCustomer == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.PotentialCustomerIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }


            //2. Lấy về thông tin System
            var system = UnitOfWork.SystemRepo.FirstOrDefault(s => s.Id == model.SystemId);
            if (system == null)
            {
                return Json(new { status = Result.Failed, msg = "Customer's system does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            model.SystemName = system.Domain;



            //3. Add lại thông tin về loại khách hàng
            var customerType = UnitOfWork.CustomerTypeRepo.SingleOrDefault(x => x.Id == model.CustomerTypeId);
            if (customerType == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.CustomerTypeIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                model.CustomerTypeId = customerType.Id;
                model.CustomerTypeName = customerType.NameType;
            }
            //4. Lay ve thanh pho
            var province = UnitOfWork.ProvinceRepo.FirstOrDefault(s => s.Id == model.ProvinceId);
            //if (province == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "City does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //5. Lay ve huyen
            var district = UnitOfWork.DistrictRepo.FirstOrDefault(s => s.Id == model.DistrictId);
            //if (district == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "District does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //6. Lay ve xa
            var ward = UnitOfWork.WardRepo.FirstOrDefault(s => s.Id == model.WardId);
            //if (ward == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "Commune does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //7. Lay ve loai khach hang
            var type = UnitOfWork.CustomerTypeRepo.FirstOrDefault(s => s.Id == model.CustomerTypeId);
            if (type == null)
            {
                return Json(new { status = Result.Failed, msg = "Type of customer does not exist" },
                 JsonRequestBehavior.AllowGet);
            }
            //8. Kiểm tra lại đối tượng người dùng
            var subjectType = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => x.Idd == (int)EnumAccountantSubject.Customer);
            if (subjectType != null)
            {
                model.TypeId = subjectType.Id;
                model.TypeIdd = subjectType.Idd;
                model.TypeName = subjectType.SubjectName;
            }

            //9. Kiểm tra lại thông tin nhân viên kinh doanh quản lý khách hàng.
            if (model.UserId != null)
            {
                var userDetail = UnitOfWork.UserRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && (x.Id == model.UserId));
                if (userDetail != null)
                {
                    model.UserId = userDetail.Id;
                    model.UserFullName = userDetail.FullName;
                    // Lấy thông tin phòng ban
                    var userPostion =
                        await
                            UnitOfWork.UserPositionRepo.FirstOrDefaultAsNoTrackingAsync(x => x.UserId == userDetail.Id && x.IsDefault);
                    if (userPostion != null)
                    {
                        model.OfficeId = userPostion.OfficeId;
                        model.OfficeName = userPostion.OfficeName;
                        model.OfficeIdPath = userPostion.OfficeIdPath;
                    }
                }
            }

            //10. Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {

                    Mapper.Map(model, potentialCustomer);
                    potentialCustomer.Updated = DateTime.Now;
                    potentialCustomer.GenderName = EnumHelper.GetEnumDescription<SexCustomer>((int)potentialCustomer.GenderId);
                    potentialCustomer.CustomerTypeName = type.NameType;


                    potentialCustomer.WardId = -1;
                    potentialCustomer.WardsName = "Thailand";
                    potentialCustomer.DistrictId = -1;
                    potentialCustomer.DistrictName = "Thailand";
                    potentialCustomer.ProvinceId = -1;
                    potentialCustomer.ProvinceName = "Thailand";


                    //UnitOfWork.PotentialCustomerRepo.Update(potentialCustomer);
                    UnitOfWork.PotentialCustomerRepo.Save();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();

                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError },
                //        JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.EditPotentialCustomerSuccess },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.PotentialCustomer)]
        public async Task<JsonResult> CreateNewPotentialCustomer(PotentialCustomerMeta model)
        {
            ModelState.Remove("Id");
            ModelState.Remove("ProvinceId");
            ModelState.Remove("WardId");
            ModelState.Remove("DistrictId");
            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = "Incomplete, inaccurate date or misformatted date of birth!" },
                    JsonRequestBehavior.AllowGet);
            }
            if (model.Birthday >= DateTime.Now)
            {
                return Json(new { status = Result.Failed, msg = "Birthday exceeds current date!" },
                    JsonRequestBehavior.AllowGet);
            }
            //1. Kiểm tra thông tin System
            var systemDetail = await UnitOfWork.SystemRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == model.SystemId && x.Status == 1);
            if (systemDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.SystemIsNotValid },
                   JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra Email khách hàng tiềm năng đã tồn tại hay chưa ?
            var potentialcustomer = UnitOfWork.PotentialCustomerRepo.FirstOrDefaultAsNoTracking(
                x => !x.IsDelete && (x.Email == model.Email));
            if (potentialcustomer != null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.EmailCustomerIsValid },
                    JsonRequestBehavior.AllowGet);
            }

            //3. Kiểm tra Email khách hàng tiềm năng đã là khách hàng thực tế hay chưa ?
            var customer = UnitOfWork.CustomerRepo.FirstOrDefaultAsNoTracking(
                x => !x.IsDelete && (x.Email == model.Email));
            if (customer != null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.CustomerIsValid },
                    JsonRequestBehavior.AllowGet);
            }

            //4. Kiểm tra lại thông tin nhân viên kinh doanh quản lý khách hàng.
            if (model.UserId != null)
            {
                var userDetail = UnitOfWork.UserRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && (x.Id == model.UserId));
                if (userDetail != null)
                {
                    model.UserId = userDetail.Id;
                    model.UserFullName = userDetail.FullName;
                    // model.OfficeId = userDetail
                    // Lấy thông tin phòng ban
                    var userPostion =
                        await
                            UnitOfWork.UserPositionRepo.FirstOrDefaultAsNoTrackingAsync(x => x.UserId == userDetail.Id && x.IsDefault);
                    if (userPostion != null)
                    {
                        model.OfficeId = userPostion.OfficeId;
                        model.OfficeName = userPostion.OfficeName;
                        model.OfficeIdPath = userPostion.OfficeIdPath;
                    }
                }
            }

            //5.kiểm tra loại khách hàng có tồn tại hay không
            var customerType = UnitOfWork.CustomerTypeRepo.FirstOrDefaultAsNoTracking(
                x => !x.IsDelete && (x.Id == model.CustomerTypeId));

            if (customerType == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.CustomerStypeIsValid },
                 JsonRequestBehavior.AllowGet);
            }

            //6. Kiểm tra lại đối tượng người dùng
            var subjectType = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => x.Idd == (int)EnumAccountantSubject.Customer);
            if (subjectType != null)
            {
                model.TypeId = subjectType.Id;
                model.TypeIdd = subjectType.Idd;
                model.TypeName = subjectType.SubjectName;
            }
            else
            {
                return Json(new { status = Result.Failed, msg = "Unable to get user object !" }, JsonRequestBehavior.AllowGet);
            }

            //7. Lay ve thanh pho
            var province = UnitOfWork.ProvinceRepo.FirstOrDefault(s => s.Id == model.ProvinceId);
            //if (province == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "City does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //8. Lay ve huyen
            var district = UnitOfWork.DistrictRepo.FirstOrDefault(s => s.Id == model.DistrictId);
            //if (district == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "District does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //9. Lay ve xa
            var ward = UnitOfWork.WardRepo.FirstOrDefault(s => s.Id == model.WardId);
            //if (ward == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "Commune does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //10. Lay ve loai khach hang
            var type = UnitOfWork.CustomerTypeRepo.FirstOrDefault(s => s.Id == model.CustomerTypeId);
            if (type == null)
            {
                return Json(new { status = Result.Failed, msg = "Type of customer does not exist" },
                 JsonRequestBehavior.AllowGet);
            }
            //11. Lưu lại vào Database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var timeNow = DateTime.Now;

                    var potentialCustomerDetail = Mapper.Map<PotentialCustomer>(model);
                    potentialCustomerDetail.Code = string.Empty;
                    potentialCustomerDetail.Avatar = "/Content/img/no-avatar.png";
                    potentialCustomerDetail.GenderName = EnumHelper.GetEnumDescription<SexCustomer>((int)potentialCustomerDetail.GenderId);
                    potentialCustomerDetail.LevelId = 1;
                    potentialCustomerDetail.LevelName = "Vip 0";
                    potentialCustomerDetail.CustomerTypeName = type.NameType;

                    potentialCustomerDetail.DistrictId = -1;
                    potentialCustomerDetail.DistrictName = "Thailand";
                    potentialCustomerDetail.ProvinceId = -1;
                    potentialCustomerDetail.ProvinceName = "Thailand";
                    potentialCustomerDetail.WardId = -1;
                    potentialCustomerDetail.WardsName = "Thailand";
                    potentialCustomerDetail.HashTag = model.FullName + "," + model.Nickname + "," + model.Email;
                    potentialCustomerDetail.SystemName = systemDetail.Domain;
                    potentialCustomerDetail.IsDelete = false;
                    potentialCustomerDetail.Created = timeNow;
                    potentialCustomerDetail.Updated = timeNow;
                    potentialCustomerDetail.CustomerTypeName = customerType.NameType;

                    // Thêm
                    UnitOfWork.PotentialCustomerRepo.Add(potentialCustomerDetail);
                    UnitOfWork.PotentialCustomerRepo.Save();

                    // Cập nhật Code
                    var potentialNo = UnitOfWork.PotentialCustomerRepo.Count(x => x.Id <= potentialCustomerDetail.Id);
                    potentialCustomerDetail.Code = MyCommon.GenCode(potentialNo);

                    UnitOfWork.PotentialCustomerRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError },
                //        JsonRequestBehavior.AllowGet);
                //}
            }
            var totalPotialCustomerOfStaff = UnitOfWork.PotentialCustomerRepo.Count(x => !x.IsDelete && x.UserId == UserState.UserId);
            var totalPotialCustomer = UnitOfWork.PotentialCustomerRepo.Count(x => !x.IsDelete && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                         && (UserState.Type != 0 || x.UserId == UserState.UserId));
            return Json(new { status = Result.Succeed, msg = ConstantMessage.CreateNewPotentialCustomerSuccess, totalPotialCustomerOfStaff, totalPotialCustomer },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.PotentialCustomer)]
        public async Task<ActionResult> DeletePotentialCustomer(int potentialCustomerId)
        {
            var potentialCustomer =
                await UnitOfWork.PotentialCustomerRepo.SingleOrDefaultAsync(x => x.Id == potentialCustomerId && x.IsDelete == false);
            if (potentialCustomer == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);
            potentialCustomer.IsDelete = true;
            UnitOfWork.PotentialCustomerRepo.Update(potentialCustomer);
            var rs = await UnitOfWork.PotentialCustomerRepo.SaveAsync();
            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        #endregion Khách hàng tiềm năng - PotentialCustomer

        #region Khách hàng tiềm năng đang phụ trách - PotentialCustomerByUser

        /// <summary>
        /// danh sach khach hang tiem nang toi phu trach
        /// </summary>
        /// <param name="active"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetAllPotentialCustomerByUserSearchData()
        {
            var listSystemPotentialCustomerByUser = new List<SearchMeta>();
            var listUserPotentialCustomerByUser = new List<SearchMeta>();
            var listSexPotentialCustomerByUser = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listCustomerTypePotentialCustomerByUser = new List<SearchMeta>();

            //1. Lấy danh sách giới tính
            foreach (SexCustomer sexCustomer in Enum.GetValues(typeof(SexCustomer)))
            {
                if (sexCustomer >= 0)
                {
                    listSexPotentialCustomerByUser.Add(
                        new
                        {
                            Text = sexCustomer.GetAttributeOfType<DescriptionAttribute>().Description,
                            Value = (int)sexCustomer
                        });
                }
            }
            //2. Lấy danh sách System trên hệ thống
            var systemDb = UnitOfWork.SystemRepo.FindAsNoTracking(x => x.Id > 0).ToList();
            var tempSystemDb = from p in systemDb
                               select new SearchMeta() { Text = p.Domain, Value = p.Id };

            listSystemPotentialCustomerByUser.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listSystemPotentialCustomerByUser.AddRange(tempSystemDb.ToList());

            //3.lấy ra danh sách nhân viên
            var user = UnitOfWork.UserRepo.FindAsNoTracking(x => x.Id > 0).ToList();
            var tempUserList = from p in user
                               select new SearchMeta() { Text = p.FullName, Value = p.Id };

            listUserPotentialCustomerByUser.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listUserPotentialCustomerByUser.AddRange(tempUserList.ToList());

            //4.Lấy ra danh sách loại khách hàng(CustomerStype)
            var customerStype = UnitOfWork.CustomerTypeRepo.FindAsNoTracking(x => !x.IsDelete && x.Status == 1 && x.Id > 0).ToList();
            var tempCustomerTypeDb = from p in customerStype
                                     select new SearchMeta() { Text = p.NameType, Value = p.Id };

            listCustomerTypePotentialCustomerByUser.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listCustomerTypePotentialCustomerByUser.AddRange(tempCustomerTypeDb.ToList());

            return Json(new
            {
                listSystemPotentialCustomerByUser,
                listUserPotentialCustomerByUser,
                listSexPotentialCustomerByUser,
                listCustomerTypePotentialCustomerByUser
            },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy ra danh sách tôi khách hàng tiềm năng phụ trách
        /// </summary>
        /// <param name="active"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.PotentialCustomerByStaff)]
        public async Task<JsonResult> GetAllPotentialCustomerListByUser(int page, int pageSize,
                  CustomerSearchModal searchModal)
        {
            List<PotentialCustomer> potentialCustomerByUserModal;
            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new CustomerSearchModal();
            }
            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);
                potentialCustomerByUserModal = await UnitOfWork.PotentialCustomerRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && (!x.IsDelete)
                         && (x.UserId == UserState.UserId)
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.GenderId == -1 || x.GenderId == searchModal.GenderId)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                         && (searchModal.CustomerType == -1 || x.CustomerTypeId == searchModal.CustomerType)
                         && x.Created >= dateStart && x.Created <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );


            }
            else
            {
                potentialCustomerByUserModal = await UnitOfWork.PotentialCustomerRepo.FindAsync(
                    out totalRecord,
                       x => (x.Code.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && (!x.IsDelete)
                         && (x.UserId == UserState.UserId)
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.GenderId == -1 || x.GenderId == searchModal.GenderId)
                         && (searchModal.CustomerType == -1 || x.CustomerTypeId == searchModal.CustomerType)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }

            return Json(new { totalRecord, potentialCustomerByUserModal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.PotentialCustomerByStaff)]
        public async Task<JsonResult> CreateNewPotentialCustomerByUser(PotentialCustomerMeta model)
        {
            ModelState.Remove("Id");
            ModelState.Remove("DistrictId"); ModelState.Remove("ProvinceId");
            ModelState.Remove("WardId");

            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = "Incomplete, inaccurate date or misformatted date of birth!" },
                    JsonRequestBehavior.AllowGet);
            }

            if (model.Birthday >= DateTime.Now)
            {
                return Json(new { status = Result.Failed, msg = "Birthday exceeds current date!" },
                    JsonRequestBehavior.AllowGet);
            }

            //1. Kiểm tra thông tin System
            var systemDetail = await UnitOfWork.SystemRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == model.SystemId && x.Status == 1);
            if (systemDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.SystemIsNotValid },
                   JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra Email khách hàng tiềm năng đã tồn tại hay chưa ?
            var potentialcustomer = UnitOfWork.PotentialCustomerRepo.FirstOrDefaultAsNoTracking(
                x => x.IsDelete && (x.Email == model.Email));
            if (potentialcustomer != null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.EmailCustomerIsValid },
                    JsonRequestBehavior.AllowGet);
            }

            //3. Kiểm tra Email khách hàng tiềm năng đã là khách hàng thực tế hay chưa ?
            var customer = UnitOfWork.CustomerRepo.FirstOrDefaultAsNoTracking(
                x => x.IsDelete && (x.Email == model.Email));
            if (customer != null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.CustomerIsValid },
                    JsonRequestBehavior.AllowGet);
            }

            //4. Kiểm tra lại thông tin nhân viên kinh doanh quản lý khách hàng.
            if (model.UserId != null)
            {
                var userDetail = UnitOfWork.UserRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && (x.Id == model.UserId));
                if (userDetail != null)
                {
                    model.UserId = userDetail.Id;
                    model.UserFullName = userDetail.FullName;
                    // model.OfficeId = userDetail
                    // Lấy thông tin phòng ban
                    var userPostion =
                        await
                            UnitOfWork.UserPositionRepo.FirstOrDefaultAsNoTrackingAsync(x => x.UserId == userDetail.Id && x.IsDefault);
                    if (userPostion != null)
                    {
                        model.OfficeId = userPostion.OfficeId;
                        model.OfficeName = userPostion.OfficeName;
                        model.OfficeIdPath = userPostion.OfficeIdPath;
                    }
                }
            }

            //5. Kiểm tra loại khách hàng có tồn tại hay không
            var customerType = UnitOfWork.CustomerTypeRepo.FirstOrDefaultAsNoTracking(
                x => !x.IsDelete && (x.Id == model.CustomerTypeId));

            if (customerType == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.CustomerStypeIsValid },
                 JsonRequestBehavior.AllowGet);
            }

            //6. Kiểm tra lại đối tượng người dùng
            var subjectType = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => x.Idd == (int)EnumAccountantSubject.Customer);
            if (subjectType != null)
            {
                model.TypeId = subjectType.Id;
                model.TypeIdd = subjectType.Idd;
                model.TypeName = subjectType.SubjectName;
            }
            else
            {
                return Json(new { status = Result.Failed, msg = "Unable to get user object !" }, JsonRequestBehavior.AllowGet);
            }

            //7. Lay ve thanh pho
            var province = UnitOfWork.ProvinceRepo.FirstOrDefault(s => s.Id == model.ProvinceId);
            //if (province == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "City does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //8. Lay ve huyen
            var district = UnitOfWork.DistrictRepo.FirstOrDefault(s => s.Id == model.DistrictId);
            //if (district == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "District does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //9. Lay ve xa
            var ward = UnitOfWork.WardRepo.FirstOrDefault(s => s.Id == model.WardId);
            //if (ward == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "Commune does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //10. Lay ve loai khach hang
            var type = UnitOfWork.CustomerTypeRepo.FirstOrDefault(s => s.Id == model.CustomerTypeId);
            if (type == null)
            {
                return Json(new { status = Result.Failed, msg = "Type of customer does not exist" },
                 JsonRequestBehavior.AllowGet);
            }
            //11. Lưu lại vào Database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var timeNow = DateTime.Now;

                    var potentialCustomerDetail = Mapper.Map<PotentialCustomer>(model);
                    potentialCustomerDetail.Code = string.Empty;

                    potentialCustomerDetail.GenderName = EnumHelper.GetEnumDescription<SexCustomer>((int)potentialCustomerDetail.GenderId);
                    potentialCustomerDetail.LevelId = 1;
                    potentialCustomerDetail.Avatar = "/Content/img/no-avatar.png";
                    potentialCustomerDetail.LevelName = "Vip 0";
                    potentialCustomerDetail.CustomerTypeName = type.NameType;
                    potentialCustomerDetail.DistrictId = -1;
                    potentialCustomerDetail.DistrictName = "Thailand";
                    potentialCustomerDetail.ProvinceId = -1;
                    potentialCustomerDetail.ProvinceName = "Thailand";
                    potentialCustomerDetail.WardId = -1;
                    potentialCustomerDetail.WardsName = "Thailand";
                    potentialCustomerDetail.HashTag = model.FullName + "," + model.Nickname + "," + model.Email;
                    potentialCustomerDetail.SystemName = systemDetail.Domain;
                    potentialCustomerDetail.IsDelete = false;
                    potentialCustomerDetail.UserId = UserState.UserId;
                    potentialCustomerDetail.UserFullName = UserState.FullName;
                    potentialCustomerDetail.Created = timeNow;
                    potentialCustomerDetail.Updated = timeNow;
                    potentialCustomerDetail.CustomerTypeName = customerType.NameType;

                    // Thêm
                    UnitOfWork.PotentialCustomerRepo.Add(potentialCustomerDetail);
                    UnitOfWork.PotentialCustomerRepo.Save();

                    // Cập nhật Code
                    var potentialNo = UnitOfWork.PotentialCustomerRepo.Count(x => x.Id <= potentialCustomerDetail.Id);
                    potentialCustomerDetail.Code = MyCommon.GenCode(potentialNo);

                    UnitOfWork.PotentialCustomerRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError },
                //        JsonRequestBehavior.AllowGet);
                //}
            }

            var totalPotialCustomerOfStaff = UnitOfWork.PotentialCustomerRepo.Count(x => !x.IsDelete && x.UserId == UserState.UserId);
            var totalPotialCustomer = UnitOfWork.PotentialCustomerRepo.Count(x => !x.IsDelete && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                         && (UserState.Type != 0 || x.UserId == UserState.UserId));

            return Json(new { status = Result.Succeed, msg = ConstantMessage.CreateNewPotentialCustomerSuccess, totalPotialCustomer, totalPotialCustomerOfStaff },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Update, EnumPage.PotentialCustomerByStaff)]
        public async Task<JsonResult> EditPotentialCustomerByUser(PotentialCustomerMeta model)
        {
            //check form
            ModelState.Remove("Id");
            ModelState.Remove("Created");
            ModelState.Remove("Updated");
            ModelState.Remove("DistrictId");
            ModelState.Remove("ProvinceId");
            ModelState.Remove("WardId");
            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = "Incomplete, inaccurate date or misformatted date of birth!" },
                    JsonRequestBehavior.AllowGet);
            }

            if (model.Birthday >= DateTime.Now)
            {
                return Json(new { status = Result.Failed, msg = "Birthday exceeds current date!" },
                    JsonRequestBehavior.AllowGet);
            }
            //1. Kiểm tra su ton tai cua potentialCustomer
            var potentialCustomer = UnitOfWork.PotentialCustomerRepo.SingleOrDefault(x => !x.IsDelete && x.Id == model.Id);

            if (potentialCustomer == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.PotentialCustomerIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra thông tin System
            var systemDetail = UnitOfWork.SystemRepo.FirstOrDefault(x => x.Id == model.SystemId && x.Status == 1);
            if (systemDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.SystemIsNotValid },
                   JsonRequestBehavior.AllowGet);
            }

            //3. Add lại thông tin về loại khách hàng
            var customerType = UnitOfWork.CustomerTypeRepo.SingleOrDefault(x => x.Id == model.CustomerTypeId);
            if (customerType == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.CustomerTypeIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                model.CustomerTypeId = customerType.Id;
                model.CustomerTypeName = customerType.NameType;
            }

            //7. Lay ve thanh pho
            var province = UnitOfWork.ProvinceRepo.FirstOrDefault(s => s.Id == model.ProvinceId);
            //if (province == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "City does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //8. Lay ve huyen
            var district = UnitOfWork.DistrictRepo.FirstOrDefault(s => s.Id == model.DistrictId);
            //if (district == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "District does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //9. Lay ve xa
            var ward = UnitOfWork.WardRepo.FirstOrDefault(s => s.Id == model.WardId);
            //if (ward == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "Commune does not exist" },
            //     JsonRequestBehavior.AllowGet);
            //}
            //10. Lay ve loai khach hang
            var type = UnitOfWork.CustomerTypeRepo.FirstOrDefault(s => s.Id == model.CustomerTypeId);
            if (type == null)
            {
                return Json(new { status = Result.Failed, msg = "Type of customer does not exist" },
                 JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra lại đối tượng người dùng
            var subjectType = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => x.Idd == (int)EnumAccountantSubject.Customer);
            if (subjectType != null)
            {
                model.TypeId = subjectType.Id;
                model.TypeIdd = subjectType.Idd;
                model.TypeName = subjectType.SubjectName;
            }


            //3. Kiểm tra lại thông tin nhân viên kinh doanh quản lý khách hàng.
            if (model.UserId != null)
            {
                var userDetail = UnitOfWork.UserRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && (x.Id == model.UserId));
                if (userDetail != null)
                {
                    model.UserId = userDetail.Id;
                    model.UserFullName = userDetail.FullName;
                    // model.OfficeId = userDetail
                    // Lấy thông tin phòng ban
                    var userPostion =
                        await
                            UnitOfWork.UserPositionRepo.FirstOrDefaultAsNoTrackingAsync(x => x.UserId == userDetail.Id && x.IsDefault);
                    if (userPostion != null)
                    {
                        model.OfficeId = userPostion.OfficeId;
                        model.OfficeName = userPostion.OfficeName;
                        model.OfficeIdPath = userPostion.OfficeIdPath;
                    }
                }
            }

            //11. Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {

                    Mapper.Map(model, potentialCustomer);
                    potentialCustomer.GenderName = EnumHelper.GetEnumDescription<SexCustomer>((int)potentialCustomer.GenderId);
                    potentialCustomer.Updated = DateTime.Now;
                    potentialCustomer.SystemName = systemDetail.Domain;
                    potentialCustomer.CustomerTypeName = type.NameType;
                    potentialCustomer.DistrictId = -1;
                    potentialCustomer.DistrictName = "Thailand";
                    potentialCustomer.ProvinceId = -1;
                    potentialCustomer.ProvinceName = "Thailand";
                    potentialCustomer.WardId = -1;
                    potentialCustomer.WardsName = "Thailand";

                    UnitOfWork.PotentialCustomerRepo.Save();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();

                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError },
                //        JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.EditPotentialCustomerSuccess },
                JsonRequestBehavior.AllowGet);
        }

        #endregion Khách hàng tiềm năng đang phụ trách - PotentialCustomerByUser

        #region [Danh sách khách hàng chậm đặt hàng]

        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.Customer)]
        public async Task<JsonResult> GetOrderPendingList(int page, int pageSize, CustomerSearchModal searchModal)
        {
            long totalRecord;

            if (searchModal == null)
            {
                searchModal = new CustomerSearchModal();
            }

            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            var customerModal = await UnitOfWork.CustomerRepo.GetCustomerOrderPendingList(out totalRecord, page, pageSize, searchModal.Money, DateStart, DateEnd, UserState);
            return Json(new { totalRecord, customerModal }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Danh sách khách hàng chậm đặt hàng]

        #region Lấy danh sách hệ thống

        //gender khach hang tiem nang
        [HttpPost]
        public async Task<JsonResult> GetRenderSystemPotentialCustomer(string active)
        {
            var listOrderCustomerType = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listOrderCustomer = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listPotentialCustomer = new List<SystemMeta>();
            var listuser = new List<Complain>();
            var listcomplainuser = new List<TicketComplain>();
            if (active == "ticket-support")
            {
                listcomplainuser = UnitOfWork.ComplainRepo.SystemTicketSupport(UserState);
                listPotentialCustomer = listcomplainuser.Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();
            }

            //Check user có phải là nhân viên kinh doanh công ty ko?
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeTypeCompany(OfficeType.Business);
            var checkUser = listUser.FirstOrDefault(x => x.Id == UserState.UserId);
            var check = checkUser != null;

            //Danh sách khách hàng chính thức
            var listcustomer = await UnitOfWork.CustomerRepo.FindAsNoTrackingAsync(
                    x => !x.IsDelete
                         && (UserState.Type != 0 || x.OfficeId == UserState.OfficeId)
                         && (UserState.Type != 0 || x.UserId == UserState.UserId)
                         && (check == false || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".") || x.UserId == null)))
                         && (check == true || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + "."))))
                );
            if (active == "customer")
            {
                listPotentialCustomer = listcustomer.Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();
            }
            // Danh sách khách hàng tiềm năng
            var listcustomerPo = await UnitOfWork.PotentialCustomerRepo.FindAsNoTrackingAsync(
                    x => !x.IsDelete
                         && (UserState.Type != 0 || x.OfficeId == UserState.OfficeId)
                         && (UserState.Type != 0 || x.UserId == UserState.UserId)
                         && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                );
            //khách hàng phụ trách
            if (active == "customerfeasibility-by-staff")
            {
                var listcustomerstaff = listcustomerPo.FindAll(s => s.UserId == UserState.UserId);
                listPotentialCustomer = listcustomerstaff.Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();
            }


            if (active == "PotentialCustomer")
            {
                listPotentialCustomer = listcustomerPo.Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();
            }

            //khách hàng phụ trách
            if (active == "customer-by-staff")
            {
                var listcustomerPostaff = listcustomer.FindAll(s => s.UserId == UserState.UserId);
                listPotentialCustomer = listcustomerPostaff.Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();
            }

            //5. Lấy danh sách hệ thống
            var listSystemPotentialCustomer = new List<dynamic>()
            {
                new
                {
                    Text =  Resource.All,
                    Value = -1,
                    Class = "active",
                    Total = listPotentialCustomer.Count,
                    ClassChild = "label-danger"
                }
            };

            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);

            foreach (var item in listSystemDb)
            {

                listSystemPotentialCustomer.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                    Class = "",
                    Total = listPotentialCustomer.Count(x => x.SystemId == item.Id),
                    ClassChild = "label-primary"
                });
            }
            //Lấy danh sách trạng thái đơn hàng
            foreach (OrderStatus warehouseCustomerStatus in Enum.GetValues(typeof(OrderStatus)))
            {
                if (warehouseCustomerStatus >= 0)
                {
                    listOrderCustomer.Add(
                        new
                        {
                            Text = warehouseCustomerStatus.GetAttributeOfType<DescriptionAttribute>().Description,
                            Value = (int)warehouseCustomerStatus
                        });
                }
            }
            //Lấy dánh sách loại đơn hàng
            foreach (OrderType warehouseCustomerStatus in Enum.GetValues(typeof(OrderType)))
            {
                if (warehouseCustomerStatus >= 0)
                {
                    listOrderCustomerType.Add(
                        new
                        {
                            Text = warehouseCustomerStatus.GetAttributeOfType<DescriptionAttribute>().Description,
                            Value = (int)warehouseCustomerStatus
                        });
                }
            }

            return Json(new { listSystemPotentialCustomer, listOrderCustomer, listOrderCustomerType }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetInitPotentialCustomer()
        {
            var listPotentialCustomer = await UnitOfWork.PotentialCustomerRepo.FindAsNoTrackingAsync(p => !p.IsDelete);
            var totalPotentialCustomer = listPotentialCustomer.Count(); //lay ra tat ca KHACH HANG TIEM NANG
            //var totalPotentialCustomerByUser= listPotentialCustomer.Count();
            return Json(new { totalPotentialCustomer }, JsonRequestBehavior.AllowGet);
        }

        #endregion Lấy danh sách hệ thống

        #region Hỗ trợ giải quyết khiếu nại

        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.BussinessSupport)]
        public async Task<JsonResult> GetAllSearchComplainData()
        {
            var listStatus = new List<dynamic>() { new { Text = Resource.All, Value = -1 } };
            var listSystem = new List<dynamic>() { new { Text = Resource.All, Value = -1 } };

            // Lấy các trạng thái Status của Complain
            foreach (ComplainStatus ticketStatus in Enum.GetValues(typeof(ComplainStatus)))
            {
                if (ticketStatus >= 0)
                {
                    listStatus.Add(new { Text = ticketStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)ticketStatus });
                }
            }

            // Lấy danh sách System
            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);
            foreach (var item in listSystemDb)
            {
                listSystem.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                });
            }
            return Json(new { listStatus, listSystem }, JsonRequestBehavior.AllowGet);
        }

        public int CountUser(long complainId)
        {
            var userId = UserState.UserId;
            var count = UnitOfWork.ComplainUserRepo.Find(s => s.ComplainId == complainId && s.UserId == UserState.UserId && s.IsCare == false).Count();
            return count;
        }

        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.BussinessSupport)]
        public async Task<JsonResult> GetAllTicketList(int page, int pageSize, ComplainSearchModal searchModal)
        {
            //var ticketModal = new List<TicketComplain>();
            //var office = (byte)OfficeType.Business;
            long totalRecord;

            if (searchModal == null)
            {
                searchModal = new ComplainSearchModal();
            }

            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            var dateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var dateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            //2. Lấy ra dữ liệu
            var ticketModal = await UnitOfWork.ComplainRepo.GetAllTicketSupportOfficeList(out totalRecord, page, pageSize,
                searchModal.Keyword, searchModal.Status, searchModal.SystemId, dateStart, dateEnd, UserState);

            return Json(new { totalRecord, ticketModal }, JsonRequestBehavior.AllowGet);
        }

        #region CHI TIẾT KHIẾU NẠI

        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.BussinessSupport)]
        public async Task<JsonResult> GetTicketDetail(int ticketId)
        {
            var customer = new Customer();
            var complainuserlist = new List<ComplainDetail>();
            var list = new List<ComplainDetail>();
            var usersupportlist = new List<User>();
            var usersupport = new User();
            var ticketModal = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(p => p.Id == ticketId);
            //var complainuser = UnitOfWork.DbContext.ComplainUsers.Where(s => s.ComplainId == ticketId).OrderBy(b=>b.CreateDate);
            var complainuser = await UnitOfWork.ComplainUserRepo.FindAsync(
               p => p.ComplainId == ticketId
               && p.UserId != null,
               null,
                x => x.OrderBy(y => y.CreateDate)
               );

            //Lọc nhân viên xử lý
            var complainusersupport = (from d in UnitOfWork.DbContext.ComplainUsers
                                       where (d.ComplainId == ticketId && d.IsCare == false)
                                       orderby d.CreateDate
                                       select new { d.UserId, d.UserName }).Distinct().ToList();
            //Tổng complainusser theo complainId
            foreach (var item in complainuser)
            {
                var posit = new UserPosition();
                var userPosition = "";
                if (item.UserId != null)
                {
                    posit = await UnitOfWork.UserPositionRepo.FirstOrDefaultAsync(d => d.UserId == item.UserId && d.IsDefault);
                    if (posit != null)
                    {
                        userPosition = posit.TitleName;
                    }
                }

                complainuserlist.Add(new ComplainDetail
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    UserName = item.UserName,
                    UserPosition = userPosition,
                    ComplainId = item.ComplainId,
                    Content = item.Content,
                    AttachFile = item.AttachFile,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate,
                    UserRequestId = item.UserRequestId,
                    UserRequestName = item.UserRequestName,
                    CustomerId = item.CustomerId,
                    CustomerName = item.CustomerName,
                    IsRead = item.IsRead,
                    IsCare = item.IsCare
                });
            }

            //Lọc người xử lý(hỗ trợ)
            foreach (var item in complainusersupport)
            {
                var posit = new UserPosition();
                var userPosition = "";
                if (item.UserId != null)
                {
                    posit = await UnitOfWork.UserPositionRepo.FirstOrDefaultAsync(d => d.UserId == item.UserId && d.IsDefault);
                    userPosition = posit.TitleName;
                }
                list.Add(new ComplainDetail
                {
                    Id = 0,
                    UserId = item.UserId,
                    UserName = item.UserName,
                    UserPosition = userPosition,
                    ComplainId = 0
                });
            }

            //Lấy ra nhân viên chịu trách nhiệm chính
            var complainusermain = complainuserlist.SingleOrDefault(s => s.IsCare == true);
            var hascustomer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(p => p.Id == ticketModal.CustomerId);
            customer = hascustomer;
            //}

            return Json(new { ticketModal, customer, complainuserlist, complainusermain, list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllUser()
        {
            var listUser =
                UnitOfWork.DbContext.Users.Select(x => new SelectListItem { Text = x.FullName, Value = x.Id + "" })
                    .ToList();
            return Json(new { listUser }, JsonRequestBehavior.AllowGet);
        }

        //Phản hồi cho khách hàng

        [HttpPost]
        public JsonResult feedbackComplain(string content, int complainId)
        {
            var com = UnitOfWork.ComplainUserRepo.FirstOrDefault(d => d.ComplainId == complainId && d.UserId == UserState.UserId);
            var complainback = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == complainId);
            var complain = new ComplainUser();
            complain.UserId = UserState.UserId;
            complain.UserName = UserState.UserName;
            if (com != null)
            {
                complain.UserRequestId = com.UserRequestId;
                complain.UserRequestName = com.UserRequestName;
            }
            complain.Content = content;
            complain.ComplainId = complainId;
            complain.CreateDate = DateTime.Now;
            complain.CustomerId = complainback.CustomerId;
            complain.CustomerName = complainback.CustomerName;
            UnitOfWork.ComplainUserRepo.Add(complain);
            UnitOfWork.ComplainUserRepo.Save();

            return Json(complainback, JsonRequestBehavior.AllowGet);
        }

        //Hoàn thành ticket
        [HttpPost]
        public JsonResult finishComplain(int complainId)
        {
            var complain = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == complainId);
            complain.Status = (byte)ComplainStatus.Success;
            complain.LastUpdateDate = DateTime.Now;
            UnitOfWork.ComplainRepo.Save();
            return Json(complain, JsonRequestBehavior.AllowGet);
        }

        #endregion CHI TIẾT KHIẾU NẠI

        #endregion Hỗ trợ giải quyết khiếu nại

        #region Thống kê

        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.BussinessReportRevenue)]
        public async Task<JsonResult> GetRevenueReportList(int page, int pageSize, RevenueReportSearchModel searchModal)
        {
            long totalRecord;
            decimal TotalOrderExchange;
            decimal TotalServicePurchase;
            decimal TotalOrderBargain;
            decimal TotalOrderWeight;

            var listId = new List<int>();
            var list = new List<ReportBusinessItem>();

            ReportBusinessModel listRevenueReport = new ReportBusinessModel();
            if (searchModal == null)
            {
                searchModal = new RevenueReportSearchModel();
            }
            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();
            DateTime? start = null;
            DateTime? end = null;
            if (searchModal.DateStart != null)
            {
                start = GetStartOfDay(DateTime.Parse(searchModal.DateStart ?? DateTime.Now.ToShortDateString()));
            }
            if (searchModal.DateEnd != null)
            {
                end = GetEndOfDay(DateTime.Parse(searchModal.DateEnd ?? DateTime.Now.ToShortDateString()));
            }
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            if (searchModal.UserId == -1)
            {
                listId = listUser.Where(x => UserState.Type > 0 || x.Id == UserState.UserId).Select(x => x.Id).ToList();
            }
            else
            {
                listId = listUser.Where(x => x.Id == searchModal.UserId).Select(x => x.Id).ToList();
            }
            list = await UnitOfWork.CustomerRepo.GetOrderUserList(out TotalOrderExchange, out TotalOrderWeight, out TotalServicePurchase, out TotalOrderBargain, out totalRecord, page, pageSize, searchModal.Keyword, searchModal.UserId, searchModal.CustomerStatus, start, end, listId, UserState);

            return Json(new { totalRecord, TotalOrderExchange, TotalServicePurchase, TotalOrderBargain, TotalOrderWeight, list }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetRevenueReportDepositList(int page, int pageSize, RevenueReportSearchModel searchModal)
        {
            long totalRecord;
            decimal TotalOrderExchange;
            decimal TotalOrderWeight;

            var listId = new List<int>();
            var list = new List<ReportBusinessItem>();

            ReportBusinessModel listRevenueReport = new ReportBusinessModel();
            if (searchModal == null)
            {
                searchModal = new RevenueReportSearchModel();
            }
            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();
            DateTime? start = null;
            DateTime? end = null;
            if (searchModal.DateStart != null)
            {
                start = GetStartOfDay(DateTime.Parse(searchModal.DateStart ?? DateTime.Now.ToShortDateString()));
            }
            if (searchModal.DateEnd != null)
            {
                end = GetEndOfDay(DateTime.Parse(searchModal.DateEnd ?? DateTime.Now.ToShortDateString()));
            }

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Deposit, UserState);

            if (searchModal.UserId == -1)
            {
                listId = listUser.Where(x => UserState.Type > 0 || x.Id == UserState.UserId).Select(x => x.Id).ToList();
            }
            else
            {
                listId = listUser.Where(x => x.Id == searchModal.UserId).Select(x => x.Id).ToList();
            }

            list = await UnitOfWork.CustomerRepo.GetDepositUserList(out TotalOrderExchange, out TotalOrderWeight, out totalRecord, page, pageSize, searchModal.Keyword, searchModal.UserId, searchModal.CustomerStatus, start, end, listId, UserState);

            return Json(new { totalRecord, TotalOrderExchange, TotalOrderWeight, list }, JsonRequestBehavior.AllowGet);
        }

        #endregion Thống kê

        [HttpPost]
        public async Task<JsonResult> GetRenderSystemTab(string active)
        {
            var listStatus = new List<dynamic>() { new { Text = Resource.All, Value = -1 } };
            foreach (ComplainStatus complainStatus in Enum.GetValues(typeof(ComplainStatus)))
            {
                if (complainStatus != 0)
                    listStatus.Add(new { Value = (int)complainStatus, Text = complainStatus.GetAttributeOfType<DescriptionAttribute>().Description });
            }

            var listComplain = new List<SystemMeta>();
            var listuser = new List<Complain>();
            var listcomplainuser = new List<Complain>();

            if (active == "ticket-support")
            {
                listcomplainuser = UnitOfWork.ComplainRepo.Find(x => !x.IsDelete).ToList();
                foreach (var item in listcomplainuser)
                {
                    if (CountUser(item.Id) > 0)
                    {
                        var usersupport = UnitOfWork.ComplainUserRepo.FirstOrDefault(s => s.ComplainId == item.Id && s.UserId == UserState.UserId && s.IsCare == false);
                        if (usersupport != null)
                        {
                            listuser.Add(item);
                        }
                    }
                }
            }

            listComplain = listuser.Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();

            //5. Lấy danh sách hệ thống
            var listSystem = new List<dynamic>()
            {
                new
                {
                    Text = Resource.All,
                    Value = -1,
                    Class = "active",
                    Total = listComplain.Count,
                    ClassChild = "label-danger"
                }
            };

            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);

            foreach (var item in listSystemDb)
            {
                listSystem.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                    Class = "",
                    Total = listComplain.Count(x => x.SystemId == item.Id),
                    ClassChild = "label-primary"
                });
            }

            return Json(new { listSystem, listStatus }, JsonRequestBehavior.AllowGet);
        }

        //TODO[Giỏi]: Chuyển khách hàng tiềm năng sang khách hàng chính thức
        public async Task<JsonResult> CustomerOfficialPotential(int id)
        {
            var cus = await UnitOfWork.PotentialCustomerRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);
            if (cus == null)
            {
                return Json(new { status = MsgType.Error, msg = "Potential customer does not exist!" }, JsonRequestBehavior.AllowGet);
            }

            // Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //chuyển trạng thái của khách hàng chờ chuyển chính thức
                    cus.Status = (byte)PotentialCustomerStatus.Await;
                    cus.Updated = DateTime.Now;
                    await UnitOfWork.PotentialCustomerRepo.SaveAsync();

                    //Lấy url website
                    var web = await UnitOfWork.SystemRepo.FirstOrDefaultAsync(x => x.Id == cus.SystemId && x.Status == 1);

                    //Gửi mail cho khách hàng
                    //Phần gửi mail xác thực tài khoản
                    var domain = web.Domain;
                    var filePath = AppDomain.CurrentDomain.BaseDirectory + @"/EmailTemplate/Accounting/createAccount.html";
                    var html = System.IO.File.ReadAllText(filePath);
                    var activeEmail = new CreateAccount(cus.Email, cus.SystemId.Value, DateTime.Now, domain, cus.UserId.Value);
                    html = html.Replace("{{linkActive}}", activeEmail.Link).Replace("{{urlWebsite}}", domain);

                    MailHelper.SendMail(cus.Email, "Create account on " + domain, html, false);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = MsgType.Error, msg = "Khôn thể chuyển khách hàng này sang chính thức!" }, JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(new { status = MsgType.Success, msg = "Email to create official account has been sent to customer!" }, JsonRequestBehavior.AllowGet);
        }

        #region [Xuất excel]

        [HttpPost]
        [CheckPermission(EnumAction.Export, EnumPage.Customer)]
        public async Task<ActionResult> ExportExcelCustomerOfStaff(string keyword, int status, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId)
        {
            long totalRecord;
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var listCustomer = await UnitOfWork.CustomerRepo.FindAsync(
                    out totalRecord,
                    x => (status == -1 || x.Status == status)
                    && (x.Email.Contains(keyword) || x.UserFullName.Contains(keyword))
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.Id == customerId) && (userId == null || x.UserId == userId)
                    && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId),
                    x => x.OrderByDescending(y => y.Created),
                    0,
                    1000000
                );

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "PICTURE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ACCOUNT ID", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TYPE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STAFF", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                //col++;
                //ExcelHelper.CreateHeaderTable(sheet, row, col, "TỔNG ĐƠN", ExcelHorizontalAlignment.Center, true, colorHeader);
                //col++;
                //ExcelHelper.CreateHeaderTable(sheet, row, col, "GIÁ TRỊ TRUNG BÌNH", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title

                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "ORDER LIST", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 12
                });

                var start = dateStart?.ToShortDateString() ?? "__";
                var end = dateEnd?.ToShortDateString() ?? "__";

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start} to date {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });

                #endregion Title

                var no = row + 1;

                if (listCustomer.Any())
                {
                    foreach (var customer in listCustomer)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, customer.Avatar, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, customer.Code, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, customer.LevelName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, customer.Email, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, customer.UserFullName, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<CustomerOfStaffStatus>(customer.Status), ExcelHorizontalAlignment.Center, true);

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"CUSTOMER{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        [CheckPermission(EnumAction.Export, EnumPage.PotentialCustomer)]
        public async Task<ActionResult> PotentialCustomerExcelReport(PotentialCustomerSearchModal searchModal)
        {
            List<PotentialCustomer> potentialCustomerModal;
            long totalRecord;
            int page = 1;
            int pageSize = Int32.MaxValue;

            var dateStart = new DateTime();
            var dateEnd = new DateTime();

            if (searchModal == null)
            {
                searchModal = new PotentialCustomerSearchModal();
            }

            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);

            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                dateStart = DateTime.Parse(searchModal.DateStart);
                dateEnd = DateTime.Parse(searchModal.DateEnd);

                potentialCustomerModal = await UnitOfWork.PotentialCustomerRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.Email.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && !x.IsDelete
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.GenderId == -1 || x.GenderId == searchModal.GenderId)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                         && (searchModal.CustomerType == -1 || x.CustomerTypeId == searchModal.CustomerType)
                         && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                         && (UserState.Type != 0 || x.UserId == UserState.UserId)
                         && x.Created >= dateStart && x.Created <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            else
            {
                potentialCustomerModal = await UnitOfWork.PotentialCustomerRepo.FindAsync(
                    out totalRecord,
                       x => (x.Code.Contains(searchModal.Keyword) || x.Email.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && !x.IsDelete
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.GenderId == -1 || x.GenderId == searchModal.GenderId)
                         && (UserState.Type != 0 || x.UserId == UserState.UserId)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                         && (searchModal.CustomerType == -1 || x.CustomerTypeId == searchModal.CustomerType)
                         && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + "."))),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Create date", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "LevelName", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Code", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Email", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Full name", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Phone", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Gender", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Province/City", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "District", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Commune", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Address", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Staff", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Of department", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "POTENTIAL CUSTOMER LIST", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 18
                });

                var start = dateStart.ToShortDateString() ?? "__";
                var end = dateEnd.ToShortDateString() ?? "__";

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start} to date {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (potentialCustomerModal.Any())
                {
                    foreach (var w in potentialCustomerModal)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.LevelName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Code, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Email, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.FullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Phone, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.GenderName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.DistrictName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.ProvinceName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.WardsName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Address, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.UserFullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.OfficeName, ExcelHorizontalAlignment.Right, true);

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;


                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"Potential_Customer{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }

        [CheckPermission(EnumAction.Export, EnumPage.PotentialCustomerByStaff)]
        public async Task<ActionResult> PotentialCustomerByUserExcelReport(PotentialCustomerSearchModal searchModal)
        {
            List<PotentialCustomer> potentialCustomerByUserModal;
            long totalRecord;
            int page = 1;
            int pageSize = Int32.MaxValue;

            var dateStart = new DateTime();
            var dateEnd = new DateTime();

            if (searchModal == null)
            {
                searchModal = new PotentialCustomerSearchModal();
            }

            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);

            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                dateStart = DateTime.Parse(searchModal.DateStart);
                dateEnd = DateTime.Parse(searchModal.DateEnd);

                potentialCustomerByUserModal = await UnitOfWork.PotentialCustomerRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && (!x.IsDelete)
                         && (x.UserId == UserState.UserId)
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                         && (searchModal.CustomerType == -1 || x.CustomerTypeId == searchModal.CustomerType)
                         && x.Created >= dateStart && x.Created <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            else
            {
                potentialCustomerByUserModal = await UnitOfWork.PotentialCustomerRepo.FindAsync(
                    out totalRecord,
                       x => (x.Code.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && (!x.IsDelete)
                         && (x.UserId == UserState.UserId)
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.CustomerType == -1 || x.CustomerTypeId == searchModal.CustomerType)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Create date", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "LevelName", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Code", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Email", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Full name", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Phone", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Gender", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Province/City", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "District", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Commune", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Address", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Staff", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Of department", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "POTENTIAL CUSTOMER LIST IN CHARGE", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 18
                });

                var start = dateStart.ToShortDateString() ?? "__";
                var end = dateEnd.ToShortDateString() ?? "__";

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start} to date {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (potentialCustomerByUserModal.Any())
                {
                    foreach (var w in potentialCustomerByUserModal)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.LevelName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Code, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Email, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.FullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Phone, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.GenderName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.DistrictName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.ProvinceName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.WardsName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Address, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.UserFullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.OfficeName, ExcelHorizontalAlignment.Right, true);

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;


                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"Potential_Customer_InCharge{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }

        [CheckPermission(EnumAction.Export, EnumPage.Customer)]
        public async Task<ActionResult> CustomerExcelReport(CustomerSearchModal searchModal)
        {
            List<Customer> customerModal;
            long totalRecord;
            int page = 1;
            int pageSize = Int32.MaxValue;

            var dateStart = new DateTime();
            var dateEnd = new DateTime();

            if (searchModal == null)
            {
                searchModal = new CustomerSearchModal();
            }

            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);

            //Check user có phải là nhân viên kinh doanh công ty ko?
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeTypeCompany(OfficeType.Business);
            var checkUser = listUser.FirstOrDefault(x => x.Id == UserState.UserId);
            var check = checkUser != null;

            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                dateStart = DateTime.Parse(searchModal.DateStart);
                dateEnd = DateTime.Parse(searchModal.DateEnd);

                customerModal = await UnitOfWork.CustomerRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.Email.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && !x.IsDelete
                         && (searchModal.GenderId == -1 || x.SystemId == searchModal.GenderId)
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                         && (UserState.Type != 0 || x.UserId == UserState.UserId)
                         && (searchModal.WarehouseId == -1 || x.WarehouseId == searchModal.WarehouseId)
                         && (searchModal.WarehouseCustomer == -1 || (searchModal.WarehouseCustomer == 0 && x.WarehouseId == null) || (searchModal.WarehouseCustomer == 1 && x.WarehouseId != null))
                         && (check == false || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".") || x.UserId == null)))
                         && (check == true || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + "."))))
                         && x.Created >= dateStart && x.Created <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            else
            {
                customerModal = await UnitOfWork.CustomerRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.Email.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && !x.IsDelete
                         && (searchModal.GenderId == -1 || x.SystemId == searchModal.GenderId)
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                         && (searchModal.WarehouseId == -1 || x.WarehouseId == searchModal.WarehouseId)
                         && (searchModal.WarehouseCustomer == -1 || (searchModal.WarehouseCustomer == 0 && x.WarehouseId == null) || (searchModal.WarehouseCustomer == 1 && x.WarehouseId != null))
                         && (UserState.Type != 0 || x.UserId == UserState.UserId)
                         && (check == false || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".") || x.UserId == null)))
                         && (check == true || (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Create date", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Customer warehouse", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "LevelName", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Code", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Email", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Full name", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Phone", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Gender", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Province/City", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "District", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Commune", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Address", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Staff", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Of department", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "OFFICIAL CUSTOMER LIST", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 18
                });

                var start = dateStart.ToShortDateString() ?? "__";
                var end = dateEnd.ToShortDateString() ?? "__";

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start} to date {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (customerModal.Any())
                {
                    foreach (var w in customerModal)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.WarehouseName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.LevelName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Code, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Email, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.FullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Phone, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.GenderName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.DistrictName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.ProvinceName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.WardsName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Address, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.UserFullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.OfficeName, ExcelHorizontalAlignment.Right, true);

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;


                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"Official_Customer{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }

        [CheckPermission(EnumAction.Export, EnumPage.CustomerbyStaff)]
        public async Task<ActionResult> CustomerByUserExcelReport(CustomerSearchModal searchModal)
        {
            List<Customer> customerByStaffModal;
            long totalRecord;
            int page = 1;
            int pageSize = Int32.MaxValue;

            var dateStart = new DateTime();
            var dateEnd = new DateTime();

            if (searchModal == null)
            {
                searchModal = new CustomerSearchModal();
            }

            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);

            //Check user có phải là nhân viên kinh doanh công ty ko?
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeTypeCompany(OfficeType.Business);
            var checkUser = listUser.FirstOrDefault(x => x.Id == UserState.UserId);
            var check = checkUser != null;

            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                dateStart = DateTime.Parse(searchModal.DateStart);
                dateEnd = DateTime.Parse(searchModal.DateEnd);

                customerByStaffModal = await UnitOfWork.CustomerRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.Email.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && !x.IsDelete
                         && (x.UserId == UserState.UserId)
                         && (searchModal.GenderId == -1 || x.SystemId == searchModal.GenderId)
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                         && (searchModal.WarehouseId == -1 || x.WarehouseId == searchModal.WarehouseId)
                        && (searchModal.WarehouseCustomer == -1 || (searchModal.WarehouseCustomer == 0 && x.WarehouseId == null) || (searchModal.WarehouseCustomer == 1 && x.WarehouseId != null))
                         && x.Created >= dateStart && x.Created <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            else
            {
                customerByStaffModal = await UnitOfWork.CustomerRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.Email.Contains(searchModal.Keyword) || x.FullName.Contains(searchModal.Keyword) || x.Nickname.Contains(searchModal.Keyword) || x.Phone.Contains(searchModal.Keyword))
                         && !x.IsDelete
                         && (x.UserId == UserState.UserId)
                         && (searchModal.GenderId == -1 || x.SystemId == searchModal.GenderId)
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.WarehouseId == -1 || x.WarehouseId == searchModal.WarehouseId)
                         && (searchModal.WarehouseCustomer == -1 || (searchModal.WarehouseCustomer == 0 && x.WarehouseId == null) || (searchModal.WarehouseCustomer == 1 && x.WarehouseId != null))
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Create date", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Customer warehouse", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "LevelName", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Code", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Email", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Full name", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Phone", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Gender", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Province/City", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "District", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Commune", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Address", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Staff", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Of department", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "OFFICIAL CUSTOMER LIST I AM IN CHARGE OF", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 18
                });

                var start = dateStart.ToShortDateString() ?? "__";
                var end = dateEnd.ToShortDateString() ?? "__";

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start} to date {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (customerByStaffModal.Any())
                {
                    foreach (var w in customerByStaffModal)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.WarehouseName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.LevelName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Code, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Email, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.FullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Phone, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.GenderName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.DistrictName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.ProvinceName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.WardsName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Address, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.UserFullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.OfficeName, ExcelHorizontalAlignment.Right, true);

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;


                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"Official_Customer_InCharge{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }


        #endregion [Xuất excel]

        #region [Thống kê khách hàng]

        //1. Lây danh sach khách hàng chính thức được tạo trong ngày
        [HttpPost]
        public async Task<JsonResult> GetCustomerReport(DateTime? startDay)
        {
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            var overview = UnitOfWork.CustomerRepo.GetCustomerReport(listUser, startDay);


            //1. Tạo các dữ liệu theo báo cáo
            var detailName = new List<string>();
            var detailUser = new List<int>();

            foreach (var ticket in overview)
            {

                detailName.Add(ticket.FullName);
                detailUser.Add(ticket.TotalCusstomer);
            }

            //2. Trả kết quả lên view
            return Json(new { overview, detailName, detailUser }, JsonRequestBehavior.AllowGet);
        }

        //1. Lây danh sach khách hàng tiềm năng được tạo trong ngày
        [HttpPost]
        public async Task<JsonResult> GetPotentialCustomerReport(DateTime? startDay)
        {

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            var overview = UnitOfWork.CustomerRepo.GetPotentialCustomerReport(listUser, startDay);


            //3. Tạo các dữ liệu theo báo cáo
            var detailName = new List<string>();
            var detailUser = new List<int>();

            foreach (var ticket in overview)
            {

                detailName.Add(ticket.FullName);
                detailUser.Add(ticket.TotalCusstomer);
            }
            //4. Trả kết quả lên view
            return Json(new { overview, detailName, detailUser }, JsonRequestBehavior.AllowGet);
        }

        //1. Lây danh sach tất cả khách hàng chính thức đang quản lý theo nhân viên
        [HttpPost]
        public async Task<JsonResult> GetCustomerOffStaffReport(DateTime? startDay, DateTime? endDay)
        {

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            var overview = UnitOfWork.CustomerRepo.GetCustomerOffStaffReport(listUser, startDay, endDay);


            //3. Tạo các dữ liệu theo báo cáo
            var detailName = new List<string>();
            var detailUser = new List<int>();

            foreach (var ticket in overview)
            {

                detailName.Add(ticket.FullName);
                detailUser.Add(ticket.TotalCusstomer);
            }
            //4. Trả kết quả lên view
            return Json(new { overview, detailName, detailUser }, JsonRequestBehavior.AllowGet);
        }

        //1. Lây danh sach tất cả khách hàng tiềm năng được tạo theo nhân viên
        [HttpPost]
        public async Task<JsonResult> GetPotentialCustomerOffStaffReport(DateTime? startDay, DateTime? endDay)
        {

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            var overview = UnitOfWork.CustomerRepo.GetPotentialCustomerOffStaffReport(listUser, startDay, endDay);


            //3. Tạo các dữ liệu theo báo cáo
            var detailName = new List<string>();
            var detailUser = new List<int>();

            foreach (var ticket in overview)
            {

                detailName.Add(ticket.FullName);
                detailUser.Add(ticket.TotalCusstomer);
            }
            //4. Trả kết quả lên view
            return Json(new { overview, detailName, detailUser }, JsonRequestBehavior.AllowGet);
        }
        //Xuất Excel danh sách khách hàng tiềm năng theo nhân viên trong ngày
        [HttpPost]
        public async Task<FileContentResult> ExcelPotentialCustomerReport(string titleExcel, DateTime? startDay, bool all)
        {

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            var listUserExcel = UnitOfWork.CustomerRepo.GetPotentialCustomerReport(listUser, startDay);
            return CommonCustomerReport(titleExcel, startDay, null, all, listUserExcel);
        }
        //Xuất Excel danh sách khách hàng chính thức theo nhân viên trong ngày
        [HttpPost]
        public async Task<FileContentResult> ExcelGetCustomerReport(string titleExcel, DateTime? startDay, bool all)
        {

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            var listUserExcel = UnitOfWork.CustomerRepo.GetCustomerReport(listUser, startDay);
            return CommonCustomerReport(titleExcel, startDay, null, all, listUserExcel);
        }

        //Xuất Excel danh sách tất cả khách hàng tiềm năng theo nhân viên
        [HttpPost]
        public async Task<FileContentResult> ExcelGetPotentialCustomerOffStaffReport(string titleExcel, DateTime? startDay, DateTime? endDay, bool all)
        {

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            var listUserExcel = UnitOfWork.CustomerRepo.GetPotentialCustomerOffStaffReport(listUser, startDay, endDay);
            return CommonCustomerReport(titleExcel, startDay, endDay, all, listUserExcel); ;
        }

        //Xuất Excel danh sách tất cả khách hàng chính thức theo nhân viên
        [HttpPost]
        public async Task<FileContentResult> ExcelGetCustomerOffStaffReport(string titleExcel, DateTime? startDay, DateTime? endDay, bool all)
        {

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            var listUserExcel = UnitOfWork.CustomerRepo.GetCustomerOffStaffReport(listUser, startDay, endDay);

            return CommonCustomerReport(titleExcel, startDay, endDay, all, listUserExcel);
        }

        public FileContentResult CommonCustomerReport(string titleExcel, DateTime? startDay, DateTime? endDay, bool all, List<CustomerUser> listUserExcel)
        {
            var ngay = "";
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Full name", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Username", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Gender", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Date of birth", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Email", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "PHONE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Start working date", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Account creation date", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Number of customers", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, titleExcel, new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                var start = startDay?.ToShortDateString() ?? "__";
                var end = endDay?.ToShortDateString() ?? "__";

                // biến all=true : lấy tất cả, all = false: Lấy theo ngày
                if (all == true)
                {
                    ngay = "From: " + start + " to date " + end;
                }
                else
                {
                    ngay = DateTime.Now.ToLongDateString();

                }
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"{ngay}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (listUserExcel.Any())
                {
                    foreach (var w in listUserExcel)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.FullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.TypeName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<SexCustomer>(w.Gender), ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Birthday, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Email, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Phone, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.StartDate?.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Right, true);
                        col++;

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.TotalCusstomer, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;


                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"CUSTOMER_STAFF{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }
        #endregion

        #region [Thống kê tình hình khách hàng theo thời gian]
        [HttpPost]
        public async Task<JsonResult> GetCustomerSituation(DateTime? startDay, DateTime? endDay)
        {
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);
            var customer = UnitOfWork.CustomerRepo.GetCustomerSituationOnTime(listUser, start, end, UserState);

            //1. Tạo các dữ liệu theo báo cáo
            var totalOrder = new List<int>();
            var day = new List<string>();
            foreach (var or in customer)
            {
                day.Add(or.Created);
                totalOrder.Add(or.TotalOrder);
            }
            //2. Trả kết quả lên view
            return Json(new { day, totalOrder }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        public async Task<JsonResult> GetAllOrderCustomerList(int page, int pageSize, CustomerSearchModal searchModal, int orderType)
        {
            long totalRecord;
            dynamic total;
            dynamic totalPriceBargain;
            dynamic totalServiceOrder;
            dynamic totalWeight;

            if (searchModal == null)
            {
                searchModal = new CustomerSearchModal();
            }
            var customerByStaffModal = new List<OrderCustomer>();
            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            if (searchModal.DateStart == null)
            {
                customerByStaffModal = await UnitOfWork.CustomerRepo.GetOrderCustomer(out totalRecord, out total, out totalPriceBargain, out totalServiceOrder, out totalWeight, listUser, searchModal.Status, searchModal.Keyword, searchModal.UserId, null, null, orderType, page, pageSize, UserState);
            }
            else
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);
                customerByStaffModal = await UnitOfWork.CustomerRepo.GetOrderCustomer(out totalRecord, out total, out totalPriceBargain, out totalServiceOrder, out totalWeight, listUser, searchModal.Status, searchModal.Keyword, searchModal.UserId, dateStart, dateEnd, orderType, page, pageSize, UserState);
            }

            return Json(new { totalRecord, customerByStaffModal, total, totalPriceBargain, totalServiceOrder, totalWeight }, JsonRequestBehavior.AllowGet);
        }

        //Xuất excel thông kê doanh số nhân viên
        [HttpPost]
        public async Task<ActionResult> RevenueExcelReport(RevenueReportSearchModel searchModal)
        {
            var listId = new List<int>();
            var list = new List<ReportBusinessItem>();
            ReportBusinessModel listRevenueReport = new ReportBusinessModel();
            if (searchModal == null)
            {
                searchModal = new RevenueReportSearchModel();
            }
            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();
            DateTime? start = null;
            DateTime? end = null;
            if (searchModal.DateStart != null)
            {
                start = GetStartOfDay(DateTime.Parse(searchModal.DateStart ?? DateTime.Now.ToShortDateString()));
            }
            if (searchModal.DateEnd != null)
            {
                end = GetEndOfDay(DateTime.Parse(searchModal.DateEnd ?? DateTime.Now.ToShortDateString()));
            }
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            if (searchModal.UserId == -1)
            {
                listId = listUser.Where(x => UserState.Type > 0 || x.Id == UserState.UserId).Select(x => x.Id).ToList();
            }
            else
            {
                listId = listUser.Where(x => x.Id == searchModal.UserId).Select(x => x.Id).ToList();
            }
            list = await UnitOfWork.CustomerRepo.ExcelGetOrderUserList(searchModal.Keyword, searchModal.UserId, searchModal.CustomerStatus, start, end, listId);

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");
                var style = new CustomExcelStyle
                {
                    IsMerge = false,
                    IsBold = false,
                    Border = ExcelBorderStyle.Thin,
                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                    NumberFormat = "#,##0"
                };

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "SALES STAFF", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER SETTLING DATE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TOTAL (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDERING SERVICE FEE (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "BARGAIN (CNY)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TOTAL (VNĐ)", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "REVENUE BY STAFF", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });
                var start1 = searchModal.DateStart == null ? "__" : DateTime.Parse(searchModal.DateStart).ToShortDateString();
                var end1 = searchModal.DateEnd == null ? "__" : DateTime.Parse(searchModal.DateEnd).ToShortDateString();
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start1} to date {end1}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (list.Any())
                {
                    foreach (var ticket in list)
                    {

                        col = 1;
                        //STT
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        //Ma don hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.OrderCode, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //nhân viên
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.UserFullName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Khach hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.CustomerName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //thoi gian
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.OrderFinishDate.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        col++;
                        //tiền hàng
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.OrderTotalExchange, style);
                        col++;
                        //phí dịch vụ mua hàng
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.ServicePurchase, style);
                        col++;
                        //tiền mặc cả
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.OrderBargain * ticket.ExchangeRate, style);
                        col++;
                        //tổng tiền đơn
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.OrderTotal, style);

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"ORDERLIST{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        [HttpPost]
        public async Task<ActionResult> RevenueExcelDepositReport(RevenueReportSearchModel searchModal)
        {
            var listId = new List<int>();
            var list = new List<ReportBusinessItem>();
            ReportBusinessModel listRevenueReport = new ReportBusinessModel();
            if (searchModal == null)
            {
                searchModal = new RevenueReportSearchModel();
            }
            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();
            DateTime? start = null;
            DateTime? end = null;
            if (searchModal.DateStart != null)
            {
                start = GetStartOfDay(DateTime.Parse(searchModal.DateStart ?? DateTime.Now.ToShortDateString()));
            }
            if (searchModal.DateEnd != null)
            {
                end = GetEndOfDay(DateTime.Parse(searchModal.DateEnd ?? DateTime.Now.ToShortDateString()));
            }
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Deposit, UserState);
            if (searchModal.UserId == -1)
            {
                listId = listUser.Where(x => UserState.Type > 0 || x.Id == UserState.UserId).Select(x => x.Id).ToList();
            }
            else
            {
                listId = listUser.Where(x => x.Id == searchModal.UserId).Select(x => x.Id).ToList();
            }
            list = await UnitOfWork.CustomerRepo.ExcelGetOrderUserDepositList(searchModal.Keyword, searchModal.UserId, searchModal.CustomerStatus, start, end, listId);

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");


                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "COMPLETED DATE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "SALES STAFF", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "WEIGHT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "FEE BY WEIGHT (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "INSURANCE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TOTAL (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "REVENUE BY STAFF", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });
                var start1 = searchModal.DateStart == null ? "__" : DateTime.Parse(searchModal.DateStart).ToShortDateString();
                var end1 = searchModal.DateEnd == null ? "__" : DateTime.Parse(searchModal.DateEnd).ToShortDateString();
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start1} to date {end1}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (list.Any())
                {
                    foreach (var ticket in list)
                    {

                        col = 1;
                        //STT
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.OrderFinishDate.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Ma don hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.OrderCode, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //nhân viên
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.UserFullName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Khach hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.CustomerName, ExcelHorizontalAlignment.Left, true);

                        col++;
                        //Cân nặng
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.OrderTotalWeight, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            //NumberFormat = "#,##0"
                        });
                        col++;
                        //Phí cân nặng
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.ServiceShip, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            //NumberFormat = "#,##0"
                        });
                        col++;
                        //Bảo hiểm
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, "", new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            //NumberFormat = "#,##0"
                        });
                        col++;
                        //tổng tiền đơn
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.OrderTotal, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            //NumberFormat = "#,##0"
                        });
                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"ORDERLIST{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        //Xuất excel theo dõi đơn hàng của nhân viên
        [HttpPost]
        public async Task<ActionResult> OrdercustomerExcelReport(CustomerSearchModal searchModal, int orderType)
        {
            if (searchModal == null)
            {
                searchModal = new CustomerSearchModal();
            }
            var customerByStaffModal = new List<OrderCustomer>();
            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Business, UserState);
            if (searchModal.DateStart == null)
            {
                customerByStaffModal = await UnitOfWork.CustomerRepo.ExcelGetOrderCustomer(listUser, searchModal.Status, searchModal.Keyword, searchModal.UserId, null, null, orderType, UserState);
            }
            else
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);
                customerByStaffModal = await UnitOfWork.CustomerRepo.ExcelGetOrderCustomer(listUser, searchModal.Status, searchModal.Keyword, searchModal.UserId, dateStart, dateEnd, orderType, UserState);
            }
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");


                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CREATION DATE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "SALES STAFF", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TOTAL (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDERING SERVICE FEE (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "BARGAIN (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "REVENUE BY STAFF", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });
                var start1 = searchModal.DateStart == null ? "__" : DateTime.Parse(searchModal.DateStart).ToShortDateString();
                var end1 = searchModal.DateEnd == null ? "__" : DateTime.Parse(searchModal.DateEnd).ToShortDateString();
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start1} to date {end1}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (customerByStaffModal.Any())
                {
                    foreach (var ticket in customerByStaffModal)
                    {
                        col = 1;
                        //STT
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        //Ma don hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.Code, ExcelHorizontalAlignment.Left, true);
                        col++;

                        //thoi gian
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        col++;
                        //nhân viên
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.CustomerUserName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Khach hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.CustomerName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //tổng tiền đơn
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.Total, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        //phí dịch vụ mua hàng
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.ServiceOrder, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        //tiền mặc cả
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.PriceBargain, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        //Trang thai
                        if (ticket.Type == (byte)OrderType.Order)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<OrderStatus>((int)ticket.Status), ExcelHorizontalAlignment.Center, true);
                        }

                        if (ticket.Type == (byte)OrderType.Deposit)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<DepositStatus>((int)ticket.Status), ExcelHorizontalAlignment.Center, true);
                        }

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"TICKETLIST{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        //public async Task<ActionResult> Fix()
        //{
        //    long totalRecord;
        //    var recordPerPage = 100;
        //    var currentPage = 1;

        //    var customers = await UnitOfWork.CustomerRepo.FindAsNoTrackingAsync(out totalRecord, null,
        //        x => x.OrderByDescending(y => y.Id), currentPage, recordPerPage);

        //    var totalPage = Math.Ceiling((decimal)totalRecord / recordPerPage);

        //    for (currentPage = 1; currentPage <= totalPage; currentPage++)
        //    {
        //        if (currentPage != 1)
        //        {
        //            customers = await UnitOfWork.CustomerRepo.FindAsNoTrackingAsync(out totalRecord, null, 
        //                x => x.OrderByDescending(y => y.Id),
        //                currentPage, recordPerPage);
        //        }

        //        var orderIds = $";{string.Join(";", customers.Select(x => x.Id).ToList())};";

        //        BackgroundJob.Enqueue(() => CustomerJob.UpdateCustomer(orderIds));
        //    }

        //    return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
        //}
    }
}