using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // User
    
    public partial class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
            : this("dbo")
        {
        }

        public UserConfiguration(string schema)
        {
            ToTable("User", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.UserName).HasColumnName(@"UserName").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.Password).HasColumnName(@"Password").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.FirstName).HasColumnName(@"FirstName").IsRequired().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.MidleName).HasColumnName(@"MidleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.LastName).HasColumnName(@"LastName").IsRequired().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.FullName).HasColumnName(@"FullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.UnsignName).HasColumnName(@"UnsignName").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Gender).HasColumnName(@"Gender").IsRequired().HasColumnType("tinyint");
            Property(x => x.Email).HasColumnName(@"Email").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdateUserId).HasColumnName(@"LastUpdateUserId").IsRequired().HasColumnType("int");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Birthday).HasColumnName(@"Birthday").IsOptional().HasColumnType("date");
            Property(x => x.StartDate).HasColumnName(@"StartDate").IsOptional().HasColumnType("date");
            Property(x => x.Avatar).HasColumnName(@"Avatar").IsOptional().HasColumnType("nvarchar").HasMaxLength(2000);
            Property(x => x.IsLockout).HasColumnName(@"IsLockout").IsRequired().HasColumnType("bit");
            Property(x => x.LastLockoutDate).HasColumnName(@"LastLockoutDate").IsOptional().HasColumnType("datetime");
            Property(x => x.LockoutToDate).HasColumnName(@"LockoutToDate").IsOptional().HasColumnType("datetime");
            Property(x => x.FirstLoginFailureDate).HasColumnName(@"FirstLoginFailureDate").IsOptional().HasColumnType("datetime");
            Property(x => x.LoginFailureCount).HasColumnName(@"LoginFailureCount").IsRequired().HasColumnType("tinyint");
            Property(x => x.IsSystem).HasColumnName(@"IsSystem").IsRequired().HasColumnType("bit");
            Property(x => x.Phone).HasColumnName(@"Phone").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.Mobile).HasColumnName(@"Mobile").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.NameBank).HasColumnName(@"NameBank").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Department).HasColumnName(@"Department").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.BankAccountNumber).HasColumnName(@"BankAccountNumber").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.IsCompany).HasColumnName(@"IsCompany").IsRequired().HasColumnType("bit");
            Property(x => x.TypeId).HasColumnName(@"TypeId").IsOptional().HasColumnType("int");
            Property(x => x.TypeIdd).HasColumnName(@"TypeIdd").IsOptional().HasColumnType("int");
            Property(x => x.TypeName).HasColumnName(@"TypeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.Websites).HasColumnName(@"Websites").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.Culture).HasColumnName(@"Culture").IsOptional().HasColumnType("varchar").HasMaxLength(10);

            InitializePartial();
        }
        partial void InitializePartial();
    }

}
