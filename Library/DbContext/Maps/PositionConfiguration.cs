using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Position
    
    public partial class PositionConfiguration : EntityTypeConfiguration<Position>
    {
        public PositionConfiguration()
            : this("dbo")
        {
        }

        public PositionConfiguration(string schema)
        {
            ToTable("Position", schema);
            HasKey(x => new { x.OfficeId, x.TitleId });

            Property(x => x.OfficeId).HasColumnName(@"OfficeId").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.TitleId).HasColumnName(@"TitleId").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.TitleName).HasColumnName(@"TitleName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
