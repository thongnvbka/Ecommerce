using System;

namespace Library.DbContext.Results
{
    public class AcountingLoseResult
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Loại đơn hàng: Ký gửi, Order,..
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// Nguồn Order từ website
        ///</summary>
        public string WebsiteName { get; set; } // WebsiteName (length: 300)

        ///<summary>
        /// Id Shop bán hàng
        ///</summary>
        public int? ShopId { get; set; } // ShopId

        ///<summary>
        /// Tên Shop bán hàng
        ///</summary>
        public string ShopName { get; set; } // ShopName (length: 500)

        ///<summary>
        /// Link của shop bán hàng
        ///</summary>
        public string ShopLink { get; set; } // ShopLink

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int ProductNo { get; set; } // ProductNo

        ///<summary>
        /// Số lượng kiện hàng
        ///</summary>
        public int PackageNo { get; set; } // PackageNo

        ///<summary>
        /// Mỡ hợp đồng: mã đơn hàng sau khi đặt hàng
        ///</summary>
        public string ContractCode { get; set; } // ContractCode (length: 50)

        ///<summary>
        /// Mã hợp đồng liên quan khi trùng mã vận đơn
        ///</summary>
        public string ContractCodes { get; set; } // ContractCodes (length: 300)

        ///<summary>
        /// Id cấp độ khách hàng
        ///</summary>
        public byte LevelId { get; set; } // LevelId

        ///<summary>
        /// Tên cấp độ khách hàng
        ///</summary>
        public string LevelName { get; set; } // LevelName (length: 300)

        ///<summary>
        /// Tổng số cân nặng của kiện hàng
        ///</summary>
        public decimal TotalWeight { get; set; } // TotalWeight

        ///<summary>
        /// Loại hình giảm giá: 0: %, 1: Tiền
        ///</summary>
        public byte DiscountType { get; set; } // DiscountType

        ///<summary>
        /// Giá trị giảm giá: Tiền hoặc %
        ///</summary>
        public decimal? DiscountValue { get; set; } // DiscountValue

        ///<summary>
        /// Mã khuyến mãi
        ///</summary>
        public string GiftCode { get; set; } // GiftCode (length: 30)

        ///<summary>
        /// Đơn hàng được tạo bằng: Extension, Excell, Nhân viên tạo,..
        ///</summary>
        public byte CreatedTool { get; set; } // CreatedTool

        ///<summary>
        /// Đơn vị tiền tệ: VND, USD,...
        ///</summary>
        public string Currency { get; set; } // Currency (length: 50)

        ///<summary>
        /// Tỉ giá ngoại tệ
        ///</summary>
        public decimal ExchangeRate { get; set; } // ExchangeRate

        ///<summary>
        /// Tổng tiền VND
        ///</summary>
        public decimal TotalExchange { get; set; } // TotalExchange

        ///<summary>
        /// Tổng tiền ngoại tệ
        ///</summary>
        public decimal TotalPrice { get; set; } // TotalPrice

        ///<summary>
        /// Tổng giá trị tiền + tiền dịch vụ và sau khi giảm giá VND
        ///</summary>
        public decimal Total { get; set; } // Total

        ///<summary>
        /// Tag link toàn bộ hệ thống
        ///</summary>
        public string HashTag { get; set; } // HashTag

