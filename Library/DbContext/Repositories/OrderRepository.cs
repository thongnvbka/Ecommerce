using Common.Emums;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using Library.ViewModels.Items;
using Library.ViewModels.VipLevel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Common.Helper;

namespace Library.DbContext.Repositories
{
    public class OrderRepository : Repository<Order>
    {
        public OrderRepository(ProjectXContext context) : base(context)
        {
        }

        public static DateTime GetStartOfDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
        }
        public static DateTime GetEndOfDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999);
        }

        public List<Order> GetOrderToSetComplete(DateTime dateTime)
        {
            return Db.Orders.Where(x => x.IsDelete == false && 
                        (x.Type == (byte)OrderType.Deposit && x.Status == (byte)DepositStatus.GoingDelivery 
                        || x.Type != (byte)OrderType.Deposit && x.Status == (byte)OrderStatus.GoingDelivery) &&
                        x.PackageNo == x.PackageNoDelivered && -100 < x.Debt && x.Debt < 100 &&
                        x.LastDeliveryTime != null && x.LastDeliveryTime <= dateTime)
                .ToList();
        }

        /// <summary>
        /// Lấy ra các khách hàng liên quan đến kiện  hàng
        /// </summary>
        /// <param name="packageCodes">Mã kiện dạng: ;ma1;ma2;ma3;</param>
        /// <returns>Danh sách Email khách hàng</returns>
        public Task<List<string>> CustomerEmailByPackageCodes(string packageCodes)
        {
            return Db.OrderPackages.Where(x => !x.IsDelete && packageCodes.Contains(";" + x.Code + ";"))
                .Join(Db.Orders.Where(x => !x.IsDelete), p => p.OrderId, o => o.Id, (orderPackage, order) => order)
                .Select(x => x.CustomerEmail).Distinct().ToListAsync();
        }

        /// <summary>
        /// Lấy ra các đơn hàng chưa được kiểm đếm
        /// </summary>
        /// <param name="packageCodes">Mã kiện dạng: ;ma1;ma2;ma3;</param>
        /// <returns>Danh sách mã đơn hàng chưa được kiểm đếm</returns>
        public Task<List<string>> OrderCodeByService(string packageCodes)
        {
            return Db.OrderPackages.Where(x => !x.IsDelete && packageCodes.Contains(";" + x.Code + ";"))
                .Join(Db.OrderServices.Where(x => !x.IsDelete && x.Checked && x.ServiceId == (byte)OrderServices.Audit),
                    p => p.OrderId, s => s.OrderId, (order, orderService) => order)
                .Join(Db.OrderDetails.Where(x => !x.IsDelete && x.QuantityActuallyReceived == null && x.Status == (byte)OrderDetailStatus.Order), p => p.OrderId,
                    d => d.OrderId, (order, orderDetail) => order)
                .Select(o => o.OrderCode).Distinct().ToListAsync();
        }

        /// <summary>
        /// Lấy ra các đơn hàng kiểm đếm sai chưa được xử lý
        /// </summary>
        /// <param name="packageCodes">Mã kiện dạng: ;ma1;ma2;ma3;</param>
        /// <returns>Danh sách mã đơn hàng chưa được kiểm đếm</returns>
        public Task<List<string>> OrderCodeAcountingLose(string packageCodes)
        {
            return Db.OrderPackages.Where(x => !x.IsDelete && packageCodes.Contains(";" + x.Code + ";"))
                .Join(Db.OrderDetailCountings
                    .Where(x => x.IsDelete == false && x.Status != 2), p => p.OrderId, c => c.OrderId, (p, c) => p.OrderCode)
                .Distinct().ToListAsync();
        }

        /// <summary>
        /// Lấy ra các đơn hàng chưa được kiểm đếm
        /// </summary>
        /// <param name="packageCodes">Mã kiện dạng: ;ma1;ma2;ma3;</param>
        /// <returns>Danh sách mã đơn hàng chưa được kiểm đếm</returns>
        public Task<List<string>> OrderCodeByServicePacking(string packageCodes)
        {
            // Bao hàng đóng kiện gỗ
            var walletDetail = Db.WalletDetails.Where(x => x.IsDelete == false)
                    .Join(Db.Wallet.Where(x => x.IsDelete == false && x.Mode == 1), d => d.WalletId, w => w.Id,
                        (d, w) => d);

            return Db.OrderPackages.Where(x => !x.IsDelete && packageCodes.Contains(";" + x.Code + ";"))
                .Join(Db.OrderServices.Where(x => !x.IsDelete && x.Checked && x.ServiceId == (byte)OrderServices.Packing),
                    p => p.OrderId, s => s.OrderId, (package, orderService) => package)
                .GroupJoin(walletDetail, p => p.Id, wd => wd.PackageId, (p, wd) => new { p, wd })
                .SelectMany(x => x.wd.DefaultIfEmpty(), (arg, wd) => new { arg.p, wd })
                .Where(x => x.wd == null)
                .Select(x => x.p.OrderCode)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Lấy ra danh sách mã các đơn hàng theo mã kiện và loại dịch vụ
        /// </summary>
        /// <param name="packageCodes">Mã kiện dạng: ;ma1;ma2;ma3;</param>
        /// <param name="orderServices">Enum loại dịch vụ</param>
        /// <param name="isUseService">Sử dụng dịch vụ hay không mặc định là có</param>
        /// <returns></returns>
        public Task<List<string>> OrderCodeByService(string packageCodes, OrderServices orderServices, bool isUseService = true)
        {
            return Db.OrderPackages.Where(x => !x.IsDelete && packageCodes.Contains(";" + x.Code + ";"))
                .Join(Db.OrderServices.Where(x => !x.IsDelete && ((isUseService && x.Checked) || (isUseService == false && x.Checked == false)) && x.ServiceId == (byte)orderServices),
                    p => p.OrderId, s => s.OrderId, (order, orderService) => order)
                .Select(o => o.OrderCode).Distinct().ToListAsync();
        }

        public List<OrderNotifiItem> GetTopNotifi(int systemId, int top, int customerId, byte orderStatus, ref int countOrder)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_Order_selectTopNotifi";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("Top", top));
                cmd.Parameters.Add(new SqlParameter("SystemId", systemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", customerId));
                cmd.Parameters.Add(new SqlParameter("OrderStatus", orderStatus));

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
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderNotifiItem>(reader).ToList();
                    reader.Close();
                    countOrder = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());
                    return tmpList;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
        }
        public List<OrderNotifiItem> GetTopNotifiByLinq(int systemId, int top, int customerId, byte orderStatus, ref int countOrder)
        {
            var dateTime = DateTime.Now.AddDays(-30);
            var query = Db.Orders.Where(m => m.SystemId == systemId &&
                    m.Created >= dateTime && m.CustomerId == customerId && m.Status == orderStatus && m.Type == (byte)OrderType.Order && !m.IsDelete)
                                    .Select(x => new OrderNotifiItem()
                                    {
                                        Code = x.Code,
                                        Created = x.Created,
                                        Id = x.Id
                                    });

            var list = query.Take(top).ToList();
            countOrder = query.Count();
            return list;
        }
        public Task<List<Order>> RecentSuggetion(int warehouseId, int userId, RecentMode mode)
        {
            return Db.Orders.Where(x => !x.IsDelete && x.Status != (byte)OrderStatus.Cancel &&
                                        x.Status != (byte)OrderStatus.Finish &&
                                        x.Status != (byte)OrderStatus.New &&
                                        x.Status != (byte)OrderStatus.Order &&
                                        x.WarehouseId == warehouseId)
                .Join(Db.Recents.Where(x => x.Mode == (byte)mode && x.UserId == userId), order => order.Id,
                    recent => recent.RecordId,
                    (order, recents) => order)
                .ToListAsync();
        }

        public Task<List<Order>> GetOrderForSuggetExportWarehouse(string term, string codeOrders, byte size = 6)
        {
            return Db.Orders.Where(x => x.UnsignName.Contains(term) && !x.IsDelete &&
                                        (x.Status == (byte)OrderStatus.InWarehouse) &&
                                        (codeOrders == "" || !codeOrders.Contains(";" + x.Code + ";")) &&
                                        x.TotalWeight != 0
                )
                .GroupJoin(Db.ExportWarehouseDetails, order => order.Id, detail => detail.OrderId,
                    (order, detail) => new { order, detail })
                .Where(x => x.detail.All(y => y.OrderPackageNo != x.order.PackageNo))
                .Select(x => x.order).Distinct()
                .OrderBy(y => y.Id)
                .Take(size)
                .ToListAsync();
        }

        public decimal UpdateBalance(int orderId, int customerId, int type, byte status)
        {
            decimal result;
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_order_udpateBalance";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("type", type));
                cmd.Parameters.Add(new SqlParameter("orderId", orderId));
                cmd.Parameters.Add(new SqlParameter("customerId", customerId));
                cmd.Parameters.Add(new SqlParameter("status", status));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    result = decimal.Parse(cmd.ExecuteScalar().ToString());
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
            return result;
        }
        public decimal UpdateBalanceByLinq(int orderId, int customerId, int type, byte status)
        {
            decimal result = 0M;

            //Lấy level khách hàng
            var customer = Db.Customers.FirstOrDefault(m => m.Id == customerId);
            if (customer != null)
            {
                var levelId = customer.LevelId;
                var customerLevel = Db.CustomerLevels.FirstOrDefault(m => m.Id == levelId);
                if (customerLevel != null)
                {
                    var percentDeposit = customerLevel.PercentDeposit;
                    var order = Db.Orders.FirstOrDefault(m => m.Id == orderId);
                    if (order != null)
                    {
                        //type = 0 : đặt cọc theo level khách hàng
                        if (type == 0)
                        {
                            decimal tmp = (decimal)percentDeposit / 100;
                            result = order.TotalExchange * tmp;
                            order.DepositPercent = percentDeposit;
                        }
                        else
                        {
                            //type=1: đặt cọc toàn bộ đơn hàng
                            if (type == 1)
                            {
                                result = order.TotalExchange;
                                order.DepositPercent = 100;
                            }
                        }
                        if (result > 0)
                        {
                            //Cập nhật tiền tạm ứng trong customer
                            //customer.BalanceAvalible = customer.BalanceAvalible - result;
                            //Cập nhật tiền tạm ứng trong order
                            //order.TotalAdvance = result;
                            //order.Created = DateTime.Now;
                            //order.Status = status;
                            Db.SaveChanges();
                        }
                    }

                }
            }
            return result;
        }
        public decimal GetAdvanceMoney(int orderId, int customerId, int type, byte status)
        {
            decimal result;
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_order_GetAdvanceMoney";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("type", type));
                cmd.Parameters.Add(new SqlParameter("orderId", orderId));
                cmd.Parameters.Add(new SqlParameter("customerId", customerId));
                cmd.Parameters.Add(new SqlParameter("status", status));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    result = decimal.Parse(cmd.ExecuteScalar().ToString());
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
            return result;
        }
        public decimal GetAdvanceMoneyByLinq(int orderId, int customerId, int type, byte status)
        {
            decimal result = 0M;

            //Lấy level khách hàng
            var customer = Db.Customers.FirstOrDefault(m => m.Id == customerId);
            if (customer != null)
            {
                var levelId = customer.LevelId;
                var customerLevel = Db.CustomerLevels.FirstOrDefault(m => m.Id == levelId);
                if (customerLevel != null)
                {
                    var percentDeposit = customerLevel.PercentDeposit;
                    var order = Db.Orders.FirstOrDefault(m => m.Id == orderId);
                    if (order != null)
                    {
                        //type = 0 : đặt cọc theo level khách hàng
                        if (type == 0)
                        {
                            decimal tmp = (decimal)percentDeposit / 100;
                            result = order.Total * tmp;
                        }
                        else
                        {
                            //type=1: đặt cọc toàn bộ đơn hàng
                            if (type == 1)
                            {
                                result = order.Total;
                            }
                        }
                        if (result > 0)
                        {
                            using (var transaction = Db.Database.BeginTransaction())
                            {
                                try
                                {
                                    //Cập nhật tiền tạm ứng trong customer
                                    //customer.BalanceAvalible = customer.BalanceAvalible + result;
                                    //Cập nhật tiền tạm ứng trong order
                                    order.TotalAdvance = 0;
                                    order.Status = status;
                                    Db.SaveChanges();
                                    transaction.Commit();
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    result = 0;
                                }
                            }
                        }
                    }

                }
            }
            return result;
        }

        public OrderAdvanceMoneyItem GetMoneyLevelByLinq(int orderId, int customerId)
        {
            var result = new OrderAdvanceMoneyItem();

            //Lấy level khách hàng
            var customer = Db.Customers.FirstOrDefault(m => m.Id == customerId);
            if (customer != null)
            {
                var levelId = customer.LevelId;
                var customerLevel = Db.CustomerLevels.FirstOrDefault(m => m.Id == levelId);
                if (customerLevel != null)
                {
                    var percentDeposit = customerLevel.PercentDeposit;
                    var order = Db.Orders.FirstOrDefault(m => m.Id == orderId);
                    if (order != null)
                    {
                        //type = 0 : đặt cọc theo level khách hàng
                        decimal tmp = (decimal)percentDeposit / 100;
                        result.TotalLevel = string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:#,###}", order.TotalExchange * tmp);
                        result.Total = string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:#,###}", order.TotalExchange);
                        result.Percent = percentDeposit.ToString();
                    }

                }
            }
            return result;
        }
        public OrderAdvanceItem GetOrderAdvanceLevelByLinq(int orderId, int customerId)
        {
            var result = new OrderAdvanceItem();

            //Lấy level khách hàng
            var customer = Db.Customers.FirstOrDefault(m => m.Id == customerId);
            if (customer != null)
            {
                var levelId = customer.LevelId;
                var customerLevel = Db.CustomerLevels.FirstOrDefault(m => m.Id == levelId);
                if (customerLevel != null)
                {
                    var percentDeposit = customerLevel.PercentDeposit;
                    var order = Db.Orders.FirstOrDefault(m => m.Id == orderId);
                    if (order != null)
                    {
                        //type = 0 : đặt cọc theo level khách hàng
                        decimal tmp = (decimal)percentDeposit / 100;
                        result.TotalLevel = order.TotalExchange * tmp;
                        result.Total = order.TotalExchange;
                        result.Percent = percentDeposit;
                    }

                }
            }
            return result;
        }
        public int PaymentBalanceByLinq(int orderId, int customerId, decimal advanceMoney)
        {
            int result = 0;

            //Lấy level khách hàng
            var customer = Db.Customers.FirstOrDefault(m => m.Id == customerId);
            if (customer != null)
            {
                //Cập nhật tiền tạm ứng trong customer
                //customer.BalanceAvalible = customer.BalanceAvalible + result;
                result = Db.SaveChanges();
            }
            return result;
        }
        public int PaymentBalance(int orderId, int customerId, decimal advanceMoney)
        {
            int result;
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_order_paymentBalance";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("orderId", orderId));
                cmd.Parameters.Add(new SqlParameter("customerId", customerId));
                cmd.Parameters.Add(new SqlParameter("advanceMoney", advanceMoney));
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
        public decimal RestoreBalance(int orderId, int customerId)
        {
            decimal result;
            using (var context = new ProjectXContext())
            {
                byte orderExchangeType = (byte)OrderExchangeType.Product;
                byte orderExchangeMode = (byte)OrderExchangeMode.Export;
                byte orderExchangeStatus = (byte)OrderExchangeStatus.Approved;
                byte orderStatus = (byte)OrderStatus.Cancel;
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_order_restoreBalance";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("orderId", orderId));
                cmd.Parameters.Add(new SqlParameter("customerId", customerId));
                cmd.Parameters.Add(new SqlParameter("orderExchangeType", orderExchangeType));
                cmd.Parameters.Add(new SqlParameter("orderExchangeMode", orderExchangeMode));
                cmd.Parameters.Add(new SqlParameter("orderExchangeStatus", orderExchangeStatus));
                cmd.Parameters.Add(new SqlParameter("orderStatus", orderStatus));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    result = decimal.Parse(cmd.ExecuteScalar().ToString());
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
            return result;
        }
        public decimal RestoreBalanceByLinq(int orderId, int customerId)
        {
            decimal result = 0M;
            byte orderExchangeType = (byte)OrderExchangeType.Product;
            byte orderExchangeMode = (byte)OrderExchangeMode.Export;
            byte orderExchangeStatus = (byte)OrderExchangeStatus.Approved;
            byte oeOrderType = (byte)OrderExchangeOrderType.Order;
            byte orderStatus = (byte)OrderStatus.Cancel;
            //Lấy level khách hàng
            var customer = Db.Customers.FirstOrDefault(m => m.Id == customerId);
            if (customer != null)
            {
                var levelId = customer.LevelId;
                var customerLevel = Db.CustomerLevels.FirstOrDefault(m => m.Id == levelId);
                if (customerLevel != null)
                {
                    using (var transaction = Db.Database.BeginTransaction())
                    {
                        try
                        {
                            //Tính tiền tạm ứng
                            result = (Db.OrderExchanges.Where(s => s.Type == orderExchangeType && s.Mode == orderExchangeMode && s.Status == orderExchangeStatus && !s.IsDelete && s.OrderId == orderId && s.OrderType == oeOrderType).Select(s => (decimal)s.TotalPrice).DefaultIfEmpty(0).Sum());
                            // Bỏ phần tính trừ tiền tính tiền trong hàm của Tú
                            //var percentDeposit = customerLevel.PercentDeposit;

                            //Cập nhật tiền tạm ứng trong customer
                            //customer.BalanceAvalible = customer.BalanceAvalible + result;

                            //Cập nhật trạng thái order
                            var order = Db.Orders.FirstOrDefault(m => m.Id == orderId);

                            order.Status = orderStatus;

                            //Hủy link hàng

                            var listOrderDetail = Db.OrderDetails.Where(x => x.OrderId == order.Id).ToList();
                            foreach (var orderDetail in listOrderDetail)
                            {
                                orderDetail.LastUpdate = DateTime.Now;
                                orderDetail.QuantityBooked = 0;
                                orderDetail.TotalPrice = 0;
                                orderDetail.TotalExchange = 0;
                                orderDetail.Status = (byte)OrderDetailStatus.Cancel;
                            }

                            Db.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            result = 0;
                        }
                    }
                }
            }


            return result;
        }


        /// <summary>
        /// Lấy ra % tiền đặt cọc của khách hàng
        /// </summary>
        /// <param name="levelId">Cấp độ VIP 0 -> 8 </param>
        /// <returns>% Tiền phải đặt cọc</returns>
        public VipLevelViewModel CustomerVipLevel(byte levelId)
        {
            var result = new VipLevelViewModel { Order = 0, Ship = 0, Deposit = 90 };
            var tmpLevel = new CustomerLevel();
            tmpLevel = Db.CustomerLevels.FirstOrDefault(m => m.Id == levelId);
            if (tmpLevel != null)
            {
                result.Order = tmpLevel.Order;
                result.Ship = tmpLevel.Ship;
                result.Deposit = tmpLevel.PercentDeposit;
            }
            return result;
        }

        /// <summary>
        /// Lấy ra Level hiện tại tương ứng với số tiền truyền vào
        /// </summary>
        /// <param name="balanceAvalible">Số tiền hiện tại </param>
        /// <returns>Level hiện tại của khách hàng</returns>
        public CustomerLevel GetCustomerLevel(decimal balanceAvalible)
        {
            return Db.CustomerLevels.Single(
                    m => m.StartMoney < balanceAvalible && balanceAvalible <= m.EndMoney && !m.IsDelete);
        }

        /// <summary>
        /// Tính chi phí kiểm đếm
        /// </summary>
        /// <param name="quantity">Số lượng sản phẩm/phụ kiện kiểm đếm</param>
        /// <param name="productPrice">Giá sản phẩm đơn vị tiền CNY</param>
        /// <returns>Số tiền kiểm đếm / 1 sản phẩm</returns>
        public static decimal OrderAudit(int quantity, decimal productPrice)
        {
            //            1 - 2 sản phẩm         8 bath
            //            3 - 10 sản phẩm       5 bath
            //            11 - 100 sản phẩm    3 bath
            //            101 - 500 sản phẩm  2 bath
            //            > 500 sản phẩm      1 bath

            int price;
            if (quantity <= 2)
            {
                price = 8;
            }
            else if (quantity <= 10)
            {
                price = 5;
            }
            else if (quantity <= 100)
            {
                price = 3;
            }
            else if (quantity <= 500)
            {
                price = 2;
            }
            else
            {
                price = 1;
            }

            return price;


            //            // Giá sản phẩm nhỏ hơn 10 tệ tính là phụ kiện
            //            return productPrice < 10 ? OrderAuditExtension(quantity) : OrderAuditProduct(quantity);
        }

        /// <summary>
        /// Tính chi phí kiểm đếm sản phẩm
        /// </summary>
        /// <param name="quantity">Số lượng sản phẩm kiểm đếm</param>
        /// <returns>Số tiền kiểm đếm / 1 sản phẩm</returns>
        public static decimal OrderAuditProduct(int quantity)
        {
            var price = 0;
            if (quantity <= 2)
            {
                price = 5000;
            }
            else if (quantity >= 3 && quantity <= 10)
            {
                price = 3500;
            }
            else if (quantity >= 11 && quantity <= 100)
            {
                price = 2000;
            }
            else if (quantity >= 101 && quantity <= 500)
            {
                price = 1500;
            }
            else if (quantity > 500)
            {
                price = 1000;
            }

            return price;
        }

        /// <summary>
        /// Tính chi phí kiểm đếm phụ kiện
        /// </summary>
        /// <param name="quantity">Số lượng phụ kiện kiểm đếm</param>
        /// <returns>Số tiền kiểm đếm / 1 sản phẩm</returns>
        public static decimal OrderAuditExtension(int quantity)
        {
            var price = 0;
            if (quantity <= 2)
            {
                price = 1500;
            }
            else if (quantity >= 3 && quantity <= 10)
            {
                price = 1000;
            }
            else if (quantity >= 11 && quantity <= 100)
            {
                price = 700;
            }
            else if (quantity >= 101 && quantity <= 500)
            {
                price = 700;
            }
            else if (quantity > 500)
            {
                price = 700;
            }

            return price;
        }

        /// <summary>
        /// Tính phí vận chuyển Bên ngoài (TQ -> VN) Kiện hàng ký gửi
        /// </summary>
        /// <param name="totalWeight">Tổng cân nặng</param>
        /// <param name="targetWarehouseId">KHo đến (Bằng 8 là HN)</param>
        /// <returns></returns>
        public static decimal ShippingOutSide(decimal totalWeight, int targetWarehouseId)
        {
            return targetWarehouseId == 8 ? 17000 : 22000;
        }

        /// <summary>
        /// Tính phí vận chuyển Bên ngoài (TQ -> VN)
        /// </summary>
        /// <param name="serviceType">Gói dịch vụ</param>
        /// <param name="totalWeight">Tổng cân nặng</param>
        /// <param name="targetWarehouseId">KHo đến (Bằng 8 là HN)</param>
        /// <returns></returns>
        public static decimal ShippingOutSide(byte serviceType, decimal totalWeight, int targetWarehouseId)
        {
            // 1. 0 - 4,99kg: 75 bath/kg
            // 2. 5 - 19.99kg: 65bath / kg
            // 3. 20 - 49.99: 55bath / kg
            // 4.  > 50 : 45 bath / kg"

            if (totalWeight <= (decimal)4.99)
            {
                return 75;
            }
            if (totalWeight <= (decimal)19.99)
            {
                return 65;
            }

            if (totalWeight <= (decimal)49.99)
            {
                return 55;
            }

            return 45;
        }

        /// <summary>
        /// Tính giá thu thêm tiền vận chuyển hàng không giá/1kg vận chuyển
        /// </summary>
        /// <param name="totalWeight">Tổng cân nặng</param>
        /// <param name="targetWarehouseId">KHo đến (Hiện tại chưa sử dụng)</param>
        /// <returns></returns>
        public static decimal FastDelivery(decimal totalWeight, int targetWarehouseId = 0)
        {
            if (targetWarehouseId != 1011)
                ShippingOutSide(0, totalWeight, targetWarehouseId);

            // < 10kg
            if (totalWeight < 10)
            {
                return 36000;
            }

            // 10-30kg
            if (totalWeight <= 30)
            {
                return 33000;
            }

            // > 30kg
            return 29500;
        }

        ///// <summary>
        ///// Tính giá cho đơn hàng vận chuyển tiết kiệm
        ///// </summary>
        ///// <param name="totalWeight">Tổng cân nặng</param>
        ///// <param name="targetWarehouseId">KHo đến (Hiện tại chưa sử dụng)</param>
        ///// <returns></returns>
        //public static decimal OptimalDelivery(decimal totalWeight, int targetWarehouseId = 0)
        //{
        //    // Kho đích là HCM
        //    if (targetWarehouseId == 1011)
        //    {
        //        // Bỏ qua nếu cân nặng < 200kg
        //        if(totalWeight < 200)
        //            return ShippingOutSide(0, totalWeight, targetWarehouseId);

        //        // < 500 kg giá 15.000, >= 500 giá 13000
        //        return totalWeight < 500 ? 15000 : 13000;
        //    }

        //    // Kho HN
        //    if (totalWeight < 200)
        //        return ShippingOutSide(0, totalWeight, targetWarehouseId);

        //    // < 500 kg giá 11.000, >= 500 giá 9.000
        //    return totalWeight < 500 ? 11000 : 9000;
        //}

        /// <summary>
        /// Tính chi phí mua hàng hộ
        /// </summary>
        /// <param name="serviceType">Gói dịch vụ</param>
        /// <param name="totalPrice">Tổng tiền đơn hàng đã quy đổi thành tiền Việt</param>
        /// <returns>% phí mua hàng</returns>
        public static decimal OrderPrice(byte serviceType, decimal totalPrice)
        {
            // < 1 triệu
            if (totalPrice < 1000000)
            {
                return 12;
            }

            //// 1 triệu < giá trị đơn hàng < 2 triệu
            //if (totalPrice < 2000000)
            //{
            //    return 8;
            //}

            // 2 triệu = Giá trị đơn hàng < 30 triệu
            if (totalPrice < 30000000)
            {
                return 4;
            }

            // ≥30 triệu
            if (totalPrice < 50000000)
            {
                return (decimal)3.5;
            }

            // >50 triệu
            if (totalPrice < 100000000)
            {
                return 3;
            }

            // >100 triệu
            if (totalPrice < 200000000)
            {
                return (decimal)2.5;
            }

            // >200 triệu
            return 2;

            //// Gói tiêu dùng
            //if (serviceType == 1)
            //{
            //    return 2;
            //}

            //// Lớn hơn 200 triệu
            //if (totalPrice > 200000000)
            //{
            //    return 2;
            //}
            //if (totalPrice > 100000000)
            //{
            //    return (decimal)2.5;
            //}
            //if (totalPrice > 50000000)
            //{
            //    return 3;
            //}
            //if (totalPrice >= 30000000)
            //{
            //    return (decimal)3.5;
            //}
            //if (totalPrice < 30000000)
            //{
            //    return 4;
            //}

            //return 0;
        }

        /// <summary>
        /// Chi phí đóng kiện gỗ
        /// 100 bath / kg đầu + 4 bath / kg tiếp theo
        /// </summary>
        /// <param name="totalWeight">Tổng số cân nặng của đơn hàng</param>
        /// <returns>Số tiền kiểm đếm (CNY)</returns>
        public static decimal PackingPrice(decimal totalWeight)
        {
            if (totalWeight <= 1)
                return 100;

            return 100 + (totalWeight - 1) * 4;
        }

        /// <summary>
        /// Lấy danh sách các mã hợp đồng mà kế toán cần thanh toán
        /// </summary>
        /// <param name="totalRecord">Tổng số bản ghi</param>
        /// <param name="page">Trang hiện tại</param>
        /// <param name="pageSize">Kích thước trang</param>
        /// <param name="keyword">Từ khóa</param>
        /// <param name="status">Trạng thái</param>
        /// <param name="systemId">Mã hệ thống</param>
        /// <param name="dateStart">Ngày bắt đầu</param>
        /// <param name="dateEnd">Ngày kết thúc</param>
        /// <param name="userId">Mã nhân viên</param>
        /// <param name="customerId">Mã khách hàng</param>
        /// <returns>Danh sách hợp đồng</returns>
        public Task<List<OrderContractCodeResult>> GetOrderContractCode(out decimal totalPrice, out long totalRecord, 
            int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, 
            int? userId, int? customerId)
        {
            var query = Db.Orders.Where(x =>
                        x.UnsignName.Contains(keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        //&& (userId == null || userId == -1 || x.UserId == userId || x.CustomerCareUserId == userId)
                        && (customerId == null || customerId == -1 || x.CustomerId == customerId)
                        && (x.Status >= (byte)OrderStatus.WaitAccountant && x.Status != (byte)OrderStatus.Cancel)
                        && !x.IsDelete
                    )
                    .Join(Db.OrderContractCodes.Where(
                            x => !x.IsDelete
                                && (status == -1 || x.Status == status)
                                //&& (userId == null || userId == -1 || x.AccountantId == userId)
                                && (dateStart == null || x.CreateDate >= dateStart)
                                && (dateEnd == null || x.CreateDate <= dateEnd)
                                && (x.ContractCode.Trim() != "" && x.TotalPrice != 0 && x.TotalPrice != null)
                                && x.Status > (byte)ContractCodeType.New),
                            order => order.Id,
                            contract => contract.OrderId,
                            (o, c) => new OrderContractCodeResult()
                            {
                                Id = c.Id,
                                OrderId = c.OrderId,
                                OrderCode = o.Code,
                                OrderType = c.OrderType,
                                ContractCode = c.ContractCode,
                                TotalPrice = c.TotalPrice,
                                IsDelete = c.IsDelete,
                                CreateDate = c.CreateDate,
                                UpdateDate = c.UpdateDate,
                                Status = c.Status,
                                SystemId = o.SystemId,
                                AccountantDate = c.AccountantDate,
                                AccountantId = c.AccountantId,
                                AccountantFullName = c.AccountantFullName,
                                AccountantOfficeId = c.AccountantOfficeId,
                                AccountantOfficeName = c.AccountantOfficeName,
                                AccountantOfficeIdPath = c.AccountantOfficeIdPath,
                                OrderUserId = o.UserId,
                            })
                            .Where(x=> (userId == null || userId == -1 || x.AccountantId == userId || x.OrderUserId == userId))
                            .OrderBy(x => x.Status).ThenByDescending(x => x.Id);

            totalRecord = query.Count();
            totalPrice = totalRecord > 0 ? query.Sum(s => s.TotalPrice.Value) : 0;
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        /// <summary>
        /// Lây đơn hàng chưa có mã vận đơn
        /// </summary>
        /// <param name="totalRecord"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="systemId"></param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="userId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<OrderRiskResult> GetOrderNoContractCode(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState, bool checkExactCode)
        {
            var query =
                Db.Orders.Where(x =>
                        x.UnsignName.Contains(keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && ((x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel) || (x.Type == (byte)OrderType.Deposit && x.Status > (byte)DepositStatus.WaitOrder && x.Status != (byte)DepositStatus.Cancel))
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                        && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                        && (userState.Type != 0 || x.UserId == userState.UserId)
                    ).Join(
                     Db.OrderHistories.Where(
                            x => ((x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel) || (x.Type == (byte)OrderType.Deposit && x.Status > (byte)DepositStatus.WaitOrder && x.Status != (byte)DepositStatus.Cancel))
                        ),
                        order => order.Id,
                        history => history.OrderId,
                        (o, h) => new OrderRiskResult()
                        {
                            Id = o.Id,
                            Code = o.Code,
                            Type = o.Type,
                            Status = o.Status,
                            SystemId = o.SystemId,
                            SystemName = o.SystemName,
                            Created = o.Created,
                            LastUpdate = h.CreateDate,
                            CustomerId = o.CustomerId,
                            CustomerName = o.CustomerName,
                            CustomerAddress = o.CustomerAddress,
                            CustomerEmail = o.CustomerEmail,
                            CustomerPhone = o.CustomerPhone,
                            PackageNo = o.PacketNumber,
                            PackageNoInStock = o.PackageNoInStock,
                            ContractCodeNo = o.PackageNo,
                            Note = "Order has no transport code",
                            UserEmail = o.UserFullName,
                            UserNote = o.UserNote
                        }
                    )
                    .Where(x => x.ContractCodeNo == 0);

            if (checkExactCode)
            {
                keyword = MyCommon.RemoveCode(keyword);
                query =
                    Db.Orders.Where(x =>
                                x.UnsignName.Contains(keyword)
                                && (systemId == -1 || x.SystemId == systemId)
                                && (customerId == null || x.CustomerId == customerId)
                                &&
                                ((x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess &&
                                  x.Status != (byte)OrderStatus.Cancel) ||
                                 (x.Type == (byte)OrderType.Deposit && x.Status > (byte)DepositStatus.WaitOrder &&
                                  x.Status != (byte)DepositStatus.Cancel))
                                && !x.IsDelete
                                && (userId == null || x.UserId == userId)
                                &&
                                (userState.Type == 0 ||
                                 (x.OfficeIdPath == userState.OfficeIdPath ||
                                  x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                                && (userState.Type != 0 || x.UserId == userState.UserId)
                                && x.Code == (keyword)
                        )
                        .Join(
                     Db.OrderHistories.Where(
                            x => ((x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel) || (x.Type == (byte)OrderType.Deposit && x.Status > (byte)DepositStatus.WaitOrder && x.Status != (byte)DepositStatus.Cancel))
                        ),
                        order => order.Id,
                        history => history.OrderId,
                        (o, h) => new OrderRiskResult()
                        {
                            Id = o.Id,
                            Code = o.Code,
                            Type = o.Type,
                            Status = o.Status,
                            SystemId = o.SystemId,
                            SystemName = o.SystemName,
                            Created = o.Created,
                            LastUpdate = h.CreateDate,
                            CustomerId = o.CustomerId,
                            CustomerName = o.CustomerName,
                            CustomerAddress = o.CustomerAddress,
                            CustomerEmail = o.CustomerEmail,
                            CustomerPhone = o.CustomerPhone,
                            PackageNo = o.PacketNumber,
                            PackageNoInStock = o.PackageNoInStock,
                            ContractCodeNo = o.PackageNo,
                            Note = "Order has no transport code",
                            UserEmail = o.UserFullName,
                            UserNote = o.UserNote
                        }
                    )
                    .Where(x => x.ContractCodeNo == 0);
            }

            totalRecord = query.Count();
            return query.OrderBy(x => x.LastUpdate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<OrderRiskResult> GetOrderNoContractCode3Day(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState, bool checkExactCode)
        {
            var timeNow = DateTime.Now.AddDays(-3);
            var query =
                Db.Orders.Where(x =>
                        x.UnsignName.Contains(keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && ((x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel) || (x.Type == (byte)OrderType.Deposit && x.Status > (byte)DepositStatus.WaitOrder && x.Status != (byte)DepositStatus.Cancel))
                        //&& (x.Status >= (byte)OrderStatus.OrderSuccess)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                        && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                        && (userState.Type != 0 || x.UserId == userState.UserId)
                    ).Join(
                     Db.OrderHistories.Where(
                            x => ((x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel) || (x.Type == (byte)OrderType.Deposit && x.Status > (byte)DepositStatus.WaitOrder && x.Status != (byte)DepositStatus.Cancel))
                            && x.CreateDate <= timeNow
                        ),
                        order => order.Id,
                        history => history.OrderId,
                        (o, h) => new { Order = o, History = h }
                    )
                    .Join(
                     Db.OrderReasons.Where(
                            x => x.Type == (byte)OrderReasonType.NoCodeOfLading
                        ),
                        order => order.Order.Id,
                        reason => reason.OrderId,
                        (g, r) => new OrderRiskResult()
                        {
                            Id = g.Order.Id,
                            Code = g.Order.Code,
                            Type = g.Order.Type,
                            Status = g.Order.Status,
                            SystemId = g.Order.SystemId,
                            SystemName = g.Order.SystemName,
                            Created = g.Order.Created,
                            LastUpdate = g.History.CreateDate,
                            CustomerId = g.Order.CustomerId,
                            CustomerName = g.Order.CustomerName,
                            CustomerAddress = g.Order.CustomerAddress,
                            CustomerEmail = g.Order.CustomerEmail,
                            CustomerPhone = g.Order.CustomerPhone,
                            PackageNo = g.Order.PacketNumber,
                            PackageNoInStock = g.Order.PackageNoInStock,
                            ContractCodeNo = g.Order.PackageNo,
                            Note = r.Reason,
                            UserEmail = g.Order.UserFullName,
                            UserNote = g.Order.UserNote,
                            BargainType = g.Order.BargainType
                        })
                    .Where(x => x.ContractCodeNo == 0)
                    .OrderBy(x => x.LastUpdate);

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        //Todo lấy ra danh sách dơn hàng order join với bảng oderdetail

        public Task<List<OrderJoinOrderDetailResult>> GetOrderJoinOrderDetail(out long totalRecord, int page,
            int pageSize, string keyword, int status, DateTime? dateStart, DateTime? dateEnd, int customerId)
        {
            var query =
                Db.Orders.Where(

                   x => (x.Code.Contains(keyword) || x.Code.Contains(keyword) ||
                               x.CustomerEmail.Contains(keyword) || x.CustomerName.Contains(keyword)
                             || x.CustomerAddress.Contains(keyword) || x.UnsignName.Contains(keyword))

                      && !x.IsDelete
                      && (x.CustomerId.Value == customerId)
                    )
                    .GroupJoin(
                        Db.OrderDetails.Where(
                            x => !x.IsDelete
                                && (status == -1 || x.Status == status)
                                && (dateStart == null || x.Created >= dateStart)
                                && (dateEnd == null || x.Created <= dateEnd)
                        ),
                        order => order.Id,
                        orderDetial => orderDetial.OrderId,
                        (o, od) => new { o, od })
                        .Select(x => new OrderJoinOrderDetailResult()
                        {
                            Id = x.o.Id,
                            Image = x.od.FirstOrDefault().Image,
                            Quantity = (int?)x.od.Sum(y => y.Quantity) ?? 0,
                            WebsiteName = x.o.WebsiteName,
                            ShopId = x.o.ShopId,
                            Code = x.o.Code,
                            CustomerId = x.o.CustomerId,
                            Created = x.od.FirstOrDefault().Created,
                            LastUpdate = (DateTime?)x.od.FirstOrDefault().LastUpdate ?? x.o.Created,
                            ShopName = x.o.ShopName,
                            ShopLink = x.o.ShopLink,
                            Total = x.o.Total,
                            Name = ""
                        })
                    .OrderBy(x => x.LastUpdate);

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        /// <summary>
        /// Đơn hàng chờ kế toán thanh toán
        /// </summary>
        /// <param name="totalRecord"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="systemId"></param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="userId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<OrderRiskResult> GetOrderAccountant(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState, bool checkExactCode)
        {
            var query =
                Db.Orders.Where(x =>
                        x.UnsignName.Contains(keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && (x.Status >= (byte)OrderStatus.WaitAccountant && x.Status < (byte)OrderStatus.Shipping)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                        && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                        && (userState.Type != 0 || x.UserId == userState.UserId)
                    )
                    .GroupJoin(
                        Db.OrderContractCodes.Where(
                            x => !x.IsDelete
                            && x.Status > (byte)ContractCodeType.New
                        ),
                        order => order.Id,
                        contract => contract.OrderId,
                        (o, c) => new { Order = o, ContractCode = c }
                    )
                    .Select(s => new OrderRiskResult()
                    {
                        Id = s.Order.Id,
                        Code = s.Order.Code,
                        Type = s.Order.Type,
                        Status = s.Order.Status,
                        SystemId = s.Order.SystemId,
                        SystemName = s.Order.SystemName,
                        Created = s.Order.Created,
                        LastUpdate = s.ContractCode.FirstOrDefault().CreateDate,
                        CustomerId = s.Order.CustomerId,
                        CustomerName = s.Order.CustomerName,
                        CustomerAddress = s.Order.CustomerAddress,
                        CustomerEmail = s.Order.CustomerEmail,
                        CustomerPhone = s.Order.CustomerPhone,
                        PackageNo = s.ContractCode.Count(),
                        PackageNoInStock = s.ContractCode.Count(x => x.Status == (byte)ContractCodeType.AwaitingPayment),
                        ContractCodeNo = s.ContractCode.Count(),
                        Note = "Đơn hàng chờ kế toàn thanh toán",
                        UserEmail = s.Order.UserFullName,
                        BargainType = s.Order.BargainType,
                        UserNote = s.Order.UserNote
                    })
                    .Where(x => x.PackageNoInStock > 0)
                    .OrderBy(x => x.LastUpdate);

            if (checkExactCode)
            {
                keyword = MyCommon.RemoveCode(keyword);
                query =
                Db.Orders.Where(x =>
                        x.Code == (keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && (x.Status >= (byte)OrderStatus.WaitAccountant && x.Status < (byte)OrderStatus.Shipping)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                        && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                        && (userState.Type != 0 || x.UserId == userState.UserId)
                    )
                    .GroupJoin(
                        Db.OrderContractCodes.Where(
                            x => !x.IsDelete
                            && x.Status > (byte)ContractCodeType.New
                        ),
                        order => order.Id,
                        contract => contract.OrderId,
                        (o, c) => new { Order = o, ContractCode = c }
                    )
                    .Select(s => new OrderRiskResult()
                    {
                        Id = s.Order.Id,
                        Code = s.Order.Code,
                        Type = s.Order.Type,
                        Status = s.Order.Status,
                        SystemId = s.Order.SystemId,
                        SystemName = s.Order.SystemName,
                        Created = s.Order.Created,
                        LastUpdate = s.ContractCode.FirstOrDefault().CreateDate,
                        CustomerId = s.Order.CustomerId,
                        CustomerName = s.Order.CustomerName,
                        CustomerAddress = s.Order.CustomerAddress,
                        CustomerEmail = s.Order.CustomerEmail,
                        CustomerPhone = s.Order.CustomerPhone,
                        PackageNo = s.ContractCode.Count(),
                        PackageNoInStock = s.ContractCode.Count(x => x.Status == (byte)ContractCodeType.AwaitingPayment),
                        ContractCodeNo = s.ContractCode.Count(),
                        Note = "Orders wait for payment accounting",
                        UserEmail = s.Order.UserFullName,
                        BargainType = s.Order.BargainType,
                        UserNote = s.Order.UserNote
                    })
                    .Where(x => x.PackageNoInStock > 0)
                    .OrderBy(x => x.LastUpdate);
            }

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// Danh sách đơn hàng chưa đủ kiện về kho
        /// </summary>
        /// <param name="totalRecord"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="systemId"></param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="userId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<OrderRiskResult> GetOrderNoWarehouse(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState, bool checkExactCode)
        {
            var query =
                Db.Orders.Where(x =>
                        x.UnsignName.Contains(keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && ((x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel) || (x.Type == (byte)OrderType.Deposit && x.Status > (byte)DepositStatus.WaitOrder && x.Status != (byte)DepositStatus.Cancel))
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                    && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                    && (userState.Type != 0 || x.UserId == userState.UserId)
                    )
                    .Select(s => new OrderRiskResult()
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Type = s.Type,
                        Status = s.Status,
                        SystemId = s.SystemId,
                        SystemName = s.SystemName,
                        Created = s.Created,
                        LastUpdate = s.LastUpdate,
                        CustomerId = s.CustomerId,
                        CustomerName = s.CustomerName,
                        CustomerAddress = s.CustomerAddress,
                        CustomerEmail = s.CustomerEmail,
                        CustomerPhone = s.CustomerPhone,
                        PackageNo = s.PackageNo,
                        PackageNoInStock = s.PackageNoInStock,
                        ContractCodeNo = 0,
                        Note = "Đơn hàng chưa đủ kiện về kho",
                        UserEmail = s.UserFullName,
                        UserNote = s.UserNote
                    })
                    .Where(x => x.PackageNo > x.PackageNoInStock || (x.PackageNoInStock == 0 && x.PackageNo > 0))
                    .OrderBy(x => x.LastUpdate);

            if (checkExactCode)
            {
                keyword = MyCommon.RemoveCode(keyword);
                query =
                Db.Orders.Where(x =>
                        x.Code == (keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && ((x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel) || (x.Type == (byte)OrderType.Deposit && x.Status > (byte)DepositStatus.WaitOrder && x.Status != (byte)DepositStatus.Cancel))
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                    && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                    && (userState.Type != 0 || x.UserId == userState.UserId)
                    )
                    .Select(s => new OrderRiskResult()
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Type = s.Type,
                        Status = s.Status,
                        SystemId = s.SystemId,
                        SystemName = s.SystemName,
                        Created = s.Created,
                        LastUpdate = s.LastUpdate,
                        CustomerId = s.CustomerId,
                        CustomerName = s.CustomerName,
                        CustomerAddress = s.CustomerAddress,
                        CustomerEmail = s.CustomerEmail,
                        CustomerPhone = s.CustomerPhone,
                        PackageNo = s.PackageNo,
                        PackageNoInStock = s.PackageNoInStock,
                        ContractCodeNo = 0,
                        Note = "Orders not enough condition into warehouse ",
                        UserEmail = s.UserFullName,
                        UserNote = s.UserNote
                    })
                    .Where(x => x.PackageNo > x.PackageNoInStock || (x.PackageNoInStock == 0 && x.PackageNo > 0))
                    .OrderBy(x => x.LastUpdate);
            }

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public Task<List<OrderRiskResult>> GetOrderNoWarehouse4Day(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState, bool checkExactCode)
        {
            var query =
                Db.Orders.Where(x =>
                        x.UnsignName.Contains(keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && (x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                    && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                    && (userState.Type != 0 || x.UserId == userState.UserId)
                    ).Join(
                     Db.OrderReasons.Where(
                            x => x.Type == (byte)OrderReasonType.NotEnoughInventory
                        ),
                        order => order.Id,
                        reason => reason.OrderId,
                        (s, r) => new OrderRiskResult()
                        {
                            Id = s.Id,
                            Code = s.Code,
                            Type = s.Type,
                            Status = s.Status,
                            SystemId = s.SystemId,
                            SystemName = s.SystemName,
                            Created = s.Created,
                            LastUpdate = s.LastUpdate,
                            CustomerId = s.CustomerId,
                            CustomerName = s.CustomerName,
                            CustomerAddress = s.CustomerAddress,
                            CustomerEmail = s.CustomerEmail,
                            CustomerPhone = s.CustomerPhone,
                            PackageNo = s.PackageNo,
                            PackageNoInStock = s.PackageNoInStock,
                            ContractCodeNo = 0,
                            Note = r.Reason,
                            UserEmail = s.UserFullName,
                            UserNote = s.UserNote
                        }
                    )
                    .Where(x => x.PackageNo > x.PackageNoInStock || (x.PackageNoInStock == 0 && x.PackageNo > 0))
                    .OrderBy(x => x.LastUpdate);

            if (checkExactCode)
            {
                keyword = MyCommon.RemoveCode(keyword);

                query =
                Db.Orders.Where(x =>
                        x.Code == (keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && (x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                    && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                    && (userState.Type != 0 || x.UserId == userState.UserId)
                    ).Join(
                     Db.OrderReasons.Where(
                            x => x.Type == (byte)OrderReasonType.NotEnoughInventory
                        ),
                        order => order.Id,
                        reason => reason.OrderId,
                        (s, r) => new OrderRiskResult()
                        {
                            Id = s.Id,
                            Code = s.Code,
                            Type = s.Type,
                            Status = s.Status,
                            SystemId = s.SystemId,
                            SystemName = s.SystemName,
                            Created = s.Created,
                            LastUpdate = s.LastUpdate,
                            CustomerId = s.CustomerId,
                            CustomerName = s.CustomerName,
                            CustomerAddress = s.CustomerAddress,
                            CustomerEmail = s.CustomerEmail,
                            CustomerPhone = s.CustomerPhone,
                            PackageNo = s.PackageNo,
                            PackageNoInStock = s.PackageNoInStock,
                            ContractCodeNo = 0,
                            Note = r.Reason,
                            UserEmail = s.UserFullName,
                            UserNote = s.UserNote
                        }
                    )
                    .Where(x => x.PackageNo > x.PackageNoInStock || (x.PackageNoInStock == 0 && x.PackageNo > 0))
                    .OrderBy(x => x.LastUpdate);
            }

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        /// <summary>
        /// Lây danh sách tất cả đơn hàng trễ xử lý
        /// </summary>
        /// <param name="totalRecord"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="systemId"></param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="userId"></param>
        /// <param name="customerId"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public Task<List<Order>> GetOrderDelayAll(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState)
        {
            var query =
                Db.Orders.Where(x =>
                        x.UnsignName.Contains(keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && (x.Status >= (byte)OrderStatus.Order)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                        && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                        && (userState.Type != 0 || x.UserId == userState.UserId)
                    )
                    .Join(
                        Db.OrderReasons.Where(
                            x => x.Type == (byte)OrderReasonType.Delay
                        ),
                        order => order.Id,
                        reasons => reasons.OrderId,
                        (o, r) => new { Order = o, OrderReasons = r }
                    )
                    .Select(s => s.Order)
                    .OrderBy(x => x.LastUpdate);

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public Task<List<OrderReasonResult>> GetOrderDelayReasonAll(string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState)
        {
            var query =
                Db.Orders.Where(x =>
                        x.UnsignName.Contains(keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && (x.Status >= (byte)OrderStatus.Order)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                        && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                        && (userState.Type != 0 || x.UserId == userState.UserId)
                    )
                    .Join(
                        Db.OrderReasons.Where(
                            x => x.Type == (byte)OrderReasonType.Delay
                        ),
                        order => order.Id,
                        reasons => reasons.OrderId,
                        (o, r) => new { Order = o, OrderReasons = r }
                    )
                    .Select(s => new OrderReasonResult
                    {
                        Id = s.Order.Id,
                        Code = s.Order.Code,
                        Type = s.Order.Type,
                        WebsiteName = s.Order.WebsiteName,
                        ShopId = s.Order.ShopId,
                        ShopName = s.Order.ShopName,
                        ShopLink = s.Order.ShopLink,
                        ProductNo = s.Order.ProductNo,
                        PackageNo = s.Order.PackageNo,
                        ContractCode = s.Order.ContractCode,
                        ContractCodes = s.Order.ContractCodes,
                        LevelId = s.Order.LevelId,
                        LevelName = s.Order.LevelName,
                        TotalWeight = s.Order.TotalWeight,
                        DiscountType = s.Order.DiscountType,
                        DiscountValue = s.Order.DiscountValue,
                        GiftCode = s.Order.GiftCode,
                        CreatedTool = s.Order.CreatedTool,
                        Currency = s.Order.Currency,
                        ExchangeRate = s.Order.ExchangeRate,
                        TotalExchange = s.Order.TotalExchange,
                        TotalPrice = s.Order.TotalPrice,
                        Total = s.Order.Total,
                        HashTag = s.Order.HashTag,
                        WarehouseId = s.Order.WarehouseId,
                        WarehouseName = s.Order.WarehouseName,
                        CustomerId = s.Order.CustomerId,
                        CustomerName = s.Order.CustomerName,
                        CustomerEmail = s.Order.CustomerEmail,
                        CustomerPhone = s.Order.CustomerPhone,
                        CustomerAddress = s.Order.CustomerAddress,
                        Status = s.Order.Status,
                        UserId = s.Order.Id,
                        UserFullName = s.Order.UserFullName,
                        OfficeId = s.Order.OfficeId,
                        OfficeName = s.Order.OfficeName,
                        OfficeIdPath = s.Order.OfficeIdPath,
                        CreatedOfficeIdPath = s.Order.CreatedOfficeIdPath,
                        CreatedUserId = s.Order.CreatedUserId,
                        CreatedUserFullName = s.Order.CreatedUserFullName,
                        CreatedOfficeId = s.Order.CreatedOfficeId,
                        CreatedOfficeName = s.Order.CreatedOfficeName,
                        OrderInfoId = s.Order.OrderInfoId,
                        FromAddressId = s.Order.FromAddressId,
                        ToAddressId = s.Order.ToAddressId,
                        SystemId = s.Order.SystemId,
                        SystemName = s.Order.SystemName,
                        ServiceType = s.Order.ServiceType,
                        Note = s.Order.Note,
                        PrivateNote = s.Order.PrivateNote,
                        LinkNo = s.Order.LinkNo,
                        IsDelete = s.Order.IsDelete,
                        Created = s.Order.Created,
                        LastUpdate = s.Order.LastUpdate,
                        ExpectedDate = s.Order.ExpectedDate,
                        TotalPurchase = s.Order.TotalPurchase,
                        TotalAdvance = s.Order.TotalAdvance,
                        ReasonCancel = s.Order.ReasonCancel,
                        PriceBargain = s.Order.PriceBargain,
                        PaidShop = s.Order.PaidShop,
                        FeeShip = s.Order.FeeShip,
                        FeeShipBargain = s.Order.FeeShipBargain,
                        IsPayWarehouseShip = s.Order.IsPayWarehouseShip,
                        UserNote = s.Order.UserNote,
                        PackageNoInStock = s.Order.PackageNoInStock,
                        UnsignName = s.Order.UnsignName,
                        PacketNumber = s.Order.PacketNumber,
                        Description = s.Order.Description,
                        ProvisionalMoney = s.Order.ProvisionalMoney,
                        DepositType = s.Order.DepositType,
                        WarehouseDeliveryId = s.Order.WarehouseDeliveryId,
                        WarehouseDeliveryName = s.Order.WarehouseDeliveryName,
                        ApprovelUnit = s.Order.ApprovelUnit,
                        ApprovelPrice = s.Order.ApprovelPrice,
                        ContactName = s.Order.ContactName,
                        ContactPhone = s.Order.ContactPhone,
                        ContactAddress = s.Order.ContactAddress,
                        ContactEmail = s.Order.ContactEmail,
                        CustomerCareUserId = s.Order.CustomerCareUserId,
                        CustomerCareFullName = s.Order.CustomerCareFullName,
                        CustomerCareOfficeId = s.Order.CustomerCareOfficeId,
                        CustomerCareOfficeName = s.Order.CustomerCareOfficeName,
                        CustomerCareOfficeIdPath = s.Order.CustomerCareOfficeIdPath,
                        ReasonId = s.OrderReasons.ReasonId,
                        Reason = s.OrderReasons.Reason,
                        BargainType = s.Order.BargainType
                    })
                    .OrderBy(x => x.LastUpdate);
            return query.ToListAsync();
        }

        /// <summary>
        /// Lây đơn hàng chưa có mã vận đơn xuất báo cáo
        /// </summary>
        /// <param name="totalRecord"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="systemId"></param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="userId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public Task<List<OrderRiskResult>> GetOrderNoContractCodeExcel(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState)
        {
            var query =
                Db.Orders.Where(x =>
                        x.UnsignName.Contains(keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && (x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                    && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                    && (userState.Type != 0 || x.UserId == userState.UserId)
                    ).Join(
                     Db.OrderHistories.Where(
                            x => x.Status == (byte)OrderStatus.OrderSuccess
                        ),
                        order => order.Id,
                        history => history.OrderId,
                        (o, h) => new { o, h }
                    )
                    .GroupJoin(
                        Db.OrderReasons.Where(
                            x => x.Type == (byte)OrderReasonType.NoCodeOfLading
                        ),
                        group => group.o.Id,
                        reason => reason.OrderId,
                        (g, r) => new { Order = g.o, Reasons = r, History = g.h }
                    )
                    .Select(s => new OrderRiskResult()
                    {
                        Id = s.Order.Id,
                        Code = s.Order.Code,
                        Type = s.Order.Type,
                        Status = s.Order.Status,
                        SystemId = s.Order.SystemId,
                        SystemName = s.Order.SystemName,
                        Created = s.Order.Created,
                        LastUpdate = s.History.CreateDate,
                        CustomerId = s.Order.CustomerId,
                        CustomerName = s.Order.CustomerName,
                        CustomerAddress = s.Order.CustomerAddress,
                        CustomerEmail = s.Order.CustomerEmail,
                        CustomerPhone = s.Order.CustomerPhone,
                        PackageNo = s.Order.PackageNo,
                        PackageNoInStock = s.Order.PackageNoInStock,
                        ContractCodeNo = s.Order.PackageNo,
                        Note = s.Reasons.FirstOrDefault() == null ? "" : s.Reasons.FirstOrDefault().ReasonId + ". " + s.Reasons.FirstOrDefault().Reason,
                        UserEmail = s.Order.UserFullName,
                        UserNote = s.Order.UserNote,
                        BargainType = s.Order.BargainType
                    })
                    .Where(x => x.ContractCodeNo == 0)
                    .OrderBy(x => x.LastUpdate);

            totalRecord = query.Count();
            return query.ToListAsync();
        }

        public Task<List<OrderRiskResult>> GetOrderNoContractCode3DayExcel(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState)
        {
            var timeNow = DateTime.Now.AddDays(-3);
            var query =
                Db.Orders.Where(x =>
                        x.UnsignName.Contains(keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && ((x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel) || (x.Type == (byte)OrderType.Deposit && x.Status > (byte)DepositStatus.WaitOrder && x.Status != (byte)DepositStatus.Cancel))
                        //&& (x.Status >= (byte)OrderStatus.OrderSuccess)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                        && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                        && (userState.Type != 0 || x.UserId == userState.UserId)
                    ).Join(
                     Db.OrderHistories.Where(
                            x => ((x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel) || (x.Type == (byte)OrderType.Deposit && x.Status > (byte)DepositStatus.WaitOrder && x.Status != (byte)DepositStatus.Cancel))
                            && x.CreateDate <= timeNow
                        ),
                        order => order.Id,
                        history => history.OrderId,
                        (o, h) => new { o, h }
                    )
                    .Join(
                     Db.OrderReasons.Where(
                            x => x.Type == (byte)OrderReasonType.NoCodeOfLading
                        ),
                        order => order.o.Id,
                        reason => reason.OrderId,
                        (g, r) => new { Order = g.o, History = g.h, Reason = r }
                    )
                    .Select(s => new OrderRiskResult()
                    {
                        Id = s.Order.Id,
                        Code = s.Order.Code,
                        Type = s.Order.Type,
                        Status = s.Order.Status,
                        SystemId = s.Order.SystemId,
                        SystemName = s.Order.SystemName,
                        Created = s.Order.Created,
                        LastUpdate = s.History.CreateDate,
                        CustomerId = s.Order.CustomerId,
                        CustomerName = s.Order.CustomerName,
                        CustomerAddress = s.Order.CustomerAddress,
                        CustomerEmail = s.Order.CustomerEmail,
                        CustomerPhone = s.Order.CustomerPhone,
                        PackageNo = s.Order.PackageNo,
                        PackageNoInStock = s.Order.PackageNoInStock,
                        ContractCodeNo = s.Order.PackageNo,
                        Note = s.Reason.Reason,
                        UserEmail = s.Order.UserFullName,
                        UserNote = s.Order.UserNote,
                        BargainType = s.Order.BargainType
                    })
                    .Where(x => x.ContractCodeNo == 0)
                    .OrderBy(x => x.LastUpdate);

            totalRecord = query.Count();
            return query.ToListAsync();
        }

        /// <summary>
        /// Lây đơn hàng chưa đủ kiện về kho
        /// </summary>
        /// <param name="totalRecord"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="systemId"></param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="userId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public Task<List<OrderRiskResult>> GetOrderNotEnoughInventoryExcel(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState)
        {
            var query =
                Db.Orders.Where(x =>
                        x.UnsignName.Contains(keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && (x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                    && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                    && (userState.Type != 0 || x.UserId == userState.UserId)
                    )
                    .GroupJoin(
                        Db.OrderPackages.Where(
                            x => !x.IsDelete
                        ),
                        order => order.Id,
                        packages => packages.OrderId,
                        (o, p) => new { Order = o, Packages = p }
                    ).Join(
                     Db.OrderHistories.Where(
                            x => x.Status == (byte)OrderStatus.OrderSuccess
                        ),
                        order => order.Order.Id,
                        history => history.OrderId,
                        (o, h) => new { o.Order, o.Packages, h }
                    )
                    .GroupJoin(
                         //Db.OrderReasons.Where(
                         //    x => x.Type == (byte)OrderReasonType.NotEnoughInventory
                         //),
                         Db.OrderReasons,
                        group => group.Order.Id,
                        reason => reason.OrderId,
                        (g, r) => new { Order = g.Order, Packages = g.Packages, Reasons = r, History = g.h }
                    )
                    .Select(s => new OrderRiskResult()
                    {
                        Id = s.Order.Id,
                        Code = s.Order.Code,
                        Type = s.Order.Type,
                        Status = s.Order.Status,
                        SystemId = s.Order.SystemId,
                        SystemName = s.Order.SystemName,
                        Created = s.Order.Created,
                        LastUpdate = s.History.CreateDate,
                        CustomerId = s.Order.CustomerId,
                        CustomerName = s.Order.CustomerName,
                        CustomerAddress = s.Order.CustomerAddress,
                        CustomerEmail = s.Order.CustomerEmail,
                        CustomerPhone = s.Order.CustomerPhone,
                        PackageNo = s.Packages.Count(),
                        PackageNoInStock = s.Packages.Count(x => x.Status > (byte)OrderPackageStatus.ShopDelivery),
                        Packages = s.Packages.ToList(),
                        ContractCodeNo = s.Packages.Select(x => x.Code).Distinct().Count(),
                        Note = s.Reasons.FirstOrDefault() == null ? "" : s.Reasons.FirstOrDefault().ReasonId + ". " + s.Reasons.FirstOrDefault().Reason,
                        UserEmail = s.Order.UserFullName,
                        UserNote = s.Order.UserNote,
                        BargainType = s.Order.BargainType
                    })
                    .Where(x => x.PackageNo > x.PackageNoInStock || (x.PackageNoInStock == 0 && x.PackageNo > 0))
                    .OrderBy(x => x.LastUpdate);

            totalRecord = query.Count();
            return query.ToListAsync();
        }

        public Task<List<OrderRiskResult>> GetOrderNotEnoughInventory4DayExcel(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, UserState userState)
        {
            var query =
                Db.Orders.Where(x =>
                        x.UnsignName.Contains(keyword)
                        && (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && (x.Status >= (byte)OrderStatus.OrderSuccess && x.Status != (byte)OrderStatus.Cancel)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                    && (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                    && (userState.Type != 0 || x.UserId == userState.UserId)
                    )
                    .GroupJoin(
                        Db.OrderPackages.Where(
                            x => !x.IsDelete
                        ),
                        order => order.Id,
                        packages => packages.OrderId,
                        (o, p) => new { Order = o, Packages = p }
                    ).Join(
                     Db.OrderHistories.Where(
                            x => x.Status == (byte)OrderStatus.OrderSuccess
                        ),
                        order => order.Order.Id,
                        history => history.OrderId,
                        (o, h) => new { o.Order, o.Packages, h }
                    ).Join(
                        Db.OrderReasons.Where(
                            x => x.Type == (byte)OrderReasonType.NotEnoughInventory
                        ),
                        group => group.Order.Id,
                        reason => reason.OrderId,
                        (g, r) => new { Order = g.Order, Packages = g.Packages, Reason = r, History = g.h }
                    )
                    .Select(s => new OrderRiskResult()
                    {
                        Id = s.Order.Id,
                        Code = s.Order.Code,
                        Type = s.Order.Type,
                        Status = s.Order.Status,
                        SystemId = s.Order.SystemId,
                        SystemName = s.Order.SystemName,
                        Created = s.Order.Created,
                        LastUpdate = s.History.CreateDate,
                        CustomerId = s.Order.CustomerId,
                        CustomerName = s.Order.CustomerName,
                        CustomerAddress = s.Order.CustomerAddress,
                        CustomerEmail = s.Order.CustomerEmail,
                        CustomerPhone = s.Order.CustomerPhone,
                        PackageNo = s.Packages.Count(),
                        PackageNoInStock = s.Packages.Count(x => x.Status > (byte)OrderPackageStatus.ShopDelivery),
                        Packages = s.Packages.ToList(),
                        ContractCodeNo = s.Packages.Select(x => x.Code).Distinct().Count(),
                        Note = s.Reason.Reason,
                        UserEmail = s.Order.UserFullName,
                        UserNote = s.Order.UserNote,
                        BargainType = s.Order.BargainType
                    })
                    .Where(x => x.PackageNo > x.PackageNoInStock || (x.PackageNoInStock == 0 && x.PackageNo > 0))
                    .OrderBy(x => x.LastUpdate);

            totalRecord = query.Count();
            return query.ToListAsync();
        }

        /// <summary>
        /// Quá số ngày mà chưa có mã vận đơn
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public List<OrderRiskResult> OrderNoCodeOfLadingOverDays(int day)
        {
            var timeNow = DateTime.Now.AddDays(day * -1);

            return Db.Orders.Where(x =>
                        x.Status >= (byte)OrderStatus.OrderSuccess
                         && x.Status != (byte)OrderStatus.Cancel
                        && !x.IsDelete
                    )
                    .Join(
                     Db.OrderHistories.Where(
                            x => x.Status == (byte)OrderStatus.OrderSuccess
                            && x.CreateDate <= timeNow
                        ),
                        order => order.Id,
                        history => history.OrderId,
                        (o, h) => new OrderRiskResult()
                        {
                            Id = o.Id,
                            Code = o.Code,
                            Type = o.Type,
                            Status = o.Status,
                            SystemId = o.SystemId,
                            SystemName = o.SystemName,
                            Created = o.Created,
                            LastUpdate = h.CreateDate,
                            CustomerId = o.CustomerId,
                            CustomerName = o.CustomerName,
                            CustomerAddress = o.CustomerAddress,
                            CustomerEmail = o.CustomerEmail,
                            CustomerPhone = o.CustomerPhone,
                            PackageNo = o.PacketNumber,
                            PackageNoInStock = o.PackageNoInStock,
                            ContractCodeNo = o.PackageNo,
                            Note = "",
                            UserId = o.UserId,
                            UserEmail = o.UserFullName,
                            UserNote = o.UserNote
                        })
                    .Where(x => x.ContractCodeNo == 0)
                    .OrderBy(x => x.LastUpdate).ToList();
        }

        /// <summary>
        /// Quá số ngày mà chưa đủ kiện về kho
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public List<OrderRiskResult> OrderNotEnoughInventoryOverDays(int day)
        {
            var timeNow = DateTime.Now.AddDays(day * -1);

            return Db.Orders.Where(x =>
                        x.Status >= (byte)OrderStatus.OrderSuccess
                        && x.Status != (byte)OrderStatus.Cancel
                        && !x.IsDelete
                    )
                    .GroupJoin(
                        Db.OrderPackages.Where(
                            x => !x.IsDelete
                        ),
                        order => order.Id,
                        packages => packages.OrderId,
                        (o, p) => new { Order = o, Packages = p }
                    ).Join(
                     Db.OrderHistories.Where(
                            x => x.Status == (byte)OrderStatus.OrderSuccess
                            && x.CreateDate <= timeNow
                        ),
                        order => order.Order.Id,
                        history => history.OrderId,
                        (o, h) => new { o.Order, o.Packages, History = h }
                    )
                    .Select(s => new OrderRiskResult()
                    {
                        Id = s.Order.Id,
                        Code = s.Order.Code,
                        Type = s.Order.Type,
                        Status = s.Order.Status,
                        SystemId = s.Order.SystemId,
                        SystemName = s.Order.SystemName,
                        Created = s.Order.Created,
                        LastUpdate = s.Packages.OrderByDescending(x => x.Created).FirstOrDefault().Created,
                        CustomerId = s.Order.CustomerId,
                        CustomerName = s.Order.CustomerName,
                        CustomerAddress = s.Order.CustomerAddress,
                        CustomerEmail = s.Order.CustomerEmail,
                        CustomerPhone = s.Order.CustomerPhone,
                        PackageNo = s.Packages.Count(),
                        PackageNoInStock = s.Packages.Count(x => x.Status > (byte)OrderPackageStatus.ShopDelivery),
                        PackageCodes = "",
                        ContractCodeNo = 0,
                        Note = "",
                        UserId = s.Order.UserId,
                        UserEmail = s.Order.UserFullName,
                        UserNote = s.Order.UserNote
                    })
                    .Where(x => x.PackageNo > x.PackageNoInStock || (x.PackageNoInStock == 0 && x.PackageNo > 0))
                    .OrderBy(x => x.LastUpdate).ToList();
        }

        public Task<List<LadingCodeResult>> GetOrderLadingCode(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            var query =
                Db.Orders.Where(x =>
                        (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && (x.Status >= (byte)OrderStatus.OrderSuccess)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                    //&& (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                    //&& (userState.Type != 0 || x.UserId == userState.UserId)
                    )
                    .GroupJoin(
                        Db.OrderPackages.Where(
                            x => !x.IsDelete
                            && (status == -1 || x.Status == status)
                            && x.UnsignedText.Contains(keyword)
                        ),
                        order => order.Id,
                        packages => packages.OrderId,
                        (o, p) => new { Order = o, Packages = p }
                    ).Select(s => new LadingCodeResult()
                    {
                        Id = s.Order.Id,
                        Code = s.Order.Code,
                        Type = s.Order.Type,
                        Status = s.Order.Status,
                        SystemId = s.Order.SystemId,
                        SystemName = s.Order.SystemName,
                        Created = s.Order.Created,
                        LastUpdate = s.Packages.OrderByDescending(x => x.Created).FirstOrDefault().Created,
                        CustomerId = s.Order.CustomerId,
                        CustomerName = s.Order.CustomerName,
                        CustomerAddress = s.Order.CustomerAddress,
                        CustomerEmail = s.Order.CustomerEmail,
                        CustomerPhone = s.Order.CustomerPhone,
                        Packages = s.Packages.ToList(),
                        PackageNo = s.Packages.Count(),
                        PackageNoInStock = s.Packages.Count(x => x.Status > (byte)OrderPackageStatus.ShopDelivery),
                        ContractCodeNo = 0,
                        Note = s.Order.Note,
                        UserEmail = s.Order.UserFullName,
                        UserNote = s.Order.UserNote
                    })
                    .Where(x => x.PackageNo > 0)
                    .OrderByDescending(x => x.LastUpdate);
            if (checkExactCode)
            {
                keyword = MyCommon.RemoveCode(keyword);
                query =
                Db.Orders.Where(x =>
                        (systemId == -1 || x.SystemId == systemId)
                        && (customerId == null || x.CustomerId == customerId)
                        && (x.Status >= (byte)OrderStatus.OrderSuccess)
                        && !x.IsDelete
                        && (userId == null || x.UserId == userId)
                        && x.Code == keyword
                    //&& (userState.Type == 0 || (x.OfficeIdPath == userState.OfficeIdPath || x.OfficeIdPath.StartsWith(userState.OfficeIdPath + ".")))
                    //&& (userState.Type != 0 || x.UserId == userState.UserId)
                    )
                    .GroupJoin(
                        Db.OrderPackages.Where(
                            x => !x.IsDelete
                            && (status == -1 || x.Status == status)
                        ),
                        order => order.Id,
                        packages => packages.OrderId,
                        (o, p) => new { Order = o, Packages = p }
                    ).Select(s => new LadingCodeResult()
                    {
                        Id = s.Order.Id,
                        Code = s.Order.Code,
                        Type = s.Order.Type,
                        Status = s.Order.Status,
                        SystemId = s.Order.SystemId,
                        SystemName = s.Order.SystemName,
                        Created = s.Order.Created,
                        LastUpdate = s.Packages.OrderByDescending(x => x.Created).FirstOrDefault().Created,
                        CustomerId = s.Order.CustomerId,
                        CustomerName = s.Order.CustomerName,
                        CustomerAddress = s.Order.CustomerAddress,
                        CustomerEmail = s.Order.CustomerEmail,
                        CustomerPhone = s.Order.CustomerPhone,
                        Packages = s.Packages.ToList(),
                        PackageNo = s.Packages.Count(),
                        PackageNoInStock = s.Packages.Count(x => x.Status > (byte)OrderPackageStatus.ShopDelivery),
                        ContractCodeNo = 0,
                        Note = s.Order.Note,
                        UserEmail = s.Order.UserFullName,
                        UserNote = s.Order.UserNote
                    })
                    .Where(x => x.PackageNo > 0)
                    .OrderByDescending(x => x.LastUpdate);
            }

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        //Thong ke tong tien mac ca trong ngày
        public List<CustomerUser> GetTotalMoneyReportDay(List<UserOfficeResult> list, DateTime? startDay, byte status)
        {
            startDay = startDay ?? DateTime.Now;

            var listOrder = Db.Orders.Where(x => !x.IsDelete && x.Status != (byte)OrderStatus.Cancel && x.Type == (byte)OrderType.Order)
                .Join(Db.OrderHistories.Where(x =>
                        x.CreateDate.Year == startDay.Value.Year
                        && x.CreateDate.Month == startDay.Value.Month
                        && x.CreateDate.Day == startDay.Value.Day && x.Status == status),
                    order => order.Id,
                    orderHistory => orderHistory.OrderId,
                    (o, h) => new { o, h }).Select(s => s.o).ToList();

            var query = new List<CustomerUser>();
            foreach (var item in list)
            {
                var sum = listOrder.Where(x => x.UserId == item.Id).Sum(x => x.PriceBargain ?? 0);
                var count = listOrder.Count(x => x.UserId == item.Id);

                query.Add(new CustomerUser()
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    Birthday = item.Birthday,
                    Email = item.Email,
                    Phone = item.Phone,
                    Created = item.Created,
                    StartDate = item.StartDate,
                    TypeName = item.TypeName,
                    Gender = item.Gender,
                    TotalCusstomer = count,
                    TotalMoney = sum
                });
            }
            return query.ToList();
        }

        //Thong ke tong tien mac ca tat ca ngay
        public List<CustomerUser> GetTotalMoneyReportAllDay(List<UserOfficeResult> list, DateTime? startDay, DateTime? endDay, byte status)
        {
            var start = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToString() : startDay.ToString());
            var end = DateTime.Parse(string.IsNullOrEmpty(endDay.ToString()) ? DateTime.Now.ToString() : endDay.ToString());
            start = GetStartOfDay(start);
            end = GetEndOfDay(end);

            var listOrder = Db.Orders.Where(x => !x.IsDelete && x.Status != (byte)OrderStatus.Cancel && x.Type == (byte)OrderType.Order)
                .Join(Db.OrderHistories.Where(x =>
                        x.CreateDate >= start
                        && x.CreateDate <= end
                        && x.Status == status),
                    order => order.Id,
                    orderHistory => orderHistory.OrderId,
                    (o, h) => new { o, h }).Select(s => s.o).ToList();

            var query = new List<CustomerUser>();
            foreach (var item in list)
            {

                var sum = listOrder.Where(x => x.UserId == item.Id).Sum(x => x.PriceBargain ?? 0);
                var count = listOrder.Count(x => x.UserId == item.Id);

                query.Add(new CustomerUser()
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    Birthday = item.Birthday,
                    Email = item.Email,
                    Phone = item.Phone,
                    Created = item.Created,
                    StartDate = item.StartDate,
                    TypeName = item.TypeName,
                    Gender = item.Gender,
                    TotalCusstomer = count,
                    TotalMoney = sum
                });
            }
            return query.ToList();
        }

        //Thong ke tong tien thanh toan với shop trong ngày
        public List<CustomerUser> GetTotalPriceBargainReportDay(List<UserOfficeResult> list, DateTime? startDay)
        {
            startDay = startDay ?? DateTime.Now;

            var listOrder = Db.Orders.Where(x => !x.IsDelete

                                                && x.Status != (byte)OrderStatus.Cancel && x.Type == (byte)OrderType.Order)
                                                .Join(Db.OrderHistories.Where(x =>
                                                        x.CreateDate.Year == startDay.Value.Year
                                                        && x.CreateDate.Month == startDay.Value.Month
                                                        && x.CreateDate.Day == startDay.Value.Day
                                                        && x.Status == (byte)OrderStatus.OrderSuccess),
                                                    order => order.Id,
                                                    orderHistory => orderHistory.OrderId,
                                                    (o, h) => new { o, h }).Select(s => s.o).ToList();

            var query = new List<CustomerUser>();
            foreach (var item in list)
            {
                var sum1 = listOrder.Where(x => x.UserId == item.Id).Sum(x => x.PaidShop ?? 0);
                var sum2 = listOrder.Where(x => x.UserId == item.Id).Sum(x => x.FeeShipBargain ?? 0);
                var count = listOrder.Count(x => x.UserId == item.Id);

                query.Add(new CustomerUser()
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    Birthday = item.Birthday,
                    Email = item.Email,
                    Phone = item.Phone,
                    Created = item.Created,
                    StartDate = item.StartDate,
                    TypeName = item.TypeName,
                    Gender = item.Gender,
                    TotalCusstomer = count,
                    TotalMoney = sum1 + sum2
                });
            }
           
            return query.ToList();
        }

        //Thong ke tong tien deposit thanh toan với shop trong ngày
        public List<Order> GetTotalPriceDepositReportDay(DateTime? startDay)
        {
            startDay = startDay ?? DateTime.Now;

            var listOrder = Db.Orders.Where(x => !x.IsDelete

                                                && x.Status != (byte)DepositStatus.Cancel && x.Type == (byte)OrderType.Deposit)
                                                .Join(Db.OrderHistories.Where(x =>
                                                        x.CreateDate.Year == startDay.Value.Year
                                                        && x.CreateDate.Month == startDay.Value.Month
                                                        && x.CreateDate.Day == startDay.Value.Day
                                                        && x.Status == (byte)DepositStatus.Finish),
                                                    order => order.Id,
                                                    orderHistory => orderHistory.OrderId,
                                                    (o, h) => new { o, h }).Select(s => s.o).ToList();

            return listOrder.ToList();
        }

        //Thong ke tong tien thanh toan với shop trong tất cả các ngày
        public List<CustomerUser> GetTotalPriceBargainReportAllDay(List<UserOfficeResult> list, DateTime? startDay, DateTime? endDay)
        {
            var start = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToString() : startDay.ToString());
            var end = DateTime.Parse(string.IsNullOrEmpty(endDay.ToString()) ? DateTime.Now.ToString() : endDay.ToString());
            start = GetStartOfDay(start);
            end = GetEndOfDay(end);
            var listOrder = Db.Orders.Where(x => !x.IsDelete
                                                 && x.Status != (byte)OrderStatus.Cancel && x.Type == (byte)OrderType.Order
                                                )
                                                .Join(Db.OrderHistories.Where(x =>
                                                        x.CreateDate >= start
                                                        && x.CreateDate <= end
                                                        && x.Status == (byte)OrderStatus.OrderSuccess
                                                        ),
                                                    order => order.Id,
                                                    orderHistory => orderHistory.OrderId,
                                                    (o, h) => new { o, h }).Select(s => s.o).ToList();

            var query = new List<CustomerUser>();
            foreach (var item in list)
            {
                var sum1 = listOrder.Where(x => x.UserId == item.Id).Sum(x => x.PaidShop ?? 0);
                var sum2 = listOrder.Where(x => x.UserId == item.Id).Sum(x => x.FeeShipBargain ?? 0);
                var count = listOrder.Count(x => x.UserId == item.Id);

                query.Add(new CustomerUser()
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    Birthday = item.Birthday,
                    Email = item.Email,
                    Phone = item.Phone,
                    Created = item.Created,
                    StartDate = item.StartDate,
                    TypeName = item.TypeName,
                    Gender = item.Gender,
                    TotalCusstomer = count,
                    TotalMoney = sum1 + sum2
                });
            }
            return query.ToList();
        }

        //Thong ke tong tien deposit trong tất cả các ngày
        public List<Order> GetTotalPriceDepositReportAllDay(DateTime? startDay, DateTime? endDay)
        {
            var start = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToString() : startDay.ToString());
            var end = DateTime.Parse(string.IsNullOrEmpty(endDay.ToString()) ? DateTime.Now.ToString() : endDay.ToString());
            start = GetStartOfDay(start);
            end = GetEndOfDay(end);
            var listOrder = Db.Orders.Where(x => !x.IsDelete
                                                 && x.Status != (byte)DepositStatus.Cancel && x.Type == (byte)OrderType.Deposit
                                                )
                                                .Join(Db.OrderHistories.Where(x =>
                                                        x.CreateDate >= start
                                                        && x.CreateDate <= end
                                                        && x.Status == (byte)DepositStatus.Finish
                                                        ),
                                                    order => order.Id,
                                                    orderHistory => orderHistory.OrderId,
                                                    (o, h) => new { o, h }).Select(s => s.o).ToList();

          
            return listOrder;
        }


        //Tình hình xử lý đơn hàng theo thời gian
        public List<ProfitDay> GetTotalProfitReportAllDay(DateTime? startDay, DateTime? endDay, UserState userstate)
        {
            var start = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToShortDateString() : startDay.ToString());
            var end = DateTime.Parse(string.IsNullOrEmpty(endDay.ToString()) ? DateTime.Now.ToShortDateString() : endDay.ToString());
            start = GetStartOfDay(start);
            end = GetEndOfDay(end);
            var listOrder = Db.Orders.Where(x => !x.IsDelete
                                                && x.Status != (byte)OrderStatus.Cancel && x.Type == (byte)OrderType.Order
                                                //&& (x.OfficeIdPath == userstate.OfficeIdPath || x.OfficeIdPath.StartsWith(userstate.OfficeIdPath + ".")))
                                                && (userstate.Type > 0 || x.UserId == userstate.UserId)
                                                && (userstate.Type == 0 || (x.OfficeIdPath == userstate.OfficeIdPath
                                                    || x.OfficeIdPath.StartsWith(userstate.OfficeIdPath + "."))))
                                                    //.Join(Db.OrderHistories.Where(x =>
                                                    //        x.CreateDate >= start
                                                    //        && x.CreateDate <= end
                                                    //        ),
                                                    //    order => order.Id,
                                                    //    orderHistory => orderHistory.OrderId,
                                                    //    (o, h) => new { o, h }).Select(s => s.o)
                                                    .ToList();

            var query = new List<ProfitDay>();
            var list = new List<string>();
            DateTime tmpDate = start;
            do
            {
                list.Add(tmpDate.ToShortDateString());
                tmpDate = tmpDate.AddDays(1);
            } while (tmpDate <= end);

            foreach (var item in list)
            {
                var bargain = listOrder.Where(x => x.Created.ToShortDateString() == item).Sum(x => x.PriceBargain ?? 0);
                var sum1 = listOrder.Where(x => x.Created.ToShortDateString() == item).Sum(x => x.PaidShop ?? 0);
                var sum2 = listOrder.Where(x => x.Created.ToShortDateString() == item).Sum(x => x.FeeShipBargain ?? 0);
                var count = listOrder.Where(x => x.Created.ToShortDateString() == item).Count();

                query.Add(new ProfitDay()
                {
                    Created = item,
                    TotalOrder = count,
                    TotalBargain = bargain,
                    TotalMoney = sum1 + sum2
                });
            }
            return query.ToList();
        }

        public List<ProfitDay> GetTotalProfitBargainReportAllDay(DateTime? startDay, DateTime? endDay, UserState userstate, byte status)
        {
            var start = DateTime.Parse(string.IsNullOrEmpty(startDay.ToString()) ? DateTime.Now.ToString() : startDay.ToString());
            var end = DateTime.Parse(string.IsNullOrEmpty(endDay.ToString()) ? DateTime.Now.ToString() : endDay.ToString());
            start = GetStartOfDay(start);
            end = GetEndOfDay(end);
            var listOrder = Db.Orders.Where(x => !x.IsDelete && x.Status != (byte)OrderStatus.Cancel && x.Type == (byte)OrderType.Order
                                                && (userstate.Type > 0 || x.UserId == userstate.UserId)
                                                && (userstate.Type == 0 || (x.OfficeIdPath == userstate.OfficeIdPath
                                                    || x.OfficeIdPath.StartsWith(userstate.OfficeIdPath + "."))))
                                                .Join(Db.OrderHistories.Where(x =>
                                                        x.CreateDate >= start
                                                        && x.CreateDate <= end
                                                        && (x.Status == (byte)OrderStatus.OrderSuccess /*|| x.Status == (byte)OrderStatus.Finish*/)),
                                                    order => order.Id,
                                                    orderHistory => orderHistory.OrderId,
                                                    (o, h) => new { o, h }).Select(s => s.o).ToList();

            var query = new List<ProfitDay>();
            var list = new List<string>();
            DateTime tmpDate = start;
            do
            {
                list.Add(tmpDate.ToShortDateString());
                tmpDate = tmpDate.AddDays(1);
            } while (tmpDate <= end);

            foreach (var item in list)
            {
                var sum1 = listOrder.Where(x => x.Created.ToShortDateString() == item).Sum(x => x.PaidShop ?? 0);
                var sum2 = listOrder.Where(x => x.Created.ToShortDateString() == item).Sum(x => x.FeeShipBargain ?? 0);
                var bargain = listOrder.Where(x => x.Created.ToShortDateString() == item).Sum(x => x.PriceBargain ?? 0);
                var count = listOrder.Where(x => x.Created.ToShortDateString() == item).Count();

                query.Add(new ProfitDay()
                {
                    Created = item,
                    TotalOrder = count,
                    TotalBargain = bargain,
                    TotalMoney = sum1 + sum2
                });
            }
            return query.ToList();
        }

        public List<OrderSuccessResult> GetOrderSuccess(out long totalRecord, int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            var query =
               Db.Orders.Where(x =>
                       (systemId == -1 || x.SystemId == systemId)
                       && (customerId == null || x.CustomerId == customerId)
                       && ((x.Type != (byte)OrderType.Deposit && x.Status == (byte)OrderStatus.GoingDelivery) || (x.Type == (byte)OrderType.Deposit && x.Status == (byte)DepositStatus.GoingDelivery))
                       && !x.IsDelete
                       && x.UnsignName.Contains(keyword)
                       && x.Debt > -100 && x.Debt < 100
                       && x.PackageNo == x.PackageNoDelivered
                       && (userId == null || x.UserId == userId)
                       && x.LastDeliveryTime != null
                   )
                   .GroupJoin(
                       Db.Complains.Where(
                           x => !x.IsDelete
                       ),
                       order => order.Id,
                       complains => complains.OrderId,
                       (o, c) => new { Order = o, Complains = c }
                   ).Select(s => new OrderSuccessResult()
                   {
                       Id = s.Order.Id,
                       Code = s.Order.Code,
                       Type = s.Order.Type,
                       Status = s.Order.Status,
                       SystemId = s.Order.SystemId,
                       SystemName = s.Order.SystemName,
                       Created = s.Order.LastDeliveryTime.Value,
                       LastUpdate = s.Order.LastUpdate,
                       CustomerId = s.Order.CustomerId,
                       CustomerName = s.Order.CustomerName,
                       CustomerAddress = s.Order.CustomerAddress,
                       CustomerEmail = s.Order.CustomerEmail,
                       CustomerPhone = s.Order.CustomerPhone,
                       UserId = s.Order.UserId,
                       UserEmail = s.Order.UserFullName,
                       ComplainCount = s.Complains.Count()
                   })
                   .OrderBy(x => x.Created);

            if (checkExactCode)
            {
                keyword = MyCommon.RemoveCode(keyword);
                query =
               Db.Orders.Where(x =>
                       (systemId == -1 || x.SystemId == systemId)
                       && (customerId == null || x.CustomerId == customerId)
                       && ((x.Type != (byte)OrderType.Deposit && x.Status == (byte)OrderStatus.GoingDelivery) || (x.Type == (byte)OrderType.Deposit && x.Status == (byte)DepositStatus.GoingDelivery))
                       && !x.IsDelete
                       && x.Code == keyword
                       && x.Debt > -100 && x.Debt < 100
                       && x.PackageNo == x.PackageNoDelivered
                       && (userId == null || x.UserId == userId)
                       && x.LastDeliveryTime != null
                   )
                   .GroupJoin(
                       Db.Complains.Where(
                           x => !x.IsDelete
                       ),
                       order => order.Id,
                       complains => complains.OrderId,
                       (o, c) => new { Order = o, Complains = c }
                   ).Select(s => new OrderSuccessResult()
                   {
                       Id = s.Order.Id,
                       Code = s.Order.Code,
                       Type = s.Order.Type,
                       Status = s.Order.Status,
                       SystemId = s.Order.SystemId,
                       SystemName = s.Order.SystemName,
                       Created = s.Order.LastDeliveryTime.Value,
                       LastUpdate = s.Order.LastUpdate,
                       CustomerId = s.Order.CustomerId,
                       CustomerName = s.Order.CustomerName,
                       CustomerAddress = s.Order.CustomerAddress,
                       CustomerEmail = s.Order.CustomerEmail,
                       CustomerPhone = s.Order.CustomerPhone,
                       UserId = s.Order.UserId,
                       UserEmail = s.Order.UserFullName,
                       ComplainCount = s.Complains.Count()
                   })
                   .OrderBy(x => x.Created);
            }

            totalRecord = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<Order> GetOrderReportRevenue(DateTime startDay, DateTime endDay)
        {
            //lấy danh sách hoàn thành đơn order
            var listOrder =
                Db.Orders.Where(
                        x => !x.IsDelete && x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.Finish)
                    .Join(Db.OrderHistories.Where(x => x.Status == (byte)OrderStatus.Finish && startDay <= x.CreateDate && x.CreateDate <= endDay),
                        order => order.Id,
                        orderHistory => orderHistory.OrderId,
                        (o, h) => new { o, h }
                    ).Select(s => s.o).ToList();

            //lấy danh sách hoàn thành đơn ký gửi
            var listDeposit = Db.Orders.Where(
                        x => !x.IsDelete && x.Type == (byte)OrderType.Deposit && x.Status == (byte)DepositStatus.Finish)
                    .Join(Db.OrderHistories.Where(x => x.Status == (byte)DepositStatus.Finish && startDay <= x.CreateDate && x.CreateDate <= endDay),
                        order => order.Id,
                        orderHistory => orderHistory.OrderId,
                        (o, h) => new { o, h }
                    ).Select(s => s.o).ToList();

            //ghép 2 loại dữ liệu
            listOrder.AddRange(listDeposit);

            return listOrder;
        }

        //Thống kê tình hình đơn hàng theo ngày.
        public List<Order> ReportOrderOfDay(DateTime startDay, DateTime endDay)
        {
            var listOrder = Db.Orders.Where(x => !x.IsDelete && x.Type == (byte) OrderType.Order)
                .Join(Db.OrderHistories.Where(x => startDay <= x.CreateDate && x.CreateDate <= endDay),
                    order => order.Id,
                    orderHistory => orderHistory.OrderId,
                    (o, h) => o
                ).Distinct().ToList();
            return listOrder;
        }
    }
}