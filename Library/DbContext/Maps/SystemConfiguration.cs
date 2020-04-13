using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Library.DbContext.Maps
{
    // System
    
    public partial class SystemConfiguration : EntityTypeConfiguration<Entities.System>
    {
        public SystemConfiguration()
            : this("dbo")
        {
        }

        public SystemConfiguration(string schema)
        {
            ToTable("System", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Domain).HasColumnName(@"Domain").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
