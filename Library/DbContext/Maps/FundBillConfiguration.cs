using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // FundBill

    public partial class FundBillConfiguration : EntityTypeConfiguration<FundBill>
    {
        public FundBillConfiguration()
            : this("dbo")
        {
        }

        public FundBillConfiguration(string schema)
        {
            ToTable("FundBill", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.CurrencyFluctuations).HasColumnName(@"CurrencyFluctuations").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Increase).HasColumnName(@"Increase").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Diminishe).HasColumnName(@"Diminishe").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.CurencyStart).HasColumnName(@"CurencyStart").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.CurencyEnd).HasColumnName(@"CurencyEnd").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.AccountantSubjectId).HasColumnName(@"AccountantSubjectId").IsOptional().HasColumnType("int");
            Property(x => x.AccountantSubjectName).HasColumnName(@"AccountantSubjectName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.SubjectId).HasColumnName(@"SubjectId").IsOptional().HasColumnType("int");
            Property(x => x.SubjectCode).HasColumnName(@"SubjectCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.SubjectName).HasColumnName(@"SubjectName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.SubjectPhone).HasColumnName(@"SubjectPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.SubjectEmail).HasColumnName(@"SubjectEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FinanceFundId).HasColumnName(@"FinanceFundId").IsOptional().HasColumnType("int");
            Property(x => x.FinanceFundName).HasColumnName(@"FinanceFundName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.FinanceFundBankAccountNumber).HasColumnName(@"FinanceFundBankAccountNumber").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.FinanceFundDepartment).HasColumnName(@"FinanceFundDepartment").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FinanceFundNameBank).HasColumnName(@"FinanceFundNameBank").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FinanceFundUserFullName).HasColumnName(@"FinanceFundUserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FinanceFundUserPhone).HasColumnName(@"FinanceFundUserPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.FinanceFundUserEmail).HasColumnName(@"FinanceFundUserEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.TreasureId).HasColumnName(@"TreasureId").IsOptional().HasColumnType("int");
            Property(x => x.TreasureName).HasColumnName(@"TreasureName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserCode).HasColumnName(@"UserCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.UserName).HasColumnName(@"UserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UserApprovalId).HasColumnName(@"UserApprovalId").IsOptional().HasColumnType("int");
            Property(x => x.UserApprovalCode).HasColumnName(@"UserApprovalCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.UserApprovalName).HasColumnName(@"UserApprovalName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsOptional().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsOptional().HasColumnType("tinyint");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdated).HasColumnName(@"LastUpdated").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
