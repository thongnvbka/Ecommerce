using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels.Complains;
using System.Collections.Generic;
using System.Linq;
namespace Library.DbContext.Repositories
{
    public class ComplainUserRepository : Repository<ComplainUser>
    {
        public ComplainUserRepository(ProjectXContext context) : base(context)
        {
        }
        public int Insert(ComplainUser item)
        {
            int result = 0;
            using (var context = new ProjectXContext())
            {
                if (!string.IsNullOrEmpty(item.Content.Trim()))
                {
                    context.Configuration.ValidateOnSaveEnabled = false;
                    context.ComplainUsers.Add(item);
                    result = context.SaveChanges();
                }
                
            }
            return result;
        }

        public List<ComplainUserComment> GetDetail(long id)
        {
            var result = new List<ComplainUserComment>();
            var left = Db.ComplainUsers.Where(m => m.ComplainId == id && !(bool)m.IsInHouse);
            var right = Db.Users.Where(m => !m.IsDelete);
            var query = left.GroupJoin(right, l => l.UserId, r => r.Id, (l, r) => new { l, r })
                        .SelectMany(
                             o => o.r.DefaultIfEmpty(),
                             (l, r) => new ComplainUserComment()
                             { Id = l.l.Id,
                                 ComplainId = l.l.ComplainId,
                                 UserId = l.l.UserId,
                                 Content = l.l.Content,
                                 AttachFile = l.l.AttachFile,
                                 CreateDate = l.l.CreateDate,
                                 UpdateDate = l.l.UpdateDate,
                                 UserRequestId = l.l.UserRequestId,
                                 UserRequestName = l.l.UserRequestName,
                                 CustomerId = l.l.CustomerId,
                                 CustomerName = l.l.CustomerName,
                                 UserName = l.l.UserName,
                                 IsRead = l.l.IsRead,
                                 IsCare = l.l.IsCare,
                                 UserFullName = r.FullName,
                                 UserOffice = r.Department,
                                 UserPhone = r.Phone
                             }).ToList();
            return query;
        }
        
    }
}
