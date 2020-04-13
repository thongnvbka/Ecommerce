using System;

namespace Library.DbContext.Entities
{
    // CustomerLevel

    public partial class CustomerLevel
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Tên quỹ
        ///</summary>
        public string Name { get; set; } // Name (length: 255)

        ///<summary>
        /// Mô tả
        ///</summary>
        public string Description { get; set; } // Description (length: 500)

        ///<summary>
        /// Trạng thái
        ///</summary>
        public bool Status { get; set; } // Status

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime? CreateDate { get; set; } // CreateDate

        ///<summary>
        /// Ngày cập nhật
        ///</summary>
        public DateTime? UpdateDate { get; set; } // UpdateDate

        ///<summary>
        /// Người cập nhật cuối
        ///</summary>
        public string UserName { get; set; } // UserName (length: 50)

        ///<summary>
        /// Trạng thái của bản ghi
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete
        public decimal StartMoney { get; set; } // StartMoney
        public decimal EndMoney { get; set; } // EndMoney
        public int PercentDeposit { get; set; } // PercentDeposit
        public byte Order { get; set; }
        public byte Ship { get; set; }
        public CustomerLevel()
        {
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }
}