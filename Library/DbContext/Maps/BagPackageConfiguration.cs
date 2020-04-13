using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // BagPackage
    
    public partial class BagPackageConfiguration : EntityTypeConfiguration<BagPackage>
    {
        public BagPackageConfiguration()
            : this("dbo")
        {
        }

        public BagPackageConfiguration(string schema)
        {
            ToTable("BagPackage", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.BagId).HasColumnName(@"BagId").IsRequired().HasColumnType("int");
            Property(x => x.PackageId).HasColumnName(@"PackageId").IsRequired().HasColumnType("int");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
