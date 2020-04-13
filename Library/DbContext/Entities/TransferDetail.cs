using System;

namespace Library.DbContext.Entities
{
    // TransferDetail
    
    public partial class TransferDetail
    {
        public int Id { get; set; } // Id (Primary key)
        public int TransferId { get; set; } // TransferId
        public string TransferCode { get; set; } // TransferCode (length: 30)

        ///<summary>
        /// Id kiện hàng hoặc Id của bao hàng
        ///</summary>
        public int PackageId { get; set; } // PackageId

        ///<summary>
        /// Mã kiện hàng hoặc là mã bao hàng
        ///</summary>
        public string PackageCode { get; set; } // PackageCode (length: 50)

        public int WalletId { get; set; } // WalletId

        public string WalletCode { get; set; } // WalletCode (length: 50)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public int? OrderId { get; set; } // OrderId

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 50)
        public byte OrderType { get; set; } // OrderType

        ///<summary>
        /// Ghi chú các dịch vụ trong đơn hàng: (Kiểm đếm, Đóng kiện,..)
        ///</summary>
        public string OrderServices { get; set; } // OrderServices (length: 500)
        public int? OrderPackageNo { get; set; } // OrderPackageNo

        ///<summary>
        /// Mã vận đơn
        ///</summary>
        public string TransportCode { get; set; } // TransportCode (length: 50)

        ///<summary>
        /// 1: Bình thường, 2: Hỏng vỡ
        ///</summary>
        public byte Status { get; set; } // Status
        public decimal? Weight { get; set; } // Weight

        ///<summary>
        /// Tổng cân nặng quy đổi
        ///</summary>
        public decimal? WeightConverted { get; set; } // ConvertedWeight

        ///<summary>
        /// Tổng cân nặng thực tế
        ///</summary>
        public decimal? WeightActual { get; set; } // ActualWeight
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Ghi chú đơn hàng
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        public TransferDetail()
        {
            OrderPackageNo = 0;
            Status = 1;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
