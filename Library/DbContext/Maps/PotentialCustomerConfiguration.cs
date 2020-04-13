using Library.DbContext.Entities;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema; 
namespace Library.DbContext.Maps
{
    public partial class PotentialCustomerConfiguration : EntityTypeConfiguration<PotentialCustomer>
    {
        public PotentialCustomerConfiguration()
         : this("dbo")
        {
        }

        public PotentialCustomerConfiguration(string schema)
        {
            ToTable("PotentialCustomer", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.TypeId).HasColumnName(@"TypeId").IsOptional().HasColumnType("int");
            Property(x => x.TypeIdd).HasColumnName(@"TypeIdd").IsOptional().HasColumnType("int");
            Property(x => x.TypeName).HasColumnName(@"TypeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.Code).HasColumnName(@"Code").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.Email).HasColumnName(@"Email").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(200);
            Property(x => x.FirstName).HasColumnName(@"FirstName").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.LastName).HasColumnName(@"LastName").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.MidleName).HasColumnName(@"MidleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.FullName).HasColumnName(@"FullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(90);
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsOptional().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsOptional().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.Phone).HasColumnName(@"Phone").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Avatar).HasColumnName(@"Avatar").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Nickname).HasColumnName(@"Nickname").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.Birthday).HasColumnName(@"Birthday").IsOptional().HasColumnType("date");
            Property(x => x.LevelId).HasColumnName(@"LevelId").IsOptional().HasColumnType("tinyint");
            Property(x => x.LevelName).HasColumnName(@"LevelName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
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
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.HashTag).HasColumnName(@"HashTag").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.CountryId).HasColumnName(@"CountryId").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(10);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.CustomerTypeId).HasColumnName(@"CustomerTypeId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerTypeName).HasColumnName(@"CustomerTypeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            InitializePartial();
        }

        partial void InitializePartial();
    }
}