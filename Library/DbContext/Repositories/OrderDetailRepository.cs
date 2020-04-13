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
    public class OrderDetailRepository : Repository<OrderDetail>
    {
        public OrderDetailRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<bool> CheckCustomerHasUpdateProductShoppingCart(int productId, int customerId, OrderStatus orderStatus)
        {
            var status = (byte) orderStatus;
            return Db.Orders.Where(x => x.CustomerId == customerId && x.Status == status && !x.IsDelete)
                .Join(Db.OrderDetails.Where(x => x.Id == productId && !x.IsDelete), order => order.Id, detail => detail.OrderId,
                    (order, detail) => detail).AnyAsync();
        }


        public Task<bool> CheckCustomerHasUpdateServiceShoppingCart(int serviceId, int customerId, OrderStatus orderStatus)
        {
            var status = (byte)orderStatus;
            return Db.Orders.Where(x => x.CustomerId == customerId && x.Status == status && !x.IsDelete)
                .Join(Db.OrderServices.Where(x => x.ServiceId == serviceId && !x.IsDelete), order => order.Id, detail => detail.OrderId,
                    (order, detail) => detail).AnyAsync();
        }

        #region Store
        public int CheckDeposit(int orderId, int customerId)
        {
            using (var context = new ProjectXContext())
            {
                byte orderExchangeType = (byte)OrderExchangeType.Product;
                byte orderExchangeMode = (byte)OrderExchangeMode.Export;
                byte orderExchangeStatus = (byte)OrderExchangeStatus.Approved;
                byte oeOrderType = (byte)OrderExchangeOrderType.Order;
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_checkDeposit";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("orderId", orderId));
                cmd.Parameters.Add(new SqlParameter("customerId", customerId));
                cmd.Parameters.Add(new SqlParameter("orderExchangeType", orderExchangeType));
                cmd.Parameters.Add(new SqlParameter("orderExchangeMode", orderExchangeMode));
                cmd.Parameters.Add(new SqlParameter("orderExchangeStatus", orderExchangeStatus));
                cmd.Parameters.Add(new SqlParameter("oeOrderType", oeOrderType));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = Convert.ToInt32(cmd.ExecuteScalar());
                    return reader;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }

        public OrderDetailModel GetAll(int orderId, byte commentType, int customerId)
        {
            using (var context = new ProjectXContext())
            {
                byte orderExchangeType = (byte)OrderExchangeType.Product;
                byte orderExchangeMode = (byte)OrderExchangeMode.Export;
                byte orderExchangeStatus = (byte)OrderExchangeStatus.Approved;
                byte oeOrderType = (byte)OrderExchangeOrderType.Order;

                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_orderdetail_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("orderId", orderId));
                cmd.Parameters.Add(new SqlParameter("commentOrderType", commentType));
                cmd.Parameters.Add(new SqlParameter("orderExchangeType", orderExchangeType));
                cmd.Parameters.Add(new SqlParameter("orderExchangeMode", orderExchangeMode));
                cmd.Parameters.Add(new SqlParameter("orderExchangeStatus", orderExchangeStatus));
                cmd.Parameters.Add(new SqlParameter("oeOrderType", oeOrderType));
                cmd.Parameters.Add(new SqlParameter("customerId", customerId));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpOrderDetailItem = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderDetailItem>(reader).ToList().FirstOrDefault();
                    reader.NextResult();
                    var tmpOrderAddress = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderAddress>(reader).ToList().FirstOrDefault();
                    reader.NextResult();
                    var tmpOrderServiceItem = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderServiceItem>(reader).ToList();
                    reader.NextResult();
                    var tmpProductDetailItem = ((IObjectContextAdapter)context).ObjectContext.Translate<ProductDetailItem>(reader).ToList();
                    reader.NextResult();
                    var tmpOrderPackageItem = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderPackageItem>(reader).ToList();
                    reader.NextResult();
                    var tmpOrderCommentItem = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderComment>(reader).ToList();
                    reader.Close();

                    var model = new OrderDetailModel()
                    {
                        OrderDetailItem = tmpOrderDetailItem,
                        OrderAddress = tmpOrderAddress,
                        ListOrderService = tmpOrderServiceItem,
                        ListProductDetail = tmpProductDetailItem,
                        ListOrderPackage = tmpOrderPackageItem,
                        ListOrderComment = tmpOrderCommentItem
                    };
                    return model;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
        }


        public OrderDetailModel GetByUpdateService(int orderId)
        {
            using (var context = new ProjectXContext())
            {
                byte orderExchangeType = (byte)OrderExchangeType.Product;
                byte orderExchangeMode = (byte)OrderExchangeMode.Export;
                byte orderExchangeStatus = (byte)OrderExchangeStatus.Approved;
                byte oeOrderType = (byte)OrderExchangeOrderType.Order;
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_orderdetail_selectByUpdateServices";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("orderId", orderId));
                cmd.Parameters.Add(new SqlParameter("orderExchangeType", orderExchangeType));
                cmd.Parameters.Add(new SqlParameter("orderExchangeMode", orderExchangeMode));
                cmd.Parameters.Add(new SqlParameter("orderExchangeStatus", orderExchangeStatus));
                cmd.Parameters.Add(new SqlParameter("oeOrderType", oeOrderType));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpOrderDetailItem = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderDetailItem>(reader).ToList().FirstOrDefault();
                    reader.NextResult();
                    var tmpOrderServiceItem = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderServiceItem>(reader).ToList();
                    reader.Close();

                    var model = new OrderDetailModel()
                    {
                        OrderDetailItem = tmpOrderDetailItem,
                        ListOrderService = tmpOrderServiceItem,
                    };
                    return model;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }
        public int UpdateQuantity(int orderDetailId, int quantity, ref int beginAmount)
        {
            var result = 0;
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_orderdetail_updateQuantity";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("orderDetailId", orderDetailId));
                cmd.Parameters.Add(new SqlParameter("quantity", quantity));
                SqlParameter outputCount = new SqlParameter("@result", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputCount);
                SqlParameter tmpAmount = new SqlParameter("@tmpAmount", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(tmpAmount);
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    result = cmd.ExecuteNonQuery();
                    result = int.Parse(cmd.Parameters["@result"].Value == null ? "0" : cmd.Parameters["@result"].Value.ToString());
                    beginAmount = int.Parse(cmd.Parameters["@tmpAmount"].Value == null ? "0" : cmd.Parameters["@tmpAmount"].Value.ToString());
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
            return result;
        }
        public int UpdateNote(int orderId, string note)
        {
            var result = 0;
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_order_updateNote";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("orderId", orderId));
                cmd.Parameters.Add(new SqlParameter("note", note));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    result = cmd.ExecuteNonQuery();
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
            return result;
        }
        public OrderDetailModel GetForShip(int orderId)
        {
            using (var context = new ProjectXContext())
            {
                byte tmpStatus = (byte)OrderStatus.Finish;
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_orderdetail_selectShip";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("orderId", orderId));
                cmd.Parameters.Add(new SqlParameter("status", tmpStatus));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpOrderDetailItem = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderDetailItem>(reader).ToList().FirstOrDefault();
                    reader.NextResult();
                    var tmpOrderAddress = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderAddress>(reader).ToList().FirstOrDefault();
                    reader.NextResult();
                    var tmpProductDetailItem = ((IObjectContextAdapter)context).ObjectContext.Translate<ProductDetailItem>(reader).ToList();
                    reader.NextResult();
                    var tmpOrderPackageItem = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderPackageItem>(reader).ToList();
                    reader.NextResult();
                    var tmpRequestShipItem = ((IObjectContextAdapter)context).ObjectContext.Translate<RequestShipItem>(reader).ToList();
                    reader.Close();

                    var model = new OrderDetailModel()
                    {
                        OrderDetailItem = tmpOrderDetailItem,
                        OrderAddress = tmpOrderAddress,
                        ListProductDetail = tmpProductDetailItem,
                        ListOrderPackage = tmpOrderPackageItem,
                        ListRequestShip = tmpRequestShipItem
                    };
                    return model;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }
        #endregion
        #region Linq
        public int CheckDepositByLinq(int orderId, int customerId)
        {
            var result = 0;
            byte orderExchangeType = (byte)OrderExchangeType.Product;
            byte orderExchangeMode = (byte)OrderExchangeMode.Export;
            byte orderExchangeStatus = (byte)OrderExchangeStatus.Approved;
            byte oeOrderType = (byte)OrderExchangeOrderType.Order;
            //tinh tong tien phai tam ung
            var tmpAdvance = Db.OrderExchanges.Where(s => s.Type == orderExchangeType 
                                                    && s.Mode == orderExchangeMode 
                                                    && s.Status == orderExchangeStatus 
                                                    && !s.IsDelete 
                                                    && s.OrderId == orderId
                                                    && s.OrderType == oeOrderType)
                                                .Select(s => (decimal)s.TotalPrice).DefaultIfEmpty(0).Sum();

            //Lay tien kha dung cua khach hang
            var tmpBlance = 0M;
            var tmpCustomer = Db.Customers.FirstOrDefault(m => m.Id == customerId);
            if (tmpCustomer != null)
            {
                tmpBlance = tmpCustomer.BalanceAvalible;
            }
            if (tmpAdvance <= tmpBlance)
            {
                result = 1;
            }
            return result;

        }

        public List<ProductDetailItem> GetListOrderDetail(int orderId)
        {
            var listProductDetailItem = Db.OrderDetails.Where(m => m.OrderId == orderId && !m.IsDelete)
                                                       .Select(m => new ProductDetailItem()
                                                       {
                                                           Id = m.Id,
                                                           Name = m.Name,
                                                           Image = m.Image,
                                                           Quantity = m.Quantity,
                                                           Price = m.Price,
                                                           ExchangeRate = m.ExchangeRate,
                                                           TotalPrice = m.TotalPrice,
                                                           Link = m.Link,
                                                           Created = m.Created,
                                                           LastUpdate = m.LastUpdate,
                                                           Size = m.Size,
                                                           Color = m.Color,
                                                           Note = m.Note,
                                                           PrivateNote = m.PrivateNote,
                                                           ComplainNote = "",
                                                           Properties = m.Properties
                                                       })
                                                       .OrderBy(m => m.Id)
                                                       .ToList();
            return listProductDetailItem;
        }

        public OrderDetailModel GetAllByLinq(int orderId, byte commentType, int customerId)
        {
            byte orderExchangeType = (byte)OrderExchangeType.Product;
            byte orderExchangeMode = (byte)OrderExchangeMode.Export;
            byte orderExchangeStatus = (byte)OrderExchangeStatus.Approved;
            byte oeOrderType = (byte)OrderExchangeOrderType.Order;
            var tmpOrderDetail = Db.Orders.Where(m => m.Id == orderId && !m.IsDelete && m.CustomerId == customerId)
                                            .Select(m => new OrderDetailItem() {
                                                Id = m.Id,
                                                Code = m.Code,
                                                ProductNo = m.ProductNo,
                                                PackageNo = m.PackageNo,
                                                ExchangeRate = m.ExchangeRate,
                                                TotalExchange = m.TotalExchange,
                                                TotalPrice = m.TotalPrice,
                                                Total = m.Total,
                                                Status = m.Status,
                                                ServiceType = m.ServiceType,
                                                Note = m.Note,
                                                Created = m.Created,
                                                LastUpdate = m.LastUpdate,
                                                ExpectedDate = m.ExpectedDate ?? new DateTime(1753, 1, 1),
                                                TotalAdvance = (Db.OrderExchanges.Where(s => s.Type == orderExchangeType
                                                                    && s.Mode == orderExchangeMode
                                                                    && s.Status == orderExchangeStatus
                                                                    && !s.IsDelete
                                                                    && s.OrderId == m.Id
                                                                    && s.OrderType == oeOrderType)
                                                                .Select(s => (decimal)s.TotalPrice).DefaultIfEmpty(0).Sum()),
                                                IsComplain = (Db.Complains.Where(s => s.OrderId == m.Id).FirstOrDefault() != null ? 1 : 0),
                                                ReasonCancel = m.ReasonCancel,
                                                PercentDeposit = (Db.CustomerLevels.FirstOrDefault(s => s.Id == m.LevelId).PercentDeposit),
                                                CustomerCareFullName = m.CustomerCareFullName
                                            })
                                            .FirstOrDefault();

        var tmpOrderAddress = Db.OrderAddresses.Where(m => m.Id == (Db.Orders.FirstOrDefault(s => s.Id == orderId).ToAddressId))
                                                    .Select(m => new OrderAddress()
                                                    {
                                                        Id = m.Id,
                                                        ProvinceId = m.ProvinceId,
                                                        DistrictId = m.DistrictId,
                                                        WardId = m.WardId,
                                                        Address = m.Address,
                                                        ProvinceName = m.ProvinceName,
                                                        DistrictName = m.DistrictName,
                                                        WardName = m.WardName,
                                                        Phone = m.Phone,
                                                        FullName = m.FullName
                                                    }).FirstOrDefault();

            var listOrderServiceItem = Db.OrderServices.Where(m => m.OrderId == orderId)
                                                        .OrderBy(m => m.ServiceId)
                                                        .Select(m => new OrderServiceItem()
                                                        {
                                                            Id = m.Id,
                                                            ServiceId = m.ServiceId,
                                                            ServiceName = m.ServiceName,
                                                            TotalPrice = m.TotalPrice,
                                                            Checked = m.Checked,
                                                            IsDelete = m.IsDelete
                                                        })                                                        
                                                        .ToList();
            var listProductDetailItem = Db.OrderDetails.Where(m => m.OrderId == orderId)
                                                        .Select(m => new ProductDetailItem()
                                                        {
                                                            Id = m.Id,
                                                            Name = m.Name,
                                                            Image = m.Image,
                                                            Quantity = m.Quantity,
                                                            Price = m.Price,
                                                            ExchangeRate = m.ExchangeRate,
                                                            TotalPrice = m.TotalPrice,
                                                            Link = m.Link,
                                                            Created = m.Created,
                                                            LastUpdate = m.LastUpdate,
                                                            Size = m.Size,
                                                            Color = m.Color,
                                                            Note = m.Note,
                                                            PrivateNote = m.PrivateNote
                                                        }).ToList();

            

            var listOrderPackageItem = Db.OrderPackages.Where(m => m.OrderId == orderId)
                                                        .Select(m => new OrderPackageItem()
                                                        {
                                                            Id = m.Id,
                                                            Weight = (decimal)m.Weight,
                                                            TotalPrice = m.TotalPrice,
                                                            Note = (Db.HistoryPackages.Where(s => s.OrderId == orderId).OrderByDescending(s => s.CreateDate).FirstOrDefault() == null ? "" : Db.HistoryPackages.Where(s => s.OrderId == orderId).OrderByDescending(s => s.CreateDate).FirstOrDefault().Note),
                                                            CreateDate = (DateTime)(Db.HistoryPackages.Where(s => s.OrderId == orderId).OrderByDescending(s => s.CreateDate).FirstOrDefault() == null ? (new DateTime(1753,1,1)) : Db.HistoryPackages.Where(s => s.OrderId == orderId).OrderByDescending(s => s.CreateDate).FirstOrDefault().CreateDate),
                                                            TransportCode = m.TransportCode,
                                                            Status = m.Status
                                                        }).ToList();

            var listOrderComment = Db.OrderComments.Where(m => m.OrderId == orderId && m.OrderType == oeOrderType).ToList();


            var model = new OrderDetailModel()
            {
                OrderDetailItem = tmpOrderDetail,
                OrderAddress = tmpOrderAddress,
                ListOrderService = listOrderServiceItem,
                ListProductDetail = listProductDetailItem,
                ListOrderPackage = listOrderPackageItem,
                ListOrderComment = listOrderComment
            };
            return model;

        }

        public OrderDetailModel GetByUpdateServiceByLinq(int orderId)
        {
            byte orderExchangeType = (byte)OrderExchangeType.Product;
            byte orderExchangeMode = (byte)OrderExchangeMode.Export;
            byte orderExchangeStatus = (byte)OrderExchangeStatus.Approved;
            byte oeOrderType = (byte)OrderExchangeOrderType.Order;

            var tmpOrderDetail = Db.Orders.Where(m => m.Id == orderId && !m.IsDelete)
                                            .Select(m => new OrderDetailItem()
                                            {
                                                Id = m.Id,
                                                Code = m.Code,
                                                ProductNo = m.ProductNo,
                                                PackageNo = m.PackageNo,
                                                ExchangeRate = m.ExchangeRate,
                                                TotalExchange = m.TotalExchange,
                                                TotalPrice = m.TotalPrice,
                                                Total = m.Total,
                                                Status = m.Status,
                                                ServiceType = m.ServiceType,
                                                Note = m.Note,
                                                Created = m.Created,
                                                LastUpdate = m.LastUpdate,
                                                ExpectedDate = m.ExpectedDate ?? new DateTime(1753, 1, 1),
                                                TotalAdvance = m.Total - (Db.OrderExchanges.Where(s => s.Type == orderExchangeType
                                                                    && s.Mode == orderExchangeMode
                                                                    && s.Status == orderExchangeStatus
                                                                    && !s.IsDelete
                                                                    && s.OrderId == m.Id
                                                                    && s.OrderType == oeOrderType)
                                                                .Select(s => (decimal)s.TotalPrice).DefaultIfEmpty(0).Sum())
                                            })
                                            .FirstOrDefault();

            var listOrderServiceItem = Db.OrderServices.Where(m => m.OrderId == orderId)
                                                       .Select(m => new OrderServiceItem()
                                                       {
                                                           Id = m.Id,
                                                           ServiceId = m.ServiceId,
                                                           ServiceName = m.ServiceName,
                                                           TotalPrice = m.TotalPrice,
                                                           Checked = m.Checked,
                                                           IsDelete = m.IsDelete
                                                       }).ToList();

            var model = new OrderDetailModel()
            {
                OrderDetailItem = tmpOrderDetail,
                ListOrderService = listOrderServiceItem,
            };
            return model;
        }
        public OrderDetailModel GetForShipByLinq(int orderId)
        {
            var tmpOrderDetail = Db.Orders.Where(m => m.Id == orderId && !m.IsDelete)
                                            .Select(m => new OrderDetailItem()
                                            {
                                                Id = m.Id,
                                                Code = m.Code,
                                                ProductNo = m.ProductNo,
                                                PackageNo = m.PackageNo,
                                                ExchangeRate = m.ExchangeRate,
                                                TotalExchange = m.TotalExchange,
                                                TotalPrice = m.TotalPrice,
                                                Total = m.Total,
                                                Status = m.Status,
                                                ServiceType = m.ServiceType,
                                                Note = m.Note,
                                                Created = m.Created,
                                                LastUpdate = m.LastUpdate,
                                                ExpectedDate = m.ExpectedDate ?? new DateTime(1753, 1, 1),
                                                TotalAdvance = 0,
                                                IsComplain = (Db.Complains.Where(s => s.OrderId == m.Id).FirstOrDefault() != null ? 1 : 0)
                                            })
                                            .FirstOrDefault();
            var tmpOrderAddress = Db.OrderAddresses.Where(m => m.Id == (Db.Orders.FirstOrDefault(s => s.Id == orderId).ToAddressId))
                                                    .Select(m => new OrderAddress()
                                                    {
                                                        Id = m.Id,
                                                        ProvinceId = m.ProvinceId,
                                                        DistrictId = m.DistrictId,
                                                        WardId = m.WardId,
                                                        Address = m.Address,
                                                        ProvinceName = m.ProvinceName,
                                                        DistrictName = m.DistrictName,
                                                        WardName = m.WardName,
                                                        Phone = m.Phone,
                                                        FullName = m.FullName
                                                    }).FirstOrDefault();

            var listProductDetailItem = Db.OrderDetails.Where(m => m.OrderId == orderId)
                                                        .Select(m => new ProductDetailItem()
                                                        {
                                                            Id = m.Id,
                                                            Name = m.Name,
                                                            Image = m.Image,
                                                            Quantity = m.Quantity,
                                                            Price = m.Price,
                                                            ExchangeRate = m.ExchangeRate,
                                                            TotalPrice = m.TotalPrice,
                                                            Link = m.Link,
                                                            Created = m.Created,
                                                            LastUpdate = m.LastUpdate,
                                                            Size = m.Size,
                                                            Color = m.Color,
                                                            Note = m.Note
                                                        }).ToList();

            var listOrderPackageItem = Db.OrderPackages.Where(m => m.OrderId == orderId)
                                                        .Select(m => new OrderPackageItem()
                                                        {
                                                            Id = m.Id,
                                                            Weight = (decimal)m.Weight,
                                                            TotalPrice = m.TotalPrice,
                                                            Note = (Db.HistoryPackages.Where(s => s.OrderId == orderId).OrderByDescending(s => s.CreateDate).FirstOrDefault() == null ? "" : Db.HistoryPackages.Where(s => s.OrderId == orderId).OrderByDescending(s => s.CreateDate).FirstOrDefault().Note),
                                                            CreateDate = (DateTime)(Db.HistoryPackages.Where(s => s.OrderId == orderId).OrderByDescending(s => s.CreateDate).FirstOrDefault() == null ? (new DateTime(1753, 1, 1)) : Db.HistoryPackages.Where(s => s.OrderId == orderId).OrderByDescending(s => s.CreateDate).FirstOrDefault().CreateDate),
                                                            TransportCode = m.TransportCode,
                                                            Status = m.Status
                                                        }).ToList();
            var tmpRequestShipItem = Db.RequestShips.Where(m => m.OrderId == orderId && !m.IsDelete)
                                                    .Select(m => new RequestShipItem() { PackageCode = m.PackageCode}).ToList();

            var model = new OrderDetailModel()
            {
                OrderDetailItem = tmpOrderDetail,
                OrderAddress = tmpOrderAddress,
                ListProductDetail = listProductDetailItem,
                ListOrderPackage = listOrderPackageItem,
                ListRequestShip = tmpRequestShipItem
            };
            return model;

        }
        #endregion

    }
}
