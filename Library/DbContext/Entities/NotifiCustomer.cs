using System;

namespace Library.DbContext.Entities
{

    // NotifiCustomer

    public partial class NotifiCustomer
    {

        ///<summary>
        /// Id bảng map giữa
        ///</summary>
        public long Id { get; set; } // Id (Primary key)
        ///<summary>
        /// Id hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Tên hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 200)
        ///<summary>
        /// Id bảng thông tin chung
        ///</summary>
        public long? NotiCommonId { get; set; } // NotiCommonId

        ///<summary>
        /// Mã khách hàng
        ///</summary>
        public int? CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 90)

        ///<summary>
        /// Đã đọc hay chưa
        ///</summary>
        public bool IsRead { get; set; } // IsRead

        public NotifiCustomer()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
