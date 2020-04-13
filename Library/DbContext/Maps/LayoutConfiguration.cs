using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Layout

    public partial class LayoutConfiguration : EntityTypeConfiguration<Layout>
    {
        public LayoutConfiguration()
            : this("dbo")
        {
        }

        public LayoutConfiguration(string schema)
        {
            ToTable("Layout", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.WarehouseId).HasColumnName(@"WarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ParentLayoutId).HasColumnName(@"ParentLayoutId").IsOptional().HasColumnType("int");
            Property(x => x.ParentLayoutName).HasColumnName(@"ParentLayoutName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IdPath).HasColumnName(@"IdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.NamePath).HasColumnName(@"NamePath").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Length).HasColumnName(@"Length").IsOptional().HasColumnType("int");
            Property(x => x.Width).HasColumnName(@"Width").IsOptional().HasColumnType("int");
            Property(x => x.Height).HasColumnName(@"Height").IsOptional().HasColumnType("int");
            Property(x => x.MaxWeight).HasColumnName(@"MaxWeight").IsOptional().HasColumnType("int");
            Property(x => x.ChildNo).HasColumnName(@"ChildNo").IsOptional().HasColumnType("int");
            Property(x => x.UnsignName).HasColumnName(@"UnsignName").IsOptional().HasColumnType("varchar").HasMaxLength(300);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
