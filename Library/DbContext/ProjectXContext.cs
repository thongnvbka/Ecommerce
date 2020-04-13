using Library.DbContext.Entities;
using Library.DbContext.Maps;
using System.Data.Entity;
using Library.DbContext.Results;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Library.DbContext
{
    public partial class ProjectXContext : System.Data.Entity.DbContext
    {
        public DbSet<App> Apps { get; set; } // App
        public DbSet<Bag> Bags { get; set; } // Bag
        public DbSet<BagPackage> BagPackages { get; set; } // BagPackage
        public DbSet<Category> Categories { get; set; } // Category
        public DbSet<ChangePasswordLog> ChangePasswordLogs { get; set; } // ChangePasswordLog
        public DbSet<ConfigLoginFailure> ConfigLoginFailures { get; set; } // ConfigLoginFailure
        public DbSet<Customer> Customers { get; set; } // Customer
        public DbSet<Gift> Gifts { get; set; } // Gift
        public DbSet<GroupPermision> GroupPermisions { get; set; } // GroupPermision
        public DbSet<HashTag> HashTags { get; set; } // HashTag
        public DbSet<HistorySatu> HistorySatus { get; set; } // HistorySatus
        public DbSet<Layout> Layouts { get; set; } // Layout
        public DbSet<LockHistory> LockHistories { get; set; } // LockHistory
        public DbSet<Module> Modules { get; set; } // Module
        public DbSet<Office> Offices { get; set; } // Office
        public DbSet<Order> Orders { get; set; } // Order
        public DbSet<OrderAddress> OrderAddresses { get; set; } // OrderAddress
        public DbSet<OrderDetail> OrderDetails { get; set; } // OrderDetail
        public DbSet<OrderExchange> OrderExchanges { get; set; } // OrderExchange
        public DbSet<OrderInfo> OrderInfoes { get; set; } // OrderInfo
        public DbSet<OrderPackage> OrderPackages { get; set; } // OrderPackage
        public DbSet<OrderService> OrderServices { get; set; } // OrderService
        public DbSet<OrderProcessItem> OrderProcessItem { get; set; } // OrderProcessItem
        public DbSet<PackageTranport> PackageTranports { get; set; } // PackageTranport
        public DbSet<Page> Pages { get; set; } // Page
        public DbSet<PermissionAction> PermissionActions { get; set; } // PermissionAction
        public DbSet<Position> Positions { get; set; } // Position
        public DbSet<RoleAction> RoleActions { get; set; } // RoleAction
        public DbSet<Shop> Shops { get; set; } // Shops
        public DbSet<Entities.System> Systems { get; set; } // System
        public DbSet<Title> Titles { get; set; } // Title
        public DbSet<Tracker> Trackers { get; set; } // Tracker
        public DbSet<Transaction> Transactions { get; set; } // Transaction
        public DbSet<User> Users { get; set; } // User
        public DbSet<UserPosition> UserPositions { get; set; } // UserPosition
        public DbSet<Warehouse> Warehouses { get; set; } // Warehouse
        public DbSet<LogAction> LogActions { get; set; } // LogAction
        public DbSet<LogLogin> LogLogins { get; set; } // LogLogin
        public DbSet<LogSystem> LogSystems { get; set; } // LogSystem
        public DbSet<FinanceAccount> FinanceAccounts { get; set; } // FinanceAccount
        public DbSet<FinanceFund> FinanceFunds { get; set; } // FinanceFund
        public DbSet<CustomerLogLogin> CustomerLogLogins { get; set; } // CustomerLogLogin
        public DbSet<CustomerLog> CustomerLogs { get; set; } // CustomerLog
        public DbSet<Province> Provinces { get; set; } // Province
        public DbSet<District> Districts { get; set; } // District
        public DbSet<Ward> Wards { get; set; } // Ward
        public DbSet<Deposit> Deposits { get; set; } // Deposit
        public DbSet<DepositDetail> DepositDetails { get; set; } // DepositDetail
        public DbSet<HistoryPackage> HistoryPackages { get; set; } // HistoryPackage
        public DbSet<OrderComment> OrderComments { get; set; } // OrderComment
        public DbSet<CustomerSale> CustomerSales { get; set; } //Customer Sale
        public DbSet<OrderHistory> OrderHistories { get; set; } // OrderHistory
        public DbSet<CustomerLevel> CustomerLevels { get; set; }//customer level
        public DbSet<CustomerConfigLevel> CustomerConfigLevels { get; set; }// Customerconfig level
        public DbSet<CustomerType> CustomerTypes { get; set; } // CustomerTypes
        public DbSet<Treasure> Treasures { get; set; } // Treasure
        public DbSet<ImportWarehouse> ImportWarehouse { get; set; } // ImportWarehouse
        public DbSet<ExportWarehouse> ExportWarehouse { get; set; } // ExportWarehouse
        public DbSet<ExportWarehouseDetail> ExportWarehouseDetails { get; set; } // ExportWarehouseDetails
        public DbSet<PackingList> PackingList { get; set; } // PackingList
        public DbSet<Complain> Complains { get; set; } // Complain
        public DbSet<ComplainUser> ComplainUsers { get; set; } // ComplainUser
        public DbSet<Wallet> Wallet { get; set; } // Wallet
        public DbSet<Source> Sources { get; set; } // Source
        public DbSet<SourceDetail> SourceDetails { get; set; } // SourceDetail
        public DbSet<SourceSupplier> SourceSuppliers { get; set; } // SourceSupplier
        public DbSet<FundBill> FundBill { get; set; } // FundBill
        public DbSet<AccountantSubject> AccountantSubject { get; set; } // AccountantSubject
        public DbSet<RechargeBill> RechargeBill { get; set; } // RechargeBill
        public DbSet<SourceService> SourceServices { get; set; } // SourceService
        public DbSet<SourceServiceCustomer> SourceServiceCustomers { get; set; } // SourceServiceCustomer
        public DbSet<ImportWarehouseDetail> ImportWarehouseDetails { get; set; } // ImportWarehouseDetails
        public DbSet<Draw> Draws { get; set; } // Draw
        public DbSet<WalletDetail> WalletDetails { get; set; } // WalletDetails
        public DbSet<Dispatcher> Dispatchers { get; set; } // Dispatcher
        public DbSet<DispatcherDetail> DispatcherDetails { get; set; } // DispatcherDetail
        public DbSet<Delivery> Delivery { get; set; } // Delivery
        public DbSet<DeliveryDetail> DeliveryDetails { get; set; } // DispatcherDetail
        public DbSet<DeliverySpend> DeliverySpends { get; set; } 
        public DbSet<PutAway> PutAways { get; set; }
        public DbSet<PutAwayDetail> PutAwayDetails { get; set; }
        public DbSet<PotentialCustomer> PotentialCustomers { get; set; } // PotentialCustomer
        public DbSet<OrderContractCode> OrderContractCodes { get; set; }
        public DbSet<ClaimForRefund> ClaimForRefund { get; set; }
        public DbSet<ClaimForRefundDetail> ClaimForRefundDetail { get; set; }
        public DbSet<Notification> Notifications { get; set; } // Notification
        public DbSet<NotifiCommon> NotifiCommons { get; set; } // NotifiCommon
        public DbSet<NotifiCustomer> NotifiCustomers { get; set; } // NotifiCustomer
        public DbSet<Recent> Recents { get; set; } // Recents
        public DbSet<OrderDetailCounting> OrderDetailCountings { get; set; } // Recents
        public DbSet<Partner> Partners { get; set; } // Recents
        public DbSet<NotifyRealTime> NotifyRealTimes { get; set; } // NotifyRealTimes
        public DbSet<MessageRealTime> MessageRealTimes { get; set; } // MessageRealTimes
        public DbSet<MessageUser> MessageUsers { get; set; } // MessageUsers
        public DbSet<AttachmentMessage> AttachmentMessages { get; set; } // Recents
        public DbSet<Attachment> Attachments { get; set; } // Attachments
        public DbSet<SendEmail> SendEmails { get; set; } // SendEmails
        public DbSet<SendEmailResult> SendEmailResults { get; set; } // SendEmailResults
        public DbSet<UserConnection> UserConnections { get; set; } // SendEmailResults
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<GroupChatContent> GroupChatContents { get; set; }
        public DbSet<GroupChatLike> GroupChatLikes { get; set; }
        public DbSet<GroupChatRead> GroupChatReads { get; set; }
        public DbSet<GroupChatUser> GroupChatUsers { get; set; }
        public DbSet<ChatFilesAttach> ChatFilesAttachs { get; set; }
        public DbSet<Entrepot> Entrepots { get; set; }
        public DbSet<RequestShip> RequestShips { get; set; }
        public DbSet<CustomerWallet> CustomerWallets { get; set; }
        public DbSet<PayReceivable> PayReceivables { get; set; }
        public DbSet<Debit> Debit { get; set; }
        public DbSet<DebitHistory> DebitHistorys { get; set; }
        public DbSet<TransportMethod> TransportMethods { get; set; }
        public DbSet<OrderLog> OrderLogs { get; set; }
        public DbSet<PackageNoCode> PackageNoCodes { get; set; }
        public DbSet<OrderReason> OrderReasons { get; set; }
        public DbSet<OrderRefund> OrderRefunds { get; set; }
        public DbSet<OrderRefundDetail> OrderRefundDetails { get; set; }
        public DbSet<CustomerCallHistory> CustomerCallHistorys { get; set; }
        public DbSet<ComplainType> ComplainTypes { get; set; }
        public DbSet<PackageHistory> PackageHistories { get; set; }
        public DbSet<ComplainHistory> ComplainHistories { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TransferDetail> TransferDetails { get; set; }
        public DbSet<PackageNote> PackageNotes { get; set; }
        public DbSet<OrderServiceOther> OrderServiceOthers { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<DebitReport> DebitReports { get; set; }
        public DbSet<ComplainOrder> ComplainOrders { get; set; }
        static ProjectXContext()
        {
            Database.SetInitializer<ProjectXContext>(null);
        }

        public ProjectXContext()
            : base("Name=FinGroupContext")
        {
            InitializePartial();
        }

        public ProjectXContext(string connectionString)
            : base(connectionString)
        {
            InitializePartial();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new AppConfiguration());
            modelBuilder.Configurations.Add(new DispatcherDetailConfiguration());
            modelBuilder.Configurations.Add(new DispatcherConfiguration());
            modelBuilder.Configurations.Add(new WalletDetailConfiguration());
            modelBuilder.Configurations.Add(new ImportWarehouseDetailConfiguration());
            modelBuilder.Configurations.Add(new BagConfiguration());
            modelBuilder.Configurations.Add(new BagPackageConfiguration());
            modelBuilder.Configurations.Add(new CategoryConfiguration());
            modelBuilder.Configurations.Add(new ChangePasswordLogConfiguration());
            modelBuilder.Configurations.Add(new ConfigLoginFailureConfiguration());
            modelBuilder.Configurations.Add(new CustomerConfiguration());
            modelBuilder.Configurations.Add(new GiftConfiguration());
            modelBuilder.Configurations.Add(new GroupPermisionConfiguration());
            modelBuilder.Configurations.Add(new HashTagConfiguration());
            modelBuilder.Configurations.Add(new HistorySatuConfiguration());
            modelBuilder.Configurations.Add(new LayoutConfiguration());
            modelBuilder.Configurations.Add(new LockHistoryConfiguration());
            modelBuilder.Configurations.Add(new ModuleConfiguration());
            modelBuilder.Configurations.Add(new OfficeConfiguration());
            modelBuilder.Configurations.Add(new OrderConfiguration());
            modelBuilder.Configurations.Add(new OrderAddressConfiguration());
            modelBuilder.Configurations.Add(new OrderDetailConfiguration());
            modelBuilder.Configurations.Add(new OrderExchangeConfiguration());
            modelBuilder.Configurations.Add(new OrderInfoConfiguration());
            modelBuilder.Configurations.Add(new OrderPackageConfiguration());
            modelBuilder.Configurations.Add(new OrderServiceConfiguration());
            modelBuilder.Configurations.Add(new OrderProcessItemConfiguration());
            modelBuilder.Configurations.Add(new PackageTranportConfiguration());
            modelBuilder.Configurations.Add(new PageConfiguration());
            modelBuilder.Configurations.Add(new PermissionActionConfiguration());
            modelBuilder.Configurations.Add(new PositionConfiguration());
            modelBuilder.Configurations.Add(new RoleActionConfiguration());
            modelBuilder.Configurations.Add(new ShopConfiguration());
            modelBuilder.Configurations.Add(new SystemConfiguration());
            modelBuilder.Configurations.Add(new TitleConfiguration());
            modelBuilder.Configurations.Add(new TrackerConfiguration());
            modelBuilder.Configurations.Add(new TransactionConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new UserPositionConfiguration());
            modelBuilder.Configurations.Add(new WarehouseConfiguration());
            modelBuilder.Configurations.Add(new LogActionConfiguration());
            modelBuilder.Configurations.Add(new LogLoginConfiguration());
            modelBuilder.Configurations.Add(new LogSystemConfiguration());
            modelBuilder.Configurations.Add(new FinanceAccountConfiguration());
            modelBuilder.Configurations.Add(new FinanceFundConfiguration());
            modelBuilder.Configurations.Add(new CustomerLogLoginConfiguration());
            modelBuilder.Configurations.Add(new CustomerLogConfiguration());
            modelBuilder.Configurations.Add(new ProvinceConfiguration());
            modelBuilder.Configurations.Add(new DistrictConfiguration());
            modelBuilder.Configurations.Add(new WardConfiguration());
            modelBuilder.Configurations.Add(new DepositConfiguration());
            modelBuilder.Configurations.Add(new DepositDetailConfiguration());
            modelBuilder.Configurations.Add(new HistoryPackageConfiguration());
            modelBuilder.Configurations.Add(new OrderCommentConfiguration());
            modelBuilder.Configurations.Add(new CustomerLevelConfiguration());
            modelBuilder.Configurations.Add(new CustomerConfigLevelConfiguration());
            modelBuilder.Configurations.Add(new OrderHistoryConfiguration());
            modelBuilder.Configurations.Add(new CustomerTypeConfiguration());
            modelBuilder.Configurations.Add(new CustomerSaleConfiguration());
            modelBuilder.Configurations.Add(new ImportWarehouseConfiguration());
            modelBuilder.Configurations.Add(new ExportWarehouseConfiguration());
            modelBuilder.Configurations.Add(new ExportWarehouseDetailConfiguration());
            modelBuilder.Configurations.Add(new PackingListConfiguration());
            modelBuilder.Configurations.Add(new WalletConfiguration());
            modelBuilder.Configurations.Add(new TreasureConfiguration());
            modelBuilder.Configurations.Add(new DeliverySpendConfiguration());
            modelBuilder.Configurations.Add(new DeliveryConfiguration());
            modelBuilder.Configurations.Add(new DeliveryDetailConfiguration());
            modelBuilder.Configurations.Add(new ComplainConfiguration());
            modelBuilder.Configurations.Add(new ComplainUserConfiguration());
            modelBuilder.Configurations.Add(new SourceConfiguration());
            modelBuilder.Configurations.Add(new SourceDetailConfiguration());
            modelBuilder.Configurations.Add(new SourceSupplierConfiguration());
            modelBuilder.Configurations.Add(new FundBillConfiguration());
            modelBuilder.Configurations.Add(new AccountantSubjectConfiguration());
            modelBuilder.Configurations.Add(new RechargeBillConfiguration());
            modelBuilder.Configurations.Add(new SourceServiceConfiguration());
            modelBuilder.Configurations.Add(new SourceServiceCustomerConfiguration());
            modelBuilder.Configurations.Add(new DrawConfiguration());
            modelBuilder.Configurations.Add(new PutAwayConfiguration());
            modelBuilder.Configurations.Add(new PutAwayDetailConfiguration());
            modelBuilder.Configurations.Add(new OrderContractCodeConfiguration());
            modelBuilder.Configurations.Add(new PotentialCustomerConfiguration());
            modelBuilder.Configurations.Add(new ClaimForRefundConfiguration());
            modelBuilder.Configurations.Add(new ClaimForRefundDetailConfiguration());
            modelBuilder.Configurations.Add(new NotificationConfiguration());
            modelBuilder.Configurations.Add(new NotifiCommonConfiguration());
            modelBuilder.Configurations.Add(new NotifiCustomerConfiguration());
            modelBuilder.Configurations.Add(new RecentConfiguration());
            modelBuilder.Configurations.Add(new OrderDetailCountingConfiguration());
            modelBuilder.Configurations.Add(new PartnerConfiguration());
            modelBuilder.Configurations.Add(new NotifyRealTimeConfiguration());
            modelBuilder.Configurations.Add(new MessageRealTimeConfiguration());
            modelBuilder.Configurations.Add(new MessageUserConfiguration());
            modelBuilder.Configurations.Add(new AttachmentMessageConfiguration());
            modelBuilder.Configurations.Add(new AttachmentConfiguration());
            modelBuilder.Configurations.Add(new SendEmailConfiguration());
            modelBuilder.Configurations.Add(new SendEmailResultConfiguration());
            modelBuilder.Configurations.Add(new UserConnectionConfiguration());
            modelBuilder.Configurations.Add(new GroupChatConfiguration());
            modelBuilder.Configurations.Add(new GroupChatContentConfiguration());
            modelBuilder.Configurations.Add(new GroupChatLikeConfiguration());
            modelBuilder.Configurations.Add(new GroupChatReadConfiguration());
            modelBuilder.Configurations.Add(new GroupChatUserConfiguration());
            modelBuilder.Configurations.Add(new ChatFilesAttachConfiguration());
            modelBuilder.Configurations.Add(new EntrepotConfiguration());
            modelBuilder.Configurations.Add(new TransportMethodConfiguration());
            modelBuilder.Configurations.Add(new RequestShipConfiguration());
            modelBuilder.Configurations.Add(new CustomerWalletConfiguration());
            modelBuilder.Configurations.Add(new PayReceivableConfiguration());
            modelBuilder.Configurations.Add(new OrderLogConfiguration());
            modelBuilder.Configurations.Add(new DebitConfiguration());
            modelBuilder.Configurations.Add(new DebitHistoryConfiguration());
            modelBuilder.Configurations.Add(new PackageNoCodeConfiguration());
            modelBuilder.Configurations.Add(new OrderReasonConfiguration());
            modelBuilder.Configurations.Add(new OrderRefundConfiguration());
            modelBuilder.Configurations.Add(new OrderRefundDetailConfiguration());
            modelBuilder.Configurations.Add(new CustomerCallHistoryConfiguration());
            modelBuilder.Configurations.Add(new OrderServiceOtherConfiguration());
            modelBuilder.Configurations.Add(new ComplainTypeConfiguration());
            modelBuilder.Configurations.Add(new PackageHistoryConfiguration());
            modelBuilder.Configurations.Add(new ComplainHistoryConfiguration());
            modelBuilder.Configurations.Add(new TransferConfiguration());
            modelBuilder.Configurations.Add(new TransferDetailConfiguration());
            modelBuilder.Configurations.Add(new PackageNoteConfiguration());
            modelBuilder.Configurations.Add(new SettingConfiguration());
            modelBuilder.Configurations.Add(new DebitReportConfiguration());
            modelBuilder.Configurations.Add(new ComplainOrderConfiguration());
            OnModelCreatingPartial(modelBuilder);
        }

        public static DbModelBuilder CreateModel(DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Configurations.Add(new DebitReportConfiguration(schema));
            modelBuilder.Configurations.Add(new SettingConfiguration(schema));
            modelBuilder.Configurations.Add(new PackageNoteConfiguration(schema));
            modelBuilder.Configurations.Add(new PackageHistoryConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderServiceOtherConfiguration(schema));
            modelBuilder.Configurations.Add(new CustomerCallHistoryConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderRefundConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderRefundDetailConfiguration(schema));
            modelBuilder.Configurations.Add(new PackageNoCodeConfiguration(schema));
            modelBuilder.Configurations.Add(new TransportMethodConfiguration(schema));
            modelBuilder.Configurations.Add(new EntrepotConfiguration(schema));
            modelBuilder.Configurations.Add(new GroupChatConfiguration(schema));
            modelBuilder.Configurations.Add(new GroupChatContentConfiguration(schema));
            modelBuilder.Configurations.Add(new GroupChatLikeConfiguration(schema));
            modelBuilder.Configurations.Add(new GroupChatReadConfiguration(schema));
            modelBuilder.Configurations.Add(new GroupChatUserConfiguration(schema));
            modelBuilder.Configurations.Add(new ChatFilesAttachConfiguration(schema));
            modelBuilder.Configurations.Add(new UserConnectionConfiguration(schema));
            modelBuilder.Configurations.Add(new SendEmailConfiguration(schema));
            modelBuilder.Configurations.Add(new SendEmailResultConfiguration(schema));
            modelBuilder.Configurations.Add(new AttachmentConfiguration(schema));
            modelBuilder.Configurations.Add(new NotifyRealTimeConfiguration(schema));
            modelBuilder.Configurations.Add(new MessageRealTimeConfiguration(schema));
            modelBuilder.Configurations.Add(new MessageUserConfiguration(schema));
            modelBuilder.Configurations.Add(new AttachmentMessageConfiguration(schema));
            modelBuilder.Configurations.Add(new PartnerConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderDetailCountingConfiguration(schema));
            modelBuilder.Configurations.Add(new RecentConfiguration(schema));
            modelBuilder.Configurations.Add(new PutAwayConfiguration(schema));
            modelBuilder.Configurations.Add(new PutAwayDetailConfiguration(schema));
            modelBuilder.Configurations.Add(new AppConfiguration(schema));
            modelBuilder.Configurations.Add(new DispatcherDetailConfiguration(schema));
            modelBuilder.Configurations.Add(new DeliveryConfiguration(schema));
            modelBuilder.Configurations.Add(new DeliverySpendConfiguration(schema));
            modelBuilder.Configurations.Add(new DeliveryDetailConfiguration(schema));
            modelBuilder.Configurations.Add(new DispatcherConfiguration(schema));
            modelBuilder.Configurations.Add(new WalletDetailConfiguration(schema));
            modelBuilder.Configurations.Add(new ImportWarehouseDetailConfiguration(schema));
            modelBuilder.Configurations.Add(new BagConfiguration(schema));
            modelBuilder.Configurations.Add(new BagPackageConfiguration(schema));
            modelBuilder.Configurations.Add(new CategoryConfiguration(schema));
            modelBuilder.Configurations.Add(new ChangePasswordLogConfiguration(schema));
            modelBuilder.Configurations.Add(new ConfigLoginFailureConfiguration(schema));
            modelBuilder.Configurations.Add(new CustomerConfiguration(schema));
            modelBuilder.Configurations.Add(new GiftConfiguration(schema));
            modelBuilder.Configurations.Add(new GroupPermisionConfiguration(schema));
            modelBuilder.Configurations.Add(new HashTagConfiguration(schema));
            modelBuilder.Configurations.Add(new HistorySatuConfiguration(schema));
            modelBuilder.Configurations.Add(new LayoutConfiguration(schema));
            modelBuilder.Configurations.Add(new LockHistoryConfiguration(schema));
            modelBuilder.Configurations.Add(new ModuleConfiguration(schema));
            modelBuilder.Configurations.Add(new OfficeConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderAddressConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderDetailConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderExchangeConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderInfoConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderPackageConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderServiceConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderProcessItemConfiguration(schema));
            modelBuilder.Configurations.Add(new PackageTranportConfiguration(schema));
            modelBuilder.Configurations.Add(new PageConfiguration(schema));
            modelBuilder.Configurations.Add(new PermissionActionConfiguration(schema));
            modelBuilder.Configurations.Add(new PositionConfiguration(schema));
            modelBuilder.Configurations.Add(new RoleActionConfiguration(schema));
            modelBuilder.Configurations.Add(new ShopConfiguration(schema));
            modelBuilder.Configurations.Add(new SystemConfiguration(schema));
            modelBuilder.Configurations.Add(new TitleConfiguration(schema));
            modelBuilder.Configurations.Add(new TrackerConfiguration(schema));
            modelBuilder.Configurations.Add(new TransactionConfiguration(schema));
            modelBuilder.Configurations.Add(new UserConfiguration(schema));
            modelBuilder.Configurations.Add(new UserPositionConfiguration(schema));
            modelBuilder.Configurations.Add(new WarehouseConfiguration(schema));
            modelBuilder.Configurations.Add(new LogActionConfiguration(schema));
            modelBuilder.Configurations.Add(new LogLoginConfiguration(schema));
            modelBuilder.Configurations.Add(new LogSystemConfiguration(schema));
            modelBuilder.Configurations.Add(new FinanceAccountConfiguration(schema));
            modelBuilder.Configurations.Add(new FinanceFundConfiguration(schema));
            modelBuilder.Configurations.Add(new ProvinceConfiguration(schema));
            modelBuilder.Configurations.Add(new DistrictConfiguration(schema));
            modelBuilder.Configurations.Add(new WardConfiguration(schema));
            modelBuilder.Configurations.Add(new CustomerLogLoginConfiguration(schema));
            modelBuilder.Configurations.Add(new CustomerLogConfiguration(schema));
            modelBuilder.Configurations.Add(new DepositConfiguration(schema));
            modelBuilder.Configurations.Add(new DepositDetailConfiguration(schema));
            modelBuilder.Configurations.Add(new HistoryPackageConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderCommentConfiguration(schema));
            modelBuilder.Configurations.Add(new CustomerLevelConfiguration(schema));
            modelBuilder.Configurations.Add(new CustomerConfigLevelConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderHistoryConfiguration(schema));
            modelBuilder.Configurations.Add(new CustomerTypeConfiguration(schema));
            modelBuilder.Configurations.Add(new CustomerSaleConfiguration(schema));
            modelBuilder.Configurations.Add(new TreasureConfiguration(schema));
            modelBuilder.Configurations.Add(new ImportWarehouseConfiguration(schema));
            modelBuilder.Configurations.Add(new ExportWarehouseConfiguration(schema));
            modelBuilder.Configurations.Add(new ExportWarehouseDetailConfiguration(schema));
            modelBuilder.Configurations.Add(new PackingListConfiguration(schema));
            modelBuilder.Configurations.Add(new WalletConfiguration(schema));
            modelBuilder.Configurations.Add(new CustomerTypeConfiguration(schema));
            modelBuilder.Configurations.Add(new ComplainConfiguration(schema));
            modelBuilder.Configurations.Add(new ComplainUserConfiguration(schema));
            modelBuilder.Configurations.Add(new SourceConfiguration(schema));
            modelBuilder.Configurations.Add(new SourceDetailConfiguration(schema));
            modelBuilder.Configurations.Add(new SourceSupplierConfiguration(schema));
            modelBuilder.Configurations.Add(new FundBillConfiguration(schema));
            modelBuilder.Configurations.Add(new AccountantSubjectConfiguration(schema));
            modelBuilder.Configurations.Add(new RechargeBillConfiguration(schema));
            modelBuilder.Configurations.Add(new SourceServiceConfiguration(schema));
            modelBuilder.Configurations.Add(new SourceServiceCustomerConfiguration(schema));
            modelBuilder.Configurations.Add(new DrawConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderContractCodeConfiguration(schema));
            modelBuilder.Configurations.Add(new PotentialCustomerConfiguration(schema));
            modelBuilder.Configurations.Add(new ClaimForRefundConfiguration(schema));
            modelBuilder.Configurations.Add(new ClaimForRefundDetailConfiguration(schema));
            modelBuilder.Configurations.Add(new NotificationConfiguration(schema));
            modelBuilder.Configurations.Add(new NotifiCommonConfiguration(schema));
            modelBuilder.Configurations.Add(new NotifiCustomerConfiguration(schema));
            modelBuilder.Configurations.Add(new RequestShipConfiguration(schema));
            modelBuilder.Configurations.Add(new CustomerWalletConfiguration(schema));
            modelBuilder.Configurations.Add(new PayReceivableConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderLogConfiguration(schema));
            modelBuilder.Configurations.Add(new DebitConfiguration(schema));
            modelBuilder.Configurations.Add(new DebitHistoryConfiguration(schema));
            modelBuilder.Configurations.Add(new OrderReasonConfiguration(schema));
            modelBuilder.Configurations.Add(new ComplainTypeConfiguration(schema));
            modelBuilder.Configurations.Add(new ComplainHistoryConfiguration(schema));
            modelBuilder.Configurations.Add(new TransferConfiguration(schema));
            modelBuilder.Configurations.Add(new TransferDetailConfiguration(schema));
            modelBuilder.Configurations.Add(new ComplainOrderConfiguration(schema));
            return modelBuilder;
        }

        partial void InitializePartial();

        partial void OnModelCreatingPartial(DbModelBuilder modelBuilder);

        public bool IsSqlParameterNull(System.Data.SqlClient.SqlParameter param)
        {
            var sqlValue = param.SqlValue;
            var nullableValue = sqlValue as System.Data.SqlTypes.INullable;
            if (nullableValue != null)
                return nullableValue.IsNull;
            return (sqlValue == null || sqlValue == System.DBNull.Value);
        }

        // Stored Procedures
        public List<ComplainSelectUserResult> ComplainSelectUser()
        {
            int procResult;
            return ComplainSelectUser(out procResult);
        }

        public List<ComplainSelectUserResult> ComplainSelectUser(out int procResult)
        {
            var procResultParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@procResult", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output };
            var procResultData = Database.SqlQuery<ComplainSelectUserResult>("EXEC @procResult = [dbo].[sp_Complain_Select_User] ", procResultParam).ToList();

            procResult = (int)procResultParam.Value;
            return procResultData;
        }

        //public async Task<List<ComplainSelectUserResult>> ComplainSelectUserAsync()
        //{
        //    var procResultData = await Database.SqlQuery<ComplainSelectUserResult>("EXEC [dbo].[sp_Complain_Select_User] ").ToListAsync();

        //    return procResultData;
        //}

        public List<ComplainSelectUserResult> ComplainSelectUserOut(out int count)
        {
            var countParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@Count", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output, Precision = 10, Scale = 0 };
            var procResultParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@procResult", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output };
            var procResultData = Database.SqlQuery<ComplainSelectUserResult>("EXEC @procResult = [dbo].[sp_Complain_Select_User] @Count OUTPUT", countParam, procResultParam).ToList();
            if (IsSqlParameterNull(countParam))
                count = 0;
            else
                count = (int)countParam.Value;
            return procResultData;
        }
    }
}