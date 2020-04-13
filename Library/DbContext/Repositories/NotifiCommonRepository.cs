using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels.Notifi;
using Common.Items;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Library.ViewModels.Items;
using System.Collections.Generic;

namespace Library.DbContext.Repositories
{
    public class NotifiCommonRepository : Repository<NotifiCommon>
    {
        public NotifiCommonRepository(ProjectXContext context) : base(context)
        {
        }
        public NotifiCommonModel GetAll(PageItem pageInfor, SearchInfor searchInfor)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiCommon_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("PageIndex", pageInfor.PageIndex == 0 ? 1 : pageInfor.PageIndex));
                cmd.Parameters.Add(new SqlParameter("PageSize", pageInfor.PageSize == 0 ? 25 : pageInfor.PageSize));
                cmd.Parameters.Add(new SqlParameter("SystemId", searchInfor.SystemId));
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
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<NotifiCommonItem>(reader).ToList();
                    reader.Close();
                    pageInfor.CurrentPage = pageInfor.PageIndex;
                    pageInfor.Total = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());

                    var model = new NotifiCommonModel()
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
        public NotifiCommonModel GetAllMonth(PageItem pageInfor, SearchInfor searchInfor)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiCommon_selectByMonth";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("PageIndex", pageInfor.PageIndex == 0 ? 1 : pageInfor.PageIndex));
                cmd.Parameters.Add(new SqlParameter("PageSize", pageInfor.PageSize == 0 ? 25 : pageInfor.PageSize));
                cmd.Parameters.Add(new SqlParameter("SystemId", searchInfor.SystemId));
                cmd.Parameters.Add(new SqlParameter("Year", searchInfor.NotiYear));
                cmd.Parameters.Add(new SqlParameter("Month", searchInfor.NotiMonth));
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
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<NotifiCommonItem>(reader).ToList();
                    reader.Close();
                    pageInfor.CurrentPage = pageInfor.PageIndex;
                    pageInfor.Total = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());

                    var model = new NotifiCommonModel()
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
        public List<NotifiCommonItem> GetTopNew(int systemId, int top)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiCommon_selectTopNew";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("Top", top));
                cmd.Parameters.Add(new SqlParameter("SystemId", systemId));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<NotifiCommonItem>(reader).ToList();
                    reader.Close();
                    return tmpList;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }
        public List<NotifiCommonItem> GetTopNew(int systemId, int top, int customerId, ref int countNotifi)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiCommon_selectTopNotifi";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("Top", top));
                cmd.Parameters.Add(new SqlParameter("SystemId", systemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));
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
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<NotifiCommonItem>(reader).ToList();
                    reader.Close();
                    countNotifi = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());
                    return tmpList;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }

        public List<NotifiCommonItem> GetTopNewByLinq(int systemId, int top, int customerId, ref int countNotifi)
        {
            var query = Db.NotifiCommons.Where(m => m.SystemId == systemId).OrderByDescending(m => m.CreateDate)
                                        .Select(x => new NotifiCommonItem()
                                        {
                                            Description = x.Description,
                                            CreateDate = x.CreateDate,
                                            Title = x.Title,
                                            Url = x.Url,
                                            UserName = x.UserName
                                        }).Take(top).ToList();
            countNotifi = Db.NotifiCustomers.Where(m => m.SystemId == systemId && m.CustomerId == customerId && !m.IsRead).Select(m => m.Id).Count();
            return query;
        }
        public void UpdateTopNew(int systemId, int customerId)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiCommon_updateTopNotifi";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("SystemId", systemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }
        public void UpdateTopNewByLinq(int systemId, int customerId)
        {
            var notiCus = Db.NotifiCustomers.FirstOrDefault(m => m.SystemId == systemId && m.CustomerId == customerId);
            if (notiCus != null)
            {
                notiCus.IsRead = true;
                Db.SaveChanges();
            }
        }
        public NotifiCommonItem GetDetail(int systemId, string url, int customerId)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiCommon_selectByUrl";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("Url", url));
                cmd.Parameters.Add(new SqlParameter("SystemId", systemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<NotifiCommonItem>(reader).ToList().FirstOrDefault();
                    reader.Close();
                    return tmpList;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }
        public List<DropdownItem> GetTopMonth(int systemId)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiCommon_selectGroupMonth";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("SystemId", systemId));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<DropdownItem>(reader).ToList();
                    reader.Close();
                    return tmpList;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }
    }
}
