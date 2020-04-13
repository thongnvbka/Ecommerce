using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class RechargeBillMeta
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã Code phiếu giao dịch ví điện tử khách hàng
        ///</summary>
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Loại giao dịch ví điện tử (0: Nạp tiền ví, 1: Rút tiền ví)
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// Trạng thái loại giao dịch quỹ (0: Chờ duyệt, 1: Đã duyệt)
        ///</summary>
        public byte Status { get; set; } // Status
        public string Note { get; set; } // Note

        ///<summary>
        /// Số dư biến động
        ///</summary>
        public decimal CurrencyFluctuations { get; set; } // CurrencyFluctuations

        ///<summary>
        /// Phát sinh tăng
        ///</summary>
        public decimal? Increase { get; set; } // Increase

        ///<summary>
        /// Phát sinh giảm
        ///</summary>
        public decimal? Diminishe { get; set; } // Diminishe

        ///<summary>
        /// Số dư trước giao dịch
        ///</summary>
        public decimal? CurencyStart { get; set; } // CurencyStart

        ///<summary>
        /// Số dư sau giao dịch
        ///</summary>
        public decimal? CurencyEnd { get; set; } // CurencyEnd

        ///<summary>
        /// Id nhân viên thực hiện tạo phiếu
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Mã Code nhân viên thực hiện tạo phiếu
        ///</summary>
        public string UserCode { get; set; } // UserCode (length: 20)

        ///<summary>
        /// Tên nhân viên thực hiện tạo phiếu
        ///</summary>
        public string UserName { get; set; } // UserName (length: 100)

        ///<summary>
        /// Id người duyệt phiếu
        ///</summary>
        public int? UserApprovalId { get; set; } // UserApprovalId

        ///<summary>
        /// Mã Code người duyệt phiếu
        ///</summary>
        public string UserApprovalCode { get; set; } // UserApprovalCode (length: 20)

        ///<summary>
        /// Tên người duyệt phiếu
        ///</summary>
        public string UserApprovalName { get; set; } // UserApprovalName (length: 300)

        ///<summary>
        /// Id khách hàng có ví điện tử
        ///</summary>
        public int? CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Mã Code khách hàng có ví điện tử
        ///</summary>
        public string CustomerCode { get; set; } // CustomerCode (length: 20)

        ///<summary>
        /// Tên khách hàng có ví điện tử
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 100)
        public string CustomerPhone { get; set; } // CustomerPhone (length: 100)
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)
        public string CustomerAddress { get; set; } // CustomerAddress (length: 300)

        ///<summary>
        /// Id định khoản trong bảng định khoản ví điện tử khách hàng
        ///</summary>
        public int? TreasureId { get; set; } // TreasureId

        ///<summary>
        /// Tên định khoản ví điện tử
        ///</summary>
        public string TreasureName { get; set; } // TreasureName (length: 100)

        ///<summary>
        /// Idd định khoản từ bảng định khoản ví điện tử của khách hàng
        ///</summary>
        public int? TreasureIdd { get; set; } // TreasureIdd

        ///<summary>
        /// Quy định phiếu được tạo tự động hay bằng tay (0: Tự động; 1: Bằng tay)
        ///</summary>
        public bool? IsAutomatic { get; set; } // IsAutomatic

        ///<summary>
        /// Id của đơn hàng có liên quan tới giao dịch ví
        ///</summary>
        public int? OrderId { get; set; } // OrderId

        ///<summary>
        /// Mã Code đơn hàng có liên quan tới giao dịch ví
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 20)

        ///<summary>
        /// Kiểu đơn hàng có liên quan tới giao dịch ví (Theo Type của đơn hàng)
        ///</summary>
        public byte? OrderType { get; set; } // OrderType
    }

    public class AutoProcessRechargeBillModel
    {
        ///<summary>
        /// Id khách hàng có ví điện tử
        ///</summary>
        public int CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Idd tự động định khoản trong bảng định khoản ví điện tử khách hàng
        ///</summary>
        public int TreasureIdd { get; set; } // TreasureId

        ///<summary>
        /// Số dư biến động
        ///</summary>
        public decimal CurrencyFluctuations { get; set; } // CurrencyFluctuations

        ///<summary>
        /// Id của đơn hàng có liên quan tới giao dịch ví
        ///</summary>
        public int? OrderId { get; set; } // OrderId

        ///<summary>
        /// Ghi chú khi lưu thông tin
        ///</summary>
        public string Note { get; set; } // OrderId

    }


}
