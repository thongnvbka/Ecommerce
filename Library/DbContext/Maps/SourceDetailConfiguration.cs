using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{

    // SourceDetail

    public partial class SourceDetailConfiguration : EntityTypeConfiguration<SourceDetail>
    {
        public SourceDetailConfiguration()
            : this("dbo")
        {
        }

        public SourceDetailConfiguration(string schema)
        {
            ToTable("SourceDetail", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.SourceId).HasColumnName(@"SourceId").IsRequired().HasColumnType("bigint");
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.Quantity).HasColumnName(@"Quantity").IsRequired().HasColumnType("int");
            Property(x => x.BeginAmount).HasColumnName(@"BeginAmount").IsOptional().HasColumnType("int");
            Property(x => x.Price).HasColumnName(@"Price").HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.ExchangeRate).HasColumnName(@"ExchangeRate").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.ExchangePrice).HasColumnName(@"ExchangePrice").HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalPrice).HasColumnName(@"TotalPrice").HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalExchange).HasColumnName(@"TotalExchange").HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.Status).HasColumnName(@"Status").HasColumnType("tinyint");
            Property(x => x.Link).HasColumnName(@"Link").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.QuantityBooked).HasColumnName(@"QuantityBooked").HasColumnType("int");
            Property(x => x.Properties).HasColumnName(@"Properties").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.HashTag).HasColumnName(@"HashTag").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.CategoryId).HasColumnName(@"CategoryId").IsOptional().HasColumnType("int");
            Property(x => x.CategoryName).HasColumnName(@"CategoryName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdate).HasColumnName(@"LastUpdate").IsRequired().HasColumnType("datetime");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.UniqueCode).HasColumnName(@"UniqueCode").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Size).HasColumnName(@"Size").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Color).HasColumnName(@"Color").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ImagePath1).HasColumnName(@"ImagePath1").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.ImagePath2).HasColumnName(@"ImagePath2").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.ImagePath3).HasColumnName(@"ImagePath3").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.ImagePath4).HasColumnName(@"ImagePath4").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
