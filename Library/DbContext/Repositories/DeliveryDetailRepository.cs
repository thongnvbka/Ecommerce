using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class DeliveryDetailRepository : Repository<DeliveryDetail>
    {
        public DeliveryDetailRepository(ProjectXContext context) : base(context)
        {
        }

        public DeliveryDetail SingleOrDefaultAsNoTracking(int packageId)
        {
            return Db.Delivery.Where(x => x.IsDelete == false)
                .Join(Db.DeliveryDetails.Where(x => x.IsDelete == false), d => d.Id, dd => dd.DeliveryId,
                    (delivery, detail) => detail)
                .AsNoTracking()
                .SingleOrDefault(x => x.PackageId == packageId);
        }
    }
}

