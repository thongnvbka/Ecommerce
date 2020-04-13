using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // AccountantSubject

    public partial class OrderServiceOtherConfiguration : EntityTypeConfiguration<OrderServiceOther>
    {
        public OrderServiceOtherConfiguration()
            : this("dbo")
        {
        }

        public OrderServiceOtherConfiguration(string schema)
        {
            ToTable("OrderServiceOther", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ExchangeRate).HasColumnName(@"ExchangeRate").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Currency).HasColumnName(@"Currency").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Value).HasColumnName(@"Value").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalPrice).HasColumnName(@"TotalPrice").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.ObjectId).HasColumnName(@"ObjectId").IsRequired().HasColumnType("int");
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.CreatedUserId).HasColumnName(@"CreatedUserId").IsRequired().HasColumnType("int");
            Property(x => x.CreatedUserFullName).HasColumnName(@"CreatedUserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedUserUserName).HasColumnName(@"CreatedUserUserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.CreatedUserTitleId).HasColumnName(@"CreatedUserTitleId").IsRequired().HasColumnType("int");
            Property(x => x.CreatedUserTitleName).HasColumnName(@"CreatedUserTitleName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedOfficeId).HasColumnName(@"CreatedOfficeId").IsRequired().HasColumnType("int");
            Property(x => x.CreatedOfficeName).HasColumnName(@"CreatedOfficeName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedOfficeIdPath).HasColumnName(@"CreatedOfficeIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.PackageNo).HasColumnName(@"PackageNo").IsRequired().HasColumnType("int");
            Property(x => x.TotalWeightActual).HasColumnName(@"TotalWeightActual").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.PackageCodes).HasColumnName(@"PackageCodes").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.DataJson).HasColumnName(@"DataJson").IsOptional().HasColumnType("nvarchar");
            Property(x => x.UnsignText).HasColumnName(@"UnsignText").IsOptional().HasColumnType("nvarchar");
            InitializePartial();
        }
        partial void InitializePartial();
    }
}
