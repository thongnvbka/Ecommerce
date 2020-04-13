using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Delivery
    
    public partial class DeliveryConfiguration : EntityTypeConfiguration<Delivery>
    {
        public DeliveryConfiguration()
            : this("dbo")
        {
        }

        public DeliveryConfiguration(string schema)
        {
            ToTable("Delivery", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.UnsignedText).HasColumnName(@"UnsignedText").IsRequired().HasColumnType("nvarchar");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.OrderNo).HasColumnName(@"OrderNo").IsOptional().HasColumnType("int");
            Property(x => x.PackageNo).HasColumnName(@"PackageNo").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseId).HasColumnName(@"WarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.WarehouseIdPath).HasColumnName(@"WarehouseIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseAddress).HasColumnName(@"WarehouseAddress").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedUserId).HasColumnName(@"CreatedUserId").IsRequired().HasColumnType("int");
            Property(x => x.CreatedUserFullName).HasColumnName(@"CreatedUserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedUserUserName).HasColumnName(@"CreatedUserUserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.CreatedUserTitleId).HasColumnName(@"CreatedUserTitleId").IsRequired().HasColumnType("int");
            Property(x => x.CreatedUserTitleName).HasColumnName(@"CreatedUserTitleName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedOfficeId).HasColumnName(@"CreatedOfficeId").IsRequired().HasColumnType("int");
            Property(x => x.CreatedOfficeName).HasColumnName(@"CreatedOfficeName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedOfficeIdPath).HasColumnName(@"CreatedOfficeIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CreatedTime).HasColumnName(@"CreatedTime").IsRequired().HasColumnType("datetime");
            Property(x => x.ExpertiseUserId).HasColumnName(@"ExpertiseUserId").IsOptional().HasColumnType("int");
            Property(x => x.ExpertiseUserFullName).HasColumnName(@"ExpertiseUserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ExpertiseUserUserName).HasColumnName(@"ExpertiseUserUserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ExpertiseUserTitleId).HasColumnName(@"ExpertiseUserTitleId").IsOptional().HasColumnType("int");
            Property(x => x.ExpertiseUserTitleName).HasColumnName(@"ExpertiseUserTitleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ExpertiseOfficeId).HasColumnName(@"ExpertiseOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.ExpertiseOfficeName).HasColumnName(@"ExpertiseOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ExpertiseOfficeIdPath).HasColumnName(@"ExpertiseOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ExpertiseTime).HasColumnName(@"ExpertiseTime").IsOptional().HasColumnType("datetime");
            Property(x => x.ShipperUserId).HasColumnName(@"ShipperUserId").IsOptional().HasColumnType("int");
            Property(x => x.ShipperFullName).HasColumnName(@"ShipperFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShipperUserUserName).HasColumnName(@"ShipperUserUserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ShipperUserTitleId).HasColumnName(@"ShipperUserTitleId").IsOptional().HasColumnType("int");
            Property(x => x.ShipperUserTitleName).HasColumnName(@"ShipperUserTitleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShipperOfficeId).HasColumnName(@"ShipperOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.ShipperOfficeName).HasColumnName(@"ShipperOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShipperOfficeIdPath).HasColumnName(@"ShipperOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ShipperTime).HasColumnName(@"shipperTime").IsOptional().HasColumnType("datetime");
            Property(x => x.ApprovelUserId).HasColumnName(@"ApprovelUserId").IsOptional().HasColumnType("int");
            Property(x => x.ApprovelFullName).HasColumnName(@"ApprovelFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ApprovelUserUserName).HasColumnName(@"ApprovelUserUserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ApprovelUserTitleId).HasColumnName(@"ApprovelUserTitleId").IsOptional().HasColumnType("int");
            Property(x => x.ApprovelUserTitleName).HasColumnName(@"ApprovelUserTitleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ApprovelOfficeId).HasColumnName(@"ApprovelOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.ApprovelOfficeName).HasColumnName(@"ApprovelOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ApprovelOfficeIdPath).HasColumnName(@"ApprovelOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ApprovelTime).HasColumnName(@"ApprovelTime").IsOptional().HasColumnType("datetime");
            Property(x => x.AccountantUserId).HasColumnName(@"AccountantUserId").IsOptional().HasColumnType("int");
            Property(x => x.AccountantFullName).HasColumnName(@"AccountantFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.AccountantUserUserName).HasColumnName(@"AccountantUserUserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.AccountantUserTitleId).HasColumnName(@"AccountantUserTitleId").IsOptional().HasColumnType("int");
            Property(x => x.AccountantUserTitleName).HasColumnName(@"AccountantUserTitleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.AccountantOfficeId).HasColumnName(@"AccountantOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.AccountantOfficeName).HasColumnName(@"AccountantOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.AccountantOfficeIdPath).HasColumnName(@"AccountantOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.AccountantTime).HasColumnName(@"AccountantTime").IsOptional().HasColumnType("datetime");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.CarNumber).HasColumnName(@"CarNumber").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
            Property(x => x.CarDescription).HasColumnName(@"CarDescription").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.IsLast).HasColumnName(@"IsLast").IsRequired().HasColumnType("bit");
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerCode).HasColumnName(@"CustomerCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.CustomerFullName).HasColumnName(@"CustomerFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.CustomerAddress).HasColumnName(@"CustomerAddress").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CustomerVipId).HasColumnName(@"CustomerVipId").IsRequired().HasColumnType("tinyint");
            Property(x => x.CustomerVipName).HasColumnName(@"CustomerVipName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Weight).HasColumnName(@"Weight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.WeightConverted).HasColumnName(@"WeightConverted").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.WeightActual).HasColumnName(@"WeightActual").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.PriceWeight).HasColumnName(@"PriceWeight").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PricePacking).HasColumnName(@"PricePacking").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PriceOrder).HasColumnName(@"PriceOrder").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PriceOther).HasColumnName(@"PriceOther").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PriceStored).HasColumnName(@"PriceStored").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PriceShip).HasColumnName(@"PriceShip").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Total).HasColumnName(@"Total").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Debit).HasColumnName(@"Debit").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.DebitPre).HasColumnName(@"DebitPre").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PricePayed).HasColumnName(@"PricePayed").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Receivable).HasColumnName(@"Receivable").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.BlanceBefo).HasColumnName(@"BlanceBefo").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.BlanceAfter).HasColumnName(@"BlanceAfter").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.DebitAfter).HasColumnName(@"DebitAfter").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
