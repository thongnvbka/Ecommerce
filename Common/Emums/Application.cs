using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Common.Attributes;
using ResourcesLikeOrderThaiLan;

namespace Common.Emums
{
    /// <summary>
    /// Enum định nghĩa các đối tượng làm việc trên hệ thống
    /// </summary>
    public enum EnumAccountantSubject
    {
        [LocalizedDescription("DoiTuongKH", typeof(Resource))]
        Customer = 100,
    }

    /// <summary>
    /// Enum action trong phân quyền
    /// </summary>
    public enum EnumAction
    {
        [LocalizedDescription("Phanquyen_View", typeof(Resource))]
        View = 1,

        [LocalizedDescription("Phanquyen_ThemMoi", typeof(Resource))]
        Add = 2,

        [LocalizedDescription("Phanquyen_CapNhat", typeof(Resource))]
        Update = 3,

        [LocalizedDescription("Phanquyen_Xoa", typeof(Resource))]
        Delete = 4,

        [LocalizedDescription("Phanquyen_Duyet", typeof(Resource))]
        Approvel = 5,

        [Description("Upload")]
        Upload = 6,

        [Description("Import")]
        Import = 6,

        [Description("Export")]
        Export = 7,

        [Description("Print")]
        Print = 8
    }
    /// <summary>
    /// Enum các file trong hệ thống
    /// </summary>
    public enum EnumPage
    {
        #region Thống kê

        [LocalizedDescription("TrangChu", typeof(Resource))]
        Home = 1111,

        [LocalizedDescription("Incompetent", typeof(Resource))]
        NoPermisison = 2222,

        [LocalizedDescription("Dasboard", typeof(Resource))]
        Dashboard = 1000,

        #endregion Thống kê

        #region Danh mục

        [LocalizedDescription("Department", typeof(Resource))]
        Office = 3,

        [LocalizedDescription("Regency", typeof(Resource))]
        Title = 4,

        [LocalizedDescription("ViTriChucVu", typeof(Resource))]
        Position = 5,
        #endregion Danh mục

        #region Hệ thống

        [LocalizedDescription("Nguoidung", typeof(Resource))]
        Staff = 1,

        [LocalizedDescription("Role", typeof(Resource))]
        GroupPermission = 1102,

        [LocalizedDescription("SettingPage", typeof(Resource))]
        Pages = 1103,

        [LocalizedDescription("Notification", typeof(Resource))]
        NotifiCommon = 1104,

        [LocalizedDescription("SettingNotifi", typeof(Resource))]
        Setting = 1105,

        #endregion Hệ thống

        #region Cấu hình kho

        [LocalizedDescription("LayoutKho", typeof(Resource))]
        Layout = 1200,

        [LocalizedDescription("DoiTacKho", typeof(Resource))]
        Partner = 1201,

        #endregion Cấu hình kho

        #region Cấu hình bảng giá

        [LocalizedDescription("BangGiaShipHang", typeof(Resource))]
        ShipPrice = 1300,

        #endregion Cấu hình bảng giá

        #region Cấu hình khách hàng

        [LocalizedDescription("LevelKhachHang", typeof(Resource))]
        Level = 1400,

        [LocalizedDescription("UuDaiKhachHang", typeof(Resource))]
        Favorable = 1401,

        [LocalizedDescription("TheoDoiKhachHang", typeof(Resource))]
        Type = 1402,

        [LocalizedDescription("LoaiKhhieuNaiKhachHang", typeof(Resource))]
        ComplainType = 1403,

        #endregion Cấu hình khách hàng

        #region Cấu hình kế toán

        [LocalizedDescription("DoiTuongKeToan", typeof(Resource))]
        AccountantSubject = 1500,

        #endregion Cấu hình kế toán

        #region Cấu hình quỹ-ví

        [LocalizedDescription("DanhSachQuy", typeof(Resource))]
        Fund = 1600,

        [LocalizedDescription("DinhKhoanQuy", typeof(Resource))]
        AccountingRegulations = 1601,

        [LocalizedDescription("DinhKhoanVi", typeof(Resource))]
        AccountingWallet = 1602,

        [LocalizedDescription("DinhKhoanCongNo", typeof(Resource))]
        AccountingPayReceivable = 1603,

        #endregion Cấu hình quỹ-ví
        #region Đơn hàng

        //order
        [LocalizedDescription("XuLyDonHang", typeof(Resource))]
        Order = 6,

        [LocalizedDescription("ThongKeTongTienDDH", typeof(Resource))]
        ReportOrder = 61,

        [LocalizedDescription("ThongKeTienMacCaDDH", typeof(Resource))]
        ReportPriceBargain = 82,

        [LocalizedDescription("ThongKeTinhHinhDonHang", typeof(Resource))]
        ReportProfitBargain = 83,

        [LocalizedDescription("DonHangOrderChuaNhanXuLy", typeof(Resource))]
        OrderNew = 62,

        [LocalizedDescription("DonHangOrder", typeof(Resource))]
        OrderOrder = 63,

        [LocalizedDescription("DonHangOrderTreXuLy", typeof(Resource))]
        OrderDelay = 64,

        [LocalizedDescription("DonHangLe", typeof(Resource))]
        OrderRetail = 6401,

        [LocalizedDescription("QuanLyMaVanDon", typeof(Resource))]
        OrderLadingCode = 350,

        [LocalizedDescription("CauHinhWebsiteChoNhanVien", typeof(Resource))]
        OrderUserWebsite = 351,

        [LocalizedDescription("DonHangOrderChoBaoGiaMoi", typeof(Resource))]
        OrderWaitNew = 6301,

        [LocalizedDescription("DonHangOrderChoBaoGia", typeof(Resource))]
        OrderWait = 6302,

        [LocalizedDescription("DonHangCuaCSKH", typeof(Resource))]
        OrderCustomerCare = 6303,

        //deposit
        [LocalizedDescription("DonHangKyGuiChuaNhanXuLy", typeof(Resource))]
        OrderDepositNew = 65,

        [LocalizedDescription("DonHangKyGui", typeof(Resource))]
        OrderDeposit = 66,

        [LocalizedDescription("DonHangKyGuiTreXuLy", typeof(Resource))]
        OrderDepositDelay = 67,

        [LocalizedDescription("PhieuBaoGiaTimNguonChuaNhanXuLy", typeof(Resource))]
        //source
        StockQuotesNew = 68,

        [LocalizedDescription("PhieuBaoGiaTimNguon", typeof(Resource))]
        StockQuotes = 69,

        [LocalizedDescription("DonHangTimNguon", typeof(Resource))]
        OrderSourcing = 70,

        [LocalizedDescription("DonHangTimNguonMoi", typeof(Resource))]
        OrderSourcingNew = 72,

        [LocalizedDescription("DonHangTimNguonTreXuLy", typeof(Resource))]
        OrderSourcingDelay = 73,

        //customer
        [LocalizedDescription("TraCuuThongTinKhachHang", typeof(Resource))]
        CustomerSearch = 71,

        //order wanning
        [LocalizedDescription("DonHangRuiRo", typeof(Resource))]
        OrderWanning = 74,

        //accountant
        [LocalizedDescription("KeToanThanhToanTienDH", typeof(Resource))]
        OrderAccountant = 75,

        //Hỗ trợ
        [LocalizedDescription("HoTroGiaiQuyetKhieuNai", typeof(Resource))]
        OrderSupport = 76,

