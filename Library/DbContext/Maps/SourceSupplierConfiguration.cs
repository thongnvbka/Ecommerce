using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{

    // SourceSupplier

    public partial class SourceSupplierConfiguration : EntityTypeConfiguration<SourceSupplier>
    {
        public SourceSupplierConfiguration()
            : this("dbo")
        {
        }

        public SourceSupplierConfiguration(string schema)
        {
            ToTable("SourceSupplier", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.SourceId).HasColumnName(@"SourceId").IsOptional().HasColumnType("bigint");
            Property(x => x.Price).HasColumnName(@"Price").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.ExchangeRate).HasColumnName(@"ExchangeRate").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.ExchangePrice).HasColumnName(@"ExchangePrice").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalPrice).HasColumnName(@"TotalPrice").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalExchange).HasColumnName(@"TotalExchange").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.Quantity).HasColumnName(@"Quantity").IsRequired().HasColumnType("int");
            Property(x => x.Name).HasColumnName(@"Name").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Link).HasColumnName(@"Link").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdate).HasColumnName(@"LastUpdate").IsRequired().HasColumnType("datetime");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.ShipMoney).HasColumnName(@"ShipMoney").IsOptional().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.ActiveDate).HasColumnName(@"ActiveDate").IsOptional().HasColumnType("datetime");
            Property(x => x.LimitDate).HasColumnName(@"LimitDate").IsOptional().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
