using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Deposit
    
    public partial class DepositConfiguration : EntityTypeConfiguration<Deposit>
    {
        public DepositConfiguration()
            : this("dbo")
        {
        }

        public DepositConfiguration(string schema)
        {
            ToTable("Deposit", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.UpdateDate).HasColumnName(@"UpdateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsRequired().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerAddress).HasColumnName(@"CustomerAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.LevelId).HasColumnName(@"LevelId").IsRequired().HasColumnType("tinyint");
            Property(x => x.LevelName).HasColumnName(@"LevelName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ContactName).HasColumnName(@"ContactName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.ContactPhone).HasColumnName(@"ContactPhone").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(300);
            Property(x => x.ContactAddress).HasColumnName(@"ContactAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.ContactEmail).HasColumnName(@"ContactEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserName).HasColumnName(@"UserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.OfficeId).HasColumnName(@"OfficeId").IsOptional().HasColumnType("int");
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OfficeIdPath).HasColumnName(@"OfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.PacketNumber).HasColumnName(@"PacketNumber").IsOptional().HasColumnType("int");
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsRequired().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.WarehouseId).HasColumnName(@"WarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ProvisionalMoney).HasColumnName(@"ProvisionalMoney").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalWeight).HasColumnName(@"TotalWeight").IsRequired().HasColumnType("float");
            Property(x => x.Currency).HasColumnName(@"Currency").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ExchangeRate).HasColumnName(@"ExchangeRate").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.UnsignName).HasColumnName(@"UnsignName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ReasonCancel).HasColumnName(@"ReasonCancel").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.DepositType).HasColumnName(@"DepositType").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseDeliveryId).HasColumnName(@"WarehouseDeliveryId").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseDeliveryName).HasColumnName(@"WarehouseDeliveryName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ApprovelPrice).HasColumnName(@"ApprovelPrice").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.ApprovelUnit).HasColumnName(@"ApprovelUnit").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.OrderInfoId).HasColumnName(@"OrderInfoId").IsOptional().HasColumnType("int");

            InitializePartial();
        }
        partial void InitializePartial();
    }

}
