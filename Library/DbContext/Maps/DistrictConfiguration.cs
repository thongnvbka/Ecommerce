using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // District
    public partial class DistrictConfiguration : EntityTypeConfiguration<District>
    {
        public DistrictConfiguration()
            : this("dbo")
        {
        }

        public DistrictConfiguration(string schema)
        {
            ToTable("District", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ProvinceId).HasColumnName(@"ProvinceId").IsRequired().HasColumnType("int");
            Property(x => x.ProvinceName).HasColumnName(@"ProvinceName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Culture).HasColumnName(@"Culture").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(2);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
