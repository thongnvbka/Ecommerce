using System;

namespace Library.DbContext.Entities
{
    // Transaction
    
    public partial class Transaction
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Tiên trong giao dịch VNĐ
        ///</summary>
        public decimal Value { get; set; } // Value

        ///<summary>
        /// Loại giao dịch, Thanh toán, Đặt cọc, Hoàn tiền,..
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// WithDraw
        ///</summary>
        public byte Mode { get; set; } // Mode

        ///<summary>
        /// Id nhân viên xử lý
        ///</summary>
        public int UserId { get; set; } // UserId

        ///<summary>
        /// Tên đầy đủ của nhân viên xử lý
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 150)

        ///<summary>
        /// Thời gian thực hiện giao dịch
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Id khách hàng
        ///</summary>
        public int CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Khách hàng thuộc hệ thống
        ///</summary>
        public byte SystemId { get; set; } // SystemId
        public string SystemName { get; set; } // SystemName (length: 300)

        ///<summary>
        /// Hình thức giao dịch Chuyển khoản hay tiền mặt
        ///</summary>
        public byte Method { get; set; } // Method

        ///<summary>
        /// Ghi chú cho giao dịch
        ///</summary>
        public string Description { get; set; } // Description (length: 600)

        public Transaction()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
