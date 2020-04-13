using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderRefundDetailRepository : Repository<OrderRefundDetail>
    {
        public OrderRefundDetailRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<OrderRefundDetailResult>> GetByRefundId(int id)
        {
            return Db.OrderRefundDetails.Where(x => x.IsDelete == false && x.OrderRefundId == id)
                .Join(Db.OrderDetailCountings, r => r.OrderDetailCountingId, d => d.Id,
                    (r, d) => new OrderRefundDetailResult()
                    {
                        Id = d.Id,
                        OrderId = d.OrderId,
                        OrderCode = d.OrderCode,
                        OrderType = d.OrderType,
                        WebsiteName = d.WebsiteName,
                        ShopId = d.ShopId,
                        ShopName = d.ShopName,
                        ShopLink = d.ShopLink,
                        WarehouseId = d.WarehouseId,
                        WarehouseName = d.WarehouseName,
                        CustomerId = d.CustomerId,
                        CustomerName = d.CustomerName,
                        CustomerEmail = d.CustomerEmail,
                        CustomerPhone = d.CustomerPhone,
                        CustomerAddress = d.CustomerAddress,
                        OrderDetailId = d.OrderDetailId,
                        Name = d.Name,
                        Image = d.Image,
                        Link = d.Link,
                        Quantity = d.Quantity,
                        Properties = d.Properties,
                        ProductNo = d.ProductNo,
                        BeginAmount = d.BeginAmount,
                        Price = d.Price,
                        ExchangePrice = d.ExchangePrice,
                        ExchangeRate = d.ExchangeRate,
                        TotalPrice = d.TotalPrice,
                        TotalExchange = d.TotalExchange,
                        UserId = d.UserId,
                        UserFullName = d.UserFullName,
                        OfficeId = d.OfficeId,
                        OfficeName = d.OfficeName,
                        OfficeIdPath = d.OfficeIdPath,
                        QuantityLose = d.QuantityLose,
                        Mode = d.Mode,
                        Status = d.Status,
                        NotePrivate = d.NotePrivate,
                        TotalPriceLose = d.TotalPriceLose,
                        TotalExchangeLose = d.TotalExchangeLose,
                        TotalPriceShop = d.TotalPriceShop,
                        TotalExchangeShop = d.TotalExchangeShop,
                        TotalPriceCustomer = d.TotalPriceCustomer,
                        CommentNo = d.CommentNo,
                        Note = r.Note,
                        Created = r.Created,
                        Updated = r.Updated,
                        OrderRefundDetailId = r.Id,
                        OrderRefundId = r.OrderRefundId,
                        OrderDetailCountingId = r.OrderDetailCountingId,
                    }).ToListAsync();
        }
    }
}