        [LocalizedDescription("PhieuYeuCauHoanTien", typeof(Resource))]
        OrderClaimForRefund = 77,

        //Đơn hàng rủi ro
        [LocalizedDescription("ChuaCoMVD", typeof(Resource))]
        OrderNoCodeOfLading = 78,

        [LocalizedDescription("ChoKeToanTT", typeof(Resource))]
        OrderAwaitingPayment = 79,

        [LocalizedDescription("ChuaDuKienVeKho", typeof(Resource))]
        OrderNotEnoughInventory = 80,

        [LocalizedDescription("KiemDemThieuHang", typeof(Resource))]
        OrderTallyMissing = 81,

        //Đơn hàng thương mại
        [LocalizedDescription("DHThuongMai", typeof(Resource))]
        OrderCommerce = 360,

        //hoàn thành đơn
        [LocalizedDescription("HoanThanhDH", typeof(Resource))]
        OrderSuccess = 298,

        [LocalizedDescription("DanhSachShop", typeof(Resource))]
        Shop = 90,

        [LocalizedDescription("ChuyenMucNganhHang", typeof(Resource))]
        Category = 91,

        [LocalizedDescription("Products", typeof(Resource))]
        Product = 92,

        #endregion Đơn hàng
        #region Kho

        [LocalizedDescription("TheoDoiPhiPhatSinh", typeof(Resource))]
        ServiceOther = 7,

        [LocalizedDescription("Package", typeof(Resource))]
        Package = 8,

        [LocalizedDescription("YeuCauNhapKho_Package", typeof(Resource))]
        ImportWarehouse = 9,

        [LocalizedDescription("YeuCauNhapKho_packaging", typeof(Resource))]
        ImportWarehouseWallet = 22,

        [LocalizedDescription("BaoHang", typeof(Resource))]
        Wallet = 10,

        [Description("PutAway")]
        PutAway = 11,

        [Description("Packing List")]
        PackingList = 12,

        [LocalizedDescription("PhieuDieuVan", typeof(Resource))]
        Dispatcher = 13,

        [LocalizedDescription("PhieuDieuVan", typeof(Resource))]
        WalletTracker = 18,

        [LocalizedDescription("PhieuGiaoHang", typeof(Resource))]
        Delivery = 14,

        [LocalizedDescription("YeuCauXuatKho", typeof(Resource))]
        ExportWarehouse = 15,

        [LocalizedDescription("XacNhanPhieuXuatKho", typeof(Resource))]
        ExportWarehouseApprovel = 16,

        [LocalizedDescription("KiemDemHangHoa", typeof(Resource))]
        OrderDetailAcounting = 17,

        [LocalizedDescription("DongKienGo", typeof(Resource))]
        Packing = 19,

        [LocalizedDescription("KienHangMatMa", typeof(Resource))]
        PackageNoCode = 801,

        [LocalizedDescription("TrungMVD", typeof(Resource))]
        SameTransportCode = 811,

        [LocalizedDescription("XacNhanKienHang", typeof(Resource))]
        PackageNoCodeApprovel = 802,

        [LocalizedDescription("KiemDemSai", typeof(Resource))]
        AcountingLose = 803,

        [LocalizedDescription("PhieuTheoDoiHoanTien", typeof(Resource))]
        OrderRefundPrice = 804,

        [LocalizedDescription("PhieuTheoDoiDoiTra", typeof(Resource))]
        OrderRefundProduct = 805,

        [LocalizedDescription("KienHangChoXuatGiao", typeof(Resource))]
        AwaitingDelivery = 806,

        [LocalizedDescription("TheoDoiKienHang", typeof(Resource))]
        TrackingPackage = 807,

        [LocalizedDescription("KienHangMatThongTin", typeof(Resource))]
        PackageNoCodeProcess = 808,

        [LocalizedDescription("PhieuChuyenNoiBo", typeof(Resource))]
        Transfer = 809,
        // Report
        [Description("Báo cáo kiện hàng")]
        PackageReport = 810,
        #endregion Kho


        #region Phân quyền kế toán

        [LocalizedDescription("DanhSachNap_TruTienQuy", typeof(Resource))]
        FundBill = 100,                                 //FundBill

        [LocalizedDescription("DanhSachNap_TruTienVi", typeof(Resource))]
        RechargeBill = 101,                             //RechargeBill

        [LocalizedDescription("DanhSachXuLyYeuCauRutTien", typeof(Resource))]
        WithDrawal = 102,                               //WithDrawal

        [Description("Danh sách công nợ ")]
        Debit = 103,                            //Withdrawal

        [LocalizedDescription("ThanhToanTienDatHang", typeof(Resource))]
        AccountantOrder = 104,                               //AccountantOrder

        [LocalizedDescription("HoTroGiaiQuyetKN", typeof(Resource))]
        AccountantSupportTicket = 105,                  //AccountantSupportTicket

        [LocalizedDescription("XuLyYeuCauHoanTien", typeof(Resource))]
        ExecuteClaimForRefund = 106,                    //ExecuteClaimForRefund

        [LocalizedDescription("TraCuuThongTinKH", typeof(Resource))]
        AccountantFindCustomer = 107,                   //AccountantFindCustomer

        [LocalizedDescription("ThongKeQuy", typeof(Resource))]
        AccountantReportFundBill = 108,                 //AccountantReportFundBill

        [LocalizedDescription("ThongKeThuChi", typeof(Resource))]
        AccountantReportAll = 109,                      //AccountantReportAll

        [LocalizedDescription("ThongKeCongNo", typeof(Resource))]
        AccountantReportMust = 110,                     //AccountantReportMust

        [LocalizedDescription("TheoDoiCongNoDH", typeof(Resource))]
        TrackingDebt = 120,                     //TrackingDebt

        [LocalizedDescription("TheoDoiTienPhaiThu", typeof(Resource))]
        DebitReport = 121,

        #endregion Phân quyền kế toán

        #region Kinh doanh

        [LocalizedDescription("KHTiemNang", typeof(Resource))]
        PotentialCustomer = 300,

        [LocalizedDescription("KHTiemNangDangPhuTrach", typeof(Resource))]
        PotentialCustomerByStaff = 301,

        [LocalizedDescription("KHChinhThuc", typeof(Resource))]
        Customer = 302,

        [LocalizedDescription("KHChinhThucDangPhuTrach", typeof(Resource))]
        CustomerbyStaff = 303,

        [LocalizedDescription("HoTroGiaiQuyetKN", typeof(Resource))]
        BussinessSupport = 304,

        [LocalizedDescription("TraCuuThongTinKH", typeof(Resource))]
        BussinessReportCustomer = 305,

        [LocalizedDescription("ThongKeDoanhSoNV", typeof(Resource))]
        BussinessReportRevenue = 306,

        [LocalizedDescription("ThongKeKH", typeof(Resource))]
        BussinessCustomer = 307,

        [LocalizedDescription("ThongKeKHChamDH", typeof(Resource))]
        BussinessLastOrder = 308,

        #endregion Kinh doanh

        #region CSKH

        [LocalizedDescription("TicketChuaNhanXuLy", typeof(Resource))]
        TicketAssign = 200,

        [LocalizedDescription("TicketKhieuNai", typeof(Resource))]
        TicketComplain = 201,

        [LocalizedDescription("TicketToiCanXuLy", typeof(Resource))]
        TicketForMe = 202,

