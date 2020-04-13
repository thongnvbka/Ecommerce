using Common.Constant;
using Common.Helper;
using Common.Items;
using Library.DbContext.Entities;
using Library.ViewModels.Account;
using ProjectV.LikeOrderThaiLan.com.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Emums;
using Newtonsoft.Json;
using AutoMapper;
using System.Web;
using System.IO;
using System.Configuration;
using Common.Host;
using ResourcesLikeOrderThaiLan;

namespace ProjectV.LikeOrderThaiLan.com.Areas.CMS.Controllers
{
    [Authorize]
    public class TicketController : BaseController
    {
        // GET: CMS/Ticket

        //TODO  DANH SACH KHIẾU NẠI
        public ActionResult Ticket()
        {
            // lấy danh sách loại khiếu nại
            var complainTypeService = new List<dynamic>();
            var listtype = UnitOfWork.ComplainTypeRepo.Find(x => /*x.IsParent &&*/ x.ParentId == 0 && !x.IsDelete).OrderBy(x => x.Index).ToList();
            foreach (var item in listtype)
            {
                complainTypeService.Add(new { Text = item.Name, Value = item.Id });
            }
            ViewBag.ListComplainTypeService1 = JsonConvert.SerializeObject(complainTypeService.ToList());

           
            ViewBag.ActiveListTicket = "cl_on";
            return View();
        }

        #region [Tạo Ticket]
        //TODO TẠO YÊU CẦU KHIẾU NẠI
        public ActionResult CreateTicket(long id=0)
        {
            // lấy danh sách loại khiếu nại
            var complainTypeService = new List<dynamic>();
            var listtype = UnitOfWork.ComplainTypeRepo.Find(x => /*x.IsParent && */x.ParentId == 0 && !x.IsDelete).OrderBy(x => x.Index).ToList();
            foreach (var item in listtype)
            {
                complainTypeService.Add(new { Text = item.Name, Value = item.Id });
            }
            ViewBag.ListComplainTypeService = JsonConvert.SerializeObject(complainTypeService.ToList());
            var objOrder = UnitOfWork.OrderRepo.SingleOrDefault(x => x.Id == id && !x.IsDelete && x.CustomerId == CustomerState.Id);
            if (objOrder != null)
            {
                ViewBag.OrderId = id;
                ViewBag.OrderCode = objOrder.Code;
            }
            else
            {
                ViewBag.OrderId = id;
                ViewBag.OrderCode = "0";
            }
          
            ViewBag.ActiveCreateTicket = "cl_on";
            return View();
        }

