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

namespace Library.DbContext.Repositories
{
    public class SourceDetailRepository : Repository<SourceDetail>
    {
        public SourceDetailRepository(ProjectXContext context) : base(context)
        {
        }
        public SourceDetailModel GetAll(long sourceId, byte commentType)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_sourcedetail_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("sourceId", sourceId));
                cmd.Parameters.Add(new SqlParameter("commentOrderType", commentType));
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpSourceDetailItem = ((IObjectContextAdapter)context).ObjectContext.Translate<SourceDetailItem>(reader).ToList().FirstOrDefault();
                    reader.NextResult();
                    var tmpSourceProduct = ((IObjectContextAdapter)context).ObjectContext.Translate<SourceProductItem>(reader).ToList().FirstOrDefault();
                    reader.NextResult();
                    var tmpSupplierItem = ((IObjectContextAdapter)context).ObjectContext.Translate<SourceSupplierItem>(reader).ToList();
                    reader.NextResult();
                    var tmpOrderCommentItem = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderComment>(reader).ToList();
                    reader.Close();

                    var model = new SourceDetailModel()
                    {
                        SourceDetailItem = tmpSourceDetailItem,
                        SourceProductItem = tmpSourceProduct,
                        ListSourceSupplier = tmpSupplierItem,
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

        public SourceDetailModel GetAllByLinq(long sourceId, byte commentType)
        {
            var tmpSource = Db.Sources.Where(m => m.Id == sourceId && !m.IsDelete).Join(Db.Customers,
                s => s.CustomerId,
                c => c.Id,
                (s, c) => new SourceDetailItem()
                {
                    Id = s.Id,
                    Code = s.Code,
                    AnalyticSupplier = s.AnalyticSupplier,
                    CreateDate = s.CreateDate,
                    UpdateDate = s.UpdateDate ?? s.CreateDate,
                    Status = s.Status,
                    ServiceMoney = s.ServiceMoney,
                    TypeService = s.TypeService,
                    ShipMoney = s.ShipMoney,
                    SourceSupplierId = (long)s.SourceSupplierId,
                    TypeServiceName = s.TypeServiceName,
                    CustomerName = c.FullName,
                    CustomerPhone = c.Phone,
                    CustomerAddress = c.Address,
                    CustomerEmail = c.Email
                }).FirstOrDefault();

            var tmpDetail = Db.SourceDetails.Where(m => m.SourceId == sourceId)
                                            .Select(m => new SourceProductItem()
                                            {
                                                Name = m.Name,
                                                CategoryName = m.CategoryName,
                                                Color = m.Color,
                                                Size = m.Size,
                                                Link = m.Link,
                                                Note = m.Note,
                                                Quantity = m.Quantity,
                                                ImagePath1 = m.ImagePath1,
                                                ImagePath2 = m.ImagePath2,
                                                ImagePath3 = m.ImagePath3,
                                                ImagePath4 = m.ImagePath4
                                            }).FirstOrDefault();

            var tmpSuplier = Db.SourceSuppliers.Where(m => m.SourceId == sourceId && !m.IsDelete)
                                                .Select(m => new SourceSupplierItem()
                                                {
                                                    Price = m.Price,
                                                    ExchangePrice = m.ExchangePrice,
                                                    Quantity = m.Quantity,
                                                    TotalPrice = m.TotalPrice,
                                                    TotalExchange = m.TotalExchange,
                                                    ShipMoney = m.ShipMoney,
                                                    Name = m.Name,
                                                    Id = m.Id
                                                }).Skip(3).ToList();
            var listOrderComment = Db.OrderComments.Where(m => m.OrderId == sourceId && m.OrderType == commentType).ToList();

            var model = new SourceDetailModel()
            {
                SourceDetailItem = tmpSource,
                SourceProductItem = tmpDetail,
                ListSourceSupplier = tmpSuplier,
                ListOrderComment = listOrderComment
            };
            return model;

        }
    }
}