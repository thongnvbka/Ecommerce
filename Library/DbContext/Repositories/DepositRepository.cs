using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using Common.Items;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Library.ViewModels.Items;
using Common.Emums;
using System;

namespace Library.DbContext.Repositories
{
    public class DepositRepository : Repository<Order>
    {
        public DepositRepository(ProjectXContext context) : base(context)
        {
        }


        #region By Sql
        public DepositModel GetAll(PageItem pageInfor, SearchInfor searchInfor)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_deposit_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("PageIndex", pageInfor.PageIndex == 0 ? 1 : pageInfor.PageIndex));
                cmd.Parameters.Add(new SqlParameter("PageSize", pageInfor.PageIndex == 0 ? 25 : pageInfor.PageSize));
                cmd.Parameters.Add(new SqlParameter("Keyword", searchInfor.Keyword));
                cmd.Parameters.Add(new SqlParameter("StartDate", searchInfor.StartDate));
                cmd.Parameters.Add(new SqlParameter("FinishDate", searchInfor.FinishDate));
                cmd.Parameters.Add(new SqlParameter("AllTime", searchInfor.AllTime));
                cmd.Parameters.Add(new SqlParameter("Status", searchInfor.Status));
                cmd.Parameters.Add(new SqlParameter("SystemId", searchInfor.SystemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", searchInfor.CustomerId));
                cmd.Parameters.Add(new SqlParameter("IsSearch", searchInfor.IsSearch));
                cmd.Parameters.Add(new SqlParameter("StartStatus", searchInfor.StartStatus));
                cmd.Parameters.Add(new SqlParameter("EndStatus", searchInfor.EndStatus));
                cmd.Parameters.Add(new SqlParameter("OrderType", searchInfor.OrderType));
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
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<DepositItem>(reader).ToList();
                    reader.NextResult();
                    var tmpStatus = ((IObjectContextAdapter)context).ObjectContext.Translate<DepositStatusItem>(reader).ToList().FirstOrDefault();
                    reader.Close();
                    pageInfor.CurrentPage = pageInfor.PageIndex;
                    pageInfor.Total = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());

                    var model = new DepositModel()
                    {
                        Page = pageInfor,
                        Search = searchInfor,
                        ListItems = tmpList,
                        DepositStatusItem = tmpStatus
                    };
                    return model;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }


