using System;

namespace Library.DbContext.Entities
{
    // Delivery

    public partial class Delivery
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã Code phiếu giao hàng
        ///</summary>
        public string Code { get; set; } // Code (length: 50)
        public string UnsignedText { get; set; } // UnsignedText (length: 500)

        ///<summary>
        /// Trạng thái phiếu giao hàng (0: Mới khởi tạo/Chờ duyệt, 1: Đã duyệt, 2: Đã xuất giao, 3: Giao thành công, 4: Hoàn thành phiếu, 5: Hủy/Hoàn phiếu)
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Số lượng đơn hàng trong phiếu giao
        ///</summary>
        public int? OrderNo { get; set; } // OrderNo

        ///<summary>
        /// Số lượng kiện trong phiếu giao
        ///</summary>
        public int? PackageNo { get; set; } // PackageNo

        ///<summary>
        /// Id kho xuất hàng
        ///</summary>
        public int WarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Địa chỉ kho xuất hàng
        ///</summary>
        public string WarehouseIdPath { get; set; } // WarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho xuất hàng
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 300)

        ///<summary>
        /// Địa chỉ kho xuất hàng
        ///</summary>
        public string WarehouseAddress { get; set; } // WarehouseAddress (length: 300)

        ///<summary>
        /// Nhân viên tạo phiếu
        ///</summary>
        public int CreatedUserId { get; set; } // CreatedUserId

        ///<summary>
        /// Tên nhân viên tạo phiếu
        ///</summary>
        public string CreatedUserFullName { get; set; } // CreatedUserFullName (length: 300)

        ///<summary>
        /// UserName của nhân viên tạo phiếu
        ///</summary>
        public string CreatedUserUserName { get; set; } // CreatedUserUserName (length: 50)

        ///<summary>
        /// Chức vụ của nhân viên tạo phiếu
        ///</summary>
        public int CreatedUserTitleId { get; set; } // CreatedUserTitleId

        ///<summary>
        /// Tên chức vụ của nhân viên tạo phiếu
        ///</summary>
        public string CreatedUserTitleName { get; set; } // CreatedUserTitleName (length: 300)

        ///<summary>
        /// Phòng ban nhân viên tạo phiếu
        ///</summary>
        public int CreatedOfficeId { get; set; } // CreatedOfficeId

        ///<summary>
        /// Tên phòng ban của nhân viên tạo phiếu
        ///</summary>
        public string CreatedOfficeName { get; set; } // CreatedOfficeName (length: 300)

        ///<summary>
        /// IdPath phòng ban của nhân viên tạo phiếu
        ///</summary>
        public string CreatedOfficeIdPath { get; set; } // CreatedOfficeIdPath (length: 500)

        ///<summary>
        /// Ngày khởi tạo
        ///</summary>
        public DateTime CreatedTime { get; set; } // CreatedTime

        ///<summary>
        /// Nhân viên xác thực
        ///</summary>
        public int? ExpertiseUserId { get; set; } // ExpertiseUserId

        ///<summary>
        /// Tên đầy đủ nhân viên xác thực
        ///</summary>
        public string ExpertiseUserFullName { get; set; } // ExpertiseUserFullName (length: 300)

        ///<summary>
        /// UserName của nhân viên xác thực
        ///</summary>
        public string ExpertiseUserUserName { get; set; } // ExpertiseUserUserName (length: 50)

        ///<summary>
        /// Chức vụ nhân viên xác thực
        ///</summary>
        public int? ExpertiseUserTitleId { get; set; } // ExpertiseUserTitleId

        ///<summary>
        /// Tiêu đề của nhân viên xác thực
        ///</summary>
        public string ExpertiseUserTitleName { get; set; } // ExpertiseUserTitleName (length: 300)

        ///<summary>
        /// Id đơn vị của nhân viên xác thực
        ///</summary>
        public int? ExpertiseOfficeId { get; set; } // ExpertiseOfficeId

        ///<summary>
        /// PHòng ban của nhân viên xác thực
        ///</summary>
        public string ExpertiseOfficeName { get; set; } // ExpertiseOfficeName (length: 300)

        ///<summary>
        /// IdPath đơn vị của nhân viên xác thực
        ///</summary>
        public string ExpertiseOfficeIdPath { get; set; } // ExpertiseOfficeIdPath (length: 500)

        ///<summary>
        /// IdPath đơn vị của nhân viên xác thực
        ///</summary>
        public DateTime? ExpertiseTime { get; set; } // ExpertiseTime

        ///<summary>
        /// Id nhân viên shipper chuyển hàng
        ///</summary>
        public int? ShipperUserId { get; set; } // ShipperUserId

        ///<summary>
        /// Tên đầy đủ của nhân viên chuyển hàng
        ///</summary>
        public string ShipperFullName { get; set; } // ShipperFullName (length: 300)

        ///<summary>
        /// UserName của nhân viên vận chuyển
        ///</summary>
        public string ShipperUserUserName { get; set; } // ShipperUserUserName (length: 50)

        ///<summary>
        /// Chức vụ nhân viên vận chuyển
        ///</summary>
        public int? ShipperUserTitleId { get; set; } // ShipperUserTitleId

        ///<summary>
        /// Tiêu đề của nhân viên vận chuyển
        ///</summary>
        public string ShipperUserTitleName { get; set; } // ShipperUserTitleName (length: 300)

        ///<summary>
        /// Id đơn vị của nhân viên vận chuyển
        ///</summary>
        public int? ShipperOfficeId { get; set; } // ShipperOfficeId

        ///<summary>
        /// Tên đơn vị của nhân viên vận chuyển
        ///</summary>
        public string ShipperOfficeName { get; set; } // ShipperOfficeName (length: 300)

        ///<summary>
        /// OfficeIdPaht của nhân viên vận chuyển
        ///</summary>
        public string ShipperOfficeIdPath { get; set; } // ShipperOfficeIdPath (length: 500)

        ///<summary>
        /// OfficeIdPaht của nhân viên vận chuyển
        ///</summary>
        public DateTime? ShipperTime { get; set; } // shipperTime

        ///<summary>
        /// Id thủ kho, nhân viên duyệt phiếu
        ///</summary>
        public int? ApprovelUserId { get; set; } // ApprovelUserId

        ///<summary>
        /// Tên đầy đủ của thủ kho/nhân viên duyệt phiếu
        ///</summary>
        public string ApprovelFullName { get; set; } // ApprovelFullName (length: 300)

        ///<summary>
        /// UserName của thủ kho/ nhân viên duyệt phiếu
        ///</summary>
        public string ApprovelUserUserName { get; set; } // ApprovelUserUserName (length: 50)

        ///<summary>
        /// Chức vụ thủ kho/nhân viên duyệt phiếu
        ///</summary>
        public int? ApprovelUserTitleId { get; set; } // ApprovelUserTitleId

        ///<summary>
        /// Tiêu đề của thủ kho/nhân viên duyệt phiếu
        ///</summary>
        public string ApprovelUserTitleName { get; set; } // ApprovelUserTitleName (length: 300)

        ///<summary>
        /// Id đơn vị của thủ kho/ nhân viên duyệt phiếu
        ///</summary>
        public int? ApprovelOfficeId { get; set; } // ApprovelOfficeId

        ///<summary>
        /// Tên đơn vị của thủ kho/ nhân viên duyệt phiếu
        ///</summary>
        public string ApprovelOfficeName { get; set; } // ApprovelOfficeName (length: 300)

        ///<summary>
        /// OfficeIdPath của thủ kho/nhân viên duyệt phiếu
        ///</summary>
        public string ApprovelOfficeIdPath { get; set; } // ApprovelOfficeIdPath (length: 500)

        ///<summary>
        /// OfficeIdPath của thủ kho/nhân viên duyệt phiếu
        ///</summary>
        public DateTime? ApprovelTime { get; set; } // ApprovelTime

        ///<summary>
        /// Id kế toán
        ///</summary>
        public int? AccountantUserId { get; set; } // AccountantUserId

        ///<summary>
        /// Tên đầy đủ kế toán
        ///</summary>
        public string AccountantFullName { get; set; } // AccountantFullName (length: 300)

        ///<summary>
        /// UserName kế toán
        ///</summary>
        public string AccountantUserUserName { get; set; } // AccountantUserUserName (length: 50)

        ///<summary>
        /// Chức vụ kế toán
        ///</summary>
        public int? AccountantUserTitleId { get; set; } // AccountantUserTitleId

        ///<summary>
        /// Tiêu đề kế toán
        ///</summary>
        public string AccountantUserTitleName { get; set; } // AccountantUserTitleName (length: 300)

        ///<summary>
        /// Id kế toán
        ///</summary>
        public int? AccountantOfficeId { get; set; } // AccountantOfficeId

        ///<summary>
        /// Tên kế toán
        ///</summary>
        public string AccountantOfficeName { get; set; } // AccountantOfficeName (length: 300)

        ///<summary>
        /// OfficeIdPath kế toán
        ///</summary>
        public string AccountantOfficeIdPath { get; set; } // AccountantOfficeIdPath (length: 500)

        ///<summary>
        /// OfficeIdPath kế toán
        ///</summary>
        public DateTime? AccountantTime { get; set; } // AccountantTime
        public string Note { get; set; } // Note

        ///<summary>
        /// Biển số xe giao hàng
        ///</summary>
        public string CarNumber { get; set; } // CarNumber (length: 10)

        ///<summary>
        /// Biển số xe giao hàng
        ///</summary>
        public string CarDescription { get; set; } // CarDescription (length: 10)
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Là phiếu giao cuối cùng
        ///</summary>
        public bool IsLast { get; set; } // IsLast
        public int CustomerId { get; set; } // CustomerId
        public string CustomerCode { get; set; } // CustomerCode (length: 50)
        public string CustomerFullName { get; set; } // CustomerFullName (length: 300)
        public string CustomerEmail { get; set; } // CustomerEmail (length: 50)
        public string CustomerPhone { get; set; } // CustomerPhone (length: 50)
        public string CustomerAddress { get; set; } // CustomerAddress (length: 500)
        public byte CustomerVipId { get; set; } // CustomerVipId
        public string CustomerVipName { get; set; } // CustomerVipName (length: 300)

        ///<summary>
        /// Tổng cân nặng của kiện hàng trong phiếu
        ///</summary>
        public decimal? Weight { get; set; } // Weight

        ///<summary>
        /// Tổng cân nặng chuyển đổi của kiện hàng trong phiếu
        ///</summary>
        public decimal? WeightConverted { get; set; } // WeightConverted

        ///<summary>
        /// Tổng cân nặng tính tiền của phiếu giao hàng
        ///</summary>
        public decimal? WeightActual { get; set; } // WeightActual

        ///<summary>
        /// Cước cân nặng
        ///</summary>
        public decimal? PriceWeight { get; set; } // PriceWeight

        ///<summary>
        /// Tiền đóng kiện gỗ
        ///</summary>
        public decimal? PricePacking { get; set; } // PricePacking

        ///<summary>
        /// Tiền hoàn thành đơn của phiếu xuất
        ///</summary>
        public decimal? PriceOrder { get; set; } // PriceOrder

        ///<summary>
        /// Tiền dịch vụ khách
        ///</summary>
        public decimal? PriceOther { get; set; } // PriceOther

        ///<summary>
        /// Tiền lưu kho
        ///</summary>
        public decimal? PriceStored { get; set; } // PriceStored

        ///<summary>
        /// Tiền Ship
        ///</summary>
        public decimal PriceShip { get; set; } // PriceShip

        ///<summary>
        /// Tổng tiền cần để hoàn thành phiếu giao
        ///</summary>
        public decimal? Total { get; set; } // Total

        ///<summary>
        /// Nợ phiếu xuất
        ///</summary>
        public decimal? Debit { get; set; } // Debit

        ///<summary>
        /// Nợ kỳ trước
        ///</summary>
        public decimal? DebitPre { get; set; } // DebitPre

        ///<summary>
        /// Số tiền phải thu -> Nợ kỳ trước và nợ của phiếu
        ///</summary>
        public decimal? PricePayed { get; set; } // PricePayed

        ///<summary>
        /// Số tiền đã thanh toán (Số tiền thanh toán qua trừ tiền khách hàng)
        ///</summary>
        public decimal? Receivable { get; set; } // Receivable

        ///<summary>
        /// Số dư trước
        ///</summary>
        public decimal? BlanceBefo { get; set; } // BlanceBefo

        ///<summary>
        /// Số dư sau
        ///</summary>
        public decimal? BlanceAfter { get; set; } // BlanceAfter

        /// <summary>
        /// Tổng nợ phiếu xuất của khách -> Nợ trước và nợ hiện tại
        /// </summary>
        public decimal? DebitAfter { get; set; } // DebitAfter

        public Delivery()
        {
            IsDelete = false;
            IsLast = true;
            CustomerFullName = "";
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
