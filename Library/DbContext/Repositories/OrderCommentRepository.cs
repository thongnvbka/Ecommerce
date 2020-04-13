using System;
using System.Linq;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderCommentRepository : Repository<OrderComment>
    {
        public OrderCommentRepository(ProjectXContext context) : base(context)
        {
        }
        public int Insert(OrderComment item)
        {
            int result = 0;
            using (var context = new ProjectXContext())
            {
                if (!string.IsNullOrEmpty(item.Description.Trim()))
                {
                    int groupId;
                    var check = CheckGroup(item.OrderId, (int)item.OrderType, out groupId);
                    if (check)
                    {
                        item.GroupId = groupId;
                    }

                    context.Configuration.ValidateOnSaveEnabled = false;
                    context.OrderComments.Add(item);
                    result = context.SaveChanges();

                    if (!check)
                    {
                        item.GroupId = (int)item.Id;
                        context.SaveChanges();
                    }
                }
            }
            return result;
        }

        private bool CheckGroup(int orderId, int orderType, out int groupId)
        {
            var timeOld = DateTime.Now.AddMinutes(-2);
            using (var context = new ProjectXContext())
            {
                context.Configuration.ValidateOnSaveEnabled = false;
                var chat = context.OrderComments.Where(
                            x => x.OrderId == orderId && x.OrderType == orderType && x.CreateDate >= timeOld)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();
                groupId = chat?.GroupId ?? 0;
                return chat?.CustomerId != null;
            }
        }
    }
}
