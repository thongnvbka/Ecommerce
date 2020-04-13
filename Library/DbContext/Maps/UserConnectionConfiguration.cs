using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // UserConnection
    
    public partial class UserConnectionConfiguration : EntityTypeConfiguration<UserConnection>
    {
        public UserConnectionConfiguration()
            : this("dbo")
        {
        }

        public UserConnectionConfiguration(string schema)
        {
            ToTable("UserConnection", schema);
            HasKey(x => new { x.ConnectionId, x.UserId });

            Property(x => x.ConnectionId).HasColumnName(@"ConnectionId").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.SessionId).HasColumnName(@"Session_ID").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(150);
            Property(x => x.UserName).HasColumnName(@"UserName").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(200);
            Property(x => x.OfficeId).HasColumnName(@"OfficeID").IsOptional().HasColumnType("int");
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(250);
            Property(x => x.TitleName).HasColumnName(@"TitleName").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.FullName).HasColumnName(@"FullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(250);
            Property(x => x.Image).HasColumnName(@"Image").IsOptional().IsUnicode(false).HasColumnType("varchar(max)");
            Property(x => x.Platform).HasColumnName(@"Platform").IsOptional().HasColumnType("int");
            Property(x => x.UnsignName).HasColumnName(@"UnsignName").IsOptional().IsUnicode(false).HasColumnType("varchar(max)");
            Property(x => x.UserType).HasColumnName(@"UserType").IsOptional().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