        public int PaymentBalance(int customerId, decimal advanceMoney)
        {
            var result = 0;
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_deposit_paymentBalance";
                cmd.CommandType = CommandType.StoredProcedure;
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
        public DepositDetailModel GetDetail(int id, byte commentType)
        {
            using (var context = new ProjectXContext())
            {
                byte orderExchangeType = (byte)OrderExchangeType.Product;
                byte orderExchangeMode = (byte)OrderExchangeMode.Export;
                byte orderExchangeStatus = (byte)OrderExchangeStatus.Approved;
                byte oeOrderType = (byte)OrderExchangeOrderType.Order;
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_depositdetail_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("id", id));
                cmd.Parameters.Add(new SqlParameter("commentOrderType", commentType));
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
                    var tmpDetailItem = ((IObjectContextAdapter)context).ObjectContext.Translate<DepositViewItem>(reader).ToList().FirstOrDefault();
                    reader.NextResult();
                    var tmpDetail = ((IObjectContextAdapter)context).ObjectContext.Translate<DepositDetailViewItem>(reader).ToList();
                    reader.NextResult();
                    var tmpOrderCommentItem = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderComment>(reader).ToList();
                    reader.NextResult();
                    var tmpOrderServiceItem = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderServiceItem>(reader).ToList();
                    reader.Close();

                    var model = new DepositDetailModel()
                    {
                        DepositViewItem = tmpDetailItem,
                        ListDetail = tmpDetail,
                        ListOrderComment = tmpOrderCommentItem,
                        ListOrderService = tmpOrderServiceItem
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
        #region By Linq
        public DepositModel GetAllByLinq(PageItem pageInfor, SearchInfor searchInfor)
        {
            var query = Db.Orders.Where(
                    x => (searchInfor.Status == -1 || (x.Status >= searchInfor.StartStatus && x.Status <= searchInfor.EndStatus))
                    && x.Code.Contains(searchInfor.Keyword)
                    && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.Created >= searchInfor.StartDate)
                    && (searchInfor.FinishDate == null || x.Created <= searchInfor.FinishDate)))
                    && !x.IsDelete
                    && x.CustomerId == searchInfor.CustomerId
                    && x.SystemId == searchInfor.SystemId
                    && x.Type == searchInfor.OrderType
                ).Select(m => new DepositItem()
                {
                    ROW = 1,
                    Id = m.Id,
                    code = m.Code,
                    CreateDate = m.Created,
                    PacketNumber = (m.PacketNumber != null ? (int)m.PacketNumber : 0),
                    ProvisionalMoney = (m.ProvisionalMoney != null ? (int)m.ProvisionalMoney : 0),
                    TotalWeight = (Db.DepositDetails.Where(s => s.DepositId == m.Id).Select(s => s.Weight).DefaultIfEmpty(0).Sum()),
                    //IsComplain = (Db.Complains.Where(s => s.OrderId == m.Id).FirstOrDefault() != null ? 1 : 0),
                    IsComplain = 0,
                    Status = (byte)m.Status
                });
            var queryCount = Db.Orders.Where(
                    x => x.Code.Contains(searchInfor.Keyword)
                    && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.Created >= searchInfor.StartDate)
                    && (searchInfor.FinishDate == null || x.Created <= searchInfor.FinishDate)))
                    && !x.IsDelete
                    && x.CustomerId == searchInfor.CustomerId
                    && x.SystemId == searchInfor.SystemId
                    && x.Type == searchInfor.OrderType
                ).Select(m => new ComplainItem()
                {
                    Status = (byte)m.Status,
                });
            pageInfor.CurrentPage = pageInfor.PageIndex;
            pageInfor.Total = query.Count();
            var tmpList = query.OrderByDescending(x => new { x.CreateDate })
                    .Skip((pageInfor.CurrentPage - 1) * pageInfor.PageSize)
                    .Take(pageInfor.PageSize)
                    .ToList();
            var tmpStatus = new DepositStatusItem()
            {
                choBaoGia = queryCount.Where(m => m.Status >= searchInfor.StartStatus1 && m.Status <= searchInfor.EndStatus1).Count(),
                choXuLy = queryCount.Where(m => m.Status >= searchInfor.StartStatus2 && m.Status <= searchInfor.EndStatus2).Count(),
                choKetDon = queryCount.Where(m => m.Status >= searchInfor.StartStatus3 && m.Status <= searchInfor.EndStatus3).Count(),
                choXuatKho = queryCount.Where(m => m.Status >= searchInfor.StartStatus4 && m.Status <= searchInfor.EndStatus4).Count(),
                hangTrongKho = queryCount.Where(m => m.Status >= searchInfor.StartStatus5 && m.Status <= searchInfor.EndStatus5).Count(),
                dangVanChuyen = queryCount.Where(m => m.Status >= searchInfor.StartStatus6 && m.Status <= searchInfor.EndStatus6).Count(),
                choGiaoHang = queryCount.Where(m => m.Status >= searchInfor.StartStatus7 && m.Status <= searchInfor.EndStatus7).Count(),
                daGiaoHang = queryCount.Where(m => m.Status >= searchInfor.StartStatus8 && m.Status <= searchInfor.EndStatus8).Count(),
                hoanThanh = queryCount.Where(m => m.Status >= searchInfor.StartStatus9 && m.Status <= searchInfor.EndStatus9).Count(),
                huy = queryCount.Where(m => m.Status >= searchInfor.StartStatus10 && m.Status <= searchInfor.EndStatus10).Count(),
            };
            var model = new DepositModel()
            {
                Page = pageInfor,
                Search = searchInfor,
                ListItems = tmpList,
                DepositStatusItem = tmpStatus
            };
            return model;
        }

        public DepositDetailModel GetDetailByLinq(int id, byte commentType)
        {

            byte orderExchangeType = (byte)OrderExchangeType.Product;
            byte orderExchangeMode = (byte)OrderExchangeMode.Export;
            byte orderExchangeStatus = (byte)OrderExchangeStatus.Approved;
            byte oeOrderType = (byte)OrderExchangeOrderType.Order;
            var tmpDeposit = Db.Orders.Where(m => m.Id == id && !m.IsDelete)
                                            .Select(m => new DepositViewItem()
                                            {
                                                ContactAddress = m.ContactAddress,
                                                ContactEmail = m.ContactEmail,
                                                ContactName = m.ContactName,
                                                ContactPhone = m.ContactPhone,
                                                CustomerAddress = m.CustomerAddress,
                                                CustomerEmail = m.CustomerEmail,
                                                CustomerName = m.CustomerName,
                                                CustomerPhone = m.CustomerPhone,
                                                Status = m.Status,
                                                Code = m.Code,
                                                Note = m.Note,
                                                CreateDate = m.Created,
                                                UpdateDate = m.LastUpdate,
                                                Id = m.Id,
                                                Type = m.Type,
                                                WarehouseName = m.WarehouseName,
                                                WarehouseDeliveryName=m.WarehouseDeliveryName,
                                                TotalAdvance = (Db.OrderExchanges.Where(s => s.Type == orderExchangeType
                                                                    && s.Mode == orderExchangeMode
                                                                    && s.Status == orderExchangeStatus
                                                                    && !s.IsDelete
                                                                    && s.OrderId == m.Id
                                                                    && s.OrderType == oeOrderType)
                                                                .Select(s => (decimal)s.TotalPrice).DefaultIfEmpty(0).Sum()),
                                                ProvisionalMoney = m.ProvisionalMoney,
                                                ExchangeRate = m.ExchangeRate,
                                                Total = m.Total,
                                                TotalPayed = m.TotalPayed,
                                                TotalRefunded = m.TotalRefunded,
                                                Debt = m.Debt
                                            })
                                            .FirstOrDefault();

            var tmpDetail = Db.DepositDetails.Where(m => m.DepositId == id)
                                            .Select(m => new DepositDetailViewItem()
                                            {
                                                CategoryName = m.CategoryName,
                                                CreateDate = m.CreateDate,
                                                UpdateDate = m.UpdateDate,
                                                ProductName = m.ProductName,
                                                PacketNumber = m.PacketNumber,
                                                ListCode = m.ListCode,
                                                Image = m.Image,
                                                Size = m.Size,
                                                Quantity = m.Quantity,
                                                Note = m.Note,
                                                Weight = m.Weight
                                            }).ToList();
            var listOrderComment = Db.OrderComments.Where(m => m.OrderId == id && m.OrderType == oeOrderType).ToList();

            var listOrderServiceItem = Db.OrderServices.Where(m => m.OrderId == id)
                                                        .Select(m => new OrderServiceItem()
                                                        {
                                                            Id = m.Id,
                                                            ServiceId = m.ServiceId,
                                                            ServiceName = m.ServiceName,
                                                            TotalPrice = m.TotalPrice,
                                                            Checked = m.Checked,
                                                            IsDelete = m.IsDelete
                                                        }).ToList();
            //6. Lấy thông tin thanh toán trong đơn hàng
            var orderExchange = Db.OrderExchanges.FirstOrDefault(x => !x.IsDelete && x.OrderId == id && x.Type == (byte)OrderExchangeType.Product);
            //7. Lấy thông tin các kiện hàng trong đơn hàng
            int orderService = (int)OrderServices.OutSideShipping;
            var orderPackage = Db.OrderPackages.Where(x => x.OrderId == id && !x.IsDelete)
                                        .Select(m => new OrderPackageItem()
                                        {
                                            Id = m.Id,
                                            Code = m.Code,
                                            Weight = m.Weight,
                                            WeightActual = m.WeightActual,
                                            TransportCode = m.TransportCode,
                                            CurrentWarehouseName = m.CurrentWarehouseName,
                                            Status = m.Status,
                                            Length = m.Length,
                                            Height = m.Height,
                                            Width = m.Width,
                                            ActualMoney = (((decimal)(Db.OrderServices.FirstOrDefault(z => z.ServiceId == orderService & z.OrderId == id).TotalPrice * m.WeightActualPercent)) / 100),
                                            IsGross = (Db.Wallet.FirstOrDefault(y => y.Id == ((Db.WalletDetails.FirstOrDefault(z => z.PackageId == m.Id) == null ? 0 : Db.WalletDetails.FirstOrDefault(z => z.PackageId == m.Id).WalletId))) == null ? 1 : (Db.Wallet.FirstOrDefault(y => y.Id == ((Db.WalletDetails.FirstOrDefault(z => z.PackageId == m.Id) == null ? 0 : Db.WalletDetails.FirstOrDefault(z => z.PackageId == m.Id).WalletId))).PackageNo))
                                        }).ToList();
            //9. lay danh sach lich su giao dich
            var recharge = Db.RechargeBill.Where(m =>!m.IsDelete && m.OrderId == id).OrderByDescending(m => m.Created).ToList();
            //10: lay danh sach khieu nai
            var complains = Db.Complains.Where(m => !m.IsDelete && m.OrderId == id).OrderByDescending(m => m.CreateDate).ToList();
            var model = new DepositDetailModel()
            {
                DepositViewItem = tmpDeposit,
                ListDetail = tmpDetail,
                ListOrderComment = listOrderComment,
                ListOrderService = listOrderServiceItem,
                OrderExchange = orderExchange,
                RechargeBill = recharge,
                Complains = complains,
                OrderPackages = orderPackage
            };
            return model;

        }
        #endregion
    }
}
