using System;

namespace Library.DbContext.Entities
{
    // PutAwayDetail

    public partial class PutAwayDetail
    {
        public int Id { get; set; } // Id (Primary key)
        public int PutAwayId { get; set; } // PutAwayId
        public string PutAwayCode { get; set; } // PutAwayCode (length: 30)

        ///<summary>
        /// Id kiện hàng hoặc Id của bao hàng
        ///</summary>
        public int PackageId { get; set; } // PackageId

        ///<summary>
        /// Id kiện hàng hoặc Id của bao hàng
        ///</summary>
        public string PackageCode { get; set; } // PackageCode (length: 50)

        ///<summary>
        /// Mã vận đơn
        ///</summary>
        public string TransportCode { get; set; } // TransportCode (length: 50)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 50)
        public byte OrderType { get; set; } // OrderType

        ///<summary>
        /// Ghi chú các dịch vụ trong đơn hàng: (Kiểm đếm, Đóng kiện,..)
        ///</summary>
        public string OrderServices { get; set; } // OrderServices (length: 500)
        public int OrderPackageNo { get; set; } // OrderPackageNo

        ///<summary>
        /// Id khách hàng
        ///</summary>
        public int CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Tài khoản khách hàng
        ///</summary>
        public string CustomerUserName { get; set; } // CustomerUserName (length: 300)

        ///<summary>
        /// Ghi chú đơn hàng
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// 1: Bình thường, 2: Hỏng vỡ
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Tổng cân nặng quy đổi
        ///</summary>
        public decimal Length { get; set; } // Length
        public decimal Weight { get; set; } // Weight

        ///<summary>
        /// Tổng cân nặng quy đổi
        ///</summary>
        public decimal Width { get; set; } // Width

        ///<summary>
        /// Chi tiết kích thước kiện hàng: Rộng x Dài x Cao
        ///</summary>
        public string Size { get; set; } // Size (length: 500)

        ///<summary>
        /// Tổng cân nặng quy đổi
        ///</summary>
        public decimal Height { get; set; } // Height

        ///<summary>
        /// Kho hàng
        ///</summary>
        public int? LayoutId { get; set; } // LayoutId

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string LayoutName { get; set; } // LayoutName (length: 300)
        public string LayoutIdPath { get; set; } // LayoutIdPath (length: 300)
        public string LayoutNamePath { get; set; } // LayoutNamePath

        ///<summary>
        /// Tổng cân nặng quy đổi
        ///</summary>
        public decimal ConvertedWeight { get; set; } // ConvertedWeight

        ///<summary>
        /// Tổng cân nặng thực tế
        ///</summary>
        public decimal ActualWeight { get; set; } // ActualWeight
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated
        public bool IsDelete { get; set; } // IsDelete

        public PutAwayDetail()
        {
            PackageCode = "";
            OrderPackageNo = 0;
            Status = 1;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
