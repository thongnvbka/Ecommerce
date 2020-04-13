namespace Library.DbContext.Entities
{
    // CustomerType

    public partial class CustomerType
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mô tả cho nhóm khách hàng
        ///</summary>
        public string Description { get; set; } // Description (length: 600)

        ///<summary>
        /// Tên của nhóm khách hàng
        ///</summary>
        public string NameType { get; set; } // NameType (length: 300)

        ///<summary>
        /// 0:chưa xóa; 1: đã xóa
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// 0: đang mở; 1: đã mở
        ///</summary>
        public byte Status { get; set; } // Status

        public CustomerType()
        {
            IsDelete = false;
            Status = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
