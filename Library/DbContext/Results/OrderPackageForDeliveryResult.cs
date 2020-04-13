using System;

namespace Library.DbContext.Results
{
    public class OrderPackageForDeliveryResult
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã kiện hàng
        ///</summary>
        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Trạng thái kiện hàng: 0: Chờ nhập kho, 1: Đang trong kho, 2: Đang điều chuyển, 3: Mất mã, 4: Hoàn thành, 5: Mất hàng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Ghi chú kiện hàng
        ///</summary>
        public string Note { get; set; } // Note (length: 600)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 50)

        ///<summary>
        /// Id khách hàng
        ///</summary>
        public int CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerFullName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Tài khoản khách hàng
        ///</summary>
        public string CustomerUserName { get; set; } // CustomerUserName (length: 300)

        public string CustomerPhone { get; set; }

        public string CustomerAddress { get; set; }
        public decimal CustomerDebt { get; set; }
        public decimal CustomerWeight { get; set; }

        ///<summary>
        /// Độ Vip của khách hàng
        ///</summary>
        public byte CustomerLevelId { get; set; } // CustomerLevelId

        ///<summary>
        /// Tên độ Vip của khách hàng
        ///</summary>
        public string CustomerLevelName { get; set; } // CustomerLevelName (length: 300)

        ///<summary>
        /// Kho hàng
        ///</summary>
        public int CustomerWarehouseId { get; set; } // CustomerWarehouseId

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string CustomerWarehouseAddress { get; set; } // CustomerWarehouseAddress (length: 500)

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string CustomerWarehouseName { get; set; } // CustomerWarehouseName (length: 300)
        public string CustomerWarehouseIdPath { get; set; } // CustomerWarehouseIdPath (length: 300)

        ///<summary>
        /// Mã vận đơn của kiện hàng
        ///</summary>
        public string TransportCode { get; set; } // TransportCode (length: 50)

        ///<summary>
        /// Cân nặng kiện hàng
        ///</summary>
        public decimal? Weight { get; set; } // Weight

        ///<summary>
        /// Cân nặng chuyển đổi
        ///</summary>
        public decimal? WeightConverted { get; set; } // WeightConverted
        public decimal? WeightActual { get; set; } // WeightActual

        ///<summary>
        /// Chi tiết kích thước kiện hàng: Rộng x Dài x Cao
        ///</summary>
        public string Size { get; set; } // Size (length: 500)

        ///<summary>
        /// Chiều rộng của kiện hàng
        ///</summary>
        public decimal? Width { get; set; } // Width

        ///<summary>
        /// Chiều cao của kiện hàng
        ///</summary>
        public decimal? Height { get; set; } // Height

        ///<summary>
        /// Chiều dài kiện hàng
        ///</summary>
        public decimal? Length { get; set; } // Length

        ///<summary>
        /// Thành tiền
        ///</summary>
        public decimal? TotalPrice { get; set; } // TotalPrice

        ///<summary>
        /// Kho hàng
        ///</summary>
        public int WarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 300)
        public string WarehouseIdPath { get; set; } // WarehouseIdPath (length: 300)
        public string WarehouseAddress { get; set; } // WarehouseAddress (length: 300)

        ///<summary>
        /// Id nhân viên kho xử lý
        ///</summary>
        public int? UserId { get; set; } // UserId
        public string UserFullName { get; set; } // UserFullName (length: 300)

        ///<summary>
        /// Kiện hàng phát sinh từ hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Tên hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 300)

        ///<summary>
        /// Ngày tạo kiện - Ngày Shop phát hàng
        ///</summary>
        public DateTime Created { get; set; } // Created
        public DateTime LastUpdate { get; set; } // LastUpdate

        ///<summary>
        /// Tag link toàn bộ hệ thống
        ///</summary>
        public string HashTag { get; set; } // HashTag

        ///<summary>
        /// Ngày dự kiến về kho
        ///</summary>
        public DateTime? ForcastDate { get; set; } // ForcastDate

        ///<summary>
        /// Tổng số kiện hàng trong cùng một đơn hàng
        ///</summary>
        public int PackageNo { get; set; } // PackageNo
        public string UnsignedText { get; set; } // UnsignedText (length: 500)

        ///<summary>
        /// Kho hàng
        ///</summary>
        public int? CurrentWarehouseId { get; set; } // CurrentWarehouseId

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string CurrentWarehouseName { get; set; } // CurrentWarehouseName (length: 300)
        public string CurrentWarehouseIdPath { get; set; } // CurrentWarehouseIdPath (length: 300)
        public string CurrentWarehouseAddress { get; set; } // CurrentWarehouseAddress (length: 300)
        public string OrderCodes { get; set; } // OrderCodes (length: 1000)
        public string PackageCodes { get; set; } // PackageCodes (length: 1000)
        public string Customers { get; set; } // Customers (length: 1000)
        public string OrderCodesUnsigned { get; set; } // OrderCodesUnsigned (length: 1000)
        public string PackageCodesUnsigned { get; set; } // PackageCodesUnsigned (length: 1000)
        public string CustomersUnsigned { get; set; } // CustomersUnsigned (length: 1000)
        ///<summary>
        /// Loại packet, 0: đơn order, 1: đơn ký gửi
        ///</summary>
        public byte OrderType { get; set; } // OrderType
        public OrderPackageForDeliveryResult()
        {
            Code = "";
            HashTag = "";
            PackageNo = 1;
        }
    }
}
