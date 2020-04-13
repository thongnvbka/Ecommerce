using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // FinanceAccount
    
    public partial class FinanceAccountConfiguration : EntityTypeConfiguration<FinanceAccount>
    {
        public FinanceAccountConfiguration()
            : this("dbo")
        {
        }

        public FinanceAccountConfiguration(string schema)
        {
            ToTable("FinanceAccount", schema);
            HasKey(x => x.AccountId);

            Property(x => x.AccountId).HasColumnName(@"AccountId").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Card).HasColumnName(@"Card").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.FullName).HasColumnName(@"FullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Phone).HasColumnName(@"Phone").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.Status).HasColumnName(@"Status").IsOptional().HasColumnType("bit");
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.UpdateDate).HasColumnName(@"UpdateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.LastUpdate).HasColumnName(@"LastUpdate").IsOptional().HasColumnType("datetime");
            Property(x => x.MoneyAvaiable).HasColumnName(@"MoneyAvaiable").IsOptional().HasColumnType("money").HasPrecision(19,4);
            Property(x => x.MoneyCurrent).HasColumnName(@"MoneyCurrent").IsOptional().HasColumnType("money").HasPrecision(19,4);
            Property(x => x.DeputyName).HasColumnName(@"DeputyName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.DeputyEmail).HasColumnName(@"DeputyEmail").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.DeputyPhone).HasColumnName(@"DeputyPhone").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.DeputyCard).HasColumnName(@"DeputyCard").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.DeputyAddress).HasColumnName(@"DeputyAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.FundId).HasColumnName(@"FundId").IsOptional().HasColumnType("int");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
