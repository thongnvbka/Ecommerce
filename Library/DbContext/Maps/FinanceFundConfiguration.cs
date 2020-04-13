using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // FinanceFund
    
    public partial class FinanceFundConfiguration : EntityTypeConfiguration<FinanceFund>
    {
        public FinanceFundConfiguration()
           : this("dbo")
        {
        }

        public FinanceFundConfiguration(string schema)
        {
            ToTable("FinanceFund", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.IdPath).HasColumnName(@"IdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.NamePath).HasColumnName(@"NamePath").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ParentId).HasColumnName(@"ParentId").IsRequired().HasColumnType("int");
            Property(x => x.ParentName).HasColumnName(@"ParentName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.UserCode).HasColumnName(@"UserCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.UserEmail).HasColumnName(@"UserEmail").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.UserPhone).HasColumnName(@"UserPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.NameBank).HasColumnName(@"NameBank").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Department).HasColumnName(@"Department").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.BankAccountNumber).HasColumnName(@"BankAccountNumber").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.Balance).HasColumnName(@"Balance").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.IsParent).HasColumnName(@"IsParent").IsRequired().HasColumnType("bit");
            Property(x => x.Maxlength).HasColumnName(@"Maxlength").IsOptional().HasColumnType("nvarchar").HasMaxLength(4000);
            Property(x => x.CardName).HasColumnName(@"CardName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.CardId).HasColumnName(@"CardId").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.CardBank).HasColumnName(@"CardBank").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.CardBranch).HasColumnName(@"CardBranch").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.Currency).HasColumnName(@"Currency").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
