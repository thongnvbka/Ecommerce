using Library.DbContext.Entities;
using Library.UnitOfWork;
using Common.Items;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Library.DbContext.Repositories
{
    public class WarehouseRepository : Repository<Warehouse>
    {
        public WarehouseRepository(ProjectXContext context) : base(context)
        {
        }
        
    }
}

