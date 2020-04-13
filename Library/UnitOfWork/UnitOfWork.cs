using System;
using Library.DbContext;
using Library.DbContext.Repositories;

namespace Library.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {
        private bool _disposed;
        // Field Dbcontext
        /// <summary>
        /// Khai báo DbContext
        /// </summary>
        private ProjectXContext _dbContext;

        // Field Repository
        // Khái Báo Repository
        private UserRepository _userRepository;
        private OfficeRepository _officeRepository;
        private TitleRepository _titleRepository;
        private PositionRepository _positionRepository;
        private UserPositionRepository _userPositionRepository;
        private AppRepository _appRepository;
        private ModuleRepository _moduleRepository;
        private PageRepository _pageRepository;
        private PermissionActionRepository _permissionActionRepository;
        private RoleActionRepository _roleActionRepository;
        private OrderRepository _orderRepository;
        private OrderDetailRepository _orderDetailRepository;
        private OrderServiceRepository _orderServiceRepository;
        private ConfigLoginFailureRepository _configLoginFailureRepository;
        private LogActionRepository _logActionRepository;
        private LogLoginRepository _logLoginRepository;
        private LogSystemRepository _logSystemRepository;
        private CategoryRepository _categoryRepository;

        private FinanceFundRepository _financeFundRepository;
        private FinanceAccountRepository _financeAccountRepository;
        private CustomerLevelRepository _customerLevelRepository;
        private ShopRepository _shopRepository;
        private ProductRepository _productRepository;
        private CustomerRepository _customerRepository;
        private CustomerLogLoginRepository _customerLogLoginRepository;
        private CustomerLogRepository _customerLogRepository;
        private WarehouseRepository _warehouseRepository;
        private ProvinceRepository _provinceRepository;
        private DistrictRepository _districtRepository;
        private WardRepository _wardRepository;
        private OrderExhibitionRepository _orderExhibitionRepository;
        private DepositRepository _depositRepository;
        private GroupPermissionRepository _groupPermissionRepository;
        private TrackerRepository _trackerRepository;
        private OrderPackageRepository _orderPackageRepository;
        private HistoryPackageRepository _historyPackageRepository;
        private DepositDetailRepository _depositDetailRepository;
        private OrderCommentRepository _orderCommentRepository;
        private CustomerConfigLevelRepository _customerConfigLevelRepository;
        private OrderHistoryRepository _orderHistoryRepository;
        private CustomerTypeRepository _customerTypeRepository;
        private TreasureRepository _treasureRepository;
        private OrderContractCodeRepository _orderContractCodeRepository;

        private ComplainRepository _complainRepository;
        private ComplainUserRepository _complainUserRepository;
        private ImportWarehouseRepository _importWarehouseRepository;
        private ExportWarehouseRepository _exportWarehouseRepository;
        private ExportWarehouseDetailRepository _exportWarehouseDetailRepository;
        private WalletRepository _walletRepository;
        private PackingListRepository _packingListRepository;
        private DeliveryRepository _deliveryRepository;
        private OrderAddressRepository _orderAddressRepository;
        private OrderExchangeRepository _orderExchangeRepository;
        private OrderInfoRepository _orderInfoRepository;

        private SourceRepository _sourceRepository;
        private SourceDetailRepository _sourceDetailRepository;
        private SourceSupplierRepository _sourceSupplierRepository;

        private FundBillRepository _fundBillRepository;
        private AccountantSubjectRepository _accountantSubjectRepository;
        private RechargeBillRepository _rechargeBillRepository;
        private SourceServiceRepository _sourceServiceRepository;
        private SourceServiceCustomerRepository _sourceServiceCustomerRepository;
        private SystemRepository _systemRepository;
        private ImportWarehouseDetailRepository _importWarehouseDetailRepository;
        private DrawRepository _drawRepository;
        private DispatcherRepository _dispatcherRepository;
        private DispatcherDetailRepository _dispatcherDetailRepository;
        private WalletDetailRepository _walletDetailRepository;
        private DeliveryDetailRepository _deliveryDetailRepository;
        private DeliverySpendRepository _deliverySpendRepository;
        private PutAwayRepository _putAwayRepository;
        private PutAwayDetailRepository _putAwayDetailRepository;
        private LayoutRepository _layoutRepository;
        private PotentialCustomerRepository _potentialCustomerRepository;
        private ClaimForRefundRepository _claimForRefundRepository;
        private ClaimForRefundDetailRepository _claimForRefundDetailRepository;


