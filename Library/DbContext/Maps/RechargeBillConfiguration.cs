using System;
using Library.DbContext.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Library.DbContext.Maps
{
    // RechargeBill

    public partial class RechargeBillConfiguration : EntityTypeConfiguration<RechargeBill>
    {
        public RechargeBillConfiguration()
            : this("dbo")
        {
        }

        public RechargeBillConfiguration(string schema)
        {
            ToTable("RechargeBill", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.CurrencyFluctuations).HasColumnName(@"CurrencyFluctuations").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Increase).HasColumnName(@"Increase").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Diminishe).HasColumnName(@"Diminishe").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.CurencyStart).HasColumnName(@"CurencyStart").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.CurencyEnd).HasColumnName(@"CurencyEnd").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserCode).HasColumnName(@"UserCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.UserName).HasColumnName(@"UserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.UserApprovalId).HasColumnName(@"UserApprovalId").IsOptional().HasColumnType("int");
            Property(x => x.UserApprovalCode).HasColumnName(@"UserApprovalCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.UserApprovalName).HasColumnName(@"UserApprovalName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerCode).HasColumnName(@"CustomerCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerAddress).HasColumnName(@"CustomerAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.TreasureId).HasColumnName(@"TreasureId").IsOptional().HasColumnType("int");
            Property(x => x.TreasureName).HasColumnName(@"TreasureName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.TreasureIdd).HasColumnName(@"TreasureIdd").IsOptional().HasColumnType("int");
            Property(x => x.IsAutomatic).HasColumnName(@"IsAutomatic").IsOptional().HasColumnType("bit");
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsOptional().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsOptional().HasColumnType("tinyint");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdated).HasColumnName(@"LastUpdated").IsRequired().HasColumnType("datetime");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
