using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Customer
    public partial class CustomerConfiguration : EntityTypeConfiguration<Customer>
    {
        public CustomerConfiguration()
            : this("dbo")
        {
        }

        public CustomerConfiguration(string schema)
        {
            ToTable("Customer", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.TypeId).HasColumnName(@"TypeId").IsOptional().HasColumnType("int");
            Property(x => x.TypeIdd).HasColumnName(@"TypeIdd").IsOptional().HasColumnType("int");
            Property(x => x.TypeName).HasColumnName(@"TypeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.Email).HasColumnName(@"Email").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(100);
            Property(x => x.FirstName).HasColumnName(@"FirstName").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.LastName).HasColumnName(@"LastName").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.MidleName).HasColumnName(@"MidleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.FullName).HasColumnName(@"FullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(90);
            Property(x => x.Password).HasColumnName(@"Password").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsRequired().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsOptional().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.Phone).HasColumnName(@"Phone").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Avatar).HasColumnName(@"Avatar").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Nickname).HasColumnName(@"Nickname").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.LevelId).HasColumnName(@"LevelId").IsRequired().HasColumnType("tinyint");
            Property(x => x.LevelName).HasColumnName(@"LevelName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.Point).HasColumnName(@"Point").IsRequired().HasColumnType("int");
            Property(x => x.GenderId).HasColumnName(@"GenderId").IsOptional().HasColumnType("tinyint");
            Property(x => x.GenderName).HasColumnName(@"GenderName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.DistrictId).HasColumnName(@"DistrictId").IsOptional().HasColumnType("int");
            Property(x => x.DistrictName).HasColumnName(@"DistrictName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ProvinceId).HasColumnName(@"ProvinceId").IsOptional().HasColumnType("int");
            Property(x => x.ProvinceName).HasColumnName(@"ProvinceName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WardId).HasColumnName(@"WardId").IsOptional().HasColumnType("int");
            Property(x => x.WardsName).HasColumnName(@"WardsName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Address).HasColumnName(@"Address").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.OfficeId).HasColumnName(@"OfficeId").IsOptional().HasColumnType("int");
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OfficeIdPath).HasColumnName(@"OfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsOptional().HasColumnType("datetime");
            Property(x => x.LastLockoutDate).HasColumnName(@"LastLockoutDate").IsOptional().HasColumnType("datetime");
            Property(x => x.LockoutToDate).HasColumnName(@"LockoutToDate").IsOptional().HasColumnType("datetime");
            Property(x => x.FirstLoginFailureDate).HasColumnName(@"FirstLoginFailureDate").IsOptional().HasColumnType("datetime");
            Property(x => x.LoginFailureCount).HasColumnName(@"LoginFailureCount").IsRequired().HasColumnType("tinyint");
            Property(x => x.HashTag).HasColumnName(@"HashTag").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.Balance).HasColumnName(@"Balance").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.BalanceAvalible).HasColumnName(@"BalanceAvalible").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.IsActive).HasColumnName(@"IsActive").IsRequired().HasColumnType("bit");
            Property(x => x.IsLockout).HasColumnName(@"IsLockout").IsRequired().HasColumnType("bit");
            Property(x => x.CodeActive).HasColumnName(@"CodeActive").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.CreateDateActive).HasColumnName(@"CreateDateActive").IsOptional().HasColumnType("datetime");
            Property(x => x.DateActive).HasColumnName(@"DateActive").IsOptional().HasColumnType("datetime");
            Property(x => x.CountryId).HasColumnName(@"CountryId").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(10);
            Property(x => x.Code).HasColumnName(@"Code").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.CardName).HasColumnName(@"CardName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.CardId).HasColumnName(@"CardId").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.CardBank).HasColumnName(@"CardBank").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.CardBranch).HasColumnName(@"CardBranch").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.WarehouseId).HasColumnName(@"WarehouseId").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.DepositPrice).HasColumnName(@"DepositPrice").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Birthday).HasColumnName(@"Birthday").IsOptional().HasColumnType("date");
            Property(x => x.Url).HasColumnName(@"Url").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
