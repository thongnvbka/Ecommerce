using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // CustomerCallHistory
    
    public partial class CustomerCallHistoryConfiguration : EntityTypeConfiguration<CustomerCallHistory>
    {
        public CustomerCallHistoryConfiguration()
            : this("dbo")
        {
        }

        public CustomerCallHistoryConfiguration(string schema)
        {
            ToTable("CustomerCallHistory", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UserName).HasColumnName(@"UserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.TitleId).HasColumnName(@"TitleId").IsRequired().HasColumnType("int");
            Property(x => x.TitleName).HasColumnName(@"TitleName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OfficeId).HasColumnName(@"OfficeId").IsRequired().HasColumnType("int");
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OfficeIdPath).HasColumnName(@"OfficeIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.OfficeNamePath).HasColumnName(@"OfficeNamePath").IsRequired().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerVipId).HasColumnName(@"CustomerVipId").IsRequired().HasColumnType("tinyint");
            Property(x => x.CustomerVipName).HasColumnName(@"CustomerVipName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.IsLast).HasColumnName(@"IsLast").IsRequired().HasColumnType("bit");
            Property(x => x.ObjectId).HasColumnName(@"ObjectId").IsOptional().HasColumnType("int");
            Property(x => x.Content).HasColumnName(@"Content").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
