using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{

    // PackageNote
    
    public partial class PackageNoteConfiguration : EntityTypeConfiguration<PackageNote>
    {
        public PackageNoteConfiguration()
            : this("dbo")
        {
        }

        public PackageNoteConfiguration(string schema)
        {
            ToTable("PackageNote", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.PackageId).HasColumnName(@"PackageId").IsOptional().HasColumnType("int");
            Property(x => x.PackageCode).HasColumnName(@"PackageCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.Time).HasColumnName(@"Time").IsRequired().HasColumnType("datetime");
            Property(x => x.ObjectId).HasColumnName(@"ObjectId").IsOptional().HasColumnType("int");
            Property(x => x.ObjectCode).HasColumnName(@"ObjectCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.DataJson).HasColumnName(@"DataJson").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Content).HasColumnName(@"Content").IsOptional().HasColumnType("nvarchar(max)");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
