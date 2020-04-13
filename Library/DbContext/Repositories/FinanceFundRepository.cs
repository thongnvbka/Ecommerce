using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.DbContext.Results;
namespace Library.DbContext.Repositories
{
    public class FinanceFundRepository : Repository<FinanceFund>
    {
        public FinanceFundRepository(ProjectXContext context) : base(context)
        {
        }
        public DbSet<FinanceFund> GetFinanceFundList()
        {
            return Db.FinanceFunds;
        }
        public IEnumerable<UserPosition> List_Position()
        {
            var x = Db.UserPositions.GroupBy(p => p.OfficeId, p => p.TitleId) as IEnumerable<UserPosition>;
            return x;
        }

        //public int Update(FinanceFund item)
        //{
        //    int result = 0;
        //    using (var context = new ProjectXContext())
        //    {
        //        context.Configuration.ValidateOnSaveEnabled = false;
        //        var tmpObj = context.FinanceFunds.Find(item.FundId);
        //        tmpObj.FundName = item.FundName;
        //        tmpObj.Status = item.Status;
        //        tmpObj.Description = item.Description;
        //        tmpObj.UpdateDate = DateTime.Now;
        //        result = context.SaveChanges();
        //    }
        //    return result;
        //}
        //public int Delete(long id)
        //{
        //    int result = 0;
        //    using (var context = new ProjectXContext())
        //    {
        //        var tmpObj = context.FinanceFunds.Find(id);
        //        context.FinanceFunds.Remove(tmpObj);
        //        result = context.SaveChanges();
        //    }
        //    return result;
        //}
        //public int DeleteAll(string arrId)
        //{
        //    int result = 0;
        //    using (var context = new ProjectXContext())
        //    {
        //        string[] arr = arrId.Split('|');
        //        foreach (var s in arr)
        //        {
        //            if (s != "" && !string.IsNullOrEmpty(s))
        //            {
        //                long tmpId = 0;
        //                long.TryParse(s, out tmpId);
        //                var tmp = context.FinanceFunds.Find(tmpId);
        //                if (tmp != null)
        //                {
        //                    context.FinanceFunds.Remove(tmp);
        //                }
        //            }
        //        }
        //        result = context.SaveChanges();
        //    }
        //    return result;
        //}
    }
}
