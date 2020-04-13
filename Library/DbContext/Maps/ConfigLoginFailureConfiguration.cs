using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // ConfigLoginFailure
    
    public partial class ConfigLoginFailureConfiguration : EntityTypeConfiguration<ConfigLoginFailure>
    {
        public ConfigLoginFailureConfiguration()
            : this("dbo")
        {
        }

        public ConfigLoginFailureConfiguration(string schema)
        {
            ToTable("ConfigLoginFailure", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.MaximumLoginFailure).HasColumnName(@"MaximumLoginFailure").IsRequired().HasColumnType("tinyint");
            Property(x => x.LockDuration).HasColumnName(@"LockDuration").IsRequired().HasColumnType("smallint");
            Property(x => x.LoginFailureInterval).HasColumnName(@"LoginFailureInterval").IsRequired().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
