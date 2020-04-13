using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // OrderService
    
    public partial class OrderServiceConfiguration : EntityTypeConfiguration<OrderService>
    {
        public OrderServiceConfiguration()
            : this("dbo")
        {
        }

        public OrderServiceConfiguration(string schema)
        {
            ToTable("OrderService", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.ServiceId).HasColumnName(@"ServiceId").IsRequired().HasColumnType("int");
            Property(x => x.ServiceName).HasColumnName(@"ServiceName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ExchangeRate).HasColumnName(@"ExchangeRate").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Currency).HasColumnName(@"Currency").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.Value).HasColumnName(@"Value").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalPrice).HasColumnName(@"TotalPrice").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.HashTag).HasColumnName(@"HashTag").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.Checked).HasColumnName(@"Checked").IsRequired().HasColumnType("bit");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdate).HasColumnName(@"LastUpdate").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
