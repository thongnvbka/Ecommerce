using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // DeliverySpend
    
    public partial class DeliverySpendConfiguration : EntityTypeConfiguration<DeliverySpend>
    {
        public DeliverySpendConfiguration()
            : this("dbo")
        {
        }

        public DeliverySpendConfiguration(string schema)
        {
            ToTable("DeliverySpend", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.DeliveryCode).HasColumnName(@"DeliveryCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.DeliveryId).HasColumnName(@"DeliveryId").IsRequired().HasColumnType("int");
            Property(x => x.SpendName).HasColumnName(@"SpendName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.SpendId).HasColumnName(@"SpendId").IsRequired().HasColumnType("tinyint");
            Property(x => x.Value).HasColumnName(@"Value").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
