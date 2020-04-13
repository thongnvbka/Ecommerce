using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;
namespace Library.DbContext.Maps
{
    public partial class OrderReasonConfiguration : EntityTypeConfiguration<OrderReason>
    {
        public OrderReasonConfiguration()
            : this("dbo")
        {
        }

        public OrderReasonConfiguration(string schema)
        {
            ToTable("OrderReason", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.ReasonId).HasColumnName(@"ReasonId").IsRequired().HasColumnType("tinyint");
            Property(x => x.Reason).HasColumnName(@"Reason").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }
}
