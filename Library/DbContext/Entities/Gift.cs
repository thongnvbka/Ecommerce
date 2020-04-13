using System;

namespace Library.DbContext.Entities
{
    // Gift
    
    public partial class Gift
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã giảm giá
        ///</summary>
        public string Code { get; set; } // Code (length: 50)

        ///<summary>
        /// Loại hình giảm giá tiền mặt hay phần trăm
        ///</summary>
        public byte DiscountType { get; set; } // DiscountType

        ///<summary>
        /// Giá trị giảm giá tiền hay %
        ///</summary>
        public decimal DiscountValue { get; set; } // DiscountValue

        ///<summary>
        /// Ngày bắt đầu có hiệu lực
        ///</summary>
        public DateTime FromDate { get; set; } // FromDate

        ///<summary>
        /// Ngày hết hiệu lực
        ///</summary>
        public DateTime ToDate { get; set; } // ToDate

        ///<summary>
        /// Nhân viên tạo mã giảm giá
        ///</summary>
        public int UserId { get; set; } // UserId

        ///<summary>
        /// Nhân viên tạo mã giảm giá
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 150)

        ///<summary>
        /// Hình thức áp dụng: Cho đơn hàng, ...
        ///</summary>
        public byte Mode { get; set; } // Mode

        ///<summary>
        /// Số lượng
        ///</summary>
        public int Quantity { get; set; } // Quantity

        ///<summary>
        /// Số lần sử dụng
        ///</summary>
        public int UseNo { get; set; } // UseNo

        ///<summary>
        /// Trạng thái mã giảm giá
        ///</summary>
        public byte Status { get; set; } // Status
        public bool IsDelete { get; set; } // IsDelete
        public DateTime Updated { get; set; } // Updated
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Tag link toàn bộ hệ thống
        ///</summary>
        public string HashTag { get; set; } // HashTag

        public Gift()
        {
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
