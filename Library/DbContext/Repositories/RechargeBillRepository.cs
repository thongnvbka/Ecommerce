using System;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using Common.Items;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;
using AutoMapper.QueryableExtensions;
using Common.Constant;
using Common.Emums;
using Library.Models;
using Library.ViewModels.Items;

namespace Library.DbContext.Repositories
{
    public class RechargeBillRepository : Repository<RechargeBill>
    {
        public RechargeBillRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
        public RechargeModel GetAllRecharge(PageItem pageInfor, SearchInfor searchInfor)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_recharge_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("PageIndex", pageInfor.PageIndex == 0 ? 1 : pageInfor.PageIndex));
                cmd.Parameters.Add(new SqlParameter("PageSize", pageInfor.PageIndex == 0 ? 25 : pageInfor.PageSize));
                cmd.Parameters.Add(new SqlParameter("Keyword", searchInfor.Keyword));
                cmd.Parameters.Add(new SqlParameter("StartDate", searchInfor.StartDate));
                cmd.Parameters.Add(new SqlParameter("FinishDate", searchInfor.FinishDate));
                cmd.Parameters.Add(new SqlParameter("AllTime", searchInfor.AllTime));
                cmd.Parameters.Add(new SqlParameter("CustomerId", searchInfor.CustomerId));
                cmd.Parameters.Add(new SqlParameter("IsSearch", searchInfor.IsSearch));
                SqlParameter outputCount = new SqlParameter("@Count", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputCount);
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<RechargeItem>(reader).ToList();
                    reader.Close();
                    pageInfor.CurrentPage = pageInfor.PageIndex;
                    pageInfor.Total = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());

                    var model = new RechargeModel()
                    {
                        Page = pageInfor,
                        Search = searchInfor,
                        ListItems = tmpList
                    };
                    return model;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }
        public RechargeModel GetAllRechargeByLinq(PageItem pageInfor, SearchInfor searchInfor)
        {
            var query = Db.RechargeBill.Where(
                   x => ((string.IsNullOrEmpty(searchInfor.Keyword) || x.OrderCode == null) || (x.OrderCode.Contains(searchInfor.Keyword)  && !string.IsNullOrEmpty(searchInfor.Keyword)))
                   && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.Created >= searchInfor.StartDate)
                   && (searchInfor.FinishDate == null || x.Created <= searchInfor.FinishDate)))
                   && !x.IsDelete
                   && x.CustomerId == searchInfor.CustomerId
                   && x.Status == (byte)FundBillStatus.Approved
                   &&  (searchInfor.WalletId == -1 || x.TreasureId == searchInfor.WalletId)
               )
               .Select(m => new RechargeItem()
               {
                   Id = m.Id,
                   Code = m.Code,
                   Status = m.Status,
                   Note = m.Note,
                   CurrencyFluctuations = m.CurrencyFluctuations,
                   Type = m.Type,
                   Created = m.Created,
                   CurencyStart = m.CurencyStart,
                   CurencyEnd = m.CurencyEnd,
                   OrderCode = m.OrderCode,
                   OrderType = m.OrderType,
                   OrderId = m.OrderId,
                   TreasureName = m.TreasureName
               });
            
            pageInfor.CurrentPage = pageInfor.PageIndex;
            pageInfor.Total = query.Count();
            var tmpList = query.OrderByDescending(x => new { x.Created })
                    .Skip((pageInfor.CurrentPage - 1) * pageInfor.PageSize)
                    .Take(pageInfor.PageSize)
                    .ToList();

            //Lay danh sach cac dinh khoan
            var listParent = Db.CustomerWallets.Where(m => !m.IsDelete).Select(x => x.ParentId).Distinct().ToArray();
            var listWallet = Db.CustomerWallets.Where(m => !m.IsDelete && !listParent.Contains(m.Id))
                            .Select(m => new DropdownItem()
                            {
                                Text = m.Name,
                                Value = m.Id.ToString()
                            }).ToList();
            var model = new RechargeModel()
            {
                Page = pageInfor,
                Search = searchInfor,
                ListItems = tmpList,
                Wallets = listWallet
            };
            return model;
        }

        //public RechargeModel GetAll(PageItem pageInfor, SearchInfor searchInfor)
        //{


        //}
        /// <summary>
        /// Thanh toán tiền
        /// </summary>
        /// <param name="model">Thông số thanh toán</param>
        /// <returns>Trạng thái lỗi</returns>
        public ProcessRechargeBillResult ProcessRechargeBill(AutoProcessRechargeBillModel model)
        {
            var rechargeBillData = new RechargeBill();

            //0. Kiểm tra số tiền
            if (model.CurrencyFluctuations <= 0)
            {
                return new ProcessRechargeBillResult { Status = -1, Msg = "Balance arising not be less than 0!" };
            }

            rechargeBillData.CurrencyFluctuations = model.CurrencyFluctuations;

            //1. Kiểm tra thông tin khách hàng
            var customer = Db.Customers.FirstOrDefault(x => !x.IsDelete && x.Id == model.CustomerId);

            if (customer == null)
            {
                return new ProcessRechargeBillResult { Status = -2, Msg = "Customer does not exist or is suspended, or deleted!" };
            }
            // Khởi tạo phiếu nạp/trừ tiền ví điện tử
            rechargeBillData.CustomerId = customer.Id;
            rechargeBillData.CustomerName = customer.FullName;
            rechargeBillData.CustomerCode = customer.Code;
            rechargeBillData.CustomerPhone = customer.Phone;
            rechargeBillData.CustomerEmail = customer.Email;
            rechargeBillData.CustomerAddress = customer.Address;

            //2. Kiểm tra thông tin định khoản
            var customerWallet = Db.CustomerWallets.FirstOrDefault(x => !x.IsDelete && x.IsIdSystem && x.Idd == model.TreasureIdd);
            if (customerWallet == null)
            {
                return new ProcessRechargeBillResult { Status = -3, Msg = "Entry automatic is incorrect!" };
            }
            //Thông tin định khoản
            rechargeBillData.TreasureId = customerWallet.Id;
            rechargeBillData.TreasureIdd = customerWallet.Idd;
            rechargeBillData.TreasureName = customerWallet.Name;

            if (customerWallet.Operator == true)
            {
                //Nạp tiền ví điện tử
                rechargeBillData.Type = (byte)RechargeBillType.Increase;
                rechargeBillData.Increase = model.CurrencyFluctuations;
                rechargeBillData.Diminishe = 0;
            }

            if (customerWallet.Operator == false)
            {
                //Trừ tiền ví điện tử
                rechargeBillData.Type = (byte)RechargeBillType.Diminishe;
                rechargeBillData.Diminishe = model.CurrencyFluctuations;
                rechargeBillData.Increase = 0;

                //3. Kiểm tra số dư có được phép thực hiện hay không ?
                if (customer.BalanceAvalible < model.CurrencyFluctuations)
                {
                    return new ProcessRechargeBillResult { Status = -4, Msg = "The balance of customer's e-wallet is not enough to make deductions!" };
                }
            }

            //3. Kiểm tra thông tin đơn hàng
            var orderDetail = Db.Orders.FirstOrDefault(x => !x.IsDelete && x.Id == model.OrderId.Value);
            if (orderDetail != null)
            {
                rechargeBillData.OrderId = orderDetail.Id;
                rechargeBillData.OrderCode = orderDetail.Code;
                rechargeBillData.OrderType = orderDetail.Type;
                // return new ProcessRechargeBillResult { Status = -5, Msg = "Đơn hàng không tồn tại hoặc đã bị xóa!" };
            }

            //Set tham số tự động
            rechargeBillData.IsAutomatic = true;

            //Set thông tin số dư trước và sau giao dịch
            rechargeBillData.CurencyStart = customer.BalanceAvalible;
            if (customerWallet.Operator == true)
            {
                rechargeBillData.CurencyEnd = customer.BalanceAvalible + rechargeBillData.CurrencyFluctuations;
            }

            if (customerWallet.Operator == false)
            {
                rechargeBillData.CurencyEnd = customer.BalanceAvalible - rechargeBillData.CurrencyFluctuations;
            }

            //Lấy thông tin người tạo phiếu
            rechargeBillData.UserId = 0;
            rechargeBillData.UserCode = string.Empty;
            rechargeBillData.UserName = "[System]";

            //Thông tin người duyệt phiếu
            rechargeBillData.UserApprovalId = 0;
            rechargeBillData.UserApprovalCode = string.Empty;
            rechargeBillData.UserApprovalName = "[System]";

            //Trạng thái
            rechargeBillData.Status = (byte)RechargeBillStatus.Approved;
            rechargeBillData.Note = model.Note;

            // Lưu phiếu nạp/trừ tiền ví điện tử
            //Db.Entry(rechargeBillData).State = EntityState.Added;
            Db.RechargeBill.Add(rechargeBillData);

            //Khởi tạo Code mã phiếu
            var rechargeBillOfDay = Db.RechargeBill.Count(x =>
                x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                x.Created.Day == DateTime.Now.Day);

            rechargeBillData.Code = $"{rechargeBillOfDay}{DateTime.Now:ddMMyy}";
            Db.SaveChanges();

            // Thay đổi số dư của khách hàng
            if (customerWallet.Operator == true)
            {
                customer.BalanceAvalible = customer.BalanceAvalible + model.CurrencyFluctuations;
                //Db.Entry(customer).State = EntityState.Modified;
                Db.SaveChanges();
            }

            if (customerWallet.Operator == false)
            {
                customer.BalanceAvalible = customer.BalanceAvalible - model.CurrencyFluctuations;
                //customer.Balance = customer.Balance + model.CurrencyFluctuations;

                //Db.Entry(customer).State = EntityState.Modified;

                Db.SaveChanges();
            }

            return new ProcessRechargeBillResult { Status = 1, Msg = "Create ticket charge / subtract successful electronic wallet" };
        }
    }

    public class ProcessRechargeBillResult
    {
        /// <summary>
        /// Trạng thái lỗi thanh toán
        /// -1: Số dư phát sinh không được nhỏ hơn 0
        /// -2: Khách hàng không tồn tại hoặc bị tạm ngưng, hoặc bị xóa!
        /// -3: Định khoản tự động truyền vào không chính xác!
        /// -4: Đơn hàng không tồn tại hoặc đã bị xóa!
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Nội dung lỗi
        /// </summary>
        public string Msg { get; set; }
    }
}
