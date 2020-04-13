using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{

    // NotifiCommon

    public partial class NotifiCommonConfiguration : EntityTypeConfiguration<NotifiCommon>
    {
        public NotifiCommonConfiguration()
            : this("dbo")
        {
        }

        public NotifiCommonConfiguration(string schema)
        {
            ToTable("NotifiCommon", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsRequired().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsRequired().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.Description).HasColumnName(@"Description").IsRequired().HasColumnType("ntext").IsMaxLength();
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.UpdateDate).HasColumnName(@"UpdateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.IsRead).HasColumnName(@"IsRead").IsRequired().HasColumnType("bit");
            Property(x => x.Title).HasColumnName(@"Title").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserName).HasColumnName(@"UserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(90);
            Property(x => x.Url).HasColumnName(@"Url").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("bit");
            Property(x => x.PublishDate).HasColumnName(@"PublishDate").IsOptional().HasColumnType("datetime");
            Property(x => x.ImagePath).HasColumnName(@"ImagePath").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