        [LocalizedDescription("TicketBiTreXuLy", typeof(Resource))]
        TicketLast = 203,

        [LocalizedDescription("TicketCoNguoiHoTro", typeof(Resource))]
        TicketSupport = 204,

        [LocalizedDescription("TicketTraCuuThongTinKH", typeof(Resource))]
        TicketSearchCustomer = 205,

        [LocalizedDescription("TicketYeuCauHoanTien", typeof(Resource))]
        TicketClaimforrefund = 206,

        [LocalizedDescription("TichketReportTheoNgay", typeof(Resource))]
        TicketReportForDay = 207,

        [LocalizedDescription("DonHangChoBaoGia", typeof(Resource))]
        OrderWaitPrice = 208,

        [LocalizedDescription("DonHangDangBaoGia", typeof(Resource))]
        OrderDoingPrice = 209,

        [LocalizedDescription("DanhSachDonHang", typeof(Resource))]
        ListOrder = 210,

        #endregion CSKH

        #region Profile

        [LocalizedDescription("ThongTinTaiKhoan", typeof(Resource))]
        Profile = 1554

        #endregion Profile
    }

    public enum CustomerCallHistoryMode
    {
        [LocalizedDescription("GoiDienGiaoHang", typeof(Resource))]
        CallDelivery,
    }

    public enum OfficeType
    {
        [LocalizedDescription("BinhThuong", typeof(Resource))]
        Normal,

        [LocalizedDescription("Kho", typeof(Resource))]
        Warehouse,

        [LocalizedDescription("ThongTinTaiKhoan", typeof(Resource))]
        //[Description("Nhân sự")]
        HumanResources,

        [LocalizedDescription("KeToan", typeof(Resource))]
        Accountancy,

        //todo:Giỏi bổ xung thêm
        [LocalizedDescription("KinhDoanh", typeof(Resource))]
        Business,

        [LocalizedDescription("CSKH", typeof(Resource))]
        CustomerCare,

        [LocalizedDescription("DatHang", typeof(Resource))]
        Order,

        [LocalizedDescription("BanGiamDoc", typeof(Resource))]
        Directorate,

        [Description("KDHKG")]
        Deposit
    }

    public enum RecentMode
    {
        [LocalizedDescription("DonHang", typeof(Resource))]
        Order,

        [LocalizedDescription("NhanVienGiaoHang", typeof(Resource))]
        Shipper,

        [LocalizedDescription("NhanVien", typeof(Resource))]
        User,
    }

    public enum GroupChatId
    {
        Order,
        News,
        TaskDetail,
        ProjectDetail,
        PlanDetail,
        PlanPending,
        Quotation,
        Memorandum,
        Customer
    }

    public enum OfficeStatus
    {
        //0: Mới tạo, 1 Đang sử dụng, 2 Đơn vị cũ
        [LocalizedDescription("MoiTao", typeof(Resource))]
        New,

        [LocalizedDescription("DangSuDung", typeof(Resource))]
        Use,

        [LocalizedDescription("Cu", typeof(Resource))]
        Old
    }

    public enum PackageNoteMode
    {
        [LocalizedDescription("DatHang", typeof(Resource))]
        Order,

        [LocalizedDescription("NhapKhoTQ", typeof(Resource))]
        ChinaImport,

        [LocalizedDescription("PutKhoTQ", typeof(Resource))]
        ChinaPutaway,

        [LocalizedDescription("DongKienGo", typeof(Resource))]
        Packing,

        [LocalizedDescription("DongBao", typeof(Resource))]
        Wallet,

        [LocalizedDescription("DienVan", typeof(Resource))]
        Dispatch,

        [LocalizedDescription("NhapKhoVN", typeof(Resource))]
        Import,

        [LocalizedDescription("PutawayVN", typeof(Resource))]
        Putaway,

        [LocalizedDescription("DieuChuyenNoiBo", typeof(Resource))]
        Transfer,

        [LocalizedDescription("GiaoHang", typeof(Resource))]
        Delivery,

        [LocalizedDescription("HoanThanhDon", typeof(Resource))]
        Comleted,

        [LocalizedDescription("PhiPhatSinh", typeof(Resource))]
        OtherSerive,

        [LocalizedDescription("KienHangMatMa", typeof(Resource))]
        PackageNoCode,

        [LocalizedDescription("CapNhatCanNang", typeof(Resource))]
        UpdateWeight,

        [LocalizedDescription("CapNhatDon", typeof(Resource))]
        UpdateOrder,
    }

    public enum Application
    {
        JavaScript = 0,
        NativeConfidential = 1
    }

    public enum CategoryStatus
    {
        Active = 0,         // Kích hoạt
        NoActive = 1        // Chưa kích hoạt
    }

    public enum Currency
    {
        [LocalizedDescription("TienMy", typeof(Resource))]
        USD,

        [LocalizedDescription("TienTrung", typeof(Resource))]
        CNY,

        [LocalizedDescription("TienViet", typeof(Resource))]
        VND,

        [LocalizedDescription("TienThai", typeof(Resource))]
        BATH,

        [LocalizedDescription("TienAlipay", typeof(Resource))]
        ALP,
    }

    public enum CreatedTool
    {
        Extension,
        Excell,
        West,
        Auto
    }

    public enum ServicePack
    {
        [LocalizedDescription("GoiKinhDoanh", typeof(Resource))]
        Business,

        [LocalizedDescription("GoiTieuDung", typeof(Resource))]
        Consumer
    }


    public enum OrderType
    {
        [LocalizedDescription("DonHangKyGui", typeof(Resource))]
        Deposit,

        [LocalizedDescription("DonHangOrder", typeof(Resource))]
        Order,

        [LocalizedDescription("DonHangTimNguon", typeof(Resource))]
        Source,

        [LocalizedDescription("PhieuTimNguon", typeof(Resource))]
        StockQuotes,

        [LocalizedDescription("DHThuongMai", typeof(Resource))]
        Commerce
    }

    public enum UnitType
    {
        [LocalizedDescription("Percent", typeof(Resource))]
        Percent,

        [LocalizedDescription("Currency", typeof(Resource))]
        Money
    }

    public enum OrderServiceMode
    {
        [LocalizedDescription("SelectOp", typeof(Resource))]
        Option,

        [LocalizedDescription("BatBuoc", typeof(Resource))]
        Required
    }


    public enum OrderExchangeStatus
    {
        [LocalizedDescription("ChoDuyet", typeof(Resource))]
        New,

        [LocalizedDescription("DaDuyet", typeof(Resource))]
        Approved
    }
    public enum OrderExchangeOrderType
    {
        [LocalizedDescription("DonHang", typeof(Resource))]
        Order,

        [LocalizedDescription("DonKyGui", typeof(Resource))]
        Deposit,

        [LocalizedDescription("DonTimNguon", typeof(Resource))]
        Source
    }

    //public enum TransportMethod
    //{
    //    [Description("Tiểu nghạch")]
    //    Mini = 1,
    //    [Description("Đường biển")]
    //    Ship = 2,
    //    [Description("Container")]
    //    Container = 3,
    //    [Description("Đường hàng không")]
    //    Air = 4,
    //    [Description("Ô tô")]
    //    Car = 5,
    //    [Description("Tàu hỏa")]
    //    Train = 6,
    //}

    public enum TransportPartner
    {
        [LocalizedDescription("CongTyTuVanChuyen", typeof(Resource))]
        Self = 1,

