namespace Library.DbContext.Entities
{
    public partial class CustomerConfigLevel
    {
        public int Id { get; set; } // ID (Primary key)

        ///<summary>
        /// Tên level khách hàng
        ///</summary>
        public string CustomerConfigName { get; set; } // CustomerConfigName (length: 100)

        ///<summary>
        /// Mức doanh thu
        ///</summary>
        public string TurnoverRate { get; set; } // TurnoverRate (length: 300)

        ///<summary>
        /// Miêu tả
        ///</summary>
        public string Description { get; set; } // Description

        ///<summary>
        /// Trạng thái level khách hàng 0 đã xóa 1 đang tồn tại
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete

        public CustomerConfigLevel()
        {
            IsDelete = true;
            InitializePartial();
        }

        partial void InitializePartial();
    }
}