using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OfficeRepository : Repository<Office>
    {
        public OfficeRepository(ProjectXContext context) : base(context)
        {
        }

        public List<T> GetAll<T>(Expression<Func<Office, T>> projector)
        {
            return Db.Offices.Where(o => !o.IsDelete && o.Status == 1).OrderByDescending(c => c.Type).Select(projector).ToList();
        }

        public List<T> GetBySpec<T>(Expression<Func<Office, T>> projector, Expression<Func<Office, bool>> spec)
        {
            return Db.Offices.Where(spec).Select(projector).ToList();
        }

        public bool CheckOfficeType(int officeId, byte type)
        {
            var office = Db.Offices.FirstOrDefault(x => x.Id == officeId && !x.IsDelete && x.Type == type);
            return office != null;
        }
    }
}
