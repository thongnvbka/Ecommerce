using System;

namespace Library.DbContext.Entities
{
    // PackingList
    public partial class PackingList
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã Code PackingList
        ///</summary>
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Trạng thái PackingList (0: Mới khởi tạo, 1: Đã duyệt, 2: Đang vận chuyển, 3: Đã tới đích)
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Tên PackingList
        ///</summary>
        public string PackingListName { get; set; } // PackingListName (length: 300)

        ///<summary>
        /// Hình thức vận chuyển(0: Tiểu nghạch, 1: Chính ngạch)
        ///</summary>
        public byte? TransportType { get; set; } // TransportType

        ///<summary>
        /// Số kiện hàng vận chuyển
        ///</summary>
        public int? PackageNumber { get; set; } // PackageNumber

        ///<summary>
        /// Số bao hàng xuất kho - vận chuyển
        ///</summary>
        public int? WalletNumber { get; set; } // WalletNumber

        ///<summary>
        /// Id gắn với phiếu xuất kho
        ///</summary>
        public int? ExportWarehouseId { get; set; } // ExportWarehouseId

        ///<summary>
        /// Mã Code gắn với phiếu xuất kho
        ///</summary>
        public string ExportWarehouseCode { get; set; } // ExportWarehouseCode (length: 20)

        ///<summary>
        /// Tên kho xuất hàng
        ///</summary>
        public string ExportWarehouseName { get; set; } // ExportWarehouseName (length: 300)

        ///<summary>
        /// Địa chỉ kho xuất hàng
        ///</summary>
        public string ExportWarehouseAddress { get; set; } // ExportWarehouseAddress (length: 300)
        public DateTime? TimeStart { get; set; } // TimeStart
        public DateTime? TimeEnd { get; set; } // TimeEnd

        ///<summary>
        /// Id kho nguồn điều chuyển
        ///</summary>
        public int? WarehouseSourceId { get; set; } // WarehouseSourceId

        ///<summary>
        /// Mã Code kho nguồn điều chuyển
        ///</summary>
        public string WarehouseSourceCode { get; set; } // WarehouseSourceCode (length: 20)

        ///<summary>
        /// Tên kho nguồn điều chuyển
        ///</summary>
        public string WarehouseSourceName { get; set; } // WarehouseSourceName (length: 300)

        ///<summary>
        /// Địa chỉ kho nguồn điều chuyển
        ///</summary>
        public string WarehouseSourceAddress { get; set; } // WarehouseSourceAddress (length: 300)

        ///<summary>
        /// Id kho đích điều chuyển
        ///</summary>
        public int? WarehouseDesId { get; set; } // WarehouseDesId

        ///<summary>
        /// Mã Code kho đích điều chuyển
        ///</summary>
        public string WarehouseDesCode { get; set; } // WarehouseDesCode (length: 20)

        ///<summary>
        /// Tên kho đích điều chuyển
        ///</summary>
        public string WarehouseDesName { get; set; } // WarehouseDesName (length: 300)

        ///<summary>
        /// Địa chỉ kho đích điều chuyển
        ///</summary>
        public string WarehouseDesAddress { get; set; } // WarehouseDesAddress (length: 300)

        ///<summary>
        /// Id nhân viên tạo phiếu
        ///</summary>
        public int UserId { get; set; } // UserId

        ///<summary>
        /// Mã nhân viên tạo PackingList
        ///</summary>
        public string UserCode { get; set; } // UserCode (length: 20)

        ///<summary>
        /// Họ tên nhân viên tạo PackingList
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 300)

        ///<summary>
        /// Id quản lý kho
        ///</summary>
        public int? WarehouseManagerId { get; set; } // WarehouseManagerId

        ///<summary>
        /// Code quản lý kho
        ///</summary>
        public string WarehouseManagerCode { get; set; } // WarehouseManagerCode (length: 20)

        ///<summary>
        /// Họ tên quản lý kho
        ///</summary>
        public string WarehouseManagerFullName { get; set; } // WarehouseManagerFullName (length: 300)

        ///<summary>
        /// Id kế toán viên
        ///</summary>
        public int? WarehouseAccountantId { get; set; } // WarehouseAccountantId

        ///<summary>
        /// Code kế toán viên
        ///</summary>
        public string WarehouseAccountantCode { get; set; } // WarehouseAccountantCode (length: 20)

        ///<summary>
        /// Họ tên kế toán viên
        ///</summary>
        public string WarehouseAccountantFullName { get; set; } // WarehouseAccountantFullName (length: 300)

        ///<summary>
        /// Ngày tạo PackingList
        ///</summary>
        public DateTime? Created { get; set; } // Created

        ///<summary>
        /// Ngày cập nhật PackingList
        ///</summary>
        public DateTime? LastUpdate { get; set; } // LastUpdate

        ///<summary>
        /// Tên người nhận hàng
        ///</summary>
        public string ShipperName { get; set; } // ShipperName (length: 300)

        ///<summary>
        /// Điện thoại người nhận hàng
        ///</summary>
        public string ShipperPhone { get; set; } // ShipperPhone (length: 50)

        ///<summary>
        /// Email người nhận hàng
        ///</summary>
        public string ShipperEmail { get; set; } // ShipperEmail (length: 300)

        ///<summary>
        /// Địa chỉ người nhận hàng
        ///</summary>
        public string ShipperAddress { get; set; } // ShipperAddress (length: 300)

        ///<summary>
        /// Biển số xe đến nhận hàng
        ///</summary>
        public string ShipperLicensePlate { get; set; } // ShipperLicensePlate (length: 100)

        ///<summary>
        /// Ghi chú nhân viên cho PackingList
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        public PackingList()
        {
            Created = DateTime.Now;
            LastUpdate = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
