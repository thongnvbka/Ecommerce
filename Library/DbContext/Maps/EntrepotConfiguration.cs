using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Entrepot
    
    public partial class EntrepotConfiguration : EntityTypeConfiguration<Entrepot>
    {
        public EntrepotConfiguration()
            : this("dbo")
        {
        }

        public EntrepotConfiguration(string schema)
        {
            ToTable("Entrepot", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Status).HasColumnName(@"Status").IsOptional().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
