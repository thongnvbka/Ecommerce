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
    public partial class LogLoginRepository : Repository<LogLogin>
    {
        public LogLoginRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> Insert(string userName, string fullName, string ip, string sessionId, string os, string browser,
            int? version)
        {
            var model = new LogLogin()
            {
                UserName = userName,
                FullName = fullName,
                Ip =  ip,
                UnsignName = MyCommon.Ucs2Convert(fullName),
                LoginTime = DateTime.Now,
                Os = os,
                Browser = browser,
                Version = version?.ToString() ?? "",
                Token = sessionId
            };

            Add(model);

            return await SaveAsync();
        }

        public async Task<int> UpdateLogoutTime(string userName, string sesionId, DateTime logoutTime)
        {
            var logLogin = await Db.LogLogins.SingleOrDefaultAsync(x => x.UserName.Equals(userName) && x.Token.Equals(sesionId));

            if (logLogin == null) return -1;

            logLogin.LogoutTime = logoutTime;

            return await Db.SaveChangesAsync();
        }

        public Task<List<LogLogin>> Search(string userName, string fullName, DateTime? startDate, DateTime? endDate,
            int pageIndex, int recordPerPage, out int totalRecord)
        {
            long total;

            var items = FindAsync(out total, x =>
                    (userName == "" || x.UserName == userName) &&
                    (!startDate.HasValue || x.LoginTime >= startDate.Value) &&
                    (!endDate.HasValue || x.LoginTime <= endDate.Value),
                logins => logins.OrderByDescending(x => x.LoginTime), pageIndex, recordPerPage);

            totalRecord = (int)total;

            return items;
        }
    }
}
