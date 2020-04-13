using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Setting

    public partial class SettingConfiguration : EntityTypeConfiguration<Setting>
    {
        public SettingConfiguration()
            : this("dbo")
        {
        }

        public SettingConfiguration(string schema)
        {
            ToTable("Setting", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.SettingKey).HasColumnName(@"SettingKey").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.SettingValue).HasColumnName(@"SettingValue").IsOptional().HasColumnType("ntext");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
