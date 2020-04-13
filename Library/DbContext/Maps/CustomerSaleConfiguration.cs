using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;
using System;

namespace Library.DbContext.Maps
{
    // CustomerSale
    
    public partial class CustomerSaleConfiguration : EntityTypeConfiguration<CustomerSale>
    {
        public CustomerSaleConfiguration()
            : this("dbo")
        {
        }

        public CustomerSaleConfiguration(string schema)
        {
            ToTable("CustomerSale", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.ImagePath).HasColumnName(@"ImagePath").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(600);
            Property(x => x.CardId).HasColumnName(@"CardId").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.CardNumber).HasColumnName(@"CardNumber").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.SaleShoping).HasColumnName(@"SaleShoping").IsRequired().HasColumnType("int");
            Property(x => x.SaleShiping).HasColumnName(@"SaleShiping").IsRequired().HasColumnType("int");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsOptional().HasColumnType("bit");
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsOptional().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.LevelId).HasColumnName(@"LevelId").IsOptional().HasColumnType("int");
            Property(x => x.LevelName).HasColumnName(@"LevelName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.UserCreateId).HasColumnName(@"UserCreateId").IsOptional().HasColumnType("int");
            Property(x => x.UserCreateName).HasColumnName(@"UserCreateName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.UpdateDate).HasColumnName(@"UpdateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.UserUpdateId).HasColumnName(@"UserUpdateId").IsOptional().HasColumnType("int");
            Property(x => x.UserUpdateName).HasColumnName(@"UserUpdateName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            InitializePartial();
        }

        private object Property(Func<object, object> p)
        {
            throw new NotImplementedException();
        }

        partial void InitializePartial();
    }

}
