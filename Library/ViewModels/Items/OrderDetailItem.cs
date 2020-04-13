using System;

namespace Library.ViewModels.Items
{
    public class OrderPackageWalletItem
    {
        public byte Status { get; set; } // Status (Primary key)

        ///<summary>
        /// Mã kiện hàng
        ///</summary>
        public string PackageCode { get; set; } // packageCode (length: 30)
    }

    public class OrderPackageItem
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã kiện hàng
        ///</summary>
        public string Code { get; set; } // Code (length: 30)

                                         ///<summary>
                                         /// Cân nặng kiện hàng
                                         ///</summary>
        public decimal? Weight { get; set; } // Weight

        public decimal? WeightActual { get; set; } // WeightActual
        public decimal? Length { get; set; } // Length
        public decimal? Height { get; set; } // Height
        public decimal? Width { get; set; } // Width

        ///<summary>
        /// Mã vận đơn của kiện hàng
        ///</summary>
        public string TransportCode { get; set; } // TransportCode (length: 50)

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string CurrentWarehouseName { get; set; } // CurrentWarehouseName (length: 300)

        ///<summary>
        /// Trạng thái kiện hàng: 0: Chờ nhập kho, 1: Đang trong kho, 2: Đang điều chuyển, 3: Mất mã, 4: Hoàn thành, 5: Mất hàng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Ghi chú kiện hàng
        ///</summary>
        public string Note { get; set; } // Note (length: 600)

        ///<summary>
        /// Thành tiền
        ///</summary>
        public decimal? TotalPrice { get; set; } // TotalPrice

        public int IsGross { get; set; }

        ///<summary>
        /// Ngày tạo kiện - Ngày Shop phát hàng
        ///</summary>
        public DateTime CreateDate { get; set; } // Created

        public decimal? ActualMoney { get; set; } // ActualMoney
    }

    public class ProductDetailItem
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Tên sản phẩm
        ///</summary>
        public string Name { get; set; } // Name (length: 600)

        ///<summary>
        /// Ảnh
        ///</summary>
        public string Image { get; set; } // Image

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int Quantity { get; set; } // Quantity

        public int QuantityBooked { get; set; } // QuantityBooked
        public int QuantityActuallyReceived { get; set; } // QuantityActuallyReceived

        ///<summary>
        /// Tỷ giá ngoại tệ
        ///</summary>
        public decimal ExchangeRate { get; set; } // ExchangeRate

        ///<summary>
        /// Thành tiền ngoại tệ số lượng * Giá (Ngoại tệ)
        ///</summary>
        public decimal TotalPrice { get; set; } // TotalPrice

        ///<summary>
        /// Thành tiền sau khi chuyển đổi VNĐ
        ///</summary>
        public decimal TotalExchange { get; set; } // TotalExchange

        ///<summary>
        /// Link sản phẩm
        ///</summary>
        public string Link { get; set; } // Link

        ///<summary>
        /// Thời gian tạo link sản phẩm
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian cập nhật gần đây
        ///</summary>
        public DateTime LastUpdate { get; set; } // LastUpdate

        ///<summary>
        /// Kích thước sản phẩm
        ///</summary>
        public string Size { get; set; } // Size (length: 50)

        ///<summary>
        /// Màu sắc sản phẩm
        ///</summary>
        public string Color { get; set; } // Color  (length: 50)

        ///<summary>
        /// Ghi chú sản phẩm
        ///</summary>
        public string Note { get; set; } // Note  (length: 500)

        ///<summary>
        /// Giá ngoại tệ
        ///</summary>
        public decimal Price { get; set; } // Price

        public string PrivateNote { get; set; } // PrivateNote (length: 2000)
        public string ComplainNote { get; set; } // ComplainNote (length: 2000)
        public string Properties { get; set; }
        public int LinkOrder { get; set; }
    }

    public class OrderServiceItem
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id dịch vụ
        ///</summary>
        public int ServiceId { get; set; } // ServiceId

        ///<summary>
        /// Tên dịch vụ: Kiểm đếm, đóng kiện,...
        ///</summary>
        public string ServiceName { get; set; } // ServiceName (length: 300)

        ///<summary>
        /// Thành tiền VND
        ///</summary>
        public decimal TotalPrice { get; set; } // TotalPrice

        public bool Checked { get; set; } // Checked

        ///<summary>
        /// Trạn thái bị xóa hay không
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete
    }

    public class OrderDetailItem
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int ProductNo { get; set; } // ProductNo

        ///<summary>
        /// Số lượng kiện hàng
        ///</summary>
        public int PackageNo { get; set; } // PackageNo

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
        /// Trạng thái đơn hàng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Loại dịch vụ: 0: Gói kinh doanh, 1: Gói tiêu dùng
        ///</summary>
        public byte ServiceType { get; set; } // ServiceType

        ///<summary>
        /// Ghi chú khách viết cho công ty
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// Thời gian tạo đơn hàng
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian cập nhật gần đây
        ///</summary>
        public DateTime LastUpdate { get; set; } // LastUpdate

        public DateTime ExpectedDate { get; set; }

        ///<summary>
        /// Phí mua hàng
        ///</summary>
        public decimal TotalPurchase { get; set; } // TotalPurchase

        ///<summary>
        /// Tiền khách đặt cọc
        ///</summary>
        public decimal? TotalAdvance { get; set; } // TotalAdvance

        public long IsComplain { get; set; }

        public string ReasonCancel { get; set; }
        public int PercentDeposit { get; set; }
        public string CustomerCareFullName { get; set; }
    }

    public class RequestShipItem
    {
        public string PackageCode { get; set; }
    }

    public class ComplainOrderItem
    {
        public int OrderDetailId { get; set; }
        public string Note { get; set; }
        public int LinkOrder { get; set; }
    }
}