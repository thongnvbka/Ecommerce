using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using Common.Items;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Collections.Generic;
using Common.Emums;
using Library.ViewModels.Items;

namespace Library.DbContext.Repositories
{
    public class DebitHistoryRepository : Repository<DebitHistory>
    {
        public DebitHistoryRepository(ProjectXContext context) : base(context)
        {
        }

        public DebitHistoryModel GetAllByCustomer(PageItem pageInfor, SearchInfor searchInfor)
        {
            using (var context = new ProjectXContext())
            {
                byte debitType = (byte)DebitType.Return;
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_debitHistory_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("PageIndex", pageInfor.PageIndex == 0 ? 1 : pageInfor.PageIndex));
                cmd.Parameters.Add(new SqlParameter("PageSize", pageInfor.PageSize == 0 ? 25 : pageInfor.PageSize));
                cmd.Parameters.Add(new SqlParameter("Keyword", searchInfor.Keyword));
                cmd.Parameters.Add(new SqlParameter("StartDate", searchInfor.StartDate));
                cmd.Parameters.Add(new SqlParameter("FinishDate", searchInfor.FinishDate));
                cmd.Parameters.Add(new SqlParameter("AllTime", searchInfor.AllTime));
                cmd.Parameters.Add(new SqlParameter("CustomerId", searchInfor.CustomerId));
                cmd.Parameters.Add(new SqlParameter("DebitType", debitType));
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
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<DebitItem>(reader).ToList();
                    reader.Close();
                    pageInfor.CurrentPage = pageInfor.PageIndex;
                    pageInfor.Total = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());

                    var model = new DebitHistoryModel()
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


        public DebitHistoryModelV2 GetAllByLinq(PageItem pageInfor, SearchInfor searchInfor)
        {
            byte debitType = (byte)DebitType.Return;
            var query = ((Db.DebitHistorys.Where(
                    x => x.DebitCode.Contains(searchInfor.Keyword)
                    && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.Created >= searchInfor.StartDate)
                    && (searchInfor.FinishDate == null || x.Created <= searchInfor.FinishDate)))
                    && x.SubjectId == searchInfor.CustomerId
                    && x.DebitType == debitType
                    && x.OrderId != null)
                .OrderByDescending(x => x.Created)
                .GroupBy(x => x.DebitId)
                .Select(g => new { g, count = g.Count() }))
                .ToList()
                .SelectMany(t => t.g.Select(b => b).Zip(Enumerable.Range(1, t.count), (j, i) => new DebitItemV2()
                {
                    DebitId = (int)j.DebitId,
                    DebitType = (byte)j.DebitType,
                    DebitCode = j.DebitCode,
                    Money = (Db.DebitHistorys.Where(s => s.OrderId == j.OrderId && s.DebitType == debitType && s.SubjectId == searchInfor.CustomerId).Select(s => (decimal)s.Money).DefaultIfEmpty(0).Sum()),
                    OrderId = (int)j.OrderId,
                    OrderType = (byte)j.OrderType,
                    OrderCode = j.OrderCode,
                    PayReceivableIName = j.PayReceivableIName,
                    SubjectId = (int)j.SubjectId,
                    Created = j.Created,
                    //ListDebitHistory = Db.DebitHistorys.Where(m => m.DebitId == j.DebitId && m.SubjectId == searchInfor.CustomerId && m.DebitType == debitType).ToList(),
                    ROW = i
                })))
                .Union(
                    (Db.DebitHistorys.Where(
                        x => x.DebitCode.Contains(searchInfor.Keyword)
                        && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.Created >= searchInfor.StartDate)
                        && (searchInfor.FinishDate == null || x.Created <= searchInfor.FinishDate)))
                        && x.SubjectId == searchInfor.CustomerId
                        && x.DebitType == debitType
                        && x.OrderId == null)
                    .OrderByDescending(x => x.Created)
                    .GroupBy(x => x.DebitId)
                    .Select(g => new { g, count = g.Count() }))
                    .ToList()
                    .SelectMany(t => t.g.Select(b => b).Zip(Enumerable.Range(1, t.count), (j, i) => new DebitItemV2()
                    {
                        DebitId = (int)j.DebitId,
                        DebitType = (byte)j.DebitType,
                        DebitCode = j.DebitCode,
                        Money = (Db.DebitHistorys.Where(s => s.DebitId == j.DebitId && s.DebitType == debitType && s.SubjectId == searchInfor.CustomerId && s.OrderId == null).Select(s => (decimal)s.Money).DefaultIfEmpty(0).Sum()),
                        OrderId = (int)j.OrderId,
                        OrderType = (byte)j.OrderType,
                        OrderCode = j.OrderCode,
                        PayReceivableIName = j.PayReceivableIName,
                        SubjectId = (int)j.SubjectId,
                        Created = j.Created,
                        //ListDebitHistory = Db.DebitHistorys.Where(m => m.DebitId == j.OrderId && m.SubjectId == searchInfor.CustomerId && m.DebitType == debitType).ToList(),
                        ROW = i
                    }))
                );

            pageInfor.CurrentPage = pageInfor.PageIndex;
            pageInfor.Total = query.Where(s => s.ROW == 1).Count();
            var tmpList = query.Where(s => s.ROW == 1).Skip((pageInfor.CurrentPage - 1) * pageInfor.PageSize)
                    .Take(pageInfor.PageSize)
                    .ToList();
            
            var model = new DebitHistoryModelV2()
            {
                Page = pageInfor,
                Search = searchInfor,
                ListItems = tmpList
            };
            return model;
        }
    }
}

