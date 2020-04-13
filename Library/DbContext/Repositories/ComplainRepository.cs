using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using Common.Items;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Library.ViewModels.Items;
using Library.DbContext.Results;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.ViewModels.Complains;
using System;
using System.Data.Entity;
using Library.Models;
using Common.Emums;
using Common.Helper;
using System.Globalization;

namespace Library.DbContext.Repositories
{
    public class ComplainRepository : Repository<Complain>
    {
        public ComplainRepository(ProjectXContext context) : base(context)
        {
        }
        public static DateTime GetStartOfDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
        }
        public static DateTime GetEndOfDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999);
        }

        /// <summary>
        /// Đếm số lượng tickets chờ xử lý
        /// </summary>
        /// <returns>Số lượng ticket chờ sử lý</returns>
        public Task<int> CountTicketWait()
        {
            return Db.Complains.CountAsync(x => x.IsDelete == false && x.Status == (byte)ComplainStatus.Wait);
        }

        /// <summary>
        /// Đếm số lượng ticket đã được nhận xử lý
        /// </summary>
        /// <param name="userId">UserId cần đếm số lượng ticket</param>
        /// <returns>Số lượng toàn bộ ticket </returns>
        public Task<int> CountTicketComplain(UserState userState)
        {
            return Db.Complains.Where(x =>/* x.IsDelete == false &&*/ x.Status > (byte)ComplainStatus.Wait)
                .CountAsync();
        }

        /// <summary>
        /// Đếm số lượng ticket tôi cần xử lý
        /// </summary>
        /// <returns>Số lượng ticket được gán cho UserId</returns>
        public Task<int> CountTicketAssignForUser(int userId)
        {
            return Db.Complains.Where(x => /*x.IsDelete == false &&*/ x.Status > (byte)ComplainStatus.Wait && x.Status < (byte)ComplainStatus.Success)
                .Join(Db.ComplainUsers.Where(x => x.UserId == userId && x.IsCare == true),
                    c => c.Id, cu => cu.ComplainId, (c, cu) => c.Id)
                .Distinct()
                .CountAsync();
        }

        /// <summary>
        /// Đếm số lượng ticket trễ xử lý
        /// </summary>
        /// <param name="userId">Thông tin tài khoản đăng nhập</param>
        /// <returns>Số lượng ticket trễ xử lý</returns>
        public Task<int> CountTicketPending(UserState userState)
        {
            var timeNow = DateTime.Now.AddDays(-1);

            return Db.Complains.Where(x =>/* x.IsDelete == false &&*/ x.CreateDate <= timeNow)
                .CountAsync();

            //return Db.Complains.Join(Db.ComplainUsers, c => c.Id, cu => cu.ComplainId, (c, cu) => new { c, cu })
            //        .Where(x => x.c.IsDelete == false && x.c.CreateDate <= timeNow && userState.Type > 0 || x.cu.UserId == userState.UserId)
            //    .Select(x => x.c.Id)
            //    .Distinct()
            //    .Count();
        }

        /// <summary>
        /// Đếm số lượng ticket có người hỗ trợ
        /// </summary>
        /// <param name="userId">Thông tin tài khoản đăng nhập</param>
        /// <returns>Số lượng ticket có người hỗ trợ</returns>
        public Task<int> CountTicketHasSupporter(UserState userState)
        {
            var query =
                 Db.Complains
                     .Join(
                         Db.ComplainUsers.Where(x => x.IsCare == false),
                         complain => complain.Id,
                         complainUser => complainUser.ComplainId,
                         (c, cu) => new { c.Id}
                     ).Distinct()
                    .CountAsync();

            return query;
        }

        //Đếm số lượng danh sách ticket hỗ trợ
        public List<TicketComplain> SystemTicketSupport(UserState userState)
        {
            var query =
                Db.Complains
                    .Join(
                        Db.ComplainUsers.Where(x => x.IsCare == false &&
                        (userState.Type > 0 || x.UserId == userState.UserId)
                        && (userState.Type != 0 || x.OfficeId == userState.OfficeId)
                        && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || (x.OfficeIdPath.StartsWith(userState.OfficeIdPath + "."))))),
                        complain => complain.Id,
                        complainUser => complainUser.ComplainId,
                        (c, cu) => new TicketComplain()
                        {
                            Id = c.Id,
                            SystemId = c.SystemId
                        }
                    ).Distinct().ToList();
            return query;
        }

        public Task<int> TicketSupportCountAsync(UserState userState)
        {
            return Db.Complains.Join(Db.ComplainUsers.Where(x => x.IsCare == false &&
                        (userState.Type > 0 || x.UserId == userState.UserId)
                        && (userState.Type != 0 || x.OfficeId == userState.OfficeId)
                        && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || (x.OfficeIdPath.StartsWith(userState.OfficeIdPath + "."))))),
                        complain => complain.Id,
                        complainUser => complainUser.ComplainId, (c, cu) => c.Id).Distinct().CountAsync();
        }

        public List<ComplainSelectUserResult> ComplainSelectUser()
        {
            return Db.ComplainSelectUser();
        }

        public List<ComplainSelectUserResult> ComplainSelectUserOut(out int count)
        {
            return Db.ComplainSelectUserOut(out count);
        }

        //public System.Collections.Generic.List<ComplainSelectUserResult> SpComplainSelectUser(out int? count)
        //{
        //    int procResult;
        //    return SpComplainSelectUser(out count, out procResult);
        //}

        public ComplainModel GetAll(PageItem pageInfor, SearchInfor searchInfor)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_complain_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("PageIndex", pageInfor.PageIndex == 0 ? 1 : pageInfor.PageIndex));
                cmd.Parameters.Add(new SqlParameter("PageSize", pageInfor.PageIndex == 0 ? 25 : pageInfor.PageSize));
                cmd.Parameters.Add(new SqlParameter("Keyword", searchInfor.Keyword));
                cmd.Parameters.Add(new SqlParameter("StartDate", searchInfor.StartDate));
                cmd.Parameters.Add(new SqlParameter("FinishDate", searchInfor.FinishDate));
                cmd.Parameters.Add(new SqlParameter("AllTime", searchInfor.AllTime));
                cmd.Parameters.Add(new SqlParameter("Status", searchInfor.Status));
                cmd.Parameters.Add(new SqlParameter("SystemId", searchInfor.SystemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", searchInfor.CustomerId));
                cmd.Parameters.Add(new SqlParameter("StartStatus", searchInfor.StartStatus));
                cmd.Parameters.Add(new SqlParameter("EndStatus", searchInfor.EndStatus));

                cmd.Parameters.Add(new SqlParameter("StartStatus1", searchInfor.StartStatus1));
                cmd.Parameters.Add(new SqlParameter("EndStatus1", searchInfor.EndStatus1));

                cmd.Parameters.Add(new SqlParameter("StartStatus2", searchInfor.StartStatus2));
                cmd.Parameters.Add(new SqlParameter("EndStatus2", searchInfor.EndStatus2));

                cmd.Parameters.Add(new SqlParameter("StartStatus3", searchInfor.StartStatus3));
                cmd.Parameters.Add(new SqlParameter("EndStatus3", searchInfor.EndStatus3));

                cmd.Parameters.Add(new SqlParameter("StartStatus4", searchInfor.StartStatus4));
                cmd.Parameters.Add(new SqlParameter("EndStatus4", searchInfor.EndStatus4));
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
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<ComplainItem>(reader).ToList();
                    reader.NextResult();
                    var tmpStatus = ((IObjectContextAdapter)context).ObjectContext.Translate<ComplainStatusItem>(reader).ToList().FirstOrDefault();
                    reader.Close();
                    pageInfor.CurrentPage = pageInfor.PageIndex;
                    pageInfor.Total = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());

                    var model = new ComplainModel()
                    {
                        Page = pageInfor,
                        Search = searchInfor,
                        ListItems = tmpList,
                        ComplainStatusItem = tmpStatus
                    };
                    return model;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }

        public ComplainDetailModel GetDetail(long complainId)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_complainDetail_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("complainId", complainId));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpItem = ((IObjectContextAdapter)context).ObjectContext.Translate<Complain>(reader).ToList().FirstOrDefault();
                    reader.NextResult();
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<ComplainUserComment>(reader).ToList();
                    reader.Close();

                    var model = new ComplainDetailModel()
                    {
                        ComplainItem = tmpItem,
                        ListComments = tmpList,
                    };
                    return model;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }
        #region [Danh sách ticket chưa nhận xử lý]
        public Task<List<TicketComplain>> GetAllTicketAssignList(out long totalRecord, int page, int pageSize, string keyword,
            int systemId, DateTime? dateStart, DateTime? dateEnd)
        {
            var query =
                 Db.Complains.Where(x =>
                       (x.Code.Contains(keyword) || x.OrderCode == keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && x.Status == (byte)ComplainStatus.Wait
                        && (dateStart == null || x.CreateDate >= dateStart)
                        && (dateEnd == null || x.CreateDate <= dateEnd)
                    )
                .Join(
                        Db.Orders.Where(x => !x.IsDelete),
                        complain => complain.OrderId,
                        order => order.Id,
                        (c, o) => new { c, o }
                    )

                    .Select(s => new TicketComplain()
                    {
                        Id = s.c.Id,
                        UserId = s.o.CustomerCareUserId,
                        UserName = s.o.CustomerCareFullName,
                        Code = s.c.Code,
                        CustomerId = s.c.CustomerId,
                        CustomerName = s.c.CustomerName,
                        CreateDate = s.c.CreateDate,
                        Status = s.c.Status,
                        OrderId = s.c.OrderId,
                        LastUpdateDate = s.c.LastUpdateDate,
                        OrderCode = s.c.OrderCode,
                        OrderType = s.c.OrderType,
                        Content = s.c.Content,
                        TypeService = s.c.TypeService,
                        TypeServiceName = s.c.TypeServiceName,
                        TypeServiceClose = s.c.TypeServiceClose,
                        TypeServiceCloseName = s.c.TypeServiceCloseName,
                        SystemName = s.c.SystemName,
                        BigMoney = s.c.BigMoney,
                        RequestMoney = s.c.RequestMoney,
                    })
                    .OrderBy(x => x.CreateDate);

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        #endregion

        #region [Danh sách ticket khiếu nại]
        public Task<List<TicketComplain>> GetAllTicketComplainList(out long totalRecord, int page, int pageSize, string keyword,
            int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState)
        {
            var query =
                Db.Complains.Where(x =>
                       (x.Code.Contains(keyword) || x.OrderCode == keyword || x.CustomerName.Contains(keyword))
                        && (systemId == -1 || x.SystemId == systemId)
                        && (status == -1 || x.Status == status)
                        && (customerId == -1 || customerId == null || x.CustomerId == customerId)
                        && x.Status > (byte)ComplainStatus.Wait
                        //&& !x.IsDelete
                        && x.CreateDate >= dateStart
                        && x.CreateDate <= dateEnd
                    )
                    .Join(
                        Db.ComplainUsers.Where(x => (userId == -1 || userId == null || x.UserId == userId) && x.IsCare == true),
                        complain => complain.Id,
                        complainUser => complainUser.ComplainId,
                        (c, cu) => new { c, cu }
                    )
                    .GroupJoin(
                        Db.ClaimForRefund.Where(x=>x.IsDelete == false),
                        ticketComplain => ticketComplain.c.Id,
                        claimForRefund => (long)claimForRefund.TicketId.Value,
                        (tc, cr) => new { tc, cr }
                    )
                    .Select(s => new TicketComplain()
                    {
                        Id = s.tc.c.Id,
                        UserId = s.tc.cu.UserId,
                        UserName = s.tc.cu.UserName,
                        Code = s.tc.c.Code,
                        CustomerId = s.tc.c.CustomerId,
                        CustomerName = s.tc.c.CustomerName,
                        CreateDate = s.tc.c.CreateDate,
                        Status = s.tc.c.Status,
                        OrderId = s.tc.c.OrderId,
                        LastUpdateDate = s.tc.c.LastUpdateDate,
                        OrderCode = s.tc.c.OrderCode,
                        OrderType = s.tc.c.OrderType,
                        Content = s.tc.c.Content,
                        TypeService = s.tc.c.TypeService,
                        TypeServiceName = s.tc.c.TypeServiceName,
                        TypeServiceClose = s.tc.c.TypeServiceClose,
                        TypeServiceCloseName = s.tc.c.TypeServiceCloseName,
                        SystemName = s.tc.c.SystemName,
                        BigMoney = s.tc.c.BigMoney,
                        RequestMoney = s.tc.c.RequestMoney,
                        RealTotalRefund = s.cr.FirstOrDefault(x => x.TicketId == s.tc.c.Id && !x.IsDelete).RealTotalRefund ?? 0,
                        UserClaimName = s.cr.FirstOrDefault(x => x.TicketId == s.tc.c.Id && !x.IsDelete).UserName,
                        ReceiveDate = s.tc.cu.CreateDate,
                        ContentInternal = s.tc.c.ContentInternal,
                        ContentInternalOrder = s.tc.c.ContentInternalOrder,
                        StatusClaimForRefund = s.cr.Count() == 0 ? -1 : s.cr.FirstOrDefault(x => !x.IsDelete) != null ? s.cr.FirstOrDefault(x => !x.IsDelete).Status : s.cr.FirstOrDefault().Status,

                        CountClaimForRefund = s.cr.Count(x => x.IsDelete == false),
                    })
                    .OrderByDescending(x => x.CreateDate);

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        #endregion

        #region [Danh sách ticket tôi cần xử lý]
        public Task<List<TicketComplain>> GetAllTicketList(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState)
        {
            var query =
                Db.Complains.Where(x =>
                       (x.Code.Contains(keyword) || x.OrderCode == keyword || x.CustomerName.Contains(keyword))
                        && (systemId == -1 || x.SystemId == systemId)
                        && (status == -1 || x.Status == status)
                        && (customerId == -1 || customerId == null || x.CustomerId == customerId)
                        && x.Status > (byte)ComplainStatus.Wait
                        && x.Status < (byte)ComplainStatus.Success
                        && x.CreateDate >= dateStart
                        && x.CreateDate <= dateEnd
                    )
                    .Join(
                        Db.ComplainUsers.Where(d => d.UserId == userState.UserId && d.IsCare == true),
                        complain => complain.Id,
                        complainUser => complainUser.ComplainId,
                        (c, cu) => new { c, cu }
                    )
                    .GroupJoin(
                        Db.ClaimForRefund,
                        ticketComplain => ticketComplain.c.Id,
                        claimForRefund => (long)claimForRefund.TicketId.Value,
                        (tc, cr) => new { tc, cr }
                    )
                    .Select(s => new TicketComplain()
                    {
                        Id = s.tc.c.Id,
                        UserId = s.tc.cu.UserId,
                        UserName = s.tc.cu.UserName,
                        Code = s.tc.c.Code,
                        CustomerId = s.tc.c.CustomerId,
                        CustomerName = s.tc.c.CustomerName,
                        CreateDate = s.tc.c.CreateDate,
                        Status = s.tc.c.Status,
                        OrderId = s.tc.c.OrderId,
                        LastUpdateDate = s.tc.c.LastUpdateDate,
                        OrderCode = s.tc.c.OrderCode,
                        OrderType = s.tc.c.OrderType,
                        Content = s.tc.c.Content,
                        TypeService = s.tc.c.TypeService,
                        TypeServiceName = s.tc.c.TypeServiceName,
                        TypeServiceClose = s.tc.c.TypeServiceClose,
                        TypeServiceCloseName = s.tc.c.TypeServiceCloseName,
                        SystemName = s.tc.c.SystemName,
                        BigMoney = s.tc.c.BigMoney,
                        RequestMoney = s.tc.c.RequestMoney,
                        ContentInternal = s.tc.c.ContentInternal,
                        ContentInternalOrder = s.tc.c.ContentInternalOrder,
                        RealTotalRefund = s.cr.FirstOrDefault(x => x.TicketId == s.tc.c.Id && !x.IsDelete).RealTotalRefund ?? 0,
                        UserClaimName = s.cr.FirstOrDefault(x => x.TicketId == s.tc.c.Id && !x.IsDelete).UserName,
                        ReceiveDate = s.tc.cu.CreateDate,

                        StatusClaimForRefund = s.cr.Count() == 0 ? -1 : s.cr.FirstOrDefault(x => !x.IsDelete) != null ?
                                                s.cr.FirstOrDefault(x => !x.IsDelete).Status : s.cr.FirstOrDefault().Status,
                        CountClaimForRefund = s.cr.Count(x => x.IsDelete == false)
                    })
                    .OrderByDescending(x => x.CreateDate);

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        #endregion

        #region [Danh sách ticket trễ xử lý]
        public Task<List<TicketComplain>> GetAllTicketLastList(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState)
        {
            var timeNow = DateTime.Now.AddDays(-1);

            var listticket = new List<TicketComplain>();
            var query =
                Db.Complains.Where(x =>
                       (x.Code.Contains(keyword) || x.OrderCode == keyword || x.CustomerName.Contains(keyword))
                        && (systemId == -1 || x.SystemId == systemId)
                        && (status == -1 || x.Status == status)
                        && (customerId == -1 || customerId == null || x.CustomerId == customerId)
                        //&& x.Status > (byte)ComplainStatus.Wait
                        && x.CreateDate <= timeNow
                        && x.CreateDate >= dateStart
                        && x.CreateDate <= dateEnd
                    )
                    .Join(
                        Db.ComplainUsers.Where(x => (userId == -1 || userId == null || x.UserId == userId) && x.IsCare == true),
                        complain => complain.Id,
                        complainUser => complainUser.ComplainId,
                        (c, cu) => new { c, cu }
                    )
                    .GroupJoin(
                        Db.ClaimForRefund,
                        ticketComplain => ticketComplain.c.Id,
                        claimForRefund => (long)claimForRefund.TicketId.Value,
                        (tc, cr) => new { tc, cr }
                    )
                    .Select(s => new TicketComplain()
                    {
                        Id = s.tc.c.Id,
                        UserId = s.tc.cu.UserId,
                        UserName = s.tc.cu.UserName,
                        Code = s.tc.c.Code,
                        CustomerId = s.tc.c.CustomerId,
                        CustomerName = s.tc.c.CustomerName,
                        CreateDate = s.tc.c.CreateDate,
                        Status = s.tc.c.Status,
                        OrderId = s.tc.c.OrderId,
                        LastUpdateDate = s.tc.c.LastUpdateDate,
                        OrderCode = s.tc.c.OrderCode,
                        OrderType = s.tc.c.OrderType,
                        Content = s.tc.c.Content,
                        TypeService = s.tc.c.TypeService,
                        TypeServiceName = s.tc.c.TypeServiceName,
                        TypeServiceClose = s.tc.c.TypeServiceClose,
                        TypeServiceCloseName = s.tc.c.TypeServiceCloseName,
                        SystemName = s.tc.c.SystemName,
                        BigMoney = s.tc.c.BigMoney,
                        RequestMoney = s.tc.c.RequestMoney,
                        ContentInternal = s.tc.c.ContentInternal,
                        ContentInternalOrder = s.tc.c.ContentInternalOrder,
                        RealTotalRefund = s.cr.FirstOrDefault(x => x.TicketId == s.tc.c.Id && !x.IsDelete).RealTotalRefund ?? 0,
                        UserClaimName = s.cr.FirstOrDefault(x => x.TicketId == s.tc.c.Id && !x.IsDelete).UserName,
                        ReceiveDate = s.tc.cu.CreateDate,
                        StatusClaimForRefund = s.cr.Count() == 0 ? -1 : s.cr.FirstOrDefault(x => !x.IsDelete) != null ? s.cr.FirstOrDefault(x => !x.IsDelete).Status : s.cr.FirstOrDefault().Status,
                        CountClaimForRefund = s.cr.Count(x => x.IsDelete == false)
                    })
                    //.Where(x => x.UserSupportNo > 0)
                    .OrderByDescending(x => x.CreateDate);

            totalRecord = query.Count();

            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        #endregion

        #region [Danh sách ticket có người hỗ trợ]
        public Task<List<TicketComplain>> GetAllTicketSupportList(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState)
        {
            var query =
                Db.Complains.Where(x =>
                       (x.Code.Contains(keyword) || x.OrderCode == keyword || x.CustomerName.Contains(keyword))
                        && (systemId == -1 || x.SystemId == systemId)
                        && (status == -1 || x.Status == status)
                        && (customerId == -1 || customerId == null || x.CustomerId == customerId)
                        && x.Status > (byte)ComplainStatus.Wait
                        && x.CreateDate >= dateStart
                        && x.CreateDate <= dateEnd
                    )
                    .GroupJoin(
                        Db.ComplainUsers.Where(x =>
                             (userId == -1 || userId == null || x.UserId == userId && x.IsCare != null)
                        ),
                        complain => complain.Id,
                        complainUser => complainUser.ComplainId,
                        (c, cu) => new { c, cu }
                    )
                    .GroupJoin(
                        Db.ClaimForRefund,
                        ticketComplain => ticketComplain.c.Id,
                        claimForRefund => (long)claimForRefund.TicketId.Value,
                        (tc, cr) => new { tc, cr }
                    )
                    .Select(s => new TicketComplain()
                    {
                        Id = s.tc.c.Id,
                        UserName = s.tc.cu.FirstOrDefault(x => x.IsCare == true && (userState.Type != 0 || x.UserId == userState.UserId)) == null ? "" : s.tc.cu.FirstOrDefault(x => x.IsCare == true).UserName,
                        Code = s.tc.c.Code,
                        CustomerId = s.tc.c.CustomerId,
                        CustomerName = s.tc.c.CustomerName,
                        CreateDate = s.tc.c.CreateDate,
                        Status = s.tc.c.Status,
                        OrderId = s.tc.c.OrderId,
                        LastUpdateDate = s.tc.c.LastUpdateDate,
                        OrderCode = s.tc.c.OrderCode,
                        OrderType = s.tc.c.OrderType,
                        Content = s.tc.c.Content,
                        TypeService = s.tc.c.TypeService,
                        TypeServiceName = s.tc.c.TypeServiceName,
                        TypeServiceClose = s.tc.c.TypeServiceClose,
                        TypeServiceCloseName = s.tc.c.TypeServiceCloseName,
                        SystemName = s.tc.c.SystemName,
                        BigMoney = s.tc.c.BigMoney,
                        RequestMoney = s.tc.c.RequestMoney,
                        ContentInternal = s.tc.c.ContentInternal,
                        ContentInternalOrder = s.tc.c.ContentInternalOrder,
                        RealTotalRefund = s.cr.FirstOrDefault(x => x.TicketId == s.tc.c.Id && !x.IsDelete).RealTotalRefund ?? 0,
                        UserClaimName = s.cr.FirstOrDefault(x => x.TicketId == s.tc.c.Id && !x.IsDelete).UserName,
                        ReceiveDate = s.tc.cu.FirstOrDefault(x => x.IsCare == true).CreateDate,
                        UserId = s.tc.cu.FirstOrDefault(x => x.IsCare == true).UserId,
                        UserSupport = s.tc.cu.Where(x => x.IsCare == false).Select(x => x.UserName).ToList(),
                        StatusClaimForRefund = s.cr.Count() == 0 ? -1 : s.cr.FirstOrDefault(x => !x.IsDelete) != null ? s.cr.FirstOrDefault(x => !x.IsDelete).Status : s.cr.FirstOrDefault().Status,
                        CountClaimForRefund = s.cr.Count(x => x.IsDelete == false),
                        UserSupportNo = s.tc.cu.Count(x => x.IsCare == false)
                    })
                    .Where(x => x.UserSupportNo > 0)
                    .OrderByDescending(x => x.CreateDate);
            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        #endregion

        #region [Lấy danh sách user trong phòng CSKH]
        //Lấy về danh sách User theo phòng 
        public List<User> GetUserSearch(out long totalRecord, string keyword, int? page)
        {
            var listUser = Db.Users.Where(x =>
                      !x.IsDelete && (x.FullName.Contains(keyword) || x.Email.Contains(keyword) || x.Phone.Contains(keyword))
                   )
                   .Join(
                       Db.UserPositions,
                       users => users.Id,
                       userPositions => userPositions.UserId,
                       (u, up) => new { u, up }
                   ).Join(
                       Db.Offices.Where(x => !x.IsDelete && x.Type == (byte)OfficeType.CustomerCare),
                       group => group.up.OfficeId,
                       offices => offices.Id,
                       (g, o) => new { u = g.u }
                   ).ToList()
                   .Select(s => new User()
                   {
                       Id = s.u.Id,
                       FullName = s.u.FullName,
                       Email = s.u.Email,
                       Avatar = s.u.Avatar,
                       Phone = s.u.Phone,
                   })
                   .OrderByDescending(x => x.FullName);
            totalRecord = listUser.Count();

            return listUser.Skip((page ?? 1 - 1) * 10).Take(10).ToList();
        }
        #endregion

        #region [Danh sách hỗ trợ xử lý khiếu nại của các phòng ban khác]
        public Task<List<TicketComplain>> GetAllTicketSupportOfficeList(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, UserState userState)
        {
            var query =
                Db.Complains.Where(x =>
                       (x.Code.Contains(keyword)
                       || x.OrderCode == keyword || x.CustomerName.Contains(keyword))
                        && (systemId == -1 || x.SystemId == systemId)
                        && (status == -1 || x.Status == status)
                        && x.Status > (byte)ComplainStatus.Wait
                        && x.CreateDate >= dateStart
                        && x.CreateDate <= dateEnd
                    )
                    .GroupJoin(
                        Db.ComplainUsers,
                        complain => complain.Id,
                        complainUser => complainUser.ComplainId,
                        (c, cu) => new { c, cu }
                    )
                    .Select(s => new TicketComplain()
                    {
                        Id = s.c.Id,
                        UserId = s.cu.FirstOrDefault(x => x.IsCare == true) == null ? null : s.cu.FirstOrDefault(x => x.IsCare == true).UserId,
                        UserName = s.cu.FirstOrDefault(x => x.IsCare == true) == null ? null : s.cu.FirstOrDefault(x => x.IsCare == true).UserName,
                        Code = s.c.Code,
                        CustomerId = s.c.CustomerId,
                        CustomerName = s.c.CustomerName,
                        CreateDate = s.c.CreateDate,
                        Status = s.c.Status,
                        OrderId = s.c.OrderId,
                        OrderCode = s.c.OrderCode,
                        OrderType = s.c.OrderType,
                        Content = s.c.Content,
                        TypeService = s.c.TypeService,
                        TypeServiceName = s.c.TypeServiceName,
                        TypeServiceClose = s.c.TypeServiceClose,
                        TypeServiceCloseName = s.c.TypeServiceCloseName,
                        ContentInternal = s.c.ContentInternal,
                        ContentInternalOrder = s.c.ContentInternalOrder,
                        UserSupportNo = s.cu.Count(x =>
                        (userState.Type > 0 || x.UserId == userState.UserId)
                        && (userState.Type != 0 || x.OfficeId == userState.OfficeId)
                        && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || (x.OfficeIdPath.StartsWith(userState.OfficeIdPath + "."))))),
                    })
                    .Where(x => x.UserSupportNo > 0)
                    .OrderByDescending(x => x.CreateDate);

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        #endregion

        #region [Thống kê tình hình khiếu nại theo thời gian]
        public List<ProfitDay> GetTicketSituationOnTime(out long Total, DateTime startDay, DateTime endDay)
        {
            var listTicket = Db.Complains.Where(s =>
                                                    s.CreateDate >= startDay
                                                    && s.CreateDate <= endDay).ToList();

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
                var count = listTicket.Where(x => x.CreateDate.ToShortDateString() == item).Count();

                query.Add(new ProfitDay()
                {
                    Created = item,
                    TotalOrder = count
                });
            }
            Total = query.Sum(s => s.TotalOrder);
            return query.ToList();
        }

        //Thống kê ticket mà nhân viên tiếp nhân theo thời gian
        public List<TicketComplain> GetTicketReceiveSituation(out long Total, DateTime startDay, DateTime endDay)
        {
            var listTicket = Db.Complains.Where(x => !x.IsDelete && x.Status > (byte)ComplainStatus.Wait)
                                    .Join(
                                        Db.ComplainUsers.Where(s => s.UpdateDate >= startDay
                                                    && s.UpdateDate <= endDay && s.IsCare == true
                                        ),
                                        complain => complain.Id,
                                        complainUser => complainUser.ComplainId,
                                        (c, cu) => new { c, cu })
                                        .Select(s => new TicketComplain()
                                        {
                                            Id = s.c.Id,
                                            UserId = s.cu.UserId,
                                            UserName = s.cu.UserName
                                        }).ToList();
            Total = listTicket.Count();

            return listTicket;
        }

        //Thống kê ticket theo loại VIP theo thời gian
        public List<TicketComplain> GetTicketVIPSituation(out long Total, DateTime startDay, DateTime endDay)
        {
            var listTicket = Db.Complains.Where(s => !s.IsDelete && s.CreateDate >= startDay
                                                    && s.CreateDate <= endDay)
                                    .Join(
                                        Db.Customers,
                                        complain => complain.CustomerId,
                                        customer => customer.Id,
                                        (c, cu) => new { c, cu })
                                        .Select(s => new TicketComplain()
                                        {
                                            Id = s.c.Id,
                                            LevelId = s.cu.LevelId,
                                            LevelName = s.cu.LevelName
                                        }).ToList();
            Total = listTicket.Count();
            return listTicket;
        }

        //Thống kê ticket theo loại Khiếu nại theo thời gian
        public List<TicketComplain> GetTicketTypeSituation(out long Total, DateTime startDay, DateTime endDay)
        {
            var listTicket = Db.Complains.Where(s => !s.IsDelete && s.CreateDate >= startDay
                                                    && s.CreateDate <= endDay)
                                        .Select(s => new TicketComplain()
                                        {
                                            Id = s.Id,
                                            TypeService = (s.TypeServiceClose == null ? s.TypeService : s.TypeServiceClose) ?? 0,
                                            TypeServiceName = s.TypeServiceCloseName == null ? s.TypeServiceName : s.TypeServiceCloseName
                                        }).ToList();
            Total = listTicket.Count();
            return listTicket;
        }

        //Thống kê ticket xử lý xong theo thời gian
        public List<ProfitDay> GetTicketSuccessSituation(out long Total, DateTime startDay, DateTime endDay)
        {
            var listTicket = Db.Complains.Where(s => !s.IsDelete && s.LastUpdateDate >= startDay
                                                    && s.LastUpdateDate <= endDay && s.Status == (byte)ComplainStatus.Success)
                                        .Select(s => new TicketComplain()
                                        {
                                            Id = s.Id,
                                            LastUpdateDate = s.LastUpdateDate
                                        }).ToList();
            Total = listTicket.Count();
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
                var count = listTicket.Where(x => (x.LastUpdateDate ?? DateTime.Now).ToShortDateString() == item).Count();

                query.Add(new ProfitDay()
                {
                    Created = item,
                    TotalOrder = count
                });
            }
            return query;
        }

        //Thống kê số tiền hoàn bồi/ ticket theo thời gian
        public List<ProfitDay> GetTicketClaimSituation(out long Total, DateTime startDay, DateTime endDay)
        {
            var listTicket = Db.Complains.Where(x => !x.IsDelete && x.Status >= (byte)ComplainStatus.AccountantFinish)
                                    .Join(
                                        Db.ComplainHistories.Where(s => s.CreateDate >= startDay
                                                    && s.CreateDate <= endDay && s.Status == (byte)ComplainStatus.AccountantFinish
                                        ),
                                        complain => complain.Id,
                                        complainUser => complainUser.ComplainId,
                                        (c, ch) => new { c, ch })
                                        .Join(
                                            Db.ClaimForRefund.Where(x => x.IsDelete == false),
                                            complainhistory => complainhistory.c.Id,
                                            claim => claim.TicketId.Value,
                                            (cch, cl) => new { cch, cl }
                                           ).ToList()
                                           .Select(s => new TicketComplain()
                                           {
                                               Id = s.cch.c.Id,
                                               LastUpdateDate = s.cch.ch.CreateDate,
                                               BigMoney = s.cl.RealTotalRefund
                                           }).ToList();
            Total = listTicket.Count();
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
                var value = listTicket.Where(x => (x.LastUpdateDate ?? DateTime.Now).ToShortDateString() == item);
                query.Add(new ProfitDay()
                {
                    Created = item,
                    TotalOrder = value.Count(),
                    TotalMoney = value.Sum(s => s.BigMoney)
                });
            }
            return query;
        }
        #endregion

        #region Ticket knockout
        public ComplainModel GetAllByLinq(PageItem pageInfor, SearchInfor searchInfor)
        {
            var query = Db.Complains.Where(
                    x => (searchInfor.Status == -1 || x.Status == searchInfor.Status)
                    && x.Code.Contains(searchInfor.Keyword)
                    && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.CreateDate >= searchInfor.StartDate)
                    && (searchInfor.FinishDate == null || x.CreateDate <= searchInfor.FinishDate)))
                    && !x.IsDelete
                    && x.CustomerId == searchInfor.CustomerId
                    && x.SystemId == searchInfor.SystemId
                ).Select(m => new ComplainItem()
                {
                    ROW = 1,
                    id = m.Id,
                    orderId = m.OrderId,
                    orderCode = m.OrderCode,
                    code = m.Code,
                    createDate = m.CreateDate,
                    lastUpdateDate = m.LastUpdateDate,
                    content = m.Content,
                    lastReply = m.LastReply,
                    Status = (byte)m.Status,
                    OrderType = (byte)m.OrderType
                });
            var queryCount = Db.Complains.Where(
                    x => x.Code.Contains(searchInfor.Keyword)
                    && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.CreateDate >= searchInfor.StartDate)
                    && (searchInfor.FinishDate == null || x.CreateDate <= searchInfor.FinishDate)))
                    && !x.IsDelete
                    && x.CustomerId == searchInfor.CustomerId
                    && x.SystemId == searchInfor.SystemId
                ).Select(m => new ComplainItem()
                {
                    Status = (byte)m.Status,
                });
            pageInfor.CurrentPage = pageInfor.PageIndex;
            pageInfor.Total = query.Count();
            var tmpList = query.OrderByDescending(x => new { x.createDate })
                    .Skip((pageInfor.CurrentPage - 1) * pageInfor.PageSize)
                    .Take(pageInfor.PageSize)
                    .ToList();
            var tmpStatus = new ComplainStatusItem()
            {
                Wait = queryCount.Where(m => m.Status == 0).Count(),
                Process = queryCount.Where(m => m.Status == 1).Count(),
                Success = queryCount.Where(m => m.Status == 2).Count(),
                Cancel = queryCount.Where(m => m.Status == 3).Count(),
            };
            var model = new ComplainModel()
            {
                Page = pageInfor,
                Search = searchInfor,
                ListItems = tmpList,
                ComplainStatusItem = tmpStatus
            };
            return model;
        }
        #endregion
        //Thong ke tổng số lượng đơn báo giá theo đầu nhân viên theo ngày, tháng
        public List<CustomerUser> GetOrderWaitSituationAllDay(List<UserOfficeResult> list, DateTime? startDay, DateTime? endDay, byte status)
        {
            var start = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToString() : startDay.ToString());
            var end = DateTime.Parse(string.IsNullOrEmpty(endDay.ToString()) ? DateTime.Now.ToString() : endDay.ToString());
            start = GetStartOfDay(start);
            end = GetEndOfDay(end);
            var listOrder = Db.Orders.Where(x => !x.IsDelete)
                                                .Join(Db.OrderHistories.Where(x =>
                                                        x.CreateDate >= start
                                                        && x.CreateDate <= end
                                                        && x.Status == status),
                                                    order => order.Id,
                                                    orderHistory => orderHistory.OrderId,
                                                    (o, h) => new { o, h }).Select(s => new { s.o.Id, s.o.CustomerCareUserId, s.o.CustomerName }).Distinct().ToList();

            var query = new List<CustomerUser>();
            foreach (var item in list)
            {
                var count = listOrder.Count(x => x.CustomerCareUserId == item.Id);

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

        //thống kê nhận đơn báo giá theo thời gian
        public List<CustomerUser> GetOrderWaitSituationContinuteAllDay(DateTime? startDay, DateTime? endDay, byte status)
        {
            var start = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToString() : startDay.ToString());
            var end = DateTime.Parse(string.IsNullOrEmpty(endDay.ToString()) ? DateTime.Now.ToString() : endDay.ToString());
            start = GetStartOfDay(start);
            end = GetEndOfDay(end);
            var listOrder = Db.Orders.Where(x => !x.IsDelete)
                                                .Join(Db.OrderHistories.Where(x =>
                                                        x.CreateDate >= start
                                                        && x.CreateDate <= end
                                                        && x.Status == status),
                                                    order => order.Id,
                                                    orderHistory => orderHistory.OrderId,
                                                    (o, h) => new { o, h }).Select(s => new { s.o.Id, s.h.CreateDate }).Distinct().ToList();
            var query = new List<CustomerUser>();
            var list = new List<string>();
            DateTime tmpDate = start;
            do
            {
                list.Add(tmpDate.ToShortDateString());
                tmpDate = tmpDate.AddDays(1);
            } while (tmpDate <= end);

            foreach (var item in list)
            {
                var count = listOrder.Where(x => x.CreateDate.ToShortDateString() == item).Count();

                query.Add(new CustomerUser()
                {
                    FullName = item,
                    TotalCusstomer = count
                });
            }
           
            return query;
        }

        //Thời gian tạo đơn nhiều nhất trong ngày của khách hàng
        public List<CustomerUser> GetOrderCustomerSituation(DateTime? startDay, DateTime? endDay, byte status)
        {
            var start = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToString() : startDay.ToString());
            var end = DateTime.Parse(string.IsNullOrEmpty(endDay.ToString()) ? DateTime.Now.ToString() : endDay.ToString());
            start = GetStartOfDay(start);
            end = GetEndOfDay(end);
            var listOrder = Db.Orders.Where(x => !x.IsDelete /*&& x.CreatedTool == (byte)CreatedTool.Extension*/)
                                                .Join(Db.OrderHistories.Where(x =>
                                                        x.CreateDate >= start
                                                        && x.CreateDate <= end
                                                        && x.Status == status),
                                                    order => order.Id,
                                                    orderHistory => orderHistory.OrderId,
                                                    (o, h) => new { o, h }).Select(s => new { s.o.Id, s.h.CreateDate }).Distinct().ToList();
            var query = new List<CustomerUser>();
            var list = new List<string>();
            int tmpDate = 1;
            do
            {
                list.Add(tmpDate.ToString());
                tmpDate += 1;
            } while (tmpDate <= 24);

            foreach (var item in list)
            {
                var count = listOrder.Where(x => x.CreateDate.Hour.ToString() == item).Count();

                query.Add(new CustomerUser()
                {
                    FullName = item,
                    TotalCusstomer = count
                });
            }

            return query;
        }

        //Thời gian tạo đơn nhiều nhất trong ngày của đặt hàng
        public List<CustomerUser> GetOrderUserSituation(DateTime? startDay, DateTime? endDay, byte status)
        {
            var start = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToString() : startDay.ToString());
            var end = DateTime.Parse(string.IsNullOrEmpty(endDay.ToString()) ? DateTime.Now.ToString() : endDay.ToString());
            start = GetStartOfDay(start);
            end = GetEndOfDay(end);
            var listOrder = Db.Orders.Where(x => !x.IsDelete && x.CreatedTool == (byte)CreatedTool.Auto)
                                                .Join(Db.OrderHistories.Where(x =>
                                                        x.CreateDate >= start
                                                        && x.CreateDate <= end
                                                        && x.Status == status),
                                                    order => order.Id,
                                                    orderHistory => orderHistory.OrderId,
                                                    (o, h) => new { o, h }).Select(s => new { s.o.Id, s.h.CreateDate }).Distinct().ToList();
            var query = new List<CustomerUser>();
            var list = new List<string>();
            int tmpDate = 1;
            do
            {
                list.Add(tmpDate.ToString());
                tmpDate += 1;
            } while (tmpDate <= 24);

            foreach (var item in list)
            {
                var count = listOrder.Where(x => x.CreateDate.Hour.ToString() == item).Count();

                query.Add(new CustomerUser()
                {
                    FullName = item,
                    TotalCusstomer = count
                });
            }

            return query;
        }



        #region Hàm cung cấp cho Job kiểm tra khiếu nại đơn hàng
        /// <summary>
        /// Hàm kiểm tra tồn tại các khiếu nại chưa hoàn thành chạy cho Job tự động
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public CheckComplainResult AutoCheckComplainResult(int orderId)
        {
            //0. Kiểm tra OrderId
            if (orderId <= 0)
            {
                return new CheckComplainResult { Status = -1, Msg = "The order requires an incorrect check!" };
            }

            //1. Kiểm tra xem còn khiếu nại nào của khách hàng trong đơn hàng mà vẫn còn đang xử lý hay không ?
            if (Db.Complains.Any(x =>
                !x.IsDelete && x.OrderId == orderId && x.Status != (byte)ComplainStatus.Success &&
                x.Status != (byte)ComplainStatus.Cancel))
            {
                return new CheckComplainResult
                {
                    Status = -2,
                    Msg = "The complaint has not been resolved, has not settle the order!"
                };
            }

            return new CheckComplainResult { Status = 1, Msg = "Orders not exist complaints! Allows settle the order!" };
        }

        //Cập nhật thông tin giá đơn hàng trong chi tiết phiếu yêu cầu hoàn tiền

        public void UpdateClaimForRefundDetail(int productId, int orderType)
        {
            //Lấy danh sách chi tiết hoàn tiền theo productId
            var claimForRefundDetail = Db.ClaimForRefundDetail.Where(x => x.ProductId == productId);
            //Cập nhật thông tin chi tiết hoàn tiền khiếu nại
            if (claimForRefundDetail != null)
            {
                if (orderType == (byte)OrderType.Order)
                {
                    var orderDetail = Db.OrderDetails.FirstOrDefault(x => !x.IsDelete && x.Id == productId);
                    foreach (var item in claimForRefundDetail)
                    {
                        item.Price = orderDetail.Price;
                        item.TotalPrice = orderDetail.TotalPrice;
                        item.Name = orderDetail.Name;
                        item.Link = orderDetail.Link;
                        item.Image = orderDetail.Image;
                        item.Quantity = orderDetail.QuantityBooked;
                        item.Size = orderDetail.Size;
                        item.Color = orderDetail.Color;
                    }
                }
                if (orderType == (byte)OrderType.Deposit)
                {
                    var orderDetail = Db.DepositDetails.FirstOrDefault(x => !x.IsDelete && x.Id == productId);
                    foreach (var item in claimForRefundDetail)
                    {
                        item.Name = orderDetail.CategoryName;
                        item.Image = orderDetail.Image;
                        item.Quantity = orderDetail.Quantity;
                        item.Size = orderDetail.Size;
                    }
                }
                Db.SaveChanges();

            }
        }

        #endregion
    }

    public class CheckComplainResult
    {
        /// <summary>
        /// Trạng thái lỗi thanh toán
        /// -1: Đơn hàng yêu cầu kiểm tra không chính xác
        /// -2: Vẫn còn khiếu nại chưa giải quyết xong, chưa được chốt đơn!
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Nội dung lỗi
        /// </summary>
        public string Msg { get; set; }
    }
}
