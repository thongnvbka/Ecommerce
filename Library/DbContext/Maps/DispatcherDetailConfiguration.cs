using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // DispatcherDetail

    public partial class DispatcherDetailConfiguration : EntityTypeConfiguration<DispatcherDetail>
    {
        public DispatcherDetailConfiguration()
            : this("dbo")
        {
        }

        public DispatcherDetailConfiguration(string schema)
        {
            ToTable("DispatcherDetail", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.DispatcherId).HasColumnName(@"DispatcherId").IsRequired().HasColumnType("int");
            Property(x => x.DispatcherCode).HasColumnName(@"DispatcherCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.FromDispatcherId).HasColumnName(@"FromDispatcherId").IsOptional().HasColumnType("int");
            Property(x => x.FromDispatcherCode).HasColumnName(@"FromDispatcherCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ToDispatcherId).HasColumnName(@"ToDispatcherId").IsOptional().HasColumnType("int");
            Property(x => x.ToDispatcherCode).HasColumnName(@"ToDispatcherCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.TransportPartnerId).HasColumnName(@"TransportPartnerId").IsRequired().HasColumnType("int");
            Property(x => x.TransportPartnerName).HasColumnName(@"TransportPartnerName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.TransportMethodId).HasColumnName(@"TransportMethodId").IsRequired().HasColumnType("int");
            Property(x => x.TransportMethodName).HasColumnName(@"TransportMethodName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.EntrepotId).HasColumnName(@"EntrepotId").IsOptional().HasColumnType("int");
            Property(x => x.EntrepotName).HasColumnName(@"EntrepotName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WalletId).HasColumnName(@"WalletId").IsRequired().HasColumnType("int");
            Property(x => x.WalletCode).HasColumnName(@"WalletCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Amount).HasColumnName(@"Amount").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Weight).HasColumnName(@"Weight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.WeightActual).HasColumnName(@"WeightActual").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.WeightConverted).HasColumnName(@"WeightConverted").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.Volume).HasColumnName(@"Volume").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Value).HasColumnName(@"Value").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PackageNo).HasColumnName(@"PackageNo").IsRequired().HasColumnType("int");
            Property(x => x.Size).HasColumnName(@"Size").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.FromTransportPartnerId).HasColumnName(@"FromTransportPartnerId").IsOptional().HasColumnType("int");
            Property(x => x.FromTransportPartnerName).HasColumnName(@"FromTransportPartnerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FromTransportMethodId).HasColumnName(@"FromTransportMethodId").IsOptional().HasColumnType("int");
            Property(x => x.FromTransportMethodName).HasColumnName(@"FromTransportMethodName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FromEntrepotId).HasColumnName(@"FromEntrepotId").IsOptional().HasColumnType("int");
            Property(x => x.FromEntrepotName).HasColumnName(@"FromEntrepotName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ToTransportPartnerId).HasColumnName(@"ToTransportPartnerId").IsOptional().HasColumnType("int");
            Property(x => x.ToTransportPartnerName).HasColumnName(@"ToTransportPartnerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ToTransportPartnerTime).HasColumnName(@"ToTransportPartnerTime").IsOptional().HasColumnType("datetime");
            Property(x => x.ToTransportMethodId).HasColumnName(@"ToTransportMethodId").IsOptional().HasColumnType("int");
            Property(x => x.ToTransportMethodName).HasColumnName(@"ToTransportMethodName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ToEntrepotId).HasColumnName(@"ToEntrepotId").IsOptional().HasColumnType("int");
            Property(x => x.ToEntrepotName).HasColumnName(@"ToEntrepotName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }
}
