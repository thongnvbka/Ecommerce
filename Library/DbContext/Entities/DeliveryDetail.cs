using System;

namespace Library.DbContext.Entities
{
    // DeliveryDetail

    public partial class DeliveryDetail
    {
        public int Id { get; set; } // Id (Primary key)
        public int DeliveryId { get; set; } // DeliveryId
        public string DeliveryCode { get; set; } // DeliveryCode (length: 30)

        ///<summary>
        /// Id kiện hàng hoặc Id của bao hàng
        ///</summary>
        public int PackageId { get; set; } // PackageId

        ///<summary>
        /// Mã kiện hàng hoặc là mã bao hàng
        ///</summary>
        public string PackageCode { get; set; } // packageCode (length: 50)
        public int OrderPackageNo { get; set; } // OrderPackageNo

        ///<summary>
        /// Ghi chú các dịch vụ trong đơn hàng: (Kiểm đếm, Đóng kiện,..)
        ///</summary>
        public string OrderServices { get; set; } // OrderServices (length: 500)

        ///<summary>
        /// Ghi chú đơn hàng
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 50)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public int OrderId { get; set; } // OrderId
        public byte OrderType { get; set; } // OrderType

        ///<summary>
        /// Mã vận đơn
        ///</summary>
        public string TransportCode { get; set; } // TransportCode (length: 50)

        ///<summary>
        /// 1: Bình thường, 2: Hỏng vỡ
        ///</summary>
        public byte Status { get; set; } // Status
        public string WarehouseIdPath { get; set; } // WarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho nhập hàng
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 300)
        public string WarehouseAddress { get; set; } // WarehouseAddress (length: 300)
        public int WarehouseId { get; set; } // WarehouseId
        public bool IsDelete { get; set; } // IsDelete
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated

        ///<summary>
        /// Cân nặng thực tế
        ///</summary>
        public decimal? Weight { get; set; } // Weight

        ///<summary>
        /// Cân nặng chuyển đổi
        ///</summary>
        public decimal? WeightConverted { get; set; } // WeightConverted

        ///<summary>
        /// Cân nặng tính tiền
        ///</summary>
        public decimal? WeightActual { get; set; } // WeightActual

        ///<summary>
        /// Cước cân
        ///</summary>
        public decimal? PriceWeight { get; set; } // PriceWeight

        ///<summary>
        /// Giá vận chuyển / 1kg
        ///</summary>
        public decimal? Price { get; set; } // Price

        ///<summary>
        /// Tiền đóng kiện gỗ
        ///</summary>
        public decimal? PricePacking { get; set; } // PricePacking

        ///<summary>
        /// Tiền phát sinh
        ///</summary>
        public decimal? PriceOther { get; set; } // PriceOther

        ///<summary>
        /// Tiền lưu kho
        ///</summary>
        public decimal? PriceStored { get; set; } // PriceStored

        ///<summary>
        /// Tiền hoàn thành đơn của phiếu xuất
        ///</summary>
        public decimal? PriceOrder { get; set; } // PriceOrder
        public decimal? PricePayed { get; set; } // PriceOrder
        public decimal? Debit { get; set; } // PriceOrder
        public decimal PriceShip { get; set; } // PriceShip

        ///<summary>
        /// Id Layout
        ///</summary>
        public int? LayoutId { get; set; } // LayoutId

        ///<summary>
        /// Tên Layout
        ///</summary>
        public string LayoutName { get; set; } // LayoutName (length: 300)

        ///<summary>
        /// Id bao hàng
        ///</summary>
        public int? WalletId { get; set; } // WalletId

        ///<summary>
        /// Mã bao hàng
        ///</summary>
        public string WalletCode { get; set; } // WalletCode (length: 50)

        ///<summary>
        /// % Triết khấu phí vận chuyển hàng về VN
        ///</summary>
        public decimal? ShipDiscount { get; set; }

        public DeliveryDetail()
        {
            OrderPackageNo = 0;
            OrderType = 0;
            Status = 1;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }
}
