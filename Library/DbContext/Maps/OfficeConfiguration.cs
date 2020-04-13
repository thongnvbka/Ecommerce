using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Office
    
    public partial class OfficeConfiguration : EntityTypeConfiguration<Office>
    {
        public OfficeConfiguration()
            : this("dbo")
        {
        }

        public OfficeConfiguration(string schema)
        {
            ToTable("Office", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UnsignedName).HasColumnName(@"UnsignedName").IsRequired().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.ShortName).HasColumnName(@"ShortName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.IdPath).HasColumnName(@"IdPath").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(400);
            Property(x => x.NamePath).HasColumnName(@"NamePath").IsOptional().HasColumnType("nvarchar").HasMaxLength(2000);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.ParentId).HasColumnName(@"ParentId").IsOptional().HasColumnType("int");
            Property(x => x.ParentName).HasColumnName(@"ParentName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Address).HasColumnName(@"Address").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.UserNo).HasColumnName(@"UserNo").IsRequired().HasColumnType("int");
            Property(x => x.TitleNo).HasColumnName(@"TitleNo").IsRequired().HasColumnType("int");
            Property(x => x.ChildNo).HasColumnName(@"ChildNo").IsRequired().HasColumnType("int");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdateUserId).HasColumnName(@"LastUpdateUserId").IsRequired().HasColumnType("int");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Culture).HasColumnName(@"Culture").IsOptional().HasColumnType("varchar").HasMaxLength(2);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
