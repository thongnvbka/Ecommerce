using Library.DbContext.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DbContext.Maps
{
    public partial class ComplainOrderConfiguration : EntityTypeConfiguration<ComplainOrder>
    {
        public ComplainOrderConfiguration()
            : this("dbo")
        {
        }

        public ComplainOrderConfiguration(string schema)
        {
            ToTable("ComplainOrder", schema);
            HasKey(x => x.Id);

            Property(x => x.ComplainId).HasColumnName(@"ComplainId").IsRequired().HasColumnType("bigint");
            Property(x => x.OrderDetailId).HasColumnName(@"OrderDetailId").IsRequired().HasColumnType("int");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(2000);
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.LinkOrder).HasColumnName(@"LinkOrder").IsOptional().HasColumnType("int");
            InitializePartial();
        }
        partial void InitializePartial();
    }
}
