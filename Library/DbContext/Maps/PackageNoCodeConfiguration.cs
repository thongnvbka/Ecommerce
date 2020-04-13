using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    public partial class PackageNoCodeConfiguration : EntityTypeConfiguration<PackageNoCode>
    {
        public PackageNoCodeConfiguration()
            : this("dbo")
        {
        }

        public PackageNoCodeConfiguration(string schema)
        {
            ToTable("PackageNoCode", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.PackageId).HasColumnName(@"PackageId").IsRequired().HasColumnType("int");
            Property(x => x.PackageCode).HasColumnName(@"PackageCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar");
            Property(x => x.UnsignText).HasColumnName(@"UnsignText").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.ImageJson).HasColumnName(@"ImageJson").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.CreateUserId).HasColumnName(@"CreateUserId").IsOptional().HasColumnType("int");
            Property(x => x.CreateUserFullName).HasColumnName(@"CreateUserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreateUserName).HasColumnName(@"CreateUserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.CreateOfficeId).HasColumnName(@"CreateOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.CreateOfficeName).HasColumnName(@"CreateOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreateOfficeIdPath).HasColumnName(@"CreateOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UpdateUserId).HasColumnName(@"UpdateUserId").IsOptional().HasColumnType("int");
            Property(x => x.UpdateUserFullName).HasColumnName(@"UpdateUserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UpdateUserName).HasColumnName(@"UpdateUserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.UpdateOfficeId).HasColumnName(@"UpdateOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.CommentNo).HasColumnName(@"CommentNo").IsRequired().HasColumnType("int");
            Property(x => x.UpdateOfficeName).HasColumnName(@"UpdateOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UpdateOfficeIdPath).HasColumnName(@"UpdateOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            InitializePartial();
        }
        partial void InitializePartial();
    }
}