        private NotificationRepository _notificationRepository;
        private NotifiCommonRepository _notifiCommonRepository;
        private NotifiCustomerRepository _notifiCustomerRepository;

        private ReportBusinessRepository _reportBusinessRepository;
        private ComplainRecoupRepository _complainRecoupRepository;
        private RecentRepository _recentRepository;
        private OrderDetailCountingRepository _orderDetailCountingRepository;
        private PartnerRepository _partnerRepository;
        private NotifyRealTimeRepository _notifyRealTimeRepo;
        private MessageRealTimeRepository _messageRealTimeRepo;
        private MessageUserRepository _messageUserRepo;
        private AttachmentMessageRepository _attachmentMessageRepo;
        private AttachmentRepository _attachmentRepo;
        private SendEmailRepository _sendEmailRepo;
        private SendEmailResultRepository _sendEmailResultRepo;
        private UserConnectionRepository _userConnectionRepo;
        private GroupChatRepository _groupChatRepository;
        private GroupChatContentRepository _groupChatContentRepository;
        private GroupChatLikeRepository _groupChatLikeRepository;
        private GroupChatReadRepository _groupChatReadRepository;
        private GroupChatUserRepository _groupChatUserRepository;
        private ChatFilesAttachRepository _chatFilesAttachRepository;
        private EntrepotRepository _entrepotRepository;
        private TransportMethodRepository _transportMethodRepository;

        private RequestShipRepository _requestShipRepository;

        private CustomerWalletRepository _customerWalletRepository;
        private OrderLogRepository _orderLogRepository;
        private PayReceivableRepository _payReceivableRepository;

        private DebitRepository _debitRepository;
        private DebitHistoryRepository _debitHistoryRepository;
        private PackageNoCodeRepository _packageNoCodeRepository;
        private OrderReasonRepository _orderReasonRepository;
        private OrderRefundRepository _orderRefundRepository;
        private OrderRefundDetailRepository _orderRefundDetailRepository;
        private CustomerCallHistoryRepository _customerCallHistoryRepository;
        private OrderServiceOtherRepository _orderServiceOtherRepository;

        private ComplainTypeRepository _complainTypeRepository;
        private PackageHistoryRepository _packageHistoryRepository;

        private ComplainHistoryRepository _complainHistoryRepository;
        private TransferDetailRepository _transferDetailRepository;
        private TransferRepository _transferRepository;
        private PackageNoteRepository _packageNoteRepository;
        private SettingRepository _settingRepository;
        private DebitReportRepository _debitReportRepository;
        private ComplainOrderRepository _complainOrderRepository;
        #region ========================= DbContext Propeties =========================================================
        public ProjectXContext DbContext => _dbContext ?? (_dbContext = new ProjectXContext());
        #endregion

