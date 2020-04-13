using System;

namespace Library.DbContext.Entities
{
    // Debit
    public partial class Debit
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã Code khoản phải thu
        ///</summary>
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Trạng thái các khoản phải thu (0: Chờ thu, 1: Hoàn tất)
        ///</summary>
        public byte Status { get; set; } // Status
        public string Note { get; set; } // Note

        ///<summary>
        /// Số tiền phải thu
        ///</summary>
        public decimal? MustCollectMoney { get; set; } // MustCollectMoney

        ///<summary>
        /// Số tiền phải trả
        ///</summary>
        public decimal? MustReturnMoney { get; set; } // MustReturnMoney

        ///<summary>
        /// Id quỹ tiền thực hiện thu
        ///</summary>
        public int? TreasureId { get; set; } // TreasureId
        public int? TreasureIdd { get; set; } // TreasureIdd

        ///<summary>
        /// Tên quỹ tiền thực hiện thu
        ///</summary>
        public string TreasureName { get; set; } // TreasureName (length: 300)

        ///<summary>
        /// Id loại quỹ giao dịch tiếp nhận thu tiền
        ///</summary>
        public int? FinanceFundId { get; set; } // FinanceFundId

        ///<summary>
        /// Tên loại quỹ giao dịch tiếp nhận thu tiền
        ///</summary>
        public string FinanceFundName { get; set; } // FinanceFundName (length: 500)

        ///<summary>
        /// Số tài khoản quỹ thực hiện thu công nợ
        ///</summary>
        public string FinanceFundBankAccountNumber { get; set; } // FinanceFundBankAccountNumber (length: 100)

        ///<summary>
        /// Chi nhánh văn phòng của số tài khoản ngân hàng quỹ thực hiện thu công nợ
        ///</summary>
        public string FinanceFundDepartment { get; set; } // FinanceFundDepartment (length: 300)

        ///<summary>
        /// Tên ngân hàng quỹ thực hiện thu công nợ
        ///</summary>
        public string FinanceFundNameBank { get; set; } // FinanceFundNameBank (length: 300)

        ///<summary>
        /// Tên Representative quỹ thực hiện thu công nợ
        ///</summary>
        public string FinanceFundUserFullName { get; set; } // FinanceFundUserFullName (length: 300)

        ///<summary>
        /// Số điện thoại người đứng tên tài khoản quỹ thực hiện thu công nợ
        ///</summary>
        public string FinanceFundUserPhone { get; set; } // FinanceFundUserPhone (length: 100)

        ///<summary>
        /// Email người đứng tên tài khoản quỹ thực hiện thu công nợ
        ///</summary>
        public string FinanceFundUserEmail { get; set; } // FinanceFundUserEmail (length: 300)

        ///<summary>
        /// Id loại đối tượng - Từ bảng AccountantSubject
        ///</summary>
        public int? SubjectTypeId { get; set; } // SubjectTypeId

        ///<summary>
        /// Tên loại đối tượng từ bảng AccountantSubject
        ///</summary>
        public string SubjectTypeName { get; set; } // SubjectTypeName (length: 300)

        ///<summary>
        /// Id đối tượng phải thu
        ///</summary>
        public int? AccountantSubjectId { get; set; } // AccountantSubjectId

        ///<summary>
        /// Tên đối tượng phải thu
        ///</summary>
        public string AccountantSubjectName { get; set; } // AccountantSubjectName (length: 100)

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
        /// Quy định là công nợ được tạo tự động hay thủ công (0: Thủ công, 1: Tự động)
        ///</summary>
        public bool? IsSystem { get; set; } // IsSystem
        public DateTime Created { get; set; } // Created
        public DateTime LastUpdated { get; set; } // LastUpdated

        ///<summary>
        /// Dữ liệu đã xóa
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete

        public Debit()
        {
            IsSystem = false;
            Created = DateTime.Now;
            LastUpdated = DateTime.Now;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
