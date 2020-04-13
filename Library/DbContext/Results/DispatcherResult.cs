using System;

namespace Library.DbContext.Results
{
    public class DispatcherResult
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã phiếu điều vận
        ///</summary>
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Điều vận từ kho Id
        ///</summary>
        public int? FromWarehouseId { get; set; } // FromWarehouseId

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string FromWarehouseIdPath { get; set; } // FromWarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string FromWarehouseName { get; set; } // FromWarehouseName (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string FromWarehouseAddress { get; set; } // FromWarehouseAddress (length: 500)

        ///<summary>
        /// Trạng thái bao hàng (0: Mới khởi tạo, 1: Đã duyệt, 2: Trong kho, 3: Đang vận chuyển, 4: Mất, 5: Hoàn thành)
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Tổng giá trị tiền hàng trong bao hàng
        ///</summary>
        public decimal? Amount { get; set; } // Amount

        ///<summary>
        /// Tổng cân nặng hàng hóa trong bao hàng
        ///</summary>
        public decimal? TotalWeight { get; set; } // TotalWeight

        ///<summary>
        /// Tổng cân nặng thực tế
        ///</summary>
        public decimal? TotalWeightActual { get; set; } // TotalWeightActual

        ///<summary>
        /// Tổng cân nặng quy đổi
        ///</summary>
        public decimal? TotalWeightConverted { get; set; } // TotalWeightConverted

        ///<summary>
        /// Tổng thể tích của kiện hàng trong bao hàng
        ///</summary>
        public decimal? TotalVolume { get; set; } // TotalVolume

        ///<summary>
        /// Tổng số kiện hàng nằm trong bao
        ///</summary>
        public int TotalPackageNo { get; set; } // TotalPackageNo

        ///<summary>
        /// Tổng số kiện hàng nằm trong bao
        ///</summary>
        public int WalletNo { get; set; } // WalletNo

        ///<summary>
        /// Hình thức tính tiền (Theo khối lượng, Theo thể tích)
        ///</summary>
        public byte PriceType { get; set; } // PriceType

        ///<summary>
        /// Giá vốn / 1kg hoặc 1m3
        ///</summary>
        public decimal Price { get; set; } // Price

        ///<summary>
        /// Giá trị cân nặng hoặc thể tích chốt với nhà vận chuyển
        ///</summary>
        public decimal Value { get; set; } // Value

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
        public string Note { get; set; } // Note (length: 500)
        public string UnsignedText { get; set; } // UnsignedText (length: 500)

        ///<summary>
        /// Ngày dự kiến về kho
        ///</summary>
        public DateTime? ForcastDate { get; set; } // ForcastDate

        ///<summary>
        /// Id kho hàng hiện tại bao đang ở đó
        ///</summary>
        public int? ToWarehouseId { get; set; } // ToWarehouseId

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string ToWarehouseIdPath { get; set; } // ToWarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string ToWarehouseName { get; set; } // ToWarehouseName (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string ToWarehouseAddress { get; set; } // ToWarehouseAddress (length: 500)
        public int TransportPartnerId { get; set; } // TransportPartnerId
        public string TransportPartnerName { get; set; } // TransportPartnerName (length: 300)
        public int TransportMethodId { get; set; } // TransportMethodId
        public string TransportMethodName { get; set; } // TransportMethodName (length: 300)

        ///<summary>
        /// Tên người liên hệ
        ///</summary>
        public string ContactName { get; set; } // ContactName (length: 300)

        ///<summary>
        /// Số điện thoại người liên hệ
        ///</summary>
        public string ContactPhone { get; set; } // ContactPhone (length: 20)

        public int? EntrepotId { get; set; }

        public string EntrepotName { get; set; }

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

        #region DispatcherDetail
        public int? FromDispatcherId { get; set; } // FromDispatcherId
        public string FromDispatcherCode { get; set; } // FromDispatcherCode (length: 50)
        public int? ToDispatcherId { get; set; } // ToDispatcherId
        public string ToDispatcherCode { get; set; } // ToDispatcherCode (length: 50)
        public int? FromTransportPartnerId { get; set; } // FromTransportPartnerId
        public string FromTransportPartnerName { get; set; } // FromTransportPartnerName (length: 300)
        public int? FromTransportMethodId { get; set; } // FromTransportMethodId
        public string FromTransportMethodName { get; set; } // FromTransportMethodName (length: 300)

        ///<summary>
        /// Đích đến điểm trung chuyển
        ///</summary>
        public int? FromEntrepotId { get; set; } // FromEntrepotId

        ///<summary>
        /// Tên đích đến điểm trung  chuyển
        ///</summary>
        public string FromEntrepotName { get; set; } // FromEntrepotName (length: 300)
        public int? ToTransportPartnerId { get; set; } // ToTransportPartnerId
        public string ToTransportPartnerName { get; set; } // ToTransportPartnerName (length: 300)
        public DateTime? ToTransportPartnerTime { get; set; } // ToTransportPartnerTime
        public int? ToTransportMethodId { get; set; } // ToTransportMethodId
        public string ToTransportMethodName { get; set; } // ToTransportMethodName (length: 300)

        ///<summary>
        /// Đích đến điểm trung chuyển
        ///</summary>
        public int? ToEntrepotId { get; set; } // ToEntrepotId

        ///<summary>
        /// Tên đích đến điểm trung  chuyển
        ///</summary>
        public string ToEntrepotName { get; set; } // ToEntrepotName (length: 300)
        #endregion
    }

    public class DispatcherParnerResult
    {
        public int DispatcherId { get; set; } // Id (Primary key)
        public string DispatcherCode { get; set; } // Code (length: 20)
        public string TransportPartnerName { get; set; }
        public int TransportPartnerId { get; set; }
        public int TransportMethodId { get; set; } // TransportMethodId
        public string TransportMethodName { get; set; } // TransportMethodName (length: 300)
        public int? EntrepotId { get; set; } // EntrepotId
        public string EntrepotName { get; set; } // EntrepotName (length: 300)
    }
}