        [Description("Partner1")]
        Partner1 = 2,

        [Description("Partner2")]
        Partner2 = 3
    }

    public enum DispatcherStatus
    {
        [LocalizedDescription("Moi", typeof(Resource))]
        New,

        [LocalizedDescription("DaDuyet", typeof(Resource))]
        Approved,

        [LocalizedDescription("HoanThanh", typeof(Resource))]
        Complete,

        [LocalizedDescription("Huy", typeof(Resource))]
        Cancel
    }

    public enum OrderMode
    {
        [LocalizedDescription("CungCap", typeof(Resource))]
        NotFix, // 0

        [LocalizedDescription("LienQuan", typeof(Resource))]
        Fixed, // 1
    }

    public enum OrderServices
    {
        //[Description("Phí dịch vụ mua hàng")]
        //Order, // 0
        [LocalizedDescription("PDVKiemDem", typeof(Resource))]
        Audit, // 1

        [LocalizedDescription("PDVDongKien", typeof(Resource))]
        Packing, // 2

        [LocalizedDescription("PhiShopVanChuyen", typeof(Resource))]
        ShopShipping, // 4

        [LocalizedDescription("PDVChuyenHangVeVN", typeof(Resource))]
        OutSideShipping, // 5

        [LocalizedDescription("PDVGiaoHangTaiNha", typeof(Resource))]
        InSideShipping,

        //[Description("Phí dịch vụ vận chuyển nhanh (đường hàng không)")]
        //FastDelivery,
        [LocalizedDescription("PDVPhatSinh", typeof(Resource))]
        Other,

        [LocalizedDescription("PhiDongBao", typeof(Resource))]
        PackingCharge,

        [LocalizedDescription("PhiLuuKho", typeof(Resource))]
        Storaged,

        [LocalizedDescription("PhiHangLe", typeof(Resource))]
        RetailCharge
    }

    public enum OrderDetailCountingStatus
    {
        [LocalizedDescription("MoiTao", typeof(Resource))]
        New, // 0

        [LocalizedDescription("DatHangTiepNhan", typeof(Resource))]
        OrderAccept, // 1
    }

    public enum OrderDetailCountingMode
    {
        //0: Yêu cầu xử lý thiếu sản phẩm, 1: Yêu cầu sử lý sai hàng, 2: Yêu cầu xử lý đổi trả hàng

        [LocalizedDescription("ThieuSanPham", typeof(Resource))]
        Lose, // 0

        [LocalizedDescription("SaiHang", typeof(Resource))]
        Wrong, // 1

        [LocalizedDescription("DoiTraHang", typeof(Resource))]
        Switch // 2
    }

    public enum OrderExchangeType
    {
        [LocalizedDescription("TienHangHoa", typeof(Resource))]
        Product,

        [LocalizedDescription("TienMuaHangHo", typeof(Resource))]
        Order,

        [LocalizedDescription("TienShopChuyenHang", typeof(Resource))]
        ShopShipping,

        [LocalizedDescription("TienKiemDemHangHoa", typeof(Resource))]
        Audit,

        [LocalizedDescription("TienDongKienGo", typeof(Resource))]
        Packing,

        [LocalizedDescription("TienHangChuyenVeVietNam", typeof(Resource))]
        OutSideShipping,

        [LocalizedDescription("TienChuyenHangToiNha", typeof(Resource))]
        InSideShipping,

        [LocalizedDescription("TienKhachThanhToan", typeof(Resource))]
        Pay,
    }

    public enum OrderExchangeMode
    {
        /// <summary>
        /// Khách xuất tiền thanh toán cho Công Ty
        /// </summary>
        [LocalizedDescription("ThanhToan", typeof(Resource))]
        Export,

        /// <summary>
        /// Công ty xuất tiền hoàn trả khách
        /// </summary>
        [LocalizedDescription("HoanTra", typeof(Resource))]
        Import
    }

    public enum OrderStatus
    {
        [LocalizedDescription("Moi", typeof(Resource))]
        [Display(Name = "Mới")]
        New,///

        [LocalizedDescription("ChoBaoGia", typeof(Resource))]
        [Display(Name = "Chờ báo giá")]
        WaitPrice,///1

        [LocalizedDescription("DangBaoGia", typeof(Resource))]
        [Display(Name = "Đang báo giá")]
        AreQuotes,///2

        [LocalizedDescription("ChoDatCoc", typeof(Resource))]
        [Display(Name = "Chờ đặt cọc")]
        WaitDeposit,///3

        [LocalizedDescription("ChoDatHang", typeof(Resource))]
        [Display(Name = "Chờ đặt hàng")]
        WaitOrder,///4

        [LocalizedDescription("DangDatHang", typeof(Resource))]
        [Display(Name = "Đang đặt hàng")]
        Order,///5

        [LocalizedDescription("ChoKeToanTT", typeof(Resource))]
        [Display(Name = "Chờ kế toán thanh toán")]
        WaitAccountant,///6

        [LocalizedDescription("KeToanDangThanhToan", typeof(Resource))]
        [Display(Name = "Kế toán đang thanh toán")]
        AccountantProcessing,///7

        [LocalizedDescription("DatHangXong", typeof(Resource))]
        [Display(Name = "Đặt hàng xong")]
        OrderSuccess,///8

        /// <summary>
        /// 9: Shop phát hàng
        /// </summary>

        [LocalizedDescription("ShopPhatHang", typeof(Resource))]
        [Display(Name = "Shop phát hàng")]
        DeliveryShop,

        /// <summary>
        /// 10: Nhận hàng
        /// </summary>
        [LocalizedDescription("NhanHang", typeof(Resource))]
        [Display(Name = "Nhận hàng")]
        InWarehouse,

        /// <summary>
        /// 11: Vận chuyển
        /// </summary>
        [LocalizedDescription("TrenDuongVeVN", typeof(Resource))]
        [Display(Name = "Trên đường về VN")]
        Shipping,

        /// <summary>
        /// 12: Chờ giao hàng
        /// </summary>
        [LocalizedDescription("ChoGiaoHang", typeof(Resource))]
        [Display(Name = "Chờ giao hàng")]
        Pending,

        /// <summary>
        /// 13: Đang giao hàng
        /// </summary>

        [LocalizedDescription("DangGiaoHang", typeof(Resource))]
        [Display(Name = "Đang giao hàng")]
        GoingDelivery,

        /// <summary>
        /// 14: Hoàn thành
        /// </summary>
        [LocalizedDescription("HoanThanh", typeof(Resource))]
        [Display(Name = "Hoàn thành")]
        Finish,

        [LocalizedDescription("Huy", typeof(Resource))]
        [Display(Name = "Hủy")]
        Cancel,///15

        [LocalizedDescription("HongMat", typeof(Resource))]
        [Display(Name = "Hỏng mất")]
        Lost,///16
    }
    public enum DepositStatus
    {
        [LocalizedDescription("ChoBaoGia", typeof(Resource))]
        [Display(Name = "Chờ báo giá")]
        WaitDeposit,///0

        [LocalizedDescription("DangXuLy", typeof(Resource))]
        [Display(Name = "Đang xử lý")]
        Processing,///1

        [LocalizedDescription("ChoDuyetGia", typeof(Resource))]
        [Display(Name = "Chờ duyệt giá")]
        PendingPrice,///2

