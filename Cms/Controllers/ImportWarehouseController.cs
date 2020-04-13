using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Cms.Attributes;
using Common.ActionResult;
using Common.Emums;
using Common.FunctionResult;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Hangfire;
using Cms.Jobs;

namespace Cms.Controllers
{
    [Authorize]
    public class ImportWarehouseController : BaseController
    {
        private async Task InitValue()
        {
            var isManager = UserState.Type.HasValue && ((UserState.Type.Value == 2) || (UserState.Type.Value == 1));

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (isManager)
            {
                var warehouses = await UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                    x =>
                        ((isManager &&
                          ((x.IdPath == UserState.OfficeIdPath) || x.IdPath.StartsWith(UserState.OfficeIdPath + "."))) ||
                         (!isManager && (x.IdPath == UserState.OfficeIdPath))) &&
                        (x.Type == (byte) OfficeType.Warehouse) &&
                        !x.IsDelete && (x.Status == (byte) OfficeStatus.Use));

                ViewBag.Warehouses = JsonConvert.SerializeObject(warehouses.Select(Mapper.Map<OfficeDropdownResult>),
                    jsonSerializerSettings);
            }

            ViewBag.StatesSelectListItem = Enum.GetValues(typeof(ImportWarehouseStatus))
                .Cast<ImportWarehouseStatus>()
                .Select(
                    v =>
                        new SelectListItem()
                        {
                            Value = ((byte) v).ToString(),
                            Text = EnumHelper.GetEnumDescription<ImportWarehouseStatus>((int) v),
                            Selected = v == ImportWarehouseStatus.Approved
                        })
                .ToList();

