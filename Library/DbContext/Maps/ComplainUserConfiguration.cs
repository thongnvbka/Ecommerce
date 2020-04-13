using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{

    // ComplainUser

    public partial class ComplainUserConfiguration : EntityTypeConfiguration<ComplainUser>
    {
        public ComplainUserConfiguration()
            : this("dbo")
        {
        }

        public ComplainUserConfiguration(string schema)
        {
            ToTable("ComplainUser", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.ComplainId).HasColumnName(@"ComplainId").IsRequired().HasColumnType("bigint");
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.Content).HasColumnName(@"Content").IsOptional().HasColumnType("ntext").IsMaxLength();
            Property(x => x.AttachFile).HasColumnName(@"AttachFile").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.UpdateDate).HasColumnName(@"UpdateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.UserRequestId).HasColumnName(@"UserRequestId").IsOptional().HasColumnType("int");
            Property(x => x.UserRequestName).HasColumnName(@"UserRequestName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.UserName).HasColumnName(@"UserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.IsRead).HasColumnName(@"IsRead").IsOptional().HasColumnType("bit");
            Property(x => x.IsCare).HasColumnName(@"IsCare").IsOptional().HasColumnType("bit");
            Property(x => x.IsInHouse).HasColumnName(@"IsInHouse").IsOptional().HasColumnType("bit");
            Property(x => x.OfficeId).HasColumnName(@"OfficeId").IsOptional().HasColumnType("int");
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OfficeIdPath).HasColumnName(@"OfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.GroupId).HasColumnName(@"GroupId").IsOptional().HasColumnType("int");
            Property(x => x.CommentType).HasColumnName(@"CommentType").IsOptional().HasColumnType("tinyint");
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsOptional().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