        #region ========================= Repository Properties =======================================================
        public DebitReportRepository DebitReportRepo => _debitReportRepository ?? (_debitReportRepository = new DebitReportRepository(DbContext));
        public UserRepository UserRepo => _userRepository ?? (_userRepository = new UserRepository(DbContext));
        public OfficeRepository OfficeRepo => _officeRepository ?? (_officeRepository = new OfficeRepository(DbContext));
        public TitleRepository TitleRepo => _titleRepository ?? (_titleRepository = new TitleRepository(DbContext));
        public PositionRepository PositionRepo => _positionRepository ?? (_positionRepository = new PositionRepository(DbContext));
        public UserPositionRepository UserPositionRepo => _userPositionRepository ?? (_userPositionRepository = new UserPositionRepository(DbContext));
        public AppRepository AppRepo => _appRepository ?? (_appRepository = new AppRepository(DbContext));
        public ModuleRepository ModuleRepo => _moduleRepository ?? (_moduleRepository = new ModuleRepository(DbContext));
        public PageRepository PageRepo => _pageRepository ?? (_pageRepository = new PageRepository(DbContext));
        public PermissionActionRepository PermissionActionRepo => _permissionActionRepository ?? (_permissionActionRepository = new PermissionActionRepository(DbContext));
        public RoleActionRepository RoleActionRepo => _roleActionRepository ?? (_roleActionRepository = new RoleActionRepository(DbContext));
        public OrderRepository OrderRepo => _orderRepository ?? (_orderRepository = new OrderRepository(DbContext));
        public OrderDetailRepository OrderDetailRepo => _orderDetailRepository ?? (_orderDetailRepository = new OrderDetailRepository(DbContext));
        public OrderServiceRepository OrderServiceRepo => _orderServiceRepository ?? (_orderServiceRepository = new OrderServiceRepository(DbContext));
        public ConfigLoginFailureRepository ConfigLoginFailureRepo => _configLoginFailureRepository ?? (_configLoginFailureRepository = new ConfigLoginFailureRepository(DbContext));
        public LogActionRepository LogActionRepo => _logActionRepository ?? (_logActionRepository = new LogActionRepository(DbContext));
        public LogLoginRepository LogLoginRepo => _logLoginRepository ?? (_logLoginRepository = new LogLoginRepository(DbContext));
        public LogSystemRepository LogSystemRepo => _logSystemRepository ?? (_logSystemRepository = new LogSystemRepository(DbContext));
        public CategoryRepository CategoryRepo => _categoryRepository ?? (_categoryRepository = new CategoryRepository(DbContext));
        public FinanceFundRepository FinaceFundRepo => _financeFundRepository ?? (_financeFundRepository = new FinanceFundRepository(DbContext));
        public FinanceAccountRepository FinaceAccountRepo => _financeAccountRepository ?? (_financeAccountRepository = new FinanceAccountRepository(DbContext));
        public CustomerLevelRepository CustomerLevelRepo => _customerLevelRepository ?? (_customerLevelRepository = new CustomerLevelRepository(DbContext));
        public ShopRepository ShopRepo => _shopRepository ?? (_shopRepository = new ShopRepository(DbContext));
        public ProductRepository ProductRepo => _productRepository ?? (_productRepository = new ProductRepository(DbContext));
        public CustomerRepository CustomerRepo => _customerRepository ?? (_customerRepository = new CustomerRepository(DbContext));
        public CustomerLogLoginRepository CustomerLogLoginRepo => _customerLogLoginRepository ?? (_customerLogLoginRepository = new CustomerLogLoginRepository(DbContext));
        public CustomerLogRepository CustomerLogRepo => _customerLogRepository ?? (_customerLogRepository = new CustomerLogRepository(DbContext));
        public WarehouseRepository WarehouseRepo => _warehouseRepository ?? (_warehouseRepository = new WarehouseRepository(DbContext));