        [LocalizedDescription("ChoKetDon", typeof(Resource))]
        [Display(Name = "Chờ kết đơn")]
        WaitOrder,///3

        [LocalizedDescription("ChoNhanHang", typeof(Resource))]
        [Display(Name = "Chờ nhận hàng")]
        Order,//4

        /// <summary>
        /// 5: Nhận hàng
        /// </summary>

        [LocalizedDescription("NhanHang", typeof(Resource))]
        [Display(Name = "Nhận hàng")]
        InWarehouse,

        /// <summary>
        /// 6: Vận chuyển
        /// </summary>

        [LocalizedDescription("VanChuyen", typeof(Resource))]
        [Display(Name = "Vận chuyển")]
        Shipping,

        /// <summary>
        /// 7: Chờ giao hàng
        /// </summary>
        [LocalizedDescription("ChoGiaoHang", typeof(Resource))]
        [Display(Name = "Chờ giao hàng")]
        Pending,

        /// <summary>
        /// 8: Đang giao hàng
        /// </summary>
        [LocalizedDescription("DangGiaoHang", typeof(Resource))]
        [Display(Name = "Đang giao hàng")]
        GoingDelivery,

        /// <summary>
        /// 9: Hoàn thành
        /// </summary>
        [LocalizedDescription("HoanThanh", typeof(Resource))]
        [Display(Name = "Hoàn thành")]
        Finish,

        [LocalizedDescription("Huy", typeof(Resource))]
        [Display(Name = "Hủy")]
        Cancel,//10
    }
    public enum OrderDetailStatus
    {
        [LocalizedDescription("DatDuocHang", typeof(Resource))]
        Order,

        [LocalizedDescription("Huy", typeof(Resource))]
        Cancel,

        [LocalizedDescription("ChoBaoGia", typeof(Resource))]
        WaitPrice,
    }

    public enum ExportWarehouseDetailStatus
    {
        [LocalizedDescription("ChoXacNhan", typeof(Resource))]
        New,

        [LocalizedDescription("DaXacNhan", typeof(Resource))]
        Approved
    }
    public enum OrderPackageStatus
    {
        /// <summary>
        /// Kho Trung Quốc chờ nhập hàng (Shop TQ xuất hàng cho nhà vận chuyển)
        /// </summary>
        [LocalizedDescription("ShopTQPhatHang", typeof(Resource))]
        ShopDelivery,

        /// <summary>
        /// Hàng về kho TQ nhập vào kho (phiếu nhập kho nhưng chưa Putaway)
        /// </summary>
        [LocalizedDescription("KhoTQNhanHang", typeof(Resource))]
        ChinaReceived,

        /// <summary>
        /// Hàng đã nhận vào kho chờ đóng bao hàng
        /// </summary>
        [LocalizedDescription("DangTrongKhoTQ", typeof(Resource))]
        ChinaInStock,

        /// <summary>
        /// Tạo bao hàng (Nhưng chưa giao cho nhà vận chuyển)
        /// </summary>
        [LocalizedDescription("XuatKhoTQ", typeof(Resource))]
        ChinaExport,

        /// <summary>
        /// Giao hàng cho vận chuyển
        /// </summary>
        [LocalizedDescription("TrenDuongVeVN", typeof(Resource))]
        PartnerDelivery,

        /// <summary>
        /// Nhập bao hàng vào kho HN, HCM
        /// </summary>
        [LocalizedDescription("NhanHang", typeof(Resource))]
        Received,

        /// <summary>
        /// Nhập kiện hàng vào kho (sau Putaway)
        /// </summary>
        [LocalizedDescription("TrongKho", typeof(Resource))]
        InStock,

        /// <summary>
        /// Hàng đang được điều chuyển trong kho nội bộ
        /// </summary>
        [LocalizedDescription("DangDieuChuyen", typeof(Resource))]
        Transferring,

        /// <summary>
        /// Hàng được tạo phiếu giao chờ kế toán duyệt
        /// </summary>
        [LocalizedDescription("ChoGiaoHang", typeof(Resource))]
        WaitDelivery,

        /// <summary>
        /// Kiện hàng đã xuất kho đang đi giao cho khách (Kế toán duyệt phiếu giao hàng)
        /// </summary>
        [LocalizedDescription("DangDiGiaoHang", typeof(Resource))]
        GoingDelivery,

        /// <summary>
        /// Xác nhận công nợ và khách hàng đã nhận đủ hàng theo phiếu
        /// </summary>
        [LocalizedDescription("DaTraHang", typeof(Resource))]
        Completed,

        /// <summary>
        /// Hàng đã bị mất chưa xử lý
        /// </summary>
        [LocalizedDescription("HangBiMat", typeof(Resource))]
        Lost,

        /// <summary>
        /// Hàng đã bị mất mã chưa xử lý
        /// </summary>
        [LocalizedDescription("HangBiMatMa", typeof(Resource))]
        LoseCode,

        /// <summary>
        /// Hàng bị mất đã xử lý
        /// </summary>
        [LocalizedDescription("HangBiMatDaXuLy", typeof(Resource))]
        LostCompleted
    }

    public enum CommnetOrderType
    {
        [LocalizedDescription("DonHangOrder", typeof(Resource))]
        Order,

        [LocalizedDescription("DonHangKyGui", typeof(Resource))]
        Deposit,

        [LocalizedDescription("DonHangTimNguon", typeof(Resource))]
        Source
    }
    //public enum SystemConfig
    //{
    //    [Description("nhaphangkinhdoanh.com")]
    //    Nhaphangkinhdoanh = 1,
    //    [Description("timnguonhanggiare.com")]
    //    TimNguonHangGiaRe = 2,
    //}

    #region Trạng thái khiếu nại

    public enum ComplainStatus
    {
        [LocalizedDescription("ChoXuLy", typeof(Resource))]
        Wait = 0,

        [LocalizedDescription("DangXuLy", typeof(Resource))]
        Process = 1,

        [LocalizedDescription("ChoDatHangXuLy", typeof(Resource))]
        OrderWait = 2,

        [LocalizedDescription("ChoCSKHXuLy", typeof(Resource))]
        CustomerCareWait = 3,

        [LocalizedDescription("ChoPheDuyet", typeof(Resource))]
        ApprovalWait = 4,

        [LocalizedDescription("ChoKeToanXuLy", typeof(Resource))]
        AccountantWait = 5,

        [LocalizedDescription("KeToanDaHoanTien", typeof(Resource))]
        AccountantFinish = 6,

        [LocalizedDescription("HoanThanh", typeof(Resource))]
        Success = 7,

        [LocalizedDescription("DaHuy", typeof(Resource))]
        Cancel = 8
    }

    #endregion Trạng thái khiếu nại


    public enum SourceStatus
    {
        [LocalizedDescription("ChoXuLy", typeof(Resource))]
        WaitProcess,

        [LocalizedDescription("DangXuLy", typeof(Resource))]
        Process,

        [LocalizedDescription("ChoKhachChonNCC", typeof(Resource))]
        WaitingChoice,

        [LocalizedDescription("HoanThanh", typeof(Resource))]
        Success,

        [LocalizedDescription("DaHuy", typeof(Resource))]
        Cancel
    }
    public enum SourceTypeService
    {
        [LocalizedDescription("GoiMot", typeof(Resource))]
        Package1,

        [LocalizedDescription("GoiHai", typeof(Resource))]
        Package2
    }
    #region Warehouse

