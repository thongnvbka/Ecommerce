using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{

    // OrderComment

    public partial class OrderCommentConfiguration : EntityTypeConfiguration<OrderComment>
    {
        public OrderCommentConfiguration()
            : this("dbo")
        {
        }

        public OrderCommentConfiguration(string schema)
        {
            ToTable("OrderComment", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsOptional().HasColumnType("int");
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.IsRead).HasColumnName(@"IsRead").IsOptional().HasColumnType("bit");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.UserName).HasColumnName(@"UserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsRequired().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsOptional().HasColumnType("tinyint");
            Property(x => x.CommentType).HasColumnName(@"CommentType").IsOptional().HasColumnType("tinyint");
            Property(x => x.UserOffice).HasColumnName(@"UserOffice").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.UserPhone).HasColumnName(@"UserPhone").IsOptional().HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.GroupId).HasColumnName(@"GroupId").IsOptional().HasColumnType("int");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
