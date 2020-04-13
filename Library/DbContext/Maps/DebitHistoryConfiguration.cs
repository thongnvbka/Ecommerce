using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // DebitHistory
    
    public partial class DebitHistoryConfiguration : EntityTypeConfiguration<DebitHistory>
    {
        public DebitHistoryConfiguration()
            : this("dbo")
        {
        }

        public DebitHistoryConfiguration(string schema)
        {
            ToTable("DebitHistory", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.DebitCode).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.DebitId).HasColumnName(@"DebitId").IsOptional().HasColumnType("int");
            Property(x => x.DebitType).HasColumnName(@"DebitType").IsOptional().HasColumnType("tinyint");
            Property(x => x.DebitCode).HasColumnName(@"DebitCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Money).HasColumnName(@"Money").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsOptional().HasColumnType("int");
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsOptional().HasColumnType("tinyint");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.PayReceivableId).HasColumnName(@"PayReceivableId").IsOptional().HasColumnType("int");
            Property(x => x.PayReceivableIdd).HasColumnName(@"PayReceivableIdd").IsOptional().HasColumnType("int");
            Property(x => x.PayReceivableIName).HasColumnName(@"PayReceivableIName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsSystem).HasColumnName(@"IsSystem").IsRequired().HasColumnType("bit");
            Property(x => x.SubjectId).HasColumnName(@"SubjectId").IsOptional().HasColumnType("int");
            Property(x => x.SubjectCode).HasColumnName(@"SubjectCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.SubjectName).HasColumnName(@"SubjectName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.SubjectPhone).HasColumnName(@"SubjectPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.SubjectEmail).HasColumnName(@"SubjectEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.SubjectAddress).HasColumnName(@"SubjectAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.FinanceFundId).HasColumnName(@"FinanceFundId").IsOptional().HasColumnType("int");
            Property(x => x.FinanceFundName).HasColumnName(@"FinanceFundName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.FinanceFundBankAccountNumber).HasColumnName(@"FinanceFundBankAccountNumber").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.FinanceFundDepartment).HasColumnName(@"FinanceFundDepartment").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FinanceFundNameBank).HasColumnName(@"FinanceFundNameBank").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FinanceFundUserFullName).HasColumnName(@"FinanceFundUserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FinanceFundUserPhone).HasColumnName(@"FinanceFundUserPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.FinanceFundUserEmail).HasColumnName(@"FinanceFundUserEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.TreasureId).HasColumnName(@"TreasureId").IsOptional().HasColumnType("int");
            Property(x => x.TreasureName).HasColumnName(@"TreasureName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdated).HasColumnName(@"LastUpdated").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
