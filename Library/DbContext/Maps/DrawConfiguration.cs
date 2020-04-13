using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{

    // Draw

    public partial class DrawConfiguration : EntityTypeConfiguration<Draw>
    {
        public DrawConfiguration()
            : this("dbo")
        {
        }

        public DrawConfiguration(string schema)
        {
            ToTable("Draw", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.CustomerCode).HasColumnName(@"CustomerCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.CardName).HasColumnName(@"CardName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.CardId).HasColumnName(@"CardId").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.CardBank).HasColumnName(@"CardBank").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.CardBranch).HasColumnName(@"CardBranch").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsRequired().IsOptional().HasColumnType("datetime");
            Property(x => x.LastUpdate).HasColumnName(@"LastUpdate").IsRequired().IsOptional().HasColumnType("datetime");
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserName).HasColumnName(@"UserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.Status).HasColumnName(@"Status").IsOptional().HasColumnType("tinyint");
            Property(x => x.Note).HasColumnName(@"Note").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.AdvanceMoney).HasColumnName(@"AdvanceMoney").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsRequired().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
