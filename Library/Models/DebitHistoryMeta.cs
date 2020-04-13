using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Library.Models
{
    public class DebitHistoryMeta
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã Code lịch sử công nợ
        ///</summary>
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Id công nợ tham chiếu
        ///</summary>
        public int? DebitId { get; set; } // DebitId
        public byte Status { get; set; } // Status
        public string Note { get; set; } // Note
        ///<summary>
        /// Loại công nợ tham chiếu
        ///</summary>
        public byte? DebitType { get; set; } // DebitType

        ///<summary>
        /// Mã Code công nợ tham chiếu
        ///</summary>
        public string DebitCode { get; set; } // DebitCode (length: 20)


        public decimal? Money { get; set; } // Money

        ///<summary>
        /// Id của đơn hàng
        ///</summary>
        public int? OrderId { get; set; } // OrderId

        ///<summary>
        /// Loại đơn hàng
        ///</summary>
        public byte? OrderType { get; set; } // OrderType

        ///<summary>
        /// Code của đơn hàng
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 20)


        ///<summary>
        /// Id định khoản công nợ
        ///</summary>
        [Required(ErrorMessage = "Placement is required to select")]
        public int? PayReceivableId { get; set; } // PayReceivableId

        ///<summary>
        /// Idd định khoản công nợ
        ///</summary>
        public int? PayReceivableIdd { get; set; } // PayReceivableIdd

        ///<summary>
        /// Tên định khoản công nợ
        ///</summary>
        public string PayReceivableName { get; set; } // PayReceivableName

        ///<summary>
        /// Id đối tượng phải thu (Khách hàng, nhân viên, shop bán hàng, đối tác vận chuyển...)
        ///</summary>
        public int? SubjectId { get; set; } // SubjectId

        ///<summary>
        /// Mã Code đối tượng phải thu
        ///</summary>
        public string SubjectCode { get; set; } // SubjectCode (length: 20)

        ///<summary>
        /// Tên đối tượng phải thu (Tên khách hàng, tên Shop,...)
        ///</summary>
        public string SubjectName { get; set; } // SubjectName (length: 300)
        public string SubjectPhone { get; set; } // SubjectPhone (length: 100)
        public string SubjectEmail { get; set; } // SubjectEmail (length: 300)
        public string SubjectAddress { get; set; } // SubjectAddress (length: 500)

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

    }


    public class AutoUpdateDebitModel
    {
        ///<summary>
        /// Id đối tượng phát sinh công nợ
        ///</summary>
        public int SubjectId { get; set; } // SubjectId

        ///<summary>
        /// Idd đối tượng phát sinh công nợ
        ///</summary>
        public int? SubjectTypeIdd { get; set; } // SubjectTypeIdd

        ///<summary>
        /// Idd tự động định khoản trong bảng định khoản ví điện tử khách hàng
        ///</summary>
        public int PayReceivableIdd { get; set; } // PayReceivableIdd

        ///<summary>
        /// Số tiền cần cập nhật công nợ
        ///</summary>
        public decimal Money { get; set; } // Money

        ///<summary>
        /// Id của đơn hàng có liên quan tới giao dịch ví
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Loại đơn hàng
        ///</summary>
        public byte? OrderType { get; set; } // OrderType

        ///<summary>
        /// Code của đơn hàng
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 20)

        ///<summary>
        /// Ghi chú khi tạo phiếu công nợ
        ///</summary>
        public string Note { get; set; } // Note


    }

    public class ManualUpdateDebitModel
    {
        ///<summary>
        /// Id đối tượng phát sinh công nợ
        ///</summary>
        public int SubjectId { get; set; } // SubjectId

        ///<summary>
        /// Code đối tượng phát sinh công nợ
        ///</summary>
        public string SubjectCode { get; set; } // SubjectCode

        ///<summary>
        /// SubjectEmail đối tượng phát sinh công nợ
        ///</summary>
        public string SubjectEmail { get; set; } // SubjectEmail

        ///<summary>
        /// Id tự động định khoản trong bảng định khoản ví điện tử khách hàng
        ///</summary>
        public int PayReceivableId { get; set; } // PayReceivableId

        ///<summary>
        /// Số tiền cần cập nhật công nợ
        ///</summary>
        public decimal Money { get; set; } // Money

        ///<summary>
        /// Note ghi chú công nợ
        ///</summary>
        public string Note { get; set; } // Note (length: 20)
        public byte Status { get; set; } // Status

    }


    public class AutoDebitHistoryModel
    {
        ///<summary>
        /// Id công nợ tham chiếu
        ///</summary>
        public int? DebitId { get; set; } // DebitId

        ///<summary>
        /// Loại công nợ tham chiếu
        ///</summary>
        public byte? DebitType { get; set; } // DebitType

        ///<summary>
        /// Mã Code công nợ tham chiếu
        ///</summary>
        public string DebitCode { get; set; } // DebitCode (length: 20)

        ///<summary>
        /// Id đối tượng phát sinh công nợ
        ///</summary>
        public int SubjectId { get; set; } // SubjectId

        ///<summary>
        /// Id định khoản công nợ
        ///</summary>
        public int? PayReceivableId { get; set; } // PayReceivableId

        ///<summary>
        /// Idd tự động định khoản trong bảng định khoản ví điện tử khách hàng
        ///</summary>
        public int PayReceivableIdd { get; set; } // PayReceivableIdd

        ///<summary>
        /// Tên định khoản công nợ
        ///</summary>
        public string PayReceivableName { get; set; } // PayReceivableName

        ///<summary>
        /// Số tiền cần cập nhật công nợ
        ///</summary>
        public decimal? Money { get; set; } // Money

        ///<summary>
        /// Id của đơn hàng có liên quan tới giao dịch ví
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Loại đơn hàng
        ///</summary>
        public byte? OrderType { get; set; } // OrderType

        ///<summary>
        /// Code của đơn hàng
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 20)

    }
}
