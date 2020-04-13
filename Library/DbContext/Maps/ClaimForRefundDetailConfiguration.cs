using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // ClaimForRefundDetail
    public partial class ClaimForRefundDetailConfiguration : EntityTypeConfiguration<ClaimForRefundDetail>
    {
        public ClaimForRefundDetailConfiguration()
            : this("dbo")
        {
        }

        public ClaimForRefundDetailConfiguration(string schema)
        {
            ToTable("ClaimForRefundDetail", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.ProductId).HasColumnName(@"ProductId").IsOptional().HasColumnType("bigint");
            Property(x => x.Name).HasColumnName(@"Name").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.Link).HasColumnName(@"Link").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Image).HasColumnName(@"Image").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Quantity).HasColumnName(@"Quantity").IsOptional().HasColumnType("int");
            Property(x => x.Size).HasColumnName(@"Size").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Color).HasColumnName(@"Color").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Price).HasColumnName(@"Price").IsOptional().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalPrice).HasColumnName(@"TotalPrice").IsOptional().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalExchange).HasColumnName(@"TotalExchange").IsOptional().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsOptional().HasColumnType("int");
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsOptional().HasColumnType("tinyint");
            Property(x => x.QuantityFailed).HasColumnName(@"QuantityFailed").IsOptional().HasColumnType("int");
            Property(x => x.TotalQuantityFailed).HasColumnName(@"TotalQuantityFailed").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.ClaimId).HasColumnName(@"ClaimId").IsOptional().HasColumnType("int");
            Property(x => x.ClaimCode).HasColumnName(@"ClaimCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
