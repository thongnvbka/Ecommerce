using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    public partial class CustomerWalletConfiguration : EntityTypeConfiguration<CustomerWallet>
    {
        public CustomerWalletConfiguration()
            : this("dbo")
        {
        }

        public CustomerWalletConfiguration(string schema)
        {
            ToTable("CustomerWallet", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Idd).HasColumnName(@"Idd").IsOptional().HasColumnType("int");
            Property(x => x.IdPath).HasColumnName(@"IdPath").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(400);
            Property(x => x.NamePath).HasColumnName(@"NamePath").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ParentId).HasColumnName(@"ParentId").IsRequired().HasColumnType("int");
            Property(x => x.ParentName).HasColumnName(@"ParentName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Operator).HasColumnName(@"Operator").IsOptional().HasColumnType("bit");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.IsParent).HasColumnName(@"IsParent").IsRequired().HasColumnType("bit");
            Property(x => x.IsIdSystem).HasColumnName(@"IsIdSystem").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