    public enum ImportWarehouseStatus
    {
        [LocalizedDescription("MoiKhoiTao", typeof(Resource))]
        New = 0,                                        // Mới khởi tạo

        [LocalizedDescription("DaDuyet", typeof(Resource))]
        Approved = 1,                                   // Đã duyệt
    }

    public enum OrderDetailAcounting
    {
        [LocalizedDescription("ChoKiemDem", typeof(Resource))]
        Await = 0,

        [LocalizedDescription("DaKiemDem", typeof(Resource))]
        Acounted = 1,

        [LocalizedDescription("KiemDemSai", typeof(Resource))]
        Lose = 2,  // Đã duyệt
    }

    public enum WalletStatus
    {
        [LocalizedDescription("MoiKhoiTao", typeof(Resource))]
        New = 0,                                        // Mới khởi tạo

        [LocalizedDescription("DaDuyet", typeof(Resource))]
        Approved = 1,                                   // Đã duyệt

        [LocalizedDescription("TrongKho", typeof(Resource))]
        InStock = 2,                                   // Đã duyệt

        [LocalizedDescription("DangVanChuyen", typeof(Resource))]
        Shipping = 3,

        [LocalizedDescription("Mat", typeof(Resource))]
        Lose = 4,

        [LocalizedDescription("HoanThanh", typeof(Resource))]
        Complete = 5
    }

    public enum PutAwayStatus
    {
        [LocalizedDescription("MoiKhoiTao", typeof(Resource))]
        New = 0,                                        // Mới khởi tạo

        [LocalizedDescription("HoanThanh", typeof(Resource))]
        Complete = 5
    }

    public enum ExportWarehouseStatus
    {
        [LocalizedDescription("MoiKhoiTao", typeof(Resource))]
        New = 0,                                        // Mới khởi tạo

        [LocalizedDescription("DaDuyet", typeof(Resource))]
        Approved = 1,                                   // Đã duyệt
    }

    public enum PackingListStatus
    {
        [LocalizedDescription("MoiKhoiTao", typeof(Resource))]
        New = 0,                                        // Mới khởi tạo

        [LocalizedDescription("DaDuyet", typeof(Resource))]
        Approved = 1,                                   // Đã duyệt
    }

    public enum PartnerStatus
    {
        [LocalizedDescription("Moi", typeof(Resource))]
        New,

        [LocalizedDescription("HienTai", typeof(Resource))]
        Current,

        [LocalizedDescription("Cu", typeof(Resource))]
        Old
    }

    public enum EnumNotifyType
    {
        Warning = 0,
        Info = 1,
        CustomerWarning,
        CustomerInfo
    }

    public enum NotificationType
    {
        [LocalizedDescription("ThongBao", typeof(Resource))]
        Notification,

        [LocalizedDescription("NhacNho", typeof(Resource))]
        Reminder
    }

    public enum DeliveryStatus
    {
        /// <summary>
        /// Mới tạo phiếu, chờ kế toán duyệt
        /// </summary>

        [LocalizedDescription("ChoDuyet", typeof(Resource))]
        New = 0,

        /// <summary>
        /// Kế toán đã duyệt phiếu
        /// </summary>

        [LocalizedDescription("DaDuyet", typeof(Resource))]
        Approved = 1,

        /// <summary>
        /// Đã gán đơn hàng cho Shipper
        /// </summary>
        [LocalizedDescription("DaXuatGiao", typeof(Resource))]
        Assigned = 2,

        /// <summary>
        /// Phiếu đang trên đường vận chuyển
        /// </summary>
        [LocalizedDescription("DaXuatGiao", typeof(Resource))]
        Delivery = 3,

        /// <summary>
        /// Hoàn thàn phiếu giao
        /// </summary>

        [LocalizedDescription("HoanThanh", typeof(Resource))]
        Complete = 4,

        /// <summary>
        /// Hủy phiếu
        /// </summary>

        [LocalizedDescription("Huy", typeof(Resource))]
        Cancel = 5,

        /// <summary>
        /// Hoàn phiếu về kho hàng
        /// </summary>
        [LocalizedDescription("HoanPhieu", typeof(Resource))]
        Recovery = 6,
    }

    #endregion Warehouse

    #region Accountant

    public enum FundBillStatus
    {
        [LocalizedDescription("ChoDuyet", typeof(Resource))]
        New = 0,                                        // Chờ duyệt

        [LocalizedDescription("DaDuyet", typeof(Resource))]
        Approved = 1,                                   // Đã duyệt
    }

    public enum FundBillType
    {
        [LocalizedDescription("NapTienQuy", typeof(Resource))]
        Increase = 0,                                        // Nạp tiền quỹ

        [LocalizedDescription("ChiTienQuy", typeof(Resource))]
        Diminishe = 1,                                   // Chi tiền quỹ
    }

    public enum RechargeBillStatus
    {
        [LocalizedDescription("ChoDuyet", typeof(Resource))]
        New = 0,                                        // Chờ duyệt

        [LocalizedDescription("DaDuyet", typeof(Resource))]
        Approved = 1,                                   // Đã duyệt
    }

    public enum RechargeBillType
    {
        [LocalizedDescription("NapTienVi", typeof(Resource))]
        Increase = 0,                                        // Nạp tiền ví

        [LocalizedDescription("ChiTienVi", typeof(Resource))]
        Diminishe = 1,                                   // Chi tiền ví
    }

    public enum MustCollectStatus
    {
        [LocalizedDescription("ChoThu", typeof(Resource))]
        New = 0,                                        // Chờ thu

        [LocalizedDescription("HoanTatThu", typeof(Resource))]
        Approved = 1,                                   // Hoàn tất thu
    }

    public enum MustReturnStatus
    {
        [LocalizedDescription("ChoTra", typeof(Resource))]
        New = 0,                                        // Chờ trả

        [LocalizedDescription("HoanTatTra", typeof(Resource))]
        Approved = 1,                                   // Hoàn tất trả
    }

    public enum WithDrawalStatus
    {
        [LocalizedDescription("ChoXuLy", typeof(Resource))]
        //[Description("Chờ xử lý")]
        New = 0,                                        // Chờ xử lý

        [LocalizedDescription("DaXuLy", typeof(Resource))]
        Approved = 1,                                   // Đã xử lý
    }

    #endregion Accountant


    #region CustomerOfStaff

    public enum CustomerOfStaffStatus
    {
        //[LocalizedDescription("DangMo", typeof(Resource))]
        [Description("Active")]
        New = 0,                                        // Mới khởi tạo
        [Description("Non-access")]
        //[LocalizedDescription("TamNgung", typeof(Resource))]
        Approved = 1,                                   // Đã duyệt
        [Description("Deleted")]
        //[LocalizedDescription("BiXoa", typeof(Resource))]
        Deleted = 2,                                   // Bị xóa
    }

    #endregion CustomerOfStaff

    #region Quốc gia

    public enum CountryName
    {
        [LocalizedDescription("VietNam", typeof(Resource))]
        VN,

        [LocalizedDescription("TrungQuoc", typeof(Resource))]
        CN,

        [LocalizedDescription("ThaiLan", typeof(Resource))]
        TL,

        [LocalizedDescription("Indonesia", typeof(Resource))]
        InDo
    }

    #endregion Quốc gia

    #region Ticket

