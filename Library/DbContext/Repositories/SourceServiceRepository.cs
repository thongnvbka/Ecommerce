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

namespace Library.DbContext.Repositories
{
    public class SourceServiceRepository : Repository<SourceService>
    {
        public SourceServiceRepository(ProjectXContext context) : base(context)
        {
        }
        public SourceServiceItem GetSourceService(int customerId, int systemId)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_sourceService_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("customerId", customerId));
                cmd.Parameters.Add(new SqlParameter("systemId", systemId));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpItem = ((IObjectContextAdapter)context).ObjectContext.Translate<SourceServiceItem>(reader).ToList().FirstOrDefault();
                    
                    reader.Close();
                    return tmpItem;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }

        public decimal UpdateBalanceAvalible(int customerId, int sourceServiceId)
        {
            var result = 0M;
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_sourceService_UpdateBalanceAvalible";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("customerId", customerId));
                cmd.Parameters.Add(new SqlParameter("sourceServiceId", sourceServiceId));
                SqlParameter outputCount = new SqlParameter("@error", SqlDbType.Decimal)
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
                    result = cmd.ExecuteNonQuery();
                    result = decimal.Parse(cmd.Parameters["@error"].Value == null ? "0" : cmd.Parameters["@error"].Value.ToString());
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
            return result;
        }

    }
}
