using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // LockHistory
    
    public partial class LockHistoryConfiguration : EntityTypeConfiguration<LockHistory>
    {
        public LockHistoryConfiguration()
            : this("dbo")
        {
        }

        public LockHistoryConfiguration(string schema)
        {
            ToTable("LockHistory", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.KeyLock).HasColumnName(@"KeyLock").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.UpdateTime).HasColumnName(@"UpdateTime").IsRequired().HasColumnType("datetime");
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("bigint");
            Property(x => x.UserName).HasColumnName(@"UserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.FullName).HasColumnName(@"FullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.State).HasColumnName(@"State").IsRequired().HasColumnType("tinyint");
            Property(x => x.ReasonUnlock).HasColumnName(@"ReasonUnlock").IsRequired().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.ObjectId).HasColumnName(@"ObjectId").IsRequired().HasColumnType("int");
            Property(x => x.ObjectName).HasColumnName(@"ObjectName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.IsLatest).HasColumnName(@"IsLatest").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
