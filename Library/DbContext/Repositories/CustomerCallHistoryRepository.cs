using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Common.Emums;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class CustomerCallHistoryRepository : Repository<CustomerCallHistory>
    {
        public CustomerCallHistoryRepository(ProjectXContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy ra các cuộc gọi giao hàng cuối cùng cho khách hàng
        /// </summary>
        /// <param name="customerIds">Id khách hàng dạng (";customerId1;customerId2;customerIdN;")</param>
        /// <returns></returns>
        public Task<List<CustomerCallHistory>> GetByCustomerIds(string customerIds)
        {
            return Db.CustomerCallHistorys
                .Where(x => x.IsLast && customerIds.Contains(";" + x.CustomerId + ";") &&
                            x.Mode == (byte) CustomerCallHistoryMode.CallDelivery)
                .ToListAsync();
        }
    }
}
