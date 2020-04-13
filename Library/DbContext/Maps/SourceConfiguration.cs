using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    using System;

    // Source
    
    public partial class SourceConfiguration : EntityTypeConfiguration<Source>
    {
        public SourceConfiguration()
            : this("dbo")
        {
        }

        public SourceConfiguration(string schema)
        {
            ToTable("Source", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsRequired().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.WarehouseId).HasColumnName(@"WarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerAddress).HasColumnName(@"CustomerAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.OfficeId).HasColumnName(@"OfficeId").IsOptional().HasColumnType("int");
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OfficeIdPath).HasColumnName(@"OfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedOfficeIdPath).HasColumnName(@"CreatedOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedUserId).HasColumnName(@"CreatedUserId").IsOptional().HasColumnType("int");
            Property(x => x.CreatedUserFullName).HasColumnName(@"CreatedUserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.CreatedOfficeId).HasColumnName(@"CreatedOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.CreatedOfficeName).HasColumnName(@"CreatedOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.UpdateDate).HasColumnName(@"UpdateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.TypeService).HasColumnName(@"TypeService").IsRequired().HasColumnType("int");
            Property(x => x.ServiceMoney).HasColumnName(@"ServiceMoney").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.AnalyticSupplier).HasColumnName(@"AnalyticSupplier").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.ShipMoney).HasColumnName(@"ShipMoney").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.SourceSupplierId).HasColumnName(@"SourceSupplierId").IsOptional().HasColumnType("bigint");
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.UserNote).HasColumnName(@"UserNote").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.UnsignName).HasColumnName(@"UnsignName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ReasonCancel).HasColumnName(@"ReasonCancel").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.TypeServiceName).HasColumnName(@"TypeServiceName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
