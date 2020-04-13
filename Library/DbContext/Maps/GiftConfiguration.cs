using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Gift
    
    public partial class GiftConfiguration : EntityTypeConfiguration<Gift>
    {
        public GiftConfiguration()
            : this("dbo")
        {
        }

        public GiftConfiguration(string schema)
        {
            ToTable("Gift", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.DiscountType).HasColumnName(@"DiscountType").IsRequired().HasColumnType("tinyint");
            Property(x => x.DiscountValue).HasColumnName(@"DiscountValue").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.FromDate).HasColumnName(@"FromDate").IsRequired().HasColumnType("datetime");
            Property(x => x.ToDate).HasColumnName(@"ToDate").IsRequired().HasColumnType("datetime");
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.Quantity).HasColumnName(@"Quantity").IsRequired().HasColumnType("int");
            Property(x => x.UseNo).HasColumnName(@"UseNo").IsRequired().HasColumnType("int");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.HashTag).HasColumnName(@"HashTag").IsRequired().HasColumnType("nvarchar(max)");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
