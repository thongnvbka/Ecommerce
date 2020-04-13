using Library.DbContext.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Library.DbContext.Maps
{
    // Ward
    public partial class WardConfiguration : EntityTypeConfiguration<Ward>
    {
        public WardConfiguration()
            : this("dbo")
        {
        }

        public WardConfiguration(string schema)
        {
            ToTable("Ward", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ProvinceId).HasColumnName(@"ProvinceId").IsRequired().HasColumnType("int");
            Property(x => x.ProvinceName).HasColumnName(@"ProvinceName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.DistrictId).HasColumnName(@"DistrictId").IsRequired().HasColumnType("int");
            Property(x => x.DistrictName).HasColumnName(@"DistrictName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Culture).HasColumnName(@"Culture").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(2);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
