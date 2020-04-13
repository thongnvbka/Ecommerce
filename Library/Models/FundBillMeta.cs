using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class FundBillMeta
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã Code phiếu nạp/chi tiền quỹ
        ///</summary>
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Loại giao dịch chi tiền quỹ (0: Nạp tiền quỹ, 1: Chi tiền quỹ)
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// Trạng thái loại giao dịch quỹ (0: Chờ duyệt, 1: Đã duyệt)
        ///</summary>
        public byte Status { get; set; } // Status

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
        /// Id loại đối tượng đang làm việc
        ///</summary>
        public int? AccountantSubjectId { get; set; } // AccountantSubjectId

        ///<summary>
        /// Tên loại đối tượng đang làm việc
        ///</summary>
        public string AccountantSubjectName { get; set; } // AccountantSubjectName (length: 100)

        ///<summary>
        /// Id đối tượng thực tế giao dịch đầu vào
        ///</summary>
        public int? SubjectId { get; set; } // SubjectId

        ///<summary>
        /// Code đối tượng thực tế giao dịch đầu vào
        ///</summary>
        public string SubjectCode { get; set; } // SubjectCode (length: 20)

        ///<summary>
        /// Tên đối tượng thực tế giao dịch đầu vào
        ///</summary>
        public string SubjectName { get; set; } // SubjectName (length: 300)

        ///<summary>
        /// Tên đối tượng thực tế giao dịch đầu vào
        ///</summary>
        //public string SubjectPhone { get; set; } // SubjectPhone (length: 100)
        //public string SubjectEmail { get; set; } // SubjectEmail (length: 300)

        ///<summary>
        /// Id loại quỹ giao dịch
        ///</summary>
        public int? FinanceFundId { get; set; } // FinanceFundId

        ///<summary>
        /// Tên loại quỹ giao dịch
        ///</summary>
        public string FinanceFundName { get; set; } // FinanceFundName (length: 500)
        public string FinanceFundBankAccountNumber { get; set; } // FinanceFundBankAccountNumber (length: 100)
        public string FinanceFundDepartment { get; set; } // FinanceFundDepartment (length: 300)
        public string FinanceFundNameBank { get; set; } // FinanceFundNameBank (length: 300)
        public string FinanceFundUserFullName { get; set; } // FinanceFundUserFullName (length: 300)
        public string FinanceFundUserPhone { get; set; } // FinanceFundUserPhone (length: 100)
        public string FinanceFundUserEmail { get; set; } // FinanceFundUserEmail (length: 300)

        ///<summary>
        /// Id loại định khoản sử dụng
        ///</summary>
        public int? TreasureId { get; set; } // TreasureId

        ///<summary>
        /// Tên loại định khoản
        ///</summary>
        public string TreasureName { get; set; } // TreasureName (length: 300)

        ///<summary>
        /// Ghi chú nhân viên cho loại định khoản
        ///</summary>
        public string Note { get; set; } // Note

        ///<summary>
        /// Id nhân viên tạo phiếu
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Mã Code nhân viên tạo phiếu
        ///</summary>
        public string UserCode { get; set; } // UserCode (length: 20)

        ///<summary>
        /// Tên nhân viên tạo phiếu
        ///</summary>
        public string UserName { get; set; } // UserName (length: 300)

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

        public DateTime LastUpdated { get; set; }

    }
}
