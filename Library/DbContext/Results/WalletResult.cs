using System;

namespace Library.DbContext.Results
{
    public class WalletResult
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

        public decimal? Width { get; set; } // Width
        public decimal? Length { get; set; } // Length
        public decimal? Height { get; set; } // Height
        public string Size { get; set; } // Size (length: 500)

        ///<summary>
        /// Tổng cân nặng các kiện trong bao hàng
        ///</summary>
        public decimal? TotalWeight { get; set; } // TotalWeight

        ///<summary>
        /// Tổng cân nặng chyển đổi các kiện trong bao hàng
        ///</summary>
        public decimal? TotalWeightConverted { get; set; } // TotalWeightConverted

        ///<summary>
        /// Tổng cân nặng tính tiền của kiện hàng trong bao hàng
        ///</summary>
        public decimal? TotalWeightActual { get; set; } // TotalWeightActual

        ///<summary>
        /// Tổng thể tích của kiện hàng trong bao hàng
        ///</summary>
        public decimal? TotalVolume { get; set; } // TotalVolume

        ///<summary>
        /// Cân nặng của bao hàng
        ///</summary>
        public decimal? Weight { get; set; } // Weight

        ///<summary>
        /// Cân nặng chuyển đổi của bao hàng
        ///</summary>
        public decimal? WeightConverted { get; set; } // WeightConverted

        ///<summary>
        /// Cân nặng thực tế tính tiền của bao hàng
        ///</summary>
        public decimal? WeightActual { get; set; } // WeightActual

        ///<summary>
        /// Thể tích của bao hàng
        ///</summary>
        public decimal? Volume { get; set; } // Volume

        ///<summary>
        /// Tổng giá trị tiền hàng trong bao hàng
        ///</summary>
        public decimal? TotalValue { get; set; } // TotalValue

        ///<summary>
        /// Tổng số kiện hàng nằm trong bao
        ///</summary>
        public int PackageNo { get; set; } // PackageNo

        ///<summary>
        /// Id Kho hàng tạo bao
        ///</summary>
        public int CreatedWarehouseId { get; set; } // CreatedWarehouseId

        ///<summary>
        /// Id Kho hàng tạo bao
        ///</summary>
        public string CreatedWarehouseIdPath { get; set; } // CreatedWarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho hàng đã tạo bao
        ///</summary>
        public string CreatedWarehouseName { get; set; } // CreatedWarehouseName (length: 300)

        ///<summary>
        /// Tên kho hàng đã tạo bao
        ///</summary>
        public string CreatedWarehouseAddress { get; set; } // CreatedWarehouseAddress (length: 500)

        ///<summary>
        /// Id kho hàng hiện tại bao đang ở đó
        ///</summary>
        public int? CurrentWarehouseId { get; set; } // CurrentWarehouseId

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string CurrentWarehouseIdPath { get; set; } // CurrentWarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string CurrentWarehouseName { get; set; } // CurrentWarehouseName (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string CurrentWarehouseAddress { get; set; } // CurrentWarehouseAddress (length: 500)

        ///<summary>
        /// Id kho hàng hiện tại bao đang ở đó
        ///</summary>
        public int? TargetWarehouseId { get; set; } // TargetWarehouseId

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string TargetWarehouseIdPath { get; set; } // TargetWarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string TargetWarehouseName { get; set; } // TargetWarehouseName (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string TargetWarehouseAddress { get; set; } // TargetWarehouseAddress (length: 500)

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
        public bool IsDelete { get; set; } // IsDelete
        public string OrderCodes { get; set; } // OrderCodes (length: 1000)
        public string PackageCodes { get; set; } // PackageCodes (length: 1000)
        public string Customers { get; set; } // Customers (length: 1000)
        public string OrderCodesUnsigned { get; set; } // OrderCodesUnsigned (length: 1000)
        public string PackageCodesUnsigned { get; set; } // PackageCodesUnsigned (length: 1000)
        public string CustomersUnsigned { get; set; } // CustomersUnsigned (length: 1000)

        ///<summary>
        /// 0: Bao, 1: Kiện gỗ
        ///</summary>
        public byte Mode { get; set; } // Mode

        ///<summary>
        /// Id đối tác vận chuyển
        ///</summary>
        public int? PartnerId { get; set; } // PartnerId

        ///<summary>
        /// Tên đối tác vận chuyển
        ///</summary>
        public string PartnerName { get; set; } // PartnerName (length: 300)

        ///<summary>
        /// Thời gian cập nhật đối tác vận chuyển
        ///</summary>
        public DateTime? PartnerUpdate { get; set; } // PartnerUpdate

        public int? EntrepotId { get; set; }

        public string EntrepotName { get; set; }

        public string OrderServices { get; set; }
        public string OrderServicesJson { get; set; }

        public DateTime? ImportedTime { get; set; }
    }
}