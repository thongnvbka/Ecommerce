using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Library.DbContext;

namespace Library.UnitOfWork
{
    public class Repository<TEntity> where TEntity : class
    {
        protected readonly ProjectXContext Db;

        protected readonly DbSet<TEntity> DbSet;

        public Repository(ProjectXContext dbContext)
        {
            Db = dbContext;
            DbSet = Db.Set<TEntity>();
        }

        public DbSet<TEntity> Entities
        {
            get { return DbSet; }
        }

        public TEntity Find(object id)
        {
            return DbSet.Find(id);
        }

        public TEntity Find(params object[] ids)
        {
            return DbSet.Find(ids);
        }

        public Task<TEntity> FindAsync(object id)
        {
            return DbSet.FindAsync(id);
        }

        public Task<TEntity> FindAsync(params object[] ids)
        {
            return DbSet.FindAsync(ids);
        }

        public TEntity Single()
        {
            return DbSet.Single();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Single(predicate);
        }

        public Task<TEntity> SingleAsync()
        {
            return DbSet.SingleAsync();
        }

        public Task<TEntity> SingleAsync(CancellationToken cancellationToken)
        {
            return DbSet.SingleAsync(cancellationToken);
        }

        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.SingleAsync(predicate);
        }

        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            return DbSet.SingleAsync(predicate, cancellationToken);
        }

        public TEntity SingleAsNoTracking()
        {
            return DbSet.AsNoTracking().Single();
        }

        public TEntity SingleAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().Single(predicate);
        }

        public Task<TEntity> SingleAsNoTrackingAsync()
        {
            return DbSet.AsNoTracking().SingleAsync();
        }

        public Task<TEntity> SingleAsNoTrackingAsync(CancellationToken cancellationToken)
        {
            return DbSet.AsNoTracking().SingleAsync(cancellationToken);
        }

        public Task<TEntity> SingleAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().SingleAsync(predicate);
        }

        public Task<TEntity> SingleAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            return DbSet.AsNoTracking().SingleAsync(predicate, cancellationToken);
        }

        public TEntity SingleOrDefault()
        {
            return DbSet.SingleOrDefault();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.SingleOrDefault(predicate);
        }

        public Task<TEntity> SingleOrDefaultAsync()
        {
            return DbSet.SingleOrDefaultAsync();
        }

        public Task<TEntity> SingleOrDefaultAsync(CancellationToken cancellationToken)
        {
            return DbSet.SingleOrDefaultAsync(cancellationToken);
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.SingleOrDefaultAsync(predicate);
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            return DbSet.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public TEntity SingleOrDefaultAsNoTracking()
        {
            return DbSet.AsNoTracking().SingleOrDefault();
        }

        public TEntity SingleOrDefaultAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().SingleOrDefault(predicate);
        }

        public Task<TEntity> SingleOrDefaultAsNoTrackingAsync()
        {
            return DbSet.AsNoTracking().SingleOrDefaultAsync();
        }

        public Task<TEntity> SingleOrDefaultAsNoTrackingAsync(CancellationToken cancellationToken)
        {
            return DbSet.AsNoTracking().SingleOrDefaultAsync(cancellationToken);
        }

        public Task<TEntity> SingleOrDefaultAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        public Task<TEntity> SingleOrDefaultAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            return DbSet.AsNoTracking().SingleOrDefaultAsync(predicate, cancellationToken);
        }


        public bool All(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.All(predicate);
        }

        public Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AllAsync(predicate);
        }

        public Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return DbSet.AllAsync(predicate, cancellationToken);
        }

        public bool Any()
        {
            return DbSet.Any();
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Any(predicate);
        }

        public Task<bool> AnyAsync()
        {
            return DbSet.AnyAsync();
        }

        public Task<bool> AnyAsync(CancellationToken cancellationToken)
        {
            return DbSet.AnyAsync(cancellationToken);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AnyAsync(predicate);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return DbSet.AnyAsync(predicate, cancellationToken);
        }

        public int Count()
        {
            return DbSet.Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Count(predicate);
        }

        public Task<int> CountAsync()
        {
            return DbSet.CountAsync();
        }

        public Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return DbSet.CountAsync(cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.CountAsync(predicate);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return DbSet.CountAsync(predicate, cancellationToken);
        }

        public long LongCount()
        {
            return DbSet.LongCount();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.LongCount(predicate);
        }

        public Task<long> LongCountAsync()
        {
            return DbSet.LongCountAsync();
        }

        public Task<long> LongCountAsync(CancellationToken cancellationToken)
        {
            return DbSet.LongCountAsync(cancellationToken);
        }

        public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.LongCountAsync(predicate);
        }

        public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return DbSet.LongCountAsync(predicate, cancellationToken);
        }

        public bool Contains(TEntity value)
        {
            return DbSet.Contains(value);
        }

        public bool Contains(TEntity value, IEqualityComparer<TEntity> comparer)
        {
            return DbSet.Contains(value, comparer);
        }

        public Task<bool> ContainsAsync(TEntity value)
        {
            return DbSet.ContainsAsync(value);
        }

        public Task<bool> ContainsAsync(TEntity value, CancellationToken cancellationToken)
        {
            return DbSet.ContainsAsync(value, cancellationToken);
        }

        public TEntity First()
        {
            return DbSet.First();
        }

        public TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.First(predicate);
        }

        public TEntity FirstAsNoTracking()
        {
            return DbSet.AsNoTracking().First();
        }

        public TEntity FirstAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().First(predicate);
        }

        public Task<TEntity> FirstAsync()
        {
            return DbSet.FirstAsync();
        }

        public Task<TEntity> FirstAsync(CancellationToken cancellationToken)
        {
            return DbSet.FirstAsync(cancellationToken);
        }

        public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.FirstAsync(predicate);
        }

        public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            return DbSet.FirstAsync(predicate, cancellationToken);
        }

        public Task<TEntity> FirstAsNoTrackingAsync()
        {
            return DbSet.AsNoTracking().FirstAsync();
        }

        public Task<TEntity> FirstAsNoTrackingAsync(CancellationToken cancellationToken)
        {
            return DbSet.AsNoTracking().FirstAsync(cancellationToken);
        }

        public Task<TEntity> FirstAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().FirstAsync(predicate);
        }

        public Task<TEntity> FirstAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            return DbSet.AsNoTracking().FirstAsync(predicate, cancellationToken);
        }

        public TEntity Last()
        {
            return DbSet.Last();
        }

        public TEntity Last(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Last(predicate);
        }

        public TEntity LastAsNoTracking()
        {
            return DbSet.AsNoTracking().Last();
        }

        public TEntity LastAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().Last(predicate);
        }

        public TEntity FirstOrDefault()
        {
            return DbSet.FirstOrDefault();
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public TEntity FirstOrDefaultAsNoTracking()
        {
            return DbSet.AsNoTracking().FirstOrDefault();
        }

        public TEntity FirstOrDefaultAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().FirstOrDefault(predicate);
        }

        public Task<TEntity> FirstOrDefaultAsync()
        {
            return DbSet.FirstOrDefaultAsync();
        }

        public Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken)
        {
            return DbSet.FirstOrDefaultAsync(cancellationToken);
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.FirstOrDefaultAsync(predicate);
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            return DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public Task<TEntity> FirstOrDefaultAsNoTrackingAsync()
        {
            return DbSet.AsNoTracking().FirstOrDefaultAsync();
        }

        public Task<TEntity> FirstOrDefaultAsNoTrackingAsync(CancellationToken cancellationToken)
        {
            return DbSet.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }

        public Task<TEntity> FirstOrDefaultAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public Task<TEntity> FirstOrDefaultAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            return DbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public TEntity LastOrDefault()
        {
            return DbSet.LastOrDefault();
        }

        public TEntity LastOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.LastOrDefault(predicate);
        }

        public TEntity LastOrDefaultAsNoTracking()
        {
            return DbSet.AsNoTracking().LastOrDefault();
        }

        public TEntity LastOrDefaultAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().LastOrDefault(predicate);
        }

        //public IEnumerable<TEntity> GetAll(Func<TEntity, bool> predicate)
        //{
        //    return DbSet.ToList();
        //}

        //public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        //{
        //    return DbSet.Where(predicate);
        //}

        //public IEnumerable<TEntity> Where(Expression<Func<TEntity, int, bool>> predicate)
        //{
        //    return DbSet.Where(predicate).ToList();
        //}

        public IEnumerable<TEntity> Take(int count)
        {
            return DbSet.Take(count).ToList();
        }

        public Task<List<TEntity>> TakeAsync(int count)
        {
            return DbSet.Take(count).ToListAsync();
        }

        public IEnumerable<TEntity> Take(Expression<Func<int>> countAccessor)
        {
            return DbSet.Take(countAccessor).ToList();
        }

        public Task<List<TEntity>> TakeAsync(Expression<Func<int>> countAccessor)
        {
            return DbSet.Take(countAccessor).ToListAsync();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate).ToList();
        }

        public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate).ToListAsync();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter = null,
    // ReSharper disable once MethodOverloadWithOptionalParameter
    Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query.ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter = null,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> paging = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (paging != null)
            {
                query = paging(query);
            }

            return query.ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter = null,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> paging = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (paging != null)
            {
                query = paging(query);
            }

            return query.ToListAsync();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter = null,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null,
            int currentPage = 1, int pageSize = 20)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy == null) return query.ToList();

            query = orderBy(query);

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 20;
            }

            return query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter = null,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null,
            int currentPage = 1, int pageSize = 20)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy == null) return query.ToListAsync();

            query = orderBy(query);

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 20;
            }

            return query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public IEnumerable<TEntity> Find(out long totalRecord, Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null, int currentPage = 1,
            int pageSize = 20)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            totalRecord = query.LongCount();

            if (orderBy == null) return query.ToList();

            query = orderBy(query);

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 20;
            }

            return query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public Task<List<TEntity>> FindAsync(out long totalRecord, Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null, int currentPage = 1,
            int pageSize = 20)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            totalRecord = query.LongCount();

            if (orderBy == null) return query.ToListAsync();

            query = orderBy(query);

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 20;
            }

            return query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public IEnumerable<TEntity> TakeAsNoTracking(int count)
        {
            return DbSet.AsNoTracking().Take(count).ToList();
        }

        public IEnumerable<TEntity> TakeAsNoTracking(Expression<Func<int>> countAccessor)
        {
            return DbSet.AsNoTracking().Take(countAccessor).ToList();
        }

        public IEnumerable<TEntity> FindAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().Where(predicate).ToList();
        }

        public Task<List<TEntity>> FindAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public IEnumerable<TEntity> FindAsNoTracking(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> paging = null)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (paging != null)
            {
                query = paging(query);
            }

            return query.ToList();
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public Task<List<TEntity>> FindAsNoTrackingAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> paging = null)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (paging != null)
            {
                query = paging(query);
            }

            return query.ToListAsync();
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public IEnumerable<TEntity> FindAsNoTracking(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null, int currentPage = 1, int pageSize = 20)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy == null) return query.ToList();

            query = orderBy(query);

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 20;
            }

            return query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public Task<List<TEntity>> FindAsNoTrackingAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null, int currentPage = 1, int pageSize = 20)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy == null) return query.ToListAsync();

            query = orderBy(query);

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 20;
            }

            return query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public Task<List<TEntity>> FindAsNoTrackingAsync(out long totalRecord, Expression<Func<TEntity, bool>> filter = null,
    Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null, int currentPage = 1, int pageSize = 20)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            totalRecord = query.LongCount();

            if (orderBy == null) return query.ToListAsync();

            query = orderBy(query);

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 20;
            }

            return query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        }


        public List<TEntity> FindAsNoTracking(out long totalRecord, Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null, int currentPage = 1, int pageSize = 20)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            totalRecord = query.LongCount();

            if (orderBy == null) return query.ToList();

            query = orderBy(query);

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 20;
            }

            return query.Skip((currentPage - 1)*pageSize).Take(pageSize).ToList();
        }


        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            DbSet.Attach(entity);
            Db.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(TEntity entity)
        {
            if (Db.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);

            DbSet.Remove(entity);
        }

        public void Remove(object id)
        {
            var entity = Find(id);
            Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public int Save()
        {
            return Db.SaveChanges();
        }
        public int SaveNoCheck()
        {
            Db.Configuration.ValidateOnSaveEnabled = false;
            return Db.SaveChanges();
        }
        public Task<int> SaveAsync()
        {
            return Db.SaveChangesAsync();
        }
        public Task<int> SaveAsyncNoCheck()
        {
            Db.Configuration.ValidateOnSaveEnabled = false;
            return Db.SaveChangesAsync();
        }
        
    }
}