            ViewBag.States = JsonConvert.SerializeObject(Enum.GetValues(typeof(ImportWarehouseStatus))
                .Cast<ImportWarehouseStatus>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<ImportWarehouseStatus>((int) v)})
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderPackageStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int) v)})
                .ToList(), jsonSerializerSettings);

            var allWarehouse = await
                UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                    x => !x.IsDelete && (x.Type == (byte) OfficeType.Warehouse) && (x.Status == (byte) OfficeStatus.Use));

            ViewBag.AllWarehouses =
                JsonConvert.SerializeObject(allWarehouse.Select(x => new {x.Id, x.Name, x.IdPath, x.Address}).ToList(),
                    jsonSerializerSettings);

            ViewBag.WalletStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(WalletStatus))
                .Cast<WalletStatus>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<WalletStatus>((int) v)})
                .ToList(), jsonSerializerSettings);

            ViewBag.MaxFileLength = GetMaxFileLength();

            ViewBag.OrderType = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .ToDictionary(v => (byte) v, v => EnumHelper.GetEnumDescription<OrderType>((int) v)),
                jsonSerializerSettings);
        }

        // GET: ImportWarehouse
        [LogTracker(EnumAction.View, EnumPage.ImportWarehouse)]
        public async Task<ActionResult> Index()
        {
            await InitValue();

            // PHiếu nhập kho kiện
            ViewBag.ViewMode = 0;

            return View();
        }

        [LogTracker(EnumAction.View, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> Wallet()
        {
            await InitValue();

            // PHiếu nhập kho bao hàng
            ViewBag.ViewMode = 1;

            return View("Index");
        }

        [CheckPermission(EnumAction.View, EnumPage.ImportWarehouse, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> Search(string warehouseIdPath, int? userId, byte? status, DateTime? fromDate,
            DateTime? toDate, string keyword = "", byte viewMode = 0, int currentPage = 1, int recordPerPage = 20)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            // ReSharper disable once PossibleInvalidOperationException
            var isManager = (UserState.Type.Value == 2) || (UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            long totalRecord;

            Expression<Func<ImportWarehouse, bool>> queryI = x => ((status == null) || (x.Status == status.Value)) &&
                                                                  ((userId == null) || (x.UserId == userId.Value)) &&
                                                                  !x.IsDelete &&
                                                                  ((isManager &&
                                                                    ((x.WarehouseIdPath == warehouseIdPath) ||
                                                                     x.WarehouseIdPath.StartsWith(warehouseIdPath + "."))) ||
                                                                   (!isManager &&
                                                                    (x.WarehouseIdPath == warehouseIdPath))) &&
                                                                  (((fromDate == null) && (toDate == null)) ||
                                                                   ((fromDate != null) && (toDate != null) &&
                                                                    (x.Created >= fromDate) && (x.Created <= toDate)) ||
                                                                   ((fromDate == null) && toDate.HasValue &&
                                                                    (x.Created <= toDate)) ||
                                                                   ((toDate == null) && fromDate.HasValue &&
                                                                    (x.Created >= fromDate)));

            if (viewMode == 0)
            {
                Expression<Func<ImportWarehouseResult, bool>> queryR =
                    x => x.UnsignedText.Contains(keyword) || x.PackageUnsignedText.Contains(keyword);

                var packages = await UnitOfWork.ImportWarehouseRepo.Search(queryI, queryR, currentPage, recordPerPage,
                    out totalRecord);

                return JsonCamelCaseResult(new {items = packages, totalRecord}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Expression<Func<ImportWarehouseWalletResult, bool>> queryR =
                    x => x.UnsignedText.Contains(keyword) || x.WalletUnsignedText.Contains(keyword);

                var packages = await UnitOfWork.ImportWarehouseRepo.Search(queryI, queryR, currentPage, recordPerPage,
                    out totalRecord);

                return JsonCamelCaseResult(new {items = packages, totalRecord}, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckPermission(EnumAction.View, EnumPage.ImportWarehouse, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> GetDetail(int id, byte viewMode = 0)
        {
            var importWarehouse =
                await UnitOfWork.ImportWarehouseRepo.SingleOrDefaultAsNoTrackingAsync(x => x.Id == id && !x.IsDelete);

            if (importWarehouse == null)
                return JsonCamelCaseResult(new {Status = -1, Text = "Goods received note does not exist or has been deleted"},
                    JsonRequestBehavior.AllowGet);


            if (viewMode == 0)
            {
                var items = await UnitOfWork.ImportWarehouseDetailRepo.GetByImportWarehouseId(importWarehouse.Id);

                var packageIds = $";{string.Join(";", items.Select(x => x.PackageId).ToList())};";

                var packageNotesData =
                    await UnitOfWork.PackageNoteRepo.FindAsNoTrackingAsync(
                        x => x.PackageId != null && packageIds.Contains(";" + x.PackageId + ";"));

                var packageNotes = packageNotesData.GroupBy(x => x.PackageId)
                    .ToDictionary(x => x.Key, x => x.OrderBy(o => o.Time).ToList());

                var packageNoteMode = Enum.GetValues(typeof(PackageNoteMode))
                    .Cast<PackageNoteMode>()
                    .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<PackageNoteMode>((int)v));

                foreach (var orderPackage in items)
                {
                    orderPackage.PackageNotes = packageNotes.ContainsKey(orderPackage.PackageId)
                        ? packageNotes[orderPackage.PackageId]
                        : null;
                    orderPackage.PackageNoteMode = packageNoteMode;
                }

                return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var items = await UnitOfWork.ImportWarehouseDetailRepo.SearchWallet(id);

                return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckPermission(EnumAction.Add, EnumPage.ImportWarehouse)]
        public async Task<ActionResult> GetToAdd()
        {
            var timeNow = DateTime.Now;
            //1. Tạo phiếu nhập kho
            var importWarehouse = new ImportWarehouse
            {
                Updated = timeNow,
                Created = timeNow,
                Status = 1,
                UserId = UserState.UserId,
                UserFullName = UserState.FullName,
                UserName = UserState.UserName,
                WarehouseId = UserState.OfficeId ?? 0,
                WarehouseName = UserState.OfficeName,
                WarehouseIdPath = UserState.OfficeIdPath,
                WarehouseAddress = UserState.OfficeAddress,
                Note = null,
                ShipperPhone = null,
                ShipperName = null,
                ShipperAddress = null,
                ShipperEmail = null,
                Code = string.Empty,
                UnsignedText = string.Empty
            };

            UnitOfWork.ImportWarehouseRepo.Add(importWarehouse);
            await UnitOfWork.ImportWarehouseRepo.SaveAsync();

            //2. Cập nhật lại Mã Code cho phiếu nhập kho
            var importWarehouseOfDay = UnitOfWork.ImportWarehouseRepo.Count(x =>
                (x.Created.Year == timeNow.Year) && (x.Created.Month == timeNow.Month) &&
                (x.Created.Day == timeNow.Day) && (x.Id <= importWarehouse.Id));

            importWarehouse.Code = $"{importWarehouseOfDay}{timeNow:ddMMyy}";
            importWarehouse.UnsignedText =
                MyCommon.Ucs2Convert(
                    $"{importWarehouse.Code} {importWarehouse.UserFullName} {importWarehouse.UserName} {importWarehouse.WarehouseName}");

            await UnitOfWork.ImportWarehouseRepo.SaveAsync();

            return JsonCamelCaseResult(importWarehouse, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.ImportWarehouse, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> AddPackage(int packageId, string note, int importWarehouseId, string packageCode, string importWarehouseCode)
        {
            if (UserState.OfficeType != 1)
                return JsonCamelCaseResult(
                    new { Status = -2, Text = "Only warehouse staff can perform this action" },
                    JsonRequestBehavior.AllowGet);

            var package = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(
                                x => x.Id == packageId && x.IsDelete == false &&
                                     (x.Status == (byte)OrderPackageStatus.ShopDelivery));

            if (package == null)
                return JsonCamelCaseResult(new {Status = -1, Text = $"package: P{packageCode} does not exist or has been deleted"},
                        JsonRequestBehavior.AllowGet);


            if (package.Status != (byte)OrderPackageStatus.ShopDelivery)
                return JsonCamelCaseResult(new { Status = -2, Text = $"package: P{packageCode} has been put in stock" },
                        JsonRequestBehavior.AllowGet);

            var importWarehouse =
                await UnitOfWork.ImportWarehouseRepo.SingleOrDefaultAsync(
                    x => x.IsDelete == false && x.Id == importWarehouseId);

            if(importWarehouse == null)
                return JsonCamelCaseResult(new { Status = -3, Text = $"Goods received note: I{importWarehouseCode} does not exist or has been deleted" },
                        JsonRequestBehavior.AllowGet);

            var rs = await AddPackage(package, importWarehouse);

            // Cập nhật note nhập kho
            if (rs.Status > 0)
            {
                //// Cập nhật thông tin kho hiện tại cho package
                //package.Note = note;

                var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                        x => x.PackageId == package.Id && x.ObjectId == importWarehouse.Id &&
                            x.Mode == (byte)PackageNoteMode.ChinaImport);

                if (packageNote == null && !string.IsNullOrWhiteSpace(note))
                {
                    UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                    {
                        OrderId = package.OrderId,
                        OrderCode = package.OrderCode,
                        PackageId = package.Id,
                        PackageCode = package.Code,
                        UserId = importWarehouse.UserId,
                        UserFullName = importWarehouse.UserFullName,
                        Time = DateTime.Now,
                        ObjectId = importWarehouse.Id,
                        ObjectCode = importWarehouse.Code,
                        Mode = (byte)PackageNoteMode.ChinaImport,
                        Content = note
                    });
                }
                else if (packageNote != null && !string.IsNullOrWhiteSpace(note))
                {
                    packageNote.Content = note;
                }
                else if (packageNote != null && string.IsNullOrWhiteSpace(note))
                {
                    UnitOfWork.PackageNoteRepo.Remove(packageNote);
                }

                await UnitOfWork.ImportWarehouseDetailRepo.SaveAsync();
            }

            return JsonCamelCaseResult(new {rs.Status, rs.Text},
                JsonRequestBehavior.AllowGet);
        }

        private async Task<FunctionResult> AddPackage(OrderPackage p, ImportWarehouse importWarehouse)
        {
            var timeNow = DateTime.Now;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (p == null)
                        return new FunctionResult() { Status = -1, Text = "The package has been put in stock, does not exist or has been deleted" };

                    // Thêm lịch sử cho package
                    var packageHistory = new PackageHistory()
                    {
                        PackageId = p.Id,
                        PackageCode = p.Code,
                        OrderId = p.OrderId,
                        OrderCode = p.OrderCode,
                        Type = p.OrderType,
                        Status = (byte)OrderPackageStatus.ChinaReceived,
                        Content = $"[{UserState.OfficeName}] {EnumHelper.GetEnumDescription(OrderPackageStatus.ChinaReceived)}",
                        CustomerId = p.CustomerId,
                        CustomerName = p.CustomerName,
                        UserId = UserState.UserId,
                        UserName = UserState.UserName,
                        UserFullName = UserState.FullName,
                        CreateDate = DateTime.Now,
                    };

                    UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                    await UnitOfWork.PackageHistoryRepo.SaveAsync();

                    // Lấy ra tất cả các package trùng mã vận đơn
                    //var packageSameTransportCode = await UnitOfWork.OrderPackageRepo.FindAsync(
                    //    x =>
                    //        x.TransportCode == p.TransportCode && !x.IsDelete &&
                    //        x.Status != (byte)OrderPackageStatus.Completed);

                    //// Có package trùng mã vận đơn
                    //if (packageSameTransportCode.Any(x => x.Id != p.Id))
                    //{
                    //    // Một Orders có nhiều kiện trùng mã vận đơn
                    //    if (packageSameTransportCode.Count(x => x.OrderId == p.OrderId) ==
                    //        packageSameTransportCode.Count())
                    //    {
                    //        var packagesCode = string.Join(", ",
                    //            packageSameTransportCode.Select(x => "P" + x.Code).ToList());
                    //        var customers = string.Join(", ",
                    //            packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList());

                    //        foreach (var ps in packageSameTransportCode)
                    //        {
                    //            // Add cảnh báo cho package
                    //            ps.PackageCodes = packagesCode;
                    //            ps.PackageCodesUnsigned =
                    //                $";{string.Join(";", packageSameTransportCode.Select(x => x.Code).ToList())};";
                    //            ps.Customers = $"{customers}";
                    //            ps.CustomersUnsigned =
                    //                $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";
                    //        }

                    //        p.PackageCodes = packagesCode;
                    //        p.PackageCodesUnsigned =
                    //            $";{string.Join(";", packageSameTransportCode.Select(x => x.Code).ToList())};";
                    //        p.Customers = $"{customers}";
                    //        p.CustomersUnsigned =
                    //            $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";
                    //    }
                    //    else if (packageSameTransportCode.Any(x => x.OrderId != p.OrderId))
                    //    // Có Orders trùng mã vận đơn
                    //    {
                    //        var orderCode = string.Join(", ",
                    //            packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList());
                    //        var customers = string.Join(", ",
                    //            packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList());

                    //        // Kiểm tra khác kho đích
                    //        //var diffWarehouse =
                    //        //    packageSameTransportCode.Any(x => x.CustomerWarehouseId != p.CustomerWarehouseId);

                    //        foreach (var ps in packageSameTransportCode)
                    //        {
                    //            // Add cảnh báo cho package
                    //            ps.OrderCodes = orderCode;
                    //            ps.OrderCodesUnsigned =
                    //                $";{string.Join(";", packageSameTransportCode.Select(x => x.OrderCode).Distinct().ToList())};";
                    //            ps.Customers = $"{customers}";
                    //            ps.CustomersUnsigned =
                    //                $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";
                    //        }

                    //        p.OrderCodes = orderCode;
                    //        p.OrderCodesUnsigned =
                    //            $";{string.Join(";", packageSameTransportCode.Select(x => x.OrderCode).Distinct().ToList())};";
                    //        p.Customers = $"{customers}";
                    //        p.CustomersUnsigned =
                    //            $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";
                    //    }
                    //}

                    // Cập nhật thông tin kho hiện tại cho package
                    p.Status = (byte)OrderPackageStatus.ChinaReceived;
                    p.CurrentWarehouseId = UserState.OfficeId;
                    p.CurrentWarehouseName = UserState.OfficeName;
                    p.CurrentWarehouseIdPath = UserState.OfficeIdPath;
                    p.CurrentWarehouseAddress = UserState.OfficeAddress;
                    p.ForcastDate = null;

                    UnitOfWork.ImportWarehouseDetailRepo.Add(new ImportWarehouseDetail
                    {
                        Created = timeNow,
                        Updated = timeNow,
                        IsDelete = false,
                        Note = string.Empty,
                        PackageId = p.Id,
                        PackageCode = p.Code,
                        OrderCode = p.OrderCode,
                        OrderId = p.OrderId,
                        OrderType = p.OrderType,
                        OrderPackageNo = p.PackageNo,
                        Status = 1,
                        TransportCode = p.TransportCode,
                        ImportWarehouseId = importWarehouse.Id,
                        ImportWarehouseCode = importWarehouse.Code,
                        Type = 0,
                        WarehouseId = p.CustomerWarehouseId,
                        WarehouseIdPath = p.CustomerWarehouseIdPath,
                        WarehouseAddress = p.CustomerWarehouseAddress,
                        WarehouseName = p.CustomerWarehouseName,
                        CustomerId = p.CustomerId,
                        CustomerName = p.CustomerName,
                        CustomerUserName = p.CustomerUserName,
                        OrderServices = p.OrderServices,
                    });

                    importWarehouse.PackageNumber = await UnitOfWork.ImportWarehouseDetailRepo.CountAsync(
                        x => !x.IsDelete && (x.ImportWarehouseId == importWarehouse.Id));

                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    //Thêm thông tin kiện đã nhập kho cho Orders
                    if (p.OrderId > 0)
                    {
                        var o = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == p.OrderId);
                        o.PackageNoInStock = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.OrderId == o.Id);
                        await UnitOfWork.OrderRepo.SaveAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return new FunctionResult() { Status = 1, Text = $"Enter waybill code \"{p.TransportCode}\"" };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.ImportWarehouse)]
        public async Task<ActionResult> UpdatePackage(PackageMeta model)
        {
            // Định dạng dữ liệu
            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(", ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList());

                return JsonCamelCaseResult(new { Status = -1, Text = $"Error: {errorMessage}" },
                    JsonRequestBehavior.AllowGet);
            }

            // Id phiếu does not exist
            if (!model.ImportWarehouseId.HasValue)
                return JsonCamelCaseResult(
                    new { Status = -1, Text = "This goods received note does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            var importWarehouse = await UnitOfWork.ImportWarehouseRepo.SingleOrDefaultAsync(
            x => x.Id == model.ImportWarehouseId.Value && !x.IsDelete);

            if (importWarehouse == null)
                return JsonCamelCaseResult(
                    new { Status = -1, Text = "This goods received note does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            // Không thể Edit phiếu nhập kho quá 3 ngày
            if (importWarehouse.Created < DateTime.Now.AddDays(-2))
            {
                return JsonCamelCaseResult(new { Status = -2, Text = "Cannot update package in 'goods received note' after 2 days" },
                    JsonRequestBehavior.AllowGet);
            }

            var p = await UnitOfWork.ImportWarehouseDetailRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            // package does not exist
            if (p == null)
                return JsonCamelCaseResult(
                    new { Status = -2, Text = "Package does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            // Cập nhật thông tin kho hiện tại cho package
            // p.Note = model.Note;

            var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                    x => x.PackageId == p.PackageId && x.ObjectId == importWarehouse.Id &&
                        x.Mode == (byte) PackageNoteMode.ChinaImport);

            if (packageNote == null && !string.IsNullOrWhiteSpace(model.Note))
            {
                UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                {
                    OrderId = p.OrderId ?? 0,
                    OrderCode = p.OrderCode,
                    PackageId = p.PackageId,
                    PackageCode = p.PackageCode,
                    UserId = importWarehouse.UserId,
                    UserFullName = importWarehouse.UserFullName,
                    Time = DateTime.Now,
                    ObjectId = importWarehouse.Id,
                    ObjectCode = importWarehouse.Code,
                    Mode = (byte)PackageNoteMode.ChinaImport,
                    Content = model.Note
                });
            }else if (packageNote != null && !string.IsNullOrWhiteSpace(model.Note))
            {
                packageNote.Content = model.Note;
            }
            else if (packageNote != null && string.IsNullOrWhiteSpace(model.Note))
            {
                UnitOfWork.PackageNoteRepo.Remove(packageNote);
            }

            await UnitOfWork.ImportWarehouseDetailRepo.SaveAsync();

            var text = model.Type == 0 ? "Goods package" : "Goods sack";

            return JsonCamelCaseResult(new { Status = 1, Text = $"Note updated {text} successfully" },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.ImportWarehouse)]
        public async Task<ActionResult> PackageNoCodeDelete(string transportCode)
        {
            var packages = await UnitOfWork.OrderPackageRepo.FindAsync(
                x => x.TransportCode == transportCode && x.IsDelete == false && x.HashTag == ";packagelose;");

            packages.ForEach(x =>
            {
                x.IsDelete = true;
                x.LastUpdate = DateTime.Now;
            });

            await UnitOfWork.OrderPackageRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = 1, Text = "Information of code missing has been deleted successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.ImportWarehouse, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> AddPackageNoCode(string transportCode)
        {
            var timeNow = DateTime.Now;

            var orderPackage = new OrderPackage()
            {
                Code = string.Empty,
                OrderId = 0,
                OrderCode = string.Empty,
                CustomerId = 0,
                CustomerName = string.Empty,
                CustomerUserName = string.Empty,
                CustomerLevelId = 0,
                CustomerLevelName = string.Empty,
                CustomerWarehouseId = 0,
                CustomerWarehouseName = string.Empty,
                CustomerWarehouseIdPath = string.Empty,
                CustomerWarehouseAddress = string.Empty,
                TransportCode = string.IsNullOrWhiteSpace(transportCode) ? string.Empty : transportCode,
                WarehouseId = UserState.OfficeId ?? 0,
                WarehouseName = UserState.OfficeName,
                WarehouseIdPath = UserState.OfficeIdPath,
                WarehouseAddress = UserState.OfficeAddress,
                UserId = UserState.UserId,
                UserFullName = UserState.FullName,
                SystemId = 0,
                SystemName = string.Empty,
                Created = timeNow,
                LastUpdate = timeNow,
                HashTag = ";packagelose;",
                PackageNo = 0,
                UnsignedText = string.Empty,
                ForcastDate = null,
                OrderType = 0,
                Note = null,
                Status = (byte)OrderPackageStatus.ChinaReceived,
                CurrentWarehouseId = UserState.OfficeId,
                CurrentWarehouseName = UserState.OfficeName,
                CurrentWarehouseIdPath = UserState.OfficeIdPath,
                CurrentWarehouseAddress = UserState.OfficeAddress,
            };

            UnitOfWork.OrderPackageRepo.Add(orderPackage);
            await UnitOfWork.OrderPackageRepo.SaveAsync();

            var orderPackageOfDay = UnitOfWork.OrderPackageRepo.Count(x =>
                (x.Created.Year == timeNow.Year) && (x.Created.Month == timeNow.Month) &&
                (x.Created.Day == timeNow.Day) && (x.Id <= orderPackage.Id));

            orderPackage.Code = $"{orderPackageOfDay}{timeNow:ddMMyy}";

            await UnitOfWork.OrderPackageRepo.SaveAsync();

            return new JsonCamelCaseResult(new {Status = 1, Text = $"Waybill code has lost some pieces of information \"{transportCode}\"", Data = orderPackage },
                JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetOtherService(int importWarehouseId)
        {
            var items = await UnitOfWork.OrderServiceOtherRepo
                .FindAsync(x => x.Type == 0 && x.ObjectId == importWarehouseId);

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.ImportWarehouse, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> RemoveOtherService(int importWarehouseId, string importWarehouseCode, int id)
        {
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var importWarehouse =
                        await UnitOfWork.ImportWarehouseRepo.SingleOrDefaultAsync(
                            x => x.IsDelete == false && x.Id == importWarehouseId);

                    // Goods received note không tồn tại
                    if (importWarehouse == null)
                        return JsonCamelCaseResult(
                                new
                                {
                                    Status = -1,
                                    Text = $"Goods received note I{importWarehouseCode} does not exist or has been deleted"
                                }, JsonRequestBehavior.AllowGet);

                    var serviceOther = await UnitOfWork.OrderServiceOtherRepo.SingleOrDefaultAsync(x => x.Id == id);

                    if(serviceOther == null)
                        return JsonCamelCaseResult(
                                new
                                {
                                    Status = -1,
                                    Text = $"This incurred expense does not exist or has been deleted"
                                }, JsonRequestBehavior.AllowGet);

                    UnitOfWork.OrderServiceOtherRepo.Remove(serviceOther);

                    await UnitOfWork.OrderServiceOtherRepo.SaveAsync();

                    var order =
                        await UnitOfWork.OrderRepo.SingleOrDefaultAsync(
                            x => x.Id == serviceOther.OrderId && x.IsDelete == false);

                    if (order == null)
                        return JsonCamelCaseResult(
                            new
                            {
                                Status = -1,
                                Text = $"Order does not exist or has been deleted"
                            }, JsonRequestBehavior.AllowGet);


                    #region Tính lại tiền dịch vụ phát sinh trong Orders
                    // Thêm tiền dịch vụ khác cho đơn  hàng
                    var timeNow = DateTime.Now;
                    order.LastUpdate = timeNow;
                    // Tính Total money dịch vụ khác
                    var totalServiceOther = await UnitOfWork.OrderServiceOtherRepo.FindAsync(x => x.OrderId == order.Id);

                    var otherService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => !x.IsDelete && x.OrderId == order.Id &&
                                    x.ServiceId == (byte)OrderServices.Other && x.Checked);

                    if (otherService == null)
                    {
                        otherService = new OrderService
                        {
                            IsDelete = false,
                            Checked = true,
                            Created = timeNow,
                            LastUpdate = timeNow,
                            Value = totalServiceOther.Sum(x => x.Value),
                            Currency = Currency.CNY.ToString(),
                            ExchangeRate = order.ExchangeRate,
                            TotalPrice = totalServiceOther.Sum(x => x.TotalPrice),
                            HashTag = string.Empty,
                            Mode = (byte)OrderServiceMode.Option,
                            OrderId = order.Id,
                            ServiceId = (byte)OrderServices.Other,
                            ServiceName = (OrderServices.Other).GetAttributeOfType<DescriptionAttribute>()
                                .Description,
                            Type = (byte)UnitType.Money,
                            Note = $"Incurred service expenses. (Shop charging shipping fees after stock arrival, forklift renting fee...",
                        };

                        UnitOfWork.OrderServiceRepo.Add(otherService);
                    }
                    else
                    {
                        otherService.LastUpdate = timeNow;
                        otherService.Value = totalServiceOther.Sum(x => x.Value);
                        otherService.TotalPrice = totalServiceOther.Sum(x => x.TotalPrice);
                    }

                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                    x.IsDelete == false && x.Checked).ToList().Sum(x => x.TotalPrice);

                    order.Total = order.TotalExchange + totalService;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);
                    #endregion

                    var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                        x => x.PackageId == null && x.OrderId == serviceOther.OrderId && x.ObjectId == serviceOther.Id &&
                             x.Mode == (byte) PackageNoteMode.OtherSerive);

                    UnitOfWork.PackageNoteRepo.Remove(packageNote);

                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(new { Status = 1, Text = "Deleted successfully" },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.ImportWarehouse, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> AddOtherService(OrderServiceOtherMetaV2 model)
        {
            var timeNow = DateTime.Now;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (model.Value <= 0)
                    {
                        return JsonCamelCaseResult(
                                new
                                {
                                    Status = -1,
                                    Text = $"Incurred expenses must be > 0"
                                }, JsonRequestBehavior.AllowGet);
                    }

                    var importWarehouse =
                        await UnitOfWork.ImportWarehouseRepo.SingleOrDefaultAsync(
                            x => x.IsDelete == false && x.Id == model.ImportWarehouseId);

                    // Goods received note không tồn tại
                    if (importWarehouse == null)
                        return JsonCamelCaseResult(
                                new
                                {
                                    Status = -1,
                                    Text = $"Goods received note I{model.ImportWarehouseCode} does not exist or has been deleted"
                                }, JsonRequestBehavior.AllowGet);


                    // Goods received note has not had any order
                    if (!await UnitOfWork.ImportWarehouseDetailRepo.AnyAsync(
                    x => x.IsDelete == false && x.ImportWarehouseId == importWarehouse.Id))
                        return JsonCamelCaseResult(new
                        {
                            Status = -1,
                            Text = $"Cannot add incurred expenses to note I{importWarehouse.Code} has not had any order"
                        }, JsonRequestBehavior.AllowGet);

                    var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Code == model.OrderCode);
                    var packages =
                        await UnitOfWork.ImportWarehouseDetailRepo.FindAsync(
                            x =>
                                x.IsDelete == false && x.ImportWarehouseId == importWarehouse.Id &&
                                x.OrderId == order.Id);

                    var packageCode = packages.Select(x => x.PackageCode).ToList();

                    var transportCode = packages.Select(x => x.TransportCode).ToList();

                    var s = new OrderServiceOther()
                    {
                        Value = model.Value,
                        Created = DateTime.Now,
                        Mode = model.Mode,
                        CreatedOfficeId = UserState.OfficeId ?? 0,
                        CreatedOfficeIdPath = UserState.OfficeIdPath,
                        CreatedOfficeName = UserState.OfficeName,
                        CreatedUserFullName = UserState.FullName,
                        CreatedUserId = UserState.UserId,
                        CreatedUserTitleId = UserState.TitleId ?? 0,
                        CreatedUserTitleName = UserState.TitleName,
                        CreatedUserUserName = UserState.UserName,
                        Currency = Currency.CNY.ToString(),
                        Note = model.Note,
                        ObjectId = importWarehouse.Id,
                        Type = 0,
                        ExchangeRate = order.ExchangeRate,
                        TotalPrice = order.ExchangeRate * model.Value,
                        OrderCode = order.Code,
                        OrderId = order.Id,
                        PackageNo = packages.Count,
                        PackageCodes = $";{string.Join(";", packageCode)};",
                        UnsignText = MyCommon.Ucs2Convert($"{order.Code} {order.CustomerEmail}" +
                                                          $" {order.ContactPhone} {order.CustomerName}" +
                                                          $" {string.Join(" ", packageCode)}" +
                                                          $" {string.Join(" ", transportCode)}")
                    };

                    UnitOfWork.OrderServiceOtherRepo.Add(s);
                    await UnitOfWork.OrderServiceOtherRepo.SaveAsync();

                    var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                    x => x.PackageId == null && x.OrderId == s.OrderId && x.ObjectId == s.Id &&
                        x.Mode == (byte)PackageNoteMode.OtherSerive);

                    if (packageNote == null && !string.IsNullOrWhiteSpace(model.Note))
                    {
                        UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                        {
                            OrderId = s.OrderId,
                            OrderCode = s.OrderCode,
                            PackageId = null,
                            PackageCode = null,
                            UserId = importWarehouse.UserId,
                            UserFullName = importWarehouse.UserFullName,
                            Time = DateTime.Now,
                            ObjectId = s.Id,
                            ObjectCode = s.Id.ToString(),
                            Mode = (byte)PackageNoteMode.OtherSerive,
                            Content = model.Note,
                            DataJson = JsonConvert.SerializeObject(s, new JsonSerializerSettings(){ ContractResolver = new CamelCasePropertyNamesContractResolver()})
                        });
                    }
                    else if (packageNote != null && !string.IsNullOrWhiteSpace(model.Note))
                    {
                        packageNote.Content = model.Note;
                    }
                    else if (packageNote != null && string.IsNullOrWhiteSpace(model.Note))
                    {
                        UnitOfWork.PackageNoteRepo.Remove(packageNote);
                    }

                    await UnitOfWork.ImportWarehouseDetailRepo.SaveAsync();


                    //5. Cập nhật lại trạng thái của các Orders liên quan
                    #region Tính lại tiền dịch vụ phát sinh trong Orders
                    // Thêm tiền dịch vụ khác cho đơn  hàng
                    order.LastUpdate = timeNow;
                    // Tính Total money dịch vụ khác
                    var totalServiceOther = await UnitOfWork.OrderServiceOtherRepo.FindAsync(x => x.OrderId == order.Id);

                    var otherService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => !x.IsDelete && x.OrderId == order.Id &&
                                    x.ServiceId == (byte)OrderServices.Other && x.Checked);

                    if (otherService == null)
                    {
                        otherService = new OrderService
                        {
                            IsDelete = false,
                            Checked = true,
                            Created = timeNow,
                            LastUpdate = timeNow,
                            Value = totalServiceOther.Sum(x => x.Value),
                            Currency = Currency.CNY.ToString(),
                            ExchangeRate = order.ExchangeRate,
                            TotalPrice = totalServiceOther.Sum(x => x.TotalPrice),
                            HashTag = string.Empty,
                            Mode = (byte)OrderServiceMode.Option,
                            OrderId = order.Id,
                            ServiceId = (byte)OrderServices.Other,
                            ServiceName = (OrderServices.Other).GetAttributeOfType<DescriptionAttribute>()
                                .Description,
                            Type = (byte)UnitType.Money,
                            Note = $"Incurred service expenses. (Shop charging shipping fees after stock arrival, forklift renting fee...",
                        };

                        UnitOfWork.OrderServiceRepo.Add(otherService);
                    }
                    else
                    {
                        otherService.LastUpdate = timeNow;
                        otherService.Value = totalServiceOther.Sum(x => x.Value);
                        otherService.TotalPrice = totalServiceOther.Sum(x => x.TotalPrice);
                    }

                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                    x.IsDelete == false && x.Checked).ToList().Sum(x => x.TotalPrice);

                    order.Total = order.TotalExchange + totalService;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);
                    #endregion

                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(new {Status = 1, Text = "Info missing package added successfully" },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.ImportWarehouse, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> Update(ImportWarehouseMeta model)
        {
            ModelState.Remove("Packages");

            if (UserState.OfficeType != 1)
                return JsonCamelCaseResult(
                    new { Status = -2, Text = "Only warehouse staff can perform this action" },
                    JsonRequestBehavior.AllowGet);

            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(", ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList());

                return JsonCamelCaseResult(new { Status = -1, Text = $"Error: {errorMessage}" },
                    JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var importWarehouse = await UnitOfWork.ImportWarehouseRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);


                    // Goods received note does not exist or has been deleted
                    if (importWarehouse == null)
                    {
                        return JsonCamelCaseResult(new { Status = -2, Text = "Goods received note does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);
                    }

                    // Không thể Edit phiếu nhập kho quá 3 ngày
                    if (importWarehouse.Created < DateTime.Now.AddDays(-2))
                    {
                        return JsonCamelCaseResult(new { Status = -2, Text = "Cannot update goods received note after 2 days" },
                            JsonRequestBehavior.AllowGet);
                    }

                    importWarehouse.ShipperPhone = model.ShipperPhone;
                    importWarehouse.Note = model.Note;
                    importWarehouse.ShipperPhone = model.ShipperPhone;
                    importWarehouse.ShipperName = model.ShipperName;
                    importWarehouse.ShipperAddress = model.ShipperAddress;
                    importWarehouse.ShipperEmail = model.ShipperEmail;
                    importWarehouse.Updated = DateTime.Now;

                    await UnitOfWork.ImportWarehouseRepo.SaveAsync();

                    // Thêm note cho các Orders
                    var importWarehouseDetail =
                        await UnitOfWork.ImportWarehouseDetailRepo.FindAsync(
                            x => x.IsDelete == false && x.ImportWarehouseId == importWarehouse.Id);

                    foreach (var result in importWarehouseDetail.Select(x=> new {x.OrderId, x.OrderCode }).Distinct().ToList())
                    {
                        var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                            x =>
                                x.PackageId == null && x.OrderId == result.OrderId && x.ObjectId == importWarehouse.Id &&
                                x.Mode == (byte) PackageNoteMode.ChinaImport);

                        if (packageNote == null && !string.IsNullOrWhiteSpace(model.Note))
                        {
                            UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                            {
                                OrderId = result.OrderId ?? 0,
                                OrderCode = result.OrderCode,
                                PackageId = null,
                                PackageCode = null,
                                UserId = importWarehouse.UserId,
                                UserFullName = importWarehouse.UserFullName,
                                Time = DateTime.Now,
                                ObjectId = importWarehouse.Id,
                                ObjectCode = importWarehouse.Code,
                                Mode = (byte)PackageNoteMode.ChinaImport,
                                Content = model.Note
                            });
                        }
                        else if (packageNote != null && !string.IsNullOrWhiteSpace(model.Note))
                        {
                            packageNote.Content = model.Note;
                        }
                        else if (packageNote != null && string.IsNullOrWhiteSpace(model.Note))
                        {
                            UnitOfWork.PackageNoteRepo.Remove(packageNote);
                        }
                    }

                    await UnitOfWork.ImportWarehouseDetailRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(new { Status = 1, Text = "Goods received note updated successfully" },
                    JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.ImportWarehouse, EnumPage.ImportWarehouseWallet)]
        public async Task<ActionResult> Add(ImportWarehouseMeta model)
        {
            if (UserState.OfficeType != 1)
                return JsonCamelCaseResult(
                    new { Status = -2, Text = "Only warehouse staff can perform this action" },
                    JsonRequestBehavior.AllowGet);

            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(", ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList());

                return JsonCamelCaseResult(new { Status = -1, Text = $"Error: {errorMessage}" },
                    JsonRequestBehavior.AllowGet);
            }

            var timeNow = DateTime.Now;

            var codes = new List<string>();
            var codeOrder = new List<string>();

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //1. Tạo phiếu nhập kho
                    var importWarehouse = new ImportWarehouse
                    {
                        Updated = timeNow,
                        Created = timeNow,
                        Status = model.Status,
                        UserId = UserState.UserId,
                        UserFullName = UserState.FullName,
                        UserName = UserState.UserName,
                        WarehouseId = UserState.OfficeId ?? -1,
                        WarehouseName = UserState.OfficeName,
                        WarehouseIdPath = UserState.OfficeIdPath,
                        WarehouseAddress = UserState.OfficeAddress,
                        Note = model.Note,
                        ShipperPhone = model.ShipperPhone,
                        ShipperName = model.ShipperName,
                        ShipperAddress = model.ShipperAddress,
                        ShipperEmail = model.ShipperEmail,
                        Code = string.Empty,
                        UnsignedText = string.Empty
                    };

                    UnitOfWork.ImportWarehouseRepo.Add(importWarehouse);
                    await UnitOfWork.ImportWarehouseRepo.SaveAsync();

                    //2. Cập nhật lại Mã Code cho phiếu nhập kho
                    var importWarehouseOfDay = UnitOfWork.ImportWarehouseRepo.Count(x =>
                        (x.Created.Year == timeNow.Year) && (x.Created.Month == timeNow.Month) &&
                        (x.Created.Day == timeNow.Day) && (x.Id <= importWarehouse.Id));

                    importWarehouse.Code = $"{importWarehouseOfDay}{timeNow:ddMMyy}";
                    importWarehouse.UnsignedText =
                        MyCommon.Ucs2Convert(
                            $"{importWarehouse.Code} {importWarehouse.UserFullName} {importWarehouse.UserName} {importWarehouse.WarehouseName}");

                    await UnitOfWork.ImportWarehouseRepo.SaveAsync();

                    // Thêm kiền hàng mất mã
                    if (model.TransportCodes != null && model.TransportCodes.Any())
                    {
                        foreach (var transportCode in model.TransportCodes)
                        {
                            if(string.IsNullOrWhiteSpace(transportCode))
                                continue;

                            var orderPackage = new OrderPackage()
                            {
                                Code = string.Empty,
                                OrderId = 0,
                                OrderCode = string.Empty,
                                CustomerId = 0,
                                CustomerName = string.Empty,
                                CustomerUserName = string.Empty,
                                CustomerLevelId = 0,
                                CustomerLevelName = string.Empty,
                                CustomerWarehouseId = 0,
                                CustomerWarehouseName = string.Empty,
                                CustomerWarehouseIdPath = string.Empty,
                                CustomerWarehouseAddress = string.Empty,
                                TransportCode = string.IsNullOrWhiteSpace(transportCode) ? string.Empty : transportCode,
                                WarehouseId = UserState.OfficeId ?? 0,
                                WarehouseName = UserState.OfficeName,
                                WarehouseIdPath = UserState.OfficeIdPath,
                                WarehouseAddress = UserState.OfficeAddress,
                                UserId = UserState.UserId,
                                UserFullName = UserState.FullName,
                                SystemId = 0,
                                SystemName = string.Empty,
                                Created = timeNow,
                                LastUpdate = timeNow,
                                HashTag = ";packagelose;",
                                PackageNo = 0,
                                UnsignedText = string.Empty,
                                ForcastDate = null,
                                OrderType = 0,
                                Note = null,
                                Status = (byte)OrderPackageStatus.ChinaReceived,
                                CurrentWarehouseId = UserState.OfficeId,
                                CurrentWarehouseName = UserState.OfficeName,
                                CurrentWarehouseIdPath = UserState.OfficeIdPath,
                                CurrentWarehouseAddress = UserState.OfficeAddress,
                            };

                            UnitOfWork.OrderPackageRepo.Add(orderPackage);
                            await UnitOfWork.OrderPackageRepo.SaveAsync();

                            var orderPackageOfDay = UnitOfWork.OrderPackageRepo.Count(x =>
                                (x.Created.Year == timeNow.Year) && (x.Created.Month == timeNow.Month) &&
                                (x.Created.Day == timeNow.Day) && (x.Id <= orderPackage.Id));

                            orderPackage.Code = $"{orderPackageOfDay}{timeNow:ddMMyy}";
                        }

                        await UnitOfWork.OrderPackageRepo.SaveAsync();
                    }

                    //3. Thêm kiện/bao hàng
                    foreach (var package in model.Packages)
                    {
                        #region Thêm package
                        // package
                        if (package.Type == 0)
                        {
                            var p = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(
                                x => x.Id == package.PackageId && !x.IsDelete &&
                                     (x.Status == (byte)OrderPackageStatus.ShopDelivery));

                            if (p == null)
                            {
                                codes.Add($"P{package.PackageCode}");
                                continue;
                            }

                            // Thêm lịch sử cho package
                            var packageHistory = new PackageHistory()
                            {
                                PackageId = p.Id,
                                PackageCode = p.Code,
                                OrderId = p.OrderId,
                                OrderCode = p.OrderCode,
                                Type = p.OrderType,
                                Status = (byte)OrderPackageStatus.ChinaReceived,
                                Content = $"[{UserState.OfficeName}] {EnumHelper.GetEnumDescription(OrderPackageStatus.ChinaReceived)}",
                                CustomerId = p.CustomerId,
                                CustomerName = p.CustomerName,
                                UserId = UserState.UserId,
                                UserName = UserState.UserName,
                                UserFullName = UserState.FullName,
                                CreateDate = DateTime.Now,
                            };

                            UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                            await UnitOfWork.PackageHistoryRepo.SaveAsync();

                            // Lấy ra tất cả các package trùng mã vận đơn
                            //var packageSameTransportCode = await UnitOfWork.OrderPackageRepo.FindAsync(
                            //    x =>
                            //        x.TransportCode == p.TransportCode && !x.IsDelete &&
                            //        x.Status != (byte)OrderPackageStatus.Completed);

                            //// Có package trùng mã vận đơn
                            //if (packageSameTransportCode.Any(x => x.Id != p.Id))
                            //{
                            //    // Một Orders có nhiều kiện trùng mã vận đơn
                            //    if (packageSameTransportCode.Count(x => x.OrderId == p.OrderId) ==
                            //        packageSameTransportCode.Count())
                            //    {
                            //        var packagesCode = string.Join(", ",
                            //            packageSameTransportCode.Select(x => "P" + x.Code).ToList());
                            //        var customers = string.Join(", ",
                            //            packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList());

                            //        foreach (var ps in packageSameTransportCode)
                            //        {
                            //            // Add cảnh báo cho kiện hàng
                            //            ps.Note = $" Related packages: {packagesCode} in the same order";
                            //            ps.PackageCodes = packagesCode;
                            //            ps.PackageCodesUnsigned =
                            //                $";{string.Join(";", packageSameTransportCode.Select(x => x.Code).ToList())};";
                            //            ps.Customers = $"{customers}";
                            //            ps.CustomersUnsigned =
                            //                $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";
                            //        }

                            //        p.Note = $" Related packages: {packagesCode} in the same order";
                            //        p.PackageCodes = packagesCode;
                            //        p.PackageCodesUnsigned =
                            //            $";{string.Join(";", packageSameTransportCode.Select(x => x.Code).ToList())};";
                            //        p.Customers = $"{customers}";
                            //        p.CustomersUnsigned =
                            //            $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";
                            //    }
                            //    else if (packageSameTransportCode.Any(x => x.OrderId != p.OrderId))
                            //    // Có Orders trùng mã vận đơn
                            //    {
                            //        var orderCode = string.Join(", ",
                            //            packageSameTransportCode.Select(x => MyCommon.ReturnCode(x.OrderCode)).Distinct().ToList());
                            //        var customers = string.Join(", ",
                            //            packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList());

                            //        // Kiểm tra khác kho đích
                            //        var diffWarehouse =
                            //            packageSameTransportCode.Any(x => x.CustomerWarehouseId != p.CustomerWarehouseId);

                            //        foreach (var ps in packageSameTransportCode)
                            //        {
                            //            // Add cảnh báo cho kiện hàng
                            //            ps.Note = $" Order with coincided waybill code: {orderCode}";
                            //            ps.OrderCodes = orderCode;
                            //            ps.OrderCodesUnsigned =
                            //                $";{string.Join(";", packageSameTransportCode.Select(x => x.OrderCode).Distinct().ToList())};";
                            //            ps.Customers = $"{customers}";
                            //            ps.CustomersUnsigned =
                            //                $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";

                            //            if (diffWarehouse)
                            //                ps.Note += ", The destination package is different";
                            //        }

                            //        p.Note = $" Order with coincided waybill code: {orderCode}";
                            //        p.OrderCodes = orderCode;
                            //        p.OrderCodesUnsigned =
                            //            $";{string.Join(";", packageSameTransportCode.Select(x => x.OrderCode).Distinct().ToList())};";
                            //        p.Customers = $"{customers}";
                            //        p.CustomersUnsigned =
                            //            $";{string.Join(";", packageSameTransportCode.Select(x => x.CustomerUserName).Distinct().ToList())};";

                            //        // Add mã đơn hàng vào danh sách để cập nhật lại thông tin đơn hàng
                            //        codeOrder.AddRange(packageSameTransportCode.Select(x => x.OrderCode));

                            //        if (diffWarehouse)
                            //            p.Note += ", The destination package is different";
                            //    }
                            //}
                            //await UnitOfWork.OrderPackageRepo.SaveAsync();

                            // Cập nhật thông tin kho hiện tại cho package
                            p.Status = (byte)OrderPackageStatus.ChinaReceived;
                            p.CurrentWarehouseId = UserState.OfficeId;
                            p.CurrentWarehouseName = UserState.OfficeName;
                            p.CurrentWarehouseIdPath = UserState.OfficeIdPath;
                            p.CurrentWarehouseAddress = UserState.OfficeAddress;
                            p.ForcastDate = null;

                            // Add mã đơn hàng vào danh sách để cập nhật lại thông tin đơn hàng
                            codeOrder.Add(p.OrderCode);

                            UnitOfWork.ImportWarehouseDetailRepo.Add(new ImportWarehouseDetail
                            {
                                Created = timeNow,
                                Updated = timeNow,
                                IsDelete = false,
                                Note = package.Note,
                                PackageId = p.Id,
                                PackageCode = p.Code,
                                OrderCode = p.OrderCode,
                                OrderId = p.OrderId,
                                OrderType = p.OrderType,
                                OrderPackageNo = p.PackageNo,
                                Status = 1,
                                TransportCode = p.TransportCode,
                                ImportWarehouseId = importWarehouse.Id,
                                ImportWarehouseCode = importWarehouse.Code,
                                Type = package.Type,
                                WarehouseId = p.CustomerWarehouseId,
                                WarehouseIdPath = p.CustomerWarehouseIdPath,
                                WarehouseAddress = p.CustomerWarehouseAddress,
                                WarehouseName = p.CustomerWarehouseName,
                                CustomerId = p.CustomerId,
                                CustomerName = p.CustomerName,
                                CustomerUserName = p.CustomerUserName,
                                OrderServices = p.OrderServices,
                            });

                            //Thêm thông tin kiện đã nhập kho cho Orders
                            if (p.OrderId > 0)
                            {
                                var o = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == p.OrderId);
                                o.PackageNoInStock = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.OrderId == o.Id);
                                await UnitOfWork.OrderRepo.SaveAsync();
                            }

                            continue;
                        }
                        #endregion

                        #region Thêm kiện bao hàng
                        // Bao hàng
                        if (package.Type == 1)
                        {
                            var b = await UnitOfWork.WalletRepo.SingleOrDefaultAsync(
                                x =>
                                    (x.Id == package.PackageId) && !x.IsDelete &&
                                    (x.Status == (byte)WalletStatus.Shipping));

                            if (b == null)
                            {
                                codes.Add($"B{package.PackageCode}");
                                continue;
                            }

                            if (codes.Any())
                                continue;

                            // Cập nhật thông tin bao hàng
                            b.CurrentWarehouseId = UserState.OfficeId;
                            b.CurrentWarehouseIdPath = UserState.OfficeIdPath;
                            b.CurrentWarehouseAddress = UserState.OfficeAddress;
                            b.CurrentWarehouseName = UserState.OfficeName;
                            b.Status = (byte)WalletStatus.InStock;
                            b.Updated = DateTime.Now;

                            // Cập nhật lại trạng thái của phiếu điều vận cho bao hàng
                            var dispatcherDetails =
                                await UnitOfWork.DispatcherDetailRepo.FindAsync(
                                    x => x.WalletId == b.Id && x.IsDelete == false && x.Status != 2);

                            foreach (var dispatcherDetail in dispatcherDetails)
                            {
                                dispatcherDetail.Status = 2;
                                dispatcherDetail.Updated = DateTime.Now;
                            }

                            // Cập nhật lại CurrentWarehouse của toàn bộ package trong bao
                            var packages = await UnitOfWork.OrderPackageRepo.OrderPackageByWalletId(b.Id);

                            foreach (var p in packages)
                            {
                                // Thêm lịch sử cho package
                                var packageHistory = new PackageHistory()
                                {
                                    PackageId = p.Id,
                                    PackageCode = p.Code,
                                    OrderId = p.OrderId,
                                    OrderCode = p.OrderCode,
                                    Type = p.OrderType,
                                    Status = (byte)OrderPackageStatus.Received,
                                    Content = $"[{UserState.OfficeName}] {EnumHelper.GetEnumDescription(OrderPackageStatus.Received)}",
                                    CustomerId = p.CustomerId,
                                    CustomerName = p.CustomerName,
                                    UserId = UserState.UserId,
                                    UserName = UserState.UserName,
                                    UserFullName = UserState.FullName,
                                    CreateDate = DateTime.Now,
                                };

                                UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                                // Cập nhật thông tin kho hiện tại cho package
                                p.Status = (byte)OrderPackageStatus.Received;
                                p.CurrentWarehouseId = UserState.OfficeId;
                                p.CurrentWarehouseName = UserState.OfficeName;
                                p.CurrentWarehouseIdPath = UserState.OfficeIdPath;
                                p.CurrentWarehouseAddress = UserState.OfficeAddress;
                                p.ForcastDate = null;

                                // Add Orders code vào danh sách để cập nhật lại thông tin Orders
                                if (!string.IsNullOrWhiteSpace(p.OrderCodes))
                                {
                                    codeOrder.AddRange(p.OrderCodes.Split(',').ToList().Select(x => x.Trim()));
                                }
                                else
                                {
                                    codeOrder.Add(p.OrderCode);
                                }
                            }

                            UnitOfWork.ImportWarehouseDetailRepo.Add(new ImportWarehouseDetail
                            {
                                Created = timeNow,
                                Updated = timeNow,
                                IsDelete = false,
                                Note = package.Note,
                                PackageId = b.Id,
                                PackageCode = b.Code,
                                Status = 1,
                                ImportWarehouseId = importWarehouse.Id,
                                ImportWarehouseCode = importWarehouse.Code,
                                Type = package.Type,
                                WarehouseId = b.TargetWarehouseId ?? 0,
                                WarehouseIdPath = b.TargetWarehouseIdPath,
                                WarehouseAddress = b.TargetWarehouseAddress,
                                WarehouseName = b.TargetWarehouseName
                            });
                        }
                        #endregion
                    }

                    //4. Có kiên/bao does not exist
                    if (codes.Any())
                    {
                        transaction.Rollback();
                        return
                            JsonCamelCaseResult(
                                new
                                {
                                    Status = -2,
                                    Text = $"Packages/Sacks: {string.Join(", ", codes)} does not exist or has been deleted"
                                },
                                JsonRequestBehavior.AllowGet);
                    }
                    await UnitOfWork.ImportWarehouseDetailRepo.SaveAsync();

                    #region Cập nhật chi phí phát sinh của package

                    if (model.OrderServiceOthers != null && model.OrderServiceOthers.Any())
                    {
                        foreach (var serviceOther in model.OrderServiceOthers)
                        {
                            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Code == serviceOther.OrderCode);
                            var packages =
                                await UnitOfWork.ImportWarehouseDetailRepo.FindAsync(
                                    x =>
                                        x.IsDelete == false && x.ImportWarehouseId == importWarehouse.Id &&
                                        x.OrderId == order.Id);

                            var packageCode = packages.Select(x => x.PackageCode).ToList();

                            var transportCode = packages.Select(x => x.TransportCode).ToList();

                            var s = new OrderServiceOther()
                            {
                                Value = serviceOther.Value,
                                Created = DateTime.Now,
                                Mode = serviceOther.Mode,
                                CreatedOfficeId = UserState.OfficeId ?? 0,
                                CreatedOfficeIdPath = UserState.OfficeIdPath,
                                CreatedOfficeName = UserState.OfficeName,
                                CreatedUserFullName = UserState.FullName,
                                CreatedUserId = UserState.UserId,
                                CreatedUserTitleId = UserState.TitleId ?? 0,
                                CreatedUserTitleName = UserState.TitleName,
                                CreatedUserUserName = UserState.UserName,
                                Currency = Currency.CNY.ToString(),
                                Note = serviceOther.Note,
                                ObjectId = importWarehouse.Id,
                                Type = 0,
                                ExchangeRate = order.ExchangeRate,
                                TotalPrice = order.ExchangeRate * serviceOther.Value,
                                OrderCode = order.Code,
                                OrderId = order.Id,
                                PackageNo = packages.Count,
                                PackageCodes = $";{string.Join(";", packageCode)};",
                                UnsignText = MyCommon.Ucs2Convert($"{order.Code} {order.CustomerEmail}" +
                                                                  $" {order.ContactPhone} {order.CustomerName}" +
                                                                  $" {string.Join(" ", packageCode)}" +
                                                                  $" {string.Join(" ", transportCode)}")
                            };

                            UnitOfWork.OrderServiceOtherRepo.Add(s);
                        }

                        await UnitOfWork.OrderServiceOtherRepo.SaveAsync();
                    }

                    #endregion

                    importWarehouse.PackageNumber = await UnitOfWork.ImportWarehouseDetailRepo.CountAsync(
                        x => !x.IsDelete && (x.ImportWarehouseId == importWarehouse.Id));

                    //5. Cập nhật lại trạng thái của các đơn hàng liên quan
                    if (codeOrder.Any())
                    {
                        var strCodeOrder = $";{string.Join(";", codeOrder.Distinct().ToList())};";

                        var orders = await UnitOfWork.OrderRepo.FindAsync(x => strCodeOrder.Contains(";" + x.Code + ";") && !x.IsDelete);

                        #region Tính lại tiền dịch vụ phát sinh trong Orders
                        // Thêm tiền dịch vụ khác cho đơn  hàng

                        if (model.OrderServiceOthers != null && model.OrderServiceOthers.Any())
                        {
                            foreach (var order in orders)
                            {
                                order.LastUpdate = timeNow;
                                // Tính Total money dịch vụ khác
                                var totalServiceOther = await UnitOfWork.OrderServiceOtherRepo.FindAsync(x => x.OrderId == order.Id);

                                var otherService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                            x => !x.IsDelete && x.OrderId == order.Id &&
                                                x.ServiceId == (byte)OrderServices.Other && x.Checked);

                                if (otherService == null)
                                {
                                    otherService = new OrderService
                                    {
                                        IsDelete = false,
                                        Checked = true,
                                        Created = timeNow,
                                        LastUpdate = timeNow,
                                        Value = totalServiceOther.Sum(x => x.Value),
                                        Currency = Currency.CNY.ToString(),
                                        ExchangeRate = order.ExchangeRate,
                                        TotalPrice = totalServiceOther.Sum(x => x.TotalPrice),
                                        HashTag = string.Empty,
                                        Mode = (byte)OrderServiceMode.Option,
                                        OrderId = order.Id,
                                        ServiceId = (byte)OrderServices.Other,
                                        ServiceName = (OrderServices.Other).GetAttributeOfType<DescriptionAttribute>()
                                            .Description,
                                        Type = (byte)UnitType.Money,
                                        Note = $"Incurred service expenses. (Shop charging shipping fees after stock arrival, forklift renting fee...",
                                    };

                                    UnitOfWork.OrderServiceRepo.Add(otherService);
                                }
                                else
                                {
                                    otherService.LastUpdate = timeNow;
                                    otherService.Value = totalServiceOther.Sum(x => x.Value);
                                    otherService.TotalPrice = totalServiceOther.Sum(x => x.TotalPrice);
                                }

                                await UnitOfWork.OrderServiceRepo.SaveAsync();

                                var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                                x.IsDelete == false && x.Checked).ToList().Sum(x => x.TotalPrice);

                                order.Total = order.TotalExchange + totalService;
                                order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);
                            }
                        }
                        #endregion
                    }

                    await UnitOfWork.ImportWarehouseRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            //if (!string.IsNullOrWhiteSpace(orderIds))
            //    BackgroundJob.Enqueue(() => OrderJob.ProcessDebitReport(orderIds));

            return JsonCamelCaseResult(new { Status = 1, Text = "Warehousing request created successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddOtherService()
        {
            var datas = new List<OrderServiceOtherMeta>()
            {
                new OrderServiceOtherMeta()
                {
                    OrderCode = "3101602171",
                    Mode = 0,
                    Value = 65,
                    Note = "10327437282 65 renminbi for goods loading fee"
                },
                new OrderServiceOtherMeta()
                {
                    OrderCode = "1411802170",
                    Mode = 0,
                    Value = 86,
                    Note = "28093570 86 renminbi for shipping"
                }
            };

            List<string> orderCodes = new List<string>();
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var d in datas)
                    {
                        var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Code == d.OrderCode);
                        var importWarehouse = await UnitOfWork.ImportWarehouseRepo.GetImportWarehouse(d.OrderCode);

                        if (importWarehouse == null)
                        {
                            orderCodes.Add(d.OrderCode);
                            continue;
                        }

                        var packages =
                            await UnitOfWork.ImportWarehouseDetailRepo.FindAsync(
                                x =>
                                    x.IsDelete == false && x.ImportWarehouseId == importWarehouse.Id &&
                                    x.OrderId == order.Id);

                        var packageCode = packages.Select(x => x.PackageCode).ToList();

                        var transportCode = packages.Select(x => x.TransportCode).ToList();

                        var s = new OrderServiceOther()
                        {
                            Value = d.Value,
                            Created = DateTime.Now,
                            Mode = d.Mode,
                            CreatedOfficeId = UserState.OfficeId ?? 0,
                            CreatedOfficeIdPath = UserState.OfficeIdPath,
                            CreatedOfficeName = UserState.OfficeName,
                            CreatedUserFullName = UserState.FullName,
                            CreatedUserId = UserState.UserId,
                            CreatedUserTitleId = UserState.TitleId ?? 0,
                            CreatedUserTitleName = UserState.TitleName,
                            CreatedUserUserName = UserState.UserName,
                            Currency = Currency.CNY.ToString(),
                            Note = d.Note,
                            ObjectId = importWarehouse.Id,
                            Type = 0,
                            ExchangeRate = order.ExchangeRate,
                            TotalPrice = order.ExchangeRate * d.Value,
                            OrderCode = order.Code,
                            OrderId = order.Id,
                            PackageNo = packages.Count,
                            PackageCodes = $";{string.Join(";", packageCode)};",
                            UnsignText = MyCommon.Ucs2Convert($"{order.Code} {order.CustomerEmail}" +
                                                              $" {order.ContactPhone} {order.CustomerName}" +
                                                              $" {string.Join(" ", packageCode)}" +
                                                              $" {string.Join(" ", transportCode)}")
                        };

                        UnitOfWork.OrderServiceOtherRepo.Add(s);
                    }

                    await UnitOfWork.OrderServiceOtherRepo.SaveAsync();

                    var timeNow = DateTime.Now;

                    var strCodeOrder = $";{string.Join(";", datas.Select(x => x.OrderCode).Distinct().ToList())};";
                    var orders = await UnitOfWork.OrderRepo.FindAsync(
                            x => strCodeOrder.Contains(";" + x.Code + ";") && !x.IsDelete);

                    foreach (var order in orders)
                    {
                        order.LastUpdate = timeNow;
                        // Tính Total money dịch vụ khác
                        var totalServiceOther = await UnitOfWork.OrderServiceOtherRepo.FindAsync(x => x.OrderId == order.Id);

                        var otherService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                    x => !x.IsDelete && x.OrderId == order.Id &&
                                        x.ServiceId == (byte)OrderServices.Other && x.Checked);

                        if (otherService == null)
                        {
                            otherService = new OrderService
                            {
                                IsDelete = false,
                                Checked = true,
                                Created = timeNow,
                                LastUpdate = timeNow,
                                Value = totalServiceOther.Sum(x => x.Value),
                                Currency = Currency.CNY.ToString(),
                                ExchangeRate = order.ExchangeRate,
                                TotalPrice = totalServiceOther.Sum(x => x.TotalPrice),
                                HashTag = string.Empty,
                                Mode = (byte)OrderServiceMode.Option,
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.Other,
                                ServiceName = (OrderServices.Other).GetAttributeOfType<DescriptionAttribute>()
                                    .Description,
                                Type = (byte)UnitType.Money,
                                Note = $"Incurred service expenses. (Shop (supplier) charges shipping and loading fees...)",
                            };

                            UnitOfWork.OrderServiceRepo.Add(otherService);
                        }
                        else
                        {
                            otherService.LastUpdate = timeNow;
                            otherService.Value = totalServiceOther.Sum(x => x.Value);
                            otherService.TotalPrice = totalServiceOther.Sum(x => x.TotalPrice);
                        }

                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                        x.IsDelete == false && x.Checked).ToList().Sum(x => x.TotalPrice);

                        order.Total = order.TotalExchange + totalService;
                        order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);
                    }

                    //if (orderCodes.Any())
                    //{
                    //    transaction.Rollback();
                    //    return Json(orderCodes, JsonRequestBehavior.AllowGet);
                    //}

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }

            return Json(orderCodes, JsonRequestBehavior.AllowGet);
        }
    }
}