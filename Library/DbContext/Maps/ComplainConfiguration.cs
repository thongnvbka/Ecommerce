using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{

    // Complain

    public partial class ComplainConfiguration : EntityTypeConfiguration<Complain>
    {
        public ComplainConfiguration()
            : this("dbo")
        {
        }

        public ComplainConfiguration(string schema)
        {
            ToTable("Complain", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.TypeOrder).HasColumnName(@"TypeOrder").IsRequired().HasColumnType("tinyint");
            Property(x => x.TypeService).HasColumnName(@"TypeService").IsRequired().HasColumnType("int");
            Property(x => x.TypeServiceName).HasColumnName(@"TypeServiceName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.TypeServiceClose).HasColumnName(@"TypeServiceClose").IsOptional().HasColumnType("int");
            Property(x => x.TypeServiceCloseName).HasColumnName(@"TypeServiceCloseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ImagePath1).HasColumnName(@"ImagePath1").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.ImagePath2).HasColumnName(@"ImagePath2").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.ImagePath3).HasColumnName(@"ImagePath3").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.ImagePath4).HasColumnName(@"ImagePath4").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.ImagePath5).HasColumnName(@"ImagePath5").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.ImagePath6).HasColumnName(@"ImagePath6").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.Content).HasColumnName(@"Content").IsOptional().HasColumnType("nvarchar").HasMaxLength(2000);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsOptional().HasColumnType("tinyint");
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdateDate).HasColumnName(@"LastUpdateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsOptional().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.Status).HasColumnName(@"Status").IsOptional().HasColumnType("tinyint");
            Property(x => x.LastReply).HasColumnName(@"LastReply").IsOptional().HasColumnType("nvarchar").HasMaxLength(2000);
            Property(x => x.BigMoney).HasColumnName(@"BigMoney").IsOptional().HasColumnType("money").HasPrecision(19, 4);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.RequestMoney).HasColumnName(@"RequestMoney").IsOptional().HasColumnType("money").HasPrecision(19, 4);
            Property(x => x.ContentInternal).HasColumnName(@"ContentInternal").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.ContentInternalOrder).HasColumnName(@"ContentInternalOrder").IsOptional().HasColumnType("nvarchar(max)");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
