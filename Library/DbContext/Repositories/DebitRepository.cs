using Library.DbContext.Entities;
using Library.UnitOfWork;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Data.Entity;
using Library.Models;
using Common.Emums;

namespace Library.DbContext.Repositories
{
    public class DebitRepository : Repository<Debit>
    {
        public DebitRepository(ProjectXContext context) : base(context)
        {

        }
        #region [Danh sách Debit]
        public Task<List<DebitDetailHistory>> GetAllDebitList(out long totalRecord, int page, int pageSize, string keyword,
            int status, DateTime? dateStart, DateTime? dateEnd, int? userId, int? subjectId, int? subjectTypeId, int? treasureId, int? financeFundId, int type, UserState userState)
        {
            //var tmpCollect = (byte)DebitType.Collect;
            //var tmpReturn = (byte)DebitType.Return;
            var query =
                Db.Debit.Where(x =>
                        (x.Code.Contains(keyword) || x.OrderCode.Contains(keyword) || x.SubjectEmail.Contains(keyword) || x.SubjectName.Contains(keyword) || x.SubjectCode.Contains(keyword) || x.SubjectPhone.Contains(keyword))
                        && !x.IsDelete
                       && (financeFundId == null || financeFundId == 0 || x.FinanceFundId == financeFundId)
                        && (treasureId == null || treasureId == 0 || x.TreasureId == treasureId)
                        && (userId == null || userId == 0 || x.UserId == userId)
                        && (status == -1 || x.Status == status)
                        && (subjectId == -1 || x.SubjectId == subjectId)
                        && (subjectTypeId == -1 || x.SubjectTypeId == subjectTypeId)
                        && x.Created >= dateStart && x.Created <= dateEnd
                    )
                    .GroupJoin(
                        Db.DebitHistorys.Where(s=>
                        (type == -1 || s.DebitType == type)),
                        debit => debit.Id,
                        debitHis => debitHis.DebitId.Value,
                        (d, db) => new { d, db }
                    )
                    .Select(s => new DebitDetailHistory()
                    {
                        Id = s.d.Id,
                        Code = s.d.Code,
                        Status = s.d.Status,
                        MustCollectMoney = s.d.MustCollectMoney,
                        MustReturnMoney = s.d.MustReturnMoney,
                        TreasureId = s.d.TreasureId,
                        TreasureIdd = s.d.TreasureIdd,
                        TreasureName = s.d.TreasureName,
                        FinanceFundId = s.d.FinanceFundId,
                        FinanceFundName = s.d.FinanceFundName,
                        SubjectTypeId = s.d.SubjectTypeId,
                        SubjectTypeName = s.d.SubjectTypeName,
                        AccountantSubjectId = s.d.AccountantSubjectId,
                        AccountantSubjectName = s.d.AccountantSubjectName,
                        SubjectId = s.d.SubjectId,
                        SubjectCode = s.d.SubjectCode,
                        SubjectName = s.d.SubjectName,
                        SubjectPhone = s.d.SubjectPhone,
                        SubjectEmail = s.d.SubjectEmail,
                        OrderId = s.d.OrderId,
                        OrderCode = s.d.OrderCode,
                        OrderType = s.d.OrderType,
                        UserId = s.d.UserId,
                        UserCode = s.d.UserCode,
                        UserName = s.d.UserName,
                        IsSystem = s.d.IsSystem,
                        Created = s.d.Created,
                        IsDelete = s.d.IsDelete,
                        //TotalMustCollectMoney = s.db.Where(m => m.DebitType == tmpCollect).Sum(y => y.Money),
                        //TotalMustReturnMoney = s.db.Where(m => m.DebitType == tmpReturn).Sum(y => y.Money),
                        ListHistory = s.db.Where(k => k.DebitId == s.d.Id).ToList()
                    })
                    .OrderByDescending(x => x.Created);

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        #endregion

        public UpdateDebitResult UpdateDebit(AutoUpdateDebitModel model)
        {
            var debitHistory = new DebitHistory();

            //1. Kiểm tra số tiền yêu cầu cập nhật phải > 0
            if (model.Money < 0)
            {
                return new UpdateDebitResult { Status = -1, Msg = "The requested update amount must >= 0" };
            }

            debitHistory.Money = model.Money;

            //2. Kiểm tra định khoản công nợ có chính xác hay không
            var payReceivableDetail =
                Db.PayReceivables.FirstOrDefault(x => !x.IsDelete && x.IsIdSystem && x.Idd == model.PayReceivableIdd);

            if (payReceivableDetail == null)
            {
                return new UpdateDebitResult
                {
                    Status = -2,
                    Msg = "Entry debt  automatic transfer incorrect input !"
                };
            }

            debitHistory.PayReceivableId = payReceivableDetail.Id;
            debitHistory.PayReceivableIdd = payReceivableDetail.Idd;
            debitHistory.PayReceivableIName = payReceivableDetail.Name;

            //3. Kiểm tra thông tin đối tượng là khách hàng hay nhân viên có chính xác hay không ?
            if(model.SubjectTypeIdd.Value == (int)EnumAccountantSubject.Customer)
            {
                var customerDetail = Db.Customers.FirstOrDefault(x => !x.IsDelete && x.Id == model.SubjectId);
                if (customerDetail == null)
                {
                    return new UpdateDebitResult { Status = -3, Msg = "Debt information is not properly entered!" };
                }
                debitHistory.SubjectId = customerDetail.Id;
                debitHistory.SubjectCode = customerDetail.Code;
                debitHistory.SubjectName = customerDetail.FullName;
                debitHistory.SubjectPhone = customerDetail.Phone;
                debitHistory.SubjectEmail = customerDetail.Email;
                debitHistory.SubjectAddress = customerDetail.Address;
            }
            else
            {
                var userDetail = Db.Users.FirstOrDefault(x => !x.IsDelete && x.Id == model.SubjectId);
                if (userDetail == null)
                {
                    return new UpdateDebitResult { Status = -3, Msg = "Debt information is not properly entered !" };
                }
                debitHistory.SubjectId = userDetail.Id;
                debitHistory.SubjectName = userDetail.FullName;
                debitHistory.SubjectPhone = userDetail.Phone;
                debitHistory.SubjectEmail = userDetail.Email;
            }

            //4. Kiểm tra thông tin đơn hàng
            var orderDetail = Db.Orders.FirstOrDefault(x => !x.IsDelete && x.Id == model.OrderId);
            if (orderDetail == null)
            {
                return new UpdateDebitResult { Status = -4, Msg = "Order does not exist or is deleted!" };
            }

            debitHistory.OrderId = orderDetail.Id;
            debitHistory.OrderType = orderDetail.Type;
            debitHistory.OrderCode = orderDetail.Code;

            debitHistory.Note = model.Note;

            //5. Yêu cầu cập nhật phải thu - phải trả
            if (payReceivableDetail.Operator == true)
            {
                //Cập nhật phải thu vào hệ thống
                if (MustCollectDebit(debitHistory) == false)
                {
                    return new UpdateDebitResult { Status = -5, Msg = "Update error" };
                }
            }
            if (payReceivableDetail.Operator == false)
            {
                //Cập nhật phải trả vào hệ thống
                if (MustReturnDebit(debitHistory) == false)
                {
                    return new UpdateDebitResult { Status = -5, Msg = "Update error" };
                }
            }

            return new UpdateDebitResult { Status = 1, Msg = "Debt update successful" };
        }

        public bool MustCollectDebit(DebitHistory debitHistoryModel)
        {
            var timeNow = DateTime.Now;

            //1. Kiểm tra xem đã có phát sinh công nợ của khách hàng này với đơn hàng này hay chưa
            var debitDetail =
                Db.Debit.FirstOrDefault(
                    x =>
                        !x.IsDelete && x.IsSystem == true && x.OrderId == debitHistoryModel.OrderId &&
                        x.SubjectId == debitHistoryModel.SubjectId);

            if (debitDetail == null)
            {
                var debitUpdate = new Debit();

                //1.1. Thêm trong Debit
                var debitCode = Db.Debit.Count(x =>
                    x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                    x.Created.Day == DateTime.Now.Day);

                debitUpdate.Code = $"{DateTime.Now:ddMMyy}{debitCode}";

                debitUpdate.Status = 0;

                debitUpdate.MustCollectMoney = debitHistoryModel.Money;
                debitUpdate.MustReturnMoney = 0;

                debitUpdate.SubjectId = debitHistoryModel.SubjectId;
                debitUpdate.SubjectCode = debitHistoryModel.SubjectCode;
                debitUpdate.SubjectName = debitHistoryModel.SubjectName;
                debitUpdate.SubjectEmail = debitHistoryModel.SubjectEmail;
                debitUpdate.SubjectAddress = debitHistoryModel.SubjectAddress;
                debitUpdate.SubjectPhone = debitHistoryModel.SubjectPhone;

                debitUpdate.OrderId = debitHistoryModel.OrderId;
                debitUpdate.OrderCode = debitHistoryModel.OrderCode;
                debitUpdate.OrderType = debitHistoryModel.OrderType;

                debitUpdate.Note = debitHistoryModel.Note;

                debitUpdate.IsSystem = true;

                Db.Debit.Add(debitUpdate);
                Db.SaveChanges();

                //1.2. Kiểm tra xem DebitHistory đã có với định khoản đó chưa, nếu chưa có thì thêm mới, nếu có rồi thì cập nhật
                var debitHistory = Db.DebitHistorys.FirstOrDefault(x => x.IsSystem && x.DebitId == debitUpdate.Id
                    && x.PayReceivableIdd == debitHistoryModel.PayReceivableIdd && x.OrderId == debitHistoryModel.OrderId
                    && x.SubjectId == debitHistoryModel.SubjectId);

                if (debitHistory != null)
                {
                    //Cập nhật vào DebitHistory
                    debitHistory.Money = debitHistoryModel.Money;
                    debitHistory.LastUpdated = timeNow;
                    debitHistory.Note = debitHistoryModel.Note;

                    Db.SaveChanges();
                }
                else
                {
                    //Thêm dữ liệu mới vào DebitHistory
                    var debitHistoryCode = Db.DebitHistorys.Count(x =>
                    x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                    x.Created.Day == DateTime.Now.Day);

                    debitHistoryModel.Code = $"{DateTime.Now:ddMMyy}{debitHistoryCode}";

                    debitHistoryModel.IsSystem = true;
                    debitHistoryModel.DebitType = (byte)DebitHistoryType.mustCollect;
                    debitHistoryModel.DebitId = debitUpdate.Id;
                    debitHistoryModel.DebitCode = debitUpdate.Code;

                    Db.DebitHistorys.Add(debitHistoryModel);
                    Db.SaveChanges();
                }
            }
            else
            {
                decimal moneyUpdate;
                var debitHistoryType = (byte)DebitHistoryType.mustCollect;

                //1.1. Kiểm tra xem DebitHistory đã có với định khoản đó chưa, nếu chưa có thì thêm mới, nếu có rồi thì cập nhật
                var debitHistory = Db.DebitHistorys.FirstOrDefault(x => x.IsSystem && x.DebitId == debitDetail.Id
                && x.DebitType == debitHistoryType && x.PayReceivableIdd == debitHistoryModel.PayReceivableIdd
                && x.OrderId == debitHistoryModel.OrderId && x.SubjectId == debitHistoryModel.SubjectId);

                if (debitHistory != null)
                {
                    // Thì phải trừ số tiền trong Debit lúc cập nhật lại
                    moneyUpdate = debitHistoryModel.Money.HasValue && debitHistory.Money.HasValue
                        ? debitHistoryModel.Money.Value - debitHistory.Money.Value
                        : 0;

                    //Cập nhật vào DebitHistory
                    debitHistory.Money = debitHistoryModel.Money;
                    debitHistory.LastUpdated = timeNow;
                    debitHistory.Note = debitHistoryModel.Note;

                    Db.SaveChanges();
                }
                else
                {
                    //Thêm dữ liệu mới vào DebitHistory
                    moneyUpdate = debitHistoryModel.Money ?? 0;

                    var debitHistoryCode = Db.DebitHistorys.Count(x =>
                    x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                    x.Created.Day == DateTime.Now.Day);

                    debitHistoryModel.Code = $"{DateTime.Now:ddMMyy}{debitHistoryCode}";

                    debitHistoryModel.IsSystem = true;
                    debitHistoryModel.DebitType = (byte)DebitHistoryType.mustCollect;
                    debitHistoryModel.DebitId = debitDetail.Id;
                    debitHistoryModel.DebitCode = debitDetail.Code;

                    debitHistoryModel.Note = debitHistoryModel.Note;

                    Db.DebitHistorys.Add(debitHistoryModel);

                    Db.SaveChanges();
                }

                //1.2. Cập nhật trong Debit
                debitDetail.LastUpdated = timeNow;
                debitDetail.MustCollectMoney = debitDetail.MustCollectMoney + moneyUpdate;

                Db.SaveChanges();
            }

            return true;
        }

        public bool MustReturnDebit(DebitHistory debitHistoryModel)
        {
            var timeNow = DateTime.Now;

            //1. Kiểm tra xem đã có phát sinh công nợ của khách hàng này với đơn hàng này hay chưa
            var debitDetail = Db.Debit.FirstOrDefault(x => !x.IsDelete && x.IsSystem == true
                                                           && x.OrderId == debitHistoryModel.OrderId &&
                                                           x.SubjectId == debitHistoryModel.SubjectId);

            if (debitDetail == null)
            {
                var debitUpdate = new Debit();

                //1.1. Thêm trong Debit
                var debitCode = Db.Debit.Count(x =>
                    x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                    x.Created.Day == DateTime.Now.Day);

                debitUpdate.Code = $"{DateTime.Now:ddMMyy}{debitCode}";

                debitUpdate.Status = 0;

                debitUpdate.MustReturnMoney = debitHistoryModel.Money;
                debitUpdate.MustCollectMoney = 0;

                debitUpdate.SubjectId = debitHistoryModel.SubjectId;
                debitUpdate.SubjectCode = debitHistoryModel.SubjectCode;
                debitUpdate.SubjectName = debitHistoryModel.SubjectName;
                debitUpdate.SubjectEmail = debitHistoryModel.SubjectEmail;
                debitUpdate.SubjectAddress = debitHistoryModel.SubjectAddress;
                debitUpdate.SubjectPhone = debitHistoryModel.SubjectPhone;

                debitUpdate.OrderId = debitHistoryModel.OrderId;
                debitUpdate.OrderCode = debitHistoryModel.OrderCode;
                debitUpdate.OrderType = debitHistoryModel.OrderType;

                debitUpdate.Note = debitHistoryModel.Note;

                debitUpdate.IsSystem = true;

                Db.Debit.Add(debitUpdate);
                Db.SaveChanges();

                //1.2. Kiểm tra xem DebitHistory đã có với định khoản đó chưa, nếu chưa có thì thêm mới, nếu có rồi thì cập nhật
                var debitHistory = Db.DebitHistorys.FirstOrDefault(x => x.IsSystem && x.DebitId == debitUpdate.Id
                                                                        &&
                                                                        x.PayReceivableIdd ==
                                                                        debitHistoryModel.PayReceivableIdd &&
                                                                        x.OrderId == debitHistoryModel.OrderId
                                                                        && x.SubjectId == debitHistoryModel.SubjectId);

                if (debitHistory != null)
                {
                    //Cập nhật vào DebitHistory
                    debitHistory.Money = debitHistoryModel.Money;
                    debitHistory.LastUpdated = timeNow;
                    debitUpdate.Note = debitHistoryModel.Note;

                    Db.SaveChanges();
                }
                else
                {
                    //Thêm dữ liệu mới vào DebitHistory
                    var debitHistoryCode = Db.DebitHistorys.Count(x =>
                    x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                    x.Created.Day == DateTime.Now.Day);

                    debitHistoryModel.Code = $"{DateTime.Now:ddMMyy}{debitHistoryCode}";

                    debitHistoryModel.IsSystem = true;
                    debitHistoryModel.DebitType = (byte)DebitHistoryType.mustReturn;
                    debitHistoryModel.DebitId = debitUpdate.Id;
                    debitHistoryModel.DebitCode = debitUpdate.Code;
                    debitHistoryModel.Code = debitHistoryModel.Code ?? String.Empty;

                    debitHistoryModel.Note = debitHistoryModel.Note;

                    Db.DebitHistorys.Add(debitHistoryModel);
                    Db.SaveChanges();
                }
            }
            else
            {
                decimal moneyUpdate;
                var debitHistoryType = (byte)DebitHistoryType.mustReturn;

                //1.1. Kiểm tra xem DebitHistory đã có với định khoản đó chưa, nếu chưa có thì thêm mới, nếu có rồi thì cập nhật
                var debitHistory = Db.DebitHistorys.FirstOrDefault(x => x.IsSystem && x.DebitId == debitDetail.Id
                    && x.DebitType == debitHistoryType && x.PayReceivableIdd == debitHistoryModel.PayReceivableIdd
                    && x.OrderId == debitHistoryModel.OrderId && x.SubjectId == debitHistoryModel.SubjectId);

                //Xử lý trường hợp phải trả đặc biệt do hủy đơn hàng
                if(debitHistoryModel.PayReceivableIdd == (int)TreasureMustReturn.CancelOrder)
                {
                    // Xóa các Debithistory khác của công nợ này
                    var debitHistoryList = Db.DebitHistorys.Where(x => x.IsSystem && x.DebitId == debitDetail.Id && x.DebitType == debitHistoryType && x.OrderId == debitHistoryModel.OrderId && x.SubjectId == debitHistoryModel.SubjectId).ToList();
                    if(debitHistoryList.Count() > 0)
                    {
                        // Xóa toàn bộ trong Debithistory
                        foreach (var item in debitHistoryList)
                        {
                            Db.DebitHistorys.Remove(item);
                        }

                        // Tạo mới bản ghi Debithistory
                        moneyUpdate = debitHistoryModel.Money ?? 0;

                        var debitHistoryCode = Db.DebitHistorys.Count(x =>
                        x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                        x.Created.Day == DateTime.Now.Day);

                        debitHistoryModel.Code = $"{DateTime.Now:ddMMyy}{debitHistoryCode}";

                        debitHistoryModel.IsSystem = true;
                        debitHistoryModel.DebitType = (byte)DebitHistoryType.mustReturn;
                        debitHistoryModel.DebitId = debitDetail.Id;
                        debitHistoryModel.DebitCode = debitDetail.Code;

                        debitHistoryModel.Note = debitHistoryModel.Note;

                        Db.DebitHistorys.Add(debitHistoryModel);
                        Db.SaveChanges();

                        // Cập nhật lại vào debit
                        //1.2. Cập nhật trong Debit
                        debitDetail.LastUpdated = timeNow;
                        debitDetail.MustReturnMoney = debitHistoryModel.Money;

                        Db.SaveChanges();

                        return true;
                    }
                }

                if (debitHistory != null && debitHistoryModel.PayReceivableIdd != (int)TreasureMustReturn.CancelOrder)
                {
                    // Thì phải trừ số tiền trong Debit lúc cập nhật lại
                    moneyUpdate = debitHistoryModel.Money.HasValue && debitHistory.Money.HasValue
                        ? debitHistoryModel.Money.Value - debitHistory.Money.Value
                        : 0;

                    //Cập nhật vào DebitHistory
                    debitHistory.Money = debitHistoryModel.Money;
                    debitHistory.LastUpdated = timeNow;
                    debitHistory.Note = debitHistoryModel.Note;

                    Db.SaveChanges();
                }
                else
                {
                    //Thêm dữ liệu mới vào DebitHistory
                    moneyUpdate = debitHistoryModel.Money ?? 0;

                    var debitHistoryCode = Db.DebitHistorys.Count(x =>
                    x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                    x.Created.Day == DateTime.Now.Day);

                    debitHistoryModel.Code = $"{DateTime.Now:ddMMyy}{debitHistoryCode}";

                    debitHistoryModel.IsSystem = true;
                    debitHistoryModel.DebitType = (byte)DebitHistoryType.mustReturn;
                    debitHistoryModel.DebitId = debitDetail.Id;
                    debitHistoryModel.DebitCode = debitDetail.Code;

                    debitHistoryModel.Note = debitHistoryModel.Note;

                    Db.DebitHistorys.Add(debitHistoryModel);
                    Db.SaveChanges();
                }

                //1.2. Cập nhật trong Debit
                debitDetail.LastUpdated = timeNow;
                debitDetail.MustReturnMoney = debitDetail.MustReturnMoney + moneyUpdate;

                Db.SaveChanges();
            }

            return true;
        }
    }

    public class UpdateDebitResult
    {
        public int Status { get; set; }
        public string Msg { get; set; }
    }
}