        public ProvinceRepository ProvinceRepo => _provinceRepository ?? (_provinceRepository = new ProvinceRepository(DbContext));
        public DistrictRepository DistrictRepo => _districtRepository ?? (_districtRepository = new DistrictRepository(DbContext));
        public WardRepository WardRepo => _wardRepository ?? (_wardRepository = new WardRepository(DbContext));
        public OrderExhibitionRepository ExhibitionRepo => _orderExhibitionRepository ?? (_orderExhibitionRepository = new OrderExhibitionRepository(DbContext));
        public DepositRepository DepositRepo => _depositRepository ?? (_depositRepository = new DepositRepository(DbContext));
        public OrderPackageRepository OrderPackageRepo => _orderPackageRepository ?? (_orderPackageRepository = new OrderPackageRepository(DbContext));
        public HistoryPackageRepository HistoryPackageRepo => _historyPackageRepository ?? (_historyPackageRepository = new HistoryPackageRepository(DbContext));
        public GroupPermissionRepository GroupPermissionRepo => _groupPermissionRepository ?? (_groupPermissionRepository = new GroupPermissionRepository(DbContext));
        public TrackerRepository TrackerRepo => _trackerRepository ?? (_trackerRepository = new TrackerRepository(DbContext));
        public DepositDetailRepository DepositDetailRepo => _depositDetailRepository ?? (_depositDetailRepository = new DepositDetailRepository(DbContext));
        public OrderCommentRepository OrderCommentRepo => _orderCommentRepository ?? (_orderCommentRepository = new OrderCommentRepository(DbContext));
        public CustomerConfigLevelRepository CustomerConfigLevelRepo => _customerConfigLevelRepository ?? (_customerConfigLevelRepository = new CustomerConfigLevelRepository(DbContext));
        public OrderHistoryRepository OrderHistoryRepo => _orderHistoryRepository ?? (_orderHistoryRepository = new OrderHistoryRepository(DbContext));
        public CustomerTypeRepository CustomerTypeRepo => _customerTypeRepository ?? (_customerTypeRepository = new CustomerTypeRepository(DbContext));
        public TreasureRepository TreasureRepo => _treasureRepository ?? (_treasureRepository = new TreasureRepository(DbContext));
        public ComplainRepository ComplainRepo => _complainRepository ?? (_complainRepository = new ComplainRepository(DbContext));
        public ComplainUserRepository ComplainUserRepo => _complainUserRepository ?? (_complainUserRepository = new ComplainUserRepository(DbContext));
        public ImportWarehouseRepository ImportWarehouseRepo => _importWarehouseRepository ?? (_importWarehouseRepository = new ImportWarehouseRepository(DbContext));
        public ExportWarehouseRepository ExportWarehouseRepo => _exportWarehouseRepository ?? (_exportWarehouseRepository = new ExportWarehouseRepository(DbContext));
        public ExportWarehouseDetailRepository ExportWarehouseDetailRepo => _exportWarehouseDetailRepository ?? (_exportWarehouseDetailRepository = new ExportWarehouseDetailRepository(DbContext));
        public WalletRepository WalletRepo => _walletRepository ?? (_walletRepository = new WalletRepository(DbContext));
        public PackingListRepository PackingListRepo => _packingListRepository ?? (_packingListRepository = new PackingListRepository(DbContext));
        public DeliveryRepository DeliveryRepo => _deliveryRepository ?? (_deliveryRepository = new DeliveryRepository(DbContext));
        public OrderAddressRepository OrderAddressRepo => _orderAddressRepository ?? (_orderAddressRepository = new OrderAddressRepository(DbContext));
        public OrderExchangeRepository OrderExchangeRepo => _orderExchangeRepository ?? (_orderExchangeRepository = new OrderExchangeRepository(DbContext));
        public SourceRepository SourceRepo => _sourceRepository ?? (_sourceRepository = new SourceRepository(DbContext));
        public SourceDetailRepository SourceDetailRepo => _sourceDetailRepository ?? (_sourceDetailRepository = new SourceDetailRepository(DbContext));
        public SourceSupplierRepository SourceSupplierRepo => _sourceSupplierRepository ?? (_sourceSupplierRepository = new SourceSupplierRepository(DbContext));
        public FundBillRepository FundBillRepo => _fundBillRepository ?? (_fundBillRepository = new FundBillRepository(DbContext));
        public AccountantSubjectRepository AccountantSubjectRepo => _accountantSubjectRepository ?? (_accountantSubjectRepository = new AccountantSubjectRepository(DbContext));
        public RechargeBillRepository RechargeBillRepo => _rechargeBillRepository ?? (_rechargeBillRepository = new RechargeBillRepository(DbContext));
        public SourceServiceRepository SourceServiceRepo => _sourceServiceRepository ?? (_sourceServiceRepository = new SourceServiceRepository(DbContext));
        public SourceServiceCustomerRepository SourceServiceCustomerRepo => _sourceServiceCustomerRepository ?? (_sourceServiceCustomerRepository = new SourceServiceCustomerRepository(DbContext));
        public SystemRepository SystemRepo=> _systemRepository ?? (_systemRepository = new SystemRepository(DbContext));
        public ImportWarehouseDetailRepository ImportWarehouseDetailRepo=> _importWarehouseDetailRepository ?? (_importWarehouseDetailRepository = new ImportWarehouseDetailRepository(DbContext));
        public DrawRepository DrawRepo => _drawRepository ?? (_drawRepository = new DrawRepository(DbContext));
        public WalletDetailRepository WalletDetailRepo => _walletDetailRepository ?? (_walletDetailRepository = new WalletDetailRepository(DbContext));
        public DispatcherRepository DispatcherRepo => _dispatcherRepository ?? (_dispatcherRepository = new DispatcherRepository(DbContext));
        public DispatcherDetailRepository DispatcherDetailRepo => _dispatcherDetailRepository ?? (_dispatcherDetailRepository = new DispatcherDetailRepository(DbContext));
        public DeliveryDetailRepository DeliveryDetailRepo => _deliveryDetailRepository ?? (_deliveryDetailRepository = new DeliveryDetailRepository(DbContext));
        public DeliverySpendRepository DeliverySpendRepo => _deliverySpendRepository ?? (_deliverySpendRepository = new DeliverySpendRepository(DbContext));
        public PutAwayRepository PutAwayRepo => _putAwayRepository ?? (_putAwayRepository = new PutAwayRepository(DbContext));
        public PutAwayDetailRepository PutAwayDetailRepo => _putAwayDetailRepository ?? (_putAwayDetailRepository = new PutAwayDetailRepository(DbContext));
        public LayoutRepository LayoutRepo => _layoutRepository ?? (_layoutRepository = new LayoutRepository(DbContext));
        public OrderContractCodeRepository OrderContractCodeRepo => _orderContractCodeRepository ?? (_orderContractCodeRepository = new OrderContractCodeRepository(DbContext));
        public PotentialCustomerRepository PotentialCustomerRepo => _potentialCustomerRepository ?? (_potentialCustomerRepository = new PotentialCustomerRepository(DbContext));
         
