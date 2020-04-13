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
namespace Library.DbContext.Repositories
{
    public class ShopRepository : Repository<Shop>
    {
        public ShopRepository(ProjectXContext context) : base(context)
        {
        }
        public ShopItem GetAll(int id)
        {
            using (var context = new ProjectXContext())
            {
                byte statusOrder = (byte)OrderStatus.Finish;
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "Cms_Shop_SelectDetail";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("id", id));
                cmd.Parameters.Add(new SqlParameter("statusOrder", statusOrder));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpShopDetailItem = ((IObjectContextAdapter)context).ObjectContext.Translate<ShopItem>(reader).ToList().FirstOrDefault();
                    reader.Close();
                    return tmpShopDetailItem;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }
    }
}
