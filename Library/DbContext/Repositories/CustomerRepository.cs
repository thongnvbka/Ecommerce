using Library.DbContext.Entities;
using Library.UnitOfWork;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Library.ViewModels.Account;
using Common.Items;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Library.ViewModels.Items;
using Library.Models;
using System.Collections.Generic;
using Library.DbContext.Results;
using Common.Emums;
using Common.Helper;
using Library.ViewModels.Report;

namespace Library.DbContext.Repositories
{
    public class CustomerRepository : Repository<Customer>
    {
        public CustomerRepository(ProjectXContext context) : base(context)
        {
        }

        public async Task<int> CustomerUpdateLoginFailure(long id, bool isLockout, DateTime? lastLockoutDate, DateTime? firstLoginFailureDate,
            byte loginFailureCount, DateTime? lockoutToDate)
        {
            var customer = await Db.Customers.SingleAsync(x => x.Id == id);

            customer.IsLockout = isLockout;
            customer.LastLockoutDate = lastLockoutDate;
            customer.LockoutToDate = lockoutToDate;
            customer.FirstLoginFailureDate = firstLoginFailureDate;
            customer.LoginFailureCount = loginFailureCount;

            return await Db.SaveChangesAsync();
        }

        public CustomerInforModel GetInfor(int customerId, DateTime startDate, DateTime finishDate)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_infor_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));
                cmd.Parameters.Add(new SqlParameter("StartDate", startDate));
                cmd.Parameters.Add(new SqlParameter("FinishDate", finishDate));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpInfor = ((IObjectContextAdapter)context).ObjectContext.Translate<CustomerInforItem>(reader).ToList().FirstOrDefault();
                    reader.NextResult();
                    var tmpStatus = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderStatusItem>(reader).ToList().FirstOrDefault();
                    reader.NextResult();
                    var tmpOrder = ((IObjectContextAdapter)context).ObjectContext.Translate<CusInforOrderItem>(reader).ToList();
                    reader.Close();

                    var model = new CustomerInforModel()
                    {
                        CustomerInforItem = tmpInfor,
                        OrderStatusItem = tmpStatus,
                        CusInforOrderItem = tmpOrder

                    };
                    return model;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }
        public CustomerReporItem GetInfor(int customerId, int systemId, int stypeSearch)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_report_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));
                cmd.Parameters.Add(new SqlParameter("SystemId", systemId));
                cmd.Parameters.Add(new SqlParameter("TypeSearch", stypeSearch));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpCount = ((IObjectContextAdapter)context).ObjectContext.Translate<CustomerReporItem>(reader).ToList().FirstOrDefault();
                    reader.Close();
                    return tmpCount;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }

        public CustomerInforItem GetLevel(int customerId)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_Level_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpLevel = ((IObjectContextAdapter)context).ObjectContext.Translate<CustomerInforItem>(reader).ToList().FirstOrDefault();
                    reader.Close();
                    return tmpLevel;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }
        #region [Danh sách khách hàng chậm đặt hàng]
        public Task<List<CustomerOrderPending>> GetCustomerOrderPendingList(out long totalRecord, int page, int pageSize, decimal money, DateTime? dateStart, DateTime? dateEnd, UserState userstate)
        {
            var query =
                Db.Customers.Where(x => !x.IsDelete
                    && (x.OfficeIdPath == userstate.OfficeIdPath || x.OfficeIdPath.StartsWith(userstate.OfficeIdPath + ".")))
                    .GroupJoin(
                        Db.Orders.Where(s => !s.IsDelete
                        && s.Created >= dateStart
                        && s.Created <= dateEnd
                            ),
                        customer => customer.Id,
                        order => order.CustomerId.Value,
                        (c, o) => new { c, o }
                    )
                    .Select(s => new CustomerOrderPending()
                    {
                        Id = s.c.Id,
                        Code = s.c.Code,
                        Avatar = s.c.Avatar,
                        Email = s.c.Email,
                        Status = s.c.Status,
                        LevelId = s.c.LevelId,
                        LevelName = s.c.LevelName,
                        UserId = s.c.UserId,
                        UserFullName = s.c.UserFullName,
                        OfficeId = s.c.OfficeId,
                        OfficeName = s.c.OfficeName,
                        OfficeIdPath = s.c.OfficeIdPath,
                        Created = s.c.Created,
                        Updated = s.c.Updated,
                        IsDelete = s.c.IsDelete,
                        TotalOrder = s.o.Count(x => x.CustomerId == s.c.Id),
                        TotalOrderAverage = s.o.Count(x => x.CustomerId == s.c.Id) == 0 ? 0 : s.o.Where(x => x.CustomerId == s.c.Id).ToList().Average(x => x.Total) / s.o.Count(x => x.CustomerId == s.c.Id)

                    })
                    .Where(x => (money == 0 || x.TotalOrderAverage < money) && x.TotalOrder > 0)
                    .OrderByDescending(x => x.Created);

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        #endregion

        #region [Thống kê khách hàng]
        //Khách hàng tiềm năng trong ngày
        public List<CustomerUser> GetCustomerReport(List<UserOfficeResult> list, DateTime? startDay)
        {
            var DateStart = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToString() : startDay.ToString());
            var query = new List<CustomerUser>();
            foreach (var item in list)
            {
                var count = Db.Customers.Count(x =>
                                                    x.Created.Year == DateStart.Year
                                                    && x.Created.Month == DateStart.Month
                                                    && x.Created.Day == DateStart.Day
                                                    && !x.IsDelete && x.UserId == item.Id);
                query.Add(new CustomerUser()
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    Birthday = item.Birthday,
                    Email = item.Email,
                    Phone = item.Phone,
                    Created = item.Created,
                    StartDate = item.StartDate,
                    TypeName = item.TypeName,
                    Gender = item.Gender,
                    TotalCusstomer = count
                });
            }
            return query.ToList();
        }

        //Khách hàng chính thức trong ngày
        public List<CustomerUser> GetPotentialCustomerReport(List<UserOfficeResult> list, DateTime? startDay)
        {
            var DateStart = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToString() : startDay.ToString());
            var query = new List<CustomerUser>();
            foreach (var item in list)
            {
                var count = Db.PotentialCustomers.Count(x =>
                                                            x.Created.Year == DateStart.Year
                                                            && x.Created.Month == DateStart.Month
                                                            && x.Created.Day == DateStart.Day
                                                            && !x.IsDelete
                                                            && x.UserId == item.Id);
                query.Add(new CustomerUser()
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    Birthday = item.Birthday,
                    Email = item.Email,
                    Phone = item.Phone,
                    Created = item.Created,
                    StartDate = item.StartDate,
                    TypeName = item.TypeName,
                    Gender = item.Gender,
                    TotalCusstomer = count
                });
            }
            return query.ToList();
        }

        //Tất cả Khách hàng chính thức 
        public List<CustomerUser> GetCustomerOffStaffReport(List<UserOfficeResult> list, DateTime? startDay, DateTime? endDay)
        {
            var DateStart = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToString() : startDay.ToString());
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(endDay.ToString()) ? DateTime.Now.ToString() : endDay.ToString());
            var query = new List<CustomerUser>();
            foreach (var item in list)
            {
                var count = Db.Customers.Count(x => !x.IsDelete
                                                && x.UserId == item.Id
                                                && (startDay == null || x.Created >= DateStart)
                                                && (startDay == null || x.Created <= DateEnd));
                query.Add(new CustomerUser()
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    Birthday = item.Birthday,
                    Email = item.Email,
                    Phone = item.Phone,
                    Created = item.Created,
                    StartDate = item.StartDate,
                    TypeName = item.TypeName,
                    Gender = item.Gender,
                    TotalCusstomer = count
                });
            }
            return query.ToList();
        }

        //Tất cả Khách hàng tiềm năng
        public List<CustomerUser> GetPotentialCustomerOffStaffReport(List<UserOfficeResult> list, DateTime? startDay, DateTime? endDay)
        {
            var DateStart = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToString() : startDay.ToString());
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(endDay.ToString()) ? DateTime.Now.ToString() : endDay.ToString());
            var query = new List<CustomerUser>();
            foreach (var item in list)
            {
                var count = Db.PotentialCustomers.Count(x => !x.IsDelete
                                                && x.UserId == item.Id
                                                && (startDay == null || x.Created >= DateStart)
                                                && (startDay == null || x.Created <= DateEnd));
                query.Add(new CustomerUser()
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    Birthday = item.Birthday,
                    Email = item.Email,
                    Phone = item.Phone,
                    Created = item.Created,
                    StartDate = item.StartDate,
                    TypeName = item.TypeName,
                    Gender = item.Gender,
                    TotalCusstomer = count
                });
            }
            return query.ToList();
        }


        #endregion

        #region [Thống kê tình hình khách hàng theo thời gian]
        public List<ProfitDay> GetCustomerSituationOnTime(List<UserOfficeResult> listUser, DateTime startDay, DateTime endDay, UserState userstate)
        {
            var listCustomer = Db.Customers.Where(s =>
                                                    s.Created >= startDay
                                                    && s.Created <= endDay
                                                    && (userstate.Type > 0 || s.UserId == userstate.UserId)
                                                    && (userstate.Type == 0 || (s.OfficeIdPath == userstate.OfficeIdPath
                                                    || s.OfficeIdPath.StartsWith(userstate.OfficeIdPath + ".")))
                                                    )
                                                  .ToList();

            var query = new List<ProfitDay>();
            var list = new List<string>();
            DateTime tmpDate = startDay;
            do
            {
                list.Add(tmpDate.ToShortDateString());
                tmpDate = tmpDate.AddDays(1);
            } while (tmpDate <= endDay);

            foreach (var item in list)
            {
                var count = listCustomer.Where(x => x.Created.ToShortDateString() == item).Count();

                query.Add(new ProfitDay()
                {
                    Created = item,
                    TotalOrder = count
                });
            }
            return query.ToList();
        }
        #endregion

        #region [Thống kê doanh số nhân viên]
        //Đơn order
        public Task<List<ReportBusinessItem>> GetOrderUserList(out decimal TotalOrderExchange, out decimal TotalOrderWeight, out decimal TotalServicePurchase, out decimal TotalOrderBargain, out long totalRecord, int page, int pageSize, string keyWord, int userId, int customerStatus, DateTime? start, DateTime? end, List<int> listUser, UserState userState)
        {
            var q = Db.Orders.Where(x => !x.IsDelete && x.Type == (byte)OrderType.Order && x.Status != (byte)OrderStatus.Cancel && x.Status >= (byte)OrderStatus.Finish
                        && (x.Id.ToString().Contains(keyWord) || x.Code.Contains(keyWord)))
                .Join(
                    Db.Customers.Where(x => listUser.Contains(x.UserId.Value)
                        && !x.IsDelete
                        && (x.FullName.Contains(keyWord) || x.Email.Contains(keyWord) || x.Phone.Contains(keyWord) || x.UserFullName.Contains(keyWord))
                        && (customerStatus == -1 || x.Status == customerStatus)),
                    order => order.CustomerId,
                    customer => customer.Id,
                    (o, c) => new { order = o, customer = c }
                ).Join(
                    Db.OrderHistories.Where(x => ((x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.Finish) || (x.Type == (byte)OrderType.Deposit && x.Status == (byte)DepositStatus.Finish))
                        && (start == null || x.CreateDate >= start)
                        && (end == null || x.CreateDate <= end)),
                    group => group.order.Id,
                    history => history.OrderId,
                    (g, h) => new { order = g.order, history = h, customer = g.customer }
                )
                //.Join(
                //    Db.OrderServices.Where(x => !x.IsDelete && x.ServiceId == (byte)OrderServices.Order),
                //    group => group.order.Id,
                //    service => service.OrderId,
                //    (g, s) => new { order = g.order, history = g.history, customer = g.customer, service = s }
                //)
                .Select(x => new ReportBusinessItem()
                {
                    CustomerId = x.customer.Id,
                    CustomerName = x.customer.FullName,
                    OrderStatus = x.order.Status,
                    CustomerEmail = x.customer.Email,
                    CustomerAddress = x.customer.Address,
                    CustomerPhone = x.customer.Phone,
                    UserId = x.customer.UserId,
                    UserFullName = x.customer.UserFullName,
                    OrderId = x.order.Id,
                    OrderCode = x.order.Code,
                    OrderType = x.order.Type,
                    OrderTotal = x.order.Total,
                    OrderFinishDate = x.history.CreateDate,
                    OrderTotalExchange = x.order.TotalExchange,
                    ServicePurchase = /*x.service.TotalPrice*/ 0,
                    ExchangeRate = x.order.ExchangeRate,
                    OrderBargain = x.order.PriceBargain ?? 0,
                    OrderTotalWeight = x.order.TotalWeight

                }).OrderByDescending(x => x.UserId);

            //var total = Db.Orders.Where(x => !x.IsDelete && x.Type == (byte)OrderType.Order && x.Status != (byte)OrderStatus.Cancel && x.Status >= (byte)OrderStatus.Finish
            //            && (x.Id.ToString().Contains(keyWord) || x.Code.Contains(keyWord)))
            //    .Join(
            //        Db.Customers.Where(x => listUser.Contains(x.UserId.Value)
            //            && !x.IsDelete
            //            && (x.FullName.Contains(keyWord) || x.Email.Contains(keyWord) || x.Phone.Contains(keyWord) || x.UserFullName.Contains(keyWord))
            //            && (customerStatus == -1 || x.Status == customerStatus)),
            //        order => order.CustomerId,
            //        customer => customer.Id,
            //        (o, c) => new { order = o, customer = c }
            //    ).Join(
            //        Db.OrderHistories.Where(x => ((x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.Finish) || (x.Type == (byte)OrderType.Deposit && x.Status == (byte)DepositStatus.Finish))
            //            && (start == null || x.CreateDate >= start)
            //            && (end == null || x.CreateDate <= end)),
            //        group => group.order.Id,
            //        history => history.OrderId,
            //        (g, h) => new { order = g.order, history = h, customer = g.customer }
            //    ).Join(
            //        Db.OrderServices.Where(x => !x.IsDelete && x.ServiceId == (byte)OrderServices.Order),
            //        group => group.order.Id,
            //        service => service.OrderId,
            //        (g, s) => new { order = g.order, history = g.history, customer = g.customer, service = s }
            //    ).Select(x => new ReportBusinessItem()
            //    {
            //        CustomerId = x.customer.Id,
            //        CustomerName = x.customer.FullName,
            //        CustomerEmail = x.customer.Email,
            //        CustomerAddress = x.customer.Address,
            //        CustomerPhone = x.customer.Phone,
            //        UserId = x.customer.UserId,
            //        UserFullName = x.customer.UserFullName,
            //        OrderId = x.order.Id,
            //        OrderCode = x.order.Code,
            //        OrderType = x.order.Type,
            //        OrderTotal = x.order.Total,
            //        OrderFinishDate = x.history.CreateDate,
            //        OrderTotalExchange = x.order.TotalExchange,
            //        ServicePurchase = x.service.TotalPrice,
            //        ExchangeRate = x.order.ExchangeRate,
            //        OrderBargain = x.order.PriceBargain ?? 0
            //    }).OrderByDescending(x => x.UserId).ToList();
            totalRecord = q.Count();
            TotalOrderExchange = 0;
            TotalOrderWeight = 0;
            TotalOrderBargain = 0;
            TotalServicePurchase = 0;
            if(q.Count() != 0)
            {
                TotalOrderExchange = q.Sum(s => s.OrderTotalExchange);
                TotalServicePurchase = q.Sum(s => s.ServicePurchase);
                TotalOrderBargain = q.Sum(s => s.OrderBargain * s.ExchangeRate);
                TotalOrderWeight = q.Sum(s => s.OrderTotalWeight);
            }
            
            return q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        //Đơn ký gửi
        public Task<List<ReportBusinessItem>> GetDepositUserList(out decimal TotalOrderExchange, out decimal TotalOrderWeight, out long totalRecord, int page, int pageSize, string keyWord, int userId, int customerStatus, DateTime? start, DateTime? end, List<int> listUser, UserState userState)
        {
            var q = Db.Orders.Where(x => !x.IsDelete && x.Type == (byte)OrderType.Deposit && x.Status != (byte)DepositStatus.Cancel && x.Status >= (byte)DepositStatus.Finish
                        && (x.Id.ToString().Contains(keyWord) || x.Code.Contains(keyWord)))
                .Join(
                    Db.Customers.Where(x => listUser.Contains(x.UserId.Value)
                        && !x.IsDelete
                        && (x.FullName.Contains(keyWord) || x.Email.Contains(keyWord) || x.Phone.Contains(keyWord) || x.UserFullName.Contains(keyWord))
                        && (customerStatus == -1 || x.Status == customerStatus)),
                    order => order.CustomerId,
                    customer => customer.Id,
                    (o, c) => new { order = o, customer = c }
                ).Join(
                    Db.OrderHistories.Where(x => ((x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.Finish) || (x.Type == (byte)OrderType.Deposit && x.Status == (byte)DepositStatus.Finish))
                        && (start == null || x.CreateDate >= start)
                        && (end == null || x.CreateDate <= end)),
                    group => group.order.Id,
                    history => history.OrderId,
                    (g, h) => new { order = g.order, history = h, customer = g.customer }
                ).GroupJoin(
                    Db.OrderServices.Where(x => !x.IsDelete),
                    group => group.order.Id,
                    service => service.OrderId,
                    (g, s) => new { order = g.order, history = g.history, customer = g.customer, service = s }
                ).Select(x => new ReportBusinessItem()
                {
                    CustomerId = x.customer.Id,
                    CustomerName = x.customer.FullName,
                    CustomerEmail = x.customer.Email,
                    CustomerAddress = x.customer.Address,
                    CustomerPhone = x.customer.Phone,
                    UserId = x.customer.UserId,
                    UserFullName = x.customer.UserFullName,
                    OrderId = x.order.Id,
                    OrderCode = x.order.Code,
                    OrderType = x.order.Type,
                    OrderTotal = x.order.Total,
                    OrderFinishDate = x.history.CreateDate,
                    OrderTotalExchange = x.order.TotalExchange,
                    ServicePurchase = /*x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte) OrderServices.Order) == null ? 0 : x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.Order).TotalPrice*/ 0,
                    ServiceBaled = x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.Audit) == null ? 0 : x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.Audit).TotalPrice,
                    ServiceShip = x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.OutSideShipping) == null ? 0 : x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.OutSideShipping).TotalPrice,
                    OrderBargain = x.order.PriceBargain ?? 0,
                    OrderTotalPrice = x.order.TotalPrice,
                    ApprovelPrice = x.order.ApprovelPrice ?? 0,
                    OrderTotalWeight = x.order.TotalWeight
                }).OrderByDescending(x => x.UserId);

            //var total = Db.Orders.Where(x => !x.IsDelete && x.Type == (byte)OrderType.Deposit && x.Status != (byte)DepositStatus.Cancel && x.Status >= (byte)DepositStatus.Finish
            //            && (x.Id.ToString().Contains(keyWord) || x.Code.Contains(keyWord)))
            //    .Join(
            //        Db.Customers.Where(x => listUser.Contains(x.UserId.Value)
            //            && !x.IsDelete
            //            && (x.FullName.Contains(keyWord) || x.Email.Contains(keyWord) || x.Phone.Contains(keyWord) || x.UserFullName.Contains(keyWord))
            //            && (customerStatus == -1 || x.Status == customerStatus)),
            //        order => order.CustomerId,
            //        customer => customer.Id,
            //        (o, c) => new { order = o, customer = c }
            //    ).Join(
            //        Db.OrderHistories.Where(x => ((x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.Finish) || x.Type == (byte)OrderType.Deposit && x.Status == (byte)DepositStatus.Finish)
            //            && (start == null || x.CreateDate >= start)
            //            && (end == null || x.CreateDate <= end)),
            //        group => group.order.Id,
            //        history => history.OrderId,
            //        (g, h) => new { order = g.order, history = h, customer = g.customer }
            //    ).GroupJoin(
            //        Db.OrderServices.Where(x => !x.IsDelete),
            //        group => group.order.Id,
            //        service => service.OrderId,
            //        (g, s) => new { order = g.order, history = g.history, customer = g.customer, service = s }
            //    ).Select(x => new ReportBusinessItem()
            //    {
            //        CustomerId = x.customer.Id,
            //        CustomerName = x.customer.FullName,
            //        CustomerEmail = x.customer.Email,
            //        CustomerAddress = x.customer.Address,
            //        CustomerPhone = x.customer.Phone,
            //        UserId = x.customer.UserId,
            //        UserFullName = x.customer.UserFullName,
            //        OrderId = x.order.Id,
            //        OrderCode = x.order.Code,
            //        OrderType = x.order.Type,
            //        OrderTotal = x.order.Total,
            //        OrderFinishDate = x.history.CreateDate,
            //        OrderTotalExchange = x.order.TotalExchange,
            //        ServicePurchase = x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.Order) == null ? 0 : x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.Order).TotalPrice,
            //        ServiceBaled = x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.Audit) == null ? 0 : x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.Audit).TotalPrice,
            //        ServiceShip = x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.OutSideShipping) == null ? 0 : x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.OutSideShipping).TotalPrice,
            //        OrderBargain = x.order.PriceBargain ?? 0,
            //        OrderTotalPrice = x.order.TotalPrice,
            //        ApprovelPrice = x.order.ApprovelPrice ?? 0,
            //        OrderTotalWeight = x.order.TotalWeight
            //    }).OrderByDescending(x => x.UserId).ToList();
            totalRecord = q.Count();
            TotalOrderExchange = 0;
            TotalOrderWeight = 0;
            if (q.Count() != 0)
            {
                TotalOrderExchange = q.Sum(s => s.OrderTotal);
                TotalOrderWeight = q.Sum(s => s.OrderTotalWeight);
            }
            
            return q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        //Xuất excel Thống kê doanh số nhân viên
        public Task<List<ReportBusinessItem>> ExcelGetOrderUserList(string keyWord, int userId, int customerStatus, DateTime? start, DateTime? end, List<int> listUser)
        {
            var q = Db.Orders.Where(x => !x.IsDelete && x.Type == (byte)OrderType.Order && x.Status != (byte)OrderStatus.Cancel && x.Status >= (byte)OrderStatus.Finish
                        && (x.Id.ToString().Contains(keyWord) || x.Code.Contains(keyWord)))
                .Join(
                    Db.Customers.Where(x => listUser.Contains(x.UserId.Value)
                        && !x.IsDelete
                        && (x.FullName.Contains(keyWord) || x.Email.Contains(keyWord) || x.Phone.Contains(keyWord) || x.UserFullName.Contains(keyWord))
                        && (customerStatus == -1 || x.Status == customerStatus)),
                    order => order.CustomerId,
                    customer => customer.Id,
                    (o, c) => new { order = o, customer = c }
                ).Join(
                    Db.OrderHistories.Where(x => ((x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.Finish) || (x.Type == (byte)OrderType.Deposit && x.Status == (byte)DepositStatus.Finish))
                        && (start == null || x.CreateDate >= start)
                        && (end == null || x.CreateDate <= end)),
                    group => group.order.Id,
                    history => history.OrderId,
                    (g, h) => new { order = g.order, history = h, customer = g.customer }
                )
                //.Join(
                //    Db.OrderServices.Where(x => !x.IsDelete && x.ServiceId == (byte)OrderServices.Order),
                //    group => group.order.Id,
                //    service => service.OrderId,
                //    (g, s) => new { order = g.order, history = g.history, customer = g.customer, service = s }
                //)
                .Select(x => new ReportBusinessItem()
                {
                    CustomerId = x.customer.Id,
                    CustomerName = x.customer.FullName,
                    CustomerEmail = x.customer.Email,
                    CustomerAddress = x.customer.Address,
                    CustomerPhone = x.customer.Phone,
                    UserId = x.customer.UserId,
                    UserFullName = x.customer.UserFullName,
                    OrderId = x.order.Id,
                    OrderCode = x.order.Code,
                    OrderType = x.order.Type,
                    OrderTotal = x.order.Total,
                    OrderFinishDate = x.history.CreateDate,
                    OrderTotalExchange = x.order.TotalExchange,
                    ServicePurchase = /*x.service.TotalPrice*/ 0,
                    ExchangeRate = x.order.ExchangeRate,
                    OrderBargain = x.order.PriceBargain ?? 0
                }).OrderByDescending(x => x.UserId);
            return q.ToListAsync();
        }

        public Task<List<ReportBusinessItem>> ExcelGetOrderUserDepositList(string keyWord, int userId, int customerStatus, DateTime? start, DateTime? end, List<int> listUser)
        {
            var q = Db.Orders.Where(x => !x.IsDelete && x.Type == (byte)OrderType.Deposit && x.Status != (byte)DepositStatus.Cancel && x.Status >= (byte)DepositStatus.Finish
                        && (x.Id.ToString().Contains(keyWord) || x.Code.Contains(keyWord)))
                .Join(
                    Db.Customers.Where(x => listUser.Contains(x.UserId.Value)
                        && !x.IsDelete
                        && (x.FullName.Contains(keyWord) || x.Email.Contains(keyWord) || x.Phone.Contains(keyWord) || x.UserFullName.Contains(keyWord))
                        && (customerStatus == -1 || x.Status == customerStatus)),
                    order => order.CustomerId,
                    customer => customer.Id,
                    (o, c) => new { order = o, customer = c }
                ).Join(
                    Db.OrderHistories.Where(x => ((x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.Finish) || (x.Type == (byte)OrderType.Deposit && x.Status == (byte)DepositStatus.Finish))
                        && (start == null || x.CreateDate >= start)
                        && (end == null || x.CreateDate <= end)),
                    group => group.order.Id,
                    history => history.OrderId,
                    (g, h) => new { order = g.order, history = h, customer = g.customer }
                ).GroupJoin(
                    Db.OrderServices.Where(x => !x.IsDelete),
                    group => group.order.Id,
                    service => service.OrderId,
                    (g, s) => new { order = g.order, history = g.history, customer = g.customer, service = s }
                ).Select(x => new ReportBusinessItem()
                {
                    CustomerId = x.customer.Id,
                    CustomerName = x.customer.FullName,
                    CustomerEmail = x.customer.Email,
                    CustomerAddress = x.customer.Address,
                    CustomerPhone = x.customer.Phone,
                    UserId = x.customer.UserId,
                    UserFullName = x.customer.UserFullName,
                    OrderId = x.order.Id,
                    OrderCode = x.order.Code,
                    OrderType = x.order.Type,
                    OrderTotal = x.order.Total,
                    OrderFinishDate = x.history.CreateDate,
                    OrderTotalExchange = x.order.TotalExchange,
                    ServicePurchase = /*x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.Order) == null ? 0 : x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.Order).TotalPrice*/ 0,
                    ServiceBaled = x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.Audit) == null ? 0 : x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.Audit).TotalPrice,
                    ServiceShip = x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.OutSideShipping) == null ? 0 : x.service.FirstOrDefault(n => n.Checked && n.ServiceId == (byte)OrderServices.OutSideShipping).TotalPrice,
                    OrderBargain = x.order.PriceBargain ?? 0,
                    OrderTotalPrice = x.order.TotalPrice,
                    ApprovelPrice = x.order.ApprovelPrice ?? 0,
                    OrderTotalWeight = x.order.TotalWeight
                }).OrderByDescending(x => x.UserId);
            return q.ToListAsync();
        }
        #endregion

        //Lấy danh sách order của nhân viên theo phòng kinh doanh
        public Task<List<OrderCustomer>> GetOrderCustomer(out long totalRecord, out dynamic total, out dynamic totalPriceBargain, out dynamic totalServiceOrder, out dynamic totalWeight, List<UserOfficeResult> list, int status, string keyword, int userId, DateTime? start, DateTime? end, int orderType, int page, int pageSize, UserState userstate)
        {
            var query = Db.Orders.Where(x => !x.IsDelete
                                        && (orderType == -1 || x.Type == orderType)
                                        && (x.Code.Contains(keyword)
                                        || x.CustomerName.Contains(keyword)
                                        || x.CustomerEmail.Contains(keyword)
                                        || x.UserName.Contains(keyword)
                                        || x.UserFullName.Contains(keyword)
                                        || x.SystemName.Contains(keyword))
                                        && (status == -1 || x.Status == status)
                                        && (start == null || x.Created >= start)
                                        && (end == null || x.Created <= end)
                                        ).Join(
                                            Db.Customers.Where(s => !s.IsDelete
                                            && (userId == -1 || s.UserId == userId)
                                            && (userstate.Type > 0 || s.UserId == userstate.UserId)
                                            && (userstate.Type == 0 || (s.OfficeIdPath == userstate.OfficeIdPath
                                            || s.OfficeIdPath.StartsWith(userstate.OfficeIdPath + ".")))),
                                            order => order.CustomerId.Value,
                                            customer => customer.Id,
                                            (o, c) => new { o, c })
                //                            .Join(
                //Db.OrderServices.Where(x => x.ServiceId == (byte)OrderServices.Order && !x.IsDelete && x.Checked),
                //    group => group.o.Id,
                //    orderService => orderService.OrderId,
                //    (g, os) => new { g.o, g.c, os }
                //)
                .Select(d => new OrderCustomer()
                {
                    Id = d.o.Id,
                    Code = d.o.Code,
                    Type = d.o.Type,
                    Status = d.o.Status,
                    CustomerUserId = d.c.UserId,
                    CustomerUserName = d.c.UserFullName,
                    Total = d.o.Total,
                    TotalPayed = d.o.TotalPayed,
                    PriceBargain = (d.o.PriceBargain == null ? 0 : d.o.PriceBargain) * d.o.ExchangeRate,
                    Created = d.o.Created,
                    CustomerId = d.o.CustomerId,
                    CustomerName = d.o.CustomerName,
                    ServiceOrder = /*d.os.TotalPrice*/ 0,
                    TotalWeight = d.o.TotalWeight,
                    TotalExchange = d.o.TotalExchange
                }).OrderByDescending(x => x.Created);

            totalRecord = query.Count();

            var queryReport = query.Where(x => x.TotalPayed > 0 && (x.Type == (byte)OrderType.Order && x.Status < (byte)OrderStatus.Cancel
            || x.Type == (byte)OrderType.Deposit && x.Status < (byte)DepositStatus.Cancel));

            if (queryReport.Any())
            {
                total = queryReport.Sum(x => x.TotalExchange);
                totalPriceBargain = queryReport.Sum(x => x.PriceBargain ?? 0);
                totalServiceOrder = queryReport.Sum(x => x.ServiceOrder ?? 0);
                totalWeight = queryReport.Sum(x=>x.TotalWeight);
            }
            else
            {
                total = 0;
                totalPriceBargain = 0;
                totalServiceOrder = 0;
                totalWeight = 0;
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        //Xuất excel danh sách order của nhân viên theo phòng kinh doanh
        public Task<List<OrderCustomer>> ExcelGetOrderCustomer(List<UserOfficeResult> list, int status, string keyword, int userId, DateTime? start, DateTime? end, int orderType, UserState userstate)
        {
            var query = Db.Orders.Where(x => !x.IsDelete
                                        && (orderType == -1 || x.Type == orderType)
                                        && (x.Code.Contains(keyword)
                                        || x.CustomerName.Contains(keyword)
                                        || x.CustomerEmail.Contains(keyword)
                                        || x.UserName.Contains(keyword)
                                        || x.UserFullName.Contains(keyword)
                                        || x.SystemName.Contains(keyword))
                                        && (status == -1 || x.Status == status)
                                        && (start == null || x.Created >= start)
                                        && (end == null || x.Created <= end)
                                        )
                                        .Join(
                                            Db.Customers.Where(s => !s.IsDelete
                                            && (userId == -1 || s.UserId == userId)
                                            && (userstate.Type > 0 || s.UserId == userstate.UserId)
                                            && (userstate.Type == 0 || (s.OfficeIdPath == userstate.OfficeIdPath
                                            || s.OfficeIdPath.StartsWith(userstate.OfficeIdPath + ".")))),
                                            order => order.CustomerId.Value,
                                            customer => customer.Id,
                                            (o, c) => new { o, c })
                //                            .Join(
                //Db.OrderServices.Where(x => x.ServiceId == (byte)OrderServices.Order && !x.IsDelete && x.Checked),
                //    group => group.o.Id,
                //    orderService => orderService.OrderId,
                //    (g, os) => new { g.o, g.c, os }
                //)
                .Select(d => new OrderCustomer()
                {
                    Id = d.o.Id,
                    Code = d.o.Code,
                    Type = d.o.Type,
                    Status = d.o.Status,
                    CustomerUserId = d.c.UserId,
                    CustomerUserName = d.c.UserFullName,
                    Total = d.o.Total,
                    PriceBargain = d.o.PriceBargain,
                    Created = d.o.Created,
                    CustomerId = d.o.CustomerId,
                    CustomerName = d.o.CustomerName,
                    ServiceOrder = /*d.os.TotalPrice*/0
                })
                                            .OrderByDescending(x => x.Created);
            return query.ToListAsync();
        }
    }

}
