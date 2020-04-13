using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Common.Emums;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using Library.ViewModels.Items;
using System;
using System.Collections.Generic;

namespace Library.DbContext.Repositories
{
    public class ComplainOrderRepository : Repository<ComplainOrder>
    {
        public ComplainOrderRepository(ProjectXContext context) : base(context)
        {
        }

        public List<ProductDetailItem> GetListOrderDetail(long complainId)
        {
            var tmpList = Db.ComplainOrders.Where(x => x.ComplainId == complainId)
                .Join(Db.OrderDetails.Where(x => !x.IsDelete), complain => complain.OrderDetailId, detail => detail.Id,
                    (order, detail) => new { order = order, detail = detail }).Select(m => new ProductDetailItem()
                    {
                        Id = m.detail.Id,
                        Name = m.detail.Name,
                        Image = m.detail.Image,
                        Quantity = m.detail.Quantity,
                        Price = m.detail.Price,
                        ExchangeRate = m.detail.ExchangeRate,
                        TotalPrice = m.detail.TotalPrice,
                        Link = m.detail.Link,
                        Created = m.detail.Created,
                        LastUpdate = m.detail.LastUpdate,
                        Size = m.detail.Size,
                        Color = m.detail.Color,
                        Note = m.detail.Note,
                        PrivateNote = m.detail.PrivateNote,
                        ComplainNote = m.order.Note,
                        LinkOrder = m.order.LinkOrder ?? 0,
                        Properties = m.detail.Properties
                    }).OrderBy(x=>x.Id).ToList();
            return tmpList;
        }
    }
}
