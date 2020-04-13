using System;
using Library.DbContext.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Library.DbContext.Maps
{
    using System;

    // RequestShip
    
    public partial class RequestShipConfiguration : EntityTypeConfiguration<RequestShip>
    {
        public RequestShipConfiguration()
            : this("dbo")
        {
        }

        public RequestShipConfiguration(string schema)
        {
            ToTable("RequestShip", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.PackageCode).HasColumnName(@"PackageCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.UpdateDate).HasColumnName(@"UpdateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerAddress).HasColumnName(@"CustomerAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsRequired().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
