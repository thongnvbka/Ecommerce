using System;

namespace Library.DbContext.Entities
{

    // RequestShip

    public partial class RequestShip
    {
        public long Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 30)
        public int OrderId { get; set; } // OrderId
        public string OrderCode { get; set; } // OrderCode (length: 30)
        public string PackageCode { get; set; } // PackageCode (length: 500)
        public DateTime CreateDate { get; set; } // CreateDate
        public DateTime? UpdateDate { get; set; } // UpdateDate

        ///<summary>
        /// Id khách hàng
        ///</summary>
        public int CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Email khách hàng
        ///</summary>
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)

        ///<summary>
        /// Điện thoại khách hàng
        ///</summary>
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)

        ///<summary>
        /// Địa chỉ khách hàng
        ///</summary>
        public string CustomerAddress { get; set; } // CustomerAddress (length: 500)

        ///<summary>
        /// Trạng thái đơn hàng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Id hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Đơn hàng phát sinh từ hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 100)
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Ghi chú khách viết cho công ty
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        public RequestShip()
        {
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
