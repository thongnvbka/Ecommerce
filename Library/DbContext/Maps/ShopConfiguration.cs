using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Shops

    public partial class ShopConfiguration : EntityTypeConfiguration<Shop>
    {
        public ShopConfiguration()
            : this("dbo")
        {
        }

        public ShopConfiguration(string schema)
        {
            ToTable("Shops", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.Website).HasColumnName(@"Website").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Url).HasColumnName(@"Url").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.CategoryId).HasColumnName(@"CategoryId").IsOptional().HasColumnType("int");
            Property(x => x.CategoryName).HasColumnName(@"CategoryName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Vote).HasColumnName(@"Vote").IsOptional().HasColumnType("int");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.HashTag).HasColumnName(@"HashTag").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.LinkNo).HasColumnName(@"LinkNo").IsRequired().HasColumnType("int");
            Property(x => x.OrderNo).HasColumnName(@"OrderNo").IsRequired().HasColumnType("int");
            Property(x => x.ProductNo).HasColumnName(@"ProductNo").IsRequired().HasColumnType("int");
            Property(x => x.TotalPrice).HasColumnName(@"TotalPrice").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.BargainMax).HasColumnName(@"BargainMax").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.BargainMin).HasColumnName(@"BargainMin").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.UpdateDate).HasColumnName(@"UpdateDate").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
