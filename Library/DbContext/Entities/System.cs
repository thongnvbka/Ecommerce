namespace Library.DbContext.Entities
{
    // System
    
    public partial class System
    {
        public int Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 500)

        ///<summary>
        /// Tên miền website
        ///</summary>
        public string Domain { get; set; } // Domain (length: 500)

        ///<summary>
        /// Trạng thái 0: Không sử dụng, 1: Đang sử dụng
        ///</summary>
        public byte Status { get; set; }

        public System()
        {
            Status = 1;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