        // Tạo ticket
        [HttpPost]
        public async Task<JsonResult> CreateTicket(Complain complain)
        {

            var ticket = new Complain();
            Mapper.Map(complain, ticket);

            //1. Lấy thông tin đơn hàng
            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(s => s.Id == complain.OrderId && s.CustomerId == CustomerState.Id && !s.IsDelete);
            if (order == null)
            {
                return Json(new { status = Result.Failed, msg = "สั่งซื้อสินค้าที่ไม่มีอยู่หรือถูกลบออก!" }, JsonRequestBehavior.AllowGet);
                //return Json(new { status = Result.Failed, msg = "Đơn hàng không tồn tại hoặc bị xóa!" }, JsonRequestBehavior.AllowGet);
            }

            //2. Lấy thông tin loại khiếu nại
            var complainType = await UnitOfWork.ComplainTypeRepo.FirstOrDefaultAsync(s => s.Id == complain.TypeService && !s.IsDelete);
            if (complainType == null)
            {
                return Json(new { status = Result.Failed, msg = "ไม่เคยบ่นหรือเลือกประเภทของการร้องเรียนไม่ได้อยู่หรือถูกลบออก!" }, JsonRequestBehavior.AllowGet);
                //return Json(new { status = Result.Failed, msg = "Chưa chọn loại khiếu nại hoặc Loại khiếu nại không tồn tại hoặc đã bị xóa!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Kiểm tra thông tin khách hàng
            var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(s => s.Id == CustomerState.Id && !s.IsDelete);
            if (customer == null)
            {
                return Json(new { status = Result.Failed, msg = Resource.AccountPassForget_NotAccount }, JsonRequestBehavior.AllowGet);
            }

            //4. Gán dữ liệu
            ticket.TypeServiceName = complainType.Name;

            ticket.CustomerId = CustomerState.Id;
            ticket.CustomerName = CustomerState.FullName;

            ticket.OrderId = order.Id;
            ticket.OrderType = order.Type;
            ticket.OrderCode = order.Code;

            ticket.CreateDate = DateTime.Now;
            ticket.LastUpdateDate = DateTime.Now;

            ticket.SystemId = customer.SystemId;
            ticket.SystemName = customer.SystemName;

            ticket.RequestMoney = complain.RequestMoney;
            ticket.Status = (byte)ComplainStatus.Wait;

            //5. Lưu Database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    UnitOfWork.ComplainRepo.Add(ticket);
                    UnitOfWork.ComplainRepo.Save();

                    //10. Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ComplainId = ticket.Id;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = ticket.CustomerId;
                    conplainHistory.CustomerName = ticket.CustomerName;
                    conplainHistory.Status = (byte)ComplainStatus.Wait;
                    conplainHistory.Content = "สร้างข้อร้องเรียนของลูกค้า";
                    //conplainHistory.Content = "Khách hàng tạo khiếu nại";

                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();


                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    OutputLog.WriteOutputLog(ex);
                    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.CreateComplainIsSuccess, complain = complain }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region [Chi tiết khiếu nại]

        //TODO chi tiết đơn hàng order
        public ActionResult DetailTicket()
        {

           
            ViewBag.ActiveTicket = "cl_on";
            return View();
        }
        //TODO Chi tiết khiếu nại
        [HttpPost]
        public async Task<JsonResult> DetailTicket(int ticketId)
        {
            var complainDetail = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(m => m.Id == ticketId && m.CustomerId == CustomerState.Id);
            var comments = UnitOfWork.ComplainUserRepo.GetDetail(ticketId);
            //var ticketComment = UnitOfWork.ComplainUserRepo

            if (complainDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Error!", complainDetail = complainDetail, comments = comments }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = Result.Succeed, msg = "", complainDetail = complainDetail, comments = comments }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //todo DANH SÁCH KHIẾU NẠI
        
        public JsonResult GetListComplain(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = new ComplainModel();
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
            model = UnitOfWork.ComplainRepo.GetAllByLinq(pageInfor, seachInfor);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //Load ảnh
        [HttpPost]
        public ActionResult UploadImages()
        {
            var r = new List<Attachment>();

            if (!Request.Files.Cast<string>().Any())
                return Json(r);

            if (!ValidateBlackListExtensions(Request))
            {
                return Json(-5);
            }

            var statuses = new List<object>();
            var headers = Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeImage(Request, statuses);
            }
            else
            {
                UploadPartialImage(headers["X-File-Name"], Request, statuses);
            }

            return JsonCamelCaseResult(statuses, JsonRequestBehavior.AllowGet);
        }

        private bool ValidateBlackListExtensions(HttpRequestBase request)
        {
            if (request.Files == null)
            {
                throw new HttpRequestValidationException(
                    "Attempt to upload chunked file containing more than one fragment per request");
            }
            for (var i = 0; i < request.Files.Count; i++)
            {
                var ext = Path.GetExtension(request.Files[i].FileName);
                if (string.IsNullOrEmpty(ext))
                {
                    return false;
                }

                ext = ext.ToLower();

                if (GetBlackListExtensions().Any(f => f.Equals(ext)))
                {
                    return false;
                }
            }
            return true;
        }
        private void UploadWholeImage(HttpRequestBase request, IList<object> statuses)
        {
            for (var i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];
                if (file == null)
                    continue;

                // get extension file & get file name
                //var ext = Path.GetExtension(file.FileName) ?? "";
                var name = $"{DateTime.Now:ddMMyyyyssfffffff}_{Path.GetFileName(file.FileName)}";

                // create path upload
                var path = $"/upload/{DateTime.Now:yyyy/MM/dd}/";

                // create folder
                var mapPath = Server.MapPath(path);

                if (!Directory.Exists(mapPath))
                    Directory.CreateDirectory(mapPath);

                // svae file
                file.SaveAs(mapPath + name);

                //var url = GroupContentType(ext.Replace(".", "").ToLower()) == "Ảnh"
                //    ? "/Upload/Resize/" + Encryptor.Base64Encode(path + name) :
                //    "/Upload/Download/" + Encryptor.Base64Encode(path + name);

                var url = Encryptor.Base64Encode(path + name);

                // add file meta into status
                statuses.Add(new { url = url, path = (path + name) });
            }
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        private void UploadPartialImage(string fileName, HttpRequestBase request, IList<object> statuses)
        {
            if (request.Files.Count != 1 || request.Files[0] == null)
                throw new HttpRequestValidationException(
                    "Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];

            var inputStream = file.InputStream;

            // get extension file & get file name
            //var ext = Path.GetExtension(file.FileName) ?? "";
            var name = $"{DateTime.Now:ddMMyyyyssfffffff}_{Path.GetFileName(file.FileName)}";

            // build path folder
            var path = $"/upload/{DateTime.Now:yyyy/MM/dd}/";

            // create folder
            var mapPath = Server.MapPath(path);
            if (!Directory.Exists(mapPath))
                Directory.CreateDirectory(mapPath);

            // buidl full path on sẻver
            var fullName = Path.Combine(mapPath, Path.GetFileName(fileName) ?? "");

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }

            var url = Encryptor.Base64Encode(path + name);

            // add file meta into status
            statuses.Add(new { url = url, path = (path + name) });
        }

        protected string[] GetBlackListExtensions()
        {
            string[] blackList = { ".exe", ".cshtml", ".vbhtml", ".aspx", ".ascx", ".msi", ".bin", ".js", ".bat", ".cmd", ".ps1", ".reg", ".rgs", ".ws", ".wsf" };
            var blacklistConfig = ConfigurationManager.AppSettings["BlackListExtentions"];
            if (!string.IsNullOrWhiteSpace(blacklistConfig))
            {
                blacklistConfig = blacklistConfig.Replace(" ", "").ToLower();
                var split = blacklistConfig.Split(',', ';');
                if (split.Length != 0)
                {
                    blackList = split;
                }
            }
            return blackList;
        }
        
        public JsonResult GetOrderSearch(string keyword, int? page)
        {
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);
            var customer = UnitOfWork.CustomerRepo.FirstOrDefault(s => s.Id == CustomerState.Id && !s.IsDelete);
            if (customer == null)
            {
                return Json(new { status = MsgType.Error, msg = "ลูกค้าไม่มีข้อมูลหรือถูกบล็อกออกเเล้ว!" }, JsonRequestBehavior.AllowGet);
            }
            var listOrder = UnitOfWork.OrderRepo.Find(
                   out totalRecord,
                   x => !x.IsDelete
                   && x.CustomerId == CustomerState.Id
                   && x.SystemId == customer.SystemId
                   && (x.Code.Contains(keyword) || (x.CustomerEmail.Contains(keyword)) || (x.CustomerName.Contains(keyword)) || (x.CustomerPhone.Contains(keyword))),
                   x => x.OrderByDescending(y => y.Code),
                   page ?? 1,
                   10
              ).ToList();

            return Json(new { incomplete_results = true, total_count = totalRecord, items = listOrder.Select(x => new { id = x.Id, code = x.Code, systemName = x.SystemName }) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchAutoComplete(string q, int top)
        {
            if (CustomerState == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            byte tmpStatus = 9;
            tmpStatus = (byte)Common.Emums.OrderStatus.Finish;
            var customerId = CustomerState.Id;
            var model = UnitOfWork.ExhibitionRepo.SearchAutoCompleteByLinq(SystemId, customerId, q, top, tmpStatus);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

       
    }
}