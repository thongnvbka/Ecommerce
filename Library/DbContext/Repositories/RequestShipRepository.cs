using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Common.Emums;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using Library.ViewModels.Items;
using System;
using Library.ViewModels.Ship;


namespace Library.DbContext.Repositories
{
    public class RequestShipRepository : Repository<RequestShip>
    {
        public RequestShipRepository(ProjectXContext context) : base(context)
        {
        }
        public RequestShipDetailModel GetById(long id)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "cms_requestShip_selectById";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("id", id));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpDetailItem = ((IObjectContextAdapter)context).ObjectContext.Translate<RequestShip>(reader).ToList().FirstOrDefault();
                    reader.NextResult();
                    var tmpPackage = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderPackageItem>(reader).ToList();
                    reader.Close();
                    var model = new RequestShipDetailModel()
                    {
                        RequestShipDetail = tmpDetailItem,
                        ListPackage = tmpPackage
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