    public enum TicketStatus
    {
        [LocalizedDescription("DangMo", typeof(Resource))]
        New = 0,                                        // Mới khởi tạo

        [LocalizedDescription("TamNgung", typeof(Resource))]
        Approved = 1,                                   // Đã duyệt

        [LocalizedDescription("BiXoa", typeof(Resource))]
        Deleted = 2,                                   // Bị xóa
    }

    public enum ClaimForRefundStatus
    {
        [LocalizedDescription("ChoDatHangXuLy", typeof(Resource))]
        OrderWait = 0,

        [LocalizedDescription("ChoCSKHXuLy", typeof(Resource))]
        CustomerCareWait = 1,

        [LocalizedDescription("ChoPheDuyet", typeof(Resource))]
        ApprovalWait = 2,

        [LocalizedDescription("ChoKeToanXuLy", typeof(Resource))]
        AccountantWait = 3,

        [LocalizedDescription("HoanTat", typeof(Resource))]
        Success = 4,

        [LocalizedDescription("HuyHoanBoi", typeof(Resource))]
        Cancel = 5                        // Hủy
    }

    #endregion Ticket

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SexCustomer
    {
        [LocalizedDescription("Undefined", typeof(Resource))]
        Zezo = 2,

        [LocalizedDescription("Male", typeof(Resource))]
        Male = 1,

        [LocalizedDescription("Female", typeof(Resource))]
        Female = 0,
    }

    public enum CommentType
    {
        [Description("Text")]
        Text,

        [LocalizedDescription("Anh", typeof(Resource))]
        Image,

        [LocalizedDescription("BieuTuong", typeof(Resource))]
        Icon,
    }

    public enum ContractCodeType
    {
        [LocalizedDescription("Moi", typeof(Resource))]
        New,

        [LocalizedDescription("DatHangKiemTraLai", typeof(Resource))]
        Review,

        [LocalizedDescription("ChoThanhToan", typeof(Resource))]
        AwaitingPayment,

        [LocalizedDescription("DaThanhToan", typeof(Resource))]
        Paid,
    }
    //Trạng thái hoàn tiền khiếu nại
    public enum statusClaimForRefund
    {
        [LocalizedDescription("ChoXuLy", typeof(Resource))]
        Await,

        [LocalizedDescription("HoanTat", typeof(Resource))]
        Success,

        [LocalizedDescription("HuyHoanBoi", typeof(Resource))]
        Cancel,
    }
    public enum CountingStatus
    {
        [LocalizedDescription("KhoMoiTao", typeof(Resource))]
        New = 0,

        [LocalizedDescription("DatHangTiepNhan", typeof(Resource))]
        Receive = 1,

        [LocalizedDescription("DHGuiYeuCauShop", typeof(Resource))]
        Request = 2,

        [LocalizedDescription("DatHangPhanHoi", typeof(Resource))]
        Feedback = 3,

        [LocalizedDescription("GuiKeToanXacNhan", typeof(Resource))]
        Confirm = 4,

        [LocalizedDescription("DaHoanThanh", typeof(Resource))]
        Finish = 5,

        [LocalizedDescription("KhongXuLyDuoc", typeof(Resource))]
        Cancel = 6,
    }

    //Khách hàng tiềm năng
    public enum PotentialCustomerStatus
    {
        [LocalizedDescription("KichHoat", typeof(Resource))]
        Active,

        [LocalizedDescription("KhongKichHoat", typeof(Resource))]
        NoActive,

        [LocalizedDescription("ChoChuyenChinhThuc", typeof(Resource))]
        Await,
    }
    #region Danh sách các loại khiếu nại

    public enum ComplainTypeClose
    {
        [LocalizedDescription("VoHong", typeof(Resource))]
        Breakdown,

        [LocalizedDescription("HangHongDoNgamNuoc", typeof(Resource))]
        Drench,

        [LocalizedDescription("YinhSaiCanNang", typeof(Resource))]
        FalseWeight,

        [LocalizedDescription("GiamCuocVaPDVDoTraHangMuon", typeof(Resource))]
        LateFees,

        [LocalizedDescription("GiamCuocDoiTraChoKhach", typeof(Resource))]
        ChangeFees,

        [LocalizedDescription("HoanPhiDongKienGo", typeof(Resource))]
        RefundBaled,

        [LocalizedDescription("KiemSai", typeof(Resource))]
        CheckError,

        [LocalizedDescription("MatHangTrangKho", typeof(Resource))]
        WarehouseLose,

        [LocalizedDescription("MatHangTrenDuongVanChuyen", typeof(Resource))]
        TransportLose,

        [LocalizedDescription("MatHangDoBiThu", typeof(Resource))]
        SeizureLose,

        [LocalizedDescription("MatHangDoShopDongKienChung", typeof(Resource))]
        CommonPackageLose,

        [LocalizedDescription("ChuyenHangNhamKho", typeof(Resource))]
        MistakeWarehouse,

        [LocalizedDescription("HoanTienMacCa", typeof(Resource))]
        RefundBargain,

        [LocalizedDescription("ShopKhongGuiHang", typeof(Resource))]
        NoRefundMoney,

        [LocalizedDescription("KhieuNaiHangLoiKyThuat", typeof(Resource))]
        TechnicalError,

        [LocalizedDescription("LyDoKhac", typeof(Resource))]
        Different,
    }

    public enum ComplainTypeService
    {
        [LocalizedDescription("SaiLechPhiVanChuyen", typeof(Resource))]
        FalseShip,

        [LocalizedDescription("HangHong", typeof(Resource))]
        Corrupted,

        [LocalizedDescription("HangVeSaiQuyCach", typeof(Resource))]
        Incorrect,

        [LocalizedDescription("TruSaiTienViDienTu", typeof(Resource))]
        CardError,

        [LocalizedDescription("HangHoaVeCham", typeof(Resource))]
        Slow,

        [LocalizedDescription("ThoiGianDatHangLau", typeof(Resource))]
        TimeLate,

        [LocalizedDescription("ThaiDoPhucVuCuaNhanVien", typeof(Resource))]
        Incivility
    }

    #endregion Danh sách các loại khiếu nại

    public enum OrderLogType
    {
        [LocalizedDescription("DuLieuThongThuong", typeof(Resource))]
        Data,

        [LocalizedDescription("DuLieuJS", typeof(Resource))]
        DataJson,

        [LocalizedDescription("ThaoTac", typeof(Resource))]
        Acction,
    }


    #region [Định khoản thu chi ví điện tử  - Tự động]

    public enum TreasureCustomerWallet
    {
        [LocalizedDescription("DCDonHang", typeof(Resource))]
        AdvanceOrder = 101,

        [LocalizedDescription("TatToanMuaHang", typeof(Resource))]
        CompletedOrder = 102,

        [LocalizedDescription("TTPhieuGiaoHang", typeof(Resource))]
        Delivery = 103,

        [LocalizedDescription("NapTienChoKhacTuNV", typeof(Resource))]
        ShipperReturn = 300,

        [LocalizedDescription("HoanTienDonHang", typeof(Resource))]
        OrderReturn = 301,

        [LocalizedDescription("HoanTienXuLyKNDH", typeof(Resource))]
        ClaimForRefund = 302,

        [LocalizedDescription("HoanTienDonHang", typeof(Resource))]
        Withdrawals = 888,
    }

    #endregion [Định khoản thu chi ví điện tử  - Tự động]

    #region [Định khoản công nợ phải thu - phải trả]

