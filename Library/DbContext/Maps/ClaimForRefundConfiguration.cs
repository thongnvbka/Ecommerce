using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    using System;

    // ClaimForRefund
    
    public partial class ClaimForRefundConfiguration : EntityTypeConfiguration<ClaimForRefund>
    {
        public ClaimForRefundConfiguration()
            : this("dbo")
        {
        }

        public ClaimForRefundConfiguration(string schema)
        {
            ToTable("ClaimForRefund", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsOptional().HasColumnType("int");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("int");
            Property(x => x.TicketId).HasColumnName(@"TicketId").IsOptional().HasColumnType("int");
            Property(x => x.TicketCode).HasColumnName(@"TicketCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.TicketCreated).HasColumnName(@"TicketCreated").IsOptional().HasColumnType("datetime");
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerCode).HasColumnName(@"CustomerCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.CustomerFullName).HasColumnName(@"CustomerFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerAddress).HasColumnName(@"CustomerAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CustomerOfficeId).HasColumnName(@"CustomerOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerOfficeName).HasColumnName(@"CustomerOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerOfficePath).HasColumnName(@"CustomerOfficePath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OrderUserId).HasColumnName(@"OrderUserId").IsOptional().HasColumnType("int");
            Property(x => x.OrderUserCode).HasColumnName(@"OrderUserCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.OrderUserFullName).HasColumnName(@"OrderUserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.OrderUserEmail).HasColumnName(@"OrderUserEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OrderUserPhone).HasColumnName(@"OrderUserPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.OrderUserOfficeId).HasColumnName(@"OrderUserOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.OrderUserOfficeName).HasColumnName(@"OrderUserOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OrderUserOfficePath).HasColumnName(@"OrderUserOfficePath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.SupportId).HasColumnName(@"SupportId").IsOptional().HasColumnType("int");
            Property(x => x.SupportCode).HasColumnName(@"SupportCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.SupportFullName).HasColumnName(@"SupportFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.SupportEmail).HasColumnName(@"SupportEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.AccountantId).HasColumnName(@"AccountantId").IsOptional().HasColumnType("int");
            Property(x => x.AccountantCode).HasColumnName(@"AccountantCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.AccountantFullName).HasColumnName(@"AccountantFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.AccountantEmail).HasColumnName(@"AccountantEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ExchangeRate).HasColumnName(@"ExchangeRate").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserCode).HasColumnName(@"UserCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.UserName).HasColumnName(@"UserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.UserEmail).HasColumnName(@"UserEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UserPhone).HasColumnName(@"UserPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.OfficeId).HasColumnName(@"OfficeId").IsOptional().HasColumnType("int");
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OfficeIdPath).HasColumnName(@"OfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdated).HasColumnName(@"LastUpdated").IsRequired().HasColumnType("datetime");
            Property(x => x.MoneyRefund).HasColumnName(@"MoneyRefund").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.RealTotalRefund).HasColumnName(@"RealTotalRefund").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.MoneyOrderRefund).HasColumnName(@"MoneyOrderRefund").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.MoneyOrderRefundDicker).HasColumnName(@"MoneyOrderRefundDicker").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.SupporterMoneyRequest).HasColumnName(@"SupporterMoneyRequest").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.CurrencyDiscount).HasColumnName(@"CurrencyDiscount").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.NoteOrderer).HasColumnName(@"NoteOrderer").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.NoteSupporter).HasColumnName(@"NoteSupporter").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.NoteAccountanter).HasColumnName(@"NoteAccountanter").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.ApproverId).HasColumnName(@"ApproverId").IsOptional().HasColumnType("int");
            Property(x => x.ApproverName).HasColumnName(@"ApproverName").IsOptional().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.MoneyOther).HasColumnName(@"MoneyOther").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.ReasonCancel).HasColumnName(@"ReasonCancel").IsOptional().HasColumnType("nvarchar(max)");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
