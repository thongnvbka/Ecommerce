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
    public class NotificationRepository : Repository<Notification>
    {
        public NotificationRepository(ProjectXContext context) : base(context)
        {
        }
        public NotifiOrderModel GetAll(PageItem pageInfor, SearchInfor searchInfor)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiOrder_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("PageIndex", pageInfor.PageIndex == 0 ? 1 : pageInfor.PageIndex));
                cmd.Parameters.Add(new SqlParameter("PageSize", pageInfor.PageSize == 0 ? 25 : pageInfor.PageSize));
                cmd.Parameters.Add(new SqlParameter("SystemId", searchInfor.SystemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", searchInfor.CustomerId));
                
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
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<NotifiOrderItem>(reader).ToList();
                    reader.Close();
                    pageInfor.CurrentPage = pageInfor.PageIndex;
                    pageInfor.Total = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());

                    var model = new NotifiOrderModel()
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
        public NotifiOrderModel GetAllMonth(PageItem pageInfor, SearchInfor searchInfor)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiOrder_selectByMonth";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("PageIndex", pageInfor.PageIndex == 0 ? 1 : pageInfor.PageIndex));
                cmd.Parameters.Add(new SqlParameter("PageSize", pageInfor.PageSize == 0 ? 25 : pageInfor.PageSize));
                cmd.Parameters.Add(new SqlParameter("SystemId", searchInfor.SystemId));
                cmd.Parameters.Add(new SqlParameter("Year", searchInfor.NotiYear));
                cmd.Parameters.Add(new SqlParameter("Month", searchInfor.NotiMonth));
                cmd.Parameters.Add(new SqlParameter("CustomerId", searchInfor.CustomerId));
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
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<NotifiOrderItem>(reader).ToList();
                    reader.Close();
                    pageInfor.CurrentPage = pageInfor.PageIndex;
                    pageInfor.Total = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());

                    var model = new NotifiOrderModel()
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
        public List<NotifiOrderItem> GetTopNew(int systemId, int top, int customerId)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiOrder_selectTopNew";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("Top", top));
                cmd.Parameters.Add(new SqlParameter("SystemId", systemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<NotifiOrderItem>(reader).ToList();
                    reader.Close();
                    return tmpList;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }
        public List<NotifiOrderItem> GetTopNew(int systemId, int top, int customerId, ref int countNotifi)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiOrder_selectTopNotifi";
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
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<NotifiOrderItem>(reader).ToList();
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
        public List<NotifiOrderItem> GetTopNewByLinq(int systemId, int top, int customerId, ref int countNotifi)
        {
            var query = Db.Notifications.Where(m => m.SystemId == systemId && m.CustomerId == customerId)
                                        .OrderByDescending(m => m.CreateDate)
                                        .Select(m => new NotifiOrderItem()
                                        {
                                            Description = m.Description,
                                            CreateDate = m.CreateDate,
                                            OrderId = m.OrderId,
                                            OrderType = m.OrderType,
                                            Title = m.Title,
                                            UserName = m.UserName,
                                            Id = m.Id
                                        }).Take(top).ToList();
            countNotifi = Db.Notifications.Where(m => m.SystemId == systemId && m.CustomerId == customerId && !m.IsRead).Select(m => m.Id).Count();
            return query;
        }

        public List<NotifiOrderItem> GetTopNewByLinq(int systemId, int top, int customerId, int orderType, ref int countNotifi)
        {
            var query = Db.Notifications.Where(m => m.SystemId == systemId && m.CustomerId == customerId && m.OrderType == orderType)
                                        .OrderByDescending(m => m.CreateDate)
                                        .Select(m => new NotifiOrderItem()
                                        {
                                            Description = m.Description,
                                            CreateDate = m.CreateDate,
                                            OrderId = m.OrderId,
                                            OrderType = m.OrderType,
                                            Title = m.Title,
                                            UserName = m.UserName,
                                            Id = m.Id
                                        }).Take(top).ToList();
            countNotifi = Db.Notifications.Where(m => m.SystemId == systemId && m.CustomerId == customerId && !m.IsRead).Select(m => m.Id).Count();
            return query;
        }

        public void UpdateTopNew(int systemId, int customerId)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiOrder_updateTopNotifi";
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
            var obj = Db.Notifications.FirstOrDefault(m => m.SystemId == systemId && m.CustomerId == customerId);
            if (obj != null)
            {
                obj.IsRead = true;
                Db.SaveChanges();
            }
        }
        public List<DropdownItem> GetTopMonth(int systemId, int customerId)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "notifi_notifiOrder_selectGroupMonth";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("SystemId", systemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));
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