        ///<summary>
        /// Id kho đến khách hàng chọn
        ///</summary>
        public int WarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Tên kho đến khách hàng chọn
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 500)

        ///<summary>
        /// Id khách hàng
        ///</summary>
        public int? CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Email khách hàng
        ///</summary>
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)

        ///<summary>
        /// Điện thoại khách hàng
        ///</summary>
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)
        public string CustomerAddress { get; set; } // CustomerPhone (length: 500)

        ///<summary>
        /// Trạng thái đơn hàng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Id nhân viên xử lý
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên xử lý
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 150)

        ///<summary>
        /// Đơn vị nhân viên tạo
        ///</summary>
        public int? OfficeId { get; set; } // OfficeId

        ///<summary>
        /// Tên đơn vị nhân viên xử lý
        ///</summary>
        public string OfficeName { get; set; } // OfficeName (length: 300)

        ///<summary>
        /// Id Path đơn vị nhân viên
        ///</summary>
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 300)

        ///<summary>
        /// Id Path đơn vị nhân viên
        ///</summary>
        public string CreatedOfficeIdPath { get; set; } // CreatedOfficeIdPath (length: 300)

        ///<summary>
        /// Id nhân viên xử lý
        ///</summary>
        public int? CreatedUserId { get; set; } // CreatedUserId

        ///<summary>
        /// Tên nhân viên xử lý
        ///</summary>
        public string CreatedUserFullName { get; set; } // CreatedUserFullName (length: 150)

        ///<summary>
        /// Đơn vị nhân viên tạo
        ///</summary>
        public int? CreatedOfficeId { get; set; } // CreatedOfficeId

        ///<summary>
        /// Tên đơn vị nhân viên xử lý
        ///</summary>
        public string CreatedOfficeName { get; set; } // CreatedOfficeName (length: 300)

        ///<summary>
        /// Khác null là yêu cầu đi công và cần nhập thêm thông tin về để có thể đi công
        ///</summary>
        public int OrderInfoId { get; set; } // OrderInfoId

        ///<summary>
        /// Id thông tin địa chỉ người đặt
        ///</summary>
        public int FromAddressId { get; set; } // FromAddressId

        ///<summary>
        /// Id thông tin địa chỉ người nhận
        ///</summary>
        public int ToAddressId { get; set; } // ToAddressId

        ///<summary>
        /// Id hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Đơn hàng phát sinh từ hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 100)

        ///<summary>
        /// Loại dịch vụ: 0: Gói kinh doanh, 1: Gói tiêu dùng
        ///</summary>
        public byte ServiceType { get; set; } // ServiceType

        ///<summary>
        /// Ghi chú khách viết cho công ty
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// Ghi chú riêng tư của khách
        ///</summary>
        public string PrivateNote { get; set; } // PrivateNote (length: 500)

        ///<summary>
        /// Tổng số link sản phẩm trong order
        ///</summary>
        public int LinkNo { get; set; } // LinkNo

        ///<summary>
        /// Thời gian tạo đơn hàng
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian cập nhật gần đây
        ///</summary>
        public DateTime LastUpdate { get; set; } // LastUpdate
        public DateTime? ExpectedDate { get; set; } // ExpectedDate
        ///<summary>
        /// Phí mua hàng
        ///</summary>
        public decimal? TotalPurchase { get; set; } // TotalPurchase
        ///<summary>
        /// Tiền khách đặt cọc
        ///</summary>
        public decimal? TotalAdvance { get; set; } // TotalAdvance
        /// <summary>
        /// Lý do hủy đơn hàng
        /// </summary>
        public string ReasonCancel { get; set; }
        /// <summary>
        /// Tiền mặc cả với shop (NDT) /tổng đơn hàng
        /// </summary>
        public decimal? PriceBargain { get; set; }
        /// <summary>
        /// Tiền hàng công ty thanh toán cho shop
        /// </summary>
        public decimal? PaidShop { get; set; }
        /// <summary>
        /// Phí ship nội địa trung quốc
        /// </summary>
        public decimal? FeeShip { get; set; }
        /// <summary>
        /// Phí ship nội địa trung quốc mặc cả được
        /// </summary>
        public decimal? FeeShipBargain { get; set; }
        /// <summary>
        /// Kho trả tiền ship
        /// </summary>
        public bool IsPayWarehouseShip { get; set; }
        ///<summary>
        /// Nhân viên ghi chú
        ///</summary>
        public string UserNote { get; set; } // UserNote (length: 500)

        ///<summary>
        /// Số lượng kiện hàng đã về kho
        ///</summary>
        public int? PackageNoInStock { get; set; } // PackageNoInStock

        ///<summary>
        /// Phục vụ tìm kiếm
        ///</summary>
        public string UnsignName { get; set; } // UnsignName (length: 500)
        ///<summary>
        /// Số kiện
        ///</summary>
        public int? PacketNumber { get; set; } // PacketNumber

        ///<summary>
        /// Ghi chú đơn ký gửi
        ///</summary>
        public string Description { get; set; } // Description (length: 1000)
        ///<summary>
        /// Tổng tiền tạm tính
        ///</summary>
        public decimal? ProvisionalMoney { get; set; } // ProvisionalMoney

        ///<summary>
        /// Loại hàng. 0: không pin, 1: có pin
        ///</summary>
        public int? DepositType { get; set; } // DepositType

        ///<summary>
        /// Mã kho giao hàng
        ///</summary>
        public int? WarehouseDeliveryId { get; set; } // WarehouseDeliveryId

        ///<summary>
        /// Tên kho giao hàng
        ///</summary>
        public string WarehouseDeliveryName { get; set; } // WarehouseDeliveryName (length: 500)

        ///<summary>
        /// Đơn vị tính duyệt
        ///</summary>
        public string ApprovelUnit { get; set; } // ApprovelUnit (length: 50)

        ///<summary>
        /// Giá duyệt
        ///</summary>
        public decimal? ApprovelPrice { get; set; } // ApprovelPrice

        ///<summary>
        /// Tên liên lạc lấy hàng bên TQ
        ///</summary>
        public string ContactName { get; set; } // ContactName (length: 100)

        ///<summary>
        /// Điện thoại người liên lạc bên TQ
        ///</summary>
        public string ContactPhone { get; set; } // ContactPhone (length: 20)

        ///<summary>
        /// Địa chỉ người liên lạc bên TQ
        ///</summary>
        public string ContactAddress { get; set; } // ContactAddress (length: 255)

        ///<summary>
        /// Điện thoại người nhận
        ///</summary>
        public string ContactEmail { get; set; } // ContactEmail (length: 300)

        ///<summary>
        /// Id của nhân viên chăm sóc khách hàng
        ///</summary>
        public int? CustomerCareUserId { get; set; } // CustomerCareUserId

        ///<summary>
        /// Họ tên nhân viên chăm sóc khách hàng
        ///</summary>
        public string CustomerCareFullName { get; set; } // CustomerCareFullName (length: 150)

        ///<summary>
        /// Id phòng ban của nhân viên chăm sóc khách hàng
        ///</summary>
        public int? CustomerCareOfficeId { get; set; } // CustomerCareOfficeId

        ///<summary>
        /// Tên phòng ban của nhân viên chăm sóc khách hàng
        ///</summary>
        public string CustomerCareOfficeName { get; set; } // CustomerCareOfficeName (length: 300)

        ///<summary>
        /// Đường dẫn của phòng ban
        ///</summary>
        public string CustomerCareOfficeIdPath { get; set; } // CustomerCareOfficeIdPath (length: 300)

        /// <summary>
        /// Số lượng sai kiểm đếm trong đơn hàng
        /// </summary>
        public int? QuantityLose { get; set; }

        /// <summary>
        /// Trị giá hàng sai kiểm đếm
        /// </summary>
        public decimal TotalPriceLose { get; set; }

        public int CommentNo { get; set; }
        public int RequestNo { get; set; }
        public byte RequestStatus { get; set; }

        public string NoteProcess { get; set; }
    }
}
