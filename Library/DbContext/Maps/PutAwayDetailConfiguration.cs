using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // PutAwayDetail

    public partial class PutAwayDetailConfiguration : EntityTypeConfiguration<PutAwayDetail>
    {
        public PutAwayDetailConfiguration()
            : this("dbo")
        {
        }

        public PutAwayDetailConfiguration(string schema)
        {
            ToTable("PutAwayDetail", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.PutAwayId).HasColumnName(@"PutAwayId").IsRequired().HasColumnType("int");
            Property(x => x.PutAwayCode).HasColumnName(@"PutAwayCode").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(30);
            Property(x => x.PackageId).HasColumnName(@"PackageId").IsRequired().HasColumnType("int");
            Property(x => x.PackageCode).HasColumnName(@"PackageCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.TransportCode).HasColumnName(@"TransportCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsRequired().HasColumnType("tinyint");
            Property(x => x.OrderServices).HasColumnName(@"OrderServices").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.OrderPackageNo).HasColumnName(@"OrderPackageNo").IsRequired().HasColumnType("int");
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerUserName).HasColumnName(@"CustomerUserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Length).HasColumnName(@"Length").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Weight).HasColumnName(@"Weight").IsRequired().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.Width).HasColumnName(@"Width").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Size).HasColumnName(@"Size").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Height).HasColumnName(@"Height").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.LayoutId).HasColumnName(@"LayoutId").IsOptional().HasColumnType("int");
            Property(x => x.LayoutName).HasColumnName(@"LayoutName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.LayoutIdPath).HasColumnName(@"LayoutIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.LayoutNamePath).HasColumnName(@"LayoutNamePath").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.ConvertedWeight).HasColumnName(@"ConvertedWeight").IsRequired().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.ActualWeight).HasColumnName(@"ActualWeight").IsRequired().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }
}
