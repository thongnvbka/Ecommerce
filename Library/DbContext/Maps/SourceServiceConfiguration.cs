using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{

    // SourceService

    public partial class SourceServiceConfiguration : EntityTypeConfiguration<SourceService>
    {
        public SourceServiceConfiguration()
            : this("dbo")
        {
        }

        public SourceServiceConfiguration(string schema)
        {
            ToTable("SourceService", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsRequired().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.LastUpdateDate).HasColumnName(@"LastUpdateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.Price).HasColumnName(@"Price").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.CreateId).HasColumnName(@"CreateId").IsOptional().HasColumnType("int");
            Property(x => x.CreateName).HasColumnName(@"CreateName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.UpdateId).HasColumnName(@"UpdateId").IsOptional().HasColumnType("int");
            Property(x => x.UpdateName).HasColumnName(@"UpdateName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.LevelId).HasColumnName(@"LevelId").IsRequired().HasColumnType("tinyint");
            Property(x => x.LevelName).HasColumnName(@"LevelName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsFirst).HasColumnName(@"IsFirst").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
