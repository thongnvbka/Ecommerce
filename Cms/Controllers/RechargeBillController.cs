using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Library.DbContext.Entities;
using Library.Models;
using System;
using System.Collections.Generic;
using Library.ViewModels;
using Common.Helper;
using Common.Emums;
using System.ComponentModel;
using System.Globalization;
using AutoMapper;
using Cms.Attributes;
using Common.Constant;
using Common.MailHelper;
using OfficeOpenXml;
using System.Drawing;
using System.Runtime.ExceptionServices;
using OfficeOpenXml.Style;
using Cms.Helpers;
using Library.DbContext.Results;
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    [Authorize]
    public class RechargeBillController : BaseController
    {
        [CheckPermission(EnumAction.Export, EnumPage.RechargeBill)]
        #region Thống kê
        public async Task<ActionResult> RechargeExcelReport(RechargeBillSearchModal searchModal)
        {
            List<RechargeBill> rechargeBillModal;
            long totalRecord;
            int page = 1;
            int pageSize = Int32.MaxValue;

            var dateStart = new DateTime();
            var dateEnd = new DateTime();

            if (searchModal == null)
            {
                searchModal = new RechargeBillSearchModal();
            }

            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();


            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                dateStart = DateTime.Parse(searchModal.DateStart);
                dateEnd = DateTime.Parse(searchModal.DateEnd);

                rechargeBillModal = await UnitOfWork.RechargeBillRepo.FindAsync(
                    out totalRecord,
                    x =>
                        (x.Code.Contains(searchModal.Keyword) || x.CustomerEmail.Contains(searchModal.Keyword) ||
                         x.CustomerName.Contains(searchModal.Keyword) || x.CustomerCode.Contains(searchModal.Keyword) ||
                         x.CustomerEmail.Contains(searchModal.Keyword))
                        && !x.IsDelete
                        && (searchModal.Status == -1 || x.Status == searchModal.Status)
                        && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
                        && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId)
                        && x.Created >= dateStart && x.Created <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            else
            {
                rechargeBillModal = await UnitOfWork.RechargeBillRepo.FindAsync(
                    out totalRecord,
                    x =>
                        (x.Code.Contains(searchModal.Keyword) || x.CustomerEmail.Contains(searchModal.Keyword) ||
                         x.CustomerName.Contains(searchModal.Keyword) || x.CustomerCode.Contains(searchModal.Keyword) ||
                         x.CustomerEmail.Contains(searchModal.Keyword))
                        && !x.IsDelete
                        && (searchModal.Status == -1 || x.Status == searchModal.Status)
                        && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
                        && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId),
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
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Date Created", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Transaction code", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Status", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Transaction type", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Deposit (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Withdraw (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Opening balance (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Ending balance (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Customer", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Account entry", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Sheet creator", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Sheet approved by", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Note", ExcelHorizontalAlignment.Center, true, colorHeader);


                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "LIS OF CUSTOMER E-WALLET DEPOSIT/WITHDRAWAL SLIP", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
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

                decimal Increase = 0;
                decimal Diminishe = 0;
                var Type = string.Empty;
                var Status = string.Empty;

                if (rechargeBillModal.Any())
                {
                    foreach (var w in rechargeBillModal)
                    {
                        if (w.Type == (byte)RechargeBillType.Increase)
                        {
                            Increase = w.CurrencyFluctuations;
                            Diminishe = 0;

                            Type = "Deposit e-wallet";
                        }
                        if (w.Type == (byte)RechargeBillType.Diminishe)
                        {
                            Diminishe = w.CurrencyFluctuations;
                            Increase = 0;

                            Type = "Deducting e-wallet";
                        }
                        if (w.Status == (byte)RechargeBillStatus.New)
                        {
                            Status = "Waiting for approval";
                        }
                        if (w.Status == (byte)RechargeBillStatus.Approved)
                        {
                            Status = "Approved";
                        }
                        //====================================================

                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Code, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, Type, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, Status, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, Increase, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, Diminishe, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.CurencyStart, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.CurencyEnd, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.CustomerEmail, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.TreasureName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.UserName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.UserApprovalName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Note, ExcelHorizontalAlignment.Right, true);


                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;


                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"Ewallet_Report{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchModal"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.RechargeBill)]
        public async Task<JsonResult> GetAllRechargeBillList(int page, int pageSize, RechargeBillSearchModal searchModal)
        {
            // Tính Total money thu/ chi theo danh sách
            decimal collectMoney = 0;
            decimal returnMoney = 0;
            decimal minusMoney = 0;

            List<RechargeBill> rechargeBillModal;
            long totalRecord;

            if (searchModal == null)
            {
                searchModal = new RechargeBillSearchModal();
            }

            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);
                searchModal.Keyword = RemoveCode(searchModal.Keyword);
                rechargeBillModal = await UnitOfWork.RechargeBillRepo.FindAsync(
                    out totalRecord,
                    x =>
                        (x.Code.Contains(searchModal.Keyword) || x.OrderCode == searchModal.Keyword || x.CustomerEmail.Contains(searchModal.Keyword) ||
                         x.CustomerName.Contains(searchModal.Keyword) || x.CustomerCode.Contains(searchModal.Keyword) ||
                         x.CustomerEmail.Contains(searchModal.Keyword) || x.Note.Contains(searchModal.Keyword))
                        && !x.IsDelete
                        && (searchModal.Status == -1 || x.Status == searchModal.Status)
                        && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
                        && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId)
                        && (searchModal.CurrencyFluctuations == null || x.CurrencyFluctuations == searchModal.CurrencyFluctuations)
                        && (searchModal.CustomerWalletId == null || searchModal.CustomerWalletId == 0 || x.TreasureId == searchModal.CustomerWalletId)
                        && x.Created >= dateStart && x.Created <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );

                var rechargeBillModalMoney = UnitOfWork.DbContext.RechargeBill.Where(
                    x =>
                        (x.Code.Contains(searchModal.Keyword) || x.OrderCode == searchModal.Keyword || x.CustomerEmail.Contains(searchModal.Keyword) ||
                         x.CustomerName.Contains(searchModal.Keyword) || x.CustomerCode.Contains(searchModal.Keyword) ||
                         x.CustomerEmail.Contains(searchModal.Keyword) || x.Note.Contains(searchModal.Keyword))
                        && !x.IsDelete
                        && (searchModal.Status == -1 || x.Status == searchModal.Status)
                        && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
                        && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId)
                        && (searchModal.CurrencyFluctuations == null || x.CurrencyFluctuations == searchModal.CurrencyFluctuations)
                        && (searchModal.CustomerWalletId == null || searchModal.CustomerWalletId == 0 || x.TreasureId == searchModal.CustomerWalletId)
                        && x.Created >= dateStart && x.Created <= dateEnd
                ).Select(x => new { x.Increase, x.Diminishe }).ToList();

                collectMoney = rechargeBillModalMoney.Sum(x => x.Increase ?? 0);
                returnMoney = rechargeBillModalMoney.Sum(x => x.Diminishe ?? 0);
                minusMoney = collectMoney - returnMoney;
            }
            else
            {
                rechargeBillModal = await UnitOfWork.RechargeBillRepo.FindAsync(
                    out totalRecord,
                    x =>
                        (x.Code.Contains(searchModal.Keyword) || x.OrderCode == searchModal.Keyword || x.CustomerEmail.Contains(searchModal.Keyword) ||
                         x.CustomerName.Contains(searchModal.Keyword) || x.CustomerCode.Contains(searchModal.Keyword) ||
                         x.CustomerEmail.Contains(searchModal.Keyword) || x.Note.Contains(searchModal.Keyword))
                        && !x.IsDelete
                        && (searchModal.Status == -1 || x.Status == searchModal.Status)
                        && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
                        && (searchModal.CurrencyFluctuations == null || x.CurrencyFluctuations == searchModal.CurrencyFluctuations)
                        && (searchModal.CustomerWalletId == null || searchModal.CustomerWalletId == 0 || x.TreasureId == searchModal.CustomerWalletId)
                        && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId),

                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );

                var rechargeBillModalMoney = UnitOfWork.DbContext.RechargeBill.Where(
                    x =>
                        (x.Code.Contains(searchModal.Keyword) || x.OrderCode == searchModal.Keyword || x.CustomerEmail.Contains(searchModal.Keyword) ||
                         x.CustomerName.Contains(searchModal.Keyword) || x.CustomerCode.Contains(searchModal.Keyword) ||
                         x.CustomerEmail.Contains(searchModal.Keyword) || x.Note.Contains(searchModal.Keyword))
                        && !x.IsDelete
                        && (searchModal.Status == -1 || x.Status == searchModal.Status)
                        && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
                        && (searchModal.CurrencyFluctuations == null || x.CurrencyFluctuations == searchModal.CurrencyFluctuations)
                        && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId)
                        && (searchModal.CustomerWalletId == null || searchModal.CustomerWalletId == 0 || x.TreasureId == searchModal.CustomerWalletId)
                ).Select(x => new { x.Increase, x.Diminishe }).ToList();

                collectMoney = rechargeBillModalMoney.Sum(x => x.Increase ?? 0);
                returnMoney = rechargeBillModalMoney.Sum(x => x.Diminishe ?? 0);
                minusMoney = collectMoney - returnMoney;
            }

            return Json(new { totalRecord, rechargeBillModal, collectMoney, returnMoney, minusMoney }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rechargeBillId"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.RechargeBill)]
        public async Task<JsonResult> GetRechargeBillDetail(int rechargeBillId)
        {
            var result = true;

            var rechargeBillModal =
                await UnitOfWork.RechargeBillRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == rechargeBillId);
            if (rechargeBillModal == null)
            {
                result = false;
            }

            return Json(new { result, rechargeBillModal }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRechargeBillSearchData()
        {
            var listType = new List<dynamic>() { new { Text ="All", Value = -1 } };
            var listCustomer = new List<dynamic>() { new { Text ="All", Value = -1 } };
            var listStatus = new List<dynamic>() { new { Text ="All", Value = -1 } };

            // Lấy kiểu giao dịch với ví
            foreach (RechargeBillType rechargeBillType in Enum.GetValues(typeof(RechargeBillType)))
            {
                if (rechargeBillType >= 0)
                {
                    listType.Add(
                        new
                        {
                            Text = rechargeBillType.GetAttributeOfType<DescriptionAttribute>().Description,
                            Value = (int)rechargeBillType
                        });
                }
            }

            // Lấy các trạng thái Status
            foreach (RechargeBillStatus rechargeBillStatus in Enum.GetValues(typeof(RechargeBillStatus)))
            {
                if (rechargeBillStatus >= 0)
                {
                    listStatus.Add(
                        new
                        {
                            Text = rechargeBillStatus.GetAttributeOfType<DescriptionAttribute>().Description,
                            Value = (int)rechargeBillStatus
                        });
                }
            }

            return Json(new { listStatus, listType, listCustomer }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Tạo mới giao dịch nạp/trừ tiền khách hàng
        /// POST: /RechargeBill/CreateNewRechargeBill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.RechargeBill)]
        public async Task<JsonResult> CreateNewRechargeBill(RechargeBillMeta model)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }

            //1. Kiểm tra thông tin khách hàng
            var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.CustomerId);
            if (customer == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.CustomerIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                model.CustomerId = customer.Id;
                model.CustomerEmail = customer.Email;
                model.CustomerCode = customer.Code;
                model.CustomerName = customer.FullName;
                model.CustomerPhone = customer.Phone;
                model.CustomerAddress = customer.Address;
            }

            //2. Kiểm tra định khoản ví điện tử
            var customerWallet = UnitOfWork.CustomerWalletRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.TreasureId);
            if (customerWallet == null)
            {
                return Json(new { status = Result.Failed, msg = "The electronic wallet account does not exist, please check again !" },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                model.TreasureId = customerWallet.Id;
                model.TreasureIdd = customerWallet.Idd;
                model.TreasureName = customerWallet.Name;
            }

            //3. Chặn không cho trừ quá Balances trong Account
            if (model.Type == 1 && (model.CurrencyFluctuations > customer.BalanceAvalible))
            {
                return Json(new { status = Result.Failed, msg = "Amount of money to withdraw is greater than the balance !" },
                    JsonRequestBehavior.AllowGet);
            }

            if (customerWallet.Operator == true)
            {
                //Nạp tiền ví điện tử
                model.Type = (byte)RechargeBillType.Increase;
                model.Increase = model.CurrencyFluctuations;
                model.Diminishe = 0;
            }
            if (customerWallet.Operator == false)
            {
                //Nạp tiền ví điện tử
                model.Type = (byte)RechargeBillType.Diminishe;
                model.Increase = 0;
                model.Diminishe = model.CurrencyFluctuations;
            }

            //=========================== Khởi tạo Model để lưu
            model.Status = 0;               // Mới khởi tạo, chờ duyệt
            model.Code = string.Empty;      // Khởi tạo code RechargeBill

            //Thông tin người tạo phiếu
            model.UserId = UserState.UserId;
            model.UserName = UserState.UserName;

            //Khởi tạo số dư ban đầu
            model.CurencyStart = null;
            model.CurencyEnd = null;

            //Khởi tạo thêm
            model.IsAutomatic = false;

            // Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lưu thông tin phiếu nạp/trừ tiền khách hàng
                    var rechargeBillData = Mapper.Map<RechargeBill>(model);

                    UnitOfWork.RechargeBillRepo.Add(rechargeBillData);
                    UnitOfWork.RechargeBillRepo.Save();

                    var rechargeBillOfDay = UnitOfWork.RechargeBillRepo.Count(x =>
                        x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                        x.Created.Day == DateTime.Now.Day && x.Id <= rechargeBillData.Id);

                    rechargeBillData.Code = $"{rechargeBillOfDay}{DateTime.Now:ddMMyy}";
                    UnitOfWork.RechargeBillRepo.Save();

                    // Bắn Notification
                    var office = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => x.Type == (byte)OfficeType.Accountancy);
                    var listUser = await UnitOfWork.UserRepo.GetUserToOffice(0, 1, office.IdPath, office.Id);

                    foreach (var user in listUser.Where(x => x.Id != UserState.UserId).ToList())
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient(user.Id, "Request to approve e-wallet slip no. #" + model.Code, EnumNotifyType.Info, "Staff: '" + UserState.FullName + "' has made a examining request to approve the customer  e-wallet deposit/withdrawal slip: " + rechargeBillData.CustomerEmail + ". Transaction code number: #" + rechargeBillData.Code);
                    }

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

            return Json(new { status = Result.Succeed, msg = ConstantMessage.CreateNewRechargeBillIsSuccess },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Edit thông tin phiếu nạp/trừ tiền khách hàng
        /// POST: /RechargeBill/EditRechargeBill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.Update, EnumPage.RechargeBill)]
        public JsonResult EditRechargeBill(RechargeBillMeta model)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }

            //1. Kiểm tra thông tin RechargeBill 
            var rechargeBillDetail = UnitOfWork.RechargeBillRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.Id);
            if (rechargeBillDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra lại thông tin định khoản ví điện tử
            var customerWallet = UnitOfWork.CustomerWalletRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.TreasureId);
            if (customerWallet == null)
            {
                return Json(new { status = Result.Failed, msg = " Account entry of e-wallet does not exist, please check again !" },
                    JsonRequestBehavior.AllowGet);
            }

            //3. Chặn nếu trạng thái phiếu nạp/trừ tiền đã được duyệt thì ko thể Edit
            if (rechargeBillDetail.Status == 1)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.EditRechargeBillStatusIsNotImpossible },
                    JsonRequestBehavior.AllowGet);
            }
            if (customerWallet.Operator == true)
            {
                //Nạp tiền ví điện tử
                model.Type = (byte)RechargeBillType.Increase;
                model.Increase = model.CurrencyFluctuations;
                model.Diminishe = 0;
            }
            if (customerWallet.Operator == false)
            {
                //Nạp tiền ví điện tử
                model.Type = (byte)RechargeBillType.Diminishe;
                model.Increase = 0;
                model.Diminishe = model.CurrencyFluctuations;
            }

            //4. Kiểm tra thông tin khách hàng
            var customer =
                UnitOfWork.CustomerRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && x.Id == model.CustomerId);
            if (customer == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.CustomerIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }

            //5. Chặn không cho trừ quá Balances trong Account
            if (rechargeBillDetail.Type == 1)
            {
                if (rechargeBillDetail.Status != (byte) RechargeBillStatus.Approved)
                {
                    if (model.CurrencyFluctuations > customer.BalanceAvalible)
                    {
                        return Json(new { status = Result.Failed, msg = ConstantMessage.CurrencyFluctuationsImpossible },
                            JsonRequestBehavior.AllowGet);
                    }
                }
            }

            //6. Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lưu thông tin phiếu nạp/trừ tiền khách hàng
                    Mapper.Map(model, rechargeBillDetail);

                    // Lấy lại thông tin để thực hiện lưu
                    rechargeBillDetail.CustomerId = customer.Id;
                    rechargeBillDetail.CustomerName = customer.FullName;
                    rechargeBillDetail.CustomerCode = customer.Code;
                    rechargeBillDetail.CustomerEmail = customer.Email;
                    rechargeBillDetail.CustomerPhone = customer.Phone;
                    rechargeBillDetail.CustomerAddress = customer.Address;

                    // Lấy lại thông tin định khoản ví điện tử
                    rechargeBillDetail.TreasureId = customerWallet.Id;
                    rechargeBillDetail.TreasureIdd = customerWallet.Idd;
                    rechargeBillDetail.TreasureName = customerWallet.Name;

                    // Cập nhật lại ngày
                    rechargeBillDetail.LastUpdated = DateTime.Now;

                    //Lưu xuống Database
                    UnitOfWork.RechargeBillRepo.Save();

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

            return Json(new { status = Result.Succeed, msg = ConstantMessage.EditRechargeBillIsSuccess },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xóa phiếu giao dịch nạp/trừ tiền ví điện tử khách hàng
        /// POST: /RechargeBill/DeleteRechargeBill
        /// </summary>
        /// <param name="rechargeBillId"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.RechargeBill)]
        public JsonResult DeleteRechargeBill(int rechargeBillId)
        {
            // Kiểm tra thông tin RechargeBill 
            var rechargeBillDetail =
                UnitOfWork.RechargeBillRepo.FirstOrDefault(x => !x.IsDelete && x.Id == rechargeBillId);
            if (rechargeBillDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }

            // Nếu phiếu đã được duyệt thì không cho xóa.
            if (rechargeBillDetail.Status == 1)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.BillIsApproval },
                    JsonRequestBehavior.AllowGet);
            }

            // Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lấy lại thông tin để thực hiện lưu
                    rechargeBillDetail.IsDelete = true;

                    //Lưu xuống Database
                    UnitOfWork.RechargeBillRepo.Update(rechargeBillDetail);
                    UnitOfWork.RechargeBillRepo.Save();

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

            return Json(new { status = Result.Succeed, msg = ConstantMessage.DeleteRechargeBillIsSuccess },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Duyệt phiếu Nạp/Trừ tiền ví điện tử khách hàng
        /// POST: /RechargeBill/ApprovalRechargeBill
        /// </summary>
        /// <param name="rechargeBillId"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.Approvel, EnumPage.RechargeBill)]
        public JsonResult ApprovalRechargeBill(int rechargeBillId)
        {
            //1. Kiểm tra thông tin phiếu
            var rechargeBillDetail =
                UnitOfWork.RechargeBillRepo.FirstOrDefault(x => !x.IsDelete && x.Id == rechargeBillId);
            if (rechargeBillDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }
            if (rechargeBillDetail.Status == (byte)RechargeBillStatus.Approved)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }

            //2. Lấy thông tin khách hàng trong phiếu nạp/trừ tiền để thực hiện
            var customerId = rechargeBillDetail.CustomerId;
            var customerDetail = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == customerId);
            if (customerDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }

            //3. Nạp/Trừ tiền ví điện tử khách hàng
            // Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //====== Cập nhật lại thông tin phiếu nạp/trừ tiền ví điện tử
                    rechargeBillDetail.Status = (byte)RechargeBillStatus.Approved;
                    // Lưu thông tin vào phiếu RechargeBill
                    rechargeBillDetail.CurencyStart = customerDetail.BalanceAvalible;

                    if (rechargeBillDetail.Type == (byte)RechargeBillType.Increase)
                    {
                        rechargeBillDetail.CurencyStart = customerDetail.BalanceAvalible;
                        rechargeBillDetail.CurencyEnd = customerDetail.BalanceAvalible + rechargeBillDetail.CurrencyFluctuations;
                    }
                    if (rechargeBillDetail.Type == (byte)RechargeBillType.Diminishe)
                    {
                        rechargeBillDetail.CurencyStart = customerDetail.BalanceAvalible;
                        rechargeBillDetail.CurencyEnd = customerDetail.BalanceAvalible - rechargeBillDetail.CurrencyFluctuations;
                    }

                    // Người duyệt
                    rechargeBillDetail.UserApprovalId = UserState.UserId;
                    rechargeBillDetail.UserApprovalName = UserState.UserName;

                    UnitOfWork.RechargeBillRepo.Update(rechargeBillDetail);
                    UnitOfWork.RechargeBillRepo.Save();

                    //======= Cập nhật lại thông tin số dư cho khách hàng
                    // Nếu là nạp tiền ví điện tử
                    if (rechargeBillDetail.Type == (byte)RechargeBillType.Increase)
                    {
                        customerDetail.BalanceAvalible += rechargeBillDetail.CurrencyFluctuations;  // Cộng tiền vào Balances
                    }

                    // Nếu là trừ tiền ví điện tử
                    if (rechargeBillDetail.Type == (byte)RechargeBillType.Diminishe)
                    {
                        // Chặn không cho trừ quá Balances trong Account
                        if (rechargeBillDetail.CurrencyFluctuations > customerDetail.BalanceAvalible)
                        {
                            return
                                Json(
                                    new { status = Result.Failed, msg = ConstantMessage.CurrencyFluctuationsImpossible },
                                    JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            customerDetail.BalanceAvalible -= rechargeBillDetail.CurrencyFluctuations;  // Trừ tiền vào Balances
                        }
                    }

                    // Lưu thông tin vào ví điện tử khách hàng
                    UnitOfWork.CustomerRepo.Update(customerDetail);
                    UnitOfWork.CustomerRepo.Save();

                    // Bắn Notification
                    NotifyHelper.CreateAndSendNotifySystemToClient((int)rechargeBillDetail.UserId, "[Notification]:Bill of Payments / Deductible E-Wallet: #" + rechargeBillDetail.Code + " Has been approved", EnumNotifyType.Info, "Staff: '" + UserState.FullName + "' Make approval for the Charge/Subtract funds. Transaction code number: #" + rechargeBillDetail.Code);

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

            try
            {
                // Gửi thông báo Notification cho khách hàng
                var notification = new Notification()
                {
                    SystemId = customerDetail.SystemId,
                    SystemName = customerDetail.SystemName,
                    CustomerId = customerDetail.Id,
                    CustomerName = customerDetail.FullName,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    OrderType = 4, // Thông báo giành cho thay đổi ví kế toán
                    IsRead = false,
                    Title = "The balance of your e-wallet has changed",
                    Description = "The balance of your e-wallet has changed: " + rechargeBillDetail.Note
                };

                UnitOfWork.NotificationRepo.Add(notification);
                UnitOfWork.NotificationRepo.Save();

                ////4. Gửi thư thông báo cho khách hàng thông tin thay đổi số dư ví điện tử
                //var body = System.IO.File.ReadAllText(Server.MapPath("~/EmailTemplate/Accounting/rechargeInfo.html"));

                //body = body.Replace("{{Note}}", rechargeBillDetail.Note);
                //body = body.Replace("{{UserFullname}}", customerDetail.FullName);
                //body = body.Replace("{{CurrencyFluctuations}}", rechargeBillDetail.CurrencyFluctuations.ToString());
                //body = body.Replace("{{CurencyStart}}", rechargeBillDetail.CurencyStart.ToString());
                //body = body.Replace("{{CurencyEnd}}", rechargeBillDetail.CurencyEnd.ToString());
                //MailHelper.SendMail(customerDetail.Email, "The balance of your e-wallet has changed", body, false);
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.ApprovalRechargeBillIsSuccess }, JsonRequestBehavior.AllowGet);
        }
    }
}