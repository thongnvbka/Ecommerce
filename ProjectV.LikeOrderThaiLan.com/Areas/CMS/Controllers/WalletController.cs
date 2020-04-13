using Common.Items;
using Library.Models;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using ProjectV.LikeOrderThaiLan.com.Controllers;
using ResourcesLikeOrderThaiLan;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ProjectV.LikeOrderThaiLan.com.Areas.CMS.Controllers
{
    [Authorize]
    public class WalletController : BaseController
    {
        // GET: CMS/Wallet
        public ActionResult ElectronicWallet()
        {
            ViewBag.ElectronicWallet = "cl_on";

            //1. Kiểm tra thông tin khách hàng
            var tmpCustomer = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == CustomerState.Id);
            if (tmpCustomer != null)
            {
                ViewBag.BalanceAvalible = tmpCustomer.BalanceAvalible;
            }
            else
            {
                ViewBag.BalanceAvalible = 0;
            }

            return View();
        }

        [HttpPost]
        public JsonResult GetListRecharge(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = new RechargeModel();
            model = GetData(seachInfor, pageInfor);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public RechargeModel GetData(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = new RechargeModel();
            if (!string.IsNullOrEmpty(seachInfor.StartDateS))
            {
                seachInfor.StartDate = DateTime.ParseExact(seachInfor.StartDateS, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(seachInfor.FinishDateS))
            {
                seachInfor.FinishDate = DateTime.ParseExact(seachInfor.FinishDateS, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            seachInfor.SystemId = SystemId;
            seachInfor.CustomerId = CustomerState.Id;
            if (seachInfor.StartDate.ToString("dd/MM/yyyy") == "01/01/0001" || seachInfor.FinishDate.ToString("dd/MM/yyyy") == "01/01/0001")
            {
                seachInfor.AllTime = -1;
            }
            if (string.IsNullOrEmpty(seachInfor.Keyword))
            {
                seachInfor.Keyword = "";
            }
            var tmpId = -1;
            if (!string.IsNullOrEmpty(seachInfor.WalletIds))
            {
                int.TryParse(seachInfor.WalletIds, out tmpId);
            }
            seachInfor.WalletId = tmpId;
            model = UnitOfWork.RechargeBillRepo.GetAllRechargeByLinq(pageInfor, seachInfor);
            return model;
        }

        public string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public void ExportRecharge(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = GetData(seachInfor, pageInfor);

            var rechargeModal = model.ListItems;
           
            
           
           


            var sb = new StringBuilder();
            sb.Append("<table border='1px' cellpadding='1' cellspacing='1' >");
            sb.Append("<tr>");


            sb.Append("<td>STT</td>");
            sb.Append("<td>" + Resource.Code_Transition + "</td>");
            sb.Append("<td>" + Resource.TransactionHistory_Time + "</td>");
            sb.Append("<td>" + Resource.DonHang + "</td>");
            sb.Append("<td>" + Resource.TransactionHistory_TransactionType + "</td>");
            sb.Append("<td>" + Resource.CreateTicket_Content + "</td>");
            sb.Append("<td>" + Resource.TransactionHistory_Value + "</td>");
            sb.Append("<td>" + Resource.Dashboard_Surplus + "</td>");
            sb.Append("</tr>");
            //sb.Append("<td>#</td>");
            //sb.Append("<td>" + Resource.Code_Transition + "</td>");// sb.Append("<td>Mã phiếu</td>");
            //sb.Append("<td>" + Resource.TransactionHistory_Time + "</td>");//sb.Append("<td>Thời gian</td>");
            //sb.Append("<td>" + Resource.DonHang + "</td>");//sb.Append("<td>Đơn hàng</td>");
            //sb.Append("<td>" + Resource.TransactionHistory_TransactionType + "</td>");//sb.Append("<td>Loại giao dịch</td>");
            //sb.Append("<td>" + Resource.CreateTicket_Content + "</td>");// sb.Append("<td>Nội dung</td>");
            //sb.Append("<td>" + Resource.TransactionHistory_Value + "</td>");//sb.Append("<td>Giá trị</td>");
            //sb.Append("<td>" + Resource.Dashboard_Surplus + "</td>");    // sb.Append("<td>Số dư</td>");
            // 

            var index = 0;
            if (rechargeModal.Any())
            {
                for (int i = 0; i < rechargeModal.Count(); i++)
                {
                    var item = rechargeModal[i];
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", ++index);
                    sb.AppendFormat("<td>{0}</td>", item.Code);
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0:dd/MM/yyyy}\")", item.Created));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", item.OrderCode));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", (item.Type == 0 ? "Nạp tiền ví" : "Trừ tiền ví")));
                    sb.AppendFormat("<td>{0}</td>", FirstLetterToUpper(item.TreasureName));
                    sb.AppendFormat("<td>{0}</td>", item.CurrencyFluctuations);
                    sb.AppendFormat("<td>{0}</td>", (decimal)item.CurencyEnd);
                    sb.AppendFormat("</tr>").AppendLine();
                }
            }
            sb.Append("</table>");
            var fileName = string.Format("ViDienTu_{0:ddMMyyyy}.xls", DateTime.Now);
            var attachment = "attachment; filename=" + fileName + "";
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel"; //office 2003
            Response.ContentEncoding = Encoding.Unicode;
            Response.BinaryWrite(Encoding.Unicode.GetPreamble());
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }
    }
}