using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // TransportMethod
    
    public partial class TransportMethodConfiguration : EntityTypeConfiguration<TransportMethod>
    {
        public TransportMethodConfiguration()
            : this("dbo")
        {
        }

        public TransportMethodConfiguration(string schema)
        {
            ToTable("TransportMethod", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
