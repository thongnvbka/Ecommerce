using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using Common.Items;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Collections.Generic;
using Library.ViewModels.Report;

namespace Library.DbContext.Repositories
{
    public class ReportBusinessRepository : Repository<Order>
    {
        public ReportBusinessRepository(ProjectXContext context) : base(context)
        {
        }
        public ReportBusinessModel GetAll(PageItem pageInfor, SearchInfor searchInfor)
        {
            using (var context = new ProjectXContext())
            {
                //byte tmpPurchase = (byte)Common.Emums.OrderServices.Order;
                byte tmpTally = (byte)Common.Emums.OrderServices.Audit;
                byte tmpBaled = (byte)Common.Emums.OrderServices.Packing;
                byte tmpStatus = (byte)Common.Emums.OrderStatus.Finish;
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "report_order_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("PageIndex", pageInfor.PageIndex == 0 ? 1 : pageInfor.PageIndex));
                cmd.Parameters.Add(new SqlParameter("PageSize", pageInfor.PageSize == 0 ? 25 : pageInfor.PageSize));
                cmd.Parameters.Add(new SqlParameter("OrderCode", searchInfor.Keyword));
                cmd.Parameters.Add(new SqlParameter("StartDate", searchInfor.StartDate));
                cmd.Parameters.Add(new SqlParameter("FinishDate", searchInfor.FinishDate));
                cmd.Parameters.Add(new SqlParameter("AllTime", searchInfor.AllTime));
                cmd.Parameters.Add(new SqlParameter("CustomerId", searchInfor.CustomerId));
                cmd.Parameters.Add(new SqlParameter("UserId", searchInfor.UserId));
                cmd.Parameters.Add(new SqlParameter("OfficeId", searchInfor.OfficeId));
                cmd.Parameters.Add(new SqlParameter("Status", tmpStatus));
                cmd.Parameters.Add(new SqlParameter("SystemId", searchInfor.SystemId));
                cmd.Parameters.Add(new SqlParameter("IsSearch", searchInfor.IsSearch));

                //cmd.Parameters.Add(new SqlParameter("ServicePurchase", tmpPurchase));
                cmd.Parameters.Add(new SqlParameter("ServiceTally", tmpTally));
                cmd.Parameters.Add(new SqlParameter("ServiceBaled", tmpBaled));

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
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<ReportBusinessItem>(reader).ToList();
                    reader.Close();
                    pageInfor.CurrentPage = pageInfor.PageIndex;
                    pageInfor.Total = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());

                    var model = new ReportBusinessModel()
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
    }
}
