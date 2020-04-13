namespace Library.DbContext.Entities
{
    // OrderInfo
    
    public partial class OrderInfo
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Tên nghành hàng
        ///</summary>
        public string CategoryName { get; set; } // CategoryName (length: 300)

        ///<summary>
        /// Là xóa hay không
        ///</summary>
        public byte IsDelete { get; set; } // IsDelete

        public OrderInfo()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
