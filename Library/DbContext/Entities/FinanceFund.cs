namespace Library.DbContext.Entities
{

    // FinanceFund

    public partial class FinanceFund
    {

        ///<summary>
        /// Mã quỹ
        ///</summary>
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Đường dẫn
        ///</summary>
        public string IdPath { get; set; } // IdPath (length: 300)

        ///<summary>
        /// Tên đường dẫn
        ///</summary>
        public string NamePath { get; set; } // NamePath (length: 500)

        ///<summary>
        /// Tên quỹ
        ///</summary>
        public string Name { get; set; } // Name (length: 500)

        ///<summary>
        /// Mã cha, mặc định cha ngoài cùng có mã:0
        ///</summary>
        public int ParentId { get; set; } // ParentId

        ///<summary>
        /// Tên cha, mặc định là rỗng ""
        ///</summary>
        public string ParentName { get; set; } // ParentName (length: 300)

        ///<summary>
        /// Trạng thái, 0: hoạt động, 1:tạm ngưng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Mô tả
        ///</summary>
        public string Description { get; set; } // Description (length: 500)

        ///<summary>
        /// 0: chưa xóa. 1: đã xóa
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Mã người dùng
        ///</summary>
        public int UserId { get; set; } // UserId

        ///<summary>
        /// Mã code người dùng
        ///</summary>
        public string UserCode { get; set; } // UserCode (length: 100)

        ///<summary>
        /// Tên người dùng
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 100)

        ///<summary>
        /// Địa chỉ mail người dùng
        ///</summary>
        public string UserEmail { get; set; } // UserEmail (length: 50)

        ///<summary>
        /// Số điện thoại người dùng
        ///</summary>
        public string UserPhone { get; set; } // UserPhone (length: 20)

        ///<summary>
        /// Tên ngân hàng
        ///</summary>
        public string NameBank { get; set; } // NameBank (length: 50)

        ///<summary>
        /// Chi nhánh ngân hàng
        ///</summary>
        public string Department { get; set; } // Department (length: 600)

        ///<summary>
        /// Số tài khoản ngân hàng
        ///</summary>
        public string BankAccountNumber { get; set; } // BankAccountNumber (length: 50)

        ///<summary>
        /// So tien trong quy
        ///</summary>
        public decimal Balance { get; set; } // Balance

        /// <summary>
        /// True là cha, fale là con 
        /// </summary>
        public bool IsParent { get; set; }
        ///<summary>
        /// Tên chủ tài khoản ngân hàng
        ///</summary>
        public string CardName { get; set; } // CardName (length: 50)

        ///<summary>
        /// Số tài khoản
        ///</summary>
        public string CardId { get; set; } // CardId (length: 20)

        ///<summary>
        /// Ngân hàng
        ///</summary>
        public string CardBank { get; set; } // CardBank (length: 255)
         ///<summary>
        /// Chi nhánh ngân hàng
        ///</summary>
        public string CardBranch { get; set; } // CardBranch (length: 255)
        public string Maxlength { get; set; }

        ///<summary>
        /// Đơn vị tiền tệ: VND, USD,...
        ///</summary>
        public string Currency { get; set; } // Currency (length: 50)
        public FinanceFund()
        {
            Name = "getdate()";
            Status = 0;
            IsDelete = false;
            Balance = 0m;
            IsParent = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
