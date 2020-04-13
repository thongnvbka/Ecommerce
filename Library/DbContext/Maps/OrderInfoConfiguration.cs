using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // OrderInfo
    
    public partial class OrderInfoConfiguration : EntityTypeConfiguration<OrderInfo>
    {
        public OrderInfoConfiguration()
            : this("dbo")
        {
        }

        public OrderInfoConfiguration(string schema)
        {
            ToTable("OrderInfo", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.CategoryName).HasColumnName(@"CategoryName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
