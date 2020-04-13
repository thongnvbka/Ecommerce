using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // OrderRefund
    
    public partial class OrderRefundConfiguration : EntityTypeConfiguration<OrderRefund>
    {
        public OrderRefundConfiguration()
            : this("dbo")
        {
        }

        public OrderRefundConfiguration(string schema)
        {
            ToTable("OrderRefund", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.LinkNo).HasColumnName(@"LinkNo").IsRequired().HasColumnType("int");
            Property(x => x.ProductNo).HasColumnName(@"ProductNo").IsRequired().HasColumnType("int");
            Property(x => x.UnsignText).HasColumnName(@"UnsignText").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.CreateUserId).HasColumnName(@"CreateUserId").IsOptional().HasColumnType("int");
            Property(x => x.CreateUserFullName).HasColumnName(@"CreateUserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreateUserName).HasColumnName(@"CreateUserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.CreateOfficeId).HasColumnName(@"CreateOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.CreateOfficeName).HasColumnName(@"CreateOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreateOfficeIdPath).HasColumnName(@"CreateOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UpdateUserId).HasColumnName(@"UpdateUserId").IsOptional().HasColumnType("int");
            Property(x => x.UpdateUserFullName).HasColumnName(@"UpdateUserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UpdateUserName).HasColumnName(@"UpdateUserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.UpdateOfficeId).HasColumnName(@"UpdateOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.UpdateOfficeName).HasColumnName(@"UpdateOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UpdateOfficeIdPath).HasColumnName(@"UpdateOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CommentNo).HasColumnName(@"CommentNo").IsRequired().HasColumnType("int");
            Property(x => x.Amount).HasColumnName(@"Amount").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.AmountActual).HasColumnName(@"AmountActual").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalAcount).HasColumnName(@"TotalAcount").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.Percent).HasColumnName(@"Percent").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