        public NotificationRepository NotificationRepo => _notificationRepository ?? (_notificationRepository = new NotificationRepository(DbContext));
        public NotifiCommonRepository NotifiCommonRepo => _notifiCommonRepository ?? (_notifiCommonRepository = new NotifiCommonRepository(DbContext));
        public NotifiCustomerRepository NotifiCustomerRepo => _notifiCustomerRepository ?? (_notifiCustomerRepository = new NotifiCustomerRepository(DbContext));

        public ReportBusinessRepository ReportBusinessRepo => _reportBusinessRepository ?? (_reportBusinessRepository = new ReportBusinessRepository(DbContext));
        public ComplainRecoupRepository ComplainRecoupRepo => _complainRecoupRepository ?? (_complainRecoupRepository = new ComplainRecoupRepository(DbContext));

        public ClaimForRefundRepository ClaimForRefundRepo => _claimForRefundRepository ?? (_claimForRefundRepository = new ClaimForRefundRepository(DbContext));
        public ClaimForRefundDetailRepository ClaimForRefundDetailRepo => _claimForRefundDetailRepository ?? (_claimForRefundDetailRepository = new ClaimForRefundDetailRepository(DbContext));
        public RecentRepository RecentRepo => _recentRepository ?? (_recentRepository = new RecentRepository(DbContext));
        public OrderInfoRepository OrderInfoRepo => _orderInfoRepository ?? (_orderInfoRepository = new OrderInfoRepository(DbContext));
        public OrderDetailCountingRepository OrderDetailCountingRepo => _orderDetailCountingRepository ?? (_orderDetailCountingRepository = new OrderDetailCountingRepository(DbContext));
        public PartnerRepository PartnerRepo => _partnerRepository ?? (_partnerRepository = new PartnerRepository(DbContext));
        public NotifyRealTimeRepository NotifyRealTimeRepo => _notifyRealTimeRepo ?? (_notifyRealTimeRepo = new NotifyRealTimeRepository(DbContext));
        public MessageRealTimeRepository MessageRealTimeRepo => _messageRealTimeRepo ?? (_messageRealTimeRepo = new MessageRealTimeRepository(DbContext));
        public MessageUserRepository MessageUserRepo => _messageUserRepo ?? (_messageUserRepo = new MessageUserRepository(DbContext));
        public AttachmentMessageRepository AttachmentMessageRepo => _attachmentMessageRepo ?? (_attachmentMessageRepo = new AttachmentMessageRepository(DbContext));
        public AttachmentRepository AttachmentRepo => _attachmentRepo ?? (_attachmentRepo = new AttachmentRepository(DbContext));
        public SendEmailRepository SendEmailRepo => _sendEmailRepo ?? (_sendEmailRepo = new SendEmailRepository(DbContext));
        public SendEmailResultRepository SendEmailResultRepo => _sendEmailResultRepo ?? (_sendEmailResultRepo = new SendEmailResultRepository(DbContext));
        public UserConnectionRepository UserConnectionRepo => _userConnectionRepo ?? (_userConnectionRepo = new UserConnectionRepository(DbContext));
        public GroupChatRepository GroupChatRepo => _groupChatRepository ?? (_groupChatRepository = new GroupChatRepository(DbContext));
        public GroupChatContentRepository GroupChatContentRepo => _groupChatContentRepository ?? (_groupChatContentRepository = new GroupChatContentRepository(DbContext));
        public GroupChatLikeRepository GroupChatLikeRepo => _groupChatLikeRepository ?? (_groupChatLikeRepository = new GroupChatLikeRepository(DbContext));
        public GroupChatReadRepository GroupChatReadRepo => _groupChatReadRepository ?? (_groupChatReadRepository = new GroupChatReadRepository(DbContext));
        public GroupChatUserRepository GroupChatUserRepo => _groupChatUserRepository ?? (_groupChatUserRepository = new GroupChatUserRepository(DbContext));
        public ChatFilesAttachRepository ChatFilesAttachRepo => _chatFilesAttachRepository ?? (_chatFilesAttachRepository = new ChatFilesAttachRepository(DbContext));
        public EntrepotRepository EntrepotRepo => _entrepotRepository ?? (_entrepotRepository = new EntrepotRepository(DbContext));
        public TransportMethodRepository TransportMethodRepo => _transportMethodRepository ?? (_transportMethodRepository = new TransportMethodRepository(DbContext));
        public RequestShipRepository RequestShipRepo => _requestShipRepository ?? (_requestShipRepository = new RequestShipRepository(DbContext));
        public CustomerWalletRepository CustomerWalletRepo => _customerWalletRepository ?? (_customerWalletRepository = new CustomerWalletRepository(DbContext));
        public OrderLogRepository OrderLogRepo => _orderLogRepository ?? (_orderLogRepository = new OrderLogRepository(DbContext));
        public PayReceivableRepository PayReceivableRepo => _payReceivableRepository ?? (_payReceivableRepository = new PayReceivableRepository(DbContext));

