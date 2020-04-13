using System;
namespace Library.DbContext.Entities
{
    // DebitHistory
    public partial class DebitHistory
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

        ///<summary>
        /// Loại công nợ (0: Phải thu; 1: Phải trả)
        ///</summary>
        public byte? DebitType { get; set; } // DebitType

        ///<summary>
        /// Mã Code công nợ tham chiếu
        ///</summary>
        public string DebitCode { get; set; } // DebitCode (length: 20)

        ///<summary>
        /// Trạng thái các khoản phải thu (0: Chưa hoàn tất, 1: Hoàn tất)
        ///</summary>
        public byte Status { get; set; } // Status
        public string Note { get; set; } // Note
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
        public int? PayReceivableId { get; set; } // PayReceivableId

        ///<summary>
        /// Idd định khoản công nợ
        ///</summary>
        public int? PayReceivableIdd { get; set; } // PayReceivableIdd

        ///<summary>
        /// Tên định khoản công nợ
        ///</summary>
        public string PayReceivableIName { get; set; } // PayReceivableIName (length: 300)

        ///<summary>
        /// Quy định là công nợ được tạo tự động hay thủ công (0: Thủ công, 1: Tự động)
        ///</summary>
        public bool IsSystem { get; set; } // IsSystem

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
        public DateTime Created { get; set; } // Created
        public DateTime LastUpdated { get; set; } // LastUpdated

        public DebitHistory()
        {
            IsSystem = false;
            Created = DateTime.Now;
            LastUpdated = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
