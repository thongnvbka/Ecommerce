using Common.Items;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using Library.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Library.DbContext.Repositories
{
    public class SourceRepository : Repository<Source>
    {
        public SourceRepository(ProjectXContext context) : base(context)
        {
        }

        public SourceModel GetAll(PageItem pageInfor, SearchInfor searchInfor)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_source_select";
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
                cmd.Parameters.Add(new SqlParameter("IsSearch", searchInfor.IsSearch));
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

                cmd.Parameters.Add(new SqlParameter("StartStatus5", searchInfor.StartStatus5));
                cmd.Parameters.Add(new SqlParameter("EndStatus5", searchInfor.EndStatus5));
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
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<SourceItem>(reader).ToList();
                    reader.NextResult();
                    var tmpStatus = ((IObjectContextAdapter)context).ObjectContext.Translate<SourceStatusItem>(reader).ToList().FirstOrDefault();
                    reader.Close();
                    pageInfor.CurrentPage = pageInfor.PageIndex;
                    pageInfor.Total = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());

                    var model = new SourceModel()
                    {
                        Page = pageInfor,
                        Search = searchInfor,
                        ListItems = tmpList,
                        StatusItem = tmpStatus
                    };
                    return model;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
        }
        public SourceModel GetAllByLinq(PageItem pageInfor, SearchInfor searchInfor)
        {
            var query = Db.Sources.Where(
                    x => (searchInfor.Status == -1 || x.Status == searchInfor.Status)
                    && x.Code.Contains(searchInfor.Keyword)
                    && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.CreateDate >= searchInfor.StartDate)
                    && (searchInfor.FinishDate == null || x.CreateDate <= searchInfor.FinishDate)))
                    && !x.IsDelete
                    && x.CustomerId == searchInfor.CustomerId
                    && x.SystemId == searchInfor.SystemId
                ).Select(m => new SourceItem()
                {
                    ROW = 1,
                    id = m.Id,
                    code = m.Code,
                    CreateDate = m.CreateDate,
                    ImagePath = (Db.SourceDetails.Where(s => s.SourceId == m.Id).FirstOrDefault().ImagePath1),
                    Quantity = (Db.SourceDetails.Where(s => s.SourceId == m.Id).FirstOrDefault() != null ? Db.SourceDetails.Where(s => s.SourceId == m.Id).FirstOrDefault().Quantity : 0),
                    TypeService = (byte)m.TypeService,
                    TypeServiceName = m.TypeServiceName,
                    Status = m.Status
                });
            var queryCount = Db.Sources.Where(
                    x => x.Code.Contains(searchInfor.Keyword)
                    && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.CreateDate >= searchInfor.StartDate)
                    && (searchInfor.FinishDate == null || x.CreateDate <= searchInfor.FinishDate)))
                    && !x.IsDelete
                    && x.CustomerId == searchInfor.CustomerId
                    && x.SystemId == searchInfor.SystemId
                ).Select(m => new SourceItem()
                {
                    Status = m.Status,
                });
            pageInfor.CurrentPage = pageInfor.PageIndex;
            pageInfor.Total = query.Count();
            var tmpList = query.OrderByDescending(x => new { x.CreateDate })
                    .Skip((pageInfor.CurrentPage - 1) * pageInfor.PageSize)
                    .Take(pageInfor.PageSize)
                    .ToList();
            var tmpStatus = new SourceStatusItem()
            {
                WaitProccess = queryCount.Where(m => m.Status == 0).Count(),
                Proccess = queryCount.Where(m => m.Status == 1).Count(),
                WaitingChoice = queryCount.Where(m => m.Status == 2).Count(),
                Finish = queryCount.Where(m => m.Status == 3).Count(),
                Cancel = queryCount.Where(m => m.Status == 4).Count(),
            };
            var model = new SourceModel()
            {
                Page = pageInfor,
                Search = searchInfor,
                ListItems = tmpList,
                StatusItem = tmpStatus
            };
            return model;
        }
        //todo lay ra danh sach don hang tim nguon cung voi chi tiet cua no
        public Task<List<SourcingBySourcingDetailResults>> GetSourcingJoinSourcingDetail(
            out long totalRecord, int page, int pageSize, string keyword, int? status, 
            DateTime? dateStart, DateTime? dateEnd, int customerId)
        {
            var query =
                Db.Sources.Where(
                        x => (x.Code.Contains(keyword) || x.Code.Contains(keyword) ||
                               x.CustomerEmail.Contains(keyword) || x.CustomerName.Contains(keyword)
                             || x.CustomerAddress.Contains(keyword) || x.UnsignName.Contains(keyword))
                        && (x.CustomerId == customerId)
                        && !x.IsDelete
                    )
                    .Join(
                        Db.SourceDetails.Where(
                            x => !x.IsDelete
                                && (status == -1 || x.Status == status)
                                && (dateStart == null || x.Created >= dateStart)
                                && (dateEnd == null || x.Created <= dateEnd)),
                            sourcing => sourcing.Id,
                            sourcingDetail => sourcingDetail.SourceId,
                            (s, sd) => new SourcingBySourcingDetailResults()
                            {
                                Id = s.Id, 
                                ImagePath1 = sd.ImagePath1,
                                ImagePath2 = sd.ImagePath2,
                                ImagePath3 = sd.ImagePath3,
                                ImagePath4 = sd.ImagePath4,
                                Created = sd.Created,
                                LastUpdate = sd.LastUpdate,
                                Quantity = sd.Quantity,
                                TypeService = s.TypeService,
                                TypeServiceName = s.TypeServiceName,
                                Status = s.Status,
                                Note = sd.Note,
                                Code = s.Code,

                            }).OrderBy(x => x.Created).ThenByDescending(x => x.Id);
            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}