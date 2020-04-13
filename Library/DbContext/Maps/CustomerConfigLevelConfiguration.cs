using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    public partial class CustomerConfigLevelConfiguration : EntityTypeConfiguration<CustomerConfigLevel>
    {
        public CustomerConfigLevelConfiguration()
            : this("dbo")
        {
        }

        public CustomerConfigLevelConfiguration(string schema)
        {
            ToTable("CustomerConfigLevel", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"ID").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.CustomerConfigName).HasColumnName(@"CustomerConfigName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.TurnoverRate).HasColumnName(@"TurnoverRate").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            InitializePartial();
        }

        partial void InitializePartial();
    }
}