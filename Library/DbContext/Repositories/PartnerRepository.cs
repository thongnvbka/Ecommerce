using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Common.Helper;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public partial class PartnerRepository : Repository<Partner>
    {
        public PartnerRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
    }
}
