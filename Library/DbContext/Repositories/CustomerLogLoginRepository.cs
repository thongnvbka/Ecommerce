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
    public class CustomerLogLoginRepository : Repository<CustomerLogLogin>
    {
        public CustomerLogLoginRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> Insert(string userName, string fullName, string ip, string sessionId, string os, string browser,
            int? version, int systemId, string systemName)
        {
            var model = new CustomerLogLogin()
            {
                UserName = userName,
                FullName = fullName,
                Ip = ip,
                UnsignName = MyCommon.Ucs2Convert(fullName),
                LoginTime = DateTime.Now,
                Os = os,
                Browser = browser,
                Version = version?.ToString() ?? "",
                Token = sessionId,
                SystemId = systemId,
                SystemName = systemName
            };

            Add(model);

            return await SaveAsync();
        }

        public async Task<int> UpdateLogoutTime(string userName, string sesionId, DateTime logoutTime)
        {
            var logLogin = await Db.CustomerLogLogins.SingleOrDefaultAsync(x => x.UserName.Equals(userName) && x.Token.Equals(sesionId));

            if (logLogin == null) return -1;

            logLogin.LogoutTime = logoutTime;

            return await Db.SaveChangesAsync();
        }

        public Task<List<CustomerLogLogin>> Search(string userName, string fullName, int systemId, DateTime? startDate, DateTime? endDate,
            int pageIndex, int recordPerPage, out int totalRecord)
        {
            long total;

            var items = FindAsync(out total, x =>
                    (userName == "" || x.UserName == userName) &&
                    (!startDate.HasValue || x.LoginTime >= startDate.Value) &&
                    (!endDate.HasValue || x.LoginTime <= endDate.Value)
                    && (systemId == -1 || x.SystemId == systemId),
                logins => logins.OrderByDescending(x => x.LoginTime), pageIndex, recordPerPage);

            totalRecord = (int)total;

            return items;
        }
    }
}
