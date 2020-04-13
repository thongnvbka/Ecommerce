using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Recent
    
    public partial class RecentConfiguration : EntityTypeConfiguration<Recent>
    {
        public RecentConfiguration()
            : this("dbo")
        {
        }

        public RecentConfiguration(string schema)
        {
            ToTable("Recent", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.RecordId).HasColumnName(@"RecordId").IsRequired().HasColumnType("int");
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.CountNo).HasColumnName(@"CountNo").IsRequired().HasColumnType("int");
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");

            InitializePartial();
        }
        partial void InitializePartial();
    }

}
