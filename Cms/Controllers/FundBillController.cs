using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Globalization;
using Library.DbContext.Entities;
using Library.Models;
using System;
using System.Collections.Generic;
using Library.ViewModels;
using Common.Helper;
using Common.Emums;
using System.ComponentModel;
using Common.Constant;
using AutoMapper;
using Cms.Attributes;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Runtime.ExceptionServices;
using Library.UnitOfWork;
using Library.DbContext.Repositories;
using Cms.Helpers;
using Library.DbContext.Results;
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    [Authorize]
    public class FundBillController : BaseController
    {
        #region Thống kê
        [CheckPermission(EnumAction.Export, EnumPage.FundBill)]
        public async Task<ActionResult> FundBillExcelReport(FundBillSearchModal searchModal)
        {
            List<FundBill> fundBillModal;
            long totalRecord;
            int page = 1;
            int pageSize = Int32.MaxValue;
            var dateStart = new DateTime();
            var dateEnd = new DateTime();

            if (searchModal == null)
            {
                searchModal = new FundBillSearchModal();
            }

            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();
            var listFinanceFundId = UnitOfWork.DbContext.FinanceFunds.Where(x => !x.IsDelete && (x.IdPath == searchModal.FinanceFundIdPath || x.IdPath.StartsWith(searchModal.FinanceFundIdPath))).Select(x => x.Id).ToList();
            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                dateStart = DateTime.Parse(searchModal.DateStart);
                dateEnd = DateTime.Parse(searchModal.DateEnd);

                fundBillModal = await UnitOfWork.FundBillRepo.FindAsync(
                   out totalRecord,
                   x => (x.Code.Contains(searchModal.Keyword) || x.OrderCode.Contains(searchModal.Keyword) || x.SubjectEmail.Contains(searchModal.Keyword) || x.SubjectName.Contains(searchModal.Keyword) || x.SubjectCode.Contains(searchModal.Keyword) || x.SubjectPhone.Contains(searchModal.Keyword) || x.CurrencyFluctuations.ToString().Contains(searchModal.Keyword) || x.Note.Contains(searchModal.Keyword))
                       && !x.IsDelete
                       && (searchModal.FinanceFundId == null || searchModal.FinanceFundId == 0 || listFinanceFundId.Contains(x.FinanceFundId ?? 0))
                       && (searchModal.UserId == null || searchModal.UserId == 0 || x.UserId == searchModal.UserId)
                       && (searchModal.TreasureId == null || searchModal.TreasureId == 0 || x.TreasureId == searchModal.TreasureId)
                       && (searchModal.Status == -1 || x.Status == searchModal.Status)
                       && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
                       && (searchModal.AccountantSubjectId == -1 || x.AccountantSubjectId == searchModal.AccountantSubjectId)
                       && x.LastUpdated >= dateStart && x.LastUpdated <= dateEnd,
                   x => x.OrderByDescending(y => y.Created),
                   page,
                   pageSize
                );
            }
            else
            {
                fundBillModal = await UnitOfWork.FundBillRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.OrderCode.Contains(searchModal.Keyword) || x.SubjectEmail.Contains(searchModal.Keyword) || x.SubjectName.Contains(searchModal.Keyword) || x.SubjectCode.Contains(searchModal.Keyword) || x.SubjectPhone.Contains(searchModal.Keyword) || x.CurrencyFluctuations.ToString().Contains(searchModal.Keyword) || x.Note.Contains(searchModal.Keyword))
                        && !x.IsDelete
                        && (searchModal.FinanceFundId == null || searchModal.FinanceFundId == 0 || listFinanceFundId.Contains(x.FinanceFundId ?? 0))
                        && (searchModal.UserId == null || searchModal.UserId == 0 || x.UserId == searchModal.UserId)
                        && (searchModal.TreasureId == null || searchModal.TreasureId == 0 || x.TreasureId == searchModal.TreasureId)
                        && (searchModal.Status == -1 || x.Status == searchModal.Status)
                        && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
                        && (searchModal.AccountantSubjectId == -1 || x.AccountantSubjectId == searchModal.AccountantSubjectId),
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
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Incurring date", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Subject" , ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Transaction code", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Transaction type", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Incurred (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Opening balance (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Ending balance (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Entry", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Fund name", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Sheet creator" , ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Sheet approved by", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Order ID", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Order code", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Order type", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Note", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "DANH SÁCH PHIẾU NẠP/Subtract funds", new CustomExcelStyle
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

                if (fundBillModal.Any())
                {
                    foreach (var w in fundBillModal)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.LastUpdated.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.AccountantSubjectName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Code, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Type, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.CurrencyFluctuations, new CustomExcelStyle
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
                        ExcelHelper.CreateCellTable(sheet, no, col, w.TreasureName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.FinanceFundName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.UserName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.UserApprovalName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.OrderId, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.OrderCode, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.OrderType, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Note, ExcelHorizontalAlignment.Right, true);

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;


                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"Quarterly_Report{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }

        [CheckPermission(EnumAction.Export, EnumPage.FundBill)]
        public async Task<ActionResult> FinanceFundExcelReport()
        {
            List<FinanceFund> financeFundData;

            financeFundData = await UnitOfWork.FinaceFundRepo.FindAsync(x => !x.IsDelete);

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Daily_Report");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Fund name", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Manager", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Manager email", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Account representative", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Bank", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Branch", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Số tài khoản ngân hàng", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Balance", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "REPORT OF FUND BALANCE IN DAY ", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                var dateTime = DateTime.Now;

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"Date: {dateTime}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (financeFundData.Any())
                {
                    foreach (var w in financeFundData)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Name, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.UserFullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.UserEmail, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.UserFullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.NameBank, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Department, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.BankAccountNumber, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.Balance, new CustomExcelStyle
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

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"Report_Balance{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }

        }

        [CheckPermission(EnumAction.Export, EnumPage.FundBill)]
        public ActionResult FinanceFundBalanceExcelReport()
        {
            var financeFundData = UnitOfWork.FinaceFundRepo.Entities.Where(s => !s.IsDelete).OrderBy(x => x.IdPath).ToList();

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Daily_Report");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                var arrayCount = financeFundData.Select(x => x.IdPath.Split('.').Count()).OrderByDescending(x => x).FirstOrDefault();

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, row, arrayCount, "Fund name", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, arrayCount + 1, "Balance", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, arrayCount + 1, "REPORT OF FUND BALANCE IN DAY ", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                var dateTime = DateTime.Now;

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, arrayCount + 1, $"Date: {dateTime}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;
                if (financeFundData.Any())
                {
                    foreach (var w in financeFundData)
                    {
                        col = 1;
                        var array = w.IdPath.Split('.');
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        if (array.Count() != 2)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col + 1, no, array.Count() - 1, "", ExcelHorizontalAlignment.Right, true);
                        }

                        col = array.Count();
                        if (w.ParentId == 0)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.Name.ToUpper(), new CustomExcelStyle
                            {
                                IsMerge = false,
                                IsBold = true,
                                FontSize = 15,
                                Border = ExcelBorderStyle.Thin,
                                HorizontalAlign = ExcelHorizontalAlignment.Left
                            });
                        }
                        else
                        {
                            if (array.Count() < 4)
                            {
                                ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.Name, new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = true,
                                    FontSize = 13,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Left
                                });
                            }
                            else
                            {
                                ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.Name, ExcelHorizontalAlignment.Left, true);
                            }

                        }

                        ExcelHelper.CreateCellTable(sheet, no, arrayCount + 1, no, arrayCount + 1, financeFundData.Where(x => x.IdPath.Contains(w.IdPath)).Sum(x => x.Balance), new CustomExcelStyle
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

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"Report_Balance{DateTime.Now:yyyyMMddHHmmss}.xlsx");
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
        [CheckPermission(EnumAction.View, EnumPage.FundBill)]
        public async Task<JsonResult> GetAllFundBillList(int page, int pageSize, FundBillSearchModal searchModal)
        {

            // Tính Total money thu/ chi theo danh sách
            decimal collectMoney = 0;
            decimal returnMoney = 0;
            decimal minusMoney = 0;
            List<FundBill> fundBillModal;
            long totalRecord;

            if (searchModal == null)
            {
                searchModal = new FundBillSearchModal();
            }

            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();
            var listFinanceFundId = UnitOfWork.DbContext.FinanceFunds.Where(x => !x.IsDelete && (x.IdPath == searchModal.FinanceFundIdPath || x.IdPath.StartsWith(searchModal.FinanceFundIdPath))).Select(x => x.Id).ToList();
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);

                fundBillModal = await UnitOfWork.FundBillRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.OrderCode == searchModal.Keyword
                    || x.FinanceFundUserEmail.Contains(searchModal.Keyword)
                    || x.SubjectEmail.Contains(searchModal.Keyword) || x.Note.Contains(searchModal.Keyword))
                        && !x.IsDelete
                        && (searchModal.FinanceFundId == null || searchModal.FinanceFundId == 0 || listFinanceFundId.Contains(x.FinanceFundId ?? 0))
                        && (searchModal.UserId == null || searchModal.UserId == 0 || x.UserId == searchModal.UserId)
                        && (searchModal.TreasureId == null || searchModal.TreasureId == 0 || x.TreasureId == searchModal.TreasureId)
                        && (searchModal.Status == -1 || x.Status == searchModal.Status)
                        && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
                        && (searchModal.CurrencyFluctuations == null || x.CurrencyFluctuations == searchModal.CurrencyFluctuations.Value )
                        //&& (searchModal.FinanceFundId == -1 || x.FinanceFundId == searchModal.FinanceFundId)
                        && (searchModal.AccountantSubjectId == -1 || x.AccountantSubjectId == searchModal.AccountantSubjectId)
                        && x.LastUpdated >= dateStart && x.LastUpdated <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );

                 var fundBillModalMoney = UnitOfWork.DbContext.FundBill.Where(
                    x => (x.Code.Contains(searchModal.Keyword) || x.OrderCode == searchModal.Keyword
                    || x.FinanceFundUserEmail.Contains(searchModal.Keyword)
                    || x.SubjectEmail.Contains(searchModal.Keyword) || x.Note.Contains(searchModal.Keyword))
                        && !x.IsDelete
                        && (searchModal.FinanceFundId == null || searchModal.FinanceFundId == 0 || listFinanceFundId.Contains(x.FinanceFundId ?? 0))
                        && (searchModal.UserId == null || searchModal.UserId == 0 || x.UserId == searchModal.UserId)
                        && (searchModal.TreasureId == null || searchModal.TreasureId == 0 || x.TreasureId == searchModal.TreasureId)
                        && (searchModal.Status == -1 || x.Status == searchModal.Status)
                        && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
                        && (searchModal.CurrencyFluctuations == null || x.CurrencyFluctuations == searchModal.CurrencyFluctuations)
                        //&& (searchModal.FinanceFundId == -1 || x.FinanceFundId == searchModal.FinanceFundId)
                        && (searchModal.AccountantSubjectId == -1 || x.AccountantSubjectId == searchModal.AccountantSubjectId)
                        && x.LastUpdated >= dateStart && x.LastUpdated <= dateEnd
                ).Select(x=> new {x.Increase, x.Diminishe }).ToList();
                collectMoney = fundBillModalMoney.Sum(x => x.Increase ?? 0);
                returnMoney = fundBillModalMoney.Sum(x => x.Diminishe ?? 0);
                minusMoney = collectMoney - returnMoney;
            }
            else
            {
                fundBillModal = await UnitOfWork.FundBillRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.OrderCode == searchModal.Keyword
                    || x.FinanceFundUserEmail.Contains(searchModal.Keyword)
                    || x.SubjectEmail.Contains(searchModal.Keyword) || x.Note.Contains(searchModal.Keyword))
                        && !x.IsDelete
                        && (searchModal.FinanceFundId == null || searchModal.FinanceFundId == 0 || listFinanceFundId.Contains(x.FinanceFundId ?? 0))
                        && (searchModal.UserId == null || searchModal.UserId == 0 || x.UserId == searchModal.UserId)
                        && (searchModal.TreasureId == null || searchModal.TreasureId == 0 || x.TreasureId == searchModal.TreasureId)
                        && (searchModal.Status == -1 || x.Status == searchModal.Status)
                        && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
                        && (searchModal.CurrencyFluctuations == null || x.CurrencyFluctuations == searchModal.CurrencyFluctuations)
                        //&& (searchModal.FinanceFundId == -1 || x.FinanceFundId == searchModal.FinanceFundId)
                        && (searchModal.AccountantSubjectId == -1 || x.AccountantSubjectId == searchModal.AccountantSubjectId),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );

                var fundBillModalMoney = UnitOfWork.DbContext.FundBill.Where(
                    x => (x.Code.Contains(searchModal.Keyword) || x.OrderCode == searchModal.Keyword
                    || x.FinanceFundUserEmail.Contains(searchModal.Keyword)
                    || x.SubjectEmail.Contains(searchModal.Keyword) || x.Note.Contains(searchModal.Keyword))
                        && !x.IsDelete
                        && (searchModal.FinanceFundId == null || searchModal.FinanceFundId == 0 || listFinanceFundId.Contains(x.FinanceFundId ?? 0))
                        && (searchModal.UserId == null || searchModal.UserId == 0 || x.UserId == searchModal.UserId)
                        && (searchModal.TreasureId == null || searchModal.TreasureId == 0 || x.TreasureId == searchModal.TreasureId)
                        && (searchModal.Status == -1 || x.Status == searchModal.Status)
                        && (searchModal.TypeId == -1 || x.Type == searchModal.TypeId)
                        && (searchModal.CurrencyFluctuations == null || x.CurrencyFluctuations == searchModal.CurrencyFluctuations)
                        //&& (searchModal.FinanceFundId == -1 || x.FinanceFundId == searchModal.FinanceFundId)
                        && (searchModal.AccountantSubjectId == -1 || x.AccountantSubjectId == searchModal.AccountantSubjectId)
                ).Select(x => new { x.Increase, x.Diminishe }).ToList();
                collectMoney = fundBillModalMoney.Sum(x => x.Increase ?? 0);
                returnMoney = fundBillModalMoney.Sum(x => x.Diminishe ?? 0);
                minusMoney = collectMoney - returnMoney;
            }

            return Json(new { totalRecord, fundBillModal, collectMoney, returnMoney, minusMoney }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="funBillId"></param>
        /// <returns></returns>
        [HttpPost]
        //[CheckPermission(EnumAction.View, EnumPage.FundBill)]
        public async Task<JsonResult> GetFundBillDetail(int funBillId)
        {
            var result = true;

            var fundBillModal = await UnitOfWork.FundBillRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Id == funBillId);
            if (fundBillModal == null)
            {
                result = false;
            }

            return Json(new { result, fundBillModal }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFundBillSearchData()
        {
            var listType = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listFinanceFund = new List<SearchMeta>();
            var listAccountantSubject = new List<SearchMeta>();
            var listStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };

            // Lấy kiểu giao dịch với quỹ
            foreach (FundBillType funBillType in Enum.GetValues(typeof(FundBillType)))
            {
                if (funBillType >= 0)
                {
                    listType.Add(new { Text = funBillType.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)funBillType });
                }
            }

            // Lấy danh sách đối tượng quỹ
            // TODO: Cần lấy danh sách từ bảng FinanceFund
            listFinanceFund.Add(new SearchMeta() { Text = "- All -", Value = -1 });


            // Lấy danh sách các Type đối tượng quỹ
            // TODO: Cần lấy danh sách từ bảng AccountantSubject
            var accountantSubject = UnitOfWork.AccountantSubjectRepo.FindAsNoTracking(x => x.Id > 0).ToList();
            var tempaccountantSubjectList = from p in accountantSubject
                                            select new SearchMeta() { Text = p.SubjectName, Value = p.Id };
            listAccountantSubject.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listAccountantSubject.AddRange(tempaccountantSubjectList.ToList());


            // Lấy các trạng thái Status
            foreach (FundBillStatus funBillStatus in Enum.GetValues(typeof(FundBillStatus)))
            {
                if (funBillStatus >= 0)
                {
                    listStatus.Add(new { Text = funBillStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)funBillStatus });
                }
            }

            return Json(new { listStatus, listType, listFinanceFund, listAccountantSubject }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.FundBill)]
        public async Task<ActionResult> CreateNewFundBill(FundBillMeta model)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            var customerDetail = new Customer();
            var staffDetail = new User();

            //1. Lấy thông tin Detail quỹ
            var financeFundDetail = UnitOfWork.FinaceFundRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.FinanceFundId);
            if (financeFundDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            //2. Lấy thông tin Detail Type định khoản
            var treasureDetail = UnitOfWork.TreasureRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.TreasureId);
            if (treasureDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.TreasureIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            //2.1. Nếu là nạp tiền vào quỹ
            if (treasureDetail.Operator == true)
            {
                model.Type = (byte)FundBillType.Increase;
                model.Increase = model.CurrencyFluctuations;
                model.Diminishe = 0;
            }

            //2.2. Chặn không cho trừ quá Balances tồn tại trong quỹ
            if (treasureDetail.Operator == false)
            {
                if (model.CurrencyFluctuations > financeFundDetail.Balance)
                {
                    return Json(new { status = Result.Failed, msg = ConstantMessage.CurrencyFluctuationsImpossible }, JsonRequestBehavior.AllowGet);
                }

                model.Type = (byte)FundBillType.Diminishe;
                model.Diminishe = model.CurrencyFluctuations;
                model.Increase = 0;
            }

            //3. Lấy thông tin Detail đối tượng nhập là khách hàng
            customerDetail = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.SubjectId && x.Code == model.SubjectCode);
            if (customerDetail == null)
            {
                staffDetail = UnitOfWork.UserRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.SubjectId);
                if (staffDetail == null)
                {
                    return Json(new { status = Result.Failed, msg = "You have not entered subject information or subject does not exist !" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (treasureDetail.Idd == (int)TreasureCustomerWallet.Withdrawals)
                    {
                        return Json(new { status = Result.Failed, msg = "This subject is not CUSTOMER, cannot perform e-wallet withdrawal!" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                //4. Lấy thông tin Detail Type đối tượng kế toán.
                var accountantSubjectDetail = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => !x.IsDelete && x.Id == customerDetail.TypeId);
                if (accountantSubjectDetail == null)
                {
                    return Json(new { status = Result.Failed, msg = "This account has not been assigned to any subject !" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.AccountantSubjectId = customerDetail.TypeId;
                    model.AccountantSubjectName = customerDetail.TypeName;
                }
            }

            //Thêm ngày 10/1/2017
            //5. Kiểm tra số tiền ví điện tử của khách nếu gặp trường hợp định khoản là rút ví điện tử
            if ((customerDetail != null) && (treasureDetail.Idd == (int)TreasureCustomerWallet.Withdrawals) && (model.CurrencyFluctuations > customerDetail.BalanceAvalible))
            {
                return Json(new { status = Result.Failed, msg = "This account does not have enough debit balance to carry on !" }, JsonRequestBehavior.AllowGet);
            }

            //=========================== Khởi tạo Model để lưu
            model.Status = 0;               // Mới khởi tạo, chờ duyệt
            model.Code = string.Empty;      // Khởi tạo code FundBill

            // Khởi tạo số dư ban đầu
            model.CurencyStart = null;
            model.CurencyEnd = null;

            var fundBillOfDay = UnitOfWork.FundBillRepo.Count(x =>
                x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                x.Created.Day == DateTime.Now.Day);
            model.Code = $"{fundBillOfDay}{DateTime.Now:ddMMyy}";

            //5. Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lưu thông tin phiếu nạp/trừ quỹ
                    var fundBillData = Mapper.Map<FundBill>(model);

                    // Map lại các thông tin để lưu
                    fundBillData.FinanceFundId = financeFundDetail.Id;
                    fundBillData.FinanceFundName = financeFundDetail.Name;
                    fundBillData.FinanceFundBankAccountNumber = financeFundDetail.BankAccountNumber;
                    fundBillData.FinanceFundDepartment = financeFundDetail.Department;
                    fundBillData.FinanceFundNameBank = financeFundDetail.NameBank;
                    fundBillData.FinanceFundUserFullName = financeFundDetail.UserFullName;
                    fundBillData.FinanceFundUserPhone = financeFundDetail.UserPhone;
                    fundBillData.FinanceFundUserEmail = financeFundDetail.UserEmail;

                    if (customerDetail != null)
                    {
                        fundBillData.SubjectId = customerDetail.Id;
                        fundBillData.SubjectCode = customerDetail.Code;
                        fundBillData.SubjectName = customerDetail.FullName;
                        fundBillData.SubjectPhone = customerDetail.Phone;
                        fundBillData.SubjectEmail = customerDetail.Email;

                        fundBillData.AccountantSubjectId = customerDetail.TypeId;
                        fundBillData.AccountantSubjectName = customerDetail.TypeName;
                    }
                    else
                    {
                        fundBillData.SubjectId = staffDetail.Id;
                        fundBillData.SubjectCode = "";
                        fundBillData.SubjectName = staffDetail.FullName;
                        fundBillData.SubjectPhone = staffDetail.Phone;
                        fundBillData.SubjectEmail = staffDetail.Email;

                        fundBillData.AccountantSubjectId = staffDetail.TypeId;
                        fundBillData.AccountantSubjectName = staffDetail.TypeName;
                    }

                    fundBillData.TreasureId = treasureDetail.Id;
                    fundBillData.TreasureName = treasureDetail.Name;

                    // Gắn thông tin người tạo
                    fundBillData.UserId = UserState.UserId;
                    fundBillData.UserName = UserState.UserName;

                    // Lưu lại vào database
                    UnitOfWork.FundBillRepo.Add(fundBillData);
                    UnitOfWork.FundBillRepo.Save();

                    // Bắn Notification
                    var office = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => x.Type == (byte)OfficeType.Accountancy);
                    var listUser = await UnitOfWork.UserRepo.GetUserToOffice(0, 1, office.IdPath, office.Id);

                    foreach (var user in listUser.Where(x => x.Id != UserState.UserId).ToList())
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient(user.Id, "Approval request of fund No. #" + model.Code, EnumNotifyType.Info, "Staff: '" + UserState.FullName + "' has performed a approval request to add/deduct fund money: " + fundBillData.FinanceFundName + ". Transaction code No.: #" + model.Code);
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
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError }, JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.CreateFundBillIsSuccess }, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.Update, EnumPage.FundBill)]
        public ActionResult EditFundBill(FundBillMeta model)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            var customerDetail = new Customer();
            var staffDetail = new User();

            //1. Lấy thông tin phiếu giao dịch quỹ
            var fundBillDetail = UnitOfWork.FundBillRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.Id);
            if (fundBillDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.FundBillIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            ////2. Kiểm tra xem phiếu giao dịch quỹ đã hoàn thành hay chưa
            //if (fundBillDetail.Status == (byte)FundBillStatus.Approved)
            //{
            //    return Json(new { status = Result.Failed, msg = ConstantMessage.FundBillIsSuccess }, JsonRequestBehavior.AllowGet);
            //}

            //3. Lấy thông tin Detail quỹ
            var financeFundDetail = UnitOfWork.FinaceFundRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && x.Id == model.FinanceFundId);
            if (financeFundDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.FinanceFundIsSuccess }, JsonRequestBehavior.AllowGet);
            }

            //4. Chặn không cho trừ quá Balances tồn tại trong quỹ
            if (fundBillDetail.Type == (byte)FundBillType.Diminishe)
            {
                if (fundBillDetail.Status != (byte)FundBillStatus.Approved)
                {
                    if (model.CurrencyFluctuations > financeFundDetail.Balance)
                    {
                        return Json(new { status = Result.Failed, msg = ConstantMessage.CurrencyFluctuationsImpossible }, JsonRequestBehavior.AllowGet);
                    }
                }

            }

            //5. Lấy thông tin Detail Type định khoản để thực hiện thay đổi dữ liệu
            var treasureDetail = UnitOfWork.TreasureRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.TreasureId);
            if (treasureDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.TreasureIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            //5.1. Nếu là nạp tiền vào quỹ
            if (treasureDetail.Operator == true)
            {
                model.Type = (byte)FundBillType.Increase;
                model.Increase = model.CurrencyFluctuations;
                model.Diminishe = 0;
            }

            //5.2. Nếu là trừ tiền vào quỹ thì chặn không cho trừ quá Balances tồn tại trong quỹ
            if (treasureDetail.Operator == false)
            {
                if (fundBillDetail.Status != (byte)FundBillStatus.Approved)
                {
                    if (model.CurrencyFluctuations > financeFundDetail.Balance)
                    {
                        return Json(new { status = Result.Failed, msg = ConstantMessage.CurrencyFluctuationsImpossible }, JsonRequestBehavior.AllowGet);
                    }
                }

                model.Type = (byte)FundBillType.Diminishe;
                model.Diminishe = model.CurrencyFluctuations;
                model.Increase = 0;
            }

            ////5. Lấy thông tin Detail Type đối tượng kế toán.
            //var accountantSubjectDetail = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => !x.IsDelete && x.Id == customerDetail.TypeId && x.Idd == customerDetail.TypeIdd);
            //if (accountantSubjectDetail == null)
            //{
            //    return Json(new { status = Result.Failed, msg = ConstantMessage.AccountantSubjectIsNotValid }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    model.AccountantSubjectId = customerDetail.TypeId;
            //    model.AccountantSubjectName = customerDetail.TypeName;
            //}

            //6. Lấy thông tin Detail đối tượng nhập là khách hàng
            customerDetail = UnitOfWork.CustomerRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && x.Id == model.SubjectId);
            if (customerDetail == null)
            {
                staffDetail = UnitOfWork.UserRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.SubjectId);
                if (staffDetail == null)
                {
                    return Json(new { status = Result.Failed, msg = "You have not entered subject information or subject does not exist !" }, JsonRequestBehavior.AllowGet);
                }
            }

            //7. Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lưu thông tin phiếu nạp/trừ quỹ
                    Mapper.Map(model, fundBillDetail);

                    // Map lại các thông tin để lưu
                    fundBillDetail.FinanceFundId = financeFundDetail.Id;
                    fundBillDetail.FinanceFundName = financeFundDetail.Name;
                    fundBillDetail.FinanceFundBankAccountNumber = financeFundDetail.BankAccountNumber;
                    fundBillDetail.FinanceFundDepartment = financeFundDetail.Department;
                    fundBillDetail.FinanceFundNameBank = financeFundDetail.NameBank;
                    fundBillDetail.FinanceFundUserFullName = financeFundDetail.UserFullName;
                    fundBillDetail.FinanceFundUserPhone = financeFundDetail.UserPhone;
                    fundBillDetail.FinanceFundUserEmail = financeFundDetail.UserEmail;

                    //if (customerDetail != null)
                    //{
                    //    fundBillDetail.SubjectId = customerDetail.Id;
                    //    fundBillDetail.SubjectCode = customerDetail.Code;
                    //    fundBillDetail.SubjectName = customerDetail.FullName;
                    //    fundBillDetail.SubjectPhone = customerDetail.Phone;
                    //    fundBillDetail.SubjectEmail = customerDetail.Email;

                    //    fundBillDetail.AccountantSubjectId = customerDetail.TypeId;
                    //    fundBillDetail.AccountantSubjectName = customerDetail.TypeName;
                    //}
                    //else
                    //{
                    //    fundBillDetail.SubjectId = staffDetail.Id;
                    //    fundBillDetail.SubjectCode = "";
                    //    fundBillDetail.SubjectName = staffDetail.FullName;
                    //    fundBillDetail.SubjectPhone = staffDetail.Phone;
                    //    fundBillDetail.SubjectEmail = staffDetail.Email;

                    //    fundBillDetail.AccountantSubjectId = staffDetail.TypeId;
                    //    fundBillDetail.AccountantSubjectName = staffDetail.TypeName;
                    //}

                    fundBillDetail.TreasureId = treasureDetail.Id;
                    fundBillDetail.TreasureName = treasureDetail.Name;

                    // Lưu lại vào database
                    UnitOfWork.FundBillRepo.Save();

                    // Kiểm tra xem có thêm yêu cầu tạo một phiếu nạp/trừ tiền ví điện tử khách hàng tương ứng hay không ?

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
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError }, JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.EditFundBillIsSuccess }, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.Approvel, EnumPage.FundBill)]
        public JsonResult ApprovalFundBill(int fundBillId)
        {

            //1. Kiểm tra thông tin phiếu giao dịch
            var fundBillDetail = UnitOfWork.FundBillRepo.FirstOrDefault(x => !x.IsDelete && x.Id == fundBillId);
            if (fundBillDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }
            if (fundBillDetail.Status == (byte)FundBillStatus.Approved)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            //2. Lấy thông tin quỹ để thực hiện Nạp/Trừ tiền
            var financceFundId = fundBillDetail.FinanceFundId;
            var financeFundDetail = UnitOfWork.FinaceFundRepo.FirstOrDefault(x => !x.IsDelete && x.Id == financceFundId);
            if (financeFundDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "The fund does not exist or has been suspended or deleted !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Kiểm tra thông tin định khoản là tự động nạp tiền ví hay không
            var treasureDetail = UnitOfWork.TreasureRepo.FirstOrDefault(x => !x.IsDelete && x.IsIdSystem == true && x.Idd > 0 && x.Id == fundBillDetail.TreasureId);

            //4. Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lưu thông tin vào ví điện tử khách hàng
                    //Kiểm tra xem định khoản của phiếu quỹ có phải là tự động trong system hay không ?
                    //Nếu là tự động thì phải nạp tiền vào ví tương ứng
                    if (treasureDetail != null)
                    {
                        //Nếu phát hiện ra là có định khoản tự động thì
                        var customerWalletDetail = UnitOfWork.CustomerWalletRepo.FirstOrDefault(x => !x.IsDelete && x.IsIdSystem == true && x.Idd == treasureDetail.Idd);
                        if (customerWalletDetail == null)
                        {
                            return Json(new { status = Result.Failed, msg = "Fund entry " + treasureDetail.Name + " does not match with the entry of" + customerWalletDetail.Name + ".   !" }, JsonRequestBehavior.AllowGet);
                        }

                        //Kiểm tra số dư ví hiện tại của người dùng
                        var customerDetail = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == fundBillDetail.SubjectId);
                        if (customerDetail == null)
                        {
                            return Json(new { status = Result.Failed, msg = "Entry " + treasureDetail.Name + " only applies to subjects that are customers, or this customer does not exist !" }, JsonRequestBehavior.AllowGet);
                        }

                        //Kiểm tra nếu gặp định khoản rút ví điện tử của khách thì xem khách có đủ tiền để rút hay không
                        if ((treasureDetail.Idd == (int)TreasureCustomerWallet.Withdrawals) && (customerDetail.BalanceAvalible < fundBillDetail.CurrencyFluctuations))
                        {
                            return Json(new { status = Result.Failed, msg = "The note could not be approved. This account do not have enough debit balance!" }, JsonRequestBehavior.AllowGet);
                        }


                        //Ghi nhận tự động phiếu nạp tiền ví cho khách hàng
                        var rechargeBill = new RechargeBill();

                        rechargeBill.Code = fundBillDetail.Code;
                        rechargeBill.Status = (byte)RechargeBillStatus.Approved;                //Được duyệt tự động
                        rechargeBill.Note = customerWalletDetail.Name;

                        rechargeBill.CurrencyFluctuations = fundBillDetail.CurrencyFluctuations;
                        rechargeBill.CurencyStart = customerDetail.BalanceAvalible;

                        if (customerWalletDetail.Operator == true)
                        {
                            //Cộng tiền ví điện tử
                            rechargeBill.CurencyEnd = customerDetail.BalanceAvalible + fundBillDetail.CurrencyFluctuations;
                            rechargeBill.Type = (byte)RechargeBillType.Increase;
                            rechargeBill.Increase = fundBillDetail.CurrencyFluctuations;
                            rechargeBill.Diminishe = 0;
                        }
                        if (customerWalletDetail.Operator == false)
                        {
                            //Trừ tiền ví điện tử
                            rechargeBill.CurencyEnd = customerDetail.BalanceAvalible - fundBillDetail.CurrencyFluctuations;
                            rechargeBill.Type = (byte)RechargeBillType.Diminishe;
                            rechargeBill.Diminishe = fundBillDetail.CurrencyFluctuations;
                            rechargeBill.Increase = 0;
                        }

                        rechargeBill.UserId = UserState.UserId;
                        rechargeBill.UserName = UserState.UserName;

                        rechargeBill.UserApprovalId = UserState.UserId;
                        rechargeBill.UserApprovalName = UserState.UserName;

                        rechargeBill.CustomerId = fundBillDetail.SubjectId;
                        rechargeBill.CustomerCode = fundBillDetail.SubjectCode;
                        rechargeBill.CustomerName = fundBillDetail.SubjectName;
                        rechargeBill.CustomerPhone = fundBillDetail.SubjectPhone;
                        rechargeBill.CustomerEmail = fundBillDetail.SubjectEmail;

                        rechargeBill.TreasureId = customerWalletDetail.Id;
                        rechargeBill.TreasureName = customerWalletDetail.Name;
                        rechargeBill.TreasureIdd = customerWalletDetail.Idd;
                        rechargeBill.IsAutomatic = true;

                        rechargeBill.Created = fundBillDetail.Created;
                        rechargeBill.LastUpdated = fundBillDetail.LastUpdated;

                        //Thông tin người tạo và thông tin người duyệt phiếu nạp tiền ví
                        UnitOfWork.RechargeBillRepo.Add(rechargeBill);
                        UnitOfWork.RechargeBillRepo.Save();


                        //Nếu định khoản equaling to  của ví là Cộng tiền thì

                        // Nếu là nạp tiền ví điện tử
                        if (customerWalletDetail.Operator == true)
                        {
                            customerDetail.BalanceAvalible += fundBillDetail.CurrencyFluctuations;  // Cộng tiền vào Balances
                        }

                        // Nếu là trừ tiền ví điện tử
                        if (customerWalletDetail.Operator == false)
                        {
                            // Chặn không cho trừ quá Balances trong Account
                            if (fundBillDetail.CurrencyFluctuations > customerDetail.BalanceAvalible)
                            {
                                return
                                    Json(
                                        new { status = Result.Failed, msg = "Customer's account balance is not enough to perform this action !" },
                                        JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                customerDetail.BalanceAvalible -= fundBillDetail.CurrencyFluctuations;  // Trừ tiền vào Balances
                            }
                        }

                        // Bắn Notification
                        NotifyHelper.CreateAndSendNotifySystemToClient((int)fundBillDetail.UserId, "[Notice]: Money adding/deducting note No.: #" + fundBillDetail.Code + " has been approved", EnumNotifyType.Info, "Staff: '" + UserState.FullName + "' has just approved Money adding/deducting note " + fundBillDetail.FinanceFundName + ". Transaction code No.: #" + fundBillDetail.Code);

                        UnitOfWork.CustomerRepo.Update(customerDetail);
                        UnitOfWork.CustomerRepo.Save();
                    }

                    //============== Lưu phiếu Charge
                    fundBillDetail.Status = (byte)FundBillStatus.Approved; //Chuyển trạng thái thành được duyệt
                    fundBillDetail.CurencyStart = financeFundDetail.Balance;

                    if (fundBillDetail.Type == (byte)FundBillType.Increase)
                    {
                        fundBillDetail.CurencyEnd = financeFundDetail.Balance + fundBillDetail.CurrencyFluctuations;
                    }
                    if (fundBillDetail.Type == (byte)FundBillType.Diminishe)
                    {
                        fundBillDetail.CurencyEnd = financeFundDetail.Balance - fundBillDetail.CurrencyFluctuations;
                    }

                    //Thông tin người duyệt
                    fundBillDetail.UserApprovalId = UserState.UserId;
                    fundBillDetail.UserApprovalName = UserState.UserName;

                    UnitOfWork.FundBillRepo.Update(fundBillDetail);
                    UnitOfWork.FundBillRepo.Save();

                    //Cập nhật số tiền mới vào quỹ
                    if (fundBillDetail.Type == (byte)FundBillType.Increase)
                    {
                        financeFundDetail.Balance += fundBillDetail.CurrencyFluctuations;               // Cộng tiền mới vào quỹ
                    }

                    if (fundBillDetail.Type == (byte)FundBillType.Diminishe)
                    {
                        // Chặn không cho trừ quá Balances trong quỹ
                        if (fundBillDetail.CurrencyFluctuations > financeFundDetail.Balance)
                        {
                            return Json(new { status = Result.Failed, msg = ConstantMessage.CurrencyFluctuationsImpossible }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            financeFundDetail.Balance -= fundBillDetail.CurrencyFluctuations;            // Subtract funds
                        }
                    }

                    // Ghi lại số dư vào quỹ
                    UnitOfWork.FinaceFundRepo.Update(financeFundDetail);
                    UnitOfWork.FinaceFundRepo.Save();


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
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError }, JsonRequestBehavior.AllowGet);
                //}
            }
            return Json(new { status = Result.Succeed, msg = ConstantMessage.ApprovalFundBillIsSuccess }, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.Delete, EnumPage.FundBill)]
        public JsonResult DeleteFundBill(int fundBillId)
        {
            //1. Kiểm tra thông tin FundBill 
            var fundBillDetail = UnitOfWork.FundBillRepo.FirstOrDefault(x => !x.IsDelete && x.Id == fundBillId);
            if (fundBillDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra xem FundBill đã được xác nhận chưa
            if (fundBillDetail.Status == 1)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.BillIsApproval }, JsonRequestBehavior.AllowGet);
            }

            //2. Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lấy lại thông tin để thực hiện lưu
                    fundBillDetail.IsDelete = true;

                    //Lưu xuống Database
                    UnitOfWork.FundBillRepo.Update(fundBillDetail);
                    UnitOfWork.FundBillRepo.Save();

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
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError }, JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.DeleteFundBillIsSuccess }, JsonRequestBehavior.AllowGet);
        }

        #region Các hàm lấy thông tin cho tạo/Edit/xóa phiếu Charge

        /// <summary>
        /// Hàm lấy thông tin trên Form tạo/Charge
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFundBillInitCreateOrEdit()
        {
            // Lấy danh sách các đối tượng nhập
            var accounttantSubject = UnitOfWork.AccountantSubjectRepo.FindAsNoTracking(x => !x.IsDelete && x.Id > 0).ToList();
            //var tempAcounttantSubject = from p in accounttantSubject
            //                            select new SearchMeta() { Text = p.SubjectName, Value = p.Id };
            //var listAccountantSubject = tempAcounttantSubject.ToList();

            // Lấy danh sách quỹ
            var listFinanceFund = UnitOfWork.DbContext.FinanceFunds.Where(x => !x.IsDelete && x.Id > 0).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                idPath = o.IdPath,
            }).ToList();
            listFinanceFund.Add(new { id = "0", text = "- Select fund -", parent = "#", idPath = "0" });

            // Lấy danh sách định khoản
            // Lấy danh sách quỹ
            var listTreasure = UnitOfWork.DbContext.Treasures.Where(x => !x.IsDelete && x.Id > 0).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                idPath = o.IdPath,
            }).ToList();
            listTreasure.Add(new { id = "0", text = "Fund management", parent = "#", idPath = "0" });

            return Json(new { listFinanceFund, listTreasure }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}