        public DebitRepository DebitRepo => _debitRepository ?? (_debitRepository = new DebitRepository(DbContext));
        public DebitHistoryRepository DebitHistoryRepo => _debitHistoryRepository ?? (_debitHistoryRepository = new DebitHistoryRepository(DbContext));
        public PackageNoCodeRepository PackageNoCodeRepo => _packageNoCodeRepository ?? (_packageNoCodeRepository = new PackageNoCodeRepository(DbContext));
        public OrderReasonRepository OrderReasonRepo => _orderReasonRepository ?? (_orderReasonRepository = new OrderReasonRepository(DbContext));
        public OrderRefundRepository OrderRefundRepo => _orderRefundRepository ?? (_orderRefundRepository = new OrderRefundRepository(DbContext));
        public OrderRefundDetailRepository OrderRefundDetailRepo => _orderRefundDetailRepository ?? (_orderRefundDetailRepository = new OrderRefundDetailRepository(DbContext));
        public CustomerCallHistoryRepository CustomerCallHistoryRepo => _customerCallHistoryRepository ?? (_customerCallHistoryRepository = new CustomerCallHistoryRepository(DbContext));
        public OrderServiceOtherRepository OrderServiceOtherRepo => _orderServiceOtherRepository ?? (_orderServiceOtherRepository = new OrderServiceOtherRepository(DbContext));
        public ComplainTypeRepository ComplainTypeRepo => _complainTypeRepository ?? (_complainTypeRepository = new ComplainTypeRepository(DbContext));
        public PackageHistoryRepository PackageHistoryRepo => _packageHistoryRepository ?? (_packageHistoryRepository = new PackageHistoryRepository(DbContext));
        public ComplainHistoryRepository ComplainHistoryRepo => _complainHistoryRepository ?? (_complainHistoryRepository = new ComplainHistoryRepository(DbContext));
        public TransferRepository TransferRepo => _transferRepository ?? (_transferRepository = new TransferRepository(DbContext));
        public TransferDetailRepository TransferDetailRepo => _transferDetailRepository ?? (_transferDetailRepository = new TransferDetailRepository(DbContext));
        public PackageNoteRepository PackageNoteRepo => _packageNoteRepository ?? (_packageNoteRepository = new PackageNoteRepository(DbContext));
        public SettingRepository SettingRepo => _settingRepository ?? (_settingRepository = new SettingRepository(DbContext));
        public ComplainOrderRepository ComplainOrderRepo => _complainOrderRepository ?? (_complainOrderRepository = new ComplainOrderRepository(DbContext));
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose DbContext
                    _dbContext?.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
