using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    public partial class OrderLogConfiguration : EntityTypeConfiguration<OrderLog>
    {
        public OrderLogConfiguration()
            : this("dbo")
        {
        }

        public OrderLogConfiguration(string schema)
        {
            ToTable("OrderLog", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.DataBefore).HasColumnName(@"DataBefore").IsOptional().HasColumnType("nvarchar").HasMaxLength(3000);
            Property(x => x.DataAfter).HasColumnName(@"DataAfter").IsOptional().HasColumnType("nvarchar").HasMaxLength(3000);
            Property(x => x.Content).HasColumnName(@"Content").IsOptional().HasColumnType("nvarchar").HasMaxLength(3000);
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.UserOfficeId).HasColumnName(@"UserOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.UserOfficeName).HasColumnName(@"UserOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UserOfficePath).HasColumnName(@"UserOfficePath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerFullName).HasColumnName(@"CustomerFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
