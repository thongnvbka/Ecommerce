using System;

namespace Library.DbContext.Entities
{
    // OrderRefund
    
    public partial class OrderRefund
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã hoàn
        ///</summary>
        public string Code { get; set; } // Code (length: 50)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Số link trong phiếu hoàn
        ///</summary>
        public int LinkNo { get; set; } // LinkNo

        ///<summary>
        /// Số sản phẩm trong phiếu hoàn
        ///</summary>
        public int ProductNo { get; set; } // ProductNo

        ///<summary>
        /// Text cho tìm kiếm
        ///</summary>
        public string UnsignText { get; set; } // UnsignText

        ///<summary>
        /// Trạng thái phiếu hoàn 0: Mới tạo, 1: Đã đóng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Loại phiếu hoàn: 0- Hoàn tiền, 1- Đổi trả hàng hóa
        ///</summary>
        public byte Mode { get; set; } // Mode

        ///<summary>
        /// Kho ghi chú cho phiếu hoàn tiền
        ///</summary>
        public string Note { get; set; } // Note

        ///<summary>
        /// Người tạo phếu hoàn (Đặt hàng - với phiếu hoàn tiền cho khách, Kho - với phiếu đổi trả sản phẩm)
        ///</summary>
        public int? CreateUserId { get; set; } // CreateUserId
        public string CreateUserFullName { get; set; } // CreateUserFullName (length: 300)
        public string CreateUserName { get; set; } // CreateUserName (length: 50)
        public int? CreateOfficeId { get; set; } // CreateOfficeId
        public string CreateOfficeName { get; set; } // CreateOfficeName (length: 300)
        public string CreateOfficeIdPath { get; set; } // CreateOfficeIdPath (length: 300)

        ///<summary>
        /// Người đóng phiếu hoàn (Đặt hàng - với phiếu đổi trả sản phẩm, Kế toán - với phiếu hoàn tiền cho khách)
        ///</summary>
        public int? UpdateUserId { get; set; } // UpdateUserId
        public string UpdateUserFullName { get; set; } // UpdateUserFullName (length: 300)
        public string UpdateUserName { get; set; } // UpdateUserName (length: 50)
        public int? UpdateOfficeId { get; set; } // UpdateOfficeId
        public string UpdateOfficeName { get; set; } // UpdateOfficeName (length: 300)
        public string UpdateOfficeIdPath { get; set; } // UpdateOfficeIdPath (length: 300)
        public int CommentNo { get; set; } // CommentNo

        ///<summary>
        /// Số tiền hoàn cho khách - với phiếu hoàn tiền. Số tiền vận chuyển - với phiếu đổi trả
        ///</summary>
        public decimal Amount { get; set; } // Amount

        ///<summary>
        /// Số tiền kế toán nhận được của Shop
        ///</summary>
        public decimal AmountActual { get; set; } // AmountActual

        ///<summary>
        /// Tổng tiền thực tế tính từ số lượng đổi trả, hoàn tiền
        ///</summary>
        public decimal TotalAcount { get; set; } // TotalAcount

        ///<summary>
        /// % tiền hoàn so với thực tế, % tiền vận chuyển so với tiền hàng
        ///</summary>
        public decimal Percent { get; set; } // Percent
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated

        public bool IsDelete { get; set; }

        public OrderRefund()
        {
            IsDelete = false;
            Mode = 0;
            CommentNo = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
