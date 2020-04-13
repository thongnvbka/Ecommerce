using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Title
    
    public partial class TitleConfiguration : EntityTypeConfiguration<Title>
    {
        public TitleConfiguration()
            : this("dbo")
        {
        }

        public TitleConfiguration(string schema)
        {
            ToTable("Title", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UnsignedName).HasColumnName(@"UnsignedName").IsRequired().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.ShortName).HasColumnName(@"ShortName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdateUserId).HasColumnName(@"LastUpdateUserId").IsRequired().HasColumnType("bigint");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.OfficeNo).HasColumnName(@"OfficeNo").IsRequired().HasColumnType("int");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
