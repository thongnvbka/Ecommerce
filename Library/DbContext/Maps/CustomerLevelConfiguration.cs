using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // CustomerLevel
    public partial class CustomerLevelConfiguration : EntityTypeConfiguration<CustomerLevel>
    {
        public CustomerLevelConfiguration()
          : this("dbo")
        {
        }

        public CustomerLevelConfiguration(string schema)
        {
            ToTable("CustomerLevel", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("bit");
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.UpdateDate).HasColumnName(@"UpdateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.UserName).HasColumnName(@"UserName").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.StartMoney).HasColumnName(@"StartMoney").IsRequired().HasColumnType("money").HasPrecision(19, 4);
            Property(x => x.EndMoney).HasColumnName(@"EndMoney").IsRequired().HasColumnType("money").HasPrecision(19, 4);
            Property(x => x.PercentDeposit).HasColumnName(@"PercentDeposit").IsRequired().HasColumnType("int");
            Property(x => x.Order).HasColumnName(@"Order").IsOptional().HasColumnType("tinyint");
            Property(x => x.Ship).HasColumnName(@"Ship").IsOptional().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
