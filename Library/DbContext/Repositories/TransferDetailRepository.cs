using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class TransferDetailRepository : Repository<TransferDetail>
    {
        public TransferDetailRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<OrderPackage>> GetByTransferId(int transferId)
        {
            return Db.Transfers.Where(x => x.IsDelete == false && x.Id == transferId)
                .Join(Db.TransferDetails.Where(x => x.IsDelete == false), t => t.Id, d => d.TransferId,
                    (t, d) => new {t, d})
                .Join(Db.OrderPackages.Where(x => x.IsDelete == false), arg => arg.d.PackageId, op => op.Id,
                    (arg, p) => p)
                .Distinct()
                .ToListAsync();
        }
    }
}