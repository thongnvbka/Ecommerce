using Library.DbContext.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Library.DbContext.Maps
{
   // Province

    public partial class ProvinceConfiguration : EntityTypeConfiguration<Province>
    {
        public ProvinceConfiguration()
            : this("dbo")
        {
        }

        public ProvinceConfiguration(string schema)
        {
            ToTable("Province", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Culture).HasColumnName(@"Culture").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(2);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