    public enum TreasureMustCollect
    {
        /// <summary>
        /// Phải thu tiền thiếu đơn hàng
        /// </summary>
        [LocalizedDescription("PhaiThuTienThieuDonHang", typeof(Resource))]
        AdvanceOrder = 100,

        /// <summary>
        /// Phải thu tiền vận chuyển PS thêm tại TQ
        /// </summary>
        [LocalizedDescription("PhaiThuTienVanChuyen", typeof(Resource))]
        TransportIncurredOrder = 101,

        /// <summary>
        /// Phải thu tiền đóng kiện đơn hàng
        /// </summary>
        [LocalizedDescription("PhaiThuTienDongKienDH", typeof(Resource))]
        PackedOrder = 102,

        /// <summary>
        /// Phải thu tiền vận chuyển hàng về tới VN
        /// </summary>
        [LocalizedDescription("PhaiThuTienVanChuyenHangVeVN", typeof(Resource))]
        TransportInlandOrder = 103,

        /// <summary>
        /// Phải thu tiền hàng đi nhanh
        /// </summary>
        [LocalizedDescription("PhaiThuTienVanChuyenHangVeVN", typeof(Resource))]
        TransportFast = 105,

        /// <summary>
        /// Phải thu tiền vận chuyển tại VN
        /// </summary>
        [LocalizedDescription("PhaiThuTienVanChuyenTaiVN", typeof(Resource))]
        ShipOrder = 104,

        /// <summary>
        /// Công nợ phải thu nhân viên
        /// </summary>
        [LocalizedDescription("PhaiThuTienNoPhieuXuatHang", typeof(Resource))]
        StaffShip = 200,
    }

    public enum TreasureMustReturn
    {
        [LocalizedDescription("PhaiTraDoThieuLink", typeof(Resource))]
        MissingLinkOrder = 500,

        [LocalizedDescription("PhaiTraDoHuyToanBoDonHang", typeof(Resource))]
        CancelOrder = 501,
    }

    #endregion [Định khoản công nợ phải thu - phải trả]

    public enum TreasureEnum
    {
        [LocalizedDescription("ThanhToanTienDatHang", typeof(Resource))]
        PayForShop = 600,

        [LocalizedDescription("ThuTienPhieuGiaoHang", typeof(Resource))]
        MustCollectFromDelivery = 601,
    }

    public enum DebitHistoryType
    {
        [LocalizedDescription("CongNoPhaiThu", typeof(Resource))]
        mustCollect = 0,                                  // Công nợ phải thu
        [LocalizedDescription("CongNoPhaiTra", typeof(Resource))]
        mustReturn = 1,                                   // Công nợ phải trả
    }

    public enum DebitHistoryStatus
    {
        [LocalizedDescription("ChuaHoanThanh", typeof(Resource))]
        Incomplete = 0,                                  // Chưa hoàn thành

        [LocalizedDescription("HoanThanh", typeof(Resource))]
        Completed = 1,                                   // Hoàn thành
    }

    public enum DebitType
    {
        [LocalizedDescription("PhaiThu", typeof(Resource))]
        Collect,

        [LocalizedDescription("PhaiTra", typeof(Resource))]
        Return,
    }

    public enum OrderReasonType
    {
        [LocalizedDescription("ChamXuLy", typeof(Resource))]
        Delay,

        [LocalizedDescription("ChuaCoMVD", typeof(Resource))]
        NoCodeOfLading,

        [LocalizedDescription("ChuaDuKienVeKho", typeof(Resource))]
        NotEnoughInventory,
    }

    public enum OrderReasonNoCodeOfLading
    {
        [LocalizedDescription("ChuaChonLyDo", typeof(Resource))]
        ReasonsNotSelected,

        [LocalizedDescription("DangDoiTien", typeof(Resource))]
        AreClaiming,

        [LocalizedDescription("DatHangChoSanXuat", typeof(Resource))]
        Produce,

        [LocalizedDescription("ShopCoNgayDuLienPhatHang", typeof(Resource))]
        DeliveryDate,

        [LocalizedDescription("DangLienHeShop", typeof(Resource))]
        Contacting
    }

    public enum OrderReasonNotEnoughInventory
    {
        [LocalizedDescription("ChuaChonLyDo", typeof(Resource))]
        ReasonsNotSelected,

        [LocalizedDescription("DangDoiTien", typeof(Resource))]
        AreClaiming,

        [LocalizedDescription("DangTrenDuongVanChuyen", typeof(Resource))]
        AreInTransit,

        [LocalizedDescription("DaKyNhanChoXNKho", typeof(Resource))]
        WarehousesAwaitingConfirmation,

        [LocalizedDescription("DangLienHeShop", typeof(Resource))]
        Contacting,

        [LocalizedDescription("MaAo", typeof(Resource))]
        VirtualCode,

        [LocalizedDescription("MaTrungChoXuLy", typeof(Resource))]
        CoincidesPendingCode
    }

    public enum OrderReasons
    {
        [LocalizedDescription("ChuaChonLyDo", typeof(Resource))]
        ReasonsNotSelected,

        [LocalizedDescription("ShopKhongOnline", typeof(Resource))]
        ShopNotOnline,

        [LocalizedDescription("DoiKhachXacNhanPhiShip", typeof(Resource))]
        RisksOrders,

        [LocalizedDescription("KhongLienLacDuocVoiKhach", typeof(Resource))]
        NoContact,

        [LocalizedDescription("DonNhieuLinkNhieuSP", typeof(Resource))]
        MultipleProducts,

        [LocalizedDescription("DCNgoaiGioHC", typeof(Resource))]
        DepositOfficeHours,

        [LocalizedDescription("DHXacNhanThongTin", typeof(Resource))]
        CustomerOrdersExchanged,

        [LocalizedDescription("DHGiaTriLon", typeof(Resource))]
        NeedTimeToBargain,

        [LocalizedDescription("LuongDonNhieuTiepNhanMuon", typeof(Resource))]
        ReceivedLate,

        [LocalizedDescription("DonThayDoiGiaSoLuong", typeof(Resource))]
        UnitPriceChanges,

        [LocalizedDescription("DonNhieuYeuCau", typeof(Resource))]
        RespondentRequestedNotes,

        [LocalizedDescription("LyDoDienMang", typeof(Resource))]
        ReasonElectrical,

        [LocalizedDescription("LyDoCaNhan", typeof(Resource))]
        PersonalReasonsAgree,

        [LocalizedDescription("LyDoKhac", typeof(Resource))]
        TheOtherReason
    }


    public enum WarehouseCustomerStatus
    {
        [Description("No warehouse")]
        //[LocalizedDescription("ChuaCoKho", typeof(Resource))]
        NoWarehouse,
        [Description("Have warehouse")]
        [LocalizedDescription("DaCoKho", typeof(Resource))]
        HaveWarehouse
    }

    public enum BargainType
    {
        [LocalizedDescription("ChuaMacCa", typeof(Resource))]
        NotBargain,

        [LocalizedDescription("KhachHangMacCa", typeof(Resource))]
        CustomersBargain,

        [LocalizedDescription("KinhDoanhMacCa", typeof(Resource))]
        BusinessBargain
    }

    public enum Website
    {
        [Description("taobao.com")]
        Taobao,

        [Description("1688.com")]
        W1688,

        [Description("tmall.com")]
        Tmall
    }
}
