using System;

namespace Library.DbContext.Results
{
    public class ImportWarehouseWalletResult
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã phiếu nhập kho
        ///</summary>
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Trạng thái nhập kho (0: Mới khởi tạo, 1: Đã duyệt)
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Số kiện
        ///</summary>
        public int? PackageNumber { get; set; } // PackageNumber

        ///<summary>
        /// Số bao
        ///</summary>
        public int? WalletNumber { get; set; } // WalletNumber
        public int WarehouseId { get; set; } // WarehouseId
        public string WarehouseIdPath { get; set; } // WarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho nhập hàng
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 300)
        public string WarehouseAddress { get; set; } // WarehouseAddress (length: 300)
        public string ShipperName { get; set; } // ShipperName (length: 300)
        public string ShipperPhone { get; set; } // ShipperPhone (length: 300)
        public string ShipperAddress { get; set; } // ShipperAddress (length: 300)
        public string ShipperEmail { get; set; } // ShipperEmail (length: 300)
        public string UnsignedText { get; set; } // ShipperEmail (length: 300)

        ///<summary>
        /// Id nhân viên tạo phiếu
        ///</summary>
        public int? UserId { get; set; } // UserId
        public string UserName { get; set; } // UserName (length: 30)

        ///<summary>
        /// Họ tên nhân viên tạo phiếu
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
        public string Note { get; set; } // Note (length: 500)
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated

        #region Bao hàng
        public int WalletId { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã code bao hàng
        ///</summary>
        public string WalletCode { get; set; } // Code (length: 20)

        ///<summary>
        /// Trạng thái bao hàng (0: Mới khởi tạo, 1: Đã duyệt, 2: Trong kho, 3: Đang vận chuyển, 4: Mất, 5: Hoàn thành)
        ///</summary>
        public byte WalletStatus { get; set; } // Status
        public decimal? WalletWidth { get; set; } // Width
        public decimal? WalletLength { get; set; } // Length
        public decimal? WalletHeight { get; set; } // Height
        public string WalletSize { get; set; } // Size (length: 500)

        ///<summary>
        /// Tổng cân nặng các kiện trong bao hàng
        ///</summary>
        public decimal? WalletTotalWeight { get; set; } // TotalWeight

        ///<summary>
        /// Tổng cân nặng chyển đổi các kiện trong bao hàng
        ///</summary>
        public decimal? WalletTotalWeightConverted { get; set; } // TotalWeightConverted

        ///<summary>
        /// Tổng cân nặng tính tiền của kiện hàng trong bao hàng
        ///</summary>
        public decimal? WalletTotalWeightActual { get; set; } // TotalWeightActual

        ///<summary>
        /// Tổng thể tích của kiện hàng trong bao hàng
        ///</summary>
        public decimal? WalletTotalVolume { get; set; } // TotalVolume

        ///<summary>
        /// Cân nặng của bao hàng
        ///</summary>
        public decimal? WalletWeight { get; set; } // Weight

        ///<summary>
        /// Cân nặng chuyển đổi của bao hàng
        ///</summary>
        public decimal? WalletWeightConverted { get; set; } // WeightConverted

        ///<summary>
        /// Cân nặng thực tế tính tiền của bao hàng
        ///</summary>
        public decimal? WalletWeightActual { get; set; } // WeightActual

        ///<summary>
        /// Thể tích của bao hàng
        ///</summary>
        public decimal? WalletVolume { get; set; } // Volume

        ///<summary>
        /// Tổng giá trị tiền hàng trong bao hàng
        ///</summary>
        public decimal? WalletTotalValue { get; set; } // TotalValue

        ///<summary>
        /// Tổng số kiện hàng nằm trong bao
        ///</summary>
        public int WalletPackageNo { get; set; } // PackageNo

        ///<summary>
        /// Id Kho hàng tạo bao
        ///</summary>
        public int WalletCreatedWarehouseId { get; set; } // CreatedWarehouseId

        ///<summary>
        /// Id Kho hàng tạo bao
        ///</summary>
        public string WalletCreatedWarehouseIdPath { get; set; } // CreatedWarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho hàng đã tạo bao
        ///</summary>
        public string WalletCreatedWarehouseName { get; set; } // CreatedWarehouseName (length: 300)

        ///<summary>
        /// Tên kho hàng đã tạo bao
        ///</summary>
        public string WalletCreatedWarehouseAddress { get; set; } // CreatedWarehouseAddress (length: 500)

        ///<summary>
        /// Id kho hàng hiện tại bao đang ở đó
        ///</summary>
        public int? WalletCurrentWarehouseId { get; set; } // CurrentWarehouseId

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string WalletCurrentWarehouseIdPath { get; set; } // CurrentWarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string WalletCurrentWarehouseName { get; set; } // CurrentWarehouseName (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string WalletCurrentWarehouseAddress { get; set; } // CurrentWarehouseAddress (length: 500)

        ///<summary>
        /// Id kho hàng hiện tại bao đang ở đó
        ///</summary>
        public int? WalletTargetWarehouseId { get; set; } // TargetWarehouseId

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string WalletTargetWarehouseIdPath { get; set; } // TargetWarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string WalletTargetWarehouseName { get; set; } // TargetWarehouseName (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string WalletTargetWarehouseAddress { get; set; } // TargetWarehouseAddress (length: 500)

        ///<summary>
        /// Id Nhân viên tạo phiếu nhập bao
        ///</summary>
        public int WalletUserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên thực hiện tạo bao
        ///</summary>
        public string WalletUserName { get; set; } // UserName (length: 50)

        ///<summary>
        /// Mã Code nhân viên thực hiện tạo phiếu
        ///</summary>
        public string WalletUserFullName { get; set; } // UserFullName (length: 300)

        ///<summary>
        /// Ngày bao được tạo
        ///</summary>
        public DateTime WalletCreated { get; set; } // Created

        ///<summary>
        /// Thời gian sửa - Cập nhật bao hàng
        ///</summary>
        public DateTime WalletUpdated { get; set; } // Updated
        public string WalletUnsignedText { get; set; } // UnsignedText (length: 500)
        public string WalletNote { get; set; } // Note (length: 500)
        public string WalletOrderCodes { get; set; } // OrderCodes (length: 1000)
        public string WalletPackageCodes { get; set; } // PackageCodes (length: 1000)
        public string WalletCustomers { get; set; } // Customers (length: 1000)
        public string WalletOrderCodesUnsigned { get; set; } // OrderCodesUnsigned (length: 1000)
        public string WalletPackageCodesUnsigned { get; set; } // PackageCodesUnsigned (length: 1000)
        public string WalletCustomersUnsigned { get; set; } // CustomersUnsigned (length: 1000)

        ///<summary>
        /// 0: Bao, 1: Kiện gỗ
        ///</summary>
        public byte WalletMode { get; set; } // Mode

        ///<summary>
        /// Id đối tác vận chuyển
        ///</summary>
        public int? WalletPartnerId { get; set; } // PartnerId

        ///<summary>
        /// Tên đối tác vận chuyển
        ///</summary>
        public string WalletPartnerName { get; set; } // PartnerName (length: 300)

        ///<summary>
        /// Thời gian cập nhật đối tác vận chuyển
        ///</summary>
        public DateTime? WalletPartnerUpdate { get; set; } // PartnerUpdate
        #endregion
    }
}
