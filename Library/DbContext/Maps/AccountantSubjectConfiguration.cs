using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // AccountantSubject
    
    public partial class AccountantSubjectConfiguration : EntityTypeConfiguration<AccountantSubject>
    {
        public AccountantSubjectConfiguration()
            : this("dbo")
        {
        }

        public AccountantSubjectConfiguration(string schema)
        {
            ToTable("AccountantSubject", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Idd).HasColumnName(@"Idd").IsOptional().HasColumnType("int");
            Property(x => x.SubjectName).HasColumnName(@"SubjectName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.SubjectNote).HasColumnName(@"SubjectNote").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdated).HasColumnName(@"LastUpdated").IsRequired().HasColumnType("datetime");
            Property(x => x.IsIdSystem).HasColumnName(@"IsIdSystem").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
