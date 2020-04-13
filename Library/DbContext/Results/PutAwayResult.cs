using System;

namespace Library.DbContext.Results
{
    public class PutAwayResult
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã code bao hàng
        ///</summary>
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Trạng thái bao hàng (0: Mới khởi tạo, 1: Đã duyệt, 2: Trong kho, 3: Đang vận chuyển, 4: Mất, 5: Hoàn thành)
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Tổng cân nặng hàng hóa trong bao hàng
        ///</summary>
        public decimal? TotalWeight { get; set; } // TotalWeight

        ///<summary>
        /// Tổng cân nặng thực tế
        ///</summary>
        public decimal? TotalActualWeight { get; set; } // TotalActualWeight

        ///<summary>
        /// Tổng cân nặng quy đổi
        ///</summary>
        public decimal? TotalConversionWeight { get; set; } // TotalConversionWeight

        ///<summary>
        /// Tổng số kiện hàng nằm trong bao
        ///</summary>
        public int PackageNo { get; set; } // PackageNo

        ///<summary>
        /// Id Kho hàng tạo bao
        ///</summary>
        public int WarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Id Kho hàng tạo bao
        ///</summary>
        public string WarehouseIdPath { get; set; } // WarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho hàng đã tạo bao
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 300)

        ///<summary>
        /// Tên kho hàng đã tạo bao
        ///</summary>
        public string WarehouseAddress { get; set; } // WarehouseAddress (length: 500)

        ///<summary>
        /// Id Nhân viên tạo phiếu nhập bao
        ///</summary>
        public int UserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên thực hiện tạo bao
        ///</summary>
        public string UserName { get; set; } // UserName (length: 50)

        ///<summary>
        /// Mã Code nhân viên thực hiện tạo phiếu
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 300)

        ///<summary>
        /// Ngày bao được tạo
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian sửa - Cập nhật bao hàng
        ///</summary>
        public DateTime Updated { get; set; } // Updated
        public string UnsignedText { get; set; } // UnsignedText (length: 500)
        public string Note { get; set; } // Note (length: 500)

        #region Kien hang

        public int PackageId { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã kiện hàng
        ///</summary>
        public string PackageCode { get; set; } // Code (length: 30)

        ///<summary>
        /// Trạng thái kiện hàng: 0: Chờ nhập kho, 1: Đang trong kho, 2: Đang điều chuyển, 3: Mất mã, 4: Hoàn thành, 5: Mất hàng
        ///</summary>
        public byte PackageStatus { get; set; } // Status

        ///<summary>
        /// Ghi chú kiện hàng
        ///</summary>
        public string PackageNote { get; set; } // Note (length: 600)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public int PackageOrderId { get; set; } // OrderId

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public string PackageOrderCode { get; set; } // OrderCode (length: 50)

        ///<summary>
        /// Ghi chú các dịch vụ trong đơn hàng: (Kiểm đếm, Đóng kiện,..)
        ///</summary>
        public string PackageOrderServices { get; set; } // OrderServices (length: 500)

        ///<summary>
        /// Id khách hàng
        ///</summary>
        public int PackageCustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string PackageCustomerName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Tài khoản khách hàng
        ///</summary>
        public string PackageCustomerUserName { get; set; } // CustomerUserName (length: 300)

        ///<summary>
        /// Độ Vip của khách hàng
        ///</summary>
        public byte PackageCustomerLevelId { get; set; } // CustomerLevelId

        ///<summary>
        /// Tên độ Vip của khách hàng
        ///</summary>
        public string PackageCustomerLevelName { get; set; } // CustomerLevelName (length: 300)

        ///<summary>
        /// Kho hàng
        ///</summary>
        public int PackageCustomerWarehouseId { get; set; } // CustomerWarehouseId

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string PackageCustomerWarehouseAddress { get; set; } // CustomerWarehouseAddress (length: 500)

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string PackageCustomerWarehouseName { get; set; } // CustomerWarehouseName (length: 300)
        public string PackageCustomerWarehouseIdPath { get; set; } // CustomerWarehouseIdPath (length: 300)

        ///<summary>
        /// Mã vận đơn của kiện hàng
        ///</summary>
        public string PackageTransportCode { get; set; } // TransportCode (length: 50)

        ///<summary>
        /// Cân nặng kiện hàng
        ///</summary>
        public decimal? PackageWeight { get; set; } // Weight

        ///<summary>
        /// Cân nặng chuyển đổi
        ///</summary>
        public decimal? PackageWeightConverted { get; set; } // WeightConverted
        public decimal? PackageWeightActual { get; set; } // WeightActual

        ///<summary>
        /// % Cân nặng của kiện trong tổng cân nặng của kiện gỗ
        ///</summary>
        public decimal? PackageWeightWapperPercent { get; set; } // WeightWapperPercent

        ///<summary>
        /// Cân nặng bì gỗ khi kiện đóng kiện gỗ
        ///</summary>
        public decimal? PackageWeightWapper { get; set; } // WeightWapper

        ///<summary>
        /// Tiền đóng kiện gỗ của kiện hàng
        ///</summary>
        public decimal? PackageTotalPriceWapper { get; set; } // TotalPriceWapper

        ///<summary>
        /// Thể tích m3 của kiện hàng
        ///</summary>
        public decimal? PackageVolume { get; set; } // Volume

        ///<summary>
        /// Thể tích m3 tính tiền của kiện hàng
        ///</summary>
        public decimal? PackageVolumeActual { get; set; } // VolumeActual

        ///<summary>
        /// % thể tích trong bao m3 của kiện hàng
        ///</summary>
        public decimal? PackageVolumeWapperPercent { get; set; } // VolumeWapperPercent

        ///<summary>
        /// Thể tích chệnh lệch m3 của kiện hàng
        ///</summary>
        public decimal? PackageVolumeWapper { get; set; } // VolumeWapper

        ///<summary>
        /// Chi tiết kích thước kiện hàng: Rộng x Dài x Cao
        ///</summary>
        public string PackageSize { get; set; } // Size (length: 500)

        ///<summary>
        /// Chiều rộng của kiện hàng
        ///</summary>
        public decimal? PackageWidth { get; set; } // Width

        ///<summary>
        /// Chiều cao của kiện hàng
        ///</summary>
        public decimal? PackageHeight { get; set; } // Height

        ///<summary>
        /// Chiều dài kiện hàng
        ///</summary>
        public decimal? PackageLength { get; set; } // Length

        ///<summary>
        /// Thành tiền
        ///</summary>
        public decimal? PackageTotalPrice { get; set; } // TotalPrice

        ///<summary>
        /// Kho hàng
        ///</summary>
        public int PackageWarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string PackageWarehouseName { get; set; } // WarehouseName (length: 300)
        public string PackageWarehouseIdPath { get; set; } // WarehouseIdPath (length: 300)
        public string PackageWarehouseAddress { get; set; } // WarehouseAddress (length: 300)

        ///<summary>
        /// Id nhân viên kho xử lý
        ///</summary>
        public int? PackageUserId { get; set; } // UserId
        public string PackageUserFullName { get; set; } // UserFullName (length: 300)

        ///<summary>
        /// Kiện hàng phát sinh từ hệ thống
        ///</summary>
        public int PackageSystemId { get; set; } // SystemId

        ///<summary>
        /// Tên hệ thống
        ///</summary>
        public string PackageSystemName { get; set; } // SystemName (length: 300)

        ///<summary>
        /// Ngày tạo kiện - Ngày Shop phát hàng
        ///</summary>
        public DateTime PackageCreated { get; set; } // Created
        public DateTime PackageLastUpdate { get; set; } // LastUpdate

        ///<summary>
        /// Tag link toàn bộ hệ thống
        ///</summary>
        public string PackageHashTag { get; set; } // HashTag

        ///<summary>
        /// Ngày dự kiến về kho
        ///</summary>
        public DateTime? PackageForcastDate { get; set; } // ForcastDate

        ///<summary>
        /// Tổng số kiện hàng trong cùng một đơn hàng
        ///</summary>
        public int PackagePackageNo { get; set; } // PackageNo
        public string PackageUnsignedText { get; set; } // UnsignedText (length: 500)

        ///<summary>
        /// Kho hàng
        ///</summary>
        public int? PackageCurrentLayoutId { get; set; } // CurrentLayoutId

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string PackageCurrentLayoutName { get; set; } // CurrentLayoutName (length: 300)
        public string PackageCurrentLayoutIdPath { get; set; } // CurrentLayoutIdPath (length: 300)

        ///<summary>
        /// Kho hàng
        ///</summary>
        public int? PackageCurrentWarehouseId { get; set; } // CurrentWarehouseId

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string PackageCurrentWarehouseName { get; set; } // CurrentWarehouseName (length: 300)
        public string PackageCurrentWarehouseIdPath { get; set; } // CurrentWarehouseIdPath (length: 300)
        public string PackageCurrentWarehouseAddress { get; set; } // CurrentWarehouseAddress (length: 300)
        public bool PackageIsDelete { get; set; } // IsDelete
        public string PackageOrderCodes { get; set; } // OrderCodes (length: 1000)
        public string PackagePackageCodes { get; set; } // PackageCodes (length: 1000)
        public string PackageCustomers { get; set; } // Customers (length: 1000)
        public string PackageOrderCodesUnsigned { get; set; } // OrderCodesUnsigned (length: 1000)
        public string PackagePackageCodesUnsigned { get; set; } // PackageCodesUnsigned (length: 1000)
        public string PackageCustomersUnsigned { get; set; } // CustomersUnsigned (length: 1000)

        ///<summary>
        /// Loại packet, 0: đơn order, 1: đơn ký gửi
        ///</summary>
        public byte PackageOrderType { get; set; } // OrderType

        ///<summary>
        /// Danh sách hình ảnh bằng json
        ///</summary>
        public string PackageImageJson { get; set; } // ImageJson

        #endregion
    }
}
