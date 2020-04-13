using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Cms.Controllers
{
    public class AccountantReportController : BaseController
    {
        [HttpPost]
        public ActionResult ExportExcelRevenueOrderOffice(DateTime? startDay, DateTime? endDay)
        {

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#FF0000");

                var start = startDay ?? DateTime.Now;
                var end = endDay ?? DateTime.Now;
                var col = 1;
                var row = 4;

                //Tiêu đề
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STAFF", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "WEBSITE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "LOẠI HÀNG", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "EXCHANGE RATE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Cost of goods (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "SHIPPING FEE (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "RETAIL-ORDERING FEE (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                //ExcelHelper.CreateHeaderTable(sheet, row, col, "PHÍ MUA HÀNG (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                //col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "COUNTING FEE (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Total order amount (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Sum TIỀN THANH TOÁN (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "bargain (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "COMPLAINT FEE (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ADD PROFIT (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);


                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"REVENUE REPORT TO CALCULATE ORDERING STAFF PAYROLL FROM DATE {start.ToShortDateString()} TO DATE {end.ToShortDateString()}", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 18,
                    BackgroundColor = colorHeader
                });

                #endregion

                //Lấy Orders đã hoàn thành
                var listOrder = UnitOfWork.OrderRepo.GetOrderReportRevenue(start, end);

                //Lấy thông tin phòng đặt hàng
                var listOffice = UnitOfWork.OfficeRepo.Find(x => !x.IsDelete && x.Type == (byte)OfficeType.Order);
                if (listOffice.Any())
                {
                    if (listOrder.Any())
                    {
                        var listOfficeId = listOffice.Select(x => x.Id);
                        listOrder = listOrder.Where(x => listOfficeId.Contains(x.OfficeId.Value)).ToList();
                    }
                }
                else
                {
                    listOrder = new List<Order>();
                }

                if (listOrder.Any())
                {
                    //Lấy danh sách id Orders.
                    var listOrderId = listOrder.Select(x => x.Id).ToList();
                    var listClaimForRefund = UnitOfWork.ClaimForRefundRepo.Find(x => !x.IsDelete && x.Status == (byte)ClaimForRefundStatus.Success && listOrderId.Contains(x.OrderId)).ToList();
                    var listOrderService = UnitOfWork.OrderServiceRepo.Find(x => !x.IsDelete && x.Checked && listOrderId.Contains(x.OrderId)).ToList();

                    foreach (var order in listOrder)
                    {
                        row++;
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, row, col, MyCommon.ReturnCode(order.Code), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, order.UserFullName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, order.CustomerEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, order.WebsiteName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, EnumHelper.GetEnumDescription<OrderStatus>(order.Status), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, CheckWebsiteName(order.WebsiteName) ? "Order" : "Retail product", ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, order.ExchangeRate, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, order.TotalExchange, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, (order.FeeShip * order.ExchangeRate), new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;

                        //Lấy phí hàng lẻ
                        var orderServiceRetailCharge = listOrderService.FirstOrDefault(
                                x => x.OrderId == order.Id && x.ServiceId == (byte)OrderServices.RetailCharge);

                        if (orderServiceRetailCharge == null)
                        {
                            ExcelHelper.CreateCellTable(sheet, row, col, row, col, null,
                                new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            col++;
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, row, col, row, col, orderServiceRetailCharge.TotalPrice,
                                new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            col++;
                        }

                        ////tình phí dịch vụ mua hàng
                        //var orderServiceOrder = listOrderService.FirstOrDefault(
                        //        x => x.OrderId == order.Id && x.ServiceId == (byte)OrderServices.Order);
                        //ExcelHelper.CreateCellTable(sheet, row, col, row, col, orderServiceOrder.TotalPrice, new CustomExcelStyle
                        //{
                        //    IsMerge = false,
                        //    IsBold = false,
                        //    Border = ExcelBorderStyle.Thin,
                        //    HorizontalAlign = ExcelHorizontalAlignment.Right,
                        //    NumberFormat = "#,##0"
                        //});
                        //col++;

                        //tình phí kiểm đếm
                        var orderServiceAudit = listOrderService.FirstOrDefault(
                                x => x.OrderId == order.Id && x.ServiceId == (byte)OrderServices.Audit);
                        if (orderServiceAudit == null)
                        {
                            ExcelHelper.CreateCellTable(sheet, row, col, row, col, null,
                                new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            col++;
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, row, col, row, col, orderServiceAudit.TotalPrice,
                                new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            col++;
                        }

                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, order.Total, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, (order.PaidShop * order.ExchangeRate + order.FeeShipBargain * order.ExchangeRate), new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, order.PriceBargain * order.ExchangeRate, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;

                        //Lấy tiền chi khiếu nại
                        var totalClaimForRefund = listClaimForRefund.Where(x => x.OrderId == order.Id).Sum(x => x.RealTotalRefund);
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, totalClaimForRefund, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, (order.PriceBargain * order.ExchangeRate) - totalClaimForRefund, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"ORDER_OFFICE_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        [HttpPost]
        public ActionResult ExportExcelRevenueBusinessOffice(DateTime? startDay, DateTime? endDay)
        {

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#FF0000");

                var start = startDay ?? DateTime.Now;
                var end = endDay ?? DateTime.Now;
                var col = 1;
                var row = 4;

                //Tiêu đề
                ExcelHelper.CreateHeaderTable(sheet, row, col, "MÃ ĐƠN HÀNG", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STAFF", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "WEBSITE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TYPE OF PRODUCT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "EXCHANGE RATE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Cost of goods (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "SHIPPING FEE (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "RETAIL ORDERING FEE (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                //ExcelHelper.CreateHeaderTable(sheet, row, col, "PHÍ MUA HÀNG (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                //col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "COUNTING FEE (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TOTAL (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TOTAL PAID (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "bargain (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "COMPLAINT FEE (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ADD PROFIT (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);


                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"BÁO CÁO DOANH THU TÍNH LƯƠNG PHÒNG KINH DOANH TỪ NGÀY {start.ToShortDateString()} TO DATE {end.ToShortDateString()}", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 18,
                    BackgroundColor = colorHeader
                });

                #endregion

                //Lấy Orders đã hoàn thành
                var listOrder = UnitOfWork.OrderRepo.GetOrderReportRevenue(start, end);
                var listCustomer = new List<Customer>();
                var listUser = UnitOfWork.UserRepo.Entities.Where(x => !x.IsDelete).ToList();

                //Lấy thông tin phòng kinh doanh
                var listOffice = UnitOfWork.OfficeRepo.Find(x => !x.IsDelete && x.Type == (byte)OfficeType.Business);
                if (listOffice.Any())
                {
                    //lấy thông tin khách hàng
                    var listOfficeId = listOffice.Select(x => x.Id);
                    listCustomer = UnitOfWork.CustomerRepo.Find(x => !x.IsDelete && listOfficeId.Contains(x.OfficeId.Value)).ToList();

                    if (listCustomer.Any())
                    {
                        if (listOrder.Any())
                        {
                            var listCustomerId = listCustomer.Select(x => x.Id).ToList();
                            listOrder = listOrder.Where(x => listCustomerId.Contains(x.CustomerId.Value)).ToList();
                        }
                    }
                    else
                    {
                        listOrder = new List<Order>();
                    }
                }
                else
                {
                    listOrder = new List<Order>();
                }

                if (listOrder.Any())
                {
                    //Lấy danh sách id Orders.
                    var listOrderId = listOrder.Select(x => x.Id).ToList();
                    var listClaimForRefund = UnitOfWork.ClaimForRefundRepo.Find(x => !x.IsDelete && x.Status == (byte)ClaimForRefundStatus.Success && listOrderId.Contains(x.OrderId)).ToList();
                    var listOrderService = UnitOfWork.OrderServiceRepo.Find(x => !x.IsDelete && x.Checked && listOrderId.Contains(x.OrderId)).ToList();

                    foreach (var order in listOrder)
                    {
                        //Lấy thông tin nhân viên
                        var customer = listCustomer.FirstOrDefault(x => x.Id == order.CustomerId.Value);
                        var user = listUser.FirstOrDefault(x => x.Id == customer.UserId.Value);
                        if (user.IsCompany)
                            continue;

                        row++;
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, row, col, MyCommon.ReturnCode(order.Code), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, user.FullName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, order.CustomerEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, order.WebsiteName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        if (order.Type == (byte)OrderType.Deposit)
                        {
                            ExcelHelper.CreateCellTable(sheet, row, col, EnumHelper.GetEnumDescription<DepositStatus>(order.Status), ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, row, col, EnumHelper.GetEnumDescription<OrderStatus>(order.Status), ExcelHorizontalAlignment.Left, true);
                        }
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, CheckWebsiteName(order.WebsiteName) ? order.Type == (byte)OrderType.Deposit ? "Consign" : "Order" : "Retail product", ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, order.ExchangeRate, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, order.TotalExchange, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, (order.FeeShip * order.ExchangeRate), new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;

                        //Lấy phí hàng lẻ
                        var orderServiceRetailCharge = listOrderService.FirstOrDefault(
                                x => x.OrderId == order.Id && x.ServiceId == (byte)OrderServices.RetailCharge);

                        if (orderServiceRetailCharge == null)
                        {
                            ExcelHelper.CreateCellTable(sheet, row, col, row, col, null,
                                new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            col++;
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, row, col, row, col, orderServiceRetailCharge.TotalPrice,
                                new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            col++;
                        }

                        ////tình phí dịch vụ mua hàng
                        //var orderServiceOrder = listOrderService.FirstOrDefault(
                        //        x => x.OrderId == order.Id && x.ServiceId == (byte)OrderServices.Order);
                        //ExcelHelper.CreateCellTable(sheet, row, col, row, col, orderServiceOrder.TotalPrice, new CustomExcelStyle
                        //{
                        //    IsMerge = false,
                        //    IsBold = false,
                        //    Border = ExcelBorderStyle.Thin,
                        //    HorizontalAlign = ExcelHorizontalAlignment.Right,
                        //    NumberFormat = "#,##0"
                        //});
                        //col++;

                        //tình phí kiểm đếm
                        var orderServiceAudit = listOrderService.FirstOrDefault(
                                x => x.OrderId == order.Id && x.ServiceId == (byte)OrderServices.Audit);
                        if (orderServiceAudit == null)
                        {
                            ExcelHelper.CreateCellTable(sheet, row, col, row, col, null,
                                new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            col++;
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, row, col, row, col, orderServiceAudit.TotalPrice,
                                new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            col++;
                        }

                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, order.Total, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, (order.PaidShop * order.ExchangeRate + order.FeeShipBargain * order.ExchangeRate), new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, order.PriceBargain * order.ExchangeRate, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;

                        //Lấy tiền chi khiếu nại
                        var totalClaimForRefund = listClaimForRefund.Where(x => x.OrderId == order.Id).Sum(x => x.RealTotalRefund);
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, totalClaimForRefund, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, row, col, (order.PriceBargain * order.ExchangeRate) - totalClaimForRefund, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"BUSINESS_OFFICE_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        [HttpPost]
        public ActionResult ExportExcelRevenueGomContOffice(DateTime? startDay, DateTime? endDay)
        {

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#FF0000");

                var start = startDay ?? DateTime.Now;
                var end = endDay ?? DateTime.Now;
                var col = 1;
                var row = 4;

                //Title
                ExcelHelper.CreateHeaderTable(sheet, row, col, "MÃ package", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "MÃ ĐƠN HÀNG", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col,"STAFF", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TYPE OF PRODUCT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ACTUAL NET WEIGHT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "WEIGHT TO CHARGE CUSTOMER", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "UNIT PRICE (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "REVENUE BY WEIGHT (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CARRIER", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "RECEIVING WAREHOUSE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "COMPLAINT FEE (Baht)", ExcelHorizontalAlignment.Center, true, colorHeader);


                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"BÁO CÁO DOANH THU TÍNH LƯƠNG PHÒNG GOM CONT TỪ NGÀY {start.ToShortDateString()} TO DATE {end.ToShortDateString()}", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 18,
                    BackgroundColor = colorHeader
                });

                #endregion

                //Lấy Orders đã hoàn thành
                var listOrder = UnitOfWork.OrderRepo.GetOrderReportRevenue(start, end);
                var listCustomer = new List<Customer>();
                var listUser = UnitOfWork.UserRepo.Entities.Where(x => !x.IsDelete).ToList();

                //Lấy thông tin phòng kinh doanh gom cont
                var listOffice = UnitOfWork.OfficeRepo.Find(x => !x.IsDelete && x.Type == (byte)OfficeType.Deposit);
                if (listOffice.Any())
                {
                    //lấy thông tin khách hàng
                    var listOfficeId = listOffice.Select(x => x.Id);
                    listCustomer = UnitOfWork.CustomerRepo.Find(x => !x.IsDelete && listOfficeId.Contains(x.OfficeId.Value)).ToList();

                    if (listCustomer.Any())
                    {
                        if (listOrder.Any())
                        {
                            var listCustomerId = listCustomer.Select(x => x.Id).ToList();
                            listOrder = listOrder.Where(x => listCustomerId.Contains(x.CustomerId.Value)).ToList();
                        }
                    }
                    else
                    {
                        listOrder = new List<Order>();
                    }
                }
                else
                {
                    listOrder = new List<Order>();
                }

                if (listOrder.Any())
                {
                    //Lấy danh sách id Orders.
                    var listOrderId = listOrder.Select(x => x.Id).ToList();
                    var listClaimForRefund = UnitOfWork.ClaimForRefundRepo.Find(x => !x.IsDelete && x.Status == (byte)ClaimForRefundStatus.Success && listOrderId.Contains(x.OrderId)).ToList();
                    var listOrderService = UnitOfWork.OrderServiceRepo.Find(x => !x.IsDelete && x.Checked && listOrderId.Contains(x.OrderId)).ToList();

                    //lấy danh sách package
                    var listOrderPackage = UnitOfWork.OrderPackageRepo.Find(x => !x.IsDelete && listOrderId.Contains(x.OrderId) && x.Status == (byte)OrderPackageStatus.Completed);
                    var listOrderPackageId = listOrderPackage.Select(x => x.Id);
                    //lấy danh sách các bao
                    var listWalletDetail = UnitOfWork.WalletDetailRepo.Find(x => !x.IsDelete && listOrderPackageId.Contains(x.PackageId));
                    var listWalletId = listWalletDetail.Select(x => x.WalletId);
                    var listWallet = UnitOfWork.WalletRepo.Find(x => !x.IsDelete && listWalletId.Contains(x.Id));

                    row++;
                    foreach (var order in listOrder)
                    {
                        var listOrderPackageOrder = listOrderPackage.Where(x => x.OrderId == order.Id).ToList();
                        if (!listOrderPackageOrder.Any()) continue;

                        //Lấy thông tin nhân viên
                        var customer = listCustomer.FirstOrDefault(x => x.Id == order.CustomerId.Value);
                        var user = listUser.FirstOrDefault(x => x.Id == customer.UserId.Value);
                        if (user.IsCompany)
                            continue;

                        col = 1;
                        var rowChil = row;

                        foreach (var package in listOrderPackageOrder)
                        {
                            ExcelHelper.CreateCellTable(sheet, rowChil, col, package.Code, ExcelHorizontalAlignment.Left, true);
                            rowChil++;
                        }
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, rowChil - 1, col, MyCommon.ReturnCode(order.Code), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, rowChil - 1, col, user.FullName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, rowChil - 1, col, order.CustomerEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        if (order.Type == (byte)OrderType.Deposit)
                        {
                            ExcelHelper.CreateCellTable(sheet, row, col, rowChil - 1, col, EnumHelper.GetEnumDescription<DepositStatus>(order.Status), ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, row, col, rowChil - 1, col, EnumHelper.GetEnumDescription<OrderStatus>(order.Status), ExcelHorizontalAlignment.Left, true);
                        }
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, rowChil - 1, col, CheckWebsiteName(order.WebsiteName) ? "Order" : order.Type == (byte)OrderType.Deposit ? "Consign" : "Retail product", ExcelHorizontalAlignment.Left, true);
                        col++;

                        rowChil = row;
                        foreach (var package in listOrderPackageOrder)
                        {
                            ExcelHelper.CreateCellTable(sheet, rowChil, col, rowChil, col, package.Weight, new CustomExcelStyle
                            {
                                IsMerge = false,
                                IsBold = false,
                                Border = ExcelBorderStyle.Thin,
                                HorizontalAlign = ExcelHorizontalAlignment.Right,
                                NumberFormat = "#,##0"
                            });
                            rowChil++;
                        }
                        col++;
                        rowChil = row;
                        foreach (var package in listOrderPackageOrder)
                        {
                            ExcelHelper.CreateCellTable(sheet, rowChil, col, rowChil, col, package.WeightActual, new CustomExcelStyle
                            {
                                IsMerge = false,
                                IsBold = false,
                                Border = ExcelBorderStyle.Thin,
                                HorizontalAlign = ExcelHorizontalAlignment.Right,
                                NumberFormat = "#,##0"
                            });
                            rowChil++;
                        }
                        col++;
                        ExcelHelper.CreateCellTable(sheet, row, col, rowChil - 1, col, order.ApprovelPrice, new CustomExcelStyle
                        {
                            IsMerge = true,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        rowChil = row;

                        var outSideShipping =
                            listOrderService.FirstOrDefault(
                                x => x.OrderId == order.Id && x.ServiceId == (byte)OrderServices.OutSideShipping);

                        foreach (var package in listOrderPackageOrder)
                        {
                            ExcelHelper.CreateCellTable(sheet, rowChil, col, rowChil, col, package.WeightActualPercent * outSideShipping.TotalPrice / 100, new CustomExcelStyle
                            {
                                IsMerge = false,
                                IsBold = false,
                                Border = ExcelBorderStyle.Thin,
                                HorizontalAlign = ExcelHorizontalAlignment.Right,
                                NumberFormat = "#,##0"
                            });
                            rowChil++;
                        }
                        col++;

                        //nhà vận chuyển
                        rowChil = row;
                        foreach (var package in listOrderPackageOrder)
                        {
                            //lấy Detail bao
                            var walletDetails = listWalletDetail.Where(x => x.PackageId == package.Id).ToList();
                            if (walletDetails.Any())
                            {
                                var walletDetailIds = walletDetails.Select(x => x.WalletId);
                                var wallet = listWallet.FirstOrDefault(x => walletDetailIds.Contains(x.Id) && x.Mode == 0);

                                ExcelHelper.CreateCellTable(sheet, rowChil, col, rowChil, col, wallet.EntrepotName,
                                    ExcelHorizontalAlignment.Left, true);
                            }
                            else
                            {
                                ExcelHelper.CreateCellTable(sheet, rowChil, col, rowChil, col, null,
                                   ExcelHorizontalAlignment.Left, true);
                            }

                            rowChil++;
                        }
                        col++;
                        //kho nhận hàng
                        ExcelHelper.CreateCellTable(sheet, row, col, rowChil - 1, col, order.WarehouseDeliveryName, ExcelHorizontalAlignment.Left, true);
                        col++;

                        //Lấy tiền chi khiếu nại
                        var totalClaimForRefund = listClaimForRefund.Where(x => x.OrderId == order.Id).Sum(x => x.RealTotalRefund);
                        ExcelHelper.CreateCellTable(sheet, row, col, rowChil - 1, col, totalClaimForRefund, new CustomExcelStyle
                        {
                            IsMerge = true,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        row = rowChil;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"GOMCONT_OFFICE_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        public bool CheckWebsiteName(string website)
        {
            if (website == null)
                return false;
            if (website.Contains("taobao.com"))
                return true;
            if (website.Contains("1688.com"))
                return true;
            if (website.Contains("tmall.com"))
                return true;
            return false;
        }
    }
}