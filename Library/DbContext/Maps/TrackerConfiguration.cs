using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Tracker
    
    public partial class TrackerConfiguration : EntityTypeConfiguration<Tracker>
    {
        public TrackerConfiguration()
            : this("dbo")
        {
        }

        public TrackerConfiguration(string schema)
        {
            ToTable("Tracker", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Browser).HasColumnName(@"Browser").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.Version).HasColumnName(@"Version").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.Os).HasColumnName(@"OS").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(15);
            Property(x => x.PageUrl).HasColumnName(@"PageUrl").IsOptional().IsUnicode(false).HasColumnType("varchar(max)");
            Property(x => x.UrlReferrer).HasColumnName(@"UrlReferrer").IsOptional().IsUnicode(false).HasColumnType("varchar(max)");
            Property(x => x.SessionId).HasColumnName(@"SessionID").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(30);
            Property(x => x.Ip).HasColumnName(@"IP").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.InTime).HasColumnName(@"InTime").IsRequired().HasColumnType("datetime");
            Property(x => x.Country).HasColumnName(@"Country").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.City).HasColumnName(@"City").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.IsMobileDevice).HasColumnName(@"IsMobileDevice").IsRequired().HasColumnType("bit");
            Property(x => x.MobileDeviceManufacturer).HasColumnName(@"MobileDeviceManufacturer").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.WebsiteId).HasColumnName(@"WebsiteId").IsRequired().HasColumnType("tinyint");
            Property(x => x.Day).HasColumnName(@"Day").IsRequired().HasColumnType("tinyint");
            Property(x => x.Month).HasColumnName(@"Month").IsRequired().HasColumnType("tinyint");
            Property(x => x.Quater).HasColumnName(@"Quater").IsRequired().HasColumnType("tinyint");
            Property(x => x.Year).HasColumnName(@"Year").IsRequired().HasColumnType("smallint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
