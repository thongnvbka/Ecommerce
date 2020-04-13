using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // HistorySatus
    
    public partial class HistorySatuConfiguration : EntityTypeConfiguration<HistorySatu>
    {
        public HistorySatuConfiguration()
            : this("dbo")
        {
        }

        public HistorySatuConfiguration(string schema)
        {
            ToTable("HistorySatus", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Time).HasColumnName(@"Time").IsRequired().HasColumnType("datetime");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.Note).HasColumnName(@"Note").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.RecordId).HasColumnName(@"RecordId").IsRequired().HasColumnType("int");
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.Json).HasColumnName(@"Json").IsOptional().HasColumnType("nvarchar(max)");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
