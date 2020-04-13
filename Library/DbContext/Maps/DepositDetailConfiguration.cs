using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // DepositDetail

    public partial class DepositDetailConfiguration : EntityTypeConfiguration<DepositDetail>
    {
        public DepositDetailConfiguration()
            : this("dbo")
        {
        }

        public DepositDetailConfiguration(string schema)
        {
            ToTable("DepositDetail", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.UpdateDate).HasColumnName(@"UpdateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.DepositId).HasColumnName(@"DepositId").IsRequired().HasColumnType("int");
            Property(x => x.LadingCode).HasColumnName(@"LadingCode").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.Weight).HasColumnName(@"Weight").IsRequired().HasColumnType("float");
            Property(x => x.CategoryId).HasColumnName(@"CategoryId").IsRequired().HasColumnType("int");
            Property(x => x.CategoryName).HasColumnName(@"CategoryName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ProductName).HasColumnName(@"ProductName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Quantity).HasColumnName(@"Quantity").IsRequired().HasColumnType("int");
            Property(x => x.Size).HasColumnName(@"Size").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Image).HasColumnName(@"Image").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.PacketNumber).HasColumnName(@"PacketNumber").IsRequired().HasColumnType("int");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Long).HasColumnName(@"Long").IsRequired().HasColumnType("float");
            Property(x => x.High).HasColumnName(@"High").IsRequired().HasColumnType("float");
            Property(x => x.Wide).HasColumnName(@"Wide").IsRequired().HasColumnType("float");
            Property(x => x.ListCode).HasColumnName(@"ListCode").HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.ShipTq).HasColumnName(@"ShipTq").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
