using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cms.Attributes;
using Common.Constant;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;

namespace Cms.Controllers
{
    [Authorize]
    public class SourceController : BaseController
    {
        #region Order tìm nguồn
        [HttpPost]
        [LogTracker(EnumAction.View, EnumPage.StockQuotesNew)]
        public async Task<JsonResult> StockQuotesNew(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            //1. Tạo các biến
            long totalRecord;

            //2. Lấy dữ liệu

            var listOrder = await UnitOfWork.SourceRepo.FindAsync(
                  out totalRecord,
                  x =>
                      (status == -1 || x.Status == status) && x.Code.Contains(keyword)
                      && (systemId == -1 || x.SystemId == systemId)
                      && (customerId == null || x.CustomerId == customerId)
                      && (userId == null || x.UserId == userId)
                      && (dateStart == null || x.CreateDate >= dateStart)
                      && (dateEnd == null || x.CreateDate <= dateEnd)
                      && (x.Status == (byte)SourceStatus.WaitProcess)
                      && x.SourceSupplierId == null
                      && x.UserId == null && !x.IsDelete,
                  x => x.OrderByDescending(y => y.CreateDate),
                  page,
                  pageSize
              );
            if (checkExactCode)
            {
                keyword = RemoveCode(keyword);
                listOrder = await UnitOfWork.SourceRepo.FindAsync(
                  out totalRecord,
                  x =>
                      (status == -1 || x.Status == status) && x.Code == keyword
                      && (systemId == -1 || x.SystemId == systemId)
                      && (customerId == null || x.CustomerId == customerId)
                      && (userId == null || x.UserId == userId)
                      && (dateStart == null || x.CreateDate >= dateStart)
                      && (dateEnd == null || x.CreateDate <= dateEnd)
                      && (x.Status == (byte)SourceStatus.WaitProcess)
                      && x.SourceSupplierId == null
                      && x.UserId == null && !x.IsDelete,
                  x => x.OrderByDescending(y => y.CreateDate),
                  page,
                  pageSize
              );
            }

            if (listOrder.Any())
            {
                var ids = listOrder.Select(x => x.Id);
                var listChat = await UnitOfWork.OrderCommentRepo.FindAsync(x => !x.IsRead.Value && ids.Contains(x.OrderId) && x.CustomerId != null);

                //3. Lấy thông tin chat
                foreach (var item in listOrder)
                {
                    item.Chat = listChat.Count(x => x.OrderId == item.Id);
                }
            }

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.View, EnumPage.StockQuotes)]
        public async Task<JsonResult> StockQuotes(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            //1. Tạo các biến
            long totalRecord;

            //2. Lấy dữ liệu

            var listOrder = await UnitOfWork.SourceRepo.FindAsync(
                  out totalRecord,
                  x =>
                      (status == -1 || x.Status == status) && x.Code.Contains(keyword)
                      && (systemId == -1 || x.SystemId == systemId)
                      && (customerId == null || x.CustomerId == customerId)
                      && (userId == null || x.UserId == userId)
                      && (dateStart == null || x.CreateDate >= dateStart)
                      && (dateEnd == null || x.CreateDate <= dateEnd)
                      && (x.Status >= (byte)SourceStatus.Process)
                      //&& x.SourceSupplierId == null
                      && x.UserId != null && !x.IsDelete,
                  x => x.OrderByDescending(y => y.CreateDate),
                  page,
                  pageSize
              );
            if (checkExactCode)
            {
                keyword = RemoveCode(keyword);
                listOrder = await UnitOfWork.SourceRepo.FindAsync(
                  out totalRecord,
                  x =>
                      (status == -1 || x.Status == status) && x.Code == (keyword)
                      && (systemId == -1 || x.SystemId == systemId)
                      && (customerId == null || x.CustomerId == customerId)
                      && (userId == null || x.UserId == userId)
                      && (dateStart == null || x.CreateDate >= dateStart)
                      && (dateEnd == null || x.CreateDate <= dateEnd)
                      && (x.Status >= (byte)SourceStatus.Process)
                      //&& x.SourceSupplierId == null
                      && x.UserId != null && !x.IsDelete,
                  x => x.OrderByDescending(y => y.CreateDate),
                  page,
                  pageSize
              );
            }

            if (listOrder.Any())
            {
                var ids = listOrder.Select(x => x.Id);
                var listChat = await UnitOfWork.OrderCommentRepo.FindAsync(x => !x.IsRead.Value && ids.Contains(x.OrderId) && x.CustomerId != null);

                //3. Lấy thông tin chat
                foreach (var item in listOrder)
                {
                    item.Chat = listChat.Count(x => x.OrderId == item.Id);
                }
            }

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetData()
        {
            var exchangeRate = ExchangeRate();
            //2. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "", exchangeRate }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.View, EnumPage.StockQuotes)]
        public async Task<JsonResult> GetSourceDetail(int id)
        {
            //1. Kiểm tra thông tin Order ký gửi

            var source = await UnitOfWork.SourceRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Id == id);

            if (source == null)
            {
                return Json(new { status = MsgType.Error, msg = "Hủy Contract thành công!" }, JsonRequestBehavior.AllowGet);
            }

            var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => x.Id == source.CustomerId && !x.IsDelete);
            var listDetail = await UnitOfWork.SourceDetailRepo.FindAsync(x => x.SourceId == source.Id && !x.IsDelete);
            var listSupplier = await UnitOfWork.SourceSupplierRepo.FindAsync(x => x.SourceId == source.Id && !x.IsDelete);
            var listHistory = await UnitOfWork.OrderHistoryRepo.FindAsync(x => x.OrderId == source.Id && x.Type == source.Type, query => query.OrderByDescending(m => m.CreateDate), null);

            var userOrder = source.UserId != null ? await UnitOfWork.UserRepo.GetUserToOfficeOrder(source.UserId.Value) : null;

            //2. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "", source, listDetail, customer, listSupplier, listHistory, userOrder }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Add, EnumPage.StockQuotes)]
        public async Task<JsonResult> SaveSupplier(int id, SourceSupplier sourceSupplier)
        {
            var timeDate = DateTime.Now;

            var source = await UnitOfWork.SourceRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);

            if (source == null)
            {
                return Json(new { status = MsgType.Error, msg = "Ticket find source does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var supplier = new SourceSupplier
                    {
                        SourceId = source.Id,
                        Price = sourceSupplier.Price,
                        ExchangeRate = ExchangeRate(),
                        ExchangePrice = sourceSupplier.Price * ExchangeRate(),
                        TotalPrice = sourceSupplier.Price * sourceSupplier.Quantity,
                        TotalExchange = sourceSupplier.Price * sourceSupplier.Quantity * ExchangeRate(),
                        Quantity = sourceSupplier.Quantity,
                        Name = sourceSupplier.Name,
                        Status = 0,
                        Link = sourceSupplier.Link,
                        Description = sourceSupplier.Description,
                        Created = timeDate,
                        LastUpdate = timeDate,
                        IsDelete = false,
                        ShipMoney = 0,
                        ActiveDate = timeDate,
                        LimitDate = timeDate.AddDays(3)
                    };

                    UnitOfWork.SourceSupplierRepo.Add(supplier);
                    await UnitOfWork.SourceSupplierRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            var list = await UnitOfWork.SourceSupplierRepo.FindAsync(x => !x.IsDelete && x.SourceId == source.Id);

            return Json(new { status = MsgType.Success, msg = "Add successful", list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.StockQuotes)]
        public async Task<JsonResult> UpdateSupplier(SourceSupplier sourceSupplier)
        {
            var timeDate = DateTime.Now;
            var supplier = await UnitOfWork.SourceSupplierRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == sourceSupplier.Id);
            if (supplier == null)
            {
                return Json(new { status = MsgType.Error, msg = "Supplier information does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var source = await UnitOfWork.SourceRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == supplier.SourceId);

            if (source == null)
            {
                return Json(new { status = MsgType.Error, msg = "Ticket find source does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    supplier.Price = sourceSupplier.Price;
                    supplier.ExchangePrice = supplier.Price * supplier.ExchangeRate;
                    supplier.TotalPrice = sourceSupplier.Price * sourceSupplier.Quantity;
                    supplier.TotalExchange = supplier.TotalPrice * supplier.ExchangeRate;
                    supplier.Quantity = sourceSupplier.Quantity;
                    supplier.Name = sourceSupplier.Name;
                    supplier.Status = sourceSupplier.Status;
                    supplier.Link = sourceSupplier.Link;
                    supplier.Description = sourceSupplier.Description;
                    supplier.ShipMoney = sourceSupplier.ShipMoney;

                    await UnitOfWork.SourceSupplierRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = MsgType.Success, msg = "Updated successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Delete, EnumPage.StockQuotes)]
        public async Task<JsonResult> DeleteSupplier(int id)
        {
            var timeDate = DateTime.Now;
            var supplier = await UnitOfWork.SourceSupplierRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);
            if (supplier == null)
            {
                return Json(new { status = MsgType.Error, msg = "Supplier information does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var source = await UnitOfWork.SourceRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == supplier.SourceId);

            if (source == null)
            {
                return Json(new { status = MsgType.Error, msg = "Ticket find source does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    supplier.LastUpdate = timeDate;
                    supplier.IsDelete = true;

                    await UnitOfWork.SourceSupplierRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = MsgType.Success, msg = "Deleted successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Add, EnumPage.StockQuotes)]
        public async Task<JsonResult> SaveDetail(int id, SourceDetail sourceDetail)
        {
            var timeDate = DateTime.Now;

            var source = await UnitOfWork.SourceRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);

            if (source == null)
            {
                return Json(new { status = MsgType.Error, msg = "Phiếu tìm nguồn does not exist hoặc đã bị xóa!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var detail = new SourceDetail
                    {
                        SourceId = source.Id,
                        Name = sourceDetail.Name,
                        Quantity = sourceDetail.Quantity,
                        BeginAmount = 0,
                        Price = 0m,
                        ExchangeRate = ExchangeRate(),
                        ExchangePrice = 0m,
                        TotalPrice = 0m,
                        TotalExchange = 0m,
                        Note = sourceDetail.Note,
                        Status = 0,
                        Link = sourceDetail.Link,
                        QuantityBooked = sourceDetail.Quantity,
                        Properties = sourceDetail.Properties,
                        HashTag = $"{MyCommon.Ucs2Convert(sourceDetail.Name)}",
                        CategoryId = sourceDetail.CategoryId,
                        CategoryName = sourceDetail.CategoryName,
                        Created = timeDate,
                        LastUpdate = timeDate,
                        IsDelete = false,
                        UniqueCode = string.Empty,
                        Size = string.Empty,
                        Color = string.Empty,
                        ImagePath1 = sourceDetail.ImagePath1,
                        ImagePath2 = sourceDetail.ImagePath2,
                        ImagePath3 = sourceDetail.ImagePath3,
                        ImagePath4 = sourceDetail.ImagePath4
                    };

                    UnitOfWork.SourceDetailRepo.Add(detail);
                    await UnitOfWork.SourceDetailRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            var list = await UnitOfWork.SourceDetailRepo.FindAsync(x => !x.IsDelete && x.SourceId == source.Id);

            return Json(new { status = MsgType.Success, msg = "Add successful", list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.StockQuotes)]
        public async Task<JsonResult> UpdateDetail(SourceDetail sourceDetail)
        {
            var timeDate = DateTime.Now;
            var detail = await UnitOfWork.SourceDetailRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == sourceDetail.Id);
            if (detail == null)
            {
                return Json(new { status = MsgType.Error, msg = "Chi tiết phiếu tìm nguồn does not exist hoặc đã bị xóa!" }, JsonRequestBehavior.AllowGet);
            }

            var source = await UnitOfWork.SourceRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == detail.SourceId);

            if (source == null)
            {
                return Json(new { status = MsgType.Error, msg = "Ticket find source does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    detail.Name = sourceDetail.Name;
                    detail.Link = sourceDetail.Link;
                    detail.Note = sourceDetail.Note;
                    detail.Properties = sourceDetail.Properties;
                    detail.CategoryId = sourceDetail.CategoryId;
                    detail.CategoryName = sourceDetail.CategoryName;
                    detail.Quantity = sourceDetail.Quantity;
                    detail.ImagePath1 = sourceDetail.ImagePath1;
                    detail.ImagePath2 = sourceDetail.ImagePath2;
                    detail.ImagePath3 = sourceDetail.ImagePath3;
                    detail.ImagePath4 = sourceDetail.ImagePath4;
                    detail.LastUpdate = timeDate;

                    await UnitOfWork.SourceDetailRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = MsgType.Success, msg = "Updated successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Delete, EnumPage.StockQuotes)]
        public async Task<JsonResult> DeleteDetail(int id)
        {
            var timeDate = DateTime.Now;
            var detail = await UnitOfWork.SourceDetailRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);
            if (detail == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order detail does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var source = await UnitOfWork.SourceRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == detail.SourceId);

            if (source == null)
            {
                return Json(new { status = MsgType.Error, msg = "Ticket find source does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    detail.LastUpdate = timeDate;
                    detail.IsDelete = true;

                    await UnitOfWork.SourceSupplierRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = MsgType.Success, msg = "Deleted successfully" }, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.Add, EnumPage.StockQuotes)]
        public async Task<JsonResult> Save(Source source, List<SourceDetail> listDetails, List<SourceSupplier> listsSuppliers, byte type)
        {
            var timeDate = DateTime.Now;

            //check khách hàng
            var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => x.Id == source.CustomerId && !x.IsDelete);
            if (customer == null)
            {
                return Json(new { status = MsgType.Error, msg = "Not customer selected yet!" }, JsonRequestBehavior.AllowGet);
            }

            if (!listDetails.Any())
            {
                return Json(new { status = MsgType.Error, msg = "Not order detail yet!" }, JsonRequestBehavior.AllowGet);
            }

            if (type == 2)
            {
                if (!listsSuppliers.Any())
                {
                    return Json(new { status = MsgType.Error, msg = "No list of suppliers!" },
                        JsonRequestBehavior.AllowGet);
                }

                if (listsSuppliers.Count < 3)
                {
                    return Json(new { status = MsgType.Error, msg = "Must have 3 suppliers!" },
                        JsonRequestBehavior.AllowGet);
                }
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    source.Id = 0;
                    source.Code = string.Empty;
                    source.SystemId = customer.SystemId;
                    source.SystemName = customer.SystemName;
                    source.WarehouseId = customer.WarehouseId ?? 0;
                    source.WarehouseName = customer.WarehouseName;
                    source.CustomerId = customer.Id;
                    source.CustomerName = customer.FullName;
                    source.CustomerEmail = customer.Email;
                    source.CustomerPhone = customer.Phone;
                    source.CustomerAddress = customer.Address;
                    source.Status = (byte)SourceStatus.Process;
                    source.UserId = UserState.UserId;
                    source.UserFullName = UserState.FullName;
                    source.OfficeId = UserState.OfficeId;
                    source.OfficeName = UserState.OfficeName;
                    source.OfficeIdPath = UserState.OfficeIdPath;
                    source.CreatedOfficeIdPath = UserState.OfficeIdPath;
                    source.CreatedUserId = UserState.UserId;
                    source.CreatedUserFullName = UserState.FullName;
                    source.CreatedOfficeId = UserState.OfficeId;
                    source.CreatedOfficeName = UserState.OfficeName;
                    source.CreateDate = timeDate;
                    source.UpdateDate = timeDate;
                    source.TypeService = 0;
                    source.ServiceMoney = 0;
                    source.IsDelete = false;
                    source.ShipMoney = 0m;
                    source.Type = (byte)OrderType.Source;
                    source.UnsignName = string.Empty;


                    UnitOfWork.SourceRepo.Add(source);
                    await UnitOfWork.SourceRepo.SaveAsync();

                    //order detail
                    foreach (var item in listDetails)
                    {
                        item.Created = timeDate;
                        item.LastUpdate = timeDate;
                        item.SourceId = source.Id;
                        item.ExchangeRate = item.ExchangeRate;
                        item.IsDelete = false;

                        UnitOfWork.SourceDetailRepo.Add(item);
                    }

                    await UnitOfWork.SourceDetailRepo.SaveAsync();

                    //danh sách nhà cung cấp
                    foreach (var item in listsSuppliers)
                    {
                        item.Created = timeDate;
                        item.LastUpdate = timeDate;
                        item.SourceId = source.Id;
                        item.ExchangeRate = item.ExchangeRate;
                        item.ExchangePrice = item.ExchangeRate * item.Price;
                        item.TotalPrice = item.ExchangePrice * item.Quantity;
                        item.TotalExchange = item.TotalPrice * item.ExchangeRate;
                        item.Status = 0;
                        item.ShipMoney = 0m;
                        item.ActiveDate = timeDate;
                        item.LimitDate = timeDate.AddDays(3);
                        item.IsDelete = false;

                        UnitOfWork.SourceSupplierRepo.Add(item);
                    }

                    await UnitOfWork.SourceSupplierRepo.SaveAsync();


                    // Cập nhật lại Mã cho Order
                    var orderNo = UnitOfWork.OrderRepo.Count(x => x.CustomerId == customer.Id && x.Id <= source.Id);
                    source.Code = $"{customer.Code}-{orderNo}";

                    source.UnsignName = MyCommon.Ucs2Convert($"{source.Code} {source.CustomerName} {source.WarehouseName}").ToLower();

                    await UnitOfWork.SourceRepo.SaveAsync();

                    //Thêm lịch sử Order
                    if (source.CustomerId != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeDate,
                            Content = $"Add Ticket find source and get handle Order by {UserState.FullName}",
                            CustomerId = source.CustomerId.Value,
                            CustomerName = source.CustomerName,
                            OrderId = (int)source.Id,
                            Status = source.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = source.Type
                        });

                    if (type == 2)
                    {
                        source.Status = (byte)SourceStatus.WaitingChoice;
                        await UnitOfWork.SourceRepo.SaveAsync();

                        //Thêm lịch sử Order
                        if (source.CustomerId != null)
                            UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                            {
                                CreateDate = timeDate,
                                Content = $"Send Supplier information For customers to select",
                                CustomerId = source.CustomerId.Value,
                                CustomerName = source.CustomerName,
                                OrderId = (int)source.Id,
                                Status = source.Status,
                                UserId = UserState.UserId,
                                UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                Type = source.Type
                            });
                    }

                    await UnitOfWork.OrderHistoryRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            //2. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "Add Ticket find source thành công" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.StockQuotes)]
        public async Task<JsonResult> Update(Source source, byte type)
        {
            var timeDate = DateTime.Now;

            var sourceUpdate = await UnitOfWork.SourceRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == source.Id);

            if (sourceUpdate == null)
            {
                return Json(new { status = MsgType.Error, msg = "Phiếu tìm nguồn does not exist hoặc đã bị xóa!" }, JsonRequestBehavior.AllowGet);
            }

            //check khách hàng
            var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => x.Id == source.CustomerId && !x.IsDelete);
            if (customer == null)
            {
                return Json(new { status = MsgType.Error, msg = "Not customer selected yet!" }, JsonRequestBehavior.AllowGet);
            }


            var listDetails = await UnitOfWork.SourceDetailRepo.FindAsync(x => !x.IsDelete && x.SourceId == sourceUpdate.Id);
            var listsSuppliers = await UnitOfWork.SourceSupplierRepo.FindAsync(x => !x.IsDelete && x.SourceId == sourceUpdate.Id);
            //check kho nhận ký gửi
            if (!listDetails.Any())
            {
                return Json(new { status = MsgType.Error, msg = "Not order detail yet!" }, JsonRequestBehavior.AllowGet);
            }

            if (type == 2)
            {

                if (!listsSuppliers.Any())
                {
                    return Json(new { status = MsgType.Error, msg = "No list of suppliers!" }, JsonRequestBehavior.AllowGet);
                }

                if (listsSuppliers.Count < 3)
                {
                    return Json(new { status = MsgType.Error, msg = "Must have 3 suppliers!" }, JsonRequestBehavior.AllowGet);
                }
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    sourceUpdate.SystemId = customer.SystemId;
                    sourceUpdate.SystemName = customer.SystemName;
                    sourceUpdate.WarehouseId = customer.WarehouseId ?? 0;
                    sourceUpdate.WarehouseName = customer.WarehouseName;
                    sourceUpdate.CustomerId = customer.Id;
                    sourceUpdate.CustomerName = customer.FullName;
                    sourceUpdate.CustomerEmail = customer.Email;
                    sourceUpdate.CustomerPhone = customer.Phone;
                    sourceUpdate.CustomerAddress = customer.Address;
                    sourceUpdate.UpdateDate = timeDate;
                    sourceUpdate.UserNote = source.UserNote;
                    sourceUpdate.AnalyticSupplier = source.AnalyticSupplier;

                    await UnitOfWork.SourceRepo.SaveAsync();

                    if (type == 2)
                    {
                        sourceUpdate.Status = (byte)SourceStatus.WaitingChoice;
                        await UnitOfWork.SourceRepo.SaveAsync();

                        //Thêm lịch sử Order
                        if (sourceUpdate.CustomerId != null)
                            UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                            {
                                CreateDate = timeDate,
                                Content = $"Send Supplier information For customers to choose",
                                CustomerId = sourceUpdate.CustomerId.Value,
                                CustomerName = source.CustomerName,
                                OrderId = (int)source.Id,
                                Status = source.Status,
                                UserId = UserState.UserId,
                                UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                Type = source.Type
                            });
                    }

                    await UnitOfWork.OrderHistoryRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            //2. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "Perform a successful operation" }, JsonRequestBehavior.AllowGet);
        }


        #region [Lấy danh sách Order]

        [HttpPost]
        [LogTracker(EnumAction.View, EnumPage.OrderSourcing)]
        public async Task<JsonResult> OrderSourcing(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId)
        {
            //1. Khởi tạo các biến
            long totalRecord;

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status) && (systemId == -1 || x.SystemId == systemId) && x.UnsignName.Contains(keyword)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.UserId == userId)
                    && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId)
                    && x.Status == (byte)OrderStatus.Order
                    && x.Type == (byte)OrderType.Source,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );

            if (listOrder.Any())
            {
                var ids = listOrder.Select(x => x.Id);
                var listChat = await UnitOfWork.OrderCommentRepo.FindAsync(x => !x.IsRead.Value && ids.Contains(x.OrderId) && x.CustomerId != null);

                //3. Lấy thông tin chat
                foreach (var item in listOrder)
                {
                    item.Chat = listChat.Count(x => x.OrderId == item.Id);
                }
            }

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion
    }
}