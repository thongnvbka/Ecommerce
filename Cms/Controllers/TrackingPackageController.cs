using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cms.Attributes;
using Common.Emums;
using Common.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;
using System.Drawing;

namespace Cms.Controllers
{
    [Authorize]
    public class TrackingPackageController : BaseController
    {
        // GET: TrackingPackage
        [LogTracker(EnumAction.View, EnumPage.TrackingPackage)]
        public async Task<ActionResult> Index()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            ViewBag.States = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            var allWarehouse = await UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                x => !x.IsDelete && (x.Type == (byte)OfficeType.Warehouse) && (x.Status == (byte)OfficeStatus.Use));

            ViewBag.AllWarehouses =
                JsonConvert.SerializeObject(allWarehouse.Select(x => new { x.Id, x.Name, x.IdPath, x.Address }).ToList(),
                    jsonSerializerSettings);

            ViewBag.OrderPackageStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.WalletStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(WalletStatus))
                .Cast<WalletStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<WalletStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.TrackingPackage)]
        public async Task<ActionResult> Search(DateTime? fromDate, DateTime? toDate, byte? searchType, string timeTypeText, 
            string keyword, string statusText, string warehouseIdText, string orderStatusText, string statusDepositText,
            string orderTypeText, string orderServiceText, int currentPage = 1, int recordPerPage = 20, bool isFirstRequest = false)
        {
            keyword = MyCommon.Ucs2Convert(keyword).Trim();

            long totalRecord;
            var packages = await UnitOfWork.OrderPackageRepo.GetOrderPackageForTracking(keyword, searchType, statusText,
                warehouseIdText, orderStatusText, statusDepositText, orderTypeText, orderServiceText, fromDate, toDate, timeTypeText, 
                currentPage, recordPerPage, out totalRecord);

            var packageIds = $";{string.Join(";", packages.Select(x => x.Id).ToList())};";
            var orderIds = $";{string.Join(";", packages.Select(x => x.OrderId).Distinct().ToList())};";

            var packageHistories =
                UnitOfWork.PackageHistoryRepo.Find(x => packageIds.Contains(";" + x.PackageId + ";"))
                .GroupBy(x => x.PackageId)
                .ToDictionary(x => x.Key, x => x.OrderBy(y => y.CreateDate).ToList());

            var orderServices = UnitOfWork.OrderServiceRepo.Find(
                    x => x.IsDelete == false && x.Checked && orderIds.Contains(";" + x.OrderId + ";"))
                .GroupBy(x => x.OrderId)
                .ToDictionary(x => x.Key, x => x.ToList());

            var wallets = await UnitOfWork.WalletRepo.GetWalletPackage(packageIds);

            var walletPackages = wallets.GroupBy(x => x.PackageId + "_" + x.Mode)
                .ToDictionary(x => x.Key, x => x.ToList());

            var packageNotesData =
                await UnitOfWork.PackageNoteRepo.FindAsNoTrackingAsync(x => x.PackageId != null && packageIds.Contains(";" + x.PackageId + ";"));

            var packageNotes = packageNotesData.GroupBy(x => x.PackageId)
                .ToDictionary(x => x.Key, x => x.OrderBy(o => o.Time).ToList());

            // Không phải là lần Request đầu tiên
            if (isFirstRequest == false)
                return JsonCamelCaseResult(new { packages, walletPackages, orderServices, packageHistories, packageNotes, totalRecord },
                    JsonRequestBehavior.AllowGet);

            // Object Javascript: Order Type
            var orderType = Enum.GetValues(typeof(OrderType))
                .Cast<OrderType>()
                .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<OrderType>((int)v));

            // Array Javascript: Order Type
            var orderTypes = Enum.GetValues(typeof(OrderType))
                .Cast<OrderType>()
                .Select(x => new
                {
                    Id = (byte)x,
                    Name = EnumHelper.GetEnumDescription<OrderType>((int)x),
                    Checked = false
                })
                .ToList();           

            // Object Javascript: Trạng thái package
            var packageStatus = Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderStatus>()
                .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v));

            var packageNoteMode = Enum.GetValues(typeof(PackageNoteMode))
                .Cast<PackageNoteMode>()
                .ToDictionary(v => (byte) v, v => EnumHelper.GetEnumDescription<PackageNoteMode>((int) v));

            // Array Javascript: Trạng thái Order
            var packageStatuss = Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(x => new
                {
                    Id = (byte)x,
                    Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)x),
                    Checked = false
                })
                .ToList();

            // Object Javascript: Trạng thái Order
            var orderStatus = Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<OrderStatus>((int)v));

            // Array Javascript: Trạng thái Order
            var orderStatuss = Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(x => new
                {
                    Id = (byte)x,
                    Name = EnumHelper.GetEnumDescription<OrderStatus>((int)x),
                    Checked = false
                })
                .ToList();

            // Object Javascript: Trạng thái Order
            var depositStatus = Enum.GetValues(typeof(DepositStatus))
                .Cast<DepositStatus>()
                .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<DepositStatus>((int)v));

            // Array Javascript: Trạng thái Order
            var depositStatuss = Enum.GetValues(typeof(DepositStatus))
                .Cast<DepositStatus>()
                .Select(x => new
                {
                    Id = (byte)x,
                    Name = EnumHelper.GetEnumDescription<DepositStatus>((int)x),
                    Checked = false
                })
                .ToList();

            var services = new List<dynamic>()
            {
                new {Id = (int)OrderServices.Audit, Name = "Tally", Checked = false},
                new {Id = (int)OrderServices.Packing, Name = "Packing", Checked = false},
                //new {Id = (int)OrderServices.FastDelivery, Name = "Đi bay", Checked = false},
                //new {Id = (int)OrderServices.Optimal, Name = "Tối ưu", Checked = false}
            };

            var warehouses = UnitOfWork.OfficeRepo.Find(
                x => x.IsDelete == false && x.Status == (byte)OfficeStatus.Use &&
                     x.Type == (byte)OfficeType.Warehouse).Select(x => new { x.Id, x.Name, Checked = false });

            return JsonCamelCaseResult(
                new
                {
                    packages,
                    services,
                    walletPackages,
                    orderServices,
                    packageHistories,
                    warehouses,
                    orderStatuss,
                    orderStatus,
                    depositStatus,
                    depositStatuss,
                    orderTypes,
                    orderType,
                    packageNoteMode,
                    packageStatus,
                    packageStatuss,
                    packageNotes,
                    totalRecord
                },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> ExcelTrackingPackage(string keyword, DateTime? fromDate, DateTime? toDate, 
            byte? searchType, string timeTypeText, string statusText, string warehouseIdText, 
            string orderStatusText, string statusDepositText, string orderTypeText, string orderServiceText)
        {
            keyword = MyCommon.Ucs2Convert(keyword).Trim();

            var packages = UnitOfWork.OrderPackageRepo.ExcelOrderPackageForTracking(keyword, searchType, statusText,
                warehouseIdText, orderStatusText, statusDepositText, orderTypeText, orderServiceText, fromDate, toDate, timeTypeText);

            var packageIds = $";{string.Join(";", packages.Select(x => x.Id).ToList())};";
            var wallet1S = await UnitOfWork.WalletRepo.GetWalletPackage(packageIds);

            var walletPackages = wallet1S.GroupBy(x => x.PackageId + "_" + x.Mode)
                .ToDictionary(x => x.Key, x => x.ToList());

            var packageHistories = UnitOfWork.PackageHistoryRepo.Find(x => x.Id > 0).ToList();
            //var wallets = UnitOfWork.WalletDetailRepo.Find(x => !x.IsDelete);
            var orderServicesAudits = UnitOfWork.OrderServiceRepo.Find(x => x.IsDelete == false).ToList();
            var orderServicesPackings = UnitOfWork.OrderServiceRepo.Find(x => x.IsDelete == false && x.Checked).ToList();

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col++, "STT", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "TRANSPORT CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "PACKAGE CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "Order", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "CUSTOMER EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "WEIGHT", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "CONVERTED", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "Weight charged", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "PACKING", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "TYPE ORDER", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "TRANSIT POINT", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "CUSTOMER WAREHOUSE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "GOODS IN THE WAREHOUSE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "SHOP EXPORT", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "SHOP EXPORT", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "PACK WOODEN", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "WAREHOUSE CN RECEIVED", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "WAREHOUSE CN EXPORT", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "IN VN WAREHOUSE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "SHIPPING", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "COMPLETE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "STATUS PACKAGE", ExcelHorizontalAlignment.Center, true, colorHeader);
                //ExcelHelper.CreateHeaderTable(sheet, row, col, "CƯỚC VẬN CHUYỂN THÊM", ExcelHorizontalAlignment.Center, true, colorHeader);
                //col++;
                //ExcelHelper.CreateHeaderTable(sheet, row, col, "CƯỚC ĐÓNG GỖ", ExcelHorizontalAlignment.Center, true, colorHeader);
                //col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "VỊ TRÍ KHO", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "DANH SÁCH package", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });
                //var start = GetStartOfDay(fromDate ?? DateTime.Now);
                //var end = GetEndOfDay(toDate ?? DateTime.Now);

                string ngay;
                if (fromDate == null || toDate == null)
                {
                    ngay = "";
                }
                else
                {
                    ngay = "Từ: " + fromDate.Value.ToShortDateString() + " đến " + toDate.Value.ToShortDateString();
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

                if (packages.Any())
                {
                    foreach (var package in packages)
                    {
                        var packageHistorie = packageHistories.Where(x => x.PackageId == package.Id).ToList();
                        //var wallet = wallets.FirstOrDefault(x => x.PackageId == package.Id);
                        var orderServicesAudit = orderServicesAudits.FirstOrDefault(x => x.OrderId == package.OrderId && x.ServiceId == (byte)OrderServices.Audit);
                        var orderServicesPacking = orderServicesPackings.FirstOrDefault(x => x.OrderId == package.OrderId && x.ServiceId == (byte)OrderServices.Packing);
                        var walletCode = "";
                        var audit = "";
                        var packing = "";
                        var entropot = "";
                        
                        col = 1;
                        //STT
                        ExcelHelper.CreateCellTable(sheet, no, col++, no - row, ExcelHorizontalAlignment.Center, true);
                        //MVĐ
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.TransportCode, ExcelHorizontalAlignment.Left, true);
                        //Ma kiện
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.Code, ExcelHorizontalAlignment.Left, true);
                        //Ma don hang
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.OrderCode, ExcelHorizontalAlignment.Left, true);
                        //email
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.CustomerUserName, ExcelHorizontalAlignment.Left, true);
                        //cân nặng
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.Weight, ExcelHorizontalAlignment.Left, true);
                        //quy đổi
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.WeightConverted, ExcelHorizontalAlignment.Left, true);
                        //cân nặng tính tiền
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.WeightActual, ExcelHorizontalAlignment.Left, true);
                        //bao hàng
                        var entropotDic = walletPackages.ContainsKey(package.Id + "_" + 1) ?  walletPackages[package.Id + "_" + 1] :
                            walletPackages.ContainsKey(package.Id + "_" + 0) ? walletPackages[package.Id + "_" + 0] : null;

                        if (entropotDic != null)
                        {
                            walletCode = entropotDic[0].Code;
                            entropot = entropotDic[0].EntrepotName;
                        }

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, walletCode, ExcelHorizontalAlignment.Left, true);
                        //Type hàng
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, EnumHelper.GetEnumDescription<OrderType>(package.OrderType), ExcelHorizontalAlignment.Left, true);
                        //điểm trung chuyển
                        ExcelHelper.CreateCellTable(sheet, no, col++, entropot, ExcelHorizontalAlignment.Left, true);
                        //kho khách
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.CustomerWarehouseName, ExcelHorizontalAlignment.Left, true);
                        //hàng ở kho
                        ExcelHelper.CreateCellTable(sheet, no, col++, package.CurrentWarehouseName, ExcelHorizontalAlignment.Left, true);
                        //shop xuất hàng
                        var shopDelivery = packageHistorie.Find(s => s.Status == (byte)OrderPackageStatus.ShopDelivery);
                        if (shopDelivery != null)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, shopDelivery.CreateDate.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, "", ExcelHorizontalAlignment.Left, true);
                        }
                        //kiểm đếm
                        if (orderServicesAudit != null)
                        {
                            audit = "×";
                        }
                        ExcelHelper.CreateCellTable(sheet, no, col++, audit, ExcelHorizontalAlignment.Center, true);
                        //đóng gỗ
                        if (orderServicesPacking != null)
                        {
                            packing = "×";
                        }
                        ExcelHelper.CreateCellTable(sheet, no, col++, packing, ExcelHorizontalAlignment.Center, true);
                        //kho TQ nhận
                        var chinaReceived = packageHistorie.Find(s => s.Status == (byte)OrderPackageStatus.ChinaReceived);
                        if (chinaReceived != null)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, chinaReceived.CreateDate.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, "", ExcelHorizontalAlignment.Left, true);
                        }
                        //kho TQ gửi
                        var chinaExport = packageHistorie.Find(s => s.Status == (byte)OrderPackageStatus.ChinaExport);
                        if (chinaExport != null)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, chinaExport.CreateDate.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, "", ExcelHorizontalAlignment.Left, true);
                        }

                        //trong kho VN
                        var inStock = packageHistorie.Find(s => s.Status == (byte)OrderPackageStatus.InStock);
                        if (inStock != null)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, inStock.CreateDate.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, "", ExcelHorizontalAlignment.Left, true);
                        }

                        //đi giao hàng
                        var goingDelivery = packageHistorie.Find(s => s.Status == (byte)OrderPackageStatus.GoingDelivery);
                        if (goingDelivery != null)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, goingDelivery.CreateDate.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, "", ExcelHorizontalAlignment.Left, true);
                        }
                        //hoàn thành đơn
                        var completed = packageHistorie.Find(s => s.Status == (byte)OrderPackageStatus.Completed);
                        if (completed != null)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, completed.CreateDate.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, "", ExcelHorizontalAlignment.Left, true);
                        }

                        //trạng thái kiện
                        ExcelHelper.CreateCellTable(sheet, no, col++, EnumHelper.GetEnumDescription<OrderPackageStatus>(package.Status), ExcelHorizontalAlignment.Left, true);
                        ////cước vận chuyển thêm
                        //ExcelHelper.CreateCellTable(sheet, no, col, "", ExcelHorizontalAlignment.Left, true);
                        //col++;
                        ////cước đóng gỗ
                        //ExcelHelper.CreateCellTable(sheet, no, col, "", ExcelHorizontalAlignment.Left, true);
                        //col++;
                        //vị trí kho
                        ExcelHelper.CreateCellTable(sheet, no, col, package.CurrentLayoutName, ExcelHorizontalAlignment.Left, true);
                        no++;
                    }
                }

                ExcelHelper.CreateColumnAutofit(sheet, 1, col);
                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"DanhSachKien_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }
    }
}