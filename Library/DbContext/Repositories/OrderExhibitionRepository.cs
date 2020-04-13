using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using Common.Items;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Collections.Generic;
using Common.Emums;
using System.Globalization;

namespace Library.DbContext.Repositories
{
    public class OrderExhibitionRepository : Repository<Order>
    {
        public OrderExhibitionRepository(ProjectXContext context) : base(context)
        {
        }
        public OrderExhibitionModel GetAllOrderExhibition(PageItem pageInfor, SearchInfor searchInfor)
        {
            using (var context = new ProjectXContext())
            {
                byte orderExchangeType = (byte)OrderExchangeType.Product;
                byte orderExchangeMode = (byte)OrderExchangeMode.Export;
                byte orderExchangeStatus = (byte)OrderExchangeStatus.Approved;
                byte oeOrderType = (byte)OrderExchangeOrderType.Order;
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_order_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("PageIndex", pageInfor.PageIndex == 0 ? 1 : pageInfor.PageIndex));
                cmd.Parameters.Add(new SqlParameter("PageSize", pageInfor.PageSize == 0 ? 25 : pageInfor.PageSize));
                cmd.Parameters.Add(new SqlParameter("Keyword", searchInfor.Keyword));
                cmd.Parameters.Add(new SqlParameter("StartDate", searchInfor.StartDate));
                cmd.Parameters.Add(new SqlParameter("FinishDate", searchInfor.FinishDate));
                cmd.Parameters.Add(new SqlParameter("AllTime", searchInfor.AllTime));
                cmd.Parameters.Add(new SqlParameter("Status", searchInfor.Status));
                cmd.Parameters.Add(new SqlParameter("SystemId", searchInfor.SystemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", searchInfor.CustomerId));
                cmd.Parameters.Add(new SqlParameter("OrderType", searchInfor.OrderType));
                cmd.Parameters.Add(new SqlParameter("IsSearch", searchInfor.IsSearch));
                cmd.Parameters.Add(new SqlParameter("StartStatus", searchInfor.StartStatus));
                cmd.Parameters.Add(new SqlParameter("EndStatus", searchInfor.EndStatus));

                cmd.Parameters.Add(new SqlParameter("StartStatus1", searchInfor.StartStatus1));
                cmd.Parameters.Add(new SqlParameter("EndStatus1", searchInfor.EndStatus1));

                cmd.Parameters.Add(new SqlParameter("StartStatus2", searchInfor.StartStatus2));
                cmd.Parameters.Add(new SqlParameter("EndStatus2", searchInfor.EndStatus2));

                cmd.Parameters.Add(new SqlParameter("StartStatus3", searchInfor.StartStatus3));
                cmd.Parameters.Add(new SqlParameter("EndStatus3", searchInfor.EndStatus3));

                cmd.Parameters.Add(new SqlParameter("StartStatus4", searchInfor.StartStatus4));
                cmd.Parameters.Add(new SqlParameter("EndStatus4", searchInfor.EndStatus4));

                cmd.Parameters.Add(new SqlParameter("StartStatus5", searchInfor.StartStatus5));
                cmd.Parameters.Add(new SqlParameter("EndStatus5", searchInfor.EndStatus5));

                cmd.Parameters.Add(new SqlParameter("StartStatus6", searchInfor.StartStatus6));
                cmd.Parameters.Add(new SqlParameter("EndStatus6", searchInfor.EndStatus6));

                cmd.Parameters.Add(new SqlParameter("StartStatus7", searchInfor.StartStatus7));
                cmd.Parameters.Add(new SqlParameter("EndStatus7", searchInfor.EndStatus7));

                cmd.Parameters.Add(new SqlParameter("StartStatus8", searchInfor.StartStatus8));
                cmd.Parameters.Add(new SqlParameter("EndStatus8", searchInfor.EndStatus8));

                cmd.Parameters.Add(new SqlParameter("StartStatus9", searchInfor.StartStatus9));
                cmd.Parameters.Add(new SqlParameter("EndStatus9", searchInfor.EndStatus9));

                cmd.Parameters.Add(new SqlParameter("StartStatus10", searchInfor.StartStatus10));
                cmd.Parameters.Add(new SqlParameter("EndStatus10", searchInfor.EndStatus10));

                cmd.Parameters.Add(new SqlParameter("StartStatus11", searchInfor.StartStatus11));
                cmd.Parameters.Add(new SqlParameter("EndStatus11", searchInfor.EndStatus11));

                cmd.Parameters.Add(new SqlParameter("StartStatus12", searchInfor.StartStatus12));
                cmd.Parameters.Add(new SqlParameter("EndStatus12", searchInfor.EndStatus12));

                cmd.Parameters.Add(new SqlParameter("StartStatus13", searchInfor.StartStatus13));
                cmd.Parameters.Add(new SqlParameter("EndStatus13", searchInfor.EndStatus13));
                cmd.Parameters.Add(new SqlParameter("orderExchangeType", orderExchangeType));
                cmd.Parameters.Add(new SqlParameter("orderExchangeMode", orderExchangeMode));
                cmd.Parameters.Add(new SqlParameter("orderExchangeStatus", orderExchangeStatus));
                cmd.Parameters.Add(new SqlParameter("oeOrderType", oeOrderType));
                SqlParameter outputCount = new SqlParameter("@Count", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputCount);
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderItem>(reader).ToList();
                    reader.NextResult();
                    var tmpStatus = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderStatusItem>(reader).ToList().FirstOrDefault();
                    reader.Close();
                    pageInfor.CurrentPage = pageInfor.PageIndex;
                    pageInfor.Total = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());

                    var model = new OrderExhibitionModel()
                    {
                        Page = pageInfor,
                        Search = searchInfor,
                        ListItems = tmpList,
                        OrderStatusItem = tmpStatus
                    };
                    return model;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }


        public List<OrderAutoItem> SearchAutoComplete(int systemId, int customerId, string keyword, int top, byte orderStatus)
        {
            var tmpList = new List<OrderAutoItem>();
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_order_selectTop";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("SystemId", systemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));
                cmd.Parameters.Add(new SqlParameter("Keyword", keyword));
                cmd.Parameters.Add(new SqlParameter("Top", top));
                cmd.Parameters.Add(new SqlParameter("OrderStatus", orderStatus));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderAutoItem>(reader).ToList();
                    reader.Close();
                    
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
            return tmpList;
        }
        public OrderAutoItem GetOrderIdByOrderCode(int systemId, int customerId, string keyword, int top, byte orderStatus)
        {
            var tmpList = new OrderAutoItem();
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_order_selectByOrderCode";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("SystemId", systemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));
                cmd.Parameters.Add(new SqlParameter("Keyword", keyword));
                cmd.Parameters.Add(new SqlParameter("Top", top));
                cmd.Parameters.Add(new SqlParameter("OrderStatus", orderStatus));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderAutoItem>(reader).ToList().FirstOrDefault();
                    reader.Close();

                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
            return tmpList;
        }

        public OrderDetailCountItem GetOrderDetailCountByOrderId(int orderId)
        {
            var tmpList = new OrderDetailCountItem();
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_oderDetail_SelectCountQuantity";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("orderId", orderId));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderDetailCountItem>(reader).ToList().FirstOrDefault();
                    reader.Close();

                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
            return tmpList;
        }
        public OrderDetailCountItem GetDetailCountByOrderId(int orderId)
        {
            var countProduct = Db.OrderDetails.Where(s => s.OrderId == orderId).Select(s => s.Quantity).DefaultIfEmpty(0).Sum();
            var quantityActually = Db.OrderDetails.Where(s => s.OrderId == orderId).Select(s => s.QuantityActuallyReceived).DefaultIfEmpty(0).Sum();
            var quantityBooked = Db.OrderDetails.Where(s => s.OrderId == orderId).Select(s => s.QuantityBooked).DefaultIfEmpty(0).Sum();

            var tmpList = new OrderDetailCountItem() {
                ProductCount = countProduct,
                QuantityActually = quantityActually == null ? 0 : (int)quantityActually,
                QuantityBooked = quantityBooked == null ? 0 : (int)quantityBooked
            };
            return tmpList;
        }
        #region Get data by linq
        public OrderExhibitionModel GetAllByLinq(PageItem pageInfor, SearchInfor searchInfor)
        {
            byte orderExchangeType = (byte)OrderExchangeType.Product;
            byte orderExchangeMode = (byte)OrderExchangeMode.Export;
            byte orderExchangeStatus = (byte)OrderExchangeStatus.Approved;
            byte oeOrderType = (byte)OrderExchangeOrderType.Order;
            var query = Db.Orders.Where(
                    x => (searchInfor.Status == -1 || (x.Status >= searchInfor.StartStatus && x.Status <= searchInfor.EndStatus))
                    && x.Code.Contains(searchInfor.Keyword)
                    && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.Created >= searchInfor.StartDate)
                    && (searchInfor.FinishDate == null || x.Created <= searchInfor.FinishDate)))
                    && !x.IsDelete
                    && x.CustomerId == searchInfor.CustomerId
                    && x.SystemId == searchInfor.SystemId
                    && x.Type == searchInfor.OrderType
                    && x.Status != 0 
                ).Select(m => new OrderItem()
                {
                    ROW = 1,
                    id = m.Id,
                    code = m.Code,
                    created = m.Created,
                    ImagePath = (Db.OrderDetails.Where(s => s.OrderId == m.Id).FirstOrDefault().Image),
                    ProductCount = (Db.OrderDetails.Where(s => s.OrderId == m.Id).Count()),
                    TotalPrice = m.TotalPrice,
                    TotalExchange = m.Total,
                    //TotalMiss = m.Total - (Db.OrderExchanges.Where(s => s.Type == orderExchangeType && s.Mode == orderExchangeMode && s.Status == orderExchangeStatus && !s.IsDelete && s.OrderId == m.Id && s.OrderType == oeOrderType).Select(s => (decimal)s.TotalPrice).DefaultIfEmpty(0).Sum()),
                    TotalMiss = m.Total - m.TotalPayed,
                    //IsComplain = (Db.Complains.Where(s => s.OrderId == m.Id).FirstOrDefault() != null ? 1 : 0),
                    IsComplain = 0,
                    Status = (byte)m.Status,
                    ShopLink = m.ShopLink
                });
            var queryCount = Db.Orders.Where(
                    x => x.Code.Contains(searchInfor.Keyword)
                    && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.Created >= searchInfor.StartDate)
                    && (searchInfor.FinishDate == null || x.Created <= searchInfor.FinishDate)))
                    && !x.IsDelete
                    && x.CustomerId == searchInfor.CustomerId
                    && x.SystemId == searchInfor.SystemId
                    && x.Type == searchInfor.OrderType
                    && x.Status != 0
                ).Select(m => new OrderItem()
                {
                    Status = (byte)m.Status,
                });
            pageInfor.CurrentPage = pageInfor.PageIndex;
            pageInfor.Total = query.Count();
            var tmpList = query.OrderByDescending(x => new { x.created })
                    .Skip((pageInfor.CurrentPage - 1) * pageInfor.PageSize)
                    .Take(pageInfor.PageSize)
                    .ToList();
        var tmpStatus = new OrderStatusItem()
            {
                dhChoBaoGia = queryCount.Where(m => m.Status >= searchInfor.StartStatus1 && m.Status <= searchInfor.EndStatus1).Count(),
                dhChoDatCoc = queryCount.Where(m => m.Status >= searchInfor.StartStatus2 && m.Status <= searchInfor.EndStatus2).Count(),
                dhChoDatHang = queryCount.Where(m => m.Status >= searchInfor.StartStatus3 && m.Status <= searchInfor.EndStatus3).Count(),
                dhDangDatHang = queryCount.Where(m => m.Status >= searchInfor.StartStatus4 && m.Status <= searchInfor.EndStatus4).Count(),
                dhShopPhatHang = queryCount.Where(m => m.Status >= searchInfor.StartStatus5 && m.Status <= searchInfor.EndStatus5).Count(),
                dhHangTrongKho = queryCount.Where(m => m.Status >= searchInfor.StartStatus6 && m.Status <= searchInfor.StartStatus6).Count(),
                dhDangVanChuyen = queryCount.Where(m => m.Status >= searchInfor.StartStatus7 && m.Status <= searchInfor.StartStatus7).Count(),
                dhChoGiaoHang = queryCount.Where(m => m.Status >= searchInfor.StartStatus8 && m.Status <= searchInfor.StartStatus8).Count(),
                dhDaGiaoHang = queryCount.Where(m => m.Status >= searchInfor.StartStatus9 && m.Status <= searchInfor.StartStatus9).Count(),
                dhHoanThanh = queryCount.Where(m => m.Status >= searchInfor.StartStatus10 && m.Status <= searchInfor.StartStatus10).Count(),
                dhDaHuy = queryCount.Where(m => m.Status >= searchInfor.StartStatus11 && m.Status <= searchInfor.StartStatus11).Count(),
                dhMatHong = queryCount.Where(m => m.Status >= searchInfor.StartStatus12 && m.Status <= searchInfor.StartStatus12).Count()
            };
            var model = new OrderExhibitionModel()
            {
                Page = pageInfor,
                Search = searchInfor,
                ListItems = tmpList,
                OrderStatusItem = tmpStatus
            };
            return model;
        }

        public List<OrderAutoItem> SearchAutoCompleteByLinq(int systemId, int customerId, string keyword, int top, byte orderStatus)
        {
            var tmpList = Db.Orders.Where(
                    x => x.Code.Contains(keyword)
                    && !x.IsDelete
                    && x.CustomerId == customerId
                    && x.SystemId == systemId
                )
                .Select(m => new OrderAutoItemV2()
                {
                    OrderId = m.Id,
                    OrderCode = m.Code,
                    Created = m.Created,
                    ImagePath = (Db.OrderDetails.Where(s => s.OrderId == m.Id).FirstOrDefault().Image),
                    ProductCount = (Db.OrderDetails.Where(s => s.OrderId == m.Id).Count()),
                    TotalPrice = m.TotalPrice,
                    TotalExchange = m.TotalExchange
                })
                .ToList()
                .Select(m => new OrderAutoItem()
                {
                    OrderId = m.OrderId,
                    OrderCode = m.OrderCode,
                    created = m.Created.ToString("dd/MM/yyyy"),
                    ImagePath = m.ImagePath,
                    ProductCount = m.ProductCount,
                    TotalPrice = string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:#,###.000}", m.TotalPrice),
                    TotalExchange = string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:#,###}", m.TotalExchange)
                }).ToList();
            return tmpList;
        }
        #endregion
    }
}
