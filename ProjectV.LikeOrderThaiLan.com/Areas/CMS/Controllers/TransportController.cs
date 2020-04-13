using Common.Items;
using Library.ViewModels.Account;
using ProjectV.LikeOrderThaiLan.com.Controllers;
using System;
using System.Web.Mvc;
using System.Globalization;
using System.Text;
using System.Linq;
using ResourcesLikeOrderThaiLan;

namespace ProjectV.LikeOrderThaiLan.com.Areas.CMS.Controllers
{
    [Authorize]
    public class TransportController : BaseController
    {
  
        // GET: CMS/Transport
        //TODO gui 1 cong cu van chuyen
        public ActionResult SendTraffic()
        {
           
            ViewBag.ActiveSendTraffic = "cl_on";
            return View();
        }

        //TODO theo doi trang thai don hang van chuyen
        public ActionResult ShippingOrderStatus()
        {
           
            ViewBag.ActiveShippingOrderStatus = "cl_on";
            return View();
        }

        //TODO chi tiết đơn hàng vận chuyển
        public ActionResult ShippingOrderDetail()
        { 
            ViewBag.ActiveShippingOrderStatus = "cl_on";
            return View();
        }

        //TODO kien hang
        public ActionResult Package()
        {
            
            ViewBag.ActivePackage = "cl_on";
            return View();
        }
        //TODO Lay danh sach cac don hang ký gửi
        [HttpPost]
        public JsonResult GetAllOrderPackage(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = GetDataPackage(seachInfor, pageInfor);
            return Json(model, JsonRequestBehavior.AllowGet);


        }
        public OrderPackageModel GetDataPackage(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = new OrderPackageModel();

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

            model = UnitOfWork.OrderPackageRepo.GetAllOrderPackageByLinq(pageInfor, seachInfor);
            return model;
        }

        public string SetTextWallet(int packageId)
        {
            var result = "";
            if(UnitOfWork.OrderPackageRepo.CountPackageWallet(packageId) > 1)
            {
                //result = "(đóng chung)";
                result = "(แพคเกจที่นำแสดงโดย)";
            }
            return result;
            
        }

        public void ExportPackage(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = GetDataPackage(seachInfor, pageInfor);

            var sb = new StringBuilder();
            sb.Append("<table border='1px' cellpadding='1' cellspacing='1' >");
            sb.Append("<tr>"); 
            sb.Append("<td>#</td>"); 
            sb.Append("<td>รหัสออเดอร์</td>");//sb.Append("<td>Mã đơn</td>");
            sb.Append("<td>เวลาสร้าง</td>");//sb.Append("<td>Thời gian</td>"); 
            sb.Append("<td>ออเดอร์</td>");//sb.Append("<td>Đơn hàng</td>");
            sb.Append("<td>น้ำหนัก (kg)</td>");// sb.Append("<td>Cân nặng (kg)</td>");
            sb.Append("<td>ขนาด</td>"); //sb.Append("<td>Kích thước</td>");
            sb.Append("<td>โกดังที่สินค้าเข้า</td>"); //sb.Append("<td>Kho hàng</td>");
            sb.Append("<td>" + Resource.Order_Status + "</td>");// sb.Append("<td>Trạng thái</td>");
            sb.Append("</tr>");
            var index = 0;
            if (model.ListItems.Any())
            {
                for (int i = 0; i < model.ListItems.Count(); i++)
                {
                    var item = model.ListItems[i];

                    var tmpStatus = Common.Helper.EnumHelper.GetEnumDescription< Common.Emums.OrderPackageStatus>(item.Status);

                    //switch (item.Status)
                    //{
                    //    case (byte)Common.Emums.OrderPackageStatus.Await:
                    //        tmpStatus = "Chờ nhập kho";
                    //        break;
                    //    case (byte)Common.Emums.OrderPackageStatus.InStock:
                    //        tmpStatus = "Đang trong kho";
                    //        break;
                    //    case (byte)Common.Emums.OrderPackageStatus.Shipping:
                    //        tmpStatus = "Đang điều chuyển";
                    //        break;
                    //    case (byte)Common.Emums.OrderPackageStatus.LoseCode:
                    //        tmpStatus = "Mất mã";
                    //        break;
                    //    case (byte)Common.Emums.OrderPackageStatus.Complete:
                    //        tmpStatus = "Hoàn thành";
                    //        break;
                    //    case (byte)Common.Emums.OrderPackageStatus.Lost:
                    //        tmpStatus = "Mất hàng";
                    //        break;
                    //    default:
                    //        break;
                    //}
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", ++index);
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", item.Code));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0:dd/MM/yyyy}\")", item.Created));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", item.OrderCode));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", string.Format("{0:0.##}", item.WeightActual)));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", item.Size));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", item.CurrentWarehouseName));
                    sb.AppendFormat("<td>{0}</td>", tmpStatus);
                    sb.AppendFormat("</tr>").AppendLine();
                }
            }
            sb.Append("</table>");
            var fileName = string.Format("KienHang_{0:ddMMyyyy}.xls", DateTime.Now);
